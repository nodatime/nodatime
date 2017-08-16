// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using BenchmarkDotNet.Attributes;

namespace NodaTime.Benchmarks.BclTests
{
    [Category("BCL")]
    public class TimeZoneInfoBenchmarks
    {
        internal static readonly TimeZoneInfo PacificZone = GetPacificTime();

        private static TimeZoneInfo GetPacificTime()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            }
#if NETCORE
            catch (Exception) // TimeZoneNotFoundException doesn't exist in netstandard :(
#else
            catch (TimeZoneNotFoundException)
#endif
            {
                // Maybe we're running on Mono on a system that uses TZDB natively.
                return TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
            }
        }

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

#if !NETCORE
        [Benchmark]
        public void ConvertLocalToUtc()
        {
            TimeZoneInfo.ConvertTimeToUtc(SummerUnspecified, PacificZone);
        }

        [Benchmark]
        public void ConvertUtcToLocal()
        {
            TimeZoneInfo.ConvertTimeFromUtc(SummerUtc, PacificZone);
        }
#endif
    }
}
