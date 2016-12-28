// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using NodaTime.Calendars;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    public class UtcZonedDateTimeBenchmarks
    {
        private static readonly LocalDateTime SampleLocal = new LocalDateTime(2009, 12, 26, 10, 8, 30);
        private static readonly ZonedDateTime Sample = DateTimeZone.Utc.AtStrictly(SampleLocal);

        [Benchmark]
        public void Construction()
        {
            DateTimeZone.Utc.AtStrictly(SampleLocal);
        }

        [Benchmark]
        public int Year() => Sample.Year;

        [Benchmark]
        public int Month() => Sample.Month;

        [Benchmark]
        public int DayOfMonth() => Sample.Day;

        [Benchmark]
        public IsoDayOfWeek DayOfWeek() => Sample.DayOfWeek;

        [Benchmark]
        public int DayOfYear() => Sample.DayOfYear;

        [Benchmark]
        public int Hour() => Sample.Hour;

        [Benchmark]
        public int Minute() => Sample.Minute;

        [Benchmark]
        public int Second() => Sample.Second;

        [Benchmark]
        public int Millisecond() => Sample.Millisecond;

        [Benchmark]
        public long TickOfDay() => Sample.TickOfDay;

        [Benchmark]
        public int TickOfSecond() => Sample.TickOfSecond;

        [Benchmark]
        public int ClockHourOfHalfDay() => Sample.ClockHourOfHalfDay;

        [Benchmark]
        public Era Era() => Sample.Era;

        [Benchmark]
        public int YearOfEra() => Sample.YearOfEra;
    }
}