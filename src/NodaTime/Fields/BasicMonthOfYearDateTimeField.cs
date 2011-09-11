#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Provides time calculations for the month of the year component of time.
    /// </summary>
    internal class BasicMonthOfYearDateTimeField : ImpreciseDateTimeField
    {
        private const int MinimumValue = 1;

        private readonly BasicCalendarSystem calendarSystem;
        private readonly int max;
        private readonly int leapMonth;

        internal BasicMonthOfYearDateTimeField(BasicCalendarSystem calendarSystem, int leapMonth)
            : base(DateTimeFieldType.MonthOfYear, calendarSystem.AverageTicksPerMonth)
        {
            this.calendarSystem = calendarSystem;
            max = calendarSystem.GetMaxMonth();
            this.leapMonth = leapMonth;
        }

        internal override DurationField RangeDurationField { get { return calendarSystem.Fields.Years; } }

        internal override bool IsLenient { get { return false; } }

        #region Values
        /// <summary>
        /// Get the Month component of the specified local instant.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The month extracted from the input.</returns>
        internal override int GetValue(LocalInstant localInstant)
        {
            return calendarSystem.GetMonthOfYear(localInstant);
        }

        /// <summary>
        /// Get the Month component of the specified local instant.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The month extracted from the input.</returns>
        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetMonthOfYear(localInstant);
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
            long timePart = calendarSystem.GetTickOfDay(localInstant);
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
                yearToUse = thisYear + (monthToUse / max);
                monthToUse = (monthToUse % max) + 1;
            }
            else
            {
                yearToUse = thisYear + (monthToUse / max) - 1;
                monthToUse = Math.Abs(monthToUse);
                int remMonthToUse = monthToUse % max;
                // Take care of the boundary condition
                if (remMonthToUse == 0)
                {
                    remMonthToUse = max;
                }
                monthToUse = max - remMonthToUse + 1;
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
            long timePart = calendarSystem.GetTickOfDay(localInstant);
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
                yearToUse = thisYear + (monthToUse / max);
                monthToUse = (monthToUse % max) + 1;
            }
            else
            {
                yearToUse = thisYear + (monthToUse / max) - 1;
                monthToUse = Math.Abs(monthToUse);
                int remMonthToUse = (int)(monthToUse % max);
                // Take care of the boundary condition
                if (remMonthToUse == 0)
                {
                    remMonthToUse = max;
                }
                monthToUse = max - remMonthToUse + 1;
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

        /// <summary>
        /// Add to the Month component of the specified time instant
        /// wrapping around within that component if necessary.
        /// </summary>
        /// <param name="localInstant">The local instant to update</param>
        /// <param name="value">The months to add (can be negative)</param>
        /// <returns>The updated local instant</returns>
        internal override LocalInstant AddWrapField(LocalInstant localInstant, int value)
        {
            int months = value;
            return SetValue(localInstant, FieldUtils.GetWrappedValue(GetValue(localInstant), months, MinimumValue, max));
        }
        /*
        internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            if (minuendInstant < subtrahendInstant)
            {
                return -GetInt64Difference(subtrahendInstant, minuendInstant);
            }
            int minuendYear = calendarSystem.GetYear(minuendInstant);
            int minuendMonth = calendarSystem.GetMonthOfYear(minuendInstant, minuendYear);
            int subtrahendYear = calendarSystem.GetYear(subtrahendInstant);
            int subtrahendMonth = calendarSystem.GetMonthOfYear(subtrahendInstant, subtrahendYear);

            long difference = (minuendYear - subtrahendYear) * ((long)max) + minuendMonth - subtrahendMonth;

            // Before adjusting for remainder, account for special case of add
            // where the day-of-month is forced to the nearest sane value.
            int minuendDom = calendarSystem.GetDayOfMonth(minuendInstant, minuendYear, minuendMonth);
            if (minuendDom == calendarSystem.GetDaysInMonth(minuendYear, minuendMonth))
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
         */

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            FieldUtils.VerifyValueBounds(this, value, MinimumValue, max);

            int month = (int)value;
            int thisYear = calendarSystem.GetYear(localInstant);
            int thisDom = calendarSystem.GetDayOfMonth(localInstant, thisYear);
            int maxDom = calendarSystem.GetDaysInMonth(thisYear, month);
            if (thisDom > maxDom)
            {
                // Quietly force DOM to nearest sane value.
                thisDom = maxDom;
            }
            return new LocalInstant(calendarSystem.GetYearMonthDayTicks(thisYear, month, thisDom) + calendarSystem.GetTickOfDay(localInstant));
        }
        #endregion

        #region Leap
        internal override bool IsLeap(LocalInstant localInstant)
        {
            int thisYear = calendarSystem.GetYear(localInstant);
            return calendarSystem.IsLeapYear(thisYear) && calendarSystem.GetMonthOfYear(localInstant, thisYear) == leapMonth;
        }

        internal override int GetLeapAmount(LocalInstant localInstant)
        {
            return IsLeap(localInstant) ? 1 : 0;
        }

        internal override DurationField LeapDurationField { get { return calendarSystem.Fields.Days; } }
        #endregion

        #region Ranges
        internal override long GetMinimumValue()
        {
            return MinimumValue;
        }

        internal override long GetMaximumValue()
        {
            return max;
        }
        #endregion

        #region Rounding
        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            int year = calendarSystem.GetYear(localInstant);
            int month = calendarSystem.GetMonthOfYear(localInstant, year);
            return new LocalInstant(calendarSystem.GetYearMonthTicks(year, month));
        }

        internal override Duration Remainder(LocalInstant localInstant)
        {
            return localInstant - RoundFloor(localInstant);
        }
        #endregion
    }
}