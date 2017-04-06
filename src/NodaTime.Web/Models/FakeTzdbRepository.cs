// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.Web.Models
{
    /// <summary>
    /// Fake implementation of ITzdbRepository used in cases of emergency...
    /// </summary>
    public class FakeTzdbRepository : ITzdbRepository
    {
        private static readonly IList<TzdbDownload> releases = new[]
        {
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2013h.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2013i.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2014a.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2014b.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2014c.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2014e.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2014f.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2014g.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2014h.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2014i.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2014j.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2015a.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2015b.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2015c.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2015d.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2015e.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2015f.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2015g.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2016a.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2016b.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2016c.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2016d.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2016e.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2016f.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2016g.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2016h.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2016i.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2016j.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2017a.nzd"),
            new TzdbDownload("https://storage.googleapis.com/nodatime/tzdb/tzdb2017b.nzd")
        };

        public TzdbDownload GetRelease(string name) => releases.FirstOrDefault(r => r.Name == name);

        public IList<TzdbDownload> GetReleases() => releases;
    }
}
