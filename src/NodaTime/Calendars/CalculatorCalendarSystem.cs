// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Fields;
using NodaTime.Utility;
using System;
using System.Globalization;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Calendar system constructed from separate calculators for year/month/day, week-year/week/week-day and time fields.
    /// </summary>
    internal sealed class CalculatorCalendarSystem : CalendarSystem
    {
        private const string GregorianName = "Gregorian";
        private const string IsoName = "ISO";
        private const string CopticName = "Coptic";
        private const string JulianName = "Julian";
        private const string IslamicName = "Hijri";

        private readonly YearMonthDayCalculator yearMonthDayCalculator;
        private readonly WeekYearCalculator weekYearCalculator;

        internal static readonly CalendarSystem[] NewGregorianCalendarSystems;
        internal static readonly CalendarSystem[] NewCopticCalendarSystems;
        internal static readonly CalendarSystem[] NewJulianCalendarSystems;
        internal static readonly CalendarSystem[,] NewIslamicCalendarSystems;
        internal static readonly CalendarSystem NewIsoCalendarSystem;

        static CalculatorCalendarSystem()
        {
            NewGregorianCalendarSystems = new CalendarSystem[7];
            NewCopticCalendarSystems = new CalendarSystem[7];
            NewJulianCalendarSystems = new CalendarSystem[7];
            for (int i = 1; i <= 7; i++)
            {
                var gregorianCalculator = new GregorianYearMonthDayCalculator();
                NewGregorianCalendarSystems[i - 1] = new CalculatorCalendarSystem(
                    CreateIdFromNameAndMinDaysInFirstWeek(GregorianName, i), GregorianName, gregorianCalculator, i);
                var copticCalculator = new CopticYearMonthDayCalculator();
                NewCopticCalendarSystems[i - 1] = new CalculatorCalendarSystem(
                    CreateIdFromNameAndMinDaysInFirstWeek(CopticName, i), CopticName, copticCalculator, i);
                var julianCalculator = new JulianYearMonthDayCalculator();
                NewJulianCalendarSystems[i - 1] = new CalculatorCalendarSystem(
                    CreateIdFromNameAndMinDaysInFirstWeek(JulianName, i), JulianName, julianCalculator, i);
            }
            NewIsoCalendarSystem = new CalculatorCalendarSystem(IsoName, IsoName, new IsoYearMonthDayCalculator(), 4);

            NewIslamicCalendarSystems = new CalendarSystem[4, 2];
            for (int i = 1; i <= 4; i++)
            {
                for (int j = 1; j <= 2; j++)
                {
                    var leapYearPattern = (IslamicLeapYearPattern)i;
                    var epoch = (IslamicEpoch)j;
                    var calculator = new IslamicYearMonthDayCalculator((IslamicLeapYearPattern)i, (IslamicEpoch)j);
                    string id = string.Format(CultureInfo.InvariantCulture, "{0} {1}-{2}", IslamicName, epoch, leapYearPattern);
                    NewIslamicCalendarSystems[i - 1, j - 1] = new CalculatorCalendarSystem(id, IslamicName, calculator, 4);
                }
            }
        }

        internal CalculatorCalendarSystem(
            string id,
            string name,
            YearMonthDayCalculator yearMonthDayCalculator,
            int minDaysInFirstWeek)
            : this(id, name, yearMonthDayCalculator, new WeekYearCalculator(yearMonthDayCalculator, minDaysInFirstWeek))
        {
        }

        private CalculatorCalendarSystem(
            string id,
            string name,
            YearMonthDayCalculator yearMonthDayCalculator,
            WeekYearCalculator weekYearCalculator)
        : base(id, name,
                   yearMonthDayCalculator.MinYear,
                   yearMonthDayCalculator.MaxYear,
                   CreateFields(yearMonthDayCalculator),
                   yearMonthDayCalculator.Eras)
        {
            this.yearMonthDayCalculator = yearMonthDayCalculator;
            this.weekYearCalculator = weekYearCalculator;
        }

        private static FieldSet CreateFields(YearMonthDayCalculator yearMonthDayCalculator)
        {
            return new FieldSet.Builder(TimeOfDayCalculator.TimeFields)
            {
                Days = SimplePeriodField.Days,
                Weeks = SimplePeriodField.Weeks,
                Months = new MonthsPeriodField(yearMonthDayCalculator),
                Years = new YearsPeriodField(yearMonthDayCalculator)
            }.Build();
        }

        public override int GetDaysInMonth(int year, int month)
        {
            return yearMonthDayCalculator.GetDaysInMonth(year, month);
        }

        public override bool IsLeapYear(int year)
        {
            return yearMonthDayCalculator.IsLeapYear(year);
        }

        public override int GetMaxMonth(int year)
        {
            return yearMonthDayCalculator.MonthsInYear;
        }

        internal override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth,
                                                       long tickOfDay)
        {
            Preconditions.CheckArgumentRange("tickOfDay", tickOfDay, 0, NodaConstants.TicksPerStandardDay - 1);
            return yearMonthDayCalculator.GetLocalInstant(year, monthOfYear, dayOfMonth).PlusTicks(tickOfDay);
        }

        internal override LocalInstant GetLocalInstantFromWeekYearWeekAndDayOfWeek(int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek)
        {
            return weekYearCalculator.GetLocalInstant(weekYear, weekOfWeekYear, dayOfWeek);
        }

        internal override LocalInstant GetLocalInstant(Era era, int yearOfEra, int monthOfYear, int dayOfMonth)
        {
            return yearMonthDayCalculator.GetLocalInstant(era, yearOfEra, monthOfYear, dayOfMonth);
        }

        internal override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute, int millisecondOfSecond, int tickOfMillisecond)
        {
            LocalInstant date = yearMonthDayCalculator.GetLocalInstant(year, monthOfYear, dayOfMonth);
            long timeTicks = TimeOfDayCalculator.GetTicks(hourOfDay, minuteOfHour, secondOfMinute, millisecondOfSecond, tickOfMillisecond);
            return date.PlusTicks(timeTicks);
        }

        internal override int GetMaxYearOfEra(int eraIndex)
        {
            return yearMonthDayCalculator.GetMaxYearOfEra(eraIndex);
        }

        internal override int GetMinYearOfEra(int eraIndex)
        {
            return yearMonthDayCalculator.GetMinYearOfEra(eraIndex);
        }
        
        internal override int GetAbsoluteYear(int yearOfEra, int eraIndex)
        {
            return yearMonthDayCalculator.GetAbsoluteYear(yearOfEra, eraIndex);
        }

        internal override int GetTickOfSecond(LocalInstant localInstant)
        {
            return TimeOfDayCalculator.GetTickOfSecond(localInstant);
        }

        internal override int GetTickOfMillisecond(LocalInstant localInstant)
        {
            return TimeOfDayCalculator.GetTickOfMillisecond(localInstant);
        }

        internal override long GetTickOfDay(LocalInstant localInstant)
        {
            return TimeOfDayCalculator.GetTickOfDay(localInstant);
        }

        internal override int GetMillisecondOfSecond(LocalInstant localInstant)
        {
            return TimeOfDayCalculator.GetMillisecondOfSecond(localInstant);
        }

        internal override int GetMillisecondOfDay(LocalInstant localInstant)
        {
            return TimeOfDayCalculator.GetMillisecondOfDay(localInstant);
        }

        internal override int GetSecondOfMinute(LocalInstant localInstant)
        {
            return TimeOfDayCalculator.GetSecondOfMinute(localInstant);
        }

        internal override int GetSecondOfDay(LocalInstant localInstant)
        {
            return TimeOfDayCalculator.GetSecondOfDay(localInstant);
        }

        internal override int GetMinuteOfHour(LocalInstant localInstant)
        {
            return TimeOfDayCalculator.GetMinuteOfHour(localInstant);
        }

        internal override int GetMinuteOfDay(LocalInstant localInstant)
        {
            return TimeOfDayCalculator.GetMinuteOfDay(localInstant);
        }

        internal override int GetHourOfDay(LocalInstant localInstant)
        {
            return TimeOfDayCalculator.GetHourOfDay(localInstant);
        }

        internal override int GetHourOfHalfDay(LocalInstant localInstant)
        {
            return TimeOfDayCalculator.GetHourOfHalfDay(localInstant);
        }

        internal override int GetClockHourOfHalfDay(LocalInstant localInstant)
        {
            return TimeOfDayCalculator.GetClockHourOfHalfDay(localInstant);
        }

        internal override int GetDayOfWeek(LocalInstant localInstant)
        {
            return WeekYearCalculator.GetDayOfWeek(localInstant);
        }

        internal override int GetDayOfMonth(LocalInstant localInstant)
        {
            return yearMonthDayCalculator.GetDayOfMonth(localInstant);
        }

        internal override int GetDayOfYear(LocalInstant localInstant)
        {
            return yearMonthDayCalculator.GetDayOfYear(localInstant);
        }

        internal override int GetWeekOfWeekYear(LocalInstant localInstant)
        {
            return weekYearCalculator.GetWeekOfWeekYear(localInstant);
        }

        internal override int GetWeekYear(LocalInstant localInstant)
        {
            return weekYearCalculator.GetWeekYear(localInstant);
        }

        internal override int GetMonthOfYear(LocalInstant localInstant)
        {
            return yearMonthDayCalculator.GetMonthOfYear(localInstant);
        }

        internal override int GetYear(LocalInstant localInstant)
        {
            return yearMonthDayCalculator.GetYear(localInstant);
        }

        internal override int GetYearOfCentury(LocalInstant localInstant)
        {
            return yearMonthDayCalculator.GetYearOfCentury(localInstant);
        }

        internal override int GetYearOfEra(LocalInstant localInstant)
        {
            return yearMonthDayCalculator.GetYearOfEra(localInstant);
        }

        internal override int GetCenturyOfEra(LocalInstant localInstant)
        {
            return yearMonthDayCalculator.GetCenturyOfEra(localInstant);
        }

        internal override int GetEra(LocalInstant localInstant)
        {
            return yearMonthDayCalculator.GetEra(localInstant);
        }

        private sealed class MonthsPeriodField : IPeriodField
        {
            private readonly YearMonthDayCalculator calculator;

            internal MonthsPeriodField(YearMonthDayCalculator calculator)
            {
                this.calculator = calculator;
            }

            public LocalInstant Add(LocalInstant localInstant, long value)
            {
                // We don't try to work out the actual bounds, but we can easily tell
                // that we're out of range. Anything not in the range of an int is definitely broken.
                if (value < int.MinValue || value > int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                return calculator.AddMonths(localInstant, (int)value);
            }

            public long Subtract(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
            {
                int minuendYear = calculator.GetYear(minuendInstant);
                int subtrahendYear = calculator.GetYear(subtrahendInstant);
                int minuendMonth = calculator.GetMonthOfYear(minuendInstant);
                int subtrahendMonth = calculator.GetMonthOfYear(subtrahendInstant);

                int diff = (minuendYear - subtrahendYear) * calculator.MonthsInYear + minuendMonth - subtrahendMonth;

                // If we just add the difference in months to subtrahendInstant, what do we get?
                LocalInstant simpleAddition = Add(subtrahendInstant, diff);

                if (subtrahendInstant <= minuendInstant)
                {
                    // Moving forward: if the result of the simple addition is before or equal to the minuend,
                    // we're done. Otherwise, rewind a month because we've overshot.
                    return simpleAddition <= minuendInstant ? diff : diff - 1;
                }
                else
                {
                    // Moving backward: if the result of the simple addition (of a non-positive number)
                    // is after or equal to the minuend, we're done. Otherwise, increment by a month because
                    // we've overshot backwards.
                    return simpleAddition >= minuendInstant ? diff : diff + 1;
                }
            }
        }

        // TODO: Remove duplication in Int64Difference
        private sealed class YearsPeriodField : IPeriodField
        {
            private readonly YearMonthDayCalculator calculator;

            internal YearsPeriodField(YearMonthDayCalculator calculator)
            {
                this.calculator = calculator;
            }

            public LocalInstant Add(LocalInstant localInstant, long value)
            {
                int currentYear = calculator.GetYear(localInstant);
                // Adjust argument range based on current year
                Preconditions.CheckArgumentRange("value", value, calculator.MinYear - currentYear, calculator.MaxYear - currentYear);
                // If we got this far, the conversion to int must be fine.
                int intValue = (int)value;
                return calculator.SetYear(localInstant, intValue + currentYear);
            }

            public long Subtract(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
            {
                int minuendYear = calculator.GetYear(minuendInstant);
                int subtrahendYear = calculator.GetYear(subtrahendInstant);

                int diff = minuendYear - subtrahendYear;

                // If we just add the difference in years to subtrahendInstant, what do we get?
                LocalInstant simpleAddition = Add(subtrahendInstant, diff);

                if (subtrahendInstant <= minuendInstant)
                {
                    // Moving forward: if the result of the simple addition is before or equal to the minuend,
                    // we're done. Otherwise, rewind a year because we've overshot.
                    return simpleAddition <= minuendInstant ? diff : diff - 1;
                }
                else
                {
                    // Moving backward: if the result of the simple addition (of a non-positive number)
                    // is after or equal to the minuend, we're done. Otherwise, increment by a year because
                    // we've overshot backwards.
                    return simpleAddition >= minuendInstant ? diff : diff + 1;
                }
            }
        }
    }
}
