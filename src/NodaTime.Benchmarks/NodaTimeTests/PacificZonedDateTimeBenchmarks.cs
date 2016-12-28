// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    public class PacificZonedDateTimeBenchmarks
    {
        private static readonly DateTimeZone Pacific = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
        private static readonly LocalDateTime SampleLocal = new LocalDateTime(2009, 12, 26, 10, 8, 30);
        private static readonly ZonedDateTime SampleZoned = Pacific.AtStrictly(SampleLocal);

        [Benchmark]
        public ZonedDateTime Construction() => Pacific.AtStrictly(SampleLocal);

        [Benchmark]
        public int Year() => SampleZoned.Year;

        [Benchmark]
        public int Month() => SampleZoned.Month;

        [Benchmark]
        public int DayOfMonth() => SampleZoned.Day;

        [Benchmark]
        public IsoDayOfWeek DayOfWeek() => SampleZoned.DayOfWeek;

        [Benchmark]
        public int DayOfYear() => SampleZoned.DayOfYear;

        [Benchmark]
        public int Hour() => SampleZoned.Hour;

        [Benchmark]
        public int Minute() => SampleZoned.Minute;

        [Benchmark]
        public int Second() => SampleZoned.Second;

        [Benchmark]
        public int Millisecond() => SampleZoned.Millisecond;

        [Benchmark]
        public Instant ToInstant() => SampleZoned.ToInstant();
    }
}