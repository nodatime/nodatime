// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace NodaTime.Tools.Common;

public static class FileUtility
{
    /// <summary>
    /// Loads the given file or URL, decompressing it if it's gzip-compressed.
    /// </summary>
    public static async Task<Stream> LoadFileOrUrlAsync(string source)
    {
        if (source.StartsWith("http://") || source.StartsWith("https://"))
        {
            using var handler = new HttpClientHandler();
            handler.AllowAutoRedirect = true;
            using var client = new HttpClient(handler);
            var response = await client.GetAsync(source, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();
            // HttpClient with ResponseContentRead returns a seekable MemoryStream
            var content = await response.Content.ReadAsStreamAsync();
            return Decompress(content);
        }
        
        return Decompress(File.OpenRead(source));
    }

    /// <summary>
    /// Returns <paramref name="stream"/> decompressed if it starts with the gzip magic number.
    /// </summary>
    private static Stream Decompress(Stream stream)
    {
        Preconditions.CheckArgument(stream is { CanSeek: true, Position: 0 }, nameof(stream), "Stream should be seekable and positioned at the start.");
        if (stream.Length < 2)
        {
            // Stream cannot be a gzip file
            return stream;
        }
        
        var first = stream.ReadByte();
        var second = stream.ReadByte();
        stream.Position = 0;
        return first == 0x1f && second == 0x8b
            ? new GZipStream(stream, CompressionMode.Decompress, false)
            : stream;
    }
}
