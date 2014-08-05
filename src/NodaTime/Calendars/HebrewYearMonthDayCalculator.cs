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
        private const int UnixEpochDayAtStartOfYear1 = -2092590;
        private const int MonthsPerLeapCycle = 235;
        private const int YearsPerLeapCycle = 19;
        private readonly HebrewMonthNumbering monthNumbering;

        internal HebrewYearMonthDayCalculator(HebrewMonthNumbering monthNumbering)
            : base(HebrewScripturalCalculator.MinYear,
                  HebrewScripturalCalculator.MaxYear,
                  3654, // Average length of 10 years
                  UnixEpochDayAtStartOfYear1)
        {
            this.monthNumbering = monthNumbering;
        }

        private int CalendarToCivilMonth(int year, int month)
        {
            return monthNumbering == HebrewMonthNumbering.Civil ? month : HebrewMonthConverter.ScripturalToCivil(year, month);
        }

        private int CalendarToScripturalMonth(int year, int month)
        {
            return monthNumbering == HebrewMonthNumbering.Scriptural ? month : HebrewMonthConverter.CivilToScriptural(year, month);
        }

        private int CivilToCalendarMonth(int year, int month)
        {
            return monthNumbering == HebrewMonthNumbering.Civil ? month : HebrewMonthConverter.CivilToScriptural(year, month);
        }

        private int ScripturalToCalendarMonth(int year, int month)
        {
            return monthNumbering == HebrewMonthNumbering.Scriptural ? month : HebrewMonthConverter.ScripturalToCivil(year, month);
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
            int scripturalMonth = CalendarToScripturalMonth(year, month);
            return HebrewScripturalCalculator.GetDaysFromStartOfYearToStartOfMonth(year, scripturalMonth);
        }

        protected override int CalculateStartOfYearDays(int year)
        {
            // Note that we might get called with a year of 0 here. I think that will still be okay,
            // given how HebrewScripturalCalculator works.
            int daysSinceHebrewEpoch = HebrewScripturalCalculator.ElapsedDays(year) - 1; // ElapsedDays returns 1 for year 1.
            return daysSinceHebrewEpoch + UnixEpochDayAtStartOfYear1;
        }

        internal override YearMonthDay GetYearMonthDay(int year, int dayOfYear)
        {
            YearMonthDay scriptural = HebrewScripturalCalculator.GetYearMonthDay(year, dayOfYear);
            return monthNumbering == HebrewMonthNumbering.Scriptural ? scriptural : new YearMonthDay(year, HebrewMonthConverter.ScripturalToCivil(year, scriptural.Month), scriptural.Day);
        }

        internal override int GetDaysInYear(int year)
        {
            return HebrewScripturalCalculator.DaysInYear(year);
        }

        internal override int GetMonthsInYear(int year)
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
            int targetScripturalMonth = CalendarToScripturalMonth(currentYear, currentMonth);
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
            int targetCalendarMonth = ScripturalToCalendarMonth(year, targetScripturalMonth);
            return new YearMonthDay(year, targetCalendarMonth, targetDay);
        }

        internal override int GetDaysInMonth(int year, int month)
        {
            return HebrewScripturalCalculator.DaysInMonth(year, CalendarToScripturalMonth(year, month));
        }

        internal override YearMonthDay AddMonths(YearMonthDay yearMonthDay, int months)
        {
            // Note: this method gives the same result regardless of the month numbering used
            // by the instance. The method works in terms of civil month numbers for most of
            // the time in order to simplify the logic.
            if (months == 0)
            {
                return yearMonthDay;
            }
            int year = yearMonthDay.Year;
            int month = CalendarToCivilMonth(year, yearMonthDay.Month);
            // This arithmetic works the same both backwards and forwards.
            year += (months / MonthsPerLeapCycle) * YearsPerLeapCycle;
            months = months % MonthsPerLeapCycle;
            if (months > 0)
            {
                // Add as many months as we need to in order to act as if we'd begun at the start
                // of the year, for simplicity.
                months += month - 1;
                // Add a year at a time
                while (months >= GetMonthsInYear(year))
                {
                    months -= GetMonthsInYear(year);
                    year++;
                }
                // However many months we've got left to add tells us the final month.
                month = months + 1;
            }
            else
            {
                // Pretend we were given the month at the end of the years.
                months -= GetMonthsInYear(year) - month;
                // Subtract a year at a time
                while (months + GetMonthsInYear(year) <= 0)
                {
                    months += GetMonthsInYear(year);
                    year--;
                }
                // However many months we've got left to add (which will still be negative...)
                // tells us the final month.
                month = GetMonthsInYear(year) + months;
            }

            // Convert back to calendar month
            month = CivilToCalendarMonth(year, month);
            int day = Math.Min(GetDaysInMonth(year, month), yearMonthDay.Day);
            return new YearMonthDay(year, month, day);
        }

        // Note to self: this is (minuendInstant - subtrahendInstant) in months. So if minuendInstant
        // is later than subtrahendInstant, the result should be positive.
        internal override int MonthsBetween(YearMonthDay minuendDate, YearMonthDay subtrahendDate)
        {
            // First (quite rough) guess... we could probably be more efficient than this, but it's unlikely to be very far off.
            int minuendCivilMonth = CalendarToCivilMonth(minuendDate.Year, minuendDate.Month);
            int subtrahendCivilMonth = CalendarToCivilMonth(subtrahendDate.Year, subtrahendDate.Month);
            double minuendTotalMonths = minuendCivilMonth + (minuendDate.Year * MonthsPerLeapCycle) / (double) YearsPerLeapCycle;
            double subtrahendTotalMonths = subtrahendCivilMonth + (subtrahendDate.Year * MonthsPerLeapCycle) / (double) YearsPerLeapCycle;
            int diff = (int) (minuendTotalMonths - subtrahendTotalMonths);

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
            // The civil month numbering system allows a naive comparison.
            if (monthNumbering == HebrewMonthNumbering.Civil)
            {
                return lhs.CompareTo(rhs);
            }
            // Otherwise, try one component at a time. (We could benchmark this
            // against creating a new pair of YearMonthDay values in the civil month numbering,
            // and comparing them...)
            int yearComparison = lhs.Year.CompareTo(rhs.Year);
            if (yearComparison != 0)
            {
                return yearComparison;
            }
            int lhsCivilMonth = CalendarToCivilMonth(lhs.Year, lhs.Month);
            int rhsCivilMonth = CalendarToCivilMonth(rhs.Year, rhs.Month);
            int monthComparison = lhsCivilMonth.CompareTo(rhsCivilMonth);
            if (monthComparison != 0)
            {
                return monthComparison;
            }
            return lhs.Day.CompareTo(rhs.Day);
        }
    }
}
