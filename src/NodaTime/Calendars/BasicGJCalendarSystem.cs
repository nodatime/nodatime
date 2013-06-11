// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Calendars
{
    /// <summary>
    /// Base class for both the Gregorian and Julian calendar systems, which differ only by
    /// when leap years occur. (This also affects the date of the Unix epoch, affecting some other calculations.)
    /// </summary>
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

        protected BasicGJCalendarSystem(string id, string name, int minDaysInFirstWeek, int minYear, int maxYear, FieldAssembler fieldAssembler)
            : base(id, name, minDaysInFirstWeek, minYear, maxYear, fieldAssembler, new[] { Era.BeforeCommon, Era.Common })
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
            // (128/125) seconds.
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