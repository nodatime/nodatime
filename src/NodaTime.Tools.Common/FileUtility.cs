// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NodaTime.Tools.Common
{
    public static class FileUtility
    {
        public static byte[] LoadFileOrUrl(string source)
        {
            if (source.StartsWith("http://") || source.StartsWith("https://") || source.StartsWith("ftp://"))
            {
                // This is an ugly way of avoiding asynchrony, but we're only using it in
                // command line tools.
                var task = Task.Run(() => FetchContentAsync(source));
                return task.Result;
            }
            return File.ReadAllBytes(source);

            async Task<byte[]> FetchContentAsync(string url)
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(source);
                    return await response.EnsureSuccessStatusCode().Content.ReadAsByteArrayAsync();
                }
            }
        }
    }
}
