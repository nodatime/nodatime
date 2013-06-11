// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
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

        internal override LocalInstant Add(LocalInstant localInstant, long value)
        {
            // We don't try to work out the actual bounds, but we can easily tell
            // that we're out of range. Anything not in the range of an int is definitely broken.
            if (value < int.MinValue || value > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            int intValue = (int)value;

            int currentYear = calendarSystem.GetYear(localInstant);
            // Adjust argument range based on current year
            Preconditions.CheckArgumentRange("value", intValue, calendarSystem.MinYear - currentYear, calendarSystem.MaxYear - currentYear);
            return calendarSystem.SetYear(localInstant, intValue + currentYear);
        }

        internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            int minuendYear = calendarSystem.GetYear(minuendInstant);
            int subtrahendYear = calendarSystem.GetYear(subtrahendInstant);

            int diff = minuendYear - subtrahendYear;

            // If we just add the difference in years to subtrahendInstant, what do we get?
            LocalInstant simpleAddition = Add(subtrahendInstant, diff);

            if (subtrahendInstant <= minuendInstant)
            {
                // Moving forward: if the result of the simple addition is before or equal to the minuend,
                // we're done. Otherwise, rewind a year because we've overshot.
                return simpleAddition <= minuendInstant ? diff : diff - 1;
            }
            else
            {
                // Moving backward: if the result of the simple addition (of a non-positive number)
                // is after or equal to the minuend, we're done. Otherwise, increment by a year because
                // we've overshot backwards.
                return simpleAddition >= minuendInstant ? diff : diff + 1;
            }
        }
    }
}
