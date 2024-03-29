﻿// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using NodaTime.TimeZones;
using NodaTime.Tools.Common;
using NodaTime.TzdbCompiler.Tzdb;
using System;
using System.IO;
using System.Threading.Tasks;

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
        private static async Task<int> Main(string[] args)
        {
            Options options = new Options();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(args, options))
            {
                return 1;
            }
            // Not null after we've parsed the arguments.
            TzdbDateTimeZoneSource source = await LoadSourceAsync(options.Source);
            var dumper = new ZoneDumper(source, options);
            try
            {
                using (var writer = options.OutputFile is null ? Console.Out : File.CreateText(options.OutputFile))
                {
                    dumper.Dump(writer);
                }
            }
            catch (UserErrorException e)
            {
                Console.Error.WriteLine($"Error: {e.Message}");
                return 1;
            }

            return 0;
        }

        private static async Task<TzdbDateTimeZoneSource> LoadSourceAsync(string? source)
        {
            if (source is null)
            {
                return TzdbDateTimeZoneSource.Default;
            }
            if (source.EndsWith(".nzd"))
            {
                var data = await FileUtility.LoadFileOrUrlAsync(source);
                return TzdbDateTimeZoneSource.FromStream(new MemoryStream(data));
            }
            else
            {
                var compiler = new TzdbZoneInfoCompiler(log: null);
                var database = await compiler.CompileAsync(source);
                return database.ToTzdbDateTimeZoneSource();
            }
        }
    }
}
