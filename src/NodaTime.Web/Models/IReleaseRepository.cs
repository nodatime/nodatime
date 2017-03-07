// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System.Collections.Generic;

namespace NodaTime.Web.Models
{
    public interface IReleaseRepository
    {
        IList<ReleaseDownload> GetReleases();

        ReleaseDownload LatestRelease { get; }
    }
}
