// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using NodaTime.TimeZones;

namespace NodaTime.Benchmarks.NodaTimeTests.TimeZones
{
    public class StandardDaylightAlternatingMapBenchmarks
    {
        private static readonly StandardDaylightAlternatingMap SampleMap = new StandardDaylightAlternatingMap(Offset.FromHours(5),
            new ZoneRecurrence("Summer", Offset.FromHours(1), new ZoneYearOffset(TransitionMode.Wall, 3, 15, (int)IsoDayOfWeek.Sunday, true, new LocalTime(1, 0, 0)), int.MinValue, int.MaxValue),
            new ZoneRecurrence("Winter", Offset.Zero, new ZoneYearOffset(TransitionMode.Wall, 9, 15, (int)IsoDayOfWeek.Sunday, true, new LocalTime(1, 0, 0)), int.MinValue, int.MaxValue));

        private static readonly Instant WinterInstant = Instant.FromUtc(2010, 2, 4, 5, 10);
        private static readonly Instant SummerInstant = Instant.FromUtc(2010, 6, 19, 5, 10);

        [Benchmark]
        public ZoneInterval GetZoneInterval_InWinter() => SampleMap.GetZoneInterval(WinterInstant);

        [Benchmark]
        public ZoneInterval GetZoneInterval_InSummer() => SampleMap.GetZoneInterval(SummerInstant);
    }
}
