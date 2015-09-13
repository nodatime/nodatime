// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NodaTime.TimeZones;
using NodaTime.TzdbCompiler.Tzdb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

            List<DateTimeZone> zones = LoadSource(options);
            zones = zones.OrderBy(zone => zone.Id, StringComparer.Ordinal).ToList();

            if (options.ZoneId != null)
            {
                var zone = zones.FirstOrDefault(z => z.Id == options.ZoneId);
                if (zone == null)
                {
                    throw new Exception($"Unknown zone ID: {options.ZoneId}");
                }
                DumpZone(zone, options);
            }
            else
            {
                foreach (var zone in zones)
                {
                    DumpZone(zone, options);
                    Console.Write("\r\n");
                }
            }

            return 0;
        }

        private static void DumpZone(DateTimeZone zone, Options options)
        {
            Console.Write("{0}\r\n", zone.Id);
            var initial = zone.GetZoneInterval(Instant.MinValue);
            Console.Write("Initially:           {0} {1} {2}\r\n",
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

        private static List<DateTimeZone> LoadSource(Options options)
        {
            var source = options.Source;
            if (source == null)
            {
                var provider = DateTimeZoneProviders.Tzdb;
                return provider.Ids.Select(id => provider[id]).ToList();
            }
            if (source.EndsWith(".nzd"))
            {
                var data = LoadFileOrUrl(source);
                var tzdbSource = TzdbDateTimeZoneSource.FromStream(new MemoryStream(data));
                return tzdbSource.GetIds().Select(id => tzdbSource.ForId(id)).ToList();
            }
            else
            {
                var compiler = new TzdbZoneInfoCompiler(log: null);
                var database = compiler.Compile(source);
                return database.GenerateDateTimeZones()
                    .Concat(database.Aliases.Keys.Select(database.GenerateDateTimeZone))
                    .ToList();
            }
        }

        private static byte[] LoadFileOrUrl(string source)
        {
            if (source.StartsWith("http://") || source.StartsWith("https://") || source.StartsWith("ftp://"))
            {
                using (var client = new WebClient())
                {
                    return client.DownloadData(source);
                }
            }
            return File.ReadAllBytes(source);
        }        
    }
}
