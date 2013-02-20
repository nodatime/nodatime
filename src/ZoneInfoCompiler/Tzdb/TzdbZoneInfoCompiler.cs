// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NodaTime.TimeZones;
using NodaTime.Utility;

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
            ParseGeoLocations(sourceDirectory, database);
            database.LogCounts();
            return database;
        }

        /// <summary>
        /// Parses all of the given files.
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
        /// Attempts to parse geolocations from zone.tab and iso3166.tab, storing the results in the database.
        /// If the files are not both present, the database's geolocation list will be cleared.
        /// </summary>
        private static void ParseGeoLocations(DirectoryInfo sourceDirectory, TzdbDatabase database)
        {
            var iso3166File = Path.Combine(sourceDirectory.FullName, "iso3166.tab");
            var zoneFile = Path.Combine(sourceDirectory.FullName, "zone.tab");
            if (!File.Exists(iso3166File) || !File.Exists(zoneFile))
            {
                Console.WriteLine("Geo-location files missing; skipping");
                database.GeoLocations = null;
                return;
            }
            var iso3166 = File.ReadAllLines(iso3166File)
                              .Where(line => line != "" && !line.StartsWith("#"))
                              .Select(line => line.Split('\t'))
                              .ToDictionary(bits => bits[0], bits => bits[1]);
            database.GeoLocations = File.ReadAllLines(zoneFile)
                                        .Where(line => line != "" && !line.StartsWith("#"))
                                        .Select(line => ConvertGeoLocation(line.Split('\t'), iso3166))
                                        .ToList();
        }

        // Internal for testing
        internal static TzdbGeoLocation ConvertGeoLocation(string[] bits, Dictionary<string, string> countryMapping)
        {
            Preconditions.CheckArgument(bits.Length == 3 || bits.Length == 4, "bits", "Line must have 3 or 4 values");
            string countryCode = bits[0];
            string countryName = countryMapping[countryCode];
            int[] latLong = ConvertLatLong(bits[1]);
            string zoneId = bits[2];
            string comment = bits.Length == 4 ? bits[3] : "";
            return new TzdbGeoLocation(latLong[0], latLong[1], countryName, countryCode, zoneId, comment);
        }

        private static int[] ConvertLatLong(string coordinates)
        {
            Preconditions.CheckArgument(coordinates.Length == 11 || coordinates.Length == 15, "point", "Invalid coordinates");
            int latDegrees;
            int latMinutes;
            int latSeconds = 0;
            int latSign;
            int longDegrees;
            int longMinutes;
            int longSeconds = 0;
            int longSign;

            if (coordinates.Length == 11 /* +-DDMM+-DDDMM */)
            {
                latSign = coordinates[0] == '-' ? -1 : 1;
                latDegrees = int.Parse(coordinates.Substring(1, 2));
                latMinutes = int.Parse(coordinates.Substring(3, 2));
                longSign = coordinates[5] == '-' ? -1 : 1;
                longDegrees = int.Parse(coordinates.Substring(6, 3));
                longMinutes = int.Parse(coordinates.Substring(9, 2));
            }
            else /* +-DDMMSS+-DDDMMSS */
            {
                latSign = coordinates[0] == '-' ? -1 : 1;
                latDegrees = int.Parse(coordinates.Substring(1, 2));
                latMinutes = int.Parse(coordinates.Substring(3, 2));
                latSeconds = int.Parse(coordinates.Substring(5, 2));
                longSign = coordinates[7] == '-' ? -1 : 1;
                longDegrees = int.Parse(coordinates.Substring(8, 3));
                longMinutes = int.Parse(coordinates.Substring(11, 2));
                longSeconds = int.Parse(coordinates.Substring(13, 2));
            }
            return new[] {
                latSign * (latDegrees * 3600 + latMinutes * 60 + latSeconds),
                longSign * (longDegrees * 3600 + longMinutes * 60 + longSeconds)
            };
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
