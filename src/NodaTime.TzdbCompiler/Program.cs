// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using NodaTime.Extensions;
using NodaTime.TimeZones;
using NodaTime.TimeZones.Cldr;
using NodaTime.TzdbCompiler.Tzdb;
using System;
using System.IO;
using System.Linq;

namespace NodaTime.TzdbCompiler
{
    /// <summary>
    /// Main entry point for the time zone information compiler. In theory we could support
    /// multiple sources and formats but currently we only support one:
    /// http://www.twinsun.com/tz/tz-link.htm. This system refers to it as TZDB.
    /// This also requires a windowsZone.xml file from the Unicode CLDR repository, to
    /// map Windows time zone names to TZDB IDs.
    /// </summary>
    internal sealed class Program
    {
        /// <summary>
        /// Runs the compiler from the command line.
        /// </summary>
        /// <param name="arguments">The command line arguments. Each compiler defines its own.</param>
        /// <returns>0 for success, non-0 for error.</returns>
        private static int Main(string[] arguments)
        {
            CompilerOptions options = new CompilerOptions();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error) { MutuallyExclusive = true });
            if (!parser.ParseArguments(arguments, options))
            {
                return 1;
            }

            var tzdbCompiler = new TzdbZoneInfoCompiler();
            var tzdb = tzdbCompiler.Compile(options.SourceDirectoryName);
            tzdb.LogCounts();
            if (options.ZoneId != null)
            {
                tzdb.GenerateDateTimeZone(options.ZoneId);
                return 0;
            }
            var windowsZones = LoadWindowsZones(options, tzdb.Version);
            LogWindowsZonesSummary(windowsZones);
            var writer = CreateWriter(options);
            writer.Write(tzdb, windowsZones);

            if (options.OutputFileName != null)
            {
                Console.WriteLine("Reading generated data and validating...");
                var source = Read(options);
                source.Validate();
                if (options.TextDumpFile != null)
                {
                    CreateTextDump(source, options.TextDumpFile);
                }
            }
            return 0;
        }

        /// <summary>
        /// Loads the best windows zones file based on the options. If the WindowsMapping option is
        /// just a straight file, that's used. If it's a directory, this method loads all the XML files
        /// in the directory (expecting them all to be mapping files) and then picks the best one based
        /// on the version of TZDB we're targeting - basically, the most recent one before or equal to the
        /// target version.
        /// </summary>
        private static WindowsZones LoadWindowsZones(CompilerOptions options, string targetTzdbVersion)
        {
            var mappingPath = options.WindowsMapping;
            if (File.Exists(mappingPath))
            {
                return CldrWindowsZonesParser.Parse(mappingPath);
            }
            if (!Directory.Exists(mappingPath))
            {
                throw new Exception($"{mappingPath} does not exist as either a file or a directory");
            }
            var xmlFiles = Directory.GetFiles(mappingPath, "*.xml");
            if (xmlFiles.Length == 0)
            {
                throw new Exception($"{mappingPath} does not contain any XML files");
            }
            var allFiles = xmlFiles
                .Select(file => CldrWindowsZonesParser.Parse(file))
                .OrderByDescending(zones => zones.TzdbVersion)
                .ToList();

            var versions = string.Join(", ", allFiles.Select(z => z.TzdbVersion).ToArray());

            var bestFile = allFiles
                .Where(zones => StringComparer.Ordinal.Compare(zones.TzdbVersion, targetTzdbVersion) <= 0)
                .FirstOrDefault();

            if (bestFile == null)
            {
                throw new Exception($"No zones files suitable for version {targetTzdbVersion}. Found versions targeting: [{versions}]");
            }
            Console.WriteLine($"Picked Windows Zones with TZDB version {bestFile.TzdbVersion} out of [{versions}] as best match for {targetTzdbVersion}");
            return bestFile;
        }

        private static void LogWindowsZonesSummary(WindowsZones windowsZones)
        {
            Console.WriteLine("Windows Zones:");
            Console.WriteLine("  Version: {0}", windowsZones.Version);
            Console.WriteLine("  TZDB version: {0}", windowsZones.TzdbVersion);
            Console.WriteLine("  Windows version: {0}", windowsZones.WindowsVersion);
            Console.WriteLine("  {0} MapZones", windowsZones.MapZones.Count);
            Console.WriteLine("  {0} primary mappings", windowsZones.PrimaryMapping.Count);
        }

        private static TzdbStreamWriter CreateWriter(CompilerOptions options)
        {
            // If we don't have an actual file, just write to an empty stream.
            // That way, while debugging, we still get to see all the data written etc.
            if (options.OutputFileName == null)
            {
                return new TzdbStreamWriter(new MemoryStream());
            }
            string file = Path.ChangeExtension(options.OutputFileName, "nzd");
            return new TzdbStreamWriter(File.Create(file));
        }

        private static TzdbDateTimeZoneSource Read(CompilerOptions options)
        {
#pragma warning disable 0618
            string file = Path.ChangeExtension(options.OutputFileName, "nzd");
            using (var stream = File.OpenRead(file))
            {
                return TzdbDateTimeZoneSource.FromStream(stream);
            }
#pragma warning restore 0618
        }

        private static void CreateTextDump(TzdbDateTimeZoneSource source, string file)
        {
            var provider = new DateTimeZoneCache(source);
            using (var writer = File.CreateText(file))
            {
                provider.Dump(writer);
            }
        }
    }
}
