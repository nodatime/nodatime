// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Subclass of YearMonthDayCalculator for calendars with the following attributes:
    /// <list type="bullet">
    /// <item>A fixed number of months</item>
    /// <item>Occasional leap years which are always 1 day longer than non-leap years</item>
    /// </list>
    /// </summary>
    internal abstract class RegularYearMonthDayCalculator : YearMonthDayCalculator
    {
        private readonly int monthsInYear;

        protected RegularYearMonthDayCalculator(int minYear, int maxYear, int monthsInYear,
            long averageTicksPerYear, long ticksAtStartOfYear1, params Era[] eras)
            : base(minYear, maxYear, averageTicksPerYear, ticksAtStartOfYear1, eras)
        {
            this.monthsInYear = monthsInYear;
        }

        internal override int GetMaxMonth(int year)
        {
            return monthsInYear;
        }

        override internal LocalInstant AddMonths(LocalInstant localInstant, int months)
        {
            if (months == 0)
            {
                return localInstant;
            }
            // Save the time part first
            long timePart = TimeOfDayCalculator.GetTickOfDay(localInstant);
            // Get the year and month
            int thisYear = GetYear(localInstant);
            int thisMonth = GetMonthOfYear(localInstant, thisYear);

            // Do not refactor without careful consideration.
            // Order of calculation is important.

            int yearToUse;
            // Initially, monthToUse is zero-based
            int monthToUse = thisMonth - 1 + months;
            if (monthToUse >= 0)
            {
                yearToUse = thisYear + (monthToUse / monthsInYear);
                monthToUse = (monthToUse % monthsInYear) + 1;
            }
            else
            {
                yearToUse = thisYear + (monthToUse / monthsInYear) - 1;
                monthToUse = Math.Abs(monthToUse);
                int remMonthToUse = monthToUse % monthsInYear;
                // Take care of the boundary condition
                if (remMonthToUse == 0)
                {
                    remMonthToUse = monthsInYear;
                }
                monthToUse = monthsInYear - remMonthToUse + 1;
                // Take care of the boundary condition
                if (monthToUse == 1)
                {
                    yearToUse++;
                }
            }
            // End of do not refactor.

            // Quietly force DOM to nearest sane value.
            int dayToUse = GetDayOfMonth(localInstant, thisYear, thisMonth);
            int maxDay = GetDaysInMonth(yearToUse, monthToUse);
            dayToUse = Math.Min(dayToUse, maxDay);
            // Get proper date part, and return result
            long datePart = GetYearMonthDayTicks(yearToUse, monthToUse, dayToUse);
            return new LocalInstant(datePart + timePart);
        }

        internal override int MonthsBetween(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            int minuendYear = GetYear(minuendInstant);
            int subtrahendYear = GetYear(subtrahendInstant);
            int minuendMonth = GetMonthOfYear(minuendInstant);
            int subtrahendMonth = GetMonthOfYear(subtrahendInstant);

            int diff = (minuendYear - subtrahendYear) * monthsInYear + minuendMonth - subtrahendMonth;

            // If we just add the difference in months to subtrahendInstant, what do we get?
            LocalInstant simpleAddition = AddMonths(subtrahendInstant, diff);

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
