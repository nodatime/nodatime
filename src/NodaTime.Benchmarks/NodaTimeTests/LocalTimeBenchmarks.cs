// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Benchmarks.Framework;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    internal class LocalTimeBenchmarks
    {
        private static readonly LocalTime Sample = new LocalTime(10, 8, 30, 300, 1234);
        private static readonly Period SamplePeriod = new PeriodBuilder { Hours = 10, Minutes = 4, Seconds = 5, Milliseconds = 20, Ticks = 30 }.Build();
        private static readonly LocalDateTime LocalDateTime = new LocalDateTime(2011, 9, 14, 15, 10, 25);

        [Benchmark]
        public void ConstructionToMinute()
        {
            new LocalTime(15, 10).Consume();
        }

        [Benchmark]
        public void ConstructionToSecond()
        {
            new LocalTime(15, 10, 25).Consume();
        }

        [Benchmark]
        public void ConstructionToMillisecond()
        {
            new LocalTime(15, 10, 25, 500).Consume();
        }

        [Benchmark]
        public void ConstructionToTick()
        {
            new LocalTime(15, 10, 25, 500, 1234).Consume();
        }

        [Benchmark]
        public void ConversionFromLocalDateTime()
        {
            LocalDateTime.TimeOfDay.Consume();
        }

        [Benchmark]
        public void Hour()
        {
            Sample.Hour.Consume();
        }

        [Benchmark]
        public void Minute()
        {
            Sample.Minute.Consume();
        }

        [Benchmark]
        public void Second()
        {
            Sample.Second.Consume();
        }

        [Benchmark]
        public void Millisecond()
        {
            Sample.Millisecond.Consume();
        }

        [Benchmark]
        public void ClockHourOfHalfDay()
        {
            Sample.ClockHourOfHalfDay.Consume();
        }

        [Benchmark]
        public void TickOfSecond()
        {
            Sample.TickOfSecond.Consume();
        }

        [Benchmark]
        public void TickOfDay()
        {
            Sample.TickOfDay.Consume();
        }

        [Benchmark]
        public void PlusHours()
        {
            Sample.PlusHours(3).Consume();
        }

        [Benchmark]
        public void PlusHours_OverflowDay()
        {
            Sample.PlusHours(33).Consume();
        }

        [Benchmark]
        public void PlusHours_Negative()
        {
            Sample.PlusHours(-3).Consume();
        }

        [Benchmark]
        public void PlusHours_UnderflowDay()
        {
            Sample.PlusHours(-33).Consume();
        }

        [Benchmark]
        public void PlusMinutes()
        {
            Sample.PlusMinutes(3).Consume();
        }

        [Benchmark]
        public void PlusSeconds()
        {
            Sample.PlusSeconds(3).Consume();
        }

        [Benchmark]
        public void PlusMilliseconds()
        {
            Sample.PlusMilliseconds(3).Consume();
        }

        [Benchmark]
        public void PlusTicks()
        {
            Sample.PlusTicks(3).Consume();
        }

        [Benchmark]
        public void PlusPeriod()
        {
            (Sample + SamplePeriod).Consume();
        }

        [Benchmark]
        public void MinusPeriod()
        {
            (Sample - SamplePeriod).Consume();
        }
    }
}