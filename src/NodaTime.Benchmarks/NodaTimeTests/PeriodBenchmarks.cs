// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    [BenchmarkTask(platform: BenchmarkPlatform.X86, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.RyuJit)]
    internal class PeriodBenchmarks
    {
        private static readonly LocalDate SampleStartDate = new LocalDate(2012, 3, 26);
        private static readonly LocalDate SampleEndDate = new LocalDate(2017, 2, 13);
        private static readonly LocalDate SampleEndDateSameMonth = new LocalDate(2012, 3, 29);
        private static readonly LocalDate SampleEndDateSameYear = new LocalDate(2012, 10, 20);
        private static readonly LocalTime SampleStartTime = new LocalTime(13, 25, 10);
        private static readonly LocalTime SampleEndTime = new LocalTime(18, 10, 25);
        private static readonly LocalDateTime SampleStartDateTime = SampleStartDate + SampleStartTime;
        private static readonly LocalDateTime SampleEndDateTime = SampleEndDate + SampleEndTime;

        [Benchmark]
        public Period Between_LocalDate()
        {
            return Period.Between(SampleStartDate, SampleEndDate);
        }

        [Benchmark]
        public Period Between_LocalDate_Years()
        {
            return Period.Between(SampleStartDate, SampleEndDate, PeriodUnits.Years);
        }

        [Benchmark]
        public Period Between_LocalDate_Months()
        {
            return Period.Between(SampleStartDate, SampleEndDate, PeriodUnits.Months);
        }

        [Benchmark]
        public Period Between_LocalDate_Days()
        {
            return Period.Between(SampleStartDate, SampleEndDate, PeriodUnits.Days);
        }

        [Benchmark]
        public Period Between_LocalDate_Days_SameMonth()
        {
            return Period.Between(SampleStartDate, SampleEndDateSameMonth, PeriodUnits.Days);
        }

        [Benchmark]
        public Period Between_LocalDate_Days_SameYear()
        {
            return Period.Between(SampleStartDate, SampleEndDateSameYear, PeriodUnits.Days);
        }

        [Benchmark]
        public Period Between_LocalTime()
        {
            return Period.Between(SampleStartTime, SampleEndTime);
        }

        [Benchmark]
        public Period Between_LocalDateTime()
        {
            return Period.Between(SampleStartDateTime, SampleEndDateTime);
        }

        [Benchmark]
        public Period Between_LocalDateTime_Ticks()
        {
            return Period.Between(SampleStartDateTime, SampleEndDateTime, PeriodUnits.Ticks);
        }
    }
}
