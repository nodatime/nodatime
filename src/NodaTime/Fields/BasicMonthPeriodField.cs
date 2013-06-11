// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Period field for months in a basic calendar system with a fixed number of months of varying lengths.
    /// </summary>
    internal sealed class BasicMonthPeriodField : VariableLengthPeriodField
    {
        private readonly BasicCalendarSystem calendarSystem;
        private readonly int monthsPerYear;

        internal BasicMonthPeriodField(BasicCalendarSystem calendarSystem)
            : base(PeriodFieldType.Months, calendarSystem.AverageTicksPerMonth)
        {
            this.calendarSystem = calendarSystem;
            // Assumes 1-based value, and fixed number of months.
            monthsPerYear = calendarSystem.GetMaxMonth();
        }

        /// <summary>
        /// Add the specified month to the specified time instant.
        /// The amount added may be negative.
        /// </summary>
        /// <param name="localInstant">The local instant to update</param>
        /// <param name="value">The months to add (can be negative).</param>
        /// <returns>The updated local instant</returns>
        /// <remarks>
        /// If the new month has less total days than the specified
        /// day of the month, this value is coerced to the nearest
        /// sane value. e.g.
        /// 07-31 - (1 month) = 06-30
        /// 03-31 - (1 month) = 02-28 or 02-29 depending
        /// </remarks>
        internal override LocalInstant Add(LocalInstant localInstant, int value)
        {
            // Keep the parameter name the same as the original declaration, but
            // use a more meaningful name in the method
            int months = value;
            if (months == 0)
            {
                return localInstant;
            }
            // Save the time part first
            long timePart = BasicCalendarSystem.GetTickOfDay(localInstant);
            // Get the year and month
            int thisYear = calendarSystem.GetYear(localInstant);
            int thisMonth = calendarSystem.GetMonthOfYear(localInstant, thisYear);

            // Do not refactor without careful consideration.
            // Order of calculation is important.

            int yearToUse;
            // Initially, monthToUse is zero-based
            int monthToUse = thisMonth - 1 + months;
            if (monthToUse >= 0)
            {
                yearToUse = thisYear + (monthToUse / monthsPerYear);
                monthToUse = (monthToUse % monthsPerYear) + 1;
            }
            else
            {
                yearToUse = thisYear + (monthToUse / monthsPerYear) - 1;
                monthToUse = Math.Abs(monthToUse);
                int remMonthToUse = monthToUse % monthsPerYear;
                // Take care of the boundary condition
                if (remMonthToUse == 0)
                {
                    remMonthToUse = monthsPerYear;
                }
                monthToUse = monthsPerYear - remMonthToUse + 1;
                // Take care of the boundary condition
                if (monthToUse == 1)
                {
                    yearToUse++;
                }
            }
            // End of do not refactor.

            // Quietly force DOM to nearest sane value.
            int dayToUse = calendarSystem.GetDayOfMonth(localInstant, thisYear, thisMonth);
            int maxDay = calendarSystem.GetDaysInMonth(yearToUse, monthToUse);
            dayToUse = Math.Min(dayToUse, maxDay);
            // Get proper date part, and return result
            long datePart = calendarSystem.GetYearMonthDayTicks(yearToUse, monthToUse, dayToUse);
            return new LocalInstant(datePart + timePart);
        }

        internal override LocalInstant Add(LocalInstant localInstant, long value)
        {
            // Keep the parameter name the same as the original declaration, but
            // use a more meaningful name in the method
            long months = value;
            int intMonths = unchecked((int)months);
            if (intMonths == months)
            {
                return Add(localInstant, intMonths);
            }

            // Copied from Add(LocalInstant, int) and changed slightly
            // Save the time part first
            long timePart = BasicCalendarSystem.GetTickOfDay(localInstant);
            // Get the year and month
            int thisYear = calendarSystem.GetYear(localInstant);
            int thisMonth = calendarSystem.GetMonthOfYear(localInstant, thisYear);
            // Do not refactor without careful consideration.
            // Order of calculation is important.

            long yearToUse;

            // Initially, monthToUse is zero-based
            long monthToUse = thisMonth - 1 + months;
            if (monthToUse >= 0)
            {
                yearToUse = thisYear + (monthToUse / monthsPerYear);
                monthToUse = (monthToUse % monthsPerYear) + 1;
            }
            else
            {
                yearToUse = thisYear + (monthToUse / monthsPerYear) - 1;
                monthToUse = Math.Abs(monthToUse);
                int remMonthToUse = (int)(monthToUse % monthsPerYear);
                // Take care of the boundary condition
                if (remMonthToUse == 0)
                {
                    remMonthToUse = monthsPerYear;
                }
                monthToUse = monthsPerYear - remMonthToUse + 1;
                // Take care of the boundary condition
                if (monthToUse == 1)
                {
                    yearToUse++;
                }
            }
            // End of do not refactor.

            if (yearToUse < calendarSystem.MinYear || yearToUse > calendarSystem.MaxYear)
            {
                throw new ArgumentOutOfRangeException("value", "Magnitude of add amount is too large: " + months);
            }

            int intYearToUse = (int)yearToUse;
            int intMonthToUse = (int)monthToUse;

            // Quietly force DOM to nearest sane value.
            int dayToUse = calendarSystem.GetDayOfMonth(localInstant, thisYear, thisMonth);
            int maxDay = calendarSystem.GetDaysInMonth(intYearToUse, intMonthToUse);
            dayToUse = Math.Min(dayToUse, maxDay);
            // Get proper date part, and return result
            long datePart = calendarSystem.GetYearMonthDayTicks(intYearToUse, intMonthToUse, dayToUse);
            return new LocalInstant(datePart + timePart);
        }

        internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return minuendInstant >= subtrahendInstant
                ? GetInt64DifferenceImpl(minuendInstant, subtrahendInstant, true)
                : -GetInt64DifferenceImpl(subtrahendInstant, minuendInstant, false);
        }

        private long GetInt64DifferenceImpl(LocalInstant minuendInstant, LocalInstant subtrahendInstant, bool allowTruncation)
        {
            if (minuendInstant < subtrahendInstant)
            {
                return -GetInt64Difference(subtrahendInstant, minuendInstant);
            }
            int minuendYear = calendarSystem.GetYear(minuendInstant);
            int minuendMonth = calendarSystem.GetMonthOfYear(minuendInstant, minuendYear);
            int subtrahendYear = calendarSystem.GetYear(subtrahendInstant);
            int subtrahendMonth = calendarSystem.GetMonthOfYear(subtrahendInstant, subtrahendYear);

            long difference = (minuendYear - subtrahendYear) * ((long)monthsPerYear) + minuendMonth - subtrahendMonth;

            // Before adjusting for remainder, account for special case of add
            // where the day-of-month is forced to the nearest sane value... but only if we were originally
            // going forward.
            // For example, April 30th - March 31st is still one month, because if we add one month
            // to March 31st we'll truncate the value to April 30th. However, March 31st - April 30th is 0 months,
            // as if we add -1 month to April 30th we'd get March 30th (i.e. we'd overshoot).
            int minuendDom = calendarSystem.GetDayOfMonth(minuendInstant, minuendYear, minuendMonth);
            if (allowTruncation && minuendDom == calendarSystem.GetDaysInMonth(minuendYear, minuendMonth))
            {
                // Last day of the minuend month...
                int subtrahendDom = calendarSystem.GetDayOfMonth(subtrahendInstant, subtrahendYear, subtrahendMonth);
                if (subtrahendDom > minuendDom)
                {
                    // ...and day of subtrahend month is larger.
                    // Note: This works fine, but it ideally shouldn't invoke other
                    // fields from within a field.
                    subtrahendInstant = calendarSystem.Fields.DayOfMonth.SetValue(subtrahendInstant, minuendDom);
                }
            }

            // Inlined remainder method to avoid duplicate calls.
            long minuendRem = minuendInstant.Ticks - calendarSystem.GetYearMonthTicks(minuendYear, minuendMonth);
            long subtrahendRem = subtrahendInstant.Ticks - calendarSystem.GetYearMonthTicks(subtrahendYear, subtrahendMonth);

            if (minuendRem < subtrahendRem)
            {
                difference--;
            }

            return difference;
        }
    }
}
