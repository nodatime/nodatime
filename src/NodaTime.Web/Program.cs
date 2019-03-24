// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.using System;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace NodaTime.Web
{
    public class Program
    {
        public const string SmokeTestEnvironment = "SmokeTests";

        public static void Main(string[] args) =>
            CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                // Clear all logging here; we'll add back console and debug later. This unfortunately
                // makes startup harder to diagnose :(
                // We should probably revisit this. (How much of the configuration is available here in the context?)
                .ConfigureLogging(logging => logging.ClearProviders())
                // Uncomment these lines if startup is failing
                //.CaptureStartupErrors(true)
                //.UseSetting("detailedErrors", "true")
                ;
    }
}
