// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;

namespace NodaTime.Web.Configuration
{
    /// <summary>
    /// Options for networking, e.g. HTTPS redirection.
    /// </summary>
    public class NetworkOptions
    {
        // Options for HTTPS redirection
        public int? HttpsPort { get; set; }
        public int HttpsRedirectStatusCode { get; set; } = new HttpsRedirectionOptions().RedirectStatusCode;

        // Options for header forwarding
        public bool UseForwardedHeaders { get; set; }
        public List<string> KnownNetworks { get; set; } = new List<string>();
        public int? ForwardLimit = 1;
        public List<ForwardedHeaders> ForwardedHeaders { get; set; } = new List<ForwardedHeaders>();

        public void ConfigureServices(IServiceCollection services)
        {
            if (UseForwardedHeaders)
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    foreach (var network in KnownNetworks)
                    {
                        string[] bits = network.Split('/');
                        if (bits.Length != 2)
                        {
                            throw new ArgumentException($"Invalid NetworkOptions.KnownNetworks element: '{network}'");
                        }
                        var prefix = IPAddress.Parse(bits[0]);
                        int prefixLength = int.Parse(bits[1]);
                        options.KnownNetworks.Add(new IPNetwork(prefix, prefixLength));
                    }
                    foreach (var forwardedHeader in ForwardedHeaders)
                    {
                        options.ForwardedHeaders |= forwardedHeader;
                    }
                    options.ForwardLimit = ForwardLimit;
                });
            }

            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = HttpsPort;
                options.RedirectStatusCode = HttpsRedirectStatusCode;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (UseForwardedHeaders)
            {
                app.UseForwardedHeaders();
            }
            app.UseHttpsRedirection();
        }
    }
}
