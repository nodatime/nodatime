// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Benchmarks.Framework;
using NodaTime.Text;

namespace NodaTime.Benchmarks.NodaTimeTests
{
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
        public void PatternParse()
        {
            var parseResult = Pattern.Parse("26/12/2009");
            parseResult.Value.Consume();
        }

        [Benchmark]
        public void Construction()
        {
            new LocalDate(2009, 12, 26).Consume();
        }

        [Benchmark]
        public void ConstructionOutsidePrecomputedRange()
        {
            new LocalDate(1009, 12, 26).Consume();
        }

        [Benchmark]
        public void ConstructionAvoidingCache()
        {
            // Construct the first day of every year between 1000 and 3000 AD. This
            // should thoroughly test CalculateYearTicks, as we'll never get a cache hit.
            for (int year = 1; year <= 3000; year++)
            {
                new LocalDate(year, 1, 1).Consume();
            }
        }
 
        [Benchmark]
        public void FromWeekYearWeekAndDay()
        {
            LocalDate.FromWeekYearWeekAndDay(2009, 1, NodaTime.IsoDayOfWeek.Thursday).Consume();
        }

        [Benchmark]
        public void Year()
        {
            Sample.Year.Consume();
        }

        [Benchmark]
        public void Month()
        {
            Sample.Month.Consume();
        }

        [Benchmark]
        public void DayOfMonth()
        {
            Sample.Day.Consume();
        }

        [Benchmark]
        public void IsoDayOfWeek()
        {
            Sample.IsoDayOfWeek.Consume();
        }

        [Benchmark]
        public void IsoDayOfWeek_BeforeEpoch()
        {
            SampleBeforeEpoch.IsoDayOfWeek.Consume();
        }
        
        [Benchmark]
        public void DayOfYear()
        {
            Sample.DayOfYear.Consume();
        }

        [Benchmark]
        public void WeekOfWeekYear()
        {
            Sample.WeekOfWeekYear.Consume();
        }

        [Benchmark]
        public void WeekYear()
        {
            Sample.WeekYear.Consume();
        }

        [Benchmark]
        public void Era()
        {
            Sample.Era.Consume();
        }

        [Benchmark]
        public void CenturyOfEra()
        {
            Sample.YearOfCentury.Consume();
        }

        [Benchmark]
        public void YearOfCentury()
        {
            Sample.YearOfCentury.Consume();
        }

        [Benchmark]
        public void YearOfEra()
        {
            Sample.YearOfEra.Consume();
        }

        [Benchmark]
        public void PlusYears()
        {
            Sample.PlusYears(3).Consume();
        }

        [Benchmark]
        public void PlusMonths()
        {
            Sample.PlusMonths(3).Consume();
        }

        [Benchmark]
        public void PlusWeeks()
        {
            Sample.PlusWeeks(3).Consume();
        }

        [Benchmark]
        public void PlusDays()
        {
            Sample.PlusDays(3).Consume();
        }

        [Benchmark]
        public void PlusDays_MonthBoundary()
        {
            Sample.PlusDays(-50).Consume();
        }

        [Benchmark]
        public void PlusDays_MonthYearBoundary()
        {
            Sample.PlusDays(10).Consume();
        }

        [Benchmark]
        public void PlusDays_LargeGap()
        {
            Sample.PlusDays(1000).Consume();
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