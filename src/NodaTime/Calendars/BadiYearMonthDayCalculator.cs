// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;
using System;

namespace NodaTime.Calendars
{
    /// <summary>
    /// See <see cref="CalendarSystem.Badi" /> for details about the Badíʿ calendar.
    /// </summary>
    internal sealed class BadiYearMonthDayCalculator : YearMonthDayCalculator
    {
        // named constants to avoid use of raw numbers in the code
        private const int AverageDaysPer10Years = 3652; // Ideally 365.2425 per year...
        private const int DaysInAyyamiHaInLeapYear = 5;
        private const int DaysInAyyamiHaInNormalYear = 4;

        internal const int DaysInMonth = 19;
        private const int FirstYearOfStandardizedCalendar = 172;
        private const int GregorianYearOfFirstBadiYear = 1844;

        /// <remarks>
        /// There are 19 months in a year. Between the 18th and 19th month are the "days of Ha" (Ayyam-i-Ha).
        /// In order to make everything else in Noda Time work appropriately, Ayyam-i-Ha are counted as
        /// extra days at the end of month 18.
        /// </remarks>
        internal const int Month18 = 18;
        private const int Month19 = 19;
        private const int MonthsInYear = 19;

        private const int UnixEpochDayAtStartOfYear1 = -45941;
        private const int BadiMaxYear = 1000; // current lookup tables are pre-calculated for a thousand years
        private const int BadiMinYear = 1;

        /// <summary>
        /// This is the base64 representation of information for years 172 to 1000.
        /// NazRuzDate falls on March 19, 20, 21, or 22.
        /// DaysInAyymiHa can be 4,5.
        /// For each year, the value in the array is (NawRuzDate - 19) + 10 * (DaysInAyyamiHa - 4)
        /// </summary>
        static byte[] YearInfoRaw = Convert.FromBase64String(
            "AgELAgIBCwICAQsCAgEBCwIBAQsCAQELAgEBCwIBAQsCAQELAgEBCwIBAQELAQEBCwEBAQsBAQELAQEB" +
            "CwEBAQsBAQELAQEBCwEBAQEKAQEBCgEBAQsCAgILAgICCwICAgsCAgILAgICCwICAgELAgIBCwICAQsC" +
            "AgELAgIBCwICAQsCAgELAgIBCwICAQELAgEBCwIBAQsCAQELAgEBCwIBAQsCAQELAgEBCwIBAQELAQEB" +
            "CwEBAQsCAgIMAgICDAICAgwCAgIMAgICDAICAgILAgICCwICAgsCAgILAgICCwICAgsCAgILAgICCwIC" +
            "AgELAgIBCwICAQsCAgELAgIBCwICAQsCAgELAgIBCwICAQELAgEBCwIBAQsCAgIMAwICDAMCAgwDAgIM" +
            "AwICDAMCAgIMAgICDAICAgwCAgIMAgICDAICAgwCAgIMAgICDAICAgILAgICCwICAgsCAgILAgICCwIC" +
            "AgsCAgILAgICAQsCAgELAgIBCwICAQsCAgELAgIBCwICAQsCAgELAgIBCwICAQELAgEBCwIBAQsCAQEL" +
            "AgEBCwIBAQsCAQELAgEBCwIBAQELAQEBCwEBAQsBAQELAQEBCwEBAQsBAQELAQEBCwEBAQEKAQEBCgEB" +
            "AQoBAQELAgICCwICAgsCAgILAgICAQsCAgELAgIBCwICAQsCAgELAgIBCwICAQsCAgELAgIBAQsCAQEL" +
            "AgEBCwIBAQsCAQELAgEBCwIBAQsCAQELAgEBAQsBAQELAQEBCwEBAQsBAQELAgICDAICAgwCAgIMAgIC" +
            "AgsCAgILAgICCwICAgsCAgILAgICCwICAgsCAgILAgICAQsCAgELAgIBCwICAQsCAgELAgIBCwICAQsC" +
            "AgELAgIBAQsCAQELAgEBCwIBAQsCAQELAgICDAMCAgwDAgIMAwICAgwCAgIMAgICDAICAgwCAgIMAgIC" +
            "DAICAgwCAgIMAgICAgsCAgILAgICCwICAgsCAgILAgICCwICAgsCAgILAgICAQsCAgELAgIBCwICAQsC" +
            "AgELAgIBCwICAQsCAgELAgIBAQsCAQELAgEBCwIBAQsCAQELAgEBCwIBAQsCAQELAg==");

        static BadiYearMonthDayCalculator()
        {
            Preconditions.DebugCheckState(
                FirstYearOfStandardizedCalendar + YearInfoRaw.Length == BadiMaxYear + 1,
                "Invalid compressed data. Length: " + YearInfoRaw.Length);
        }

        internal BadiYearMonthDayCalculator()
            : base(BadiMinYear,
                BadiMaxYear - 1,
                AverageDaysPer10Years,
                UnixEpochDayAtStartOfYear1)
        {
        }

        internal static int GetDaysInAyyamiHa(int year)
        {
            Preconditions.CheckArgumentRange(nameof(year), year, BadiMinYear, BadiMaxYear);
            if (year < FirstYearOfStandardizedCalendar)
            {
                return CalendarSystem.Iso.YearMonthDayCalculator.IsLeapYear(year + GregorianYearOfFirstBadiYear)
                    ? DaysInAyyamiHaInLeapYear : DaysInAyyamiHaInNormalYear;
            }
            int num = YearInfoRaw[year - FirstYearOfStandardizedCalendar];
            return num > 10 ? DaysInAyyamiHaInLeapYear : DaysInAyyamiHaInNormalYear;
        }

        private static int GetNawRuzDayInMarch(int year)
        {
            Preconditions.CheckArgumentRange(nameof(year), year, BadiMinYear, BadiMaxYear);
            if (year < FirstYearOfStandardizedCalendar)
            {
                return 21;
            }
            const int dayInMarchForOffsetToNawRuz = 19;
            int num = YearInfoRaw[year - FirstYearOfStandardizedCalendar];
            return dayInMarchForOffsetToNawRuz + (num % 10);
        }        

        protected override int CalculateStartOfYearDays(int year)
        {
            Preconditions.CheckArgumentRange(nameof(year), year, BadiMinYear, BadiMaxYear);

            // The epoch is the same regardless of calendar system, so if we work out when the
            // start of the Badíʿ year is in terms of the Gregorian year, we can just use that
            // date's days-since-epoch value.
            var gregorianYear = year + GregorianYearOfFirstBadiYear - 1;
            var nawRuz = new LocalDate(gregorianYear, 3, GetNawRuzDayInMarch(year));
            return nawRuz.DaysSinceEpoch;
        }

        protected override int GetDaysFromStartOfYearToStartOfMonth(int year, int month)
        {
            var daysFromStartOfYearToStartOfMonth = DaysInMonth * (month - 1);

            if (month == Month19)
            {
                daysFromStartOfYearToStartOfMonth += GetDaysInAyyamiHa(year);
            }

            return daysFromStartOfYearToStartOfMonth;
        }

        internal override YearMonthDay AddMonths(YearMonthDay start, int months)
        {
            if (months == 0)
            {
                return start;
            }

            var movingBackwards = months < 0;

            var thisMonth = start.Month;
            var thisYear = start.Year;
            var thisDay = start.Day;

            var nextDay = thisDay;

            // TODO: It's not clear that this is correct. If we add 19 months,
            // it's probably okay to stay in Ayyam-i-Ha.
            if (IsInAyyamiHa(start))
            {
                nextDay = thisDay - DaysInMonth;

                if (movingBackwards)
                {
                    thisMonth++;
                }
            }

            var nextYear = thisYear;
            var nextMonthNum = thisMonth + months;

            if (nextMonthNum > MonthsInYear)
            {
                nextYear = thisYear + nextMonthNum / MonthsInYear;
                nextMonthNum = nextMonthNum % MonthsInYear;
            }
            else if (nextMonthNum < 1)
            {
                nextMonthNum = MonthsInYear - nextMonthNum;
                nextYear = thisYear - nextMonthNum / MonthsInYear;
                nextMonthNum = MonthsInYear - nextMonthNum % MonthsInYear;
            }

            if (nextYear < MinYear || nextYear > MaxYear)
            {
                throw new OverflowException("Date computation would overflow calendar bounds.");
            }

            var result = new YearMonthDay(nextYear, nextMonthNum, nextDay);

            return result;
        }

        internal override int GetDaysInMonth(int year, int month)
        {
            Preconditions.CheckArgumentRange(nameof(year), year, BadiMinYear, BadiMaxYear);
            return month == Month18 ? DaysInMonth + GetDaysInAyyamiHa(year) : DaysInMonth;
        }

        internal override int GetDaysInYear(int year) => 361 + GetDaysInAyyamiHa(year);

        internal override int GetDaysSinceEpoch(YearMonthDay target)
        {
            var month = target.Month;
            var year = target.Year;

            var firstDay0OfYear = CalculateStartOfYearDays(year) - 1;

            var daysSinceEpoch = firstDay0OfYear
                 + (month - 1) * DaysInMonth
                 + target.Day;

            if (month == Month19)
            {
                daysSinceEpoch += GetDaysInAyyamiHa(year);
            }

            return daysSinceEpoch;
        }

        internal override int GetMonthsInYear(int year) => MonthsInYear;

        internal override YearMonthDay GetYearMonthDay(int year, int dayOfYear)
        {
            Preconditions.CheckArgumentRange(nameof(dayOfYear), dayOfYear, 1, GetDaysInYear(year));

            var firstOfLoftiness = 1 + DaysInMonth * Month18 + GetDaysInAyyamiHa(year);

            if (dayOfYear >= firstOfLoftiness)
            {
                return new YearMonthDay(year, Month19, dayOfYear - firstOfLoftiness + 1);
            }

            var month = Math.Min(1 + (dayOfYear - 1) / DaysInMonth, Month18);
            var day = dayOfYear - (month - 1) * DaysInMonth;

            return new YearMonthDay(year, month, day);
        }

        internal bool IsInAyyamiHa(YearMonthDay ymd) => ymd.Month == Month18 && ymd.Day > DaysInMonth;

        internal override bool IsLeapYear(int year) => GetDaysInAyyamiHa(year) != DaysInAyyamiHaInNormalYear;

        internal override int MonthsBetween(YearMonthDay start, YearMonthDay end)
        {
            int startMonth = start.Month;
            int startYear = start.Year;

            int endMonth = end.Month;
            int endYear = end.Year;

            int diff = (endYear - startYear) * MonthsInYear + endMonth - startMonth;

            // If we just add the difference in months to start, what do we get?
            YearMonthDay simpleAddition = AddMonths(start, diff);

            // Note: this relies on naive comparison of year/month/date values.
            if (start <= end)
            {
                // Moving forward: if the result of the simple addition is before or equal to the end,
                // we're done. Otherwise, rewind a month because we've overshot.
                return simpleAddition <= end ? diff : diff - 1;
            }
            else
            {
                // Moving backward: if the result of the simple addition (of a non-positive number)
                // is after or equal to the end, we're done. Otherwise, increment by a month because
                // we've overshot backwards.
                return simpleAddition >= end ? diff : diff + 1;
            }
        }

        internal override YearMonthDay SetYear(YearMonthDay start, int newYear)
        {
            Preconditions.CheckArgumentRange(nameof(newYear), newYear, BadiMinYear, BadiMaxYear);

            var month = start.Month;
            var day = start.Day;

            if (IsInAyyamiHa(start))
            {
                // Moving a year while within Ayyam-i-Ha is not well defined.
                // In this implementation, if starting on day 5, end on day 4 (stay in Ayyam-i-Ha)
                var daysInThisAyyamiHa = GetDaysInAyyamiHa(newYear);
                return new YearMonthDay(newYear, month, Math.Min(day, DaysInMonth + daysInThisAyyamiHa));
            }

            return new YearMonthDay(newYear, month, day);
        }

        internal override void ValidateYearMonthDay(int year, int month, int day)
        {
            Preconditions.CheckArgumentRange(nameof(year), year, BadiMinYear, BadiMaxYear);
            Preconditions.CheckArgumentRange(nameof(month), month, 1, MonthsInYear);

            int daysInMonth = month == Month18 ? DaysInMonth + GetDaysInAyyamiHa(year) : DaysInMonth;
            Preconditions.CheckArgumentRange(nameof(day), day, 1, daysInMonth);
        } 
    }
}