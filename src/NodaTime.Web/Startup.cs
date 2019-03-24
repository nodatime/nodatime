// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

#if BLAZOR
using Microsoft.AspNetCore.Blazor.Server;
using Microsoft.AspNetCore.ResponseCompression;
#endif
using Google.Api.Gax;
using Google.Cloud.Diagnostics.AspNetCore;
using Google.Cloud.Logging.V2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using NodaTime.Web.Configuration;
using NodaTime.Web.Controllers;
using NodaTime.Web.Models;
using NodaTime.Web.Providers;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;

namespace NodaTime.Web
{
    public class Startup
    {
        internal const string SmokeTestEnvironment = "SmokeTests";

        private static readonly MediaTypeHeaderValue TextHtml = new MediaTypeHeaderValue("text/html");

        // Not const to avoid unreachable code warnings.
        private static readonly bool UseGoogleCloudStorage = Environment.GetEnvironmentVariable("DISABLE_GCS") == null;

        private IConfigurationRoot Configuration { get; set; }
        private IHostingEnvironment CurrentEnvironment { get; set; }
        public StackdriverOptions StackdriverOptions { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            CurrentEnvironment = env;
            StackdriverOptions = Configuration.GetSection("Stackdriver").Get<StackdriverOptions>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // TODO: Put this into appsettings.json.
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownNetworks.Clear();
                // Google Cloud Platform load balancers
                options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("130.211.0.0"), 22));
                options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("35.191.0.0"), 16));
                // GKE service which proxies the request as well.
                options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("10.0.0.0"), 8));
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.ForwardLimit = 2;
            });

            services.AddHttpsRedirection(options =>
            {
                // TODO: Put this into appsettings.json?
                // (When hosted on GKE, we don't locally host HTTPS, so we need to specify the port.
                // But in development, we need to stick with 5001.)            
                if (!CurrentEnvironment.IsDevelopment() && CurrentEnvironment.EnvironmentName != SmokeTestEnvironment)
                {
                    options.HttpsPort = 443;
                }
                options.RedirectStatusCode = 301;
            });

            // TODO: Add actual health checks, maybe.
            services.AddHealthChecks();

#if BLAZOR
            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });
#endif

            if (StackdriverOptions.UseStackdriverErrorReporting)
            {
                var platform = Platform.Instance();
                services.AddGoogleExceptionLogging(options =>
                {
                    options.ServiceName = StackdriverOptions.ServiceName ?? platform.GkeDetails?.ContainerName ?? "aspnetcore";
                    options.Version = StackdriverOptions.ServiceVersion ?? CurrentEnvironment.EnvironmentName;
                });
            }

            if (UseGoogleCloudStorage)
            {
                // Eagerly fetch the GoogleCredential so that we're not using Task.Result in
                // request processing.
                services.AddSingleton(GoogleCredentialProvider.FetchCredential(Configuration));
                services.AddSingleton<IReleaseRepository, GoogleStorageReleaseRepository>();
                services.AddSingleton<ITzdbRepository, GoogleStorageTzdbRepository>();
                services.AddSingleton<IBenchmarkRepository, GoogleStorageBenchmarkRepository>();
            }
            else
            {
                services.AddSingleton<IReleaseRepository, FakeReleaseRepository>();
                services.AddSingleton<ITzdbRepository, FakeTzdbRepository>();
                services.AddSingleton<IBenchmarkRepository, LocalBenchmarkRepository>();
            }
            services.AddSingleton<MarkdownLoader>();
            services.AddMemoryCache();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
#pragma warning disable CS0618 // Type or member is obsolete - loggerFactory.AddConsole. We need to work out how to do all this better.
            if (StackdriverOptions.UseStackdriverLogging)
            {
                var platform = Platform.Instance();
                var loggerOptions = LoggerOptions.Create(
                    logLevel: StackdriverOptions.LogLevel,
                    logName: StackdriverOptions.LogId ?? platform.GkeDetails?.ContainerName ?? "aspnetcore");
                loggerFactory.AddGoogle(app.ApplicationServices, platform.ProjectId, loggerOptions);
                if (StackdriverOptions.IncludeConsoleLogging)
                {
                    loggerFactory.AddConsole();
                }
            }
            else
            {
                // If we're not logging to Stackdriver, use console logging instead.
                loggerFactory.AddConsole();
                loggerFactory.AddDebug();
#pragma warning restore CS0618 // Type or member is obsolete
            }

            if (StackdriverOptions.UseStackdriverErrorReporting)
            {
                app.UseGoogleExceptionLogging();
            }

            app.UseForwardedHeaders();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            // Note: health checks come before HTTPS redirection so we get a 200 even on HTTP.
            app.UseHealthChecks("/healthz");
            app.UseHttpsRedirection();

            app.UseDefaultFiles();
            // Default content, e.g. CSS.
            // Even though we don't normally host the nzd files locally, it's useful to be able
            // to in case of emergency.
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = new FileExtensionContentTypeProvider
                {
                    Mappings = { [".nzd"] = "application/octet-stream" }
                },
                OnPrepareResponse = context => SetCacheControlHeaderForStaticContent(env, context.Context)
            });

            // API documentation
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "docfx")),
                ContentTypeProvider = new FileExtensionContentTypeProvider
                {
                    Mappings = { [".yml"] = "text/x-yaml" }
                },
                OnPrepareResponse = context => SetCacheControlHeaderForStaticContent(env, context.Context)
            });
            // Captures "unstable" or a specific version - used several times below.
            string anyVersion = @"((?:1\.[0-4]\.x)|(?:unstable)|(?:2\.[0-4]\.x))";
            var rewriteOptions = new RewriteOptions()
                // Docfx wants index.html to exist, which is annoying... just redirect.
                .AddRedirect($@"^index.html$", "/")
                // We don't have an index.html or equivalent for the APIs, so let's go to NodaTime.html
                .AddRedirect($@"^{anyVersion}/api/?$", "$1/api/NodaTime.html")
                // Compatibility with old links
                .AddRedirect($@"^{anyVersion}/userguide/([^.]+)\.html$", "$1/userguide/$2")
                .AddRedirect($@"^developer/([^.]+)\.html$", "developer/$1")
                // Avoid links from userguide/unstable from going to userguide/core-concepts etc
                // (There are no doubt better ways of doing this...)
                .AddRedirect($@"^{anyVersion}/userguide$", "$1/userguide/")
                .AddRedirect($@"^developer$", "developer/")
                // Make /api and /userguide links to the latest stable release.
                .AddRedirect("^(api|userguide)(/.*)?$", "2.4.x/$1$2");
            app.UseRewriter(rewriteOptions);

            // At some stage we may want an MVC view for the home page, but at the moment
            // we're just serving static files, so we don't need much.
            app.UseMvc(routes =>
            {
                // TODO: Find a better way of routing. This is pretty nasty.
                routes.MapRoute("Developer docs", "developer/{*url}", new { controller = "Documentation", bundle = "developer", action = nameof(DocumentationController.ViewDocumentation) });
                routes.MapRoute("1.0.x user guide", "1.0.x/userguide/{*url}", new { controller = "Documentation", bundle = "1.0.x", action = nameof(DocumentationController.ViewDocumentation) });
                routes.MapRoute("1.1.x user guide", "1.1.x/userguide/{*url}", new { controller = "Documentation", bundle = "1.1.x", action = nameof(DocumentationController.ViewDocumentation) });
                routes.MapRoute("1.2.x user guide", "1.2.x/userguide/{*url}", new { controller = "Documentation", bundle = "1.2.x", action = nameof(DocumentationController.ViewDocumentation) });
                routes.MapRoute("1.3.x user guide", "1.3.x/userguide/{*url}", new { controller = "Documentation", bundle = "1.3.x", action = nameof(DocumentationController.ViewDocumentation) });
                routes.MapRoute("1.4.x user guide", "1.4.x/userguide/{*url}", new { controller = "Documentation", bundle = "1.4.x", action = nameof(DocumentationController.ViewDocumentation) });
                routes.MapRoute("2.0.x user guide", "2.0.x/userguide/{*url}", new { controller = "Documentation", bundle = "2.0.x", action = nameof(DocumentationController.ViewDocumentation) });
                routes.MapRoute("2.1.x user guide", "2.1.x/userguide/{*url}", new { controller = "Documentation", bundle = "2.1.x", action = nameof(DocumentationController.ViewDocumentation) });
                routes.MapRoute("2.2.x user guide", "2.2.x/userguide/{*url}", new { controller = "Documentation", bundle = "2.2.x", action = nameof(DocumentationController.ViewDocumentation) });
                routes.MapRoute("2.3.x user guide", "2.3.x/userguide/{*url}", new { controller = "Documentation", bundle = "2.3.x", action = nameof(DocumentationController.ViewDocumentation) });
                routes.MapRoute("2.4.x user guide", "2.4.x/userguide/{*url}", new { controller = "Documentation", bundle = "2.4.x", action = nameof(DocumentationController.ViewDocumentation) });
                routes.MapRoute("Unstable user guide", "unstable/userguide/{*url}", new { controller = "Documentation", bundle = "unstable", action = nameof(DocumentationController.ViewDocumentation) });
                routes.MapRoute("default", "{action=Index}/{id?}", new { controller = "Home" });
            });

            // Force all the Markdown to be loaded on startup.
            app.ApplicationServices.GetRequiredService<MarkdownLoader>();
            // Force the set of releases to be first loaded on startup.
            app.ApplicationServices.GetRequiredService<IReleaseRepository>().GetReleases();
            // Force the set of benchmarks to be first loaded on startup.
            app.ApplicationServices.GetRequiredService<IBenchmarkRepository>().ListEnvironments();
            // Force the set of TZDB data to be first loaded on startup.
            app.ApplicationServices.GetRequiredService<ITzdbRepository>().GetReleases();

#if BLAZOR
            app.Map("/blazor", child => child.UseBlazor<NodaTime.Web.Blazor.Program>());
#endif
        }

        /// Sets the Cache-Control header for static content, conditionally allowing the browser to use the content
        /// without revalidation.
        private void SetCacheControlHeaderForStaticContent(IHostingEnvironment env, HttpContext context)
        {
            var headers = new ResponseHeaders(context.Response.Headers);

            if (headers.ContentType.IsSubsetOf(TextHtml))
            {
                // Don't set Cache-Control for HTML files (e.g. /tzdb/). The browser can figure out when to revalidate.
                // (Which it can do easily, since we send an ETag with all static content responses).
                return;
            }

            // Otherwise if the request was made with a file version query parameter (?v=..., as sent by
            // asp-append-version=true), then we can always use the response 'indefinitely', even in development mode.
            if (context.Request.Query.ContainsKey("v"))
            {
                headers.CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(365)
                };
                return;
            }

            // Otherwise, the remaining content (/favicon.ico, /fonts/, /robots.txt, /styles/docfx.js etc) should be
            // good to use for a while without revalidation. When running in the Development environment, we'll use a
            // much shorter time, since we might be iterating on it (in particular, this also covers the unminified
            // JS/CSS).
            headers.CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = env.IsDevelopment() ? TimeSpan.FromMinutes(2) : TimeSpan.FromDays(1)
            };
        }
    }
}
