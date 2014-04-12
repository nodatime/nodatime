// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Calendars
{
    /// <summary>
    /// Implementation of the Persian (Solar Hijri) calendar. This is an algorithmic
    /// implementation rather than the true observational version, and it follows the
    /// simple 33 year leap cycle implemented by .NET rather than the more complicated
    /// form of variable-length cycles and grand cycles devised by Ahmad Birashk.
    /// </summary>
    internal sealed class PersianYearMonthDayCalendar : RegularYearMonthDayCalculator
    {
        // This is a long because we're notionally handling 33 bits. The top bit is
        // false anyway, but IsLeapYear shifts a long for simplicity, so let's be consistent with that.
        private const long LeapYearPatternBits = (1L << 1) | (1L << 5) | (1L << 9) | (1L << 13)
            | (1L << 17) | (1L << 22) | (1L << 26) | (1L << 30);
        private const int LeapYearCycleLength = 33;
        private const int DaysPerNonLeapYear = (31 * 6) + (30 * 5) + 29;
        private const int DaysPerLeapYear = DaysPerNonLeapYear + 1;
        private const int DaysPerLeapCycle = DaysPerNonLeapYear * 25 + DaysPerLeapYear * 8;
        private const long AverageTicksPerYear = (DaysPerLeapCycle * NodaConstants.TicksPerStandardDay) / 33;

        /// <summary>The ticks for the epoch of March 21st 622CE.</summary>
        private const long TicksAtStartOfYear1Constant = -425319552000000000L;
        private const int DaysAtStartOfYear1 = (int) (TicksAtStartOfYear1Constant / NodaConstants.TicksPerStandardDay);

        private static readonly long[] TotalTicksByMonth;

        static PersianYearMonthDayCalendar()
        {
            long ticks = 0;
            TotalTicksByMonth = new long[13];
            for (int i = 1; i <= 12; i++)
            {
                TotalTicksByMonth[i] = ticks;
                int days = i <= 6 ? 31 : 30;
                // This doesn't take account of leap years, but that doesn't matter - because
                // it's not used on the last iteration, and leap years only affect the final month
                // in the Persian calendar.
                ticks += days * NodaConstants.TicksPerStandardDay;
            }
        }

        internal PersianYearMonthDayCalendar()
            : base(1, 30574, 12, DaysPerNonLeapYear * NodaConstants.TicksPerStandardDay,
                   AverageTicksPerYear, TicksAtStartOfYear1Constant, Era.AnnoPersico)
        {
        }

        protected override long GetTicksFromStartOfYearToStartOfMonth(int year, int month)
        {
            return TotalTicksByMonth[month];
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

        protected override int GetMonthOfYear(LocalInstant localInstant, int year)
        {
            int dayOfYearZeroBased = (int)((localInstant.Ticks - GetStartOfYearInTicks(year)) / NodaConstants.TicksPerStandardDay);
            if (dayOfYearZeroBased == DaysPerLeapYear - 1)
            {
                return 12;
            }
            // First 6 months are all 31 days, which makes it a simple division.
            if (dayOfYearZeroBased < 6 * 31)
            {
                return dayOfYearZeroBased / 31 + 1;
            }
            // Remaining months are 30 days (until the last one, for a non-leap year), so just take the
            // first six months as read, divide by 30 and then re-add the first 6 months.
            return (dayOfYearZeroBased - 6 * 31) / 30 + 7;
        }

        internal override LocalInstant SetYear(LocalInstant localInstant, int year)
        {
            // Optimized implementation of SetYear, due to fixed months.
            int thisYear = GetYear(localInstant);
            int dayOfYear = GetDayOfYear(localInstant, thisYear);
            // Truncate final day of leap year.
            if (dayOfYear == DaysPerLeapYear && !IsLeapYear(year))
            {
                dayOfYear--;
            }
            long tickOfDay = TimeOfDayCalculator.GetTickOfDay(localInstant);

            return new LocalInstant(GetStartOfYearInTicks(year) + ((dayOfYear - 1) * NodaConstants.TicksPerStandardDay) + tickOfDay);
        }

        internal override int GetDaysInMonth(int year, int month)
        {
            return month < 7 ? 31
                : month < 12 ? 30
                : IsLeapYear(year) ? 30 : 29;
        }

        internal override int GetDayOfMonth(LocalInstant localInstant)
        {
            int zeroBasedDayOfYear = GetDayOfYear(localInstant) - 1;
            if (zeroBasedDayOfYear < 6 * 31)
            {
                return (zeroBasedDayOfYear % 31) + 1;
            }
            // Leap years are irrelevant here, as a leap month at the end of the year
            // will work the same as a regular 30-day month.
            return ((zeroBasedDayOfYear - 6 * 31) % 30) + 1;
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
