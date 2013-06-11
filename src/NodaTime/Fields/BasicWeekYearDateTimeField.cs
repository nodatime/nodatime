// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Calendars;
using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// Provides time calculations for the week of the weekyear component of time.
    /// </summary>
    internal sealed class BasicWeekYearDateTimeField : DateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicWeekYearDateTimeField(BasicCalendarSystem calendarSystem)
            : base(DateTimeFieldType.WeekYear, UnsupportedPeriodField.WeekYears)
        {
            this.calendarSystem = calendarSystem;
        }

        #region Values
        /// <summary>
        /// Get the Year of a week based year component of the specified local instant.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The year extracted from the input.</returns>
        internal override int GetValue(LocalInstant localInstant)
        {
            return calendarSystem.GetWeekYear(localInstant);
        }

        /// <summary>
        /// Get the Year of a week based year component of the specified local instant.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The year extracted from the input.</returns>
        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetWeekYear(localInstant);
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            Preconditions.CheckArgumentRange("value", value, calendarSystem.MinYear, calendarSystem.MaxYear);

            int targetWeekYear = (int) value;
            
            // Find out how far we are through the week-year at the moment, and try to preserve that.
            int currentWeekYear = GetValue(localInstant);
            long startOfCurrentWeekYear = calendarSystem.GetWeekYearTicks(currentWeekYear);
            long ticksThroughYear = localInstant.Ticks - startOfCurrentWeekYear;

            int currentWeekOfWeekYear = calendarSystem.GetWeekOfWeekYear(localInstant);
            int weeksInTargetWeekYear = calendarSystem.GetWeeksInWeekYear(targetWeekYear);

            // If the target week year doesn't contain enough weeks, we'll need to go back one week.
            if (currentWeekOfWeekYear > weeksInTargetWeekYear)
            {
                ticksThroughYear -= NodaConstants.TicksPerStandardWeek;
            }

            long startOfTargetWeekYear = calendarSystem.GetWeekYearTicks(targetWeekYear);
            return new LocalInstant(startOfTargetWeekYear + ticksThroughYear);
        }
        #endregion

        #region Ranges
        internal override long GetMinimumValue()
        {
            return calendarSystem.MinYear;
        }

        internal override long GetMaximumValue()
        {
            return calendarSystem.MaxYear;
        }
        #endregion
    }
}