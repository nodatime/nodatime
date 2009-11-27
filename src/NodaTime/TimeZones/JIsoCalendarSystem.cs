#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
namespace NodaTime.TimeZones
{
    /// <summary>
    /// TODO: This is a hack to get the code working. When the real ISoCalendarSystem is ready
    ///       remove all alias lines in all files in the package and remove the JIsoCalendarSystem.cs file.
    /// </summary>
    internal class JIsoCalendarSystem
    {
        public static readonly JIsoCalendarSystem Utc = new JIsoCalendarSystem();

        public YearField Year { get; private set; }
        public MonthOfYearField MonthOfYear { get; private set; }
        public DayOfMonthField DayOfMonth { get; private set; }
        public DayOfWeekField DayOfWeek { get; private set; }
        public TicksOfDayField TicksOfDay { get; private set; }

        public JIsoCalendarSystem()
        {
            Year = new YearField();
            MonthOfYear = new MonthOfYearField();
            DayOfMonth = new DayOfMonthField();
            DayOfWeek = new DayOfWeekField();
            TicksOfDay = new TicksOfDayField();
        }

        internal static bool isLeapYear(int year)
        {
            return ((year & 3) == 0) && ((year % 100) != 0 || (year % 400) == 0);
        }
        internal static LocalInstant getYearTicks(int year)
        {
            const int DAYS_0000_TO_1970 = 719527;

            // Initial value is just temporary.
            int leapYears = year / 100;
            if (year < 0) {
                // Add 3 before shifting right since /4 and >>2 behave differently
                // on negative numbers. When the expression is written as
                // (year / 4) - (year / 100) + (year / 400),
                // it works for both positive and negative values, except this optimization
                // eliminates two divisions.
                leapYears = ((year + 3) >> 2) - leapYears + ((leapYears + 3) >> 2) - 1;
            }
            else {
                leapYears = (year >> 2) - leapYears + (leapYears >> 2);
                if (isLeapYear(year)) {
                    leapYears--;
                }
            }

            return new LocalInstant((year * 365L + (leapYears - DAYS_0000_TO_1970)) * NodaConstants.TicksPerDay);
        }

        private const long TicksPerYear = (long)(365.2425 * NodaConstants.TicksPerDay);
        internal static long getAverageTicksPerYearDividedByTwo()
        {
            return TicksPerYear / 2;
        }

        internal static long getApproxTicksAtEpochDividedByTwo()
        {
            return (1970L * TicksPerYear) / 2;
        }
        internal static int getYear(LocalInstant instant)
        {
            // Get an initial estimate of the year, and the millis value that
            // represents the start of that year. Then verify estimate and fix if
            // necessary.

            // Initial estimate uses values divided by two to avoid overflow.
            long unitTicks = getAverageTicksPerYearDividedByTwo();
            long i2 = (instant.Ticks >> 1) + getApproxTicksAtEpochDividedByTwo();
            if (i2 < 0) {
                i2 = i2 - unitTicks + 1;
            }
            int year = (int)(i2 / unitTicks);

            LocalInstant yearStart = getYearTicks(year);
            Duration diff = instant - yearStart;

            if (diff < Duration.Zero) {
                year--;
            }
            else if (diff >= new Duration(NodaConstants.TicksPerDay * 365L)) {
                // One year may need to be added to fix estimate.
                Duration oneYear;
                if (isLeapYear(year)) {
                    oneYear = new Duration(NodaConstants.TicksPerDay * 366L);
                }
                else {
                    oneYear = new Duration(NodaConstants.TicksPerDay * 365L);
                }

                yearStart += oneYear;

                if (yearStart <= instant) {
                    // Didn't go too far, so actually add one year.
                    year++;
                }
            }

            return year;
        }
        internal static Duration getTicksOfDay(LocalInstant instant)
        {
            if (instant >= LocalInstant.LocalUnixEpoch) {
                return new Duration(instant.Ticks % NodaConstants.TicksPerDay);
            }
            else {
                return new Duration((NodaConstants.TicksPerDay - 1) + (instant.Ticks + 1) % NodaConstants.TicksPerDay);
            }
        }
        private static int[] MIN_DAYS_PER_MONTH_ARRAY = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private static int[] MAX_DAYS_PER_MONTH_ARRAY = { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private static Duration[] MIN_TOTAL_TICKS_BY_MONTH_ARRAY;
        private static Duration[] MAX_TOTAL_TICKS_BY_MONTH_ARRAY;

        static JIsoCalendarSystem()
        {
            MIN_TOTAL_TICKS_BY_MONTH_ARRAY = new Duration[12];
            MAX_TOTAL_TICKS_BY_MONTH_ARRAY = new Duration[12];

            long minSum = 0;
            long maxSum = 0;
            for (int i = 0; i < 11; i++) {
                long ticks = MIN_DAYS_PER_MONTH_ARRAY[i] * NodaConstants.TicksPerDay;
                minSum += ticks;
                MIN_TOTAL_TICKS_BY_MONTH_ARRAY[i + 1] = new Duration(minSum);

                ticks = MAX_DAYS_PER_MONTH_ARRAY[i] * NodaConstants.TicksPerDay;
                maxSum += ticks;
                MAX_TOTAL_TICKS_BY_MONTH_ARRAY[i + 1] = new Duration(maxSum);
            }
        }
        internal static LocalInstant getYearMonthDayTicks(int year, int month, int dayOfMonth)
        {
            LocalInstant millis = getYearTicks(year);
            millis = millis + getTotalTicksByYearMonth(year, month);
            return new LocalInstant(millis.Ticks + (dayOfMonth - 1) * NodaConstants.TicksPerDay);
        }
        internal static Duration getTotalTicksByYearMonth(int year, int month)
        {
            if (isLeapYear(year)) {
                return MAX_TOTAL_TICKS_BY_MONTH_ARRAY[month - 1];
            }
            else {
                return MIN_TOTAL_TICKS_BY_MONTH_ARRAY[month - 1];
            }
        }
        internal static int getDayOfMonth(LocalInstant instant, int year)
        {
            int month = getMonthOfYear(instant, year);
            return getDayOfMonth(instant, year, month);
        }
        internal static int getDayOfMonth(LocalInstant instant, int year, int month)
        {
            LocalInstant dateTicks = getYearTicks(year);
            dateTicks = dateTicks + getTotalTicksByYearMonth(year, month);
            return (int)((instant - dateTicks).Ticks / NodaConstants.TicksPerDay) + 1;
        }
        internal static int getMonthOfYear(LocalInstant instant, int year)
        {
            // Perform a binary search to get the month. To make it go even faster,
            // compare using ints instead of longs. The number of milliseconds per
            // year exceeds the limit of a 32-bit int's capacity, so divide by
            // 1024. No precision is lost (except time of day) since the number of
            // milliseconds per day contains 1024 as a factor. After the division,
            // the instant isn't measured in milliseconds, but in units of
            // (128/125)seconds.

            int i = (int)(((instant - getYearTicks(year)).Ticks >> 10) / NodaConstants.TicksPerMillisecond);

            // There are 8,640,000,000,000 ticks per day, but divided by 1024 * NodaConstants.TicksPerMillisecond
            // is 84375. There are 84375 (128/125)seconds per day.

            return
                (isLeapYear(year))
                ? ((i < 182 * 84375)
                   ? ((i < 91 * 84375)
                      ? ((i < 31 * 84375) ? 1 : (i < 60 * 84375) ? 2 : 3)
                      : ((i < 121 * 84375) ? 4 : (i < 152 * 84375) ? 5 : 6))
                   : ((i < 274 * 84375)
                      ? ((i < 213 * 84375) ? 7 : (i < 244 * 84375) ? 8 : 9)
                      : ((i < 305 * 84375) ? 10 : (i < 335 * 84375) ? 11 : 12)))
                : ((i < 181 * 84375)
                   ? ((i < 90 * 84375)
                      ? ((i < 31 * 84375) ? 1 : (i < 59 * 84375) ? 2 : 3)
                      : ((i < 120 * 84375) ? 4 : (i < 151 * 84375) ? 5 : 6))
                   : ((i < 273 * 84375)
                      ? ((i < 212 * 84375) ? 7 : (i < 243 * 84375) ? 8 : 9)
                      : ((i < 304 * 84375) ? 10 : (i < 334 * 84375) ? 11 : 12)));
        }

        public class YearField
        {

            public int GetValue(LocalInstant instant)
            {
                return getYear(instant);
            }
            public LocalInstant SetValue(LocalInstant instant, int value)
            {
                int thisYear = GetValue(instant);
                int dayYearOffset = getDayYearOffset(instant, thisYear);
                Duration millisOfDay = getTicksOfDay(instant);

                if (dayYearOffset > (31 + 28)) { // after Feb 28
                    if (isLeapYear(thisYear)) {
                        // Current date is Feb 29 or later.
                        if (!isLeapYear(value)) {
                            // Moving to a non-leap year, Feb 29 does not exist.
                            dayYearOffset--;
                        }
                    }
                    else {
                        // Current date is Mar 01 or later.
                        if (isLeapYear(value)) {
                            // Moving to a leap year, account for Feb 29.
                            dayYearOffset++;
                        }
                    }
                }

                instant = getYearMonthDayTicks(value, 1, dayYearOffset) + millisOfDay;

                return instant;
            }
            private int getDayYearOffset(LocalInstant instant, int year)
            {
                LocalInstant yearStart = getYearTicks(year);
                return (int)((instant - yearStart).Ticks / NodaConstants.TicksPerDay) + 1;
            }
            public LocalInstant Add(LocalInstant instant, int value)
            {
                int oldValue = GetValue(instant);
                oldValue += value;
                return SetValue(instant, oldValue);
            }
            public bool IsLeap(LocalInstant instant)
            {
                int year = GetValue(instant);
                return isLeapYear(year);
            }
        }
        public class MonthOfYearField
        {
            public int GetValue(LocalInstant instant)
            {
                return getMonthOfYear(instant, getYear(instant));
            }
            public LocalInstant SetValue(LocalInstant instant, int value)
            {
                int thisYear = getYear(instant);
                //
                int thisDom = getDayOfMonth(instant, thisYear);
                // Return newly calculated millis value
                return getYearMonthDayTicks(thisYear, value, thisDom) + getTicksOfDay(instant);
            }
            public LocalInstant Add(LocalInstant instant, int value)
            {
                int oldValue = GetValue(instant);
                oldValue += value;
                return SetValue(instant, oldValue);
            }
        }
        public class DayOfMonthField
        {
            public int GetValue(LocalInstant instant)
            {
                int year = getYear(instant);
                int month = getMonthOfYear(instant, year);
                LocalInstant dateMillis = getYearTicks(year);
                dateMillis = dateMillis + getTotalTicksByYearMonth(year, month);
                return (int)((instant - dateMillis).Ticks / NodaConstants.TicksPerDay) + 1;
            }
            public LocalInstant SetValue(LocalInstant instant, int value)
            {
                return instant + new Duration((value - GetValue(instant)) * NodaConstants.TicksPerDay);
            }
            public LocalInstant Add(LocalInstant instant, int value)
            {
                int oldValue = GetValue(instant);
                oldValue += value;
                return SetValue(instant, oldValue);
            }
        }
        public class DayOfWeekField
        {
            public int GetValue(LocalInstant instant)
            {
                // 1970-01-01 is day of week 4, Thursday.

                long daysSince19700101;
                if (instant >= LocalInstant.LocalUnixEpoch) {
                    daysSince19700101 = instant.Ticks / NodaConstants.TicksPerDay;
                }
                else {
                    daysSince19700101 = (instant.Ticks - (NodaConstants.TicksPerDay - 1)) / NodaConstants.TicksPerDay;
                    if (daysSince19700101 < -3) {
                        return 7 + (int)((daysSince19700101 + 4) % 7);
                    }
                }

                return 1 + (int)((daysSince19700101 + 3) % 7);
            }
            public LocalInstant SetValue(LocalInstant instant, int value)
            {
                return instant + new Duration((value - GetValue(instant)) * NodaConstants.TicksPerDay);
            }
            public LocalInstant Add(LocalInstant instant, int value)
            {
                int oldValue = GetValue(instant);
                oldValue += value;
                return SetValue(instant, oldValue);
            }
        }
        public class TicksOfDayField
        {
            public Duration GetValue(LocalInstant instant)
            {
                return getTicksOfDay(instant);
            }
            public LocalInstant SetValue(LocalInstant instant, Duration value)
            {
                return instant + (value - GetValue(instant));
            }
        }
    }

}
