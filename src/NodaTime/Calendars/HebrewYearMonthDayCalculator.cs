// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Globalization;

namespace NodaTime.Calendars
{
    /// <summary>
    /// See <see cref="CalendarSystem.GetHebrewCalendar()" /> for details.
    /// </summary>
    internal sealed class HebrewYearMonthDayCalculator : YearMonthDayCalculator
    {
        private const int AbsoluteDayOfUnixEpoch = 719163;
        private const int AbsoluteDayOfHebrewEpoch = -1373427;
        private readonly Func<int, int, int> calendarToEcclesiastical;
        private readonly Func<int, int, int> ecclesiasticalToCalendar;

        internal HebrewYearMonthDayCalculator(HebrewMonthNumbering monthNumbering)
            : base(1, // Min year
                  30000, // Max year (very conservative, but let's not worry...)
                  (long) (363.4 * NodaConstants.TicksPerStandardDay), // Average year length
                  (AbsoluteDayOfHebrewEpoch - AbsoluteDayOfUnixEpoch) * NodaConstants.TicksPerStandardDay, // Tick at year 1
                  new[] { Era.AnnoMundi })
        {
            switch (monthNumbering)
            {
                case HebrewMonthNumbering.Civil:
                    calendarToEcclesiastical = HebrewMonthConverter.CivilToEcclesiastical;
                    ecclesiasticalToCalendar = HebrewMonthConverter.EcclesiasticalToCivil;
                    break;
                case HebrewMonthNumbering.Ecclesiastical:
                    calendarToEcclesiastical = NoOp;
                    ecclesiasticalToCalendar = NoOp;
                    break;
            }
        }

        private static int NoOp(int year, int month)
        {
            return month;
        }

        /// <summary>
        /// Returns whether or not the given year is a leap year - that is, one with 13 months. This is
        /// not quite the same as a leap year in (say) the Gregorian calendar system...
        /// </summary>
        internal override bool IsLeapYear(int year)
        {
            return HebrewEcclesiasticalCalculator.IsLeapYear(year);
        }

        protected override long GetTicksFromStartOfYearToStartOfMonth(int year, int month)
        {
            int ecclesiasticalMonth = calendarToEcclesiastical(year, month);
            int absoluteDayAtStartOfMonth = HebrewEcclesiasticalCalculator.AbsoluteFromHebrew(year, ecclesiasticalMonth, 1);
            // 7 = first month of year...
            int absoluteDayAtStartOfYear = HebrewEcclesiasticalCalculator.AbsoluteFromHebrew(year, 7, 1);
            return (absoluteDayAtStartOfMonth - absoluteDayAtStartOfYear) * NodaConstants.TicksPerStandardDay;
        }

        protected override int CalculateStartOfYearDays(int year)
        {
            // Note that we might get called with a year of 0 here. I think that will still be okay,
            // given CalendricalAlgorithms.
            // Note that we pass in a month of 7, as that's the start of the year in calendrical numbering.
            int absoluteDay = HebrewEcclesiasticalCalculator.AbsoluteFromHebrew(year, 7, 1);
            return absoluteDay - AbsoluteDayOfUnixEpoch;
        }

        protected override int GetMonthOfYear(LocalInstant localInstant, int year)
        {
            // Ignore the year we're given...
            int absoluteDay = AbsoluteDayFromLocalInstant(localInstant);
            YearMonthDay ymd = HebrewEcclesiasticalCalculator.HebrewFromAbsolute(absoluteDay);
            return ecclesiasticalToCalendar(year, ymd.Month);
        }

        protected override long GetTicksInYear(int year)
        {
            return HebrewEcclesiasticalCalculator.DaysInYear(year) * NodaConstants.TicksPerStandardDay;
        }

        internal override int GetMaxMonth(int year)
        {
            return IsLeapYear(year) ? 13 : 12;
        }

        internal override LocalInstant SetYear(LocalInstant localInstant, int year)
        {
            // TODO: Validate this. It's not at all clear that it makes any sense at all.
            // We try to preserve the calendrical month number where possible, which
            // corresponds to the month name. That may change the BCL month number, if we
            // start in a leap year and end in a non-leap year, or vice versa.

            long tickOfDay = TimeOfDayCalculator.GetTickOfDay(localInstant);
            int absoluteSourceDay = AbsoluteDayFromLocalInstant(localInstant);
            YearMonthDay ymd = HebrewEcclesiasticalCalculator.HebrewFromAbsolute(absoluteSourceDay);
            int targetCalendricalMonth = ymd.Month;
            if (targetCalendricalMonth == 13 && !IsLeapYear(year))
            {
                targetCalendricalMonth = 12;
            }
            int targetDay = Math.Min(ymd.Day, HebrewEcclesiasticalCalculator.DaysInMonth(year, targetCalendricalMonth));

            int absoluteTargetDay = HebrewEcclesiasticalCalculator.AbsoluteFromHebrew(year, targetCalendricalMonth, targetDay);
            return new LocalInstant((absoluteTargetDay - AbsoluteDayOfUnixEpoch) * NodaConstants.TicksPerStandardDay + tickOfDay);
        }

        internal override int GetDaysInMonth(int year, int month)
        {
            return HebrewEcclesiasticalCalculator.DaysInMonth(year, calendarToEcclesiastical(year, month));
        }

        internal override LocalInstant AddMonths(LocalInstant localInstant, int months)
        {
            throw new NotImplementedException();
        }

        internal override int MonthsBetween(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts a LocalInstant into an absolute day number.
        /// </summary>
        private static int AbsoluteDayFromLocalInstant(LocalInstant localInstant)
        {
            long ticks = localInstant.Ticks;
            int daysSinceUnixEpoch = ticks >= 0
                ? TickArithmetic.TicksToDays(ticks)
                : (int) ((ticks - (NodaConstants.TicksPerStandardDay - 1)) / NodaConstants.TicksPerStandardDay);
            return daysSinceUnixEpoch + AbsoluteDayOfUnixEpoch;
        }
    }
}
