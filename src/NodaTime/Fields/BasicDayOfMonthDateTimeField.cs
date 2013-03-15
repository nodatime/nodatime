// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Provides time calculations for the day of the month component of time.
    /// </summary>
    // Porting status: Needs partial and max for set support.
    internal sealed class BasicDayOfMonthDateTimeField : FixedLengthPeriodDateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicDayOfMonthDateTimeField(BasicCalendarSystem calendarSystem, PeriodField days) : base(DateTimeFieldType.DayOfMonth, days)
        {
            this.calendarSystem = calendarSystem;
        }

        internal override int GetValue(LocalInstant localInstant)
        {
            return calendarSystem.GetDayOfMonth(localInstant);
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetDayOfMonth(localInstant);
        }

        internal override long GetMaximumValue()
        {
            return calendarSystem.GetMaxDaysInMonth();
        }

        internal override long GetMaximumValue(LocalInstant localInstant)
        {
            return calendarSystem.GetMaxDaysInMonth(localInstant);
        }

        internal override long GetMinimumValue()
        {
            return 1;
        }
    }
}