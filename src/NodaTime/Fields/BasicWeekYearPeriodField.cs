// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Calendars;

namespace NodaTime.Fields
{
    internal sealed class BasicWeekYearPeriodField : VariableLengthPeriodField
    {
        private static readonly Duration Week53Ticks = Duration.FromStandardWeeks(52);
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicWeekYearPeriodField(BasicCalendarSystem calendarSystem)
            : base(PeriodFieldType.WeekYears, calendarSystem.AverageTicksPerYear)
        {
            this.calendarSystem = calendarSystem;
        }

        internal override LocalInstant Add(LocalInstant localInstant, int value)
        {
            return calendarSystem.Fields.WeekYear.SetValue(localInstant, calendarSystem.GetYear(localInstant) + value);
        }

        internal override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return Add(localInstant, (int)value);
        }

        internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            throw new InvalidOperationException("GetInt64Difference unsupported for WeekYear");
        }
    }
}
