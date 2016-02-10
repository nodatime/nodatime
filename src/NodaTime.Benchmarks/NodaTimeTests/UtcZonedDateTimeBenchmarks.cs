// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using NodaTime.Calendars;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    [Config(typeof(BenchmarkConfig))]
    internal class UtcZonedDateTimeBenchmarks
    {
        private static readonly LocalDateTime SampleLocal = new LocalDateTime(2009, 12, 26, 10, 8, 30);
        private static readonly ZonedDateTime Sample = DateTimeZone.Utc.AtStrictly(SampleLocal);

        [Benchmark]
        public void Construction()
        {
            DateTimeZone.Utc.AtStrictly(SampleLocal);
        }

        [Benchmark]
        public int Year()
        {
            return Sample.Year;
        }

        [Benchmark]
        public int Month()
        {
            return Sample.Month;
        }

        [Benchmark]
        public int DayOfMonth()
        {
            return Sample.Day;
        }

        [Benchmark]
        public IsoDayOfWeek IsoDayOfWeek()
        {
            return Sample.IsoDayOfWeek;
        }

        [Benchmark]
        public int DayOfYear()
        {
            return Sample.DayOfYear;
        }

        [Benchmark]
        public int Hour()
        {
            return Sample.Hour;
        }

        [Benchmark]
        public int Minute()
        {
            return Sample.Minute;
        }

        [Benchmark]
        public int Second()
        {
            return Sample.Second;
        }

        [Benchmark]
        public int Millisecond()
        {
            return Sample.Millisecond;
        }

        [Benchmark]
        public long TickOfDay()
        {
            return Sample.TickOfDay;
        }

        [Benchmark]
        public int TickOfSecond()
        {
            return Sample.TickOfSecond;
        }

        [Benchmark]
        public int WeekOfWeekYear()
        {
            return Sample.WeekOfWeekYear;
        }

        [Benchmark]
        public int WeekYear()
        {
            return Sample.WeekYear;
        }

        [Benchmark]
        public int ClockHourOfHalfDay()
        {
            return Sample.ClockHourOfHalfDay;
        }

        [Benchmark]
        public Era Era()
        {
            return Sample.Era;
        }

        [Benchmark]
        public int YearOfEra()
        {
            return Sample.YearOfEra;
        }
    }
}