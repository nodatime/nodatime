// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Date period field for fixed-length periods (weeks and days).
    /// </summary>
    internal sealed class FixedLengthDatePeriodField : IDatePeriodField
    {
        private readonly int unitDays;

        internal FixedLengthDatePeriodField(int unitDays)
        {
            this.unitDays = unitDays;
        }

        public LocalDate Add(LocalDate localDate, int value)
        {
            if (value == 0)
            {
                return localDate;
            }
            int daysToAdd = value * unitDays;
            if (daysToAdd < 20 && daysToAdd > -20)
            {
                YearMonthDayCalculator calculator = localDate.Calendar.YearMonthDayCalculator;
                YearMonthDay yearMonthDay = localDate.YearMonthDay;
                int year = yearMonthDay.Year;
                int month = yearMonthDay.Month;
                int day = yearMonthDay.Day;
                int newDayOfMonth = day + daysToAdd;
                if (1 <= newDayOfMonth && newDayOfMonth <= calculator.GetDaysInMonth(year, month))
                {
                    return new LocalDate(new YearMonthDay(year, month, newDayOfMonth), localDate.Calendar);
                }
                // TODO(2.0): Improve this if we're still in the same year. Would be reasonably simple
                // if we had GetYearMonthDay(year, dayOfYear).
            }
            int days = localDate.DaysSinceEpoch + daysToAdd;
            return new LocalDate(days, localDate.Calendar);
        }

        public int Subtract(LocalDate minuendDate, LocalDate subtrahendDate)
        {
            if (minuendDate == subtrahendDate)
            {
                return 0;
            }
            int minuendDays = minuendDate.DaysSinceEpoch;
            int subtrahendDays = subtrahendDate.DaysSinceEpoch;
            return (minuendDays - subtrahendDays) / unitDays;
        }
    }
}
