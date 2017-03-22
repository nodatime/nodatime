// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.Web.Models
{
    // TODO: See if ASP.NET Core already has a good cache strategy. This isn't ideal.
    public class GoogleStorageTzdbRepository : ITzdbRepository
    {
        private const string Bucket = "nodatime";
        private static readonly IClock Clock = SystemClock.Instance;
        private static readonly Duration CacheRefreshTime = Duration.FromMinutes(5);

        private readonly object padlock = new object();
        private readonly StorageClient client;
        private List<TzdbDownload> releases;
        private Dictionary<string, TzdbDownload> releasesByName = new Dictionary<string, TzdbDownload>();
        private Instant nextCacheRefresh;

        public GoogleStorageTzdbRepository(GoogleCredential credential)
        {
            client = StorageClient.Create(credential);
        }

        public IList<TzdbDownload> GetReleases()
        {
            lock (padlock)
            {
                MaybeUpdateReleases();
                return releases;
            }
        }

        public TzdbDownload GetRelease(string name)
        {
            lock (padlock)
            {
                TzdbDownload value;
                MaybeUpdateReleases();
                releasesByName.TryGetValue(name, out value);
                return value;
            }
        }

        private void MaybeUpdateReleases()
        {
            if (releases != null && Clock.GetCurrentInstant() < nextCacheRefresh)
            {
                return;
            }

            nextCacheRefresh = Clock.GetCurrentInstant() + CacheRefreshTime;
            releases = client.ListObjects(Bucket, "tzdb/")
                                .Where(o => o.Name.EndsWith(".nzd"))
                                .Select(obj => new TzdbDownload($"https://storage.googleapis.com/{Bucket}/{obj.Name}"))
                                .Select(r => releasesByName.ContainsKey(r.Name) ? releasesByName[r.Name] : r)
                                .OrderBy(r => r.Name, StringComparer.Ordinal)
                                .ToList();
            releasesByName = releases.ToDictionary(r => r.Name);
        }
    }
}
