// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;
using NodaTime.Calendars;
using NodaTime.Text;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    [BenchmarkTask(platform: BenchmarkPlatform.X86, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.RyuJit)]
    internal class LocalDateBenchmarks
    {
        private static readonly LocalDate Sample = new LocalDate(2009, 12, 26);
        private static readonly LocalDate SampleBeforeEpoch = new LocalDate(1909, 12, 26);

        private static readonly LocalDatePattern Pattern = LocalDatePattern.CreateWithInvariantCulture("dd/MM/yyyy");

        private static readonly Period SamplePeriod = new PeriodBuilder { Years = 1, Months = 2, Weeks = 3, Days = 4 }.Build();

        [Benchmark]
        public void PatternFormat()
        {
            Pattern.Format(Sample);
        }

        [Benchmark]
        public LocalDate PatternParse()
        {
            var parseResult = Pattern.Parse("26/12/2009");
            return parseResult.Value;
        }

        [Benchmark]
        public LocalDate Construction()
        {
            return new LocalDate(2009, 12, 26);
        }

        [Benchmark]
        public LocalDate ConstructionOutsidePrecomputedRange()
        {
            return new LocalDate(1009, 12, 26);
        }

        [Benchmark]
        public LocalDate ConstructionAvoidingCache()
        {
            // Construct the first day of every year between 1000 and 3000 AD. This
            // should thoroughly test CalculateYearTicks, as we'll never get a cache hit.
            LocalDate localDate = new LocalDate();
            for (int year = 1; year <= 3000; year++)
            {
                localDate = new LocalDate(year, 1, 1);
            }
            return localDate;
        }

#if !NO_INTERNALS
        private static readonly int SampleDays = Sample.DaysSinceEpoch;

        [Benchmark]
        public LocalDate ConstructionFromDays_SpecifyCalendar()
        {
            return new LocalDate(SampleDays, CalendarSystem.Iso);
        }

        [Benchmark]
        public void ConstructionFromDays_DefaultCalendar()
        {
            new LocalDate(SampleDays);
        }
#endif

        [Benchmark]
        public void FromWeekYearWeekAndDay()
        {
            LocalDate.FromWeekYearWeekAndDay(2009, 1, NodaTime.IsoDayOfWeek.Thursday);
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
        public IsoDayOfWeek IsoDayOfWeek_BeforeEpoch()
        {
            return SampleBeforeEpoch.IsoDayOfWeek;
        }
        
        [Benchmark]
        public int DayOfYear()
        {
            return Sample.DayOfYear;
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
        public Era Era()
        {
            return Sample.Era;
        }

        [Benchmark]
        public int YearOfEra()
        {
            return Sample.YearOfEra;
        }

        [Benchmark]
        public LocalDate PlusYears()
        {
            return Sample.PlusYears(3);
        }

        [Benchmark]
        public LocalDate PlusMonths()
        {
            return Sample.PlusMonths(3);
        }

        [Benchmark]
        public LocalDate PlusWeeks()
        {
            return Sample.PlusWeeks(3);
        }

        [Benchmark]
        public LocalDate PlusDays()
        {
            return Sample.PlusDays(3);
        }

        [Benchmark]
        public LocalDate PlusDays_MonthBoundary()
        {
            return Sample.PlusDays(-50);
        }

        [Benchmark]
        public LocalDate PlusDays_MonthYearBoundary()
        {
            return Sample.PlusDays(10);
        }

        [Benchmark]
        public LocalDate PlusDays_LargeGap()
        {
            return Sample.PlusDays(1000);
        }

        [Benchmark]
        public LocalDate PlusPeriod()
        {
            return (Sample + SamplePeriod);
        }

        [Benchmark]
        public LocalDate MinusPeriod()
        {
            return (Sample - SamplePeriod);
        }
    }
}