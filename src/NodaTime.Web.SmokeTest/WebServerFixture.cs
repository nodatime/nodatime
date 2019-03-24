// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NodaTime.Web.SmokeTest
{
    [SetUpFixture]
    public class WebServerFixture
    {
        public static WebServerFixture Instance { get; set; }
        private const int ServerStartupSeconds = 5;
        private static readonly TimeSpan GracefulShutdown = TimeSpan.FromSeconds(5);

        public IWebHost Host { get; private set; }
        public string BaseUrl { get; private set; }
        private CancellationTokenSource serverCancellation;
        private Task serverTask;

        public WebServerFixture()
        {
            if (Instance != null)
            {
                throw new Exception("WebServerFixture should only be constructed once");
            }
            Instance = this;
        }

        [OneTimeSetUp]
        public void StartServer()
        {
            serverCancellation = new CancellationTokenSource();
            var contentRoot = FindNodaTimeWebProject();
            Host = Program.CreateWebHostBuilder(new string[0])
                .UseContentRoot(contentRoot)
                .UseEnvironment(Program.SmokeTestEnvironment)
                .Build();
            serverTask = Host.StartAsync(serverCancellation.Token);
            // Wait for the server to start up, and the TZDB/release caches to populate.
            // (The benchmark cache takes longer, but we're not too worried about them.)
            Thread.Sleep(5000);
            BaseUrl = Host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.FirstOrDefault();
            if (BaseUrl == null)
            {
                throw new Exception("Couldn't find server addresses; server startup error?");
            }
        }

        [OneTimeTearDown]
        public void StopServer()
        {
            serverCancellation.Cancel();
            if (!serverTask.Wait(GracefulShutdown))
            {
                throw new Exception("Server failed to shut down");
            }
        }

        private static string FindNodaTimeWebProject()
        {
            string[] expectedFiles = { "AUTHORS.txt", "README.md", "LICENSE.txt", "NOTICE.txt", "global.json" };
            var root = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (root != null && !IsRoot(root))
            {
                root = root.Parent;
            }
            if (root == null)
            {
                throw new Exception("Couldn't determine root directory");
            }
            return Path.Combine(root.FullName, "src", "NodaTime.Web");

            bool IsRoot(DirectoryInfo candidate) =>
                candidate.GetFiles().Select(f => f.Name).Intersect(expectedFiles).Count() == expectedFiles.Length;
        }
    }
}
