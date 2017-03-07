// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars.Wondrous;
using NodaTime.Utility;
using System;

namespace NodaTime.Calendars
{
    /// <summary>
    ///     See <see cref="CalendarSystem.GetWondrousCalendar" /> for details.
    /// </summary>
    internal sealed class WondrousYearMonthDayCalculator : YearMonthDayCalculator
    {
        private const int WondrousMinYear = 1;
        private const int WondrousMaxYear = 1000;

        private const int AverageDaysPer10Years = 3652; // Ideally 365.2425 per year...
        private const int DaysInMonth = 19;
        private const int MonthsInYear = 19;

        private const int Month18 = 18;
        private const int Month19 = 19;

        public const int AyyamiHaMonth0 = WondrousCalendarHelper.AyyamiHaMonth0;

        private const int UnixEpochDayAtStartOfYear1 = -45941;

        internal WondrousYearMonthDayCalculator()
            : base(WondrousMinYear,
                WondrousMaxYear - 1,
                AverageDaysPer10Years,
                UnixEpochDayAtStartOfYear1)
        {
            ConsoleWriteLine("~A Constructed");
            //            System.Diagnostics.Debugger.Launch();
        }


        /// <summary>
        ///     Use this to provide simple trace of calls.  Review output and sort to check test code coverage.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void ConsoleWriteLine(string format, params object[] args)
        {
#if !PCL
            Console.WriteLine(format, args);
#endif
        }

        protected override int GetDaysFromStartOfYearToStartOfMonth(int year, int month)
        {
            var daysFromStartOfYearToStartOfMonth = DaysInMonth * (month - 1);

            if (month == Month19)
            {
                daysFromStartOfYearToStartOfMonth += WondrousCalendarHelper.GetYearInfo(year).DaysInAyyamiHa;
            }

            ConsoleWriteLine("~B GetDayFromStartOfYearToStartOfMonth {0} {1} ==> {2}", year, month,
                daysFromStartOfYearToStartOfMonth);
            return daysFromStartOfYearToStartOfMonth;
        }

        /// <param name="wYear"></param>
        /// <remarks>Rely on Gregorian calendar for "days" in unix epoch</remarks>
        /// <remarks>Untested</remarks>
        /// <returns></returns>
        protected override int CalculateStartOfYearDays(int wYear)
        {
            Preconditions.CheckArgumentRange(nameof(wYear), wYear, WondrousMinYear, WondrousMaxYear);

            var gregorianYear = wYear + 1843;
            var gregorianStart = new GregorianYearMonthDayCalculator().GetStartOfYearInDays(gregorianYear);

            var nawRuz = GregorianDateOfNawRuz(wYear);

            var firstDay = gregorianStart + nawRuz.DayOfYear - 1;

            ConsoleWriteLine("~C CalcStartOfYearDays {0} ==> {1} ==> {2}", wYear, nawRuz, firstDay);

            return firstDay;
        }

        /// <summary>
        /// Test this possible date
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month">Ayyam-i-Ha is appended to month 18</param>
        /// <param name="day"></param>
        internal override void ValidateYearMonthDay(int year, int month, int day)
        {
            ConsoleWriteLine("~D Validate: {0}-{1}-{2}", year, month, day);

            Preconditions.CheckArgumentRange(nameof(year), year, WondrousMinYear, WondrousMaxYear);

            if (month == Month18)
            {
                // allow Ayyam-i-Ha days in this month
                Preconditions.CheckArgumentRange(nameof(day), day, 1, DaysInMonth + DaysInAyyamHa(year));
            }
            else
            {
                Preconditions.CheckArgumentRange(nameof(month), month, 1, MonthsInYear);
                Preconditions.CheckArgumentRange(nameof(day), day, 1, DaysInMonth);
            }
        }

        /// <param name="year"></param>
        /// <remarks>All years have 19 months</remarks>
        /// <remarks>Tested</remarks>
        /// <returns></returns>
        internal override int GetMonthsInYear(int year)
        {
            return MonthsInYear;
        }

        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <remarks>All months have 19 days. Ayyam-i-Ha usually has 4 or 5 days.</remarks>
        /// <returns></returns>
        internal override int GetDaysInMonth(int year, int month)
        {
            Preconditions.CheckArgumentRange(nameof(year), year, WondrousMinYear, WondrousMaxYear);

            var daysInMonth = DaysInMonth;

            if (month == AyyamiHaMonth0)
            {
                daysInMonth = WondrousCalendarHelper.GetYearInfo(year).DaysInAyyamiHa;
            }

            ConsoleWriteLine("~E GetDaysInMonth {0} {1} ==> {2}", year, month, daysInMonth);
            return daysInMonth;
        }

        internal override bool IsLeapYear(int year)
        {
            var isLeapYear = WondrousCalendarHelper.GetYearInfo(year).DaysInAyyamiHa != 4;

            ConsoleWriteLine("~F IsLeapYear {0} ==> {1}", year, isLeapYear);
            return isLeapYear;
        }

        internal bool IsInAyyamiHa(YearMonthDay ymd)
        {
            var isInAyyamiHa = ymd.Month == Month18 && ymd.Day > DaysInMonth;

            ConsoleWriteLine("~F InAyyamiHa {0} ==> {1}", ymd, isInAyyamiHa);
            return isInAyyamiHa;
        }

        /// <param name="start"></param>
        /// <param name="months"></param>
        /// <remarks>
        ///     Adding months around Ayyam-i-Ha is not well defined. Because they are "days outside of time", this
        ///     implementation will skip over them.
        ///     Can never get to Ayyam-i-Ha with this. From Month 18, will go to month 19.
        /// </remarks>
        /// <returns></returns>
        internal override YearMonthDay AddMonths(YearMonthDay start, int months)
        {
            if (months == 0)
            {
                ConsoleWriteLine("~G AddMonths {0}", months);
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

            ConsoleWriteLine("~H AddMonths {0} {1} ==> {2}", start, months, result);

            return result;
        }

        ///// <param name="start">M - x</param>
        ///// <param name="end">x - S</param>
        ///// <remarks>Untested</remarks>
        ///// <returns></returns>
        //internal override int MonthsBetween(YearMonthDay start, YearMonthDay end)
        //{
        //    var monthsBetween = (end.Year * MonthsInYear + end.Month)
        //          - (start.Year * MonthsInYear + start.Month);

        //    ConsoleWriteLine("~I MonthsBetween {0} and {1} ==> {2}", start, end, monthsBetween);

        //    return monthsBetween;
        //}


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
            if (newYear == start.Year)
            {
                return start;
            }

            Preconditions.CheckArgumentRange(nameof(newYear), newYear, WondrousMinYear, WondrousMaxYear);

            var month = start.Month;
            var day = start.Day;

            YearMonthDay result;
            if (IsInAyyamiHa(start))
            {
                // moving a year while within Ayyam-i-Ha is not defined
                // in this implementation, if starting on day 5, end on day 4 (stay in Ayyam-i-Ha)
                var daysInThisAyyamiHa = DaysInAyyamHa(newYear);
                result = new YearMonthDay(newYear, month, Math.Min(day, DaysInMonth + daysInThisAyyamiHa));
            }
            else
            {
                result = new YearMonthDay(newYear, month, day);
            }

            ConsoleWriteLine("~J SetYear {0} {1} ==> {2}", start, newYear, result);
            return result;
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
                daysSinceEpoch += DaysInAyyamHa(year);
            }

            ConsoleWriteLine("~K GetDaysSinceEpoch {0} ==> {1}", target, daysSinceEpoch);

            return daysSinceEpoch;
        }

        /// <summary>
        /// </summary>
        /// <param name="year"></param>
        /// <remarks>Can only know by looking at when next year starts.</remarks>
        /// <remarks>Untested</remarks>
        /// <returns></returns>
        internal override int GetDaysInYear(int year)
        {
            var daysInYear = 361 + WondrousCalendarHelper.GetYearInfo(year).DaysInAyyamiHa;

            ConsoleWriteLine("~L GetDaysInYear {0} ==> {1}", year, daysInYear);
            return daysInYear;
        }

        /// <summary>
        /// </summary>
        /// <param name="year">Wondrous Year</param>
        /// <param name="dayOfYear"></param>
        /// <remarks>Untested</remarks>
        /// <returns></returns>
        internal override YearMonthDay GetYearMonthDay(int year, int dayOfYear)
        {
            Preconditions.CheckArgumentRange(nameof(dayOfYear), dayOfYear, 1, GetDaysInYear(year));

            YearMonthDay yearMonthDay;

            var firstOfLoftiness = DayOfYearOfStartOfMonth19(year);

            if (dayOfYear >= firstOfLoftiness)
            {
                yearMonthDay = new YearMonthDay(year, Month19, dayOfYear - firstOfLoftiness + 1);
            }
            else
            {
                var month = Math.Min(1 + (dayOfYear - 1) / DaysInMonth, Month18);
                var day = dayOfYear - (month - 1) * DaysInMonth;

                yearMonthDay = new YearMonthDay(year, month, day);
            }

            ConsoleWriteLine("~M GetYearMonthDay {0} {1} ==> {2}", year, dayOfYear, yearMonthDay);

            return yearMonthDay;
        }

        /// <param name="wYear"></param>
        /// <remarks>The exact day (between sunsets) of the equinox in Tehran. Look up in precalculated table.</remarks>
        /// <remarks>Untested</remarks>
        /// <returns></returns>
        private DateTime GregorianDateOfNawRuz(int wYear)
        {
            Preconditions.CheckArgumentRange(nameof(wYear), wYear, WondrousMinYear, WondrousMaxYear);

            var gregorianDateOfNawRuz = new DateTime(1843 + wYear, 3, WondrousCalendarHelper.GetYearInfo(wYear).NawRuzDayInMarch);

            ConsoleWriteLine("~N GregorianDateOfNawRuz {0} => {1}", wYear, gregorianDateOfNawRuz);
            return gregorianDateOfNawRuz;
        }

        /// <param name="wYear"></param>
        /// <remarks>Untested</remarks>
        /// <returns></returns>
        private int DayOfYearOfStartOfMonth19(int wYear)
        {
            var dayOfYearOfStartOfMonth19 = 1 + DaysInMonth * Month18 + WondrousCalendarHelper.GetYearInfo(wYear).DaysInAyyamiHa;

            ConsoleWriteLine("~O DayOfYearOfStartOfMonth19 {0} => {1}", wYear, dayOfYearOfStartOfMonth19);
            return dayOfYearOfStartOfMonth19;
        }

        public int DaysInAyyamHa(int wYear)
        {
            var daysInAyyamHa = WondrousCalendarHelper.GetYearInfo(wYear).DaysInAyyamiHa;

            ConsoleWriteLine("~P DaysInAyyamHa {0} ==> {1}", wYear, daysInAyyamHa);
            return daysInAyyamHa;
        }

    }
}