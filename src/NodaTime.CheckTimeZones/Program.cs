// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.IO;
using System.Linq;
using CommandLine;
using NodaTime.Text;
using NodaTime.TimeZones;

namespace NodaTime.CheckTimeZones
{
    /// <summary>
    /// Diagnostic program to dump time zone transitions between 1900 and 2050, or compare one set
    /// with another (e.g. from Joda Time).
    /// </summary>
    /// <remarks>
    /// Currently, there's just code to dump both sets of transitions (for all zones) - you can then
    /// use a diffing tool to see the changes (or lack thereof).
    /// </remarks>
    public class Program
    {
        private static readonly InstantPattern DateTimePattern
            = InstantPattern.CreateWithInvariantCulture("yyyy-MM-dd HH:mm:ss");

        private static readonly OffsetPattern OffsetPattern
            = OffsetPattern.CreateWithInvariantCulture("+HH:mm:ss");

        private static int Main(string[] args)
        {
            Options options = new Options();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(args, options))
            {
                return 1;
            }
            if (options.FromYear > options.ToYear)
            {
                Console.WriteLine("Error: 'from' year must be not be later than 'to' year");
                return 1;
            }
            IDateTimeZoneSource source = TzdbDateTimeZoneSource.Default;

            if (options.File != null)
            {
                using (var stream = File.OpenRead(options.File))
                {
                    source = TzdbDateTimeZoneSource.FromStream(stream);
                }
            }
            Console.WriteLine("TZDB version: {0}", source.VersionId);

            var provider = new DateTimeZoneCache(source);

            if (options.Zone != null)
            {
                var zone = provider.GetZoneOrNull(options.Zone);
                if (zone == null)
                {
                    Console.WriteLine("Unknown time zone: {0}", options.Zone);
                    return 1;
                }
                DumpZone(zone, Console.Out, options.FromYear, options.ToYear);
            }
            else
            {
                foreach (var id in provider.Ids)
                {
                    DumpZone(provider[id], Console.Out, options.FromYear, options.ToYear);
                }
            }
            return 0;
        }

        private static void DumpZone(DateTimeZone zone, TextWriter output, int fromYear, int toYear)
        {
            output.WriteLine(zone.Id);
            var start = Instant.FromUtc(fromYear, 1, 1, 0, 0);
            // Exclusive upper bound
            var end = Instant.FromUtc(toYear + 1, 1, 1, 0, 0);
            
            foreach (var interval in zone.GetZoneIntervals(start, end))
            {
               output.WriteLine("{0}  {1}  {2}",
                    DateTimePattern.Format(Instant.Max(start, interval.Start)),
                    OffsetPattern.Format(interval.StandardOffset),
                    OffsetPattern.Format(interval.Savings));
            }

            output.WriteLine();
        }
    }
}
