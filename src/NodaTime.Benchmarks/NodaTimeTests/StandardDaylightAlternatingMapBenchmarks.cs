// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using NodaTime.TimeZones;

#if !NO_INTERNALS
namespace NodaTime.Benchmarks.NodaTimeTests
{
    /// <summary>
    /// Benchmarks for operations on an uncached StandardDaylightAlternatingMap.
    /// </summary>
    public class StandardDaylightAlternatingMapBenchmarks
    {
        private static readonly Instant February1st = Instant.FromUtc(2010, 2, 1, 0, 0);
        private static readonly Instant July1st = Instant.FromUtc(2010, 7, 1, 0, 0);

        private static readonly StandardDaylightAlternatingMap SampleZone;

        static StandardDaylightAlternatingMapBenchmarks()
        {
            // Build a daylight savings zone which basically models modern America/Los_Angeles.
            SampleZone = new StandardDaylightAlternatingMap(Offset.FromHours(-8),
                new ZoneRecurrence("PDT", Offset.FromHours(1),
                    new ZoneYearOffset(TransitionMode.Wall, monthOfYear: 3, dayOfMonth: 8, dayOfWeek: 7,
                                       advance: true, timeOfDay: new LocalTime(2, 0)),
                    int.MinValue, int.MaxValue),
                new ZoneRecurrence("PST", Offset.FromHours(0),
                    new ZoneYearOffset(TransitionMode.Standard, monthOfYear: 11, dayOfMonth: 1, dayOfWeek: 7,
                                       advance: true, timeOfDay: new LocalTime(2, 0)),
                    int.MinValue, int.MaxValue));
        }

        [Benchmark]
        public ZoneInterval GetZoneInterval_Winter() => SampleZone.GetZoneInterval(February1st);

        [Benchmark]
        public ZoneInterval GetZoneInterval_Summer() => SampleZone.GetZoneInterval(July1st);
    }
}
#endif
