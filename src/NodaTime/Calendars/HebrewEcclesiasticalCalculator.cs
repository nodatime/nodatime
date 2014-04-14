// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Calendars
{
    /// <summary>
    /// Implementation of the algorithms described in
    /// http://www.cs.tau.ac.il/~nachum/calendar-book/papers/calendar.ps, using ecclesiastical
    /// month numbering.
    /// </summary>
    internal static class HebrewEcclesiasticalCalculator
    {
        // Cache of when each year starts (in  terms of absolute days). This is the heart of
        // the algorithm, so just caching this is highly effective.
        // TODO: Encode the information about month lengths in the cache too. We have plenty of
        // space as we don't need day numbers to go terribly high.
        private static readonly YearStartCacheEntry[] YearCache = YearStartCacheEntry.CreateCache();

        internal static bool IsLeapYear(int year)
        {
            return ((year * 7) + 1) % 19 < 7;
        }

        private static int MonthsInYear(int year)
        {
            return IsLeapYear(year) ? 13 : 12;
        }

        internal static int DaysInMonth(int year, int month)
        {
            switch (month)
            {
                case 2:
                case 4:
                case 6:
                case 10:
                case 13:
                    return 29;
                case 8:
                    // Is Heshvan long in this year?
                    return DaysInYear(year) % 10 == 5 ? 30 : 29;
                case 9:
                    // Is Kislev short in this year?
                    return DaysInYear(year) % 10 == 3 ? 29 : 30;
                case 12:
                    return IsLeapYear(year) ? 30 : 29;
                default:
                    return 30;
            }
        }

        // Computed ElapsedDays using the cahce where possible.
        private static int ElapsedDays(int year)
        {
            if (year < 1 || year > 30000)
            {
                return ElapsedDaysNoCache(year);
            }
            int cacheIndex = YearStartCacheEntry.GetCacheIndex(year);
            YearStartCacheEntry cacheEntry = YearCache[cacheIndex];
            if (!cacheEntry.IsValidForYear(year))
            {
                int days = ElapsedDaysNoCache(year);
                cacheEntry = new YearStartCacheEntry(year, days);
                YearCache[cacheIndex] = cacheEntry;
            }
            return cacheEntry.StartOfYearDays;
        }

        private static int ElapsedDaysNoCache(int year)
        {
            int monthsElapsed = (235 * ((year - 1) / 19)) // Months in complete cycles so far
                                + (12 * ((year - 1) % 19)) // Regular months in this cycle
                                + ((((year - 1) % 19) * 7 + 1) / 19); // Leap months this cycle
            // Second option in the paper, which keeps values smaller
            int partsElapsed = 204 + (793 * (monthsElapsed % 1080));
            int hoursElapsed = 5 + (12 * monthsElapsed) + (793 * (monthsElapsed / 1080)) + (partsElapsed / 1080);
            int day = 1 + (29 * monthsElapsed) + (hoursElapsed / 24);
            int parts = ((hoursElapsed % 24) * 1080) + (partsElapsed % 1080);
            bool postponeRoshHaShanah = (parts >= 19440) ||
                                        (day % 7 == 2 && parts >= 9924 && !IsLeapYear(year)) ||
                                        (day % 7 == 1 && parts >= 16789 && IsLeapYear(year - 1));
            int alternativeDay = postponeRoshHaShanah ? 1 + day : day;
            int alternativeDayMod7 = alternativeDay % 7;
            return (alternativeDayMod7 == 0 || alternativeDayMod7 == 3 || alternativeDayMod7 == 5)
                ? alternativeDay + 1 : alternativeDay;
        }

        internal static int DaysInYear(int year)
        {
            return ElapsedDays(year + 1) - ElapsedDays(year);
        }

        /// <summary>
        /// Returns the "absolute day number" for the given year, month and day in the Hebrew calendar.
        /// The absolute day number of 0001-01-01 AD (Gregorian) is 1.
        /// </summary>            
        internal static int AbsoluteFromHebrew(int year, int month, int day)
        {
            // Easy bits:
            // - Start of the year (in days since epoch)
            // - Day of month
            // - Constant (1AD epoch vs Hebrew epoch)
            int days = ElapsedDays(year) + day - 1373429;

            // Now work out the days leading up to this month.
            // TODO: use the cache to compute all of this in one go.
            if (month < 7) // If before Tishri
            {
                // Add days in prior months this year before and after Nisan
                int monthsInYear = MonthsInYear(year);
                for (int m = 7; m <= monthsInYear; m++)
                {
                    days += DaysInMonth(year, m);
                }
                for (int m = 1; m < month; m++)
                {
                    days += DaysInMonth(year, m);
                }
            }
            else
            {
                for (int m = 7; m < month; m++)
                {
                    days += DaysInMonth(year, m);
                }
            }
            return days;
        }

        /// <summary>
        /// Converts an "absolute day number" into a year, month and day in the Hebrew calendar.
        /// The absolute day number of 0001-01-01 AD (Gregorian) is 1.
        /// </summary>            
        internal static YearMonthDay HebrewFromAbsolute(int days)
        {
            // Initial guess (lower bound).
            // TODO: See whether we can use a higher estimate (divide by 363.4) which should require
            // fewer iterations.
            int year = (days + 1373429) / 366;
            while (days >= AbsoluteFromHebrew(year + 1, 7, 1))
            {
                year++;
            }
            // Start month search at either Tishri or Nisan
            int month = days < AbsoluteFromHebrew(year, 1, 1) ? 7 : 1;
            while (days > AbsoluteFromHebrew(year, month, DaysInMonth(year, month)))
            {
                month++;
            }
            int day = days - (AbsoluteFromHebrew(year, month, 1) - 1);
            return new YearMonthDay(year, month, day);
        }
    }
}