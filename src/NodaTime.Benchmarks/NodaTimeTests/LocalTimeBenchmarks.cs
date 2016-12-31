// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    public class LocalTimeBenchmarks
    {
        private static readonly LocalTime Sample = LocalTime.FromHourMinuteSecondMillisecondTick(10, 8, 30, 300, 1234);
        private static readonly Period SamplePeriod = new PeriodBuilder { Hours = 10, Minutes = 4, Seconds = 5, Milliseconds = 20, Ticks = 30 }.Build();
        private static readonly LocalDateTime LocalDateTime = new LocalDateTime(2011, 9, 14, 15, 10, 25);

        [Benchmark]
        public LocalTime ConstructionToMinute() => new LocalTime(15, 10);

        [Benchmark]
        public LocalTime ConstructionToSecond() => new LocalTime(15, 10, 25);

        [Benchmark]
        public LocalTime ConstructionToMillisecond() => new LocalTime(15, 10, 25, 500);

        [Benchmark]
        public LocalTime ConstructionToTick() => LocalTime.FromHourMinuteSecondMillisecondTick(15, 10, 25, 500, 1234);

        [Benchmark]
        public LocalTime ConversionFromLocalDateTime() => LocalDateTime.TimeOfDay;

        [Benchmark]
        public int Hour() => Sample.Hour;

        [Benchmark]
        public int Minute() => Sample.Minute;

        [Benchmark]
        public int Second() => Sample.Second;

        [Benchmark]
        public int Millisecond() => Sample.Millisecond;

        [Benchmark]
        public int ClockHourOfHalfDay() => Sample.ClockHourOfHalfDay;

        [Benchmark]
        public int TickOfSecond() => Sample.TickOfSecond;

        [Benchmark]
        public long TickOfDay() => Sample.TickOfDay;

        [Benchmark]
        public LocalTime PlusHours() => Sample.PlusHours(3);

        [Benchmark]
        public LocalTime PlusHours_OverflowDay() => Sample.PlusHours(33);

        [Benchmark]
        public LocalTime PlusHours_Negative() => Sample.PlusHours(-3);

        [Benchmark]
        public LocalTime PlusHours_UnderflowDay() => Sample.PlusHours(-33);

        [Benchmark]
        public LocalTime PlusMinutes() => Sample.PlusMinutes(3);

        [Benchmark]
        public LocalTime PlusSeconds() => Sample.PlusSeconds(3);

        [Benchmark]
        public LocalTime PlusMilliseconds() => Sample.PlusMilliseconds(3);

        [Benchmark]
        public LocalTime PlusTicks() => Sample.PlusTicks(3);

        [Benchmark]
        public LocalTime PlusPeriod() => (Sample + SamplePeriod);

        [Benchmark]
        public LocalTime MinusPeriod() => (Sample - SamplePeriod);
    }
}