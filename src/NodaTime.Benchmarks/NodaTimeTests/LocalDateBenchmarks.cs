// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using NodaTime.Calendars;
using NodaTime.Text;
using System;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    public class LocalDateBenchmarks
    {
        private static readonly LocalDate Sample = new LocalDate(2009, 12, 26);
        private static readonly DateTime SampleDateTime = new DateTime(2009, 12, 26, 1, 2, 3, DateTimeKind.Utc);
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
        public LocalDate Construction() => new LocalDate(2009, 12, 26);

        [Benchmark]
        public LocalDate ConstructionOutsidePrecomputedRange() => new LocalDate(1009, 12, 26);

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
        public LocalDate ConstructionFromDays_SpecifyCalendar() => new LocalDate(SampleDays, CalendarSystem.Iso);

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
        public int Year() => Sample.Year;

        [Benchmark]
        public int Month() => Sample.Month;

        [Benchmark]
        public int DayOfMonth() => Sample.Day;

        [Benchmark]
        public IsoDayOfWeek DayOfWeek() => Sample.DayOfWeek;

        [Benchmark]
        public IsoDayOfWeek DayOfWeek_BeforeEpoch()
        {
            return SampleBeforeEpoch.DayOfWeek;
        }

        [Benchmark]
        public int DayOfYear() => Sample.DayOfYear;

        [Benchmark]
        public Era Era() => Sample.Era;

        [Benchmark]
        public int YearOfEra() => Sample.YearOfEra;

        [Benchmark]
        public LocalDate PlusYears() => Sample.PlusYears(3);

        [Benchmark]
        public LocalDate PlusMonths() => Sample.PlusMonths(3);

        [Benchmark]
        public LocalDate PlusWeeks() => Sample.PlusWeeks(3);

        [Benchmark]
        public LocalDate PlusDays() => Sample.PlusDays(3);

        [Benchmark]
        public LocalDate PlusDays_MonthBoundary() => Sample.PlusDays(-50);

        [Benchmark]
        public LocalDate PlusDays_MonthYearBoundary() => Sample.PlusDays(10);

        [Benchmark]
        public LocalDate PlusDays_LargeGap() => Sample.PlusDays(1000);

        [Benchmark]
        public LocalDate PlusPeriod() => (Sample + SamplePeriod);

        [Benchmark]
        public LocalDate MinusPeriod() => (Sample - SamplePeriod);

        [Benchmark]
        public LocalDate FromDateTime() => LocalDate.FromDateTime(SampleDateTime);

        [Benchmark]
        public LocalDate FromDateTime_WithCalendar() => LocalDate.FromDateTime(SampleDateTime, CalendarSystem.Julian);
    }
}