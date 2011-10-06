#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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
using NodaTime.Fields;

namespace NodaTime.Calendars
{
    /// <summary>
     /// Implements the Coptic calendar system, which defines every fourth year as
     /// leap, much like the Julian calendar. The year is broken down into 12 months,
     /// each 30 days in length. An extra period at the end of the year is either 5
     /// or 6 days in length. In this implementation, it is considered a 13th month.
     /// </summary>
     /// <remarks>
     /// <para>
     /// Year 1 in the Coptic calendar began on August 29, 284 CE (Julian), thus
     /// Coptic years do not begin at the same time as Julian years. This chronology
     /// is not proleptic, as it does not allow dates before the first Coptic year.
     /// </para>
     /// <para>
     /// This implementation defines a day as midnight to midnight exactly as per
     /// the ISO chronology. Some references indicate that a coptic day starts at
     /// sunset on the previous ISO day, but this has not been confirmed and is not
     /// implemented.
     /// </para>
     /// </remarks>
    internal sealed class CopticCalendarSystem : BasicFixedMonthCalendarSystem
    {
        private const string CopticName = "Coptic";
        private static readonly CopticCalendarSystem[] instances;
        private static readonly DateTimeField EraField = new BasicSingleEraDateTimeField(Era.AnnoMartyrm);
        // TODO: Validate these
        internal override int MinYear { get { return -29226; } }
        internal override int MaxYear { get { return 29227; } }

        static CopticCalendarSystem()
        {
            instances = new CopticCalendarSystem[7];
            for (int i = 0; i < 7; i++)
            {
                instances[i] = new CopticCalendarSystem(i + 1);
            }
        }

        /// <summary>
        /// Returns the instance of the Coptic calendar system with the given number of days in the week.
        /// </summary>
        /// <param name="minDaysInFirstWeek">The minimum number of days at the start of the year to consider it
        /// a week in that year as opposed to at the end of the previous year.</param>
        internal static CopticCalendarSystem GetInstance(int minDaysInFirstWeek)
        {
            if (minDaysInFirstWeek < 1 || minDaysInFirstWeek > 7)
            {
                throw new ArgumentOutOfRangeException("minDaysInFirstWeek", "Minimum days in first week must be between 1 and 7 inclusive");
            }
            return instances[minDaysInFirstWeek - 1];
        }

        private CopticCalendarSystem(int minDaysInFirstWeek) : base(CopticName, minDaysInFirstWeek, AssembleCopticFields, new[] { Era.AnnoMartyrm })
        {
        }

        private static void AssembleCopticFields(FieldSet.Builder builder, CalendarSystem @this)
        {
            builder.Year = new SkipZeroDateTimeField(builder.Year);
            builder.WeekYear = new SkipZeroDateTimeField(builder.WeekYear);

            builder.Era = EraField;
            builder.MonthOfYear = new BasicMonthOfYearDateTimeField((BasicCalendarSystem) @this, 13);
            builder.Months = builder.MonthOfYear.DurationField;
        }

        protected override LocalInstant CalculateStartOfYear(int year)
        {
            // Unix epoch is 1970-01-01 Gregorian which is 1686-04-23 Coptic.
            // Calculate relative to the nearest leap year and account for the
            // difference later.

            int relativeYear = year - 1687;
            int leapYears;
            if (relativeYear <= 0)
            {
                // Add 3 before shifting right since /4 and >>2 behave differently
                // on negative numbers.
                leapYears = (relativeYear + 3) >> 2;
            }
            else
            {
                leapYears = relativeYear >> 2;
                // For post 1687 an adjustment is needed as jan1st is before leap day
                if (!IsLeapYear(year))
                {
                    leapYears++;
                }
            }

            long ticks = (relativeYear * 365L + leapYears) * NodaConstants.TicksPerStandardDay;

            // Adjust to account for difference between 1687-01-01 and 1686-04-23.
            return new LocalInstant(ticks + (365L - 112) * NodaConstants.TicksPerStandardDay);
        }

        internal override long ApproxTicksAtEpochDividedByTwo
        {
            get { return (1686L * AverageTicksPerYear + 112L * NodaConstants.TicksPerStandardDay) / 2;  }
        }
    }
}
