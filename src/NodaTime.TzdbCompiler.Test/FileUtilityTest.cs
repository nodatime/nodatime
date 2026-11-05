// Copyright 2026 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Formats.Tar;
using NodaTime.Tools.Common;
using NUnit.Framework;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace NodaTime.TzdbCompiler.Test;

public class FileUtilityTest
{
    [Test]
    [TestCase(false, TestName = "LoadFileOrUrlAsync_Uncompressed")]
    [TestCase(true, TestName = "LoadFileOrUrlAsync_GzipCompressed")]
    public async Task LoadFileAsync(bool gzip)
    {
        var content = Encoding.UTF8.GetBytes("Some file content");
        var path = Path.GetTempFileName();
        try
        {
            using (var file = File.Create(path))
            using (Stream output = gzip ? new GZipStream(file, CompressionMode.Compress) : file)
            {
                output.Write(content, 0, content.Length);
            }

            using var stream = await FileUtility.LoadFileOrUrlAsync(path);
            var actual = new MemoryStream();
            stream.CopyTo(actual);
            Assert.AreEqual(content, actual.ToArray());
        }
        finally
        {
            File.Delete(path);
        }
    }
    
    [Test]
    [Explicit("External dependency")]
    [TestCase("http://data.iana.org/time-zones/releases/tzdata2026e.tar.gz", TestName = "LoadCompressedUrl_HTTP")]
    [TestCase("https://data.iana.org/time-zones/releases/tzdata2026d.tar.gz", TestName = "LoadCompressedUrl_HTTPS")]
    [TestCase("http://github.com/eggert/tz/archive/refs/tags/2026e.tar.gz", TestName = "LoadCompressedUrl_Redirect")]
    public async Task LoadCompressedUrlAsync(string url)
    {
        using var stream = await FileUtility.LoadFileOrUrlAsync(url);
        AssertTar(stream);
    }

    [Test]
    [Explicit("External dependency")]
    [TestCase("http://ftp.gnu.org/old-gnu/emacs/windows/contrib/keyswap.tar", TestName = "LoadUncompressedUrl_HTTP")]
    [TestCase("https://raw.githubusercontent.com/golang/go/go1.22.0/src/archive/tar/testdata/gnu.tar", TestName = "LoadUncompressedUrl_HTTPS")]
    [TestCase("http://github.com/golang/go/raw/go1.22.0/src/archive/tar/testdata/gnu.tar", TestName = "LoadUncompressedUrl_Redirect")]
    public async Task LoadUncompressedUrlAsync(string url)
    {
        using var stream = await FileUtility.LoadFileOrUrlAsync(url);
        AssertTar(stream);
    }

    private static void AssertTar(Stream stream)
    {
        using var reader = new TarReader(stream);
        var entry = reader.GetNextEntry();
        Assert.IsNotNull(entry);
    }
}
