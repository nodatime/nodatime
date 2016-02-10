// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    [Config(typeof(BenchmarkConfig))]
    internal class PacificZonedDateTimeBenchmarks
    {
        private static readonly DateTimeZone Pacific = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
        private static readonly LocalDateTime SampleLocal = new LocalDateTime(2009, 12, 26, 10, 8, 30);
        private static readonly ZonedDateTime SampleZoned = Pacific.AtStrictly(SampleLocal);

        [Benchmark]
        public ZonedDateTime Construction()
        {
            return Pacific.AtStrictly(SampleLocal);
        }

        [Benchmark]
        public int Year()
        {
            return SampleZoned.Year;
        }

        [Benchmark]
        public int Month()
        {
            return SampleZoned.Month;
        }

        [Benchmark]
        public int DayOfMonth()
        {
            return SampleZoned.Day;
        }

        [Benchmark]
        public IsoDayOfWeek IsoDayOfWeek()
        {
            return SampleZoned.IsoDayOfWeek;
        }

        [Benchmark]
        public int DayOfYear()
        {
            return SampleZoned.DayOfYear;
        }

        [Benchmark]
        public int Hour()
        {
            return SampleZoned.Hour;
        }

        [Benchmark]
        public int Minute()
        {
            return SampleZoned.Minute;
        }

        [Benchmark]
        public int Second()
        {
            return SampleZoned.Second;
        }

        [Benchmark]
        public int Millisecond()
        {
            return SampleZoned.Millisecond;
        }

        [Benchmark]
        public Instant ToInstant()
        {
            return SampleZoned.ToInstant();
        }
    }
}