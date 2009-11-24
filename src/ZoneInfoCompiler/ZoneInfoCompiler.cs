#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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

using NodaTime.ZoneInfoCompiler.Tzdb;

namespace NodaTime.ZoneInfoCompiler
{
    /// <summary>
    /// Main entry point for the time zone information compiler. In theory we could support
    /// multiple sources and formats but currently we only support one:
    /// http://www.twinsun.com/tz/tz-link.htm. This system refers to it as TZDB.
    /// </summary>
    /// <remarks>
    /// Original name: ZoneInfoCompiler (in org.joda.time.tz)
    /// </remarks>
    internal sealed class ZoneInfoCompiler
    {
        /// <summary>
        /// Runs the compiler from the command line.
        /// </summary>
        /// <param name="args">The command line arguments. Each compiler defines its own.</param>
        /// <returns>0 for success, non-0 for error.</returns>
        static int Main(string[] args)
        {
            var log = new ConsoleLog();
            TzdbZoneInfoCompiler compiler = new TzdbZoneInfoCompiler(log);
            int result = compiler.Execute(args);
            return result;
        }
    }
}
