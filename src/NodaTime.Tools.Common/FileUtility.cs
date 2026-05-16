// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace NodaTime.Tools.Common
{
    public static class FileUtility
    {
        public static async Task<Stream> LoadFileOrUrlAsync(string source)
        {
            if (source.StartsWith("http://") || source.StartsWith("https://") || source.StartsWith("ftp://"))
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(source, HttpCompletionOption.ResponseHeadersRead);
                var stream = await response.EnsureSuccessStatusCode().Content.ReadAsStreamAsync();

                var mediaType = response.Content.Headers.ContentType?.MediaType;
                var gzip = string.Equals(mediaType, "application/gzip", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(mediaType, "application/x-gzip", StringComparison.OrdinalIgnoreCase);
                return gzip ? new GZipStream(stream, CompressionMode.Decompress) : stream;
            }
            return File.OpenRead(source);
        }
    }
}
