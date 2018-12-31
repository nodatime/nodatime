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
            new ReleaseDownload(new StructuredVersion("2.4.4"), "NodaTime-2.4.4.zip",
                "https://storage.cloud.google.com/nodatime/releases/NodaTime-2.4.4.zip",
                "5a672e0910353eef53cd3b6a4ff08e1287ec6fe40faf96ca42e626b107c8f8d4",
                new LocalDate(2018, 12, 31))
        };

        public ReleaseDownload LatestRelease => releases[0];

        public IList<ReleaseDownload> GetReleases() => releases;
    }
}
