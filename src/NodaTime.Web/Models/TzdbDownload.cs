using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace NodaTime.Web.Models
{
    /// <summary>
    /// A download of TZDB data in the form of an NZD file.
    /// </summary>
    public class TzdbDownload
    {
        private static readonly HttpClient httpClient = new HttpClient();
        public string Name { get; }
        public string NodaTimeOrgUrl { get; }

        private readonly Lazy<byte[]> data;

        public TzdbDownload(string storageUrl)
        {
            Name = Path.GetFileName(new Uri(storageUrl).LocalPath);
            NodaTimeOrgUrl = $"https://nodatime.org/tzdb/{Name}";
            data = new Lazy<byte[]>(
                () => httpClient.GetByteArrayAsync(storageUrl).Result,
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public Stream GetContent() => new MemoryStream(data.Value);
    }
}
