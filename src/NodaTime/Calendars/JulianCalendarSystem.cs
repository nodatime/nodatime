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
    /// Implements a pure proleptic Julian calendar system, which defines every
    /// fourth year as leap. This implementation follows the leap year rule
    /// strictly, even for dates before 8 CE, where leap years were actually
    /// irregular. Unlike Joda Time's implementation, the implementation in Noda Time
    /// *does* recognize an "absolute" year zero, although it becomes 1 BCE when viewed
    /// as a "year of era". That year (and 5 BCE, 9 BCE etc) are leap years.
    /// </summary>
    /// <remarks>
    /// Although the Julian calendar did not exist before 45 BCE, this calendar
    /// assumes it did, thus it is proleptic. This implementation also fixes the
    /// start of the year at January 1.
    /// </remarks>
    internal sealed class JulianCalendarSystem : BasicGJCalendarSystem
    {
        private const string JulianName = "Julian";

        private static readonly JulianCalendarSystem[] instances;

        static JulianCalendarSystem()
        {
            instances = new JulianCalendarSystem[7];
            for (int i = 0; i < 7; i++)
            {
                instances[i] = new JulianCalendarSystem(i + 1);
            }
        }

        /// <summary>
        /// Returns the instance of the Julian calendar system with the given number of days in the week.
        /// </summary>
        /// <param name="minDaysInFirstWeek">The minimum number of days at the start of the year to consider it
        /// a week in that year as opposed to at the end of the previous year.</param>
        internal static JulianCalendarSystem GetInstance(int minDaysInFirstWeek)
        {
            if (minDaysInFirstWeek < 1 || minDaysInFirstWeek > 7)
            {
                throw new ArgumentOutOfRangeException("minDaysInFirstWeek", "Minimum days in first week must be between 1 and 7 inclusive");
            }
            return instances[minDaysInFirstWeek - 1];
        }

        private JulianCalendarSystem(int minDaysInFirstWeek)
            : base(JulianName, minDaysInFirstWeek, -27256, 31196, null)
        {
        }

        private const long AverageTicksPerJulianYear = (long)(365.25m * NodaConstants.TicksPerStandardDay);
        private const long AverageTicksPerJulianMonth = (long)((365.25m * NodaConstants.TicksPerStandardDay) / 12);

        internal override long AverageTicksPerMonth { get { return AverageTicksPerJulianMonth; } }
        internal override long AverageTicksPerYear { get { return AverageTicksPerJulianYear; } }
        internal override long ApproxTicksAtEpochDividedByTwo { get { return (1969L * AverageTicksPerJulianYear + 352L * NodaConstants.TicksPerStandardDay) / 2; } }
        internal override long AverageTicksPerYearDividedByTwo { get { return AverageTicksPerJulianYear / 2; } }

        public override bool IsLeapYear(int year)
        {
            return (year & 3) == 0;
        }

        protected override LocalInstant CalculateStartOfYear(int year)
        {
            // Unix epoch is 1970-01-01 Gregorian which is 1969-12-19 Julian.
            // Calculate relative to the nearest leap year and account for the
            // difference later.

            int relativeYear = year - 1968;
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
                // For post 1968 an adjustment is needed as jan1st is before leap day
                if (!IsLeapYear(year))
                {
                    leapYears++;
                }
            }

            // Accounts for the difference between January 1st 1968 and December 19th 1969.
            long ticks = (relativeYear * 365L + leapYears - (366 + 352)) * NodaConstants.TicksPerStandardDay;
            return new LocalInstant(ticks);
        }
    }
}
