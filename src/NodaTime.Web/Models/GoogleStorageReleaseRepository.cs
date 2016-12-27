// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NodaTime.Web.Models
{
    public class GoogleStorageReleaseRepository : IReleaseRepository
    {
        private static readonly Regex ReleasePattern = new Regex(@"NodaTime-(\d+\.\d+\.\d+)(?:-src)?.zip");
        private const string Bucket = "nodatime-releases";
        private const string Sha256Key = "SHA-256";

        private readonly StorageClient client;
        private List<ReleaseDownload> releases;

        public GoogleStorageReleaseRepository(GoogleCredential credential)
        {
            client = StorageClient.Create(credential);
        }

        public IList<ReleaseDownload> GetReleases()
        {
            // TODO: Be more elegant than this, invalidating every 10 minutes. IMemoryCache perhaps?
            if (releases == null)
            {
                releases = client.ListObjects(Bucket).Select(ConvertObject).ToList();
            }
            return releases;
        }

        private static ReleaseDownload ConvertObject(Google.Apis.Storage.v1.Data.Object obj)
        {
            string sha256Hash = null;
            obj.Metadata?.TryGetValue(Sha256Key, out sha256Hash);
            var match = ReleasePattern.Match(obj.Name);
            string release = null;
            if (match.Success)
            {
                release = match.Groups[1].Value;
            }
            return new ReleaseDownload(release, obj.Name, $"https://storage.googleapis.com/{Bucket}/{obj.Name}", sha256Hash);
        }
    }
}
