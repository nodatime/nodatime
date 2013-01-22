// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Provides time calculations for the week of a week based year component of time.
    /// </summary>
    // Needs partial and max for set support.
    internal sealed class BasicWeekOfWeekYearDateTimeField : FixedLengthPeriodDateTimeField
    {
        private static readonly Duration ThreeDays = Duration.FromStandardDays(3);
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicWeekOfWeekYearDateTimeField(BasicCalendarSystem calendarSystem, PeriodField weeks) : base(DateTimeFieldType.WeekOfWeekYear, weeks)
        {
            this.calendarSystem = calendarSystem;
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetWeekOfWeekYear(localInstant);
        }

        internal override int GetValue(LocalInstant localInstant)
        {
            return calendarSystem.GetWeekOfWeekYear(localInstant);
        }

        internal override PeriodField RangePeriodField { get { return calendarSystem.Fields.WeekYears; } }

        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return base.RoundFloor(localInstant + ThreeDays) - ThreeDays;
        }

        internal override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return base.RoundCeiling(localInstant + ThreeDays) - ThreeDays;
        }

        internal override Duration Remainder(LocalInstant localInstant)
        {
            return base.Remainder(localInstant + ThreeDays);
        }

        internal override long GetMinimumValue()
        {
            return 1;
        }

        internal override long GetMaximumValue()
        {
            return 53;
        }

        internal override long GetMaximumValue(LocalInstant localInstant)
        {
            int weekyear = calendarSystem.GetWeekYear(localInstant);
            return calendarSystem.GetWeeksInYear(weekyear);
        }
    }
}