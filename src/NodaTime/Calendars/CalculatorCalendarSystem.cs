// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Fields;
using NodaTime.Utility;
using System;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Calendar system constructed from separate calculators for year/month/day, week-year/week/week-day and time fields.
    /// </summary>
    internal sealed class CalculatorCalendarSystem : CalendarSystem
    {
        private readonly YearMonthDayCalculator yearMonthDayCalculator;
        private readonly WeekYearCalculator weekYearCalculator;

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
                   (builder, @this) => AssembleFields(builder,
                                                      yearMonthDayCalculator,
                                                      weekYearCalculator),
                   yearMonthDayCalculator.Eras)
        {
            this.yearMonthDayCalculator = yearMonthDayCalculator;
            this.weekYearCalculator = weekYearCalculator;
        }

        private static void AssembleFields(FieldSet.Builder builder,
            YearMonthDayCalculator yearMonthDayCalculator,
            WeekYearCalculator weekYearCalculator)
        {
            // Time fields
            builder.WithSupportedFieldsFrom(TimeOfDayCalculator.TimeFields);

            // Era/year/month/day fields
            builder.DayOfMonth = new Int32DateTimeField(yearMonthDayCalculator.GetDayOfMonth);
            builder.DayOfYear = new Int32DateTimeField(yearMonthDayCalculator.GetDayOfYear);
            builder.MonthOfYear = new Int32DateTimeField(yearMonthDayCalculator.GetMonthOfYear);
            builder.Year = new Int32DateTimeField(yearMonthDayCalculator.GetYear);
            builder.YearOfEra = new Int32DateTimeField(yearMonthDayCalculator.GetYearOfEra);
            builder.YearOfCentury = new Int32DateTimeField(yearMonthDayCalculator.GetYearOfCentury);
            builder.CenturyOfEra = new Int32DateTimeField(yearMonthDayCalculator.GetCenturyOfEra);
            builder.Era = new Int32DateTimeField(yearMonthDayCalculator.GetEra);

            builder.Days = FixedLengthPeriodField.Days;
            builder.Weeks = FixedLengthPeriodField.Weeks;
            builder.Months = new YearsPeriodField(yearMonthDayCalculator);
            builder.Years = new MonthsPeriodField(yearMonthDayCalculator);

            // Week-year fields
            builder.DayOfWeek = new Int32DateTimeField(WeekYearCalculator.GetDayOfWeek);
            builder.WeekOfWeekYear = new Int32DateTimeField(weekYearCalculator.GetWeekOfWeekYear);
            builder.WeekYear = new Int32DateTimeField(weekYearCalculator.GetWeekYear);
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
            long timeTicks = TimeOfDayCalculator.GetTicks(hourOfDay, minuteOfHour, secondOfMinute, millisecondOfSecond);
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

        private class MonthsPeriodField : PeriodField
        {
            private readonly YearMonthDayCalculator calculator;

            // TODO: Remove superconstructor
            internal MonthsPeriodField(YearMonthDayCalculator calculator) : base(PeriodFieldType.Years, 1, false, true)
            {
                this.calculator = calculator;
            }

            internal override LocalInstant Add(LocalInstant localInstant, long value)
            {
                // We don't try to work out the actual bounds, but we can easily tell
                // that we're out of range. Anything not in the range of an int is definitely broken.
                if (value < int.MinValue || value > int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                return calculator.AddMonths(localInstant, (int) value);
            }

            internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
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
        private class YearsPeriodField : PeriodField
        {
            private readonly YearMonthDayCalculator calculator;

            // TODO: Remove superconstructor
            internal YearsPeriodField(YearMonthDayCalculator calculator) : base(PeriodFieldType.Years, 1, false, true)
            {
                this.calculator = calculator;
            }

            internal override LocalInstant Add(LocalInstant localInstant, long value)
            {
                int currentYear = calculator.GetYear(localInstant);
                // Adjust argument range based on current year
                Preconditions.CheckArgumentRange("value", value, calculator.MinYear - currentYear, calculator.MaxYear - currentYear);
                // If we got this far, the conversion to int must be fine.
                int intValue = (int)value;
                return calculator.SetYear(localInstant, intValue + currentYear);
            }

            internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
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
