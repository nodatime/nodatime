// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Porting status: need text.
    /// </summary>
    internal sealed class GJDayOfWeekDateTimeField : FixedLengthPeriodDateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal GJDayOfWeekDateTimeField(BasicCalendarSystem calendarSystem, PeriodField days) : base(DateTimeFieldType.DayOfWeek, days)
        {
            this.calendarSystem = calendarSystem;
        }

        internal override int GetValue(LocalInstant localInstant)
        {
            return calendarSystem.GetDayOfWeek(localInstant);
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetDayOfWeek(localInstant);
        }

        internal override PeriodField RangePeriodField { get { return calendarSystem.Fields.Weeks; } }

        internal override long GetMaximumValue()
        {
            return (long)IsoDayOfWeek.Sunday;
        }

        internal override long GetMinimumValue()
        {
            return (long)IsoDayOfWeek.Monday;
        }
    }
}