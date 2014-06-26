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
                  3654, // Average length of 10 years
                  (AbsoluteDayOfHebrewEpoch - AbsoluteDayOfUnixEpoch), // Day at year 1
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

        protected override int GetDaysFromStartOfYearToStartOfMonth(int year, int month)
        {
            int scripturalMonth = calendarToScriptural(year, month);
            int absoluteDayAtStartOfMonth = HebrewScripturalCalculator.AbsoluteFromHebrew(year, scripturalMonth, 1);
            int absoluteDayAtStartOfYear = HebrewScripturalCalculator.AbsoluteFromHebrew(year, ScripturalYearStartMonth, 1);
            return absoluteDayAtStartOfMonth - absoluteDayAtStartOfYear;
        }

        protected override int CalculateStartOfYearDays(int year)
        {
            // Note that we might get called with a year of 0 here. I think that will still be okay,
            // given how HebrewScripturalCalculator works.
            int absoluteDay = HebrewScripturalCalculator.AbsoluteFromHebrew(year, ScripturalYearStartMonth, 1);
            return absoluteDay - AbsoluteDayOfUnixEpoch;
        }

        internal override YearMonthDay GetYearMonthDay(int daysSinceEpoch)
        {
            int absoluteDay = daysSinceEpoch + AbsoluteDayOfUnixEpoch;
            YearMonthDay scripturalYmd = HebrewScripturalCalculator.HebrewFromAbsolute(absoluteDay);
            return new YearMonthDay(scripturalYmd.Year, scripturalToCalendar(scripturalYmd.Year, scripturalYmd.Month), scripturalYmd.Day);
        }

        internal override int GetDaysInYear(int year)
        {
            return HebrewScripturalCalculator.DaysInYear(year);
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
        internal override YearMonthDay SetYear(YearMonthDay yearMonthDay, int year)
        {
            int currentYear = yearMonthDay.Year;
            int currentMonth = yearMonthDay.Month;
            int targetDay = yearMonthDay.Day;
            int targetScripturalMonth = calendarToScriptural(currentYear, currentMonth);
            if (targetScripturalMonth == 13 && !IsLeapYear(year))
            {
                // If we were in Adar II and the target year is not a leap year, map to Adar.
                targetScripturalMonth = 12;
            }
            else if (targetScripturalMonth == 12 && IsLeapYear(year) && !IsLeapYear(currentYear))
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
            int targetCalendarMonth = scripturalToCalendar(year, targetScripturalMonth);
            return new YearMonthDay(year, targetCalendarMonth, targetDay);
        }

        internal override int GetDaysInMonth(int year, int month)
        {
            return HebrewScripturalCalculator.DaysInMonth(year, calendarToScriptural(year, month));
        }

        internal override YearMonthDay AddMonths(YearMonthDay yearMonthDay, int months)
        {
            // Note: this method gives the same result regardless of the month numbering used
            // by the instance. The method works in terms of civil month numbers for most of
            // the time in order to simplify the logic. There may be pointless conversions at
            // the start and end. TODO(2.0): Remove the pointlessness...
            if (months == 0)
            {
                return yearMonthDay;
            }
            int year = yearMonthDay.Year;
            int scripturalMonth = calendarToScriptural(year, yearMonthDay.Month);
            int month = HebrewMonthConverter.ScripturalToCivil(year, scripturalMonth);
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
            int day = Math.Min(HebrewScripturalCalculator.DaysInMonth(year, month), yearMonthDay.Day);
            month = scripturalToCalendar(year, month);
            return new YearMonthDay(year, month, day);
        }

        // Note to self: this is (minuendInstant - subtrahendInstant) in months. So if minuendInstant
        // is later than subtrahendInstant, the result should be positive.
        internal override int MonthsBetween(YearMonthDay minuendDate, YearMonthDay subtrahendDate)
        {
            // TODO(2.0): Tidy this up!
            // First (quite rough) guess... we could probably be more efficient than this, but it's unlikely to be very far off.
            double minuendMonths = (minuendDate.Year * MonthsPerLeapCycle) / (double) YearsPerLeapCycle + HebrewMonthConverter.ScripturalToCivil(minuendDate.Year, calendarToScriptural(minuendDate.Year, minuendDate.Month));
            double subtrahendMonths = (subtrahendDate.Year * MonthsPerLeapCycle) / (double) YearsPerLeapCycle + HebrewMonthConverter.ScripturalToCivil(subtrahendDate.Year, calendarToScriptural(subtrahendDate.Year, subtrahendDate.Month));
            int diff = (int) (minuendMonths - subtrahendMonths);

            if (Compare(subtrahendDate, minuendDate) <= 0)
            {
                // Go backwards until we've got a tight upper bound...
                while (Compare(AddMonths(subtrahendDate, diff), minuendDate) > 0)
                {
                    diff--;
                }
                // Go forwards until we've overshot
                while (Compare(AddMonths(subtrahendDate, diff), minuendDate) <= 0)
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
                while (Compare(AddMonths(subtrahendDate, diff), minuendDate) < 0)
                {
                    diff++;
                }
                // Go backwards until we've overshot
                while (Compare(AddMonths(subtrahendDate, diff), minuendDate) >= 0)
                {
                    diff--;
                }
                // Take account of the overshoot
                return diff + 1;
            }
        }

        public override int Compare(YearMonthDay lhs, YearMonthDay rhs)
        {
            // TODO(2.0): In the civil calendar, we can just use lhs.CompareTo(rhs) without conversion.
            int yearComparison = lhs.Year.CompareTo(rhs.Year);
            if (yearComparison != 0)
            {
                return yearComparison;
            }
            int lhsCivilMonth = HebrewMonthConverter.ScripturalToCivil(lhs.Year, calendarToScriptural(lhs.Year, lhs.Month));
            int rhsCivilMonth = HebrewMonthConverter.ScripturalToCivil(rhs.Year, calendarToScriptural(rhs.Year, rhs.Month));
            int monthComparison = lhsCivilMonth.CompareTo(rhsCivilMonth);
            if (monthComparison != 0)
            {
                return monthComparison;
            }
            return lhs.Day.CompareTo(rhs.Day);
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
