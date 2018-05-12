// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Calendars
{
    internal sealed class IslamicYearMonthDayCalculator : RegularYearMonthDayCalculator
    {
        /// <summary>Days in a pair of months, in days.</summary>
        private const int MonthPairLength = 59;

        /// <summary>The length of a long month, in days.</summary>
        private const int LongMonthLength = 30;

        /// <summary>The length of a short month, in days.</summary>
        private const int ShortMonthLength = 29;

        /// <summary>The typical number of days in 10 years.</summary>
        private const int AverageDaysPer10Years = 3544; // Ideally 354.36667 per year

        /// <summary>The number of days in a non-leap year.</summary>
        private const int DaysPerNonLeapYear = 354;

        /// <summary>The number of days in a leap year.</summary>
        private const int DaysPerLeapYear = 355;

        /// <summary>The days for the civil (Friday) epoch of July 16th 622CE.</summary>
        private const int DaysAtCivilEpoch = -492148;

        /// <summary>The days for the civil (Thursday) epoch of July 15th 622CE.</summary>
        private const int DaysAtAstronomicalEpoch = DaysAtCivilEpoch - 1;

        /// <summary>The length of the cycle of leap years.</summary>
        private const int LeapYearCycleLength = 30;

        /// <summary>The number of days in leap cycle.</summary>
        private const int DaysPerLeapCycle = 19 * DaysPerNonLeapYear + 11 * DaysPerLeapYear;

        /// <summary>The pattern of leap years within a cycle, one bit per year, for this calendar.</summary>
        private readonly int leapYearPatternBits;

        private static readonly int[] TotalDaysByMonth;

        static IslamicYearMonthDayCalculator()
        {
            int days = 0;
            TotalDaysByMonth = new int[12];
            for (int i = 0; i < 12; i++)
            {
                TotalDaysByMonth[i] = days;
                // Here, the month number is 0-based, so even months are long
                int daysInMonth = (i & 1) == 0 ? LongMonthLength : ShortMonthLength;
                // This doesn't take account of leap years, but that doesn't matter - because
                // it's not used on the last iteration, and leap years only affect the final month
                // in the Islamic calendar.
                days += daysInMonth;
            }
        }

        internal IslamicYearMonthDayCalculator(IslamicLeapYearPattern leapYearPattern, IslamicEpoch epoch)
            : base(1, 9665, 12, AverageDaysPer10Years, GetYear1Days(epoch))
        {
            this.leapYearPatternBits = GetLeapYearPatternBits(leapYearPattern);
        }

        protected override int GetDaysFromStartOfYearToStartOfMonth(int year, int month)
        {
            // The number of days at the *start* of a month isn't affected by
            // the year as the only month length which varies by year is the last one.
            return TotalDaysByMonth[month - 1];
        }

        internal override YearMonthDay GetYearMonthDay(int year, int dayOfYear)
        {
            int month, day;
            // Special case the last day in a leap year
            if (dayOfYear == DaysPerLeapYear)
            {
                month = 12;
                day = 30;
            }
            else
            {
                int dayOfYearZeroBased = dayOfYear - 1;
                month = ((dayOfYearZeroBased * 2) / MonthPairLength) + 1;
                day = ((dayOfYearZeroBased % MonthPairLength) % LongMonthLength) + 1;
            }
            return new YearMonthDay(year, month, day);
        }

        internal override bool IsLeapYear(int year)
        {
            // Handle negative years in order to make calculations near the start of the calendar work cleanly.
            int yearOfCycle = year >= 0 ? year % LeapYearCycleLength
                                        : (year % LeapYearCycleLength) + LeapYearCycleLength;
            int key = 1 << yearOfCycle;
            return (leapYearPatternBits & key) > 0;
        }

        internal override int GetDaysInYear(int year) => IsLeapYear(year) ? DaysPerLeapYear : DaysPerNonLeapYear;

        internal override int GetDaysInMonth(int year, int month)
        {
            if (month == 12 && IsLeapYear(year))
            {
                return LongMonthLength;
            }
            // Note: month is 1-based here, so even months are the short ones
            return (month & 1) == 0 ? ShortMonthLength : LongMonthLength;
        }

        protected override int CalculateStartOfYearDays(int year)
        {
            // The first cycle starts in year 1, not year 0.
            // We try to cope with years outside the normal range, in order to allow arithmetic at the boundaries.
            int cycle = year > 0 ? (year - 1) / LeapYearCycleLength
                                 : (year - LeapYearCycleLength) / LeapYearCycleLength;
            int yearAtStartOfCycle = (cycle * LeapYearCycleLength) + 1;

            int days = DaysAtStartOfYear1 + cycle * DaysPerLeapCycle;

            // We've got the days at the start of the cycle (e.g. at the start of year 1, 31, 61 etc).
            // Now go from that year to (but not including) the year we're looking for, adding the right
            // number of days in each year. So if we're trying to find the start of year 34, we would
            // find the days at the start of year 31, then add the days *in* year 31, the days in year 32,
            // and the days in year 33.
            // If this ever proves to be a bottleneck, we could create an array for each IslamicLeapYearPattern
            // with "the number of days for the first n years in a cycle".
            for (int i = yearAtStartOfCycle; i < year; i++)
            {
                days += GetDaysInYear(i);
            }
            return days;
        }

        /// <summary>
        /// Returns the pattern of leap years within a cycle, one bit per year, for the specified pattern.
        /// Note that although cycle years are usually numbered 1-30, the bit pattern is for 0-29; cycle year
        /// 30 is represented by bit 0.
        /// </summary>
        private static int GetLeapYearPatternBits(IslamicLeapYearPattern leapYearPattern) => leapYearPattern switch
            {
                // When reading bit patterns, don't forget to read right to left...
                IslamicLeapYearPattern.Base15 => 623158436,        // 0b100101001001001010010010100100
                IslamicLeapYearPattern.Base16 => 623191204,        // 0b100101001001010010010010100100
                IslamicLeapYearPattern.Indian => 690562340,        // 0b101001001010010010010100100100
                IslamicLeapYearPattern.HabashAlHasib => 153692453, // 0b001001001010010010100100100101
                _ => throw new ArgumentOutOfRangeException(nameof(leapYearPattern))
            };

        /// <summary>
        /// Returns the days since the Unix epoch at the specified epoch.
        /// </summary>
        private static int GetYear1Days(IslamicEpoch epoch) => epoch switch
            {
                // Epoch 1970-01-01 ISO = 1389-10-22 Islamic (civil) or 1389-10-23 Islamic (astronomical)
                IslamicEpoch.Astronomical => DaysAtAstronomicalEpoch,
                IslamicEpoch.Civil => DaysAtCivilEpoch,
                _ => throw new ArgumentOutOfRangeException(nameof(epoch))
            };
    }
}
