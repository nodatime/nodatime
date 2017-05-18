// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System.Collections.Generic;
using System.Linq;

namespace SnippetExtractor
{
    public sealed class Snippet
    {
        private readonly List<string> lines;
        private readonly List<string> usings;

        public string Uid { get; }

        public Snippet(string uid, IEnumerable<string> lines, IEnumerable<string> usings)
        {
            Uid = uid;
            this.lines = lines.ToList();
            this.usings = usings.ToList();
        }

        public string ToSeparateFile(string method)
        {
            var importSet = new HashSet<string>(usings);
            // For Console.WriteLine
            importSet.Add("using System;");
            // Not present in NodaTime.Demo, because it's not needed...
            importSet.Add("using NodaTime;");
            // Not present in NodaTime.Demo, because it's not needed...
            importSet.Add("using NodaTime.Demo;");

            // After the using directives, but before the method body.
            var prefix = new[]
            {
                 "public static partial class Samples",
                 "{",
                $"    public static void {method}()",
                 "    {",
            };

            // After the method body
            var suffix = new[]
            {
                 "    }",
                 "}"
            };
            return string.Join("\r\n", importSet.OrderBy(x => x.TrimEnd(';')).Concat(prefix).Concat(lines).Concat(suffix));
        }
    }
}