// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
#if !NETCORE
using BenchmarkDotNet.Attributes;
using NodaTime.Benchmarks.BclTests;
using NodaTime.TimeZones;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    public class BclDateTimeZoneBenchmarks
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
#endif