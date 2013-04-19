// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using NodaTime.TimeZones;
using NodaTime.TimeZones.Cldr;
using NodaTime.TzdbCompiler.Tzdb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;

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
        private static readonly Dictionary<OutputType, string> Extensions = new Dictionary<OutputType, string>
        {
            { OutputType.Resource, "resources" },
            { OutputType.ResX, "resx" },
            { OutputType.NodaZoneData, "nzd" },
        };

        /// <summary>
        /// Runs the compiler from the command line.
        /// </summary>
        /// <param name="arguments">The command line arguments. Each compiler defines its own.</param>
        /// <returns>0 for success, non-0 for error.</returns>
        private static int Main(string[] arguments)
        {
            CompilerOptions options = new CompilerOptions();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(arguments, options))
            {
                return 1;
            }

            var writer = CreateWriter(options);
            var tzdbCompiler = new TzdbZoneInfoCompiler();
            var tzdb = tzdbCompiler.Compile(options.SourceDirectoryName);
            tzdb.LogCounts();
            var windowsZones = CldrWindowsZonesParser.Parse(options.WindowsMappingFile);
            LogWindowsZonesSummary(windowsZones);
            writer.Write(tzdb, windowsZones);

            Console.WriteLine("Reading generated data and validating...");
            var source = Read(options);
            source.Validate();
            return 0;
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

        private static ITzdbWriter CreateWriter(CompilerOptions options)
        {
            string file = Path.ChangeExtension(options.OutputFileName, Extensions[options.OutputType]);
            switch (options.OutputType)
            {
                case OutputType.ResX:
                    return new TzdbResourceWriter(new ResXResourceWriter(file));
                case OutputType.Resource:
                    return new TzdbResourceWriter(new ResourceWriter(file));
                case OutputType.NodaZoneData:
                    return new TzdbStreamWriter(File.Create(file));
                default:
                    throw new ArgumentException("Invalid output type: " + options.OutputType, "options");
            }
        }

        private static TzdbDateTimeZoneSource Read(CompilerOptions options)
        {
#pragma warning disable 0618
            string file = Path.ChangeExtension(options.OutputFileName, Extensions[options.OutputType]);
            switch (options.OutputType)
            {
                case OutputType.ResX:
                    return new TzdbDateTimeZoneSource(new ResourceSet(new ResXResourceReader(file)));
                case OutputType.Resource:
                    return new TzdbDateTimeZoneSource(new ResourceSet(new ResourceReader(file)));
                case OutputType.NodaZoneData:
                    using (var stream = File.OpenRead(file))
                    {
                        return TzdbDateTimeZoneSource.FromStream(stream);
                    }
                default:
                    throw new ArgumentException("Invalid output type: " + options.OutputType, "options");
            }
#pragma warning restore 0618
        }
    }
}
