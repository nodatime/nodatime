#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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

using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Provides time calculations for the month of the year component of time.
    /// </summary>
    internal class BasicMonthOfYearDateTimeField : DateTimeField
    {
        private const int MinimumValue = 1;

        private readonly BasicCalendarSystem calendarSystem;
        private readonly int max;
        private readonly int leapMonth;

        internal BasicMonthOfYearDateTimeField(BasicCalendarSystem calendarSystem, int leapMonth)
            : base(DateTimeFieldType.MonthOfYear, new BasicMonthPeriodField(calendarSystem))
        {
            this.calendarSystem = calendarSystem;
            max = calendarSystem.GetMaxMonth();
            this.leapMonth = leapMonth;
        }

        internal override PeriodField RangePeriodField { get { return calendarSystem.Fields.Years; } }

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

        internal override PeriodField LeapPeriodField { get { return calendarSystem.Fields.Days; } }
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