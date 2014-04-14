// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;

namespace NodaTime.Calendars
{
    internal abstract class YearMonthDayCalculator
    {
        /// <summary>
        /// Cache to speed up working out when a particular year starts.
        /// See the <see cref="YearStartCacheEntry"/> documentation and <see cref="GetStartOfYearInDays"/>
        /// for more details.
        /// </summary>
        private readonly YearStartCacheEntry[] yearCache = YearStartCacheEntry.CreateCache();

        /// <summary>
        /// Array of eras in this calculator; this is never mutated.
        /// </summary>
        private readonly Era[] eras;
        internal Era[] Eras { get { return eras; } }

        private readonly int minYear;
        internal int MinYear { get { return minYear; } }

        private readonly int maxYear;
        internal int MaxYear { get { return maxYear; } }

        private readonly long averageTicksPerYear;
        
        private readonly long ticksAtStartOfYear1;
        /// <summary>
        /// Only exposed outside the calculator for validation by tests.
        /// </summary>
        internal long TicksAtStartOfYear1 { get { return ticksAtStartOfYear1; } }

        protected YearMonthDayCalculator(int minYear, int maxYear,
            long averageTicksPerYear, long ticksAtStartOfYear1, Era[] eras)
        {
            // We should really check the minimum year as well, but constructing it hurts my brain.
            Preconditions.CheckArgument(maxYear < YearStartCacheEntry.InvalidEntryYear, "maxYear",
                "Calendar year range would invalidate caching.");
            this.minYear = minYear;
            this.maxYear = maxYear;
            this.eras = Preconditions.CheckNotNull(eras, "eras");
            this.averageTicksPerYear = averageTicksPerYear;
            this.ticksAtStartOfYear1 = ticksAtStartOfYear1;
        }

        /// <summary>
        /// Returns the number of ticks from the start of the given year to the start of the given month.
        /// </summary>
        protected abstract long GetTicksFromStartOfYearToStartOfMonth(int year, int month);

        /// <summary>
        /// Compute the start of the given year in days since 1970-01-01 ISO. The year may be outside
        /// the bounds advertised by the calendar, but only by a single year - this is
        /// used for internal calls which sometimes need to compare a valid value with
        /// an invalid one, for estimates etc.
        /// </summary>
        protected abstract int CalculateStartOfYearDays(int year);
        protected abstract int GetMonthOfYear(LocalInstant localInstant, int year);
        protected abstract long GetTicksInYear(int year);
        internal abstract int GetMaxMonth(int year);
        internal abstract LocalInstant SetYear(LocalInstant localInstant, int year);
        internal abstract int GetDaysInMonth(int year, int month);
        internal abstract bool IsLeapYear(int year);
        internal abstract LocalInstant AddMonths(LocalInstant localInstant, int months);
        /// <summary>
        /// Subtract subtrahendInstant from minuendInstant, in terms of months.
        /// </summary>
        internal abstract int MonthsBetween(LocalInstant minuendInstant, LocalInstant subtrahendInstant);

        /// <summary>
        /// Returns the number of ticks since the Unix epoch at the start of the given year.
        /// This is virtual to allow GregorianCalendarSystem to override it for an ultra-efficient
        /// cache for modern years. This method can cope with a value for <paramref name="year"/> outside
        /// the normal range, so long as the resulting computation doesn't overflow. (Min and max years
        /// are therefore chosen to be slightly more restrictive than we would otherwise need, for the
        /// sake of simplicity.) This is useful for values which first involve estimates which might be out
        /// by a year either way.
        /// </summary>
        internal virtual long GetStartOfYearInTicks(int year)
        {
            return unchecked(GetStartOfYearInDays(year) * NodaConstants.TicksPerStandardDay);
        }

        internal virtual int GetDayOfMonth(LocalInstant localInstant)
        {
            int year = GetYear(localInstant);
            int month = GetMonthOfYear(localInstant, year);
            return GetDayOfMonth(localInstant, year, month);
        }

        protected int GetDayOfMonth(LocalInstant localInstant, int year, int month)
        {
            long dateTicks = GetYearMonthTicks(year, month);
            unchecked
            {
                long ticksWithinMonth = localInstant.Ticks - dateTicks;
                return TickArithmetic.TicksToDays(ticksWithinMonth) + 1;
            }
        }

        internal int GetDayOfYear(LocalInstant localInstant)
        {
            return GetDayOfYear(localInstant, GetYear(localInstant));
        }

        internal int GetDayOfYear(LocalInstant localInstant, int year)
        {
            long yearStart = GetStartOfYearInTicks(year);
            unchecked
            {
                long ticksWithinYear = localInstant.Ticks - yearStart;
                return TickArithmetic.TicksToDays(ticksWithinYear) + 1;
            }
        }

        internal virtual int GetMonthOfYear(LocalInstant localInstant)
        {
            return GetMonthOfYear(localInstant, GetYear(localInstant));
        }

        internal virtual LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth)
        {
            Preconditions.CheckArgumentRange("year", year, MinYear, MaxYear);
            Preconditions.CheckArgumentRange("monthOfYear", monthOfYear, 1, GetMaxMonth(year));
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
            long ticks = GetStartOfYearInTicks(year);
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
            int index = Array.IndexOf(Eras, era);
            Preconditions.CheckArgument(index != -1, "era", "Era is not used in this calendar");
            return index;
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
                long candidateStart = GetStartOfYearInTicks(candidate);
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
                    // Our candidate year is earlier than we want.
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

        /// <summary>
        /// Fetches the start of the year (in days since 1970-01-01 ISO) from the cache, or calculates
        /// and caches it.
        /// </summary>
        protected int GetStartOfYearInDays(int year)
        {
            if (year < MinYear || year > MaxYear)
            {
                return CalculateStartOfYearDays(year);
            }
            int cacheIndex = YearStartCacheEntry.GetCacheIndex(year);
            YearStartCacheEntry cacheEntry = yearCache[cacheIndex];
            if (!cacheEntry.IsValidForYear(year))
            {
                int days = CalculateStartOfYearDays(year);
                cacheEntry = new YearStartCacheEntry(year, days);
                yearCache[cacheIndex] = cacheEntry;
            }
            return cacheEntry.StartOfYearDays;
        }
    }
}
