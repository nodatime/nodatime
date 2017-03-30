// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using NodaTime.Web.Controllers;
using NodaTime.Web.Models;
using NodaTime.Web.Providers;
using System.IO;

namespace NodaTime.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        private static IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            
            // Eagerly fetch the GoogleCredential so that we're not using Task.Result in
            // request processing.
            services.AddSingleton(GoogleCredentialProvider.FetchCredential(Configuration));
            services.AddSingleton<IReleaseRepository, GoogleStorageReleaseRepository>();
            services.AddSingleton<ITzdbRepository, GoogleStorageTzdbRepository>();
            services.AddSingleton<MarkdownLoader>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseDefaultFiles();
            // Default content, e.g. CSS
            app.UseStaticFiles();
            // API documentation
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "docfx")),
                ContentTypeProvider = new FileExtensionContentTypeProvider
                {
                    Mappings = { [".yml"] = "text/x-yaml" }
                }
            });
            // Captures "unstable" or a specific version - used several times below.
            string anyVersion = @"((?:1\.[0-3]\.x)|(?:unstable)|(?:2\.0\.x))";
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
                .AddRedirect("^(api|userguide)((?:/.*))$", "2.0.x/$1$2");
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
                routes.MapRoute("2.0.x user guide", "2.0.x/userguide/{*url}", new { controller = "Documentation", bundle = "2.0.x", action = nameof(DocumentationController.ViewDocumentation) });
                routes.MapRoute("Unstable user guide", "unstable/userguide/{*url}", new { controller = "Documentation", bundle = "unstable", action = nameof(DocumentationController.ViewDocumentation) });
                routes.MapRoute("default", "{action=Index}/{id?}", new { controller = "Home" });
            });

            // Force all the Markdown to be loaded on startup.
            app.ApplicationServices.GetRequiredService<MarkdownLoader>();
            // Force the set of releases to be first loaded on startup.
            app.ApplicationServices.GetRequiredService<IReleaseRepository>().GetReleases();
        }
    }
}
