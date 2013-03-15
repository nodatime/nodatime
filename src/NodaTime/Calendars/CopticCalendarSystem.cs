// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using NodaTime.Fields;

namespace NodaTime.Calendars
{
    /// <summary>
    /// See <see cref="CalendarSystem.GetCopticCalendar"/> for details.
    /// </summary>
    internal sealed class CopticCalendarSystem : BasicFixedMonthCalendarSystem
    {
        private const string CopticName = "Coptic";
        private static readonly CopticCalendarSystem[] instances;
        private static readonly DateTimeField EraField = new BasicSingleEraDateTimeField(Era.AnnoMartyrm);

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

        private CopticCalendarSystem(int minDaysInFirstWeek)
            : base(CreateIdFromNameAndMinDaysInFirstWeek(CopticName, minDaysInFirstWeek), CopticName, minDaysInFirstWeek, 1, 29227, AssembleCopticFields, new[] { Era.AnnoMartyrm })
        {
        }

        private static void AssembleCopticFields(FieldSet.Builder builder, CalendarSystem @this)
        {
            builder.Era = EraField;
            builder.MonthOfYear = new BasicMonthOfYearDateTimeField((BasicCalendarSystem) @this);
            builder.Months = builder.MonthOfYear.PeriodField;
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
