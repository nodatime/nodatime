// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;

namespace NodaTime.Fields
{
    internal sealed class BasicWeekYearPeriodField : VaryiableLengthPeriodField
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
            DateTimeField field = calendarSystem.Fields.WeekYear;

            if (minuendInstant < subtrahendInstant)
            {
                return -GetInt64Difference(subtrahendInstant, minuendInstant);
            }
            int minuendWeekYear = field.GetValue(minuendInstant);
            int subtrahendWeekYear = field.GetValue(subtrahendInstant);

            Duration minuendRemainder = field.Remainder(minuendInstant);
            Duration subtrahendRemainder = field.Remainder(subtrahendInstant);

            // Balance leap weekyear differences on remainders.
            if (subtrahendRemainder >= Week53Ticks && calendarSystem.GetWeeksInYear(minuendWeekYear) <= 52)
            {
                subtrahendRemainder -= Duration.OneStandardWeek;
            }

            int difference = minuendWeekYear - subtrahendWeekYear;
            if (minuendRemainder < subtrahendRemainder)
            {
                difference--;
            }
            return difference;
        }
    }
}
