// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NodaTime.TimeZones;
using NodaTime.TzdbCompiler.Tzdb;
using SharpCompress.Reader.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using NodaTime;
using System.Diagnostics;
using CommandLine;

namespace NodaTime.TzValidate.NodaDump
{
    /// <summary>
    /// Utility to convert tz source data (or an nzd file) into the tzvalidate format, so that
    /// we can validate that Noda Time handles it the same way that zic does.
    /// This is performed using the same code that TzdbCompiler does, except without using
    /// any Windows zone mappings from CLDR.
    /// </summary>
    internal class Program
    {
        private static readonly Instant End = Instant.FromUtc(2035, 1, 1, 0, 0);
        private static readonly IPattern<Instant> InstantPattern = NodaTime.Text.InstantPattern.GeneralPattern;
        private static readonly IPattern<Offset> OffsetPattern = NodaTime.Text.OffsetPattern.CreateWithInvariantCulture("l");

        private static int Main(string[] args)
        {
            Options options = new Options();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error) { MutuallyExclusive = true });
            if (!parser.ParseArguments(args, options))
            {
                return 1;
            }

            List<DateTimeZone> zones = LoadSource(options.Source);
            zones = zones.OrderBy(zone => zone.Id, StringComparer.Ordinal).ToList();

            foreach (var zone in zones)
            {
                DumpZone(zone, options);
                Console.Write("\r\n");
            }

            return 0;
        }

        private static void DumpZone(DateTimeZone zone, Options options)
        {
            Console.Write("{0}\r\n", zone.Id);
            var initial = zone.GetZoneInterval(Instant.MinValue);
            Console.Write("Initially: {0} {1} {2}\r\n",
                OffsetPattern.Format(initial.WallOffset),
                initial.Savings != Offset.Zero ? "daylight" : "standard",
                initial.Name);
            foreach (var zoneInterval in zone.GetZoneIntervals(options.Start, options.End)
                .Where(zi => zi.HasStart && zi.Start >= options.Start))
            {
                Console.Write("{0} {1} {2} {3}\r\n",
                    InstantPattern.Format(zoneInterval.Start),
                    OffsetPattern.Format(zoneInterval.WallOffset),
                    zoneInterval.Savings != Offset.Zero ? "daylight" : "standard",
                    zoneInterval.Name);
            }
        }

        private static List<DateTimeZone> LoadSource(string source)
        {
            if (source.StartsWith("http://") || source.StartsWith("https://"))
            {
                // Remote file - could still be nzd or a gzipped tar file.
                using (var client = new WebClient())
                {
                    return LoadFileSource(source, client.DownloadData(source));
                }
            }
            else
            {
                // Local file or directory...
                if (Directory.Exists(source))
                {
                    return LoadDirectory(source);
                }
                return LoadFileSource(source, File.ReadAllBytes(source));
            }
        }

        private static List<DateTimeZone> LoadFileSource(string source, byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            // Unfortunately, we don't have a magic number at the start of nzd files.
            // Oh well.
            if (source.EndsWith(".nzd"))
            {
                var tzdbSource = TzdbDateTimeZoneSource.FromStream(stream);
                return tzdbSource.GetIds().Select(id => tzdbSource.ForId(id)).ToList();
            }
            else
            {
                TarReader reader = TarReader.Open(stream);
                List<byte[]> tzSources = new List<byte[]>();
                while (reader.MoveToNextEntry())
                {
                    if (TzdbZoneInfoCompiler.IncludedFiles.Contains(reader.Entry.Key))
                    {
                        var memoryStream = new MemoryStream();
                        using (var tarStream = reader.OpenEntryStream())
                        {
                            tarStream.CopyTo(memoryStream);
                        }
                        tzSources.Add(memoryStream.ToArray());
                    }
                }
                return LoadTzSources(tzSources);
            }
        }

        private static List<DateTimeZone> LoadDirectory(string directory)
        {
            return LoadTzSources(TzdbZoneInfoCompiler
                .IncludedFiles
                .Select(file => Path.Combine(directory, file))
                .Where(File.Exists)
                .Select(File.ReadAllBytes));
        }

        private static List<DateTimeZone> LoadTzSources(IEnumerable<byte[]> sources)
        {
            var parser = new TzdbZoneInfoParser();
            var database = new TzdbDatabase(version: "ignored");
            foreach (var source in sources)
            {
                parser.Parse(new MemoryStream(source), database);
            }
            return database.GenerateDateTimeZones()
                .Concat(database.Aliases.Keys.Select(database.GenerateDateTimeZone))
                .ToList();            
        }

    }
}
