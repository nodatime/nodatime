// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System.Collections.Generic;
using System;
using System.Linq;

namespace NodaTime.Web.Models
{
    public class MarkdownBundle
    {
        private Dictionary<string, MarkdownPage> pagesById;

        public string Name { get; set; }
        public List<Category> Categories { get; set; }

        public class Category
        {
            public string Title { get; set; }
            public List<string> PageIds { get; set; }
            public List<MarkdownPage> Pages { get; set; }
        }

        // Urgh, this is nasty...
        public void BuildIndex()
        {
            pagesById = Categories.SelectMany(c => c.Pages).ToDictionary(p => p.Id);
        }

        public MarkdownPage TryGetPage(string id)
        {
            MarkdownPage ret;
            pagesById.TryGetValue(id, out ret);
            return ret;
        }
    }
}
