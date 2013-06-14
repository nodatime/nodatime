using NodaTime.Utility;
// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Collections.Generic;

namespace NodaTime.Calendars
{
    internal abstract class YearMonthDayCalculator
    {
        private const int YearCacheSize = 1 << 10;
        private const int YearCacheMask = YearCacheSize - 1;
        private readonly YearInfo[] yearCache = new YearInfo[YearCacheSize];

        private readonly IList<Era> eras;
        internal IList<Era> Eras { get { return eras; } }

        private readonly int minYear;
        internal int MinYear { get { return minYear; } }

        private readonly int maxYear;
        internal int MaxYear { get { return maxYear; } }

        private readonly int monthsInYear;
        internal int MonthsInYear { get { return monthsInYear; } }

        private readonly long averageTicksPerYear;
        private readonly long ticksInNonLeapYear;
        private readonly long ticksInLeapYear;
        
        private readonly long ticksAtStartOfYear1;
        /// <summary>
        /// Only exposed outside the calculator for validation by tests.
        /// </summary>
        internal long TicksAtStartOfYear1 { get { return ticksAtStartOfYear1; } }

        protected YearMonthDayCalculator(int minYear, int maxYear, int monthsInYear,
            long ticksInNonLeapYear, long averageTicksPerYear, long ticksAtStartOfYear1, IList<Era> eras)
        {
            this.minYear = minYear;
            this.maxYear = maxYear;
            this.monthsInYear = monthsInYear;
            this.eras = eras;
            this.averageTicksPerYear = averageTicksPerYear;
            this.ticksAtStartOfYear1 = ticksAtStartOfYear1;
            this.ticksInNonLeapYear = ticksInNonLeapYear;
            this.ticksInLeapYear = ticksInNonLeapYear + NodaConstants.TicksPerStandardDay;
            // Effectively invalidate the first cache entry.
            // Every other cache entry will automatically be invalid,
            // by having year 0.
            yearCache[0] = new YearInfo(1, LocalInstant.LocalUnixEpoch.Ticks);
        }

        /// <summary>
        /// Returns the number of ticks from the start of the given year to the start of the given month.
        /// </summary>
        protected abstract long GetTicksFromStartOfYearToStartOfMonth(int year, int month);

        /// <summary>
        /// Compute the start of the given year in ticks. The year may be outside
        /// the bounds advertised by the calendar, but only by a single year - this is
        /// used for internal calls which sometimes need to compare a valid value with
        /// an invalid one, for estimates etc.
        /// </summary>
        protected abstract long CalculateYearTicks(int year);
        protected internal abstract int GetMonthOfYear(LocalInstant localInstant, int year);
        internal abstract int GetDaysInMonthMax(int month);
        internal abstract LocalInstant SetYear(LocalInstant localInstant, int year);
        internal abstract int GetDaysInMonth(int year, int month);
        internal abstract bool IsLeapYear(int year);

        /// <summary>
        /// Fetches the start of the year from the cache, or calculates
        /// and caches it.
        /// </summary>
        internal virtual long GetYearTicks(int year)
        {
            Preconditions.CheckArgumentRange("year", year, MinYear, MaxYear);
            lock (yearCache)
            {
                YearInfo info = yearCache[year & YearCacheMask];
                if (info.Year != year)
                {
                    info = new YearInfo(year, CalculateYearTicks(year));
                    yearCache[year & YearCacheMask] = info;
                }
                return info.StartOfYearTicks;
            }
        }

        /// <summary>
        /// Safer version of GetYearTicks which can cope with a years outside the normal range, so long as they
        /// don't actually overflow. This is useful for values which first involve estimates which might be out
        /// by a year either way.
        /// </summary>
        internal long GetYearTicksSafe(int year)
        {
            return year >= MinYear && year <= MaxYear ? GetYearTicks(year) : CalculateYearTicks(year);
        }

        /// <summary>
        /// Returns the number of ticks in the given year, based on whether or not it's a leap year.
        /// </summary>
        protected long GetTicksInYear(int year)
        {
            return IsLeapYear(year) ? ticksInLeapYear : ticksInNonLeapYear;
        }

        internal virtual int GetDayOfMonth(LocalInstant localInstant)
        {
            int year = GetYear(localInstant);
            int month = GetMonthOfYear(localInstant, year);
            return GetDayOfMonth(localInstant, year, month);
        }

        internal int GetDayOfMonth(LocalInstant localInstant, int year)
        {
            int month = GetMonthOfYear(localInstant, year);
            return GetDayOfMonth(localInstant, year, month);
        }

        internal int GetDayOfMonth(LocalInstant localInstant, int year, int month)
        {
            long dateTicks = GetYearMonthTicks(year, month);
            return (int)((localInstant.Ticks - dateTicks) / NodaConstants.TicksPerStandardDay) + 1;
        }

        internal int GetDayOfYear(LocalInstant localInstant)
        {
            return GetDayOfYear(localInstant, GetYear(localInstant));
        }

        internal int GetDayOfYear(LocalInstant localInstant, int year)
        {
            long yearStart = GetYearTicks(year);
            return (int)((localInstant.Ticks - yearStart) / NodaConstants.TicksPerStandardDay) + 1;
        }

        internal virtual int GetMonthOfYear(LocalInstant localInstant)
        {
            return GetMonthOfYear(localInstant, GetYear(localInstant));
        }

        internal virtual LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth)
        {
            Preconditions.CheckArgumentRange("year", year, MinYear, MaxYear);
            Preconditions.CheckArgumentRange("monthOfYear", monthOfYear, 1, monthsInYear);
            Preconditions.CheckArgumentRange("dayOfMonth", dayOfMonth, 1, GetDaysInMonth(year, monthOfYear));
            return new LocalInstant(GetYearMonthDayTicks(year, monthOfYear, dayOfMonth));
        }

        /// <summary>
        /// Computes the ticks of the local instant at the start of the given year/month/day.
        /// This assumes all parameters have been validated previously.
        /// </summary>
        internal long GetYearMonthDayTicks(int year, int month, int dayOfMonth)
        {
            long ticks = GetYearMonthTicks(year, month);
            return ticks + (dayOfMonth - 1) * NodaConstants.TicksPerStandardDay;
        }

        /// <summary>
        /// Returns the number of ticks (the LocalInstant, effectively) at the start of the
        /// given year/month.
        /// </summary>
        internal long GetYearMonthTicks(int year, int month)
        {
            long ticks = GetYearTicks(year);
            return ticks + GetTicksFromStartOfYearToStartOfMonth(year, month);
        }

        /// <summary>
        /// Era-based year/month/day: this implementation ignores the era, which is valid for single-era
        /// calendars, although it does validate the era first.
        /// </summary>
        internal virtual LocalInstant GetLocalInstant(Era era, int yearOfEra, int monthOfYear, int dayOfMonth)
        {
            // Just validation
            GetEraIndex(era);
            return GetLocalInstant(yearOfEra, monthOfYear, dayOfMonth);
        }

        /// <summary>
        /// Convenience method to perform nullity and validity checking on the era, converting it to
        /// the index within the list of eras used in this calendar system.
        /// </summary>
        protected int GetEraIndex(Era era)
        {
            Preconditions.CheckNotNull(era, "era");
            int index = Eras.IndexOf(era);
            Preconditions.CheckArgument(index != -1, "era", "Era is not used in this calendar");
            return index;
        }

        /// <summary>
        /// Immutable struct containing a year and the first tick of that year.
        /// This is cached to avoid it being calculated more often than is necessary.
        /// </summary>
        private struct YearInfo
        {
            private readonly int year;
            private readonly long startOfYear;

            internal YearInfo(int year, long startOfYear)
            {
                this.year = year;
                this.startOfYear = startOfYear;
            }

            internal int Year { get { return year; } }
            internal long StartOfYearTicks { get { return startOfYear; } }
        }

        internal int GetYear(LocalInstant localInstant)
        {
            long targetTicks = localInstant.Ticks;
            // Get an initial estimate of the year, and the ticks value that
            // represents the start of that year. Then verify estimate and fix if
            // necessary.

            // Initial estimate uses values divided by two to avoid overflow.
            long halfTicksPerYear = averageTicksPerYear >> 1;
            long halfTicksSinceStartOfYear1 = (targetTicks >> 1) - (ticksAtStartOfYear1 >> 1);

            if (halfTicksSinceStartOfYear1 < 0)
            {
                // When we divide, we want to round down, not towards 0.
                halfTicksSinceStartOfYear1 += 1 - halfTicksPerYear;
            }
            int candidate = (int)(halfTicksSinceStartOfYear1 / halfTicksPerYear) + 1;

            // Most of the time we'll get the right year straight away, and we'll almost
            // always get it after one adjustment - but it's safer (and easier to think about)
            // if we just keep going until we know we're right.
            while (true)
            {
                long candidateStart = GetYearTicksSafe(candidate);
                long ticksFromCandidateStartToTarget = targetTicks - candidateStart;
                if (ticksFromCandidateStartToTarget < 0)
                {
                    // Our candidate year is later than we want.
                    candidate--;
                    continue;
                }
                long candidateLength = GetTicksInYear(candidate);
                if (ticksFromCandidateStartToTarget >= candidateLength)
                {
                    // Out candidate year is earlier than we want.
                    candidate++;
                    continue;
                }
                return candidate;
            }
        }

        /// <summary>
        /// Returns the year-of-era for the given local instant. The base implementation is to return the plain
        /// year, which is suitable for single-era calendars.
        /// </summary>
        internal virtual int GetYearOfEra(LocalInstant localInstant)
        {
            return GetYear(localInstant);
        }

        /// <summary>
        /// Handling for century-of-era where (say) year 123 is in century 2... but so is year 200.
        /// </summary>
        internal virtual int GetCenturyOfEra(LocalInstant localInstant)
        {
            int yearOfEra = GetYearOfEra(localInstant);
            int zeroBasedRemainder = yearOfEra % 100;
            int zeroBasedResult = yearOfEra / 100;
            return zeroBasedRemainder == 0 ? zeroBasedResult : zeroBasedResult + 1;
        }

        /// <summary>
        /// Handling for year-of-century in the range [1, 100].
        /// </summary>
        internal virtual int GetYearOfCentury(LocalInstant localInstant)
        {
            int yearOfEra = GetYearOfEra(localInstant);
            int zeroBased = yearOfEra % 100;
            return zeroBased == 0 ? 100 : zeroBased;
        }

        /// <summary>
        /// Returns the era for the given local instant. The base implementation is to return 0, which is
        /// suitable for single-era calendars.
        /// </summary>
        internal virtual int GetEra(LocalInstant localInstant)
        {
            return 0;
        }

        internal virtual int GetDaysInYear(int year)
        {
            return IsLeapYear(year) ? 366 : 365;
        }

        internal virtual LocalInstant AddMonths(LocalInstant localInstant, int months)
        {
            if (months == 0)
            {
                return localInstant;
            }
            // Save the time part first
            long timePart = TimeOfDayCalculator.GetTickOfDay(localInstant);
            // Get the year and month
            int thisYear = GetYear(localInstant);
            int thisMonth = GetMonthOfYear(localInstant, thisYear);

            // Do not refactor without careful consideration.
            // Order of calculation is important.

            int yearToUse;
            // Initially, monthToUse is zero-based
            int monthToUse = thisMonth - 1 + months;
            if (monthToUse >= 0)
            {
                yearToUse = thisYear + (monthToUse / monthsInYear);
                monthToUse = (monthToUse % monthsInYear) + 1;
            }
            else
            {
                yearToUse = thisYear + (monthToUse / monthsInYear) - 1;
                monthToUse = Math.Abs(monthToUse);
                int remMonthToUse = monthToUse % monthsInYear;
                // Take care of the boundary condition
                if (remMonthToUse == 0)
                {
                    remMonthToUse = monthsInYear;
                }
                monthToUse = monthsInYear - remMonthToUse + 1;
                // Take care of the boundary condition
                if (monthToUse == 1)
                {
                    yearToUse++;
                }
            }
            // End of do not refactor.

            // Quietly force DOM to nearest sane value.
            int dayToUse = GetDayOfMonth(localInstant, thisYear, thisMonth);
            int maxDay = GetDaysInMonth(yearToUse, monthToUse);
            dayToUse = Math.Min(dayToUse, maxDay);
            // Get proper date part, and return result
            long datePart = GetYearMonthDayTicks(yearToUse, monthToUse, dayToUse);
            return new LocalInstant(datePart + timePart);
        }

        /// <summary>
        /// Default implementation of GetAbsoluteYear which assumes a single era.
        /// </summary>
        internal virtual int GetAbsoluteYear(int yearOfEra, int eraIndex)
        {
            if (yearOfEra < 1 || yearOfEra > MaxYear)
            {
                throw new ArgumentOutOfRangeException("yearOfEra");
            }
            return yearOfEra;
        }

        /// <summary>
        /// See <see cref="CalendarSystem.GetMinYearOfEra(NodaTime.Calendars.Era)" /> - but this uses a pre-validated index.
        /// This default implementation returns 1, but can be overridden by derived classes.
        /// </summary>
        internal virtual int GetMinYearOfEra(int eraIndex)
        {
            return 1;
        }

        /// <summary>
        /// See <see cref="CalendarSystem.GetMaxYearOfEra(Era)"/> - but this uses a pre-validated index.
        /// This default implementation returns the maximum year for this calendar, which is
        /// a valid implementation for single-era calendars.
        /// </summary>
        internal virtual int GetMaxYearOfEra(int eraIndex)
        {
            return MaxYear;
        }
    }
}
