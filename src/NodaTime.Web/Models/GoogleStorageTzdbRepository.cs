// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NodaTime.Web.Models
{
    public class GoogleStorageTzdbRepository : ITzdbRepository
    {
        private static readonly Regex PlausibleReleaseName = new Regex(@"^[-_a-zA-Z0-9.]+\.nzd$");

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
            cache.Start();
        }

        public IList<TzdbDownload> GetReleases() => (cache.Value ?? FetchReleases()).Releases;

        public TzdbDownload? GetRelease(string name)
        {
            var releasesByName = (cache.Value ?? FetchReleases()).ReleasesByName;
            if (releasesByName.TryGetValue(name, out var value))
            {
                return value;
            }
            // We don't know about the requested release from the last cache refresh. See whether
            // it's *possibly* a real release, and if so try to fetch the object metadata.
            // If that succeeds, serve that (and remember it) until we next refresh the cache.
            if (!PlausibleReleaseName.IsMatch(name))
            {
                return null;
            }
            var onDemandCache = cache.Value?.OnDemandReleasesByName ?? new ConcurrentDictionary<string, TzdbDownload>();
            if (onDemandCache.TryGetValue(name, out value))
            {
                return value;
            }

            string objectName = $"tzdb/{name}";
            try
            {
                client.GetObject(Bucket, objectName);
            }
            catch (GoogleApiException)
            {
                // If we can't get the metadata entry for *any* reason, just return that it's not been found.
                return null;
            }
            value = new TzdbDownload($"https://storage.googleapis.com/{Bucket}/{objectName}");
            onDemandCache[name] = value;
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
            /// <summary>
            /// Temporary cache for releases we are asked for but didn't previously know about.
            /// This can occur if a new release has just been published, but we haven't refreshed
            /// our list yet. We still want to serve the file, and cache it, but we're happy
            /// to wait until the main refresh before listing it directly.
            /// </summary>
            public ConcurrentDictionary<string, TzdbDownload> OnDemandReleasesByName { get; }

            public Dictionary<string, TzdbDownload> ReleasesByName { get; }
            public List<TzdbDownload> Releases { get; }
            public static CacheEntry Empty { get; } = new CacheEntry(new List<TzdbDownload>());

            public CacheEntry(List<TzdbDownload> releases)
            {
                Releases = releases;
                ReleasesByName = releases.ToDictionary(r => r.Name);
                OnDemandReleasesByName = new ConcurrentDictionary<string, TzdbDownload>();
            }
        }
    }
}
