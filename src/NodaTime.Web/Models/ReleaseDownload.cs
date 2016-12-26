﻿// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Web.Models
{
    public class ReleaseDownload
    {
        public string Release { get; }
        public string File { get; }
        public string DownloadUrl { get; }
        public string Sha256Hash { get; }

        public ReleaseDownload(string release, string file, string downloadUrl, string sha256Hash)
        {
            Release = release;
            File = file;
            DownloadUrl = downloadUrl;
            Sha256Hash = sha256Hash;
        }
    }
}
