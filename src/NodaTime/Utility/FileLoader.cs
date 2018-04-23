// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.IO;
using System.Net.Http;

namespace NodaTime.Utility
{
    /// <summary>
    /// Provides method to help with loading file or URL.
    /// </summary>
    public static class FileLoader
    {
        /// <summary>
        /// Loads a given file or URL, and returns its contents as a byte array.
        /// </summary>
        /// <param name="source">Path to the source</param>
        /// <returns></returns>
        public static byte[] LoadFileOrUrl(string source)
        {
            if (source.StartsWith("http://") || source.StartsWith("https://") || source.StartsWith("ftp://"))
            {
                using (var client = new HttpClient())
                {
                    // I know using .Result is nasty, but we're in a console app, and nothing is
                    // going to deadlock...
                    return client.GetAsync(source).Result.EnsureSuccessStatusCode().Content.ReadAsByteArrayAsync().Result;
                }
            }

            return File.ReadAllBytes(source);
        }
    }
}
