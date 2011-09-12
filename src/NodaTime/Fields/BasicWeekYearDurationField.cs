#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using NodaTime.Calendars;

namespace NodaTime.Fields
{
    internal class BasicWeekYearDurationField : ImpreciseDurationField
    {
        private static readonly Duration Week53Ticks = Duration.FromStandardWeeks(52);
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicWeekYearDurationField(BasicCalendarSystem calendarSystem)
            : base(DurationFieldType.WeekYears, calendarSystem.AverageTicksPerYear)
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
                subtrahendRemainder -= Duration.OneWeek;
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
