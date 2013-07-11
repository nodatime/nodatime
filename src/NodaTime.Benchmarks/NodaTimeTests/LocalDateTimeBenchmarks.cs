// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Benchmarks.Framework;
using System.Globalization;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    internal class LocalDateTimeBenchmarks
    {
        private static readonly LocalDateTime Sample = new LocalDateTime(2009, 12, 26, 10, 8, 30);
        private static readonly CultureInfo MutableCulture = (CultureInfo) CultureInfo.InvariantCulture.Clone();

        [Benchmark]
        public void ConstructionToMinute()
        {
            new LocalDateTime(2009, 12, 26, 10, 8).Consume();
        }

        [Benchmark]
        public void ConstructionToSecond()
        {
            new LocalDateTime(2009, 12, 26, 10, 8, 30).Consume();
        }

        [Benchmark]
        public void ConstructionToTick()
        {
            new LocalDateTime(2009, 12, 26, 10, 8, 30, 0, 0).Consume();
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
        public void Date()
        {
            Sample.Date.Consume();
        }

        [Benchmark]
        public void TimeOfDay()
        {
            Sample.TimeOfDay.Consume();
        }

        [Benchmark]
        public void TickOfDay()
        {
            Sample.TickOfDay.Consume();
        }

        [Benchmark]
        public void TickOfSecond()
        {
            Sample.TickOfSecond.Consume();
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
        public void ClockHourOfHalfDay()
        {
            Sample.ClockHourOfHalfDay.Consume();
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
        public void ToString_Parameterless()
        {
            Sample.ToString();
        }

        [Benchmark]
        public void ToString_ExplicitPattern_Invariant()
        {
            Sample.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// This test will involve creating a new NodaFormatInfo for each iteration.
        /// </summary>
        [Benchmark]
        public void ToString_ExplicitPattern_MutableCulture()
        {
            Sample.ToString("dd/MM/yyyy HH:mm:ss", MutableCulture);
        }
    }
}