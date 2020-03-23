// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ShellProgressBar;

namespace NodaTime.Tools.ValidateHistoricalNzd
{
    class Program
    {
        private static async Task<int> Main()
        {
            var httpClient = new HttpClient();
            var allUrlsText = await httpClient.GetStringAsync("https://nodatime.org/tzdb/index.txt");
            var urls = allUrlsText.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries);
            var progressBar = new ProgressBar(urls.Length, "Validating historical nzd files");
            var exceptions = new List<Exception>();
            foreach (var url in urls)
            {
                progressBar.Tick($"Validating {url}");
                var exception = await ValidateAsync(httpClient, url);
                if (exception != null)
                    exceptions.Add(exception);
            }
            foreach (var exception in exceptions)
            {
                progressBar.WriteLine(exception.Message);
            }
            return exceptions.Any() ? 1 : 0;
        }

        private static async Task<Exception?> ValidateAsync(HttpClient httpClient, string url)
        {
            try
            {
                byte[] data = await httpClient.GetByteArrayAsync(url);
                var source = TzdbDateTimeZoneSource.FromStream(new MemoryStream(data));
                source.Validate();
                return null;
            }
            catch (Exception e)
            {
                return new ApplicationException($"Validation failed for {url} [{e.GetType().Name}] {e.Message}", e);
            }
        }
    }
}
