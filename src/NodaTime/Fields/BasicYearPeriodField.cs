// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// Period field for years in a basic calendar system with a fixed number of months of varying lengths.
    /// </summary>
    internal sealed class BasicYearPeriodField : VariableLengthPeriodField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicYearPeriodField(BasicCalendarSystem calendarSystem) : base(PeriodFieldType.Years, calendarSystem.AverageTicksPerYear)
        {
            this.calendarSystem = calendarSystem;
        }

        internal override LocalInstant Add(LocalInstant localInstant, int value)
        {
            int currentYear = calendarSystem.GetYear(localInstant);
            // Adjust argument range based on current year
            Preconditions.CheckArgumentRange("value", value, calendarSystem.MinYear - currentYear, calendarSystem.MaxYear - currentYear);
            return calendarSystem.SetYear(localInstant, value + currentYear);
        }

        internal override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return Add(localInstant, (int) value);
        }

        internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return minuendInstant < subtrahendInstant
                ? -calendarSystem.GetYearDifference(subtrahendInstant, minuendInstant)
                : calendarSystem.GetYearDifference(minuendInstant, subtrahendInstant);
        }
    }
}
