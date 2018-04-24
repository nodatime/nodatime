// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using NodaTime.TimeZones;
using NodaTime.Tools.Common;
using System;
using System.IO;
using System.Net.Http;

namespace NodaTime.TzValidate.NzdCompatibility
{
    /// <summary>
    /// Based on NodaTime.TzValidate.NodaDump, this checks that Noda Time 1.1 can read an
    /// NZD file and produce the same result as the most recent version.
    /// </summary>
    internal class Program
    {
        private static int Main(string[] args)
        {
            Options options = new Options();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error) { MutuallyExclusive = true });
            if (!parser.ParseArguments(args, options))
            {
                return 1;
            }
            var data = FileUtility.LoadFileOrUrl(options.Source);
            TzdbDateTimeZoneSource source = TzdbDateTimeZoneSource.FromStream(new MemoryStream(data));
            var dumper = new ZoneDumper(source, options);
            try
            {
                using (var writer = options.OutputFile == null ? Console.Out : File.CreateText(options.OutputFile))
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
    }
}
