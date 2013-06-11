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
            int minuendYear = calendarSystem.GetYear(minuendInstant);
            int subtrahendYear = calendarSystem.GetYear(subtrahendInstant);
            int minuendMonth = calendarSystem.GetMonthOfYear(minuendInstant);
            int subtrahendMonth = calendarSystem.GetMonthOfYear(subtrahendInstant);

            int diff = (minuendYear - subtrahendYear) * monthsPerYear + minuendMonth - subtrahendMonth;

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
}
