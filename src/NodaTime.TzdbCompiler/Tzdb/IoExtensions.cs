// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.IO;

namespace NodaTime.TzdbCompiler.Tzdb
{
    /// <summary>
    /// IO-related extension methods
    /// </summary>
    internal static class IoExtensions
    {
        internal static IEnumerable<string> ReadLines(this Stream stream) => ReadLines(new StreamReader(stream));

        internal static IEnumerable<string> ReadLines(this TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        internal static IEnumerable<string> ReadLines(Func<TextReader> readerProvider)
        {
            using (var reader = readerProvider())
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }


        internal static IEnumerable<string> ReadLines(this FileSource source, string name) => ReadLines(() => new StreamReader(source.Open(name)));
    }
}
