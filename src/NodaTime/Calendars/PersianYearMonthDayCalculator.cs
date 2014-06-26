// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Diagnostics.CodeAnalysis;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Implementation of the Persian (Solar Hijri) calendar. This is an algorithmic
    /// implementation rather than the true observational version, and it follows the
    /// simple 33 year leap cycle implemented by .NET rather than the more complicated
    /// form of variable-length cycles and grand cycles devised by Ahmad Birashk.
    /// </summary>
    internal sealed class PersianYearMonthDayCalculator : RegularYearMonthDayCalculator
    {
        // This is a long because we're notionally handling 33 bits. The top bit is
        // false anyway, but IsLeapYear shifts a long for simplicity, so let's be consistent with that.
        private const long LeapYearPatternBits = (1L << 1) | (1L << 5) | (1L << 9) | (1L << 13)
            | (1L << 17) | (1L << 22) | (1L << 26) | (1L << 30);
        private const int LeapYearCycleLength = 33;
        private const int DaysPerNonLeapYear = (31 * 6) + (30 * 5) + 29;
        private const int DaysPerLeapYear = DaysPerNonLeapYear + 1;
        private const int DaysPerLeapCycle = DaysPerNonLeapYear * 25 + DaysPerLeapYear * 8;
        private const int AverageDaysPer10Years = (DaysPerLeapCycle * 10) / 33;

        /// <summary>The ticks for the epoch of March 21st 622CE.</summary>
        private const int DaysAtStartOfYear1Constant = -492268;

        private static readonly int[] TotalDaysByMonth;

        static PersianYearMonthDayCalculator()
        {
            int days = 0;
            TotalDaysByMonth = new int[13];
            for (int i = 1; i <= 12; i++)
            {
                TotalDaysByMonth[i] = days;
                int daysInMonth = i <= 6 ? 31 : 30;
                // This doesn't take account of leap years, but that doesn't matter - because
                // it's not used on the last iteration, and leap years only affect the final month
                // in the Persian calendar.
                days += daysInMonth;
            }
        }

        internal PersianYearMonthDayCalculator()
            : base(1, 30574, 12, AverageDaysPer10Years, DaysAtStartOfYear1Constant, Era.AnnoPersico)
        {
        }

        protected override int GetDaysFromStartOfYearToStartOfMonth(int year, int month)
        {
            return TotalDaysByMonth[month];
        }

        protected override int CalculateStartOfYearDays(int year)
        {
            // The first cycle starts in year 1, not year 0.
            // We try to cope with years outside the normal range, in order to allow arithmetic at the boundaries.
            int cycle = year > 0 ? (year - 1) / LeapYearCycleLength
                                 : (year - LeapYearCycleLength) / LeapYearCycleLength;
            int yearAtStartOfCycle = (cycle * LeapYearCycleLength) + 1;

            int days = DaysAtStartOfYear1 + cycle * DaysPerLeapCycle;

            // We've got the days at the start of the cycle (e.g. at the start of year 1, 34, 67 etc).
            // Now go from that year to (but not including) the year we're looking for, adding the right
            // number of days in each year. So if we're trying to find the start of year 37, we would
            // find the days at the start of year 34, then add the days *in* year 34, the days in year 35,
            // and the days in year 36.
            for (int i = yearAtStartOfCycle; i < year; i++)
            {
                days += GetDaysInYear(i);
            }
            return days;
        }

        internal override YearMonthDay GetYearMonthDay(int daysSinceEpoch)
        {
            int year = GetYear(daysSinceEpoch);
            int dayOfYearZeroBased = daysSinceEpoch - GetStartOfYearInDays(year);
            int month;
            int day;
            if (dayOfYearZeroBased == DaysPerLeapYear - 1)
            {
                // Last day of a leap year.
                month = 12;
                day = 31;
            }
            else if (dayOfYearZeroBased < 6 * 31)
            {
                // In the first 6 months, all of which are 31 days long.
                month = dayOfYearZeroBased / 31 + 1;
                day = (dayOfYearZeroBased % 31) + 1;
            }
            else
            {
                // Last 6 months (other than last day of leap year).
                // Work out where we are within that 6 month block, then use simple arithmetic.
                int dayOfSecondHalf = dayOfYearZeroBased - 6 * 31;
                month = dayOfSecondHalf / 30 + 7;
                day = (dayOfSecondHalf % 30) + 1;
            }
            return new YearMonthDay(year, month, day);
        }

        internal override YearMonthDay SetYear(YearMonthDay yearMonthDay, int year)
        {
            int month = yearMonthDay.Month;
            int day = yearMonthDay.Day;
            // The only value which might change day is the last day of a leap year
            if (month == 12 && day == 30 && !IsLeapYear(year))
            {
                day = 29;
            }
            return new YearMonthDay(year, month, day);
        }

        internal override int GetDaysInMonth(int year, int month)
        {
            return month < 7 ? 31
                : month < 12 ? 30
                : IsLeapYear(year) ? 30 : 29;
        }

        internal override bool IsLeapYear(int year)
        {
            // Handle negative years in order to make calculations near the start of the calendar work cleanly.
            int yearOfCycle = year >= 0 ? year % LeapYearCycleLength
                                        : (year % LeapYearCycleLength) + LeapYearCycleLength;
            // Note the shift of 1L rather than 1, to avoid issues where shifting by 32
            // would get us back to 1.
            long key = 1L << yearOfCycle;
            return (LeapYearPatternBits & key) > 0;
        }

        internal override int GetDaysInYear(int year)
        {
            return IsLeapYear(year) ? DaysPerLeapYear : DaysPerNonLeapYear;
        }
    }
}
