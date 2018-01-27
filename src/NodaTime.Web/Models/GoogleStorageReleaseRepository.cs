// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NodaTime.Web.Models
{
    public class GoogleStorageReleaseRepository : IReleaseRepository
    {
        private const string Bucket = "nodatime";
        private const string ObjectPrefix = "releases/";
        private static readonly Regex ReleasePattern = new Regex(ObjectPrefix + @"NodaTime-(\d+\.\d+\.\d+(?:-beta\d+)?)(?:-src)?.zip");
        private const string Sha256Key = "SHA-256";
        private const string ReleaseDateKey = "ReleaseDate";

        private static readonly Duration CacheRefreshTime = Duration.FromMinutes(6);
        private readonly StorageClient client;
        private readonly TimerCache<CacheValue> cache;

        public GoogleStorageReleaseRepository(
            IApplicationLifetime lifetime,
            ILoggerFactory loggerFactory,
            GoogleCredential credential)
        {
            client = StorageClient.Create(credential);
            cache = new TimerCache<CacheValue>(lifetime, CacheRefreshTime, FetchReleases, loggerFactory, CacheValue.Empty);
        }

        public IList<ReleaseDownload> GetReleases() => (cache.Value ?? FetchReleases()).Releases;

        public ReleaseDownload LatestRelease => (cache.Value ?? FetchReleases()).LatestRelease;

        private CacheValue FetchReleases()
        {
            var releases = client
                .ListObjects(Bucket, ObjectPrefix)
                .Where(obj => !obj.Name.EndsWith("/"))
                .Select(ConvertObject)
                .OrderByDescending(r => r.Version)
                .ToList();
            return new CacheValue(releases);
        }

        private static ReleaseDownload ConvertObject(Google.Apis.Storage.v1.Data.Object obj)
        {
            string sha256Hash = null;
            obj.Metadata?.TryGetValue(Sha256Key, out sha256Hash);
            string releaseDateMetadata = null;
            obj.Metadata?.TryGetValue(ReleaseDateKey, out releaseDateMetadata);
            var match = ReleasePattern.Match(obj.Name);
            StructuredVersion version = match.Success ? new StructuredVersion(match.Groups[1].Value) : null;
            LocalDate releaseDate = releaseDateMetadata == null
                ? LocalDate.FromDateTime(obj.Updated.Value)
                : LocalDatePattern.Iso.Parse(releaseDateMetadata).Value;
            return new ReleaseDownload(version, obj.Name.Substring(ObjectPrefix.Length), $"https://storage.googleapis.com/{Bucket}/{obj.Name}", sha256Hash, releaseDate);
        }

        private class CacheValue
        {
            public List<ReleaseDownload> Releases { get; }
            public ReleaseDownload LatestRelease { get; }

            public static CacheValue Empty { get; } =
                new CacheValue(new List<ReleaseDownload> { new ReleaseDownload(null, "Dummy", "", "", new LocalDate(2000, 1, 1)) });

            public CacheValue(List<ReleaseDownload> releases)
            {
                Releases = releases;
                // "Latest" is in terms of version, not release date. (So if
                // 1.4 comes out after 2.0, 2.0 is still latest.)
                // Pre-release versions are excluded.
                LatestRelease = releases
                    .Where(r => !r.File.Contains("-src") && r.Version?.Prerelease == null)
                    .OrderByDescending(r => r.Version)
                    .First();
            }
        }
    }
}
