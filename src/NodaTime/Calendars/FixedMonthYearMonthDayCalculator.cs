// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;
using System.Collections.Generic;

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
    internal abstract class FixedMonthYearMonthDayCalculator : YearMonthDayCalculator
    {
        private const int DaysInMonth = 30;

        // Number of ticks in all but the "short" month.
        private const long TicksPerMonth = DaysInMonth * NodaConstants.TicksPerStandardDay;

        protected const long AverageTicksPerYear = (long)(365.25 * NodaConstants.TicksPerStandardDay);

        protected FixedMonthYearMonthDayCalculator(int minYear, int maxYear,
            long approxTicksAtEpoch, IList<Era> eras)
            : base(minYear, maxYear, 13, AverageTicksPerYear,
                   approxTicksAtEpoch, eras)
        {
        }

        internal override LocalInstant SetYear(LocalInstant localInstant, int year)
        {
            // Optimized implementation of set, due to fixed months
            int thisYear = GetYear(localInstant);
            int dayOfYear = GetDayOfYear(localInstant, thisYear);
            long tickOfDay = TimeOfDayCalculator.GetTickOfDay(localInstant);

            if (dayOfYear > 365)
            {
                // Current year is leap, and day is leap.
                if (!IsLeapYear(year))
                {
                    // Moving to a non-leap year, leap day doesn't exist.
                    dayOfYear--;
                }
            }

            long ticks = GetYearTicks(year) + (dayOfYear - 1) * NodaConstants.TicksPerStandardDay + tickOfDay;
            return new LocalInstant(ticks);
        }

        internal override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth)
        {
            Preconditions.CheckArgumentRange("year", year, MinYear, MaxYear);
            Preconditions.CheckArgumentRange("monthOfYear", monthOfYear, 1, 13);
            Preconditions.CheckArgumentRange("dayOfMonth", dayOfMonth, 1, GetDaysInMonth(year, monthOfYear));

            // Just inline the arithmetic that would be done via various methods.
            long ticks = GetYearTicks(year) + (monthOfYear - 1) * TicksPerMonth + (dayOfMonth - 1) * NodaConstants.TicksPerStandardDay;
            return new LocalInstant(ticks);
        }
        
        protected override long GetTicksFromStartOfYearToStartOfMonth(int year, int month)
        {
            return (month - 1) * TicksPerMonth;
        }

        internal override int GetDayOfMonth(LocalInstant localInstant)
        {
            // Optimized for fixed months
            return (GetDayOfYear(localInstant) - 1) % DaysInMonth + 1;
        }

        internal override bool IsLeapYear(int year)
        {
            return (year & 3) == 3;
        }

        internal override int GetDaysInMonth(int year, int month)
        {
            return month != 13 ? DaysInMonth : IsLeapYear(year) ? 6 : 5;
        }

        internal override int GetDaysInMonthMax(int month)
        {
            return month != 13 ? DaysInMonth : 6;
        }

        internal override int GetMonthOfYear(LocalInstant localInstant)
        {
            return (GetDayOfYear(localInstant) - 1) / DaysInMonth + 1;
        }

        protected internal override int GetMonthOfYear(LocalInstant localInstant, int year)
        {
            long monthZeroBased = (localInstant.Ticks - GetYearTicks(year)) / TicksPerMonth;
            return ((int)monthZeroBased) + 1;
        }
    }
}
