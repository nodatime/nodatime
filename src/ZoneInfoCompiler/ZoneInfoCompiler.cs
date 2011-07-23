#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
using NodaTime.TimeZones;
using NodaTime.ZoneInfoCompiler.Tzdb;
using NodaTime.ZoneInfoCompiler.winmap;

namespace NodaTime.ZoneInfoCompiler
{
    /// <summary>
    ///   Main entry point for the time zone information compiler. In theory we could support
    ///   multiple sources and formats but currently we only support one:
    ///   http://www.twinsun.com/tz/tz-link.htm. This system refers to it as TZDB.
    /// </summary>
    /// <remarks>
    ///   Original name: ZoneInfoCompiler (in org.joda.time.tz)
    /// </remarks>
    internal sealed class ZoneInfoCompiler
    {
        /// <summary>
        ///   Runs the compiler from the command line.
        /// </summary>
        /// <param name = "arguments">The command line arguments. Each compiler defines its own.</param>
        /// <returns>0 for success, non-0 for error.</returns>
        private static int Main(string[] arguments)
        {
            int result;
            var log = new ConsoleLog();
            if (arguments.Length < 1)
            {
                result = Usage(log);
            }
            else
            {
                var command = arguments[0];
                var remainingArguments = new string[arguments.Length - 1];
                Array.ConstrainedCopy(arguments, 1, remainingArguments, 0, remainingArguments.Length);
                if (command.Equals("tzdb", StringComparison.OrdinalIgnoreCase))
                {
                    var compiler = new TzdbZoneInfoCompiler(log);
                    result = compiler.Execute(remainingArguments);
                }
                else if (command.Equals("winmap", StringComparison.OrdinalIgnoreCase))
                {
                    var compiler = new WindowsMapperCompiler(log);
                    result = compiler.Execute(remainingArguments);
                }
                else
                {
                    log.Error("Unknown comamnd: {0}", command);
                    result = Usage(log);
                }
            }
            return result;
        }

        /// <summary>
        ///   Usages the specified log.
        /// </summary>
        /// <param name = "log">The log.</param>
        /// <returns></returns>
        private static int Usage(ILog log)
        {
            log.Info("");
            log.Info("Usage: command [ options ]");
            log.Info("");
            log.Info("where command is one of:");
            log.Info("   tzdb          Build a TZDB resource file");
            log.Info("   winmap        Build a Windows to Posix mapping file");
            log.Info("");
            return 1;
        }
    }
}
