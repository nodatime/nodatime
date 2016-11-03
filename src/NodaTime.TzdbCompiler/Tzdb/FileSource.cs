// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using SharpCompress.Readers.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NodaTime.TzdbCompiler.Tzdb
{
    /// <summary>
    /// A random access collection of files.
    /// </summary>
    internal sealed class FileSource
    {
        /// <summary>
        /// The name of the origin of the files - just the last part,
        /// rather than the whole URL/path.
        /// </summary>
        internal string Origin { get; }

        /// <summary>
        /// The names of 
        /// </summary>
        internal IList<string> Names { get; }

        private readonly Func<string, Stream> openFunction;

        private FileSource(IList<string> names, Func<string, Stream> openFunction, string fullOrigin)
        {
            Names = names; // Could create a ReadOnlyCollection if we ever wanted to make this public
            Origin = Path.GetFileName(fullOrigin);
            this.openFunction = openFunction;
        }

        internal Stream Open(string name) => openFunction(name);

        internal bool Contains(string name) => Names.Contains(name);

        internal static FileSource FromArchive(Stream archiveData, string fullOrigin)
        {
            var entries = new Dictionary<string, byte[]>();
            using (var reader = TarReader.Open(archiveData))
            {
                while (reader.MoveToNextEntry())
                {
                    if (reader.Entry.IsDirectory)
                    {
                        continue;
                    }
                    var entryStream = new MemoryStream();
                    reader.WriteEntryTo(entryStream);
                    // The lzip file puts everything into a subdirectory. Let's just take the filename...
                    entries[Path.GetFileName(reader.Entry.Key)] = entryStream.ToArray();
                }
            }
            return new FileSource(entries.Keys.ToList(), file => new MemoryStream(entries[file]), fullOrigin);
        }

        internal static FileSource FromDirectory(string path)
        {
            var files = Directory.GetFiles(path).Select(p => Path.GetFileName(p)).ToList();
            return new FileSource(files, file => File.OpenRead(Path.Combine(path, file)), Path.GetFileName(path));
        }
    }
}
