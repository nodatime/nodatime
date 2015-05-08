// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    [BenchmarkTask(platform: BenchmarkPlatform.X86, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.RyuJit)]
    internal class LocalTimeBenchmarks
    {
        private static readonly LocalTime Sample = new LocalTime(10, 8, 30, 300, 1234);
        private static readonly Period SamplePeriod = new PeriodBuilder { Hours = 10, Minutes = 4, Seconds = 5, Milliseconds = 20, Ticks = 30 }.Build();
        private static readonly LocalDateTime LocalDateTime = new LocalDateTime(2011, 9, 14, 15, 10, 25);

        [Benchmark]
        public LocalTime ConstructionToMinute()
        {
            return new LocalTime(15, 10);
        }

        [Benchmark]
        public LocalTime ConstructionToSecond()
        {
            return new LocalTime(15, 10, 25);
        }

        [Benchmark]
        public LocalTime ConstructionToMillisecond()
        {
            return new LocalTime(15, 10, 25, 500);
        }

        [Benchmark]
        public LocalTime ConstructionToTick()
        {
            return new LocalTime(15, 10, 25, 500, 1234);
        }

        [Benchmark]
        public LocalTime ConversionFromLocalDateTime()
        {
            return LocalDateTime.TimeOfDay;
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
        public int ClockHourOfHalfDay()
        {
            return Sample.ClockHourOfHalfDay;
        }

        [Benchmark]
        public int TickOfSecond()
        {
            return Sample.TickOfSecond;
        }

        [Benchmark]
        public long TickOfDay()
        {
            return Sample.TickOfDay;
        }

        [Benchmark]
        public LocalTime PlusHours()
        {
            return Sample.PlusHours(3);
        }

        [Benchmark]
        public LocalTime PlusHours_OverflowDay()
        {
            return Sample.PlusHours(33);
        }

        [Benchmark]
        public LocalTime PlusHours_Negative()
        {
            return Sample.PlusHours(-3);
        }

        [Benchmark]
        public LocalTime PlusHours_UnderflowDay()
        {
            return Sample.PlusHours(-33);
        }

        [Benchmark]
        public LocalTime PlusMinutes()
        {
            return Sample.PlusMinutes(3);
        }

        [Benchmark]
        public LocalTime PlusSeconds()
        {
            return Sample.PlusSeconds(3);
        }

        [Benchmark]
        public LocalTime PlusMilliseconds()
        {
            return Sample.PlusMilliseconds(3);
        }

        [Benchmark]
        public LocalTime PlusTicks()
        {
            return Sample.PlusTicks(3);
        }

        [Benchmark]
        public LocalTime PlusPeriod()
        {
            return (Sample + SamplePeriod);
        }

        [Benchmark]
        public LocalTime MinusPeriod()
        {
            return (Sample - SamplePeriod);
        }
    }
}