// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using NodaTime.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NodaTime.Web.Models
{
    // TODO: See if ASP.NET Core already has a good cache strategy. This isn't ideal.
    public class GoogleStorageReleaseRepository : IReleaseRepository
    {
        private const string Bucket = "nodatime";
        private const string ObjectPrefix = "releases/";
        private static readonly Regex ReleasePattern = new Regex(ObjectPrefix + @"NodaTime-(\d+\.\d+\.\d+)(?:-src)?.zip");
        private const string Sha256Key = "SHA-256";
        private const string ReleaseDateKey = "ReleaseDate";

        private static readonly IClock Clock = SystemClock.Instance;
        private static readonly Duration CacheRefreshTime = Duration.FromMinutes(5);
        private readonly object padlock = new object();
        private readonly StorageClient client;

        private ReleaseDownload latestRelease;
        private List<ReleaseDownload> releases;
        private Instant nextCacheRefresh;

        public GoogleStorageReleaseRepository(GoogleCredential credential)
        {
            client = StorageClient.Create(credential);
        }

        public IList<ReleaseDownload> GetReleases()
        {
            lock (padlock)
            {
                MaybeUpdateReleases();
                return releases;
            }
        }

        public ReleaseDownload LatestRelease
        {
            get
            {
                lock (padlock)
                {
                    MaybeUpdateReleases();
                    return latestRelease;
                }
            }
        }

        private void MaybeUpdateReleases()
        {
            if (releases != null && Clock.GetCurrentInstant() < nextCacheRefresh)
            {
                return;
            }

            nextCacheRefresh = Clock.GetCurrentInstant() + CacheRefreshTime;
            releases = client
                .ListObjects(Bucket, ObjectPrefix)
                .Where(obj => !obj.Name.EndsWith("/"))
                .Select(ConvertObject)
                .OrderByDescending(r => r.Release)
                .ToList();
            // "Latest" is in terms of version, not release date. (So if
            // 1.4 comes out after 2.0, 2.0 is still latest.)
            latestRelease = releases
                .Where(r => !r.File.Contains("-src"))
                .OrderByDescending(r => r.Release)
                .First();
        }

        private static ReleaseDownload ConvertObject(Google.Apis.Storage.v1.Data.Object obj)
        {
            string sha256Hash = null;
            obj.Metadata?.TryGetValue(Sha256Key, out sha256Hash);
            string releaseDateMetadata = null;
            obj.Metadata?.TryGetValue(ReleaseDateKey, out releaseDateMetadata);
            var match = ReleasePattern.Match(obj.Name);
            string release = null;
            if (match.Success)
            {
                release = match.Groups[1].Value;
            }
            LocalDate releaseDate = releaseDateMetadata == null
                ? LocalDate.FromDateTime(obj.Updated.Value)
                : LocalDatePattern.Iso.Parse(releaseDateMetadata).Value;
            return new ReleaseDownload(release, obj.Name.Substring(ObjectPrefix.Length), $"https://storage.googleapis.com/{Bucket}/{obj.Name}", sha256Hash, releaseDate);
        }
    }
}
