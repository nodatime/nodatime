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
        // This is artificially small due to wanting to only need 23 bits for the
        // "absoluate start of year" cache entry part. With a different cache mechanism
        // we could probably manage, but it's simplest just to use a restricted range.
        internal const int MaxYear = 20000;
        internal const int MinYear = 1;
        private const int ElapsedDaysCacheMask = (1 << 23) - 1; // Low 23 bits
        private const int IsHeshvanLongCacheBit = 1 << 23;
        private const int IsKislevShortCacheBit = 1 << 24;

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
                    return IsHeshvanLong(year) ? 30 : 29;
                case 9:
                    // Is Kislev short in this year?
                    return IsKislevShort(year) ?  29 : 30;
                case 12:
                    return IsLeapYear(year) ? 30 : 29;
                default:
                    return 30;
            }
        }

        private static bool IsHeshvanLong(int year)
        {
            int cache = GetOrPopulateCache(year);
            return (cache & IsHeshvanLongCacheBit) != 0;
        }

        private static bool IsKislevShort(int year)
        {
            int cache = GetOrPopulateCache(year);
            return (cache & IsKislevShortCacheBit) != 0;
        }

        // Computed ElapsedDays using the cache where possible.
        private static int ElapsedDays(int year)
        {
            int cache = GetOrPopulateCache(year);
            return cache & ElapsedDaysCacheMask;
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

        /// <summary>
        /// Returns the cached "absolute day at start of year / IsHeshvanLong / IsKislevShort" combination,
        /// populating the cache if necessary. Bits 0-22 are the "elapsed days start of year"; bit 23 is
        /// "is Heshvan long"; bit 24 is "is Kislev short". If the year is out of the range for the cache,
        /// the value is populated but not cached.
        /// </summary>
        /// <param name="year"></param>
        private static int GetOrPopulateCache(int year)
        {
            if (year < MinYear || year > MaxYear)
            {
                return ComputeCacheEntry(year);
            }
            int cacheIndex = YearStartCacheEntry.GetCacheIndex(year);
            YearStartCacheEntry cacheEntry = YearCache[cacheIndex];
            if (!cacheEntry.IsValidForYear(year))
            {
                int days = ComputeCacheEntry(year);
                cacheEntry = new YearStartCacheEntry(year, days);
                YearCache[cacheIndex] = cacheEntry;
            }
            return cacheEntry.StartOfYearDays;
        }

        /// <summary>
        /// Computes the cache entry value for the given year, but without populating the cache.
        /// </summary>
        private static int ComputeCacheEntry(int year)
        {
            int days = ElapsedDaysNoCache(year);
            // We want the elapsed days for the next year as well. Check the cache if possible.
            int nextYear = year + 1;
            int nextYearDays;
            if (nextYear <= MaxYear)
            {
                int cacheIndex = YearStartCacheEntry.GetCacheIndex(nextYear);
                YearStartCacheEntry cacheEntry = YearCache[cacheIndex];
                nextYearDays = cacheEntry.IsValidForYear(nextYear)
                    ? cacheEntry.StartOfYearDays & ElapsedDaysCacheMask
                    : ElapsedDaysNoCache(nextYear);
            }
            else
            {
                nextYearDays = ElapsedDaysNoCache(year);
            }
            int daysInYear = nextYearDays - days;
            bool isHeshvanLong = daysInYear % 10 == 5;
            bool isKislevShort = daysInYear % 10 == 3;
            return days
                | (isHeshvanLong ? IsHeshvanLongCacheBit : 0)
                | (isKislevShort ? IsKislevShortCacheBit : 0);
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