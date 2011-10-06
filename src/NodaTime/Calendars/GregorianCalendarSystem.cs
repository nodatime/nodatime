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

namespace NodaTime.Calendars
{
    /// <summary>
    /// Implements a pure proleptic Gregorian calendar system, which defines every
    /// fourth year as leap, unless the year is divisible by 100 and not by 400.
    /// This improves upon the Julian calendar leap year rule.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Although the Gregorian calendar did not exist before 1582 CE, this
    /// chronology assumes it did, thus it is proleptic. This implementation also
    /// fixes the start of the year at January 1, and defines the year zero.
    /// </para>
    /// <para>
    /// This is exposed via <see cref="CalendarSystem.GetGregorianCalendar"/>.
    /// </para>
    /// </remarks>
    internal sealed class GregorianCalendarSystem : BasicGJCalendarSystem
    {
        private const string GregorianName = "Gregorian";

        private const int DaysFrom0000To1970 = 719527;
        private const long AverageTicksPerGregorianYear = (long)(365.2425m * NodaConstants.TicksPerStandardDay);

        private static readonly GregorianCalendarSystem[] instances;

        static GregorianCalendarSystem()
        {
            instances = new GregorianCalendarSystem[7];
            for (int i = 0; i < 7; i++)
            {
                instances[i] = new GregorianCalendarSystem(i + 1);
            }
        }

        /// <summary>
        /// Returns the instance of the Gregorian calendar system with the given number of days in the week.
        /// </summary>
        /// <param name="minDaysInFirstWeek">The minimum number of days at the start of the year to consider it
        /// a week in that year as opposed to at the end of the previous year.</param>
        internal static GregorianCalendarSystem GetInstance(int minDaysInFirstWeek)
        {
            if (minDaysInFirstWeek < 1 || minDaysInFirstWeek > 7)
            {
                throw new ArgumentOutOfRangeException("minDaysInFirstWeek", "Minimum days in first week must be between 1 and 7 inclusive");
            }
            return instances[minDaysInFirstWeek - 1];
        }

        private GregorianCalendarSystem(int minDaysInFirstWeek) : base(GregorianName, minDaysInFirstWeek)
        {
        }

        internal override long AverageTicksPerYear { get { return AverageTicksPerGregorianYear; } }
        internal override long AverageTicksPerYearDividedByTwo { get { return AverageTicksPerGregorianYear / 2; } }
        internal override long AverageTicksPerMonth { get { return (long)(365.2425m * NodaConstants.TicksPerStandardDay / 12); } }
        internal override long ApproxTicksAtEpochDividedByTwo { get { return (1970 * AverageTicksPerGregorianYear) / 2; } }

        public override int MinYear { get { return -27256; } }
        public override int MaxYear { get { return 31196; } }

        protected override LocalInstant CalculateStartOfYear(int year)
        {
            if (year < MinYear)
            {
                return LocalInstant.MinValue;
            }
            if (year > MaxYear)
            {
                return LocalInstant.MaxValue;
            }

            // Initial value is just temporary.
            int leapYears = year / 100;
            if (year < 0)
            {
                // Add 3 before shifting right since /4 and >>2 behave differently
                // on negative numbers. When the expression is written as
                // (year / 4) - (year / 100) + (year / 400),
                // it works for both positive and negative values, except this optimization
                // eliminates two divisions.
                leapYears = ((year + 3) >> 2) - leapYears + ((leapYears + 3) >> 2) - 1;
            }
            else
            {
                leapYears = (year >> 2) - leapYears + (leapYears >> 2);
                if (IsLeapYear(year))
                {
                    leapYears--;
                }
            }

            return new LocalInstant((year * 365L + (leapYears - DaysFrom0000To1970)) * NodaConstants.TicksPerStandardDay);
        }

        public override bool IsLeapYear(int year)
        {
            return ((year & 3) == 0) && ((year % 100) != 0 || (year % 400) == 0);
        }
    }
}
