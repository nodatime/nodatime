// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using NodaTime.Annotations;
using NodaTime.Utility;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Implementation of the Um Al Qura calendar, using the tabular data in the BCL. This is fetched
    /// on construction and cached - we just need to know the length of each month of each year, which is
    /// cheap as the current implementation only covers 183 years.
    /// </summary>
    internal sealed class UmAlQuraYearMonthDayCalculator : RegularYearMonthDayCalculator
    {
        private const int AverageDaysPer10Years = 3544;
        private static readonly int ComputedMinYear;
        private static readonly int ComputedMaxYear;
        private static readonly int ComputedDaysAtStartOfYear1;
        private static readonly int ComputedDaysAtStartOfMinYear;

        // Precomputed values for lengths of year and lengths of months. This is actually 4 times bigger than we
        // need it to be - we only need 13 bits per year, so could have a single short[] cache. However, the code
        // is simpler this way, and it's still only going to be about 1.5K, assuming the BCL still supports ~183 years.
        private static readonly int[] YearLengths;
        private static readonly int[] MonthLengths;
        // Number of days from ComputedMinYear on a per year basis.
        private static readonly int[] YearStartDays;

        static UmAlQuraYearMonthDayCalculator()
        {
            // Try to initialize. If anything fails, YearLengths will still be null, so IsSupported will return false.
            Calendar bclCalendar;
#if PCL
            // Can't refer to the BCL calendar by name, but it *might* be available anyway. Let's try to instantiate
            // it with reflection. If we can't, that's fair enough.
            try
            {
                var type = typeof(Calendar).Assembly.GetType("System.Globalization.UmAlQuraCalendar");
                if (type == null)
                {
                    return;
                }
                bclCalendar = (Calendar) Activator.CreateInstance(type);
            }
            catch
            {
                // Don't really care what went wrong here. We'll assume it's not supported.
                return;
            }
#else
            bclCalendar = new UmAlQuraCalendar();
#endif

            DateTime minDateTime = bclCalendar.MinSupportedDateTime;

            // Check if this looks like a sensible implementation, with a limited time range.
            // (The .NET implementation only runs from 1900-2077, for example.)
            // Mono is unfortunately broken - it advertises a large range, but then is inconsistent:
            // Year 2 (for example) either has 354 or 355 days depending on how you ask.
            if (minDateTime.Year < 1800 || minDateTime.Year > 3000)
            {
                YearLengths = null;
                MonthLengths = null;
                YearStartDays = null;
                return;
            }

            // Work out the min and max supported years, ensuring that we support complete years.
            ComputedMinYear = bclCalendar.GetYear(minDateTime);
            if (bclCalendar.GetMonth(minDateTime) != 1 || bclCalendar.GetDayOfMonth(minDateTime) != 1)
            {
                ComputedMinYear++;
            }

            DateTime maxDateTime = bclCalendar.MaxSupportedDateTime;
            ComputedMaxYear = bclCalendar.GetYear(maxDateTime);
            if (bclCalendar.GetMonth(maxDateTime) != 12 || bclCalendar.GetDayOfMonth(maxDateTime) != bclCalendar.GetDaysInMonth(ComputedMaxYear, 12))
            {
                ComputedMaxYear--;
            }

            // For year lengths, we need to handle 1 year beyond the boundaries, too.
            // We don't need MonthLengths to be quite as long, but it's simpler to be consistent.
            YearLengths = new int[ComputedMaxYear - ComputedMinYear + 3];
            YearStartDays = new int[ComputedMaxYear - ComputedMinYear + 3];
            MonthLengths = new int[ComputedMaxYear - ComputedMinYear + 3];
            int totalDays = 0;
            for (int year = ComputedMinYear; year <= ComputedMaxYear; year++)
            {
                int yearIndex = year - ComputedMinYear + 1;
                YearLengths[yearIndex] = bclCalendar.GetDaysInYear(year);
                YearStartDays[yearIndex] = totalDays;
                totalDays += YearLengths[yearIndex];
                int monthBits = 0;
                for (int month = 1; month <= 12; month++)
                {
                    if (bclCalendar.GetDaysInMonth(year, month) == 30)
                    {
                        monthBits |= 1 << month;
                    }
                }
                MonthLengths[yearIndex] = monthBits;
            }
            // Fill in the cache with dummy data for before/after the min/max year, pretending
            // that both of the "extra" years were 354 days long.
            YearStartDays[0] = -354;
            YearStartDays[YearStartDays.Length - 1] = totalDays;
            YearLengths[0] = 354;
            YearLengths[YearStartDays.Length - 1] = 354;

            // Assume every 10 years before minDateTime has exactly 3544 days... it doesn't matter whether or not that's
            // correct, but it gets roughly the right estimate. It doesn't matter that startOfMinYear isn't in UTC; we're only
            // taking the Ticks property, which doesn't take account of the Kind.
            DateTime startOfMinYear = bclCalendar.ToDateTime(ComputedMinYear, 1, 1, 0, 0, 0, 0);
            ComputedDaysAtStartOfMinYear = (int) ((startOfMinYear.Ticks - NodaConstants.BclTicksAtUnixEpoch) / NodaConstants.TicksPerStandardDay);
            ComputedDaysAtStartOfYear1 = ComputedDaysAtStartOfMinYear + (int) (((1 - ComputedMinYear) / 10.0) * AverageDaysPer10Years);
        }

        /// <summary>
        /// Checks whether the calendar is supported on this execution environment. We don't currently support the PCL
        /// or Mono.
        /// </summary>
        internal static bool IsSupported { get { return YearLengths != null; } }

        internal UmAlQuraYearMonthDayCalculator()
            : base(ComputedMinYear, ComputedMaxYear, 12, AverageDaysPer10Years, ComputedDaysAtStartOfYear1)
        {
            if (!IsSupported)
            {
                throw new InvalidOperationException("The Um Al Qura calendar is not supported on this platform");
            }
        }

        // No need to use the YearMonthDayCalculator cache, given that we've got the value in array already.
        internal override int GetStartOfYearInDays(int year)
        {
            return CalculateStartOfYearDays(year);
        }

        protected override int CalculateStartOfYearDays([Trusted] int year)
        {
            // Fine for one year either side of min/max.
            return YearStartDays[year - ComputedMinYear + 1] + ComputedDaysAtStartOfMinYear;
        }

        protected override int GetDaysFromStartOfYearToStartOfMonth([Trusted] int year, [Trusted] int month)
        {
            // TODO(2.0): Use clever bit shifting to count the number of bits.
            int monthBits = MonthLengths[year - ComputedMinYear + 1];
            int extraDays = 0;
            for (int i = 1; i < month; i++)
            {
                extraDays += (monthBits >> i) & 1;
            }
            return (month - 1) * 29 + extraDays;
        }

        internal override int GetDaysInMonth([Trusted] int year, int month)
        {
            int monthBits = MonthLengths[year - ComputedMinYear + 1];
            return 29 + ((monthBits >> month) & 1);
        }

        internal override int GetDaysInYear([Trusted] int year)
        {
            // Fine for one year either side of min/max.
            return YearLengths[year - ComputedMinYear + 1];
        }

        internal override YearMonthDay GetYearMonthDay([Trusted] int year, [Trusted] int dayOfYear)
        {
            int daysLeft = dayOfYear;
            int monthBits = MonthLengths[year - ComputedMinYear + 1];
            for (int month = 1; month <= 12; month++)
            {
                int monthLength = 29 + ((monthBits >> month) & 1);
                if (daysLeft <= monthLength)
                {
                    return new YearMonthDay(year, month, daysLeft);
                }
                daysLeft -= monthLength;
            }
            // This should throw...
            Preconditions.CheckArgumentRange("dayOfYear", dayOfYear, 1, GetDaysInYear(year));
            throw new InvalidOperationException("Bug in Noda Time: year " + year +
                " has " + GetDaysInYear(year) + " days but " + dayOfYear + " isn't valid");
        }

        internal override bool IsLeapYear([Trusted] int year)
        {
            return YearLengths[year - ComputedMinYear + 1] == 355;
        }
    }
}
