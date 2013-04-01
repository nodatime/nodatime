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

        private static readonly LocalDatePattern Pattern = LocalDatePattern.CreateWithInvariantCulture("dd/MM/yyyy");

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
    }
}