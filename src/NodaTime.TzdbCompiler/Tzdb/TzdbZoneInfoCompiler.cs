// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace NodaTime.TzdbCompiler.Tzdb
{
    /// <summary>
    /// Provides a compiler for Olson (TZDB) zone info files into the internal format used by Noda
    /// Time for its <see cref="DateTimeZone" /> definitions. This read a set of files and generates
    /// a resource file with the compiled contents suitable for reading with <see cref="TzdbDateTimeZoneSource" /> or one of its variants.
    /// </summary>
    public class TzdbZoneInfoCompiler
    {
        private const string Makefile = "Makefile";
        private const string Zone1970TabFile = "zone1970.tab";
        private const string Iso3166TabFile = "iso3166.tab";
        private const string ZoneTabFile = "zone.tab";

        private static ReadOnlyCollection<string> ZoneFiles { get; } = new ReadOnlyCollection<string>(new[]
        {
            "africa", "antarctica", "asia", "australasia", "europe",
            "northamerica", "southamerica", "pacificnew", "etcetera", "backward", "systemv"
        });

        private static readonly Regex VersionRegex = new Regex(@"\d{2,4}[a-z]");

        private readonly TextWriter log;

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbZoneInfoCompiler" /> class
        /// logging to standard output.
        /// </summary>
        public TzdbZoneInfoCompiler() : this(Console.Out) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbZoneInfoCompiler" /> class
        /// logging to the given text writer, which may be null.
        /// </summary>
        public TzdbZoneInfoCompiler(TextWriter log)
        {
            this.log = log;
        }

        /// <summary>
        /// Tries to compile the contents of a path to a TzdbDatabase. The path can be a directory
        /// name containing the TZDB files, or a local tar.gz file, or a remote tar.gz file.
        /// The version ID is taken from the Makefile, if it exists. Otherwise, an attempt is made to guess
        /// it based on the last element of the path, to match a regex of \d{2,4}[a-z] (anywhere within the element).
        /// </summary>
        public TzdbDatabase Compile(string path)
        {
            var source = LoadSource(path);
            var version = InferVersion(source);
            var database = new TzdbDatabase(version);
            LoadZoneFiles(source, database);
            LoadLocationFiles(source, database);
            return database;
        }

        private void LoadZoneFiles(FileSource source, TzdbDatabase database)
        {
            var tzdbParser = new TzdbZoneInfoParser();
            foreach (var file in ZoneFiles)
            {
                if (source.Contains(file))
                {
                    log?.WriteLine("Parsing file {0} . . .", file);
                    using (var stream = source.Open(file))
                    {
                        tzdbParser.Parse(stream, database);
                    }
                }
            }
        }

        private void LoadLocationFiles(FileSource source, TzdbDatabase database)
        {
            if (!source.Contains(Iso3166TabFile))
            {
                return;
            }
            var iso3166 = source.ReadLines(Iso3166TabFile)
                  .Where(line => line != "" && !line.StartsWith("#"))
                  .Select(line => line.Split('\t'))
                  .ToList();
            if (source.Contains(ZoneTabFile))
            {
                var iso3166Dict = iso3166.ToDictionary(bits => bits[0], bits => bits[1]);
                database.ZoneLocations = source.ReadLines(ZoneTabFile)
                   .Where(line => line != "" && !line.StartsWith("#"))
                   .Select(line => TzdbZoneLocationParser.ParseLocation(line, iso3166Dict))
                   .ToList();
            }
            if (source.Contains(Zone1970TabFile))
            {
                var iso3166Dict = iso3166.ToDictionary(bits => bits[0], bits => new TzdbZone1970Location.Country(code: bits[0], name: bits[1]));
                database.Zone1970Locations = source.ReadLines(Zone1970TabFile)
                   .Where(line => line != "" && !line.StartsWith("#"))
                   .Select(line => TzdbZoneLocationParser.ParseEnhancedLocation(line, iso3166Dict))
                   .ToList();
            }
        }

        private FileSource LoadSource(string path)
        {
            if (path.StartsWith("ftp://") || path.StartsWith("http://") || path.StartsWith("https://"))
            {
                log?.WriteLine($"Downloading {path}");
                Uri uri = new Uri(path);
                using (HttpClient client = new HttpClient())
                {
                    // I know using .Result is nasty, but we're in a console app, and nothing is
                    // going to deadlock...
                    var data = client.GetAsync(path).Result.EnsureSuccessStatusCode().Content.ReadAsByteArrayAsync().Result;
                    log?.WriteLine($"Compiling from archive");
                    return FileSource.FromArchive(new MemoryStream(data), uri.AbsolutePath);
                }
            }
            if (Directory.Exists(path))
            {
                log?.WriteLine($"Compiling from directory {path}");
                return FileSource.FromDirectory(path);
            }
            else
            {
                log?.WriteLine($"Compiling from archive file {path}");
                using (var file = File.OpenRead(path))
                {
                    return FileSource.FromArchive(file, path);
                }
            }
        }

        private string InferVersion(FileSource source)
        {
            if (source.Contains(Makefile))
            {
                foreach (var line in source.ReadLines(Makefile))
                {
                    if (Regex.IsMatch(line, @"VERSION=\d{4}.*"))
                    {
                        var version = line.Substring(8).Trim();
                        log?.WriteLine($"Inferred version {version} from {Makefile}");
                        return version;
                    }
                }
            }
            var match = VersionRegex.Match(source.Origin);
            if (match.Success)
            {
                var version = match.Value;
                log?.WriteLine($"Inferred version {version} from file/directory name {source.Origin}");
                return version;
            }
            throw new InvalidDataException("Unable to determine TZDB version from source");
        }
    }
}
