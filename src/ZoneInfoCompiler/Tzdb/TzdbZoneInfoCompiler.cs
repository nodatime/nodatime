#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NodaTime.TimeZones;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    ///   Provides a compiler for Olson (TZDB) zone info files into the internal format used by Noda
    ///   Time for its <see cref="DateTimeZone" /> definitions. This read a set of files and generates
    ///   a resource file with the compiled contents suitable for reading with <see cref="TzdbDateTimeZoneSource" /> or one of its variants.
    /// </summary>
    public class TzdbZoneInfoCompiler
    {
        // Files that we don't want to build into the TZDB resources. Excluding them here saves the maintainer from having to
        // remove them manually.
        private static readonly HashSet<string> ExcludedFiles = new HashSet<string>
        { 
            "factory",
            "iso3166.tab",
            "leapseconds",
            "Makefile",
            "solar87",
            "solar88",
            "solar89",
            "yearistype.sh",
            "zone.tab",
            "Readme.txt" // Just to handle old directories in Noda Time. Not part of tzdb.
        };

        private readonly TzdbZoneInfoParser tzdbParser;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TzdbZoneInfoCompiler" /> class.
        /// </summary>
        /// <param name="log">The log to send all output messages to.</param>
        internal TzdbZoneInfoCompiler()
        {
            tzdbParser = new TzdbZoneInfoParser();
        }

        internal TzdbDatabase Compile(string sourceDirectoryName)
        {
            Console.WriteLine("Starting compilation of directory {0}", sourceDirectoryName);
            var sourceDirectory = new DirectoryInfo(sourceDirectoryName);
            var fileList = sourceDirectory.GetFiles().Where(file => !ExcludedFiles.Contains(file.Name));
            string version = sourceDirectory.Name;
            var database = new TzdbDatabase(version);
            ParseAllFiles(fileList, database);
            LogCounts(database);
            return database;
        }

        /// <summary>
        ///   Writes various informational counts to the log.
        /// </summary>
        /// <param name="database">The database to query for the counts.</param>
        private void LogCounts(TzdbDatabase database)
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("Rule sets: {0:D}", database.Rules.Count);
            Console.WriteLine("Zones:     {0:D}", database.ZoneLists.Count);
            Console.WriteLine("Aliases:   {0:D}", database.Aliases.Count);
            Console.WriteLine("=======================================");
        }

        /// <summary>
        ///   Parses all of the given files.
        /// </summary>
        /// <param name="files">The <see cref="IEnumerable{T}" /> of <see cref="FileInfo" /> objects.</param>
        /// <param name="database">The <see cref="TzdbDatabase" /> where the parsed data is placed.</param>
        private void ParseAllFiles(IEnumerable<FileInfo> files, TzdbDatabase database)
        {
            foreach (var file in files)
            {
                Console.WriteLine("Parsing file {0} . . .", file.Name);
                ParseFile(file, database);
            }
        }

        /// <summary>
        ///   Parses the file defined by the given <see cref="FileInfo" />.
        /// </summary>
        /// <remarks>
        ///   Currently this compiler only handles files in the Olson (TZDB) zone info format.
        /// </remarks>
        /// <param name="file">The file to parse.</param>
        /// <param name="database">The <see cref="TzdbDatabase" /> where the parsed data is placed.</param>
        internal void ParseFile(FileInfo file, TzdbDatabase database)
        {
            using (FileStream stream = file.OpenRead())
            {
                tzdbParser.Parse(stream, database);
            }
        }
    }
}
