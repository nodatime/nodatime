// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Web.Models
{
    public class ReleaseDownload
    {
        public StructuredVersion Version { get; }
        public string File { get; }
        public string DownloadUrl { get; }
        public string Sha256Hash { get; }
        public LocalDate ReleaseDate { get; set; }

        public ReleaseDownload(StructuredVersion version, string file, string downloadUrl, string sha256Hash, LocalDate releaseDate)
        {
            Version = version;
            File = file;
            DownloadUrl = downloadUrl;
            Sha256Hash = sha256Hash;
            ReleaseDate = releaseDate;
        }
    }
}
