// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.Web.Models
{
    public class GoogleStorageTzdbRepository : ITzdbRepository
    {
        private const string Bucket = "nodatime";
        private static readonly Duration CacheRefreshTime = Duration.FromMinutes(7);

        private readonly StorageClient client;
        private readonly TimerCache<CacheEntry> cache;

        public GoogleStorageTzdbRepository(
            IApplicationLifetime lifetime,
            ILoggerFactory loggerFactory,
            GoogleCredential credential)
        {
            client = StorageClient.Create(credential);
            cache = new TimerCache<CacheEntry>(lifetime, CacheRefreshTime, FetchReleases, loggerFactory, CacheEntry.Empty);
        }

        public IList<TzdbDownload> GetReleases() => (cache.Value ?? FetchReleases()).Releases;

        public TzdbDownload GetRelease(string name)
        {
            var releasesByName = (cache.Value ?? FetchReleases()).ReleasesByName;
            TzdbDownload value;
            releasesByName.TryGetValue(name, out value);
            return value;
        }

        private CacheEntry FetchReleases()
        {
            var oldReleasesByName = cache.Value?.ReleasesByName ?? new Dictionary<string, TzdbDownload>();
            var releases = client.ListObjects(Bucket, "tzdb/")
                                .Where(o => o.Name.EndsWith(".nzd"))
                                .Select(obj => new TzdbDownload($"https://storage.googleapis.com/{Bucket}/{obj.Name}"))
                                .Select(r => oldReleasesByName.ContainsKey(r.Name) ? oldReleasesByName[r.Name] : r)
                                .OrderBy(r => r.Name, StringComparer.Ordinal)
                                .ToList();
            return new CacheEntry(releases);
        }

        private class CacheEntry
        {
            public Dictionary<string, TzdbDownload> ReleasesByName { get; }
            public List<TzdbDownload> Releases { get; }
            public static CacheEntry Empty { get; } = new CacheEntry(new List<TzdbDownload>());

            public CacheEntry(List<TzdbDownload> releases)
            {
                Releases = releases;
                ReleasesByName = releases.ToDictionary(r => r.Name);
            }
        }
    }
}
