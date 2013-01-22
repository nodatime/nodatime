// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// Provides time calculations for the month of the year component of time.
    /// </summary>
    internal sealed class BasicMonthOfYearDateTimeField : DateTimeField
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
            Preconditions.CheckArgumentRange("value", value, MinimumValue, max);

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