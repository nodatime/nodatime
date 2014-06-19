// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;

namespace NodaTime.Calendars
{
    /// <summary>
    /// See <see cref="CalendarSystem.GetHebrewCalendar" /> for details. This is effectively
    /// an adapter around <see cref="HebrewScripturalCalculator"/>.
    /// </summary>
    internal sealed class HebrewYearMonthDayCalculator : YearMonthDayCalculator
    {
        private const int ScripturalYearStartMonth = 7;
        private const int AbsoluteDayOfUnixEpoch = 719163;
        private const int AbsoluteDayOfHebrewEpoch = -1373427;
        private const int MonthsPerLeapCycle = 235;
        private const int YearsPerLeapCycle = 19;
        private readonly Func<int, int, int> calendarToScriptural;
        private readonly Func<int, int, int> scripturalToCalendar;

        internal HebrewYearMonthDayCalculator(HebrewMonthNumbering monthNumbering)
            : base(HebrewScripturalCalculator.MinYear,
                  HebrewScripturalCalculator.MaxYear,
                  (long) (365.4 * NodaConstants.TicksPerStandardDay), // Average year length
                  (AbsoluteDayOfHebrewEpoch - AbsoluteDayOfUnixEpoch) * NodaConstants.TicksPerStandardDay, // Tick at year 1
                  new[] { Era.AnnoMundi })
        {
            switch (monthNumbering)
            {
                case HebrewMonthNumbering.Civil:
                    calendarToScriptural = HebrewMonthConverter.CivilToScriptural;
                    scripturalToCalendar = HebrewMonthConverter.ScripturalToCivil;
                    break;
                case HebrewMonthNumbering.Scriptural:
                    calendarToScriptural = NoOp;
                    scripturalToCalendar = NoOp;
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
            return HebrewScripturalCalculator.IsLeapYear(year);
        }

        protected override long GetTicksFromStartOfYearToStartOfMonth(int year, int month)
        {
            int scripturalMonth = calendarToScriptural(year, month);
            int absoluteDayAtStartOfMonth = HebrewScripturalCalculator.AbsoluteFromHebrew(year, scripturalMonth, 1);
            int absoluteDayAtStartOfYear = HebrewScripturalCalculator.AbsoluteFromHebrew(year, ScripturalYearStartMonth, 1);
            return (absoluteDayAtStartOfMonth - absoluteDayAtStartOfYear) * NodaConstants.TicksPerStandardDay;
        }

        protected override int CalculateStartOfYearDays(int year)
        {
            // Note that we might get called with a year of 0 here. I think that will still be okay,
            // given how HebrewScripturalCalculator works.
            int absoluteDay = HebrewScripturalCalculator.AbsoluteFromHebrew(year, ScripturalYearStartMonth, 1);
            return absoluteDay - AbsoluteDayOfUnixEpoch;
        }

        internal override int GetMonthOfYear(LocalInstant localInstant)
        {
            int absoluteDay = AbsoluteDayFromLocalInstant(localInstant);
            YearMonthDay ymd = HebrewScripturalCalculator.HebrewFromAbsolute(absoluteDay);
            return scripturalToCalendar(ymd.Year, ymd.Month);
        }

        internal override int GetDaysInYear(int year)
        {
            return HebrewScripturalCalculator.DaysInYear(year);
        }

        protected override long GetTicksInYear(int year)
        {
            return GetDaysInYear(year) * NodaConstants.TicksPerStandardDay;
        }

        internal override int GetDayOfMonth(LocalInstant localInstant)
        {
            int absoluteDay = AbsoluteDayFromLocalInstant(localInstant);
            YearMonthDay ymd = HebrewScripturalCalculator.HebrewFromAbsolute(absoluteDay);
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
            return IsLeapYear(year) ? 13 : 12;
        }

        /// <summary>
        /// Change the year, maintaining month and day as well as possible. This doesn't
        /// work in the same way as other calendars; see http://judaism.stackexchange.com/questions/39053
        /// for the reasoning behind the rules.
        /// </summary>
        internal override LocalInstant SetYear(LocalInstant localInstant, int year)
        {
            long tickOfDay = TimeOfDayCalculator.GetTickOfDay(localInstant);
            int absoluteSourceDay = AbsoluteDayFromLocalInstant(localInstant);
            YearMonthDay ymd = HebrewScripturalCalculator.HebrewFromAbsolute(absoluteSourceDay);
            int targetDay = ymd.Day;
            int targetScripturalMonth = ymd.Month;
            if (targetScripturalMonth == 13 && !IsLeapYear(year))
            {
                // If we were in Adar II and the target year is not a leap year, map to Adar.
                targetScripturalMonth = 12;
            }
            else if (targetScripturalMonth == 12 && IsLeapYear(year) && !IsLeapYear(ymd.Year))
            {
                // If we were in Adar (non-leap year), go to Adar II rather than Adar I in a leap year.
                targetScripturalMonth = 13;
            }
            // If we're aiming for the 30th day of Heshvan, Kislev or an Adar, it's possible that the change in year
            // has meant the day becomes invalid. In that case, roll over to the 1st of the subsequent month.
            if (targetDay == 30 && (targetScripturalMonth == 8 || targetScripturalMonth == 9 || targetScripturalMonth == 12))
            {
                if (HebrewScripturalCalculator.DaysInMonth(year, targetScripturalMonth) != 30)
                {
                    targetDay = 1;
                    targetScripturalMonth++;
                    // From Adar, roll to Nisan.
                    if (targetScripturalMonth == 13)
                    {
                        targetScripturalMonth = 1;
                    }
                }
            }
            int absoluteTargetDay = HebrewScripturalCalculator.AbsoluteFromHebrew(year, targetScripturalMonth, targetDay);
            return LocalInstantFromAbsoluteDay(absoluteTargetDay, tickOfDay);
        }

        internal override int GetDaysInMonth(int year, int month)
        {
            return HebrewScripturalCalculator.DaysInMonth(year, calendarToScriptural(year, month));
        }

        internal override LocalInstant AddMonths(LocalInstant localInstant, int months)
        {
            // Note: this method gives the same result regardless of the month numbering used
            // by the instance. The method works in terms of civil month numbers for most of
            // the time in order to simplify the logic.
            if (months == 0)
            {
                return localInstant;
            }
            long tickOfDay = TimeOfDayCalculator.GetTickOfDay(localInstant);
            var startDate = HebrewScripturalCalculator.HebrewFromAbsolute(AbsoluteDayFromLocalInstant(localInstant));
            int year = startDate.Year;
            int month = HebrewMonthConverter.ScripturalToCivil(year, startDate.Month);
            // This arithmetic works the same both backwards and forwards.
            year += (months / MonthsPerLeapCycle) * YearsPerLeapCycle;
            months = months % MonthsPerLeapCycle;
            if (months > 0)
            {
                // Add as many months as we need to in order to act as if we'd begun at the start
                // of the year, for simplicity.
                months += month - 1;
                // Add a year at a time
                while (months >= GetMaxMonth(year))
                {
                    months -= GetMaxMonth(year);
                    year++;
                }
                // However many months we've got left to add tells us the final month.
                month = months + 1;
            }
            else
            {
                // Pretend we were given the month at the end of the years.
                months -= GetMaxMonth(year) - month;
                // Subtract a year at a time
                while (months + GetMaxMonth(year) <= 0)
                {
                    months += GetMaxMonth(year);
                    year--;
                }
                // However many months we've got left to add (which will still be negative...)
                // tells us the final month.
                month = GetMaxMonth(year) + months;
            }

            // Convert back to scriptural for the last bit
            month = HebrewMonthConverter.CivilToScriptural(year, month);
            int day = Math.Min(HebrewScripturalCalculator.DaysInMonth(year, month), startDate.Day);
            int absoluteDay = HebrewScripturalCalculator.AbsoluteFromHebrew(year, month, day);
            return LocalInstantFromAbsoluteDay(absoluteDay, tickOfDay);
        }

        // Note to self: this is (minuendInstant - subtrahendInstant) in months. So if minuendInstant
        // is later than subtrahendInstant, the result should be positive.
        internal override int MonthsBetween(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            var minuendDate = HebrewScripturalCalculator.HebrewFromAbsolute(AbsoluteDayFromLocalInstant(minuendInstant));
            var subtrahendDate = HebrewScripturalCalculator.HebrewFromAbsolute(AbsoluteDayFromLocalInstant(subtrahendInstant));
            // First (quite rough) guess... we could probably be more efficient than this, but it's unlikely to be very far off.
            double minuendMonths = (minuendDate.Year * MonthsPerLeapCycle) / (double) YearsPerLeapCycle + HebrewMonthConverter.ScripturalToCivil(minuendDate.Year, minuendDate.Month);
            double subtrahendMonths = (subtrahendDate.Year * MonthsPerLeapCycle) / (double) YearsPerLeapCycle + HebrewMonthConverter.ScripturalToCivil(subtrahendDate.Year, subtrahendDate.Month);
            int diff = (int) (minuendMonths - subtrahendMonths);

            if (subtrahendInstant <= minuendInstant)
            {
                // Go backwards until we've got a tight upper bound...
                while (AddMonths(subtrahendInstant, diff) > minuendInstant)
                {
                    diff--;
                }
                // Go forwards until we've overshot
                while (AddMonths(subtrahendInstant, diff) <= minuendInstant)
                {
                    diff++;
                }
                // Take account of the overshoot
                return diff - 1;
            }
            else
            {
                // Moving backwards, so we need to end up with a result greater than or equal to
                // minuendInstant...
                // Go forwards until we've got a tight upper bound...
                while (AddMonths(subtrahendInstant, diff) < minuendInstant)
                {
                    diff++;
                }
                // Go backwards until we've overshot
                while (AddMonths(subtrahendInstant, diff) >= minuendInstant)
                {
                    diff--;
                }
                // Take account of the overshoot
                return diff + 1;
            }
        }

        /// <summary>
        /// Converts a LocalInstant into an absolute day number.
        /// </summary>
        private static int AbsoluteDayFromLocalInstant(LocalInstant localInstant)
        {
            int daysSinceUnixEpoch = TickArithmetic.TicksToDays(localInstant.Ticks);
            return daysSinceUnixEpoch + AbsoluteDayOfUnixEpoch;
        }

        private static LocalInstant LocalInstantFromAbsoluteDay(int absoluteDay, long tickOfDay)
        {
            int daysSinceUnixEpoch = absoluteDay - AbsoluteDayOfUnixEpoch;
            return new LocalInstant(daysSinceUnixEpoch * NodaConstants.TicksPerStandardDay + tickOfDay);
        }
    }
}
