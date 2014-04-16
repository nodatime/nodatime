// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;

namespace NodaTime.Calendars
{
    /// <summary>
    /// See <see cref="CalendarSystem.GetHebrewCalendar" /> for details. This is effectively
    /// an adapter around <see cref="HebrewEcclesiasticalCalculator"/>.
    /// </summary>
    internal sealed class HebrewYearMonthDayCalculator : YearMonthDayCalculator
    {
        private const int EcclesiasticalYearStartMonth = 7;
        private const int AbsoluteDayOfUnixEpoch = 719163;
        private const int AbsoluteDayOfHebrewEpoch = -1373427;
        private readonly Func<int, int, int> calendarToEcclesiastical;
        private readonly Func<int, int, int> ecclesiasticalToCalendar;

        internal HebrewYearMonthDayCalculator(HebrewMonthNumbering monthNumbering)
            : base(HebrewEcclesiasticalCalculator.MinYear,
                  HebrewEcclesiasticalCalculator.MaxYear,
                  (long) (365.4 * NodaConstants.TicksPerStandardDay), // Average year length
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
            int absoluteDayAtStartOfYear = HebrewEcclesiasticalCalculator.AbsoluteFromHebrew(year, EcclesiasticalYearStartMonth, 1);
            return (absoluteDayAtStartOfMonth - absoluteDayAtStartOfYear) * NodaConstants.TicksPerStandardDay;
        }

        protected override int CalculateStartOfYearDays(int year)
        {
            // Note that we might get called with a year of 0 here. I think that will still be okay,
            // given how HebrewEcclesiaticalCalculator works.
            int absoluteDay = HebrewEcclesiasticalCalculator.AbsoluteFromHebrew(year, EcclesiasticalYearStartMonth, 1);
            return absoluteDay - AbsoluteDayOfUnixEpoch;
        }

        internal override int GetMonthOfYear(LocalInstant localInstant)
        {
            int absoluteDay = AbsoluteDayFromLocalInstant(localInstant);
            YearMonthDay ymd = HebrewEcclesiasticalCalculator.HebrewFromAbsolute(absoluteDay);
            return ecclesiasticalToCalendar(ymd.Year, ymd.Month);
        }

        internal override int GetDaysInYear(int year)
        {
            return HebrewEcclesiasticalCalculator.DaysInYear(year);
        }

        protected override long GetTicksInYear(int year)
        {
            return GetDaysInYear(year) * NodaConstants.TicksPerStandardDay;
        }

        internal override int GetDayOfMonth(LocalInstant localInstant)
        {
            int absoluteDay = AbsoluteDayFromLocalInstant(localInstant);
            YearMonthDay ymd = HebrewEcclesiasticalCalculator.HebrewFromAbsolute(absoluteDay);
            return ymd.Day;
        }

        protected override int GetMonthOfYear(LocalInstant localInstant, int year)
        {
            // Ignore the year we're given...
            // TODO: Consider throwing InvalidOperationException, as we're overriding the only methods
            // that should call this...
            return GetMonthOfYear(localInstant);
        }

        internal override int GetMaxMonth(int year)
        {
            // TODO: Argument validation
            return IsLeapYear(year) ? 13 : 12;
        }

        internal override LocalInstant SetYear(LocalInstant localInstant, int year)
        {
            // TODO: Validate this. It's not at all clear that it makes any sense at all.
            // We try to preserve the ecclesiastical month number where possible, which
            // corresponds to the month name. That may change the civil month number, if we
            // start in a leap year and end in a non-leap year, or vice versa.

            long tickOfDay = TimeOfDayCalculator.GetTickOfDay(localInstant);
            int absoluteSourceDay = AbsoluteDayFromLocalInstant(localInstant);
            YearMonthDay ymd = HebrewEcclesiasticalCalculator.HebrewFromAbsolute(absoluteSourceDay);
            int targetDay = ymd.Day;
            int targetEcclesiasticalMonth = ymd.Month;
            if (targetEcclesiasticalMonth == 13 && !IsLeapYear(year))
            {
                // If we were in Adar II, go to the end of Adar I.
                targetEcclesiasticalMonth = 12;
                targetDay = 40; // Definitely beyond the end of the month, so we'll truncate below.
            }
            targetDay = Math.Min(targetDay, HebrewEcclesiasticalCalculator.DaysInMonth(year, targetEcclesiasticalMonth));

            int absoluteTargetDay = HebrewEcclesiasticalCalculator.AbsoluteFromHebrew(year, targetEcclesiasticalMonth, targetDay);
            return new LocalInstant((absoluteTargetDay - AbsoluteDayOfUnixEpoch) * NodaConstants.TicksPerStandardDay + tickOfDay);
        }

        internal override int GetDaysInMonth(int year, int month)
        {
            // TODO: Argument validation
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
            int daysSinceUnixEpoch = TickArithmetic.TicksToDays(localInstant.Ticks);
            return daysSinceUnixEpoch + AbsoluteDayOfUnixEpoch;
        }
    }
}
