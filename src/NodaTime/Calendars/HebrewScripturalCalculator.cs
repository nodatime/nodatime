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
        // Use the bottom two bits of the day value to indicate Heshvan/Kislev.
        // Using the top bits causes issues for negative day values (only relevant for
        // invalid years, but still problematic in general).
        private const int IsHeshvanLongCacheBit = 1 << 0;
        private const int IsKislevShortCacheBit = 1 << 1;
        // Number of bits to shift the elapsed days in order to get the cache value.
        private const int ElapsedDaysCacheShift = 2;

        // Cache of when each year starts (in  terms of absolute days). This is the heart of
        // the algorithm, so just caching this is highly effective.
        // Each entry additionally encodes the length of Heshvan and Kislev. We could encode
        // more information too, but those are the tricky bits.
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
                // We could definitely do a binary search from here, but it would only
                // a few comparisons at most, and simplicity trumps optimization.
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
                return month switch
                {
                    // Note: this could be made slightly faster (at least in terms of the apparent IL) by
                    // putting all the additions of compile-time constants in one place. Indeed, we could
                    // go further by only using isLeap at most once per case. However, this code is clearer
                    // and there's no evidence that this is a bottleneck.
                    // Nisan
                    1 => 30 + heshvanLength + kislevLength + (29 + 30) + firstAdarLength + secondAdarLength,
                    // Iyar
                    2 => 30 + heshvanLength + kislevLength + (29 + 30) + firstAdarLength + secondAdarLength + 30,
                    // Sivan
                    3 => 30 + heshvanLength + kislevLength + (29 + 30) + firstAdarLength + secondAdarLength + (30 + 29),
                    // Tamuz
                    4 => 30 + heshvanLength + kislevLength + (29 + 30) + firstAdarLength + secondAdarLength + (30 + 29 + 30),
                    // Av
                    5 => 30 + heshvanLength + kislevLength + (29 + 30) + firstAdarLength + secondAdarLength + (30 + 29 + 30 + 29),
                    // Elul
                    6 => 30 + heshvanLength + kislevLength + (29 + 30) + firstAdarLength + secondAdarLength + (30 + 29 + 30 + 29 + 30),
                    // Tishri
                    7 => 0,
                    // Heshvan
                    8 => 30,
                    // Kislev
                    9 => 30 + heshvanLength,
                    // Tevet
                    10 => 30 + heshvanLength + kislevLength,
                    // Shevat
                    11 => 30 + heshvanLength + kislevLength + 29,
                    // Adar / Adar I
                    12 => 30 + heshvanLength + kislevLength + 29 + 30,
                    // Adar II
                    13 => 30 + heshvanLength + kislevLength + 29 + 30 + firstAdarLength,
                    // TODO: It would be nice for this to be simple via Preconditions
                    _ => throw new ArgumentOutOfRangeException(nameof(month), month, $"Value should be in range [1-13]")
                };
            }
        }

        internal static int DaysInMonth(int year, int month) => month switch
        {
            // FIXME: How do we express multiple cases in a switch expression?
            // We want: (2, 4, 6, 10, 13) => 29,
            2 => 29,
            4 => 29,
            6 => 29,
            10 => 29,
            13 => 29,
            8 => IsHeshvanLong(year) ? 30 : 29,
            9 => IsKislevShort(year) ? 29 : 30,
            12 => IsLeapYear(year) ? 30 : 29,
            _ => 30 // 1, 3, 5, 7, 11, 13
        };

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
            return cache >> ElapsedDaysCacheShift;
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
        /// populating the cache if necessary. Bits 2-24 are the "elapsed days start of year"; bit 0 is
        /// "is Heshvan long"; bit 1 is "is Kislev short". If the year is out of the range for the cache,
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
                    ? cacheEntry.StartOfYearDays >> ElapsedDaysCacheShift
                    : ElapsedDaysNoCache(nextYear);
            }
            else
            {
                nextYearDays = ElapsedDaysNoCache(year + 1);
            }
            int daysInYear = nextYearDays - days;
            bool isHeshvanLong = daysInYear % 10 == 5;
            bool isKislevShort = daysInYear % 10 == 3;
            return (days << ElapsedDaysCacheShift)
                | (isHeshvanLong ? IsHeshvanLongCacheBit : 0)
                | (isKislevShort ? IsKislevShortCacheBit : 0);
        }

        internal static int DaysInYear(int year) => ElapsedDays(year + 1) - ElapsedDays(year);
    }
}