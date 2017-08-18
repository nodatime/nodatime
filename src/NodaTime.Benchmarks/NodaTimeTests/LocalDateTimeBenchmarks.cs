// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using BenchmarkDotNet.Attributes;
using NodaTime.Calendars;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    public class LocalDateTimeBenchmarks
    {
        private static readonly DateTime SampleDateTime = new DateTime(2009, 12, 26, 10, 8, 30);
        private static readonly LocalDateTime Sample = new LocalDateTime(2009, 12, 26, 10, 8, 30);
        private static readonly CultureInfo MutableCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        private static readonly Period SampleTimePeriod = new PeriodBuilder { Hours = 10, Minutes = 4, Seconds = 5, Milliseconds = 20, Ticks = 30 }.Build();
        private static readonly Period SampleDatePeriod = new PeriodBuilder { Years = 1, Months = 2, Weeks = 3, Days = 4 }.Build();
        private static readonly Period SampleMixedPeriod = SampleDatePeriod + SampleTimePeriod;

        [Benchmark]
        public LocalDateTime ConstructionToMinute() => new LocalDateTime(2009, 12, 26, 10, 8);

        [Benchmark]
        public LocalDateTime ConstructionToSecond() => new LocalDateTime(2009, 12, 26, 10, 8, 30);

        [Benchmark]
        public LocalDateTime FromDateTime() => LocalDateTime.FromDateTime(SampleDateTime);

        [Benchmark]
        public LocalDateTime FromDateTime_WithCalendar() => LocalDateTime.FromDateTime(SampleDateTime, CalendarSystem.Julian);

        [Benchmark]
        public DateTime ToDateTimeUnspecified() => Sample.ToDateTimeUnspecified();

        [Benchmark]
        public int Year() => Sample.Year;

        [Benchmark]
        public int Month() => Sample.Month;

        [Benchmark]
        public int DayOfMonth() => Sample.Day;

        [Benchmark]
        public IsoDayOfWeek DayOfWeek() => Sample.DayOfWeek;

        [Benchmark]
        public int DayOfYear() => Sample.DayOfYear;

        [Benchmark]
        public int Hour() => Sample.Hour;

        [Benchmark]
        public int Minute() => Sample.Minute;

        [Benchmark]
        public int Second() => Sample.Second;

        [Benchmark]
        public int Millisecond() => Sample.Millisecond;

        [Benchmark]
        public LocalDate Date() => Sample.Date;

        [Benchmark]
        public LocalTime TimeOfDay() => Sample.TimeOfDay;

        [Benchmark]
        public long TickOfDay() => Sample.TickOfDay;

        [Benchmark]
        public int TickOfSecond() => Sample.TickOfSecond;

        [Benchmark]
        public int ClockHourOfHalfDay() => Sample.ClockHourOfHalfDay;

        [Benchmark]
        public Era Era() => Sample.Era;

        [Benchmark]
        public int YearOfEra() => Sample.YearOfEra;

        [Benchmark]
        public string ToString_Parameterless() => Sample.ToString();

        [Benchmark]
        public string ToString_ExplicitPattern_Invariant() => Sample.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

        /// <summary>
        /// This test will involve creating a new NodaFormatInfo for each iteration.
        /// </summary>
        [Benchmark]
        public string ToString_ExplicitPattern_MutableCulture() => Sample.ToString("dd/MM/yyyy HH:mm:ss", MutableCulture);

        [Benchmark]
        public LocalDateTime PlusYears() => Sample.PlusYears(3);

        [Benchmark]
        public LocalDateTime PlusMonths() => Sample.PlusMonths(3);

        [Benchmark]
        public LocalDateTime PlusWeeks() => Sample.PlusWeeks(3);

        [Benchmark]
        public LocalDateTime PlusDays() => Sample.PlusDays(3);

        [Benchmark]
        public LocalDateTime PlusHours() => Sample.PlusHours(3);

        public LocalDateTime PlusHours_OverflowDay() => Sample.PlusHours(33);

        [Benchmark]
        public LocalDateTime PlusHours_Negative() => Sample.PlusHours(-3);

        [Benchmark]
        public LocalDateTime PlusHours_UnderflowDay() => Sample.PlusHours(-33);

        [Benchmark]
        public LocalDateTime PlusMinutes() => Sample.PlusMinutes(3);

        [Benchmark]
        public LocalDateTime PlusSeconds() => Sample.PlusSeconds(3);

        [Benchmark]
        public LocalDateTime PlusMilliseconds() => Sample.PlusMilliseconds(3);

        [Benchmark]
        public LocalDateTime PlusTicks() => Sample.PlusTicks(3);

        [Benchmark]
        public LocalDateTime PlusDatePeriod() => (Sample + SampleDatePeriod);

        [Benchmark]
        public LocalDateTime MinusDatePeriod() => (Sample - SampleDatePeriod);

        [Benchmark]
        public LocalDateTime PlusTimePeriod() => (Sample + SampleTimePeriod);

        [Benchmark]
        public LocalDateTime MinusTimePeriod() => (Sample - SampleTimePeriod);

        [Benchmark]
        public LocalDateTime PlusMixedPeriod() => (Sample + SampleMixedPeriod);

        [Benchmark]
        public LocalDateTime MinusMixedPeriod() => (Sample - SampleMixedPeriod);

#if !NO_INTERNALS
        [Benchmark]
        public LocalInstantWrapper ToLocalInstant() => new LocalInstantWrapper(Sample.ToLocalInstant());

        public struct LocalInstantWrapper
        {
            private readonly LocalInstant value;
            internal LocalInstantWrapper(LocalInstant value) => this.value = value;
        }
#endif
    }
}