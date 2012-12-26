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

using System;
using CommandLine;
using NodaTime.ZoneInfoCompiler.Tzdb;
using NodaTime.ZoneInfoCompiler.winmap;

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
    internal sealed class ZoneInfoCompiler
    {
        /// <summary>
        /// Runs the compiler from the command line.
        /// </summary>
        /// <param name="arguments">The command line arguments. Each compiler defines its own.</param>
        /// <returns>0 for success, non-0 for error.</returns>
        private static int Main(string[] arguments)
        {
            TzdbCompilerOptions options = new TzdbCompilerOptions();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(arguments, options))
            {
                return 1;
            }

            using (var output = new ResourceOutput(options.OutputFileName, options.OutputType))
            {
                var tzdbCompiler = new TzdbZoneInfoCompiler();
                int ret = tzdbCompiler.Execute(options.SourceDirectoryName, output);
                if (ret != 0)
                {
                    return ret;
                }
                var mapperCompiler = new WindowsMapperCompiler();
                return mapperCompiler.Execute(options.WindowsMappingFile, output);
            }
        }
    }
}
