// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Implementation of the algorithms described in
    /// http://www.cs.tau.ac.il/~nachum/calendar-book/papers/calendar.ps, using scriptural
    /// month numbering.
    /// </summary>
    internal static class HebrewScripturalCalculator
    {
        internal const int MaxYear = 9999;
        internal const int MinYear = 1;
        private const int ElapsedDaysCacheMask = (1 << 23) - 1; // Low 23 bits
        private const int IsHeshvanLongCacheBit = 1 << 23;
        private const int IsKislevShortCacheBit = 1 << 24;

        // Cache of when each year starts (in  terms of absolute days). This is the heart of
        // the algorithm, so just caching this is highly effective.
        // TODO: Encode the information about month lengths in the cache too. We have plenty of
        // space as we don't need day numbers to go terribly high.
        private static readonly YearStartCacheEntry[] YearCache = YearStartCacheEntry.CreateCache();

        internal static bool IsLeapYear(int year) => ((year * 7) + 1) % 19 < 7;

        internal static YearMonthDay GetYearMonthDay(int year, int dayOfYear)
        {
            unchecked
            {
                // Work out everything about the year in one go.
                int cache = GetOrPopulateCache(year);
                int heshvanLength = (cache & IsHeshvanLongCacheBit) != 0 ? 30 : 29;
                int kislevLength = (cache & IsKislevShortCacheBit) != 0 ? 29 : 30;
                bool isLeap = IsLeapYear(year);
                int firstAdarLength = isLeap ? 30 : 29;

                if (dayOfYear < 31)
                {
                    // Tishri
                    return new YearMonthDay(year, 7, dayOfYear);
                }
                if (dayOfYear < 31 + heshvanLength)
                {
                    // Heshvan
                    return new YearMonthDay(year, 8, dayOfYear - 30);
                }
                // Now "day of year without Heshvan"...
                dayOfYear -= heshvanLength;
                if (dayOfYear < 31 + kislevLength)
                {
                    // Kislev
                    return new YearMonthDay(year, 9, dayOfYear - 30);
                }
                // Now "day of year without Heshvan or Kislev"...
                dayOfYear -= kislevLength;
                if (dayOfYear < 31 + 29)
                {
                    // Tevet
                    return new YearMonthDay(year, 10, dayOfYear - 30);
                }
                if (dayOfYear < 31 + 29 + 30)
                {
                    // Shevat
                    return new YearMonthDay(year, 11, dayOfYear - (30 + 29));
                }
                if (dayOfYear < 31 + 29 + 30 + firstAdarLength)
                {
                    // Adar / Adar I
                    return new YearMonthDay(year, 12, dayOfYear - (30 + 29 + 30));
                }
                // Now "day of year without first month of Adar"
                dayOfYear -= firstAdarLength;
                if (isLeap)
                {
                    if (dayOfYear < 31 + 29 + 30 + 29)
                    {
                        return new YearMonthDay(year, 13, dayOfYear - (30 + 29 + 30));
                    }
                    // Now "day of year without any Adar"
                    dayOfYear -= 29;
                }
                // TODO(2.0): We could definitely do a binary search from here...
                if (dayOfYear < 31 + 29 + 30 + 30)
                {
                    // Nisan
                    return new YearMonthDay(year, 1, dayOfYear - (30 + 29 + 30));
                }
                if (dayOfYear < 31 + 29 + 30 + 30 + 29)
                {
                    // Iyar
                    return new YearMonthDay(year, 2, dayOfYear - (30 + 29 + 30 + 30));
                }
                if (dayOfYear < 31 + 29 + 30 + 30 + 29 + 30)
                {
                    // Sivan
                    return new YearMonthDay(year, 3, dayOfYear - (30 + 29 + 30 + 30 + 29));
                }
                if (dayOfYear < 31 + 29 + 30 + 30 + 29 + 30 + 29)
                {
                    // Tamuz
                    return new YearMonthDay(year, 4, dayOfYear - (30 + 29 + 30 + 30 + 29 + 30));
                }
                if (dayOfYear < 31 + 29 + 30 + 30 + 29 + 30 + 29 + 30)
                {
                    // Av
                    return new YearMonthDay(year, 5, dayOfYear - (30 + 29 + 30 + 30 + 29 + 30 + 29));
                }
                // Elul
                return new YearMonthDay(year, 6, dayOfYear - (30 + 29 + 30 + 30 + 29 + 30 + 29 + 30));
            }
        }

        internal static int GetDaysFromStartOfYearToStartOfMonth(int year, int month)
        {
            // Work out everything about the year in one go. (Admittedly we don't always need it all... but for
            // anything other than Tishri and Heshvan, we at least need the length of Heshvan...)
            unchecked
            {
                int cache = GetOrPopulateCache(year);
                int heshvanLength = (cache & IsHeshvanLongCacheBit) != 0 ? 30 : 29;
                int kislevLength = (cache & IsKislevShortCacheBit) != 0 ? 29 : 30;
                bool isLeap = IsLeapYear(year);
                int firstAdarLength = isLeap ? 30 : 29;
                int secondAdarLength = isLeap ? 29 : 0;
                switch (month)
                {
                    // TODO(2.0): Check whether the constant addition is optimized here.
                    case 1: // Nisan
                        return 30 + heshvanLength + kislevLength + (29 + 30) + firstAdarLength + secondAdarLength;
                    case 2: // Iyar
                        return 30 + heshvanLength + kislevLength + (29 + 30) + firstAdarLength + secondAdarLength + 30;
                    case 3: // Sivan
                        return 30 + heshvanLength + kislevLength + (29 + 30) + firstAdarLength + secondAdarLength + (30 + 29);
                    case 4: // Tamuz
                        return 30 + heshvanLength + kislevLength + (29 + 30) + firstAdarLength + secondAdarLength + (30 + 29 + 30);
                    case 5: // Av
                        return 30 + heshvanLength + kislevLength + (29 + 30) + firstAdarLength + secondAdarLength + (30 + 29 + 30 + 29);
                    case 6: // Elul
                        return 30 + heshvanLength + kislevLength + (29 + 30) + firstAdarLength + secondAdarLength + (30 + 29 + 30 + 29 + 30);
                    case 7: // Tishri
                        return 0;
                    case 8: // Heshvan
                        return 30;
                    case 9: // Kislev
                        return 30 + heshvanLength;
                    case 10: // Tevet
                        return 30 + heshvanLength + kislevLength;
                    case 11: // Shevat
                        return 30 + heshvanLength + kislevLength + 29;
                    case 12: // Adar / Adar I
                        return 30 + heshvanLength + kislevLength + 29 + 30;
                    case 13: // Adar II
                        return 30 + heshvanLength + kislevLength + 29 + 30 + firstAdarLength;
                    default:
                        // Just shorthand for using the right exception across PCL and desktop
                        Preconditions.CheckArgumentRange(nameof(month), month, 1, 13);
                        throw new InvalidOperationException("CheckArgumentRange should have thrown...");
                }
            }
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
                // 1, 3, 5, 7, 11, 13
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

        /// <summary>
        /// Elapsed days since the Hebrew epoch at the start of the given Hebrew year.
        /// This is *inclusive* of the first day of the year, so ElapsedDays(1) returns 1.
        /// </summary>
        internal static int ElapsedDays(int year)
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
        /// Returns the cached "elapsed day at start of year / IsHeshvanLong / IsKislevShort" combination,
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
                nextYearDays = ElapsedDaysNoCache(year + 1);
            }
            int daysInYear = nextYearDays - days;
            bool isHeshvanLong = daysInYear % 10 == 5;
            bool isKislevShort = daysInYear % 10 == 3;
            return days
                | (isHeshvanLong ? IsHeshvanLongCacheBit : 0)
                | (isKislevShort ? IsKislevShortCacheBit : 0);
        }

        internal static int DaysInYear(int year) => ElapsedDays(year + 1) - ElapsedDays(year);
    }
}