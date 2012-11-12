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

using NodaTime.Benchmarks.FrameworkComparisons;
using NodaTime.Benchmarks.Timing;
using NodaTime.TimeZones;

namespace NodaTime.Benchmarks
{
    internal sealed class BclDateTimeZoneBenchmarks
    {
        private static readonly DateTimeZone PacificZone = BclDateTimeZone.FromTimeZoneInfo(TimeZoneInfoBenchmarks.PacificZone);
        private static readonly Instant SummerInstant = Instant.FromDateTimeUtc(TimeZoneInfoBenchmarks.SummerUtc);
        private static readonly Instant WinterInstant = Instant.FromDateTimeUtc(TimeZoneInfoBenchmarks.WinterUtc);

        // This is somewhat unfair due to caching, admittedly...
        [Benchmark]
        public void GetZoneInterval()
        {
            PacificZone.GetZoneInterval(SummerInstant);
            PacificZone.GetZoneInterval(WinterInstant);
        }
    }
}
