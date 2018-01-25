// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.using System;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NodaTime.Web
{
    public class Program
    {
        private const string SmokeTestArg = "--smoke-test";

        public static void Main(string[] args)
        {
            if (args.Contains(SmokeTestArg))
            {
                RunSmokeTests().GetAwaiter().GetResult();
            }
            else
            {
                CreateHost().Run();
            }
        }

        private static IWebHost CreateHost() =>
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                // Uncomment these lines if startup is failing
                //.CaptureStartupErrors(true)
                //.UseSetting("detailedErrors", "true")
                .Build();

        const int TestDurationSeconds = 15;
        const int ServerStartupSeconds = 5;
        const int GracefulShutdownSeconds = 5;
        const string TestUrlBase = "http://localhost:5000";

        private static async Task RunSmokeTests()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "smoketests");
            var cts = new CancellationTokenSource();
            Task serverTask = CreateHost().RunAsync(cts.Token);

            // Give it 5 seconds to come up. We probably don't need this long, but it's harmless.
            Thread.Sleep(ServerStartupSeconds * 1000);

            bool success = false;
            try
            {
                var testTask = FetchSmokePagesAsync();
                var completed = await Task.WhenAny(testTask, Task.Delay(TimeSpan.FromSeconds(TestDurationSeconds)));
                if (testTask != completed)
                {
                    throw new Exception($"Tests did not complete in {TestDurationSeconds} seconds");
                }
                await testTask;
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failure: {e.Message}");
            }
            // Try to let the server shut down gracefully.
            cts.Cancel();
            await Task.WhenAny(serverTask, Task.Delay(GracefulShutdownSeconds));

            Console.WriteLine($"Smoke test complete. Result: {success}");
            Environment.Exit(success ? 0 : 1);
        }

        private static async Task FetchSmokePagesAsync()
        {
            await ExpectPageText("downloads", "NodaTime-1.0.0.zip");
            await ExpectPageText("1.0.x/userguide/text", "There are two options for text handling");
            await ExpectPageText("1.0.x/api/NodaTime.DateTimeZone.html", "The mapping is unambiguous");
            await ExpectPageText("tzdb/index.txt", "https://nodatime.org/tzdb/tzdb2013h.nzd");
            await ExpectBinarySize("tzdb/tzdb2013h.nzd", 125962);
        }

        private static async Task ExpectPageText(string relativeUrl, string expectedText)
        {
            var client = new HttpClient();
            string page = await client.GetStringAsync($"{TestUrlBase}/{relativeUrl}");
            if (!page.Contains(expectedText))
            {
                string start = page.Substring(0, Math.Min(40, page.Length));
                throw new Exception($"Content of {relativeUrl} did not contain {expectedText}. Start of content: {start}. Content length: {page.Length}");
            }
            Console.WriteLine($"Smoke test success for {relativeUrl}");
        }

        private static async Task ExpectBinarySize(string relativeUrl, int expectedSize)
        {
            var client = new HttpClient();
            byte[] data = await client.GetByteArrayAsync($"{TestUrlBase}/{relativeUrl}");
            if (data.Length != expectedSize)
            {
                throw new Exception($"URL {relativeUrl} returned {data.Length} bytes; expected {expectedSize}");
            }
            Console.WriteLine($"Smoke test success for {relativeUrl}");
        }
    }
}
