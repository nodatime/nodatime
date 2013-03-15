// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// A year field suitable for many calendars.
    /// </summary>
    internal sealed class BasicYearDateTimeField : DateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicYearDateTimeField(BasicCalendarSystem calendarSystem)
            : base(DateTimeFieldType.Year, new BasicYearPeriodField(calendarSystem))
        {
            this.calendarSystem = calendarSystem;
        }

        #region Values
        /// <summary>
        /// Get the Year component of the specified local instant.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The year extracted from the input.</returns>
        internal override int GetValue(LocalInstant localInstant)
        {
            return calendarSystem.GetYear(localInstant);
        }

        /// <summary>
        /// Get the Year component of the specified local instant.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The year extracted from the input.</returns>
        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetYear(localInstant);
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            Preconditions.CheckArgumentRange("value", value, calendarSystem.MinYear, calendarSystem.MaxYear);
            return calendarSystem.SetYear(localInstant, (int)value);
        }
        #endregion

        #region Leap
        internal override bool IsLeap(LocalInstant localInstant)
        {
            return calendarSystem.IsLeapYear(GetValue(localInstant));
        }

        internal override int GetLeapAmount(LocalInstant localInstant)
        {
            return IsLeap(localInstant) ? 1 : 0;
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

        #region Rounding
        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return new LocalInstant(calendarSystem.GetYearTicks(GetValue(localInstant)));
        }

        internal override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            int year = GetValue(localInstant);
            long yearStartTicks = calendarSystem.GetYearTicks(year);
            return localInstant.Ticks == yearStartTicks ? localInstant : new LocalInstant(calendarSystem.GetYearTicks(year + 1));
        }
        #endregion
    }
}