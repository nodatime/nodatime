#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
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
using System.IO;
using System.Linq;
using NodaTime.Text;

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
    // TODO(Post-V1): Include the ability to dump a single zone
    // TODO(Post-V1): Include the ability to parse the output of JodaDump and just validate it
    // TODO(Post-V1): Include the ability to specify a resx file or possibly even a tzdb data directory
    public class Program
    {
        private static readonly Instant Start = Instant.FromUtc(1900, 1, 1, 0,  0);
        private static readonly Instant End = Instant.FromUtc(2050, 1, 1, 0, 0);

        private static readonly InstantPattern DateTimePattern
            = InstantPattern.CreateWithInvariantInfo("yyyy-MM-dd HH:mm:ss");

        private static readonly OffsetPattern OffsetPattern
            = OffsetPattern.CreateWithInvariantInfo("+HH:mm:ss");

        private static void Main()
        {
            // Ensure it really is a culture-insensitive ordinal ordering.
            foreach (var id in DateTimeZoneProviders.Default.Ids.OrderBy(x => x, StringComparer.Ordinal))
            {
                DumpZone(DateTimeZoneProviders.Default[id], Console.Out);
            }
        }

        private static void DumpZone(DateTimeZone zone, TextWriter output)
        {
            output.WriteLine(zone.Id);

            Instant instant = Start;
            while (instant < End)
            {
                var interval = zone.GetZoneInterval(instant);
                output.WriteLine("{0}  {1}  {2}",
                    DateTimePattern.Format(instant),
                    OffsetPattern.Format(interval.StandardOffset),
                    OffsetPattern.Format(interval.Savings));
                instant = interval.End;
            }

            output.WriteLine();
        }
    }
}
