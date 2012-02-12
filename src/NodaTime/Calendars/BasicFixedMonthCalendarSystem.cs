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

using System.Collections.Generic;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Abstract implementation of a calendar system based around months which always have 30 days.
    /// </summary>
    /// <remarks>
    /// As the month length is fixed various calculations can be optimised.
    /// This implementation assumes any additional days after twelve
    /// months fall into a thirteenth month.
    /// </remarks>
    internal abstract class BasicFixedMonthCalendarSystem : BasicCalendarSystem
    {
        private const int DaysInMonth = 30;

        private const long TicksPerMonth = DaysInMonth * NodaConstants.TicksPerStandardDay;

        private const long AverageTicksPerFixedMonthYear = (long)(365.25 * NodaConstants.TicksPerStandardDay);

        protected BasicFixedMonthCalendarSystem(string name, int minDaysInFirstWeek, int minYear, int maxYear, FieldAssembler fieldAssembler, IEnumerable<Era> eras)
            : base(name, minDaysInFirstWeek, minYear, maxYear, fieldAssembler, eras)
        {
        }

        internal override LocalInstant SetYear(LocalInstant localInstant, int year)
        {
            // Optimized implementation of set, due to fixed months
            int thisYear = GetYear(localInstant);
            int dayOfYear = GetDayOfYear(localInstant, thisYear);
            long tickOfDay = GetTickOfDay(localInstant);

            if (dayOfYear > 365)
            {
                // Current year is leap, and day is leap.
                if (!IsLeapYear(year))
                {
                    // Moving to a non-leap year, leap day doesn't exist.
                    dayOfYear--;
                }
            }

            return new LocalInstant(GetYearMonthDayTicks(year, 1, dayOfYear) + tickOfDay);
        }

        internal override long GetYearDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            // Optimized implementation due to fixed months
            int minuendYear = GetYear(minuendInstant);
            int subtrahendYear = GetYear(subtrahendInstant);

            // Inlined remainder method to avoid duplicate calls to get.
            long minuendRem = minuendInstant.Ticks - GetYearTicks(minuendYear);
            long subtrahendRem = subtrahendInstant.Ticks - GetYearTicks(subtrahendYear);

            int difference = minuendYear - subtrahendYear;
            if (minuendRem < subtrahendRem)
            {
                difference--;
            }
            return difference;
        }

        protected override long GetTicksFromStartOfYearToStartOfMonth(int year, int month)
        {
            return (month - 1) * TicksPerMonth;
        }

        internal override int GetDayOfMonth(LocalInstant localInstant)
        {
            // Optimized for fixed months
            return (GetDayOfYear(localInstant) - 1) % DaysInMonth + 1;
        }

        // Presumably the case for both the fixed month calendar systems we have...
        public override bool IsLeapYear(int year)
        {
            return (year & 3) == 3;
        }

        public override int GetDaysInMonth(int year, int month)
        {
            return month != 13 ? DaysInMonth : IsLeapYear(year) ? 6 : 5;
        }

        internal override int GetMaxDaysInMonth()
        {
            return DaysInMonth;
        }

        internal override int GetDaysInMonthMax(int month)
        {
            return month != 13 ? DaysInMonth : 6;
        }

        internal override int GetMonthOfYear(LocalInstant localInstant)
        {
            return (GetDayOfYear(localInstant) - 1) / DaysInMonth + 1;
        }

        protected internal override int GetMonthOfYear(LocalInstant localInstant, int year)
        {
            long monthZeroBased = (localInstant.Ticks - GetYearTicks(year)) / TicksPerMonth;
            return ((int)monthZeroBased) + 1;
        }

        internal override sealed int GetMaxMonth()
        {
            return 13;
        }

        internal override long AverageTicksPerYear { get { return AverageTicksPerFixedMonthYear; } }
        internal override long AverageTicksPerYearDividedByTwo { get { return AverageTicksPerFixedMonthYear / 2; } }
        internal override long AverageTicksPerMonth { get { return TicksPerMonth; } }        
    }
}
