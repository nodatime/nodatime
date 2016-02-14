// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using BenchmarkDotNet.Attributes;
using NodaTime.Calendars;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    [Config(typeof(BenchmarkConfig))]
    public class LocalDateTimeBenchmarks
    {
        private static readonly DateTime SampleDateTime = new DateTime(2009, 12, 26, 10, 8, 30);
        private static readonly LocalDateTime Sample = new LocalDateTime(2009, 12, 26, 10, 8, 30);
        private static readonly CultureInfo MutableCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        private static readonly Period SampleTimePeriod = new PeriodBuilder { Hours = 10, Minutes = 4, Seconds = 5, Milliseconds = 20, Ticks = 30 }.Build();
        private static readonly Period SampleDatePeriod = new PeriodBuilder { Years = 1, Months = 2, Weeks = 3, Days = 4 }.Build();
        private static readonly Period SampleMixedPeriod = SampleDatePeriod + SampleTimePeriod;

        [Benchmark]
        public LocalDateTime ConstructionToMinute()
        {
            return new LocalDateTime(2009, 12, 26, 10, 8);
        }

        [Benchmark]
        public LocalDateTime ConstructionToSecond()
        {
            return new LocalDateTime(2009, 12, 26, 10, 8, 30);
        }

        [Benchmark]
        public LocalDateTime ConstructionToTick()
        {
            return new LocalDateTime(2009, 12, 26, 10, 8, 30, 0, 0);
        }

        [Benchmark]
        public LocalDateTime FromDateTime()
        {
            return LocalDateTime.FromDateTime(SampleDateTime);
        }

        [Benchmark]
        public DateTime ToDateTimeUnspecified()
        {
            return Sample.ToDateTimeUnspecified();
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
        public int DayOfYear()
        {
            return Sample.DayOfYear;
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
        public LocalDate Date()
        {
            return Sample.Date;
        }

        [Benchmark]
        public LocalTime TimeOfDay()
        {
            return Sample.TimeOfDay;
        }

        [Benchmark]
        public long TickOfDay()
        {
            return Sample.TickOfDay;
        }

        [Benchmark]
        public int TickOfSecond()
        {
            return Sample.TickOfSecond;
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
        public int ClockHourOfHalfDay()
        {
            return Sample.ClockHourOfHalfDay;
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
        public string ToString_Parameterless()
        {
            return Sample.ToString();
        }

        [Benchmark]
        public string ToString_ExplicitPattern_Invariant()
        {
            return Sample.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// This test will involve creating a new NodaFormatInfo for each iteration.
        /// </summary>
        [Benchmark]
        public string ToString_ExplicitPattern_MutableCulture()
        {
            return Sample.ToString("dd/MM/yyyy HH:mm:ss", MutableCulture);
        }

        [Benchmark]
        public LocalDateTime PlusYears()
        {
            return Sample.PlusYears(3);
        }

        [Benchmark]
        public LocalDateTime PlusMonths()
        {
            return Sample.PlusMonths(3);
        }

        [Benchmark]
        public LocalDateTime PlusWeeks()
        {
            return Sample.PlusWeeks(3);
        }

        [Benchmark]
        public LocalDateTime PlusDays()
        {
            return Sample.PlusDays(3);
        }

        [Benchmark]
        public LocalDateTime PlusHours()
        {
            return Sample.PlusHours(3);
        }

        public LocalDateTime PlusHours_OverflowDay()
        {
            return Sample.PlusHours(33);
        }

        [Benchmark]
        public LocalDateTime PlusHours_Negative()
        {
            return Sample.PlusHours(-3);
        }

        [Benchmark]
        public LocalDateTime PlusHours_UnderflowDay()
        {
            return Sample.PlusHours(-33);
        }

        [Benchmark]
        public LocalDateTime PlusMinutes()
        {
            return Sample.PlusMinutes(3);
        }

        [Benchmark]
        public LocalDateTime PlusSeconds()
        {
            return Sample.PlusSeconds(3);
        }

        [Benchmark]
        public LocalDateTime PlusMilliseconds()
        {
            return Sample.PlusMilliseconds(3);
        }

        [Benchmark]
        public LocalDateTime PlusTicks()
        {
            return Sample.PlusTicks(3);
        }

        [Benchmark]
        public LocalDateTime PlusDatePeriod()
        {
            return (Sample + SampleDatePeriod);
        }

        [Benchmark]
        public LocalDateTime MinusDatePeriod()
        {
            return (Sample - SampleDatePeriod);
        }

        [Benchmark]
        public LocalDateTime PlusTimePeriod()
        {
            return (Sample + SampleTimePeriod);
        }

        [Benchmark]
        public LocalDateTime MinusTimePeriod()
        {
            return (Sample - SampleTimePeriod);
        }

        [Benchmark]
        public LocalDateTime PlusMixedPeriod()
        {
            return (Sample + SampleMixedPeriod);
        }

        [Benchmark]
        public LocalDateTime MinusMixedPeriod()
        {
            return (Sample - SampleMixedPeriod);
        }

#if !NO_INTERNALS
        //        [Benchmark]
        //        public LocalInstant ToLocalInstant()
        //        {
        //            return Sample.ToLocalInstant();
        //        }
#endif
    }
}