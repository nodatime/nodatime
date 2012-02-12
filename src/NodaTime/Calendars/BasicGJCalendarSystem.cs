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
namespace NodaTime.Calendars
{
    internal abstract class BasicGJCalendarSystem : BasicCalendarSystem
    {
        // These arrays are NOT public. We trust ourselves not to alter the array.
        // They use zero-based array indexes so the that valid range of months is
        // automatically checked.
        private static readonly int[] MinDaysPerMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private static readonly int[] MaxDaysPerMonth = { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        private static readonly long[] MinTotalTicksByMonth;
        private static readonly long[] MaxTotalTicksByMonth;
        private const long Feb29thTicks = (31L + 29 - 1) * NodaConstants.TicksPerStandardDay;

        static BasicGJCalendarSystem()
        {
            MinTotalTicksByMonth = new long[12];
            MaxTotalTicksByMonth = new long[12];
            long minSum = 0;
            long maxSum = 0;
            for (int i = 0; i < 11; i++)
            {
                minSum += MinDaysPerMonth[i] * NodaConstants.TicksPerStandardDay;
                maxSum += MaxDaysPerMonth[i] * NodaConstants.TicksPerStandardDay;
                MinTotalTicksByMonth[i + 1] = minSum;
                MaxTotalTicksByMonth[i + 1] = maxSum;
            }
        }

        protected BasicGJCalendarSystem(string name, int minDaysInFirstWeek, int minYear, int maxYear)
            : this(name, minDaysInFirstWeek, minYear, maxYear, null)
        {
        }

        protected BasicGJCalendarSystem(string name, int minDaysInFirstWeek, int minYear, int maxYear, FieldAssembler fieldAssembler)
            : base(name, minDaysInFirstWeek, minYear, maxYear, fieldAssembler, new[] { Era.BeforeCommon, Era.Common })
        {
        }

        protected internal override int GetMonthOfYear(LocalInstant localInstant, int year)
        {
            // Perform a binary search to get the month. To make it go even faster,
            // compare using ints instead of longs. The number of ticks per
            // year exceeds the limit of a 32-bit int's capacity, so divide by
            // 1024 * 10000. No precision is lost (except time of day) since the number of
            // ticks per day contains 1024 * 10000 as a factor. After the division,
            // the instant isn't measured in ticks, but in units of
            // (128/125)seconds.
            int i = (int)((((localInstant.Ticks - GetYearTicks(year))) >> 10) / NodaConstants.TicksPerMillisecond);

            return (IsLeapYear(year))
                       ? ((i < 182 * 84375)
                              ? ((i < 91 * 84375) ? ((i < 31 * 84375) ? 1 : (i < 60 * 84375) ? 2 : 3) : ((i < 121 * 84375) ? 4 : (i < 152 * 84375) ? 5 : 6))
                              : ((i < 274 * 84375)
                                     ? ((i < 213 * 84375) ? 7 : (i < 244 * 84375) ? 8 : 9)
                                     : ((i < 305 * 84375) ? 10 : (i < 335 * 84375) ? 11 : 12)))
                       : ((i < 181 * 84375)
                              ? ((i < 90 * 84375) ? ((i < 31 * 84375) ? 1 : (i < 59 * 84375) ? 2 : 3) : ((i < 120 * 84375) ? 4 : (i < 151 * 84375) ? 5 : 6))
                              : ((i < 273 * 84375)
                                     ? ((i < 212 * 84375) ? 7 : (i < 243 * 84375) ? 8 : 9)
                                     : ((i < 304 * 84375) ? 10 : (i < 334 * 84375) ? 11 : 12)));
        }

        public override int GetDaysInMonth(int year, int month)
        {
            return IsLeapYear(year) ? MaxDaysPerMonth[month - 1] : MinDaysPerMonth[month - 1];
        }

        internal override int GetDaysInMonthMax(int month)
        {
            return MaxDaysPerMonth[month - 1];
        }

        protected override long GetTicksFromStartOfYearToStartOfMonth(int year, int month)
        {
            return IsLeapYear(year) ? MaxTotalTicksByMonth[month - 1] : MinTotalTicksByMonth[month - 1];
        }

        internal override long GetYearDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            int minuendYear = GetYear(minuendInstant);
            int subtrahendYear = GetYear(subtrahendInstant);

            // Inlined remainder method to avoid duplicate calls to get.
            long minuendRem = minuendInstant.Ticks - GetYearTicks(minuendYear);
            long subtrahendRem = subtrahendInstant.Ticks - GetYearTicks(subtrahendYear);

            // Balance leap year differences on remainders.
            if (subtrahendRem >= Feb29thTicks)
            {
                if (IsLeapYear(subtrahendYear))
                {
                    if (!IsLeapYear(minuendYear))
                    {
                        subtrahendRem -= NodaConstants.TicksPerStandardDay;
                    }
                }
                else if (minuendRem >= Feb29thTicks && IsLeapYear(minuendYear))
                {
                    minuendRem -= NodaConstants.TicksPerStandardDay;
                }
            }

            int difference = minuendYear - subtrahendYear;
            if (minuendRem < subtrahendRem)
            {
                difference--;
            }
            return difference;
        }

        internal override LocalInstant SetYear(LocalInstant localInstant, int year)
        {
            int thisYear = GetYear(localInstant);
            int dayOfYear = GetDayOfYear(localInstant, thisYear);
            long tickOfDay = GetTickOfDay(localInstant);

            if (dayOfYear > (31 + 28))
            {
                // after Feb 28
                if (IsLeapYear(thisYear))
                {
                    // Current date is Feb 29 or later.
                    if (!IsLeapYear(year))
                    {
                        // Moving to a non-leap year, Feb 29 does not exist.
                        dayOfYear--;
                    }
                }
                else
                {
                    // Current date is Mar 01 or later.
                    if (IsLeapYear(year))
                    {
                        // Moving to a leap year, account for Feb 29.
                        dayOfYear++;
                    }
                }
            }

            long ticks = GetYearMonthDayTicks(year, 1, dayOfYear);
            return new LocalInstant(ticks + tickOfDay);
        }

        #region Era handling
        internal override int GetAbsoluteYear(int yearOfEra, int eraIndex)
        {
            // By now the era will have been validated; it's either 0 (BC) or 1 (AD)
            return eraIndex == 0 ? 1 - yearOfEra: yearOfEra;
        }

        internal override int GetMaxYearOfEra(int eraIndex)
        {
            // By now the era will have been validated; it's either 0 (BC) or 1 (AD)
            return eraIndex == 0 ? 1 - MinYear : MaxYear;
        }

        // No need to override GetMinYearOfEra; 1 is fine.
        #endregion
    }
}