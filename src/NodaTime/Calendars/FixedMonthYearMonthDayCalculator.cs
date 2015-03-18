// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Calendars
{
    /// <summary>
    /// Abstract implementation of a year/month/day calculator based around months which always have 30 days.
    /// </summary>
    /// <remarks>
    /// As the month length is fixed various calculations can be optimised.
    /// This implementation assumes any additional days after twelve
    /// months fall into a thirteenth month.
    /// </remarks>
    internal abstract class FixedMonthYearMonthDayCalculator : RegularYearMonthDayCalculator
    {
        private const int DaysInMonth = 30;

        private const int AverageDaysPer10Years = 3653; // Ideally 365.25 days per year...

        protected FixedMonthYearMonthDayCalculator(int minYear, int maxYear, int daysAtStartOfYear1)
            : base(minYear, maxYear, 13, AverageDaysPer10Years, daysAtStartOfYear1)
        {
        }

        internal override int GetDaysSinceEpoch(YearMonthDay yearMonthDay) =>
            // Just inline the arithmetic that would be done via various methods.
            GetStartOfYearInDays(yearMonthDay.Year)
                   + (yearMonthDay.Month - 1) * DaysInMonth
                   + (yearMonthDay.Day - 1);

        protected override int GetDaysFromStartOfYearToStartOfMonth(int year, int month) => (month - 1) * DaysInMonth;

        internal override bool IsLeapYear(int year) => (year & 3) == 3;

        internal override int GetDaysInYear(int year) => IsLeapYear(year) ? 366 : 365;

        internal override int GetDaysInMonth(int year, int month) => month != 13 ? DaysInMonth : IsLeapYear(year) ? 6 : 5;

        internal override YearMonthDay GetYearMonthDay(int year, int dayOfYear)
        {
            int zeroBasedDayOfYear = dayOfYear - 1;
            int month = zeroBasedDayOfYear / DaysInMonth + 1;
            int day = zeroBasedDayOfYear % DaysInMonth + 1;
            return new YearMonthDay(year, month, day);
        }
    }
}
