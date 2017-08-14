// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System.Collections.Generic;

namespace NodaTime.Web.Models
{
    /// <summary>
    /// Fake implementation of IReleaseRepository used in cases of emergency...
    /// </summary>
    public class FakeReleaseRepository : IReleaseRepository
    {
        private static readonly IList<ReleaseDownload> releases = new[]
        {
            new ReleaseDownload(new StructuredVersion("2.0.0"), "NodaTime-2.0.0.zip",
                "https://storage.cloud.google.com/nodatime/releases/NodaTime-2.0.0.zip",
                "36c6e7b4c10ba21e39b8652987e5c8c0f46a3f03f83f5265bf2893c8837cf635",
                new LocalDate(2017, 3, 31))
        };

        public ReleaseDownload LatestRelease => releases[0];

        public IList<ReleaseDownload> GetReleases() => releases;
    }
}
