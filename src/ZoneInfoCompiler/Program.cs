#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using CommandLine;
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
            { OutputType.NodaResource, "nodaresources" },
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
            var windowsMapping = WindowsMapping.Parse(options.WindowsMappingFile);
            writer.Write(tzdb, windowsMapping);
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
                // TODO: Change this to a different ITzdbWriter implementation (not resources)
                case OutputType.NodaResource:
                    return new TzdbResourceWriter(new NodaResourceWriter(file));
                default:
                    throw new ArgumentException("Invalid output type: " + options.OutputType, "options");
            }
        }
    }
}
