// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SnippetExtractor
{
    /// <summary>
    /// A snippet as it is in the original source code.
    /// </summary>
    public sealed class SourceSnippet
    {
        public ReadOnlyCollection<string> Lines { get; }
        public ReadOnlyCollection<string> Usings { get; }
        public string Uid { get; }

        public SourceSnippet(string uid, IEnumerable<string> lines, IEnumerable<string> usings)
        {
            Uid = uid;
            Lines = lines.ToList().AsReadOnly();
            Usings = usings.ToList().AsReadOnly();
        }
    }
}