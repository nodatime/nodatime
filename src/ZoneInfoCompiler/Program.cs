// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using NodaTime.TimeZones.Cldr;
using NodaTime.ZoneInfoCompiler.Tzdb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;

namespace NodaTime.ZoneInfoCompiler
{
    /// <summary>
    /// Main entry point for the time zone information compiler. In theory we could support
    /// multiple sources and formats but currently we only support one:
    /// http://www.twinsun.com/tz/tz-link.htm. This system refers to it as TZDB.
    /// This also requires a windowsZone.xml file from the Unicode CLDR repository, to
    /// map Windows time zone names to TZDB IDs.
    /// </summary>
    /// <remarks>
    /// Original name: ZoneInfoCompiler (in org.joda.time.tz)
    /// </remarks>
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
            var windowsZones = CldrWindowsZonesParser.Parse(options.WindowsMappingFile);
            writer.Write(tzdb, windowsZones);
            return 0;
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
    }
}
