// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using NodaTime.Annotations;
using NodaTime.Utility;

namespace NodaTime.Calendars
{
    internal abstract class YearMonthDayCalculator : IComparer<YearMonthDay>
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

        private readonly int averageDaysPer10Years;

        private readonly int daysAtStartOfYear1;
        /// <summary>
        /// Only exposed outside the calculator for validation by tests.
        /// </summary>
        internal int DaysAtStartOfYear1 { get { return daysAtStartOfYear1; } }

        protected YearMonthDayCalculator(int minYear, int maxYear,
            int averageDaysPer10Years, int daysAtStartOfYear1, Era[] eras)
        {
            // We should really check the minimum year as well, but constructing it hurts my brain.
            Preconditions.CheckArgument(maxYear < YearStartCacheEntry.InvalidEntryYear, "maxYear",
                "Calendar year range would invalidate caching.");
            this.minYear = minYear;
            this.maxYear = maxYear;
            this.eras = Preconditions.CheckNotNull(eras, "eras");
            this.averageDaysPer10Years = averageDaysPer10Years;
            this.daysAtStartOfYear1 = daysAtStartOfYear1;
        }

        /// <summary>
        /// Compares two YearMonthDay values according to the rules of this calendar.
        /// The default implementation simply uses a naive comparison of the values,
        /// as this is suitable for most calendars.
        /// </summary>
        public virtual int Compare(YearMonthDay lhs, YearMonthDay rhs)
        {
            return lhs.CompareTo(rhs);
        }

        /// <summary>
        /// Returns the number of days from the start of the given year to the start of the given month.
        /// </summary>
        protected abstract int GetDaysFromStartOfYearToStartOfMonth(int year, int month);

        /// <summary>
        /// Compute the start of the given year in days since 1970-01-01 ISO. The year may be outside
        /// the bounds advertised by the calendar, but only by a single year - this is
        /// used for internal calls which sometimes need to compare a valid value with
        /// an invalid one, for estimates etc.
        /// </summary>
        protected abstract int CalculateStartOfYearDays(int year);
        internal abstract int GetMaxMonth(int year);
        internal abstract int GetDaysInMonth(int year, int month);
        internal abstract bool IsLeapYear(int year);
        internal abstract YearMonthDay AddMonths(YearMonthDay yearMonthDay, int months);
        internal abstract YearMonthDay GetYearMonthDay(int daysSinceEpoch);
        /// <summary>
        /// Subtract subtrahendDate from minuendDate, in terms of months.
        /// </summary>
        internal abstract int MonthsBetween(YearMonthDay minuendDate, YearMonthDay subtrahendDate);

        /// <summary>
        /// Adjusts the given YearMonthDay to the specified year, potentially adjusting
        /// other fields as required.
        /// </summary>
        internal abstract YearMonthDay SetYear(YearMonthDay yearMonthDay, int year);

        /// <summary>
        /// Converts from a YearMonthDay representation to "day of year".
        /// This assumes the parameter have been validated previously.
        /// </summary>
        internal int GetDayOfYear(YearMonthDay yearMonthDay)
        {
            int daysSinceEpoch = GetDaysSinceEpoch(yearMonthDay);
            int yearStart = GetStartOfYearInDays(yearMonthDay.Year);
            return daysSinceEpoch - yearStart + 1;
        }

        /// <summary>
        /// Computes the days since the Unix epoch at the start of the given year/month/day.
        /// This is the opposite of <see cref="GetYearMonthDay"/>.
        /// This assumes the parameter have been validated previously.
        /// </summary>
        internal virtual int GetDaysSinceEpoch(YearMonthDay yearMonthDay)
        {
            int year = yearMonthDay.Year;
            int startOfYear = GetStartOfYearInDays(year);
            int startOfMonth = startOfYear + GetDaysFromStartOfYearToStartOfMonth(year, yearMonthDay.Month);
            return startOfMonth + yearMonthDay.Day - 1;
        }

        /// <summary>
        /// Work out the year from the number of days since the epoch, as well as the
        /// day of that year (0-based).
        /// </summary>
        [VisibleForTesting] // Would be protected otherwise.
        internal int GetYear(int daysSinceEpoch, out int zeroBasedDayOfYear)
        {
            // Get an initial estimate of the year, and the days-since-epoch value that
            // represents the start of that year. Then verify estimate and fix if
            // necessary. We have the average days per 100 years to avoid getting bad candidates
            // pretty quickly.
            int daysSinceYear1 = daysSinceEpoch - daysAtStartOfYear1;
            int candidate = ((daysSinceYear1 * 10) / averageDaysPer10Years) + 1;

            // Most of the time we'll get the right year straight away, and we'll almost
            // always get it after one adjustment - but it's safer (and easier to think about)
            // if we just keep going until we know we're right.
            int candidateStart = GetStartOfYearInDays(candidate);
            int daysFromCandidateStartToTarget = daysSinceEpoch - candidateStart;
            if (daysFromCandidateStartToTarget < 0)
            {
                // Our candidate year is later than we want. Keep going backwards until we've got
                // a non-negative result, which must then be correct.
                do
                {
                    candidate--;
                    daysFromCandidateStartToTarget += GetDaysInYear(candidate);
                }
                while (daysFromCandidateStartToTarget < 0);
                zeroBasedDayOfYear = daysFromCandidateStartToTarget;
                return candidate;
            }
            // Our candidate year is correct or earlier than the right one. Find out which by
            // comparing it with the length of the candidate year.
            int candidateLength = GetDaysInYear(candidate);
            while (daysFromCandidateStartToTarget >= candidateLength)
            {
                // Our candidate year is earlier than we want, so fast forward a year,
                // removing the current candidate length from the "remaining days" and
                // working out the length of the new candidate.
                candidate++;
                daysFromCandidateStartToTarget -= candidateLength;
                candidateLength = GetDaysInYear(candidate);
            }
            zeroBasedDayOfYear = daysFromCandidateStartToTarget;
            return candidate;
        }

        /// <summary>
        /// Returns the year-of-era for the given date. The base implementation is to return the plain
        /// year, which is suitable for single-era calendars.
        /// </summary>
        internal virtual int GetYearOfEra(YearMonthDay yearMonthDay)
        {
            return yearMonthDay.Year;
        }

        /// <summary>
        /// Handling for century-of-era where (say) year 123 is in century 2... but so is year 200.
        /// </summary>
        internal virtual int GetCenturyOfEra(YearMonthDay yearMonthDay)
        {
            int yearOfEra = GetYearOfEra(yearMonthDay);
            int zeroBasedRemainder = yearOfEra % 100;
            int zeroBasedResult = yearOfEra / 100;
            return zeroBasedRemainder == 0 ? zeroBasedResult : zeroBasedResult + 1;
        }

        /// <summary>
        /// Handling for year-of-century in the range [1, 100].
        /// </summary>
        internal virtual int GetYearOfCentury(YearMonthDay yearMonthDay)
        {
            int yearOfEra = GetYearOfEra(yearMonthDay);
            int zeroBased = yearOfEra % 100;
            return zeroBased == 0 ? 100 : zeroBased;
        }

        /// <summary>
        /// Returns the era for the given date. The base implementation is to return 0, which is
        /// suitable for single-era calendars.
        /// </summary>
        internal virtual int GetEra(YearMonthDay yearMonthDay)
        {
            return 0;
        }

        internal virtual int GetDaysInYear(int year)
        {
            return IsLeapYear(year) ? 366 : 365;
        }

        /// <summary>
        /// Default implementation of GetAbsoluteYear which assumes a single era.
        /// This does not perform any validation.
        /// </summary>
        internal virtual int GetAbsoluteYear(int yearOfEra, int eraIndex)
        {
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
        internal virtual int GetStartOfYearInDays(int year)
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

        // TODO(2.0): Optimizations for specific calendars.
        internal void ValidateYearMonthDay(int year, int month, int day)
        {
            Preconditions.CheckArgumentRange("year", year, minYear, maxYear);
            Preconditions.CheckArgumentRange("month", month, 1, GetMaxMonth(year));
            Preconditions.CheckArgumentRange("day", day, 1, GetDaysInMonth(year, month));
        }
    }
}
