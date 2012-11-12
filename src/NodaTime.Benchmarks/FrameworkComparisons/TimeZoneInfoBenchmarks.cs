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
using NodaTime.Benchmarks.Timing;

namespace NodaTime.Benchmarks.FrameworkComparisons
{
    internal sealed class TimeZoneInfoBenchmarks
    {
        internal static readonly TimeZoneInfo PacificZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

        internal static readonly DateTime SummerUtc = new DateTime(1976, 6, 19, 0, 0, 0, DateTimeKind.Utc);
        internal static readonly DateTime WinterUtc = new DateTime(2003, 12, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime SummerUnspecified = DateTime.SpecifyKind(SummerUtc, DateTimeKind.Unspecified);
        private static readonly DateTime WinterUnspecified = DateTime.SpecifyKind(WinterUtc, DateTimeKind.Unspecified);
        private static readonly DateTimeOffset SummerOffset = new DateTimeOffset(SummerUnspecified, TimeSpan.FromHours(5));
        private static readonly DateTimeOffset WinterOffset = new DateTimeOffset(WinterUnspecified, TimeSpan.FromHours(5));

        [Benchmark]
        public void GetUtcOffset_Utc()
        {
            PacificZone.GetUtcOffset(SummerUtc);
            PacificZone.GetUtcOffset(WinterUtc);
        }

        [Benchmark]
        public void GetUtcOffset_Unspecified()
        {
            PacificZone.GetUtcOffset(SummerUnspecified);
            PacificZone.GetUtcOffset(WinterUnspecified);
        }

        [Benchmark]
        public void GetUtcOffset_DateTimeOffset()
        {
            PacificZone.GetUtcOffset(SummerOffset);
            PacificZone.GetUtcOffset(WinterOffset);
        }
    }
}
