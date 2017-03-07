// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NodaTime.Calendars
{
    /// <summary>
    ///     See <see cref="CalendarSystem.Wondrous" /> for details.
    /// </summary>
    internal sealed class WondrousYearMonthDayCalculator : YearMonthDayCalculator
    {
        // named constants to avoid use of raw numbers in the code
        private const int AverageDaysPer10Years = 3652; // Ideally 365.2425 per year...
        private const int DaysInAyyamiHaInLeapYear = 5;
        private const int DaysInAyyamiHaInNormalYear = 4;

        private const int DaysInMonth = 19;
        private const int FirstYearOfStandardizedCalendar = 172;
        private const int GregorianYearOfFirstWondrousYear = 1844;

        private const int Month18 = 18;
        private const int Month19 = 19;
        private const int MonthsInYear = 19;

        private const int UnixEpochDayAtStartOfYear1 = -45941;
        private const int WondrousMaxYear = 1000; // current lookup tables are pre-calculated for a thousand years
        private const int WondrousMinYear = 1;

        private static Dictionary<int, WondrousYearInfo> _calculatedYears;

        /// <remarks>
        ///     There are 19 months in a year. Between the 18th and 19th month are the "days of Ha" (Ayyam-i-Ha).
        ///     Options for numbering the months:
        ///     - treat Ayyam-i-Ha as month 0. This will cause problems if 0 is treated as unknown. When stored internally,
        ///       NodaTime months are converted to 0-based numbers, so this would by -1 which is not supported.
        ///     - treat Ayyam-i-Ha as month -1. This would be a problem if the base classes convert to 0 based months
        ///     - treat Ayyam-i-Ha as month 19. This is confusing because the 19th month would be displayed as month #20.
        ///     - treat Ayyam-i-Ha as month 20. This will cause problems if months are sorted naively.
        ///     - treat Ayyam-i-Ha as extra days in month 18. This would require special handling to sometimes display
        ///       days in "month 18" as being Ayyam-i-Ha, but we have no direct control over display.
        ///       
        /// This version will treat Ayyam-i-Ha days as extra days in month 18.
        /// 
        /// Users interacting with the system should use 0 as Ayyam-i-Ha.
        /// </remarks>
        public const int AyyamiHaMonth0 = 0;

        static byte[] YearInfoRaw = Convert.FromBase64String("AgELAgIBCwICAQsCAgEBCwIBAQsCAQELAgEBCwIBAQsCAQELAgEBCwIBAQELAQEBCwEBAQsBAQELAQEB" +
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

        static WondrousYearMonthDayCalculator()
        {
            Preconditions.CheckState(FirstYearOfStandardizedCalendar + YearInfoRaw.Length == WondrousMaxYear + 1, "Invalid compressed data. Length: " + YearInfoRaw.Length);
        }


        internal WondrousYearMonthDayCalculator()
            : base(WondrousMinYear,
                WondrousMaxYear - 1,
                AverageDaysPer10Years,
                UnixEpochDayAtStartOfYear1)
        { }

        private int DayOfYearOfStartOfMonth19(int year)
        {
            var dayOfYearOfStartOfMonth19 = 1 + DaysInMonth * Month18 + GetYearInfo(year).DaysInAyyamiHa;
            return dayOfYearOfStartOfMonth19;
        }

        static WondrousYearInfo GetYearInfo(int year)
        {
            // Considered preloading this knowledge from the byte array into an array of 1000 years, but in typical use, only one or two of the years may be used. 
            // Calculated results are cached in a dictionary in case they are re-used.

            Preconditions.CheckArgumentRange(nameof(year), year, WondrousMinYear, WondrousMaxYear);

            if (_calculatedYears == null)
            {
                _calculatedYears = new Dictionary<int, WondrousYearInfo>();
            }
            else if (_calculatedYears.ContainsKey(year))
            {
                return _calculatedYears[year];
            }

            WondrousYearInfo wondrousYearInfo;

            if (year < FirstYearOfStandardizedCalendar)
            {
                // Feb 29 is near the end of the Wondrous year in the next Gregorian year
                wondrousYearInfo = new WondrousYearInfo(CalendarSystem.Iso.YearMonthDayCalculator.IsLeapYear(year + GregorianYearOfFirstWondrousYear)
                                    ? DaysInAyyamiHaInLeapYear : DaysInAyyamiHaInNormalYear,
                                    21);
                _calculatedYears.Add(year, wondrousYearInfo);
                return wondrousYearInfo;
            }

            /*
            Base64 string only has information for years 172 to 1000.
                NazRuzDate falls on March 19,20,21, or 22
                DaysInAyymiHa can be 4,5

                For each year, num = (NawRuzDate - 19) + 10 * (DaysInAyyamiHa - 4)
                ... DaysInAyyamiHa are 4 or 5, so we add 10 only when there are 5 days

                string = Convert.ToBase64String(new byte[] { num, num, ...})
            */

            int info = YearInfoRaw[year - FirstYearOfStandardizedCalendar];
            int daysInAyyamiHa;
            if (info > 10)
            {
                daysInAyyamiHa = DaysInAyyamiHaInLeapYear;
                info -= 10;
            }
            else
            {
                daysInAyyamiHa = DaysInAyyamiHaInNormalYear;
            }

            const int dayInMarchForOffsetToNawRuz = 19;
            wondrousYearInfo = new WondrousYearInfo(daysInAyyamiHa, dayInMarchForOffsetToNawRuz + info);

            _calculatedYears.Add(year, wondrousYearInfo);

            return wondrousYearInfo;
        }

        private DateTime GregorianDateOfNawRuz(int year)
        {
            Preconditions.CheckArgumentRange(nameof(year), year, WondrousMinYear, WondrousMaxYear);

            var gregorianDateOfNawRuz = new DateTime(GregorianYearOfFirstWondrousYear - 1 + year, 3, GetYearInfo(year).NawRuzDayInMarch);

            return gregorianDateOfNawRuz;
        }

        protected override int CalculateStartOfYearDays(int year)
        {
            Preconditions.CheckArgumentRange(nameof(year), year, WondrousMinYear, WondrousMaxYear);

            // leverage Noda Time's Iso knowledge of when Jan 1 is, then add days to Naw Ruz
            var gregorianYear = year + GregorianYearOfFirstWondrousYear - 1;
            var gregorianStart = CalendarSystem.Iso.YearMonthDayCalculator.GetStartOfYearInDays(gregorianYear);

            var nawRuz = GregorianDateOfNawRuz(year);
            var firstDay = gregorianStart + nawRuz.DayOfYear - 1;

            return firstDay;
        }

        protected override int GetDaysFromStartOfYearToStartOfMonth(int year, int month)
        {
            var daysFromStartOfYearToStartOfMonth = DaysInMonth * (month - 1);

            if (month == Month19)
            {
                daysFromStartOfYearToStartOfMonth += GetYearInfo(year).DaysInAyyamiHa;
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

            Preconditions.CheckArgumentRange(nameof(nextYear), nextYear, WondrousMinYear, WondrousMaxYear);

            var result = new YearMonthDay(nextYear, nextMonthNum, nextDay);

            return result;
        }

        internal override int GetDaysInMonth(int year, int month)
        {
            Preconditions.CheckArgumentRange(nameof(year), year, WondrousMinYear, WondrousMaxYear);

            if (month == AyyamiHaMonth0)
            {
                return GetYearInfo(year).DaysInAyyamiHa;
            }

            return DaysInMonth;
        }

        internal override int GetDaysInYear(int year)
        {
            var daysInYear = 361 + GetYearInfo(year).DaysInAyyamiHa;

            return daysInYear;
        }

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
                daysSinceEpoch += DaysInAyyamiHa(year);
            }

            return daysSinceEpoch;
        }

        internal override int GetMonthsInYear(int year)
        {
            return MonthsInYear;
        }

        internal override YearMonthDay GetYearMonthDay(int year, int dayOfYear)
        {
            Preconditions.CheckArgumentRange(nameof(dayOfYear), dayOfYear, 1, GetDaysInYear(year));

            var firstOfLoftiness = DayOfYearOfStartOfMonth19(year);

            if (dayOfYear >= firstOfLoftiness)
            {
                return new YearMonthDay(year, Month19, dayOfYear - firstOfLoftiness + 1);
            }

            var month = Math.Min(1 + (dayOfYear - 1) / DaysInMonth, Month18);
            var day = dayOfYear - (month - 1) * DaysInMonth;

            return new YearMonthDay(year, month, day);
        }

        internal bool IsInAyyamiHa(YearMonthDay ymd)
        {
            return ymd.Month == Month18 && ymd.Day > DaysInMonth;
        }

        internal override bool IsLeapYear(int year)
        {
            return GetYearInfo(year).DaysInAyyamiHa != DaysInAyyamiHaInNormalYear;
        }

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
            Preconditions.CheckArgumentRange(nameof(newYear), newYear, WondrousMinYear, WondrousMaxYear);

            var month = start.Month;
            var day = start.Day;

            if (IsInAyyamiHa(start))
            {
                // Moving a year while within Ayyam-i-Ha is not well defined.
                // In this implementation, if starting on day 5, end on day 4 (stay in Ayyam-i-Ha)
                var daysInThisAyyamiHa = DaysInAyyamiHa(newYear);
                return new YearMonthDay(newYear, month, Math.Min(day, DaysInMonth + daysInThisAyyamiHa));
            }

            return new YearMonthDay(newYear, month, day);
        }

        internal override void ValidateYearMonthDay(int year, int month, int day)
        {
            Preconditions.CheckArgumentRange(nameof(year), year, WondrousMinYear, WondrousMaxYear);

            switch (month)
            {
                case AyyamiHaMonth0:
                    Preconditions.CheckArgumentRange(nameof(day), day, 1, DaysInAyyamiHa(year));
                    break;
                case Month18:
                    // allow Ayyam-i-Ha days in this month
                    Preconditions.CheckArgumentRange(nameof(day), day, 1, DaysInMonth + DaysInAyyamiHa(year));
                    break;
                default:
                    Preconditions.CheckArgumentRange(nameof(month), month, 1, MonthsInYear);
                    Preconditions.CheckArgumentRange(nameof(day), day, 1, DaysInMonth);
                    break;
            }
        }

        /// <summary>
        /// Create a <see cref="LocalDate"/> in the Wondrous calendar
        /// </summary>
        /// <param name="year">Year in the Wondrous calendar</param>
        /// <param name="month">Month (use 0 for Ayyam-i-Ha)</param>
        /// <param name="day">Day in month</param>
        /// <returns></returns>
        public static LocalDate CreateWondrousDate(int year, int month, int day)
        {
            if (month == AyyamiHaMonth0)
            {
                var maxDay = GetYearInfo(year).DaysInAyyamiHa;

                Preconditions.CheckArgumentRange(nameof(day), day, 1, maxDay);

                // move Ayyam-i-Ha days to fall after the last day of month 18.
                month = Month18;
                day += DaysInMonth;
            }
            return new LocalDate(year, month, day, CalendarSystem.Wondrous);
        }

        public int DaysInAyyamiHa(int year)
        {
            return GetYearInfo(year).DaysInAyyamiHa;
        }

        /// <summary>
        /// Return the day of this month. 
        /// </summary>
        /// <remarks>Deals with days in Ayyam-i-Ha.</remarks>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int WondrousDay(LocalDate input)
        {
            Preconditions.CheckArgument(input.Calendar == CalendarSystem.Wondrous, nameof(input), "Only valid when using the Wondrous calendar");

            if (input.Month == Month18 && input.Day > DaysInMonth)
            {
                return input.Day - DaysInMonth;
            }
            return input.Day;
        }

        /// <summary>
        /// Return the month of this date. If in Ayyam-i-Ha, returns 0.
        /// </summary>
        /// <remarks>Deals with Ayyam-i-Ha.</remarks>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int WondrousMonth(LocalDate input)
        {
            Preconditions.CheckArgument(input.Calendar == CalendarSystem.Wondrous, nameof(input), "Only valid when using the Wondrous calendar");

            if (input.Month == Month18 && input.Day > DaysInMonth)
            {
                return 0;
            }
            return input.Month;
        }

        [DebuggerDisplay("{DaysInAyyamiHa}-{NawRuzDayInMarch}")]
        public struct WondrousYearInfo
        {
            public int NawRuzDayInMarch;
            public int DaysInAyyamiHa;

            public WondrousYearInfo(int daysInAyyamiHa, int nawRuzDayInMarch)
            {
                NawRuzDayInMarch = nawRuzDayInMarch;
                DaysInAyyamiHa = daysInAyyamiHa;
            }
        }
    }
}