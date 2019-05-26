// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NodaTime.Tools.ValidateHistoricalNzd
{
    class Program
    {
        private static async Task<int> Main()
        {
            var httpClient = new HttpClient();
            var allUrlsText = await httpClient.GetStringAsync("https://nodatime.org/tzdb/index.txt");
            var urls = allUrlsText.Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries);
            bool success = true;
            foreach (var url in urls)
            {
                success &= await ValidateAsync(httpClient, url);
            }
            return success ? 0 : 1;
        }

        private static async Task<bool> ValidateAsync(HttpClient httpClient, string url)
        {
            try
            {
                Console.WriteLine($"Validating {url}");
                byte[] data = await httpClient.GetByteArrayAsync(url);
                var source = TzdbDateTimeZoneSource.FromStream(new MemoryStream(data));
                source.Validate();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType().Name}: {e.Message}");
                return false;
            }
        }
    }
}
