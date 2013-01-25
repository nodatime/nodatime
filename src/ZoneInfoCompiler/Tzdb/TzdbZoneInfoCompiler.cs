// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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
            database.LogCounts();
            return database;
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
