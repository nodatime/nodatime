#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using NodaTime.Fields;
using NodaTime.Utility;
using System.Globalization;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Abstract implementation for calendar systems that use a typical day/month/year/leapYear model,
    /// assuming a constant number of months. Most concrete calendars in Noda Time either derive from
    /// this class or delegate to another instance of it.
    /// </summary>
    internal abstract class BasicCalendarSystem : CalendarSystem
    {
        private static readonly FieldSet FixedLengthFields = CreateFixedLengthFields();

        private const int YearCacheSize = 1 << 10;
        private const int YearCacheMask = YearCacheSize - 1;
        private readonly YearInfo[] yearCache = new YearInfo[YearCacheSize];

        private readonly int minDaysInFirstWeek;

        /// <summary>
        /// Returns the number of ticks from the start of the given year to the start of the given month.
        /// </summary>
        protected abstract long GetTicksFromStartOfYearToStartOfMonth(int year, int month);

        internal abstract long AverageTicksPerMonth { get; }
        internal abstract long AverageTicksPerYear { get; }
        internal abstract long AverageTicksPerYearDividedByTwo { get; }
        internal abstract long ApproxTicksAtEpochDividedByTwo { get; }
        protected abstract LocalInstant CalculateStartOfYear(int year);
        protected internal abstract int GetMonthOfYear(LocalInstant localInstant, int year);
        internal abstract int GetDaysInMonthMax(int month);
        internal abstract long GetYearDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant);
        internal abstract LocalInstant SetYear(LocalInstant localInstant, int year);

        /// <summary>
        /// Creates an ID for a calendar system which only needs to be distinguished by its name and
        /// the minimum number of days in the first week of the week-year.
        /// </summary>
        protected static string CreateIdFromNameAndMinDaysInFirstWeek(string name, int minDaysInFirstWeek)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1}", name, minDaysInFirstWeek);
        }

        private static FieldSet CreateFixedLengthFields()
        {
            // First create the simple durations, then fill in date/time fields,
            // which rely on the other properties
            FieldSet.Builder builder = new FieldSet.Builder
                                       {
                                           Ticks = TicksPeriodField.Instance,
                                           Milliseconds = FixedLengthPeriodField.Milliseconds,
                                           Seconds = FixedLengthPeriodField.Seconds,
                                           Minutes = FixedLengthPeriodField.Minutes,
                                           Hours = FixedLengthPeriodField.Hours,
                                           HalfDays = FixedLengthPeriodField.HalfDays,
                                           Days = FixedLengthPeriodField.Days,
                                           Weeks = FixedLengthPeriodField.Weeks
                                       };
            builder.TickOfSecond = new FixedLengthDateTimeField(DateTimeFieldType.TickOfSecond, builder.Ticks, builder.Seconds);
            builder.TickOfMillisecond = new FixedLengthDateTimeField(DateTimeFieldType.TickOfMillisecond, builder.Ticks, builder.Milliseconds);
            builder.TickOfDay = new FixedLengthDateTimeField(DateTimeFieldType.TickOfDay, builder.Ticks, builder.Days);
            builder.MillisecondOfSecond = new FixedLengthDateTimeField(DateTimeFieldType.MillisecondOfSecond, builder.Milliseconds, builder.Seconds);
            builder.MillisecondOfDay = new FixedLengthDateTimeField(DateTimeFieldType.MillisecondOfDay, builder.Milliseconds, builder.Days);
            builder.SecondOfMinute = new FixedLengthDateTimeField(DateTimeFieldType.SecondOfMinute, builder.Seconds, builder.Minutes);
            builder.SecondOfDay = new FixedLengthDateTimeField(DateTimeFieldType.SecondOfDay, builder.Seconds, builder.Days);
            builder.MinuteOfHour = new FixedLengthDateTimeField(DateTimeFieldType.MinuteOfHour, builder.Minutes, builder.Hours);
            builder.MinuteOfDay = new FixedLengthDateTimeField(DateTimeFieldType.MinuteOfDay, builder.Minutes, builder.Days);
            builder.HourOfDay = new FixedLengthDateTimeField(DateTimeFieldType.HourOfDay, builder.Hours, builder.Days);
            builder.HourOfHalfDay = new FixedLengthDateTimeField(DateTimeFieldType.HourOfHalfDay, builder.Hours, builder.HalfDays);
            builder.ClockHourOfDay = new ZeroIsMaxDateTimeField(builder.HourOfDay, DateTimeFieldType.ClockHourOfDay);
            builder.ClockHourOfHalfDay = new ZeroIsMaxDateTimeField(builder.HourOfHalfDay, DateTimeFieldType.ClockHourOfHalfDay);
            // This was a separate subclass in Joda, for i18n purposes
            // Our calendar systems don't have their own i18n support.
            builder.HalfDayOfDay = new FixedLengthDateTimeField(DateTimeFieldType.HalfDayOfDay, builder.HalfDays, builder.Days);
            return builder.Build();
        }

        protected BasicCalendarSystem(string id, string name, int minDaysInFirstWeek, int minYear, int maxYear, FieldAssembler assembler, IEnumerable<Era> eras)
            : base(id, name, minYear, maxYear, AssembleFields + assembler, eras)
        {
            if (minDaysInFirstWeek < 1 || minDaysInFirstWeek > 7)
            {
                throw new ArgumentOutOfRangeException("minDaysInFirstWeek", "Minimum days in first week must be between 1 and 7 inclusive");
            }
            this.minDaysInFirstWeek = minDaysInFirstWeek;
            // Effectively invalidate the first cache entry.
            // Every other cache entry will automatically be invalid,
            // by having year 0.
            yearCache[0] = new YearInfo(1, LocalInstant.LocalUnixEpoch.Ticks);
        }

        private static void AssembleFields(FieldSet.Builder builder, CalendarSystem @this)
        {
            // None of the fields will call anything on the calendar system *yet*, so this is safe enough.
            BasicCalendarSystem thisCalendar = (BasicCalendarSystem) @this;
            // First copy the fields that are the same for all basic
            // calendars
            builder.WithSupportedFieldsFrom(FixedLengthFields);

            // Now create fields that have unique behavior for Gregorian and Julian
            // calendars.

            builder.Year = new BasicYearDateTimeField(thisCalendar);
            builder.YearOfEra = new GJYearOfEraDateTimeField(builder.Year, thisCalendar);

            // Define one-based centuryOfEra and yearOfCentury.
            DateTimeField field = new OffsetDateTimeField(builder.YearOfEra, 99);
            builder.CenturyOfEra = new DividedDateTimeField(field, DateTimeFieldType.CenturyOfEra, 100);

            field = new RemainderDateTimeField((DividedDateTimeField)builder.CenturyOfEra);
            builder.YearOfCentury = new OffsetDateTimeField(field, DateTimeFieldType.YearOfCentury, 1);

            builder.Era = new GJEraDateTimeField(thisCalendar);
            builder.DayOfWeek = new GJDayOfWeekDateTimeField(thisCalendar, builder.Days);
            builder.DayOfMonth = new BasicDayOfMonthDateTimeField(thisCalendar, builder.Days);
            builder.DayOfYear = new BasicDayOfYearDateTimeField(thisCalendar, builder.Days);
            builder.MonthOfYear = new BasicMonthOfYearDateTimeField(thisCalendar, 2); // February is the leap month
            builder.WeekYear = new BasicWeekYearDateTimeField(thisCalendar);
            builder.WeekOfWeekYear = new BasicWeekOfWeekYearDateTimeField(thisCalendar, builder.Weeks);

            field = new RemainderDateTimeField(builder.WeekYear, DateTimeFieldType.WeekYearOfCentury, 100);
            builder.WeekYearOfCentury = new OffsetDateTimeField(field, DateTimeFieldType.WeekYearOfCentury, 1);
            // The remaining (variable length) periods are available from the newly
            // created date/time fields.

            builder.Years = builder.Year.PeriodField;
            builder.Centuries = builder.CenturyOfEra.PeriodField;
            builder.Months = builder.MonthOfYear.PeriodField;
            builder.WeekYears = builder.WeekYear.PeriodField;
        }

        /// <summary>
        /// Fetches the start of the year from the cache, or calculates
        /// and caches it.
        /// </summary>
        internal virtual long GetYearTicks(int year)
        {
            lock (yearCache)
            {
                YearInfo info = yearCache[year & YearCacheMask];
                if (info.Year != year)
                {
                    info = new YearInfo(year, CalculateStartOfYear(year).Ticks);
                    yearCache[year & YearCacheMask] = info;
                }
                return info.StartOfYearTicks;
            }
        }

        internal int GetDayOfWeek(LocalInstant localInstant)
        {
            // 1970-01-01 is day of week 4, Thursday.

            long daysSince19700101;
            long ticks = localInstant.Ticks;
            if (ticks >= 0)
            {
                daysSince19700101 = ticks / NodaConstants.TicksPerStandardDay;
            }
            else
            {
                daysSince19700101 = (ticks - (NodaConstants.TicksPerStandardDay - 1)) / NodaConstants.TicksPerStandardDay;
                if (daysSince19700101 < -3)
                {
                    return 7 + (int)((daysSince19700101 + 4) % 7);
                }
            }

            return 1 + (int)((daysSince19700101 + 3) % 7);
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

        internal virtual int GetMaxDaysInMonth()
        {
            return 31;
        }

        internal virtual int GetMonthOfYear(LocalInstant localInstant)
        {
            return GetMonthOfYear(localInstant, GetYear(localInstant));
        }

        internal int GetMaxDaysInMonth(LocalInstant localInstant)
        {
            int thisYear = GetYear(localInstant);
            int thisMonth = GetMonthOfYear(localInstant, thisYear);
            return GetDaysInMonth(thisYear, thisMonth);
        }

        internal virtual int GetYear(LocalInstant localInstant)
        {
            long ticks = localInstant.Ticks;
            // Get an initial estimate of the year, and the millis value that
            // represents the start of that year. Then verify estimate and fix if
            // necessary.

            // Initial estimate uses values divided by two to avoid overflow.
            long unitTicks = AverageTicksPerYearDividedByTwo;
            long i2 = (ticks >> 1) + ApproxTicksAtEpochDividedByTwo;
            if (i2 < 0)
            {
                i2 = i2 - unitTicks + 1;
            }
            int year = (int)(i2 / unitTicks);

            long yearStart = GetYearTicks(year);
            long diff = ticks - yearStart;
            if (diff < 0)
            {
                year--;
            }
            else if (diff >= NodaConstants.TicksPerStandardDay * 365L)
            {
                // One year may need to be added to fix estimate.
                long oneYear = NodaConstants.TicksPerStandardDay * (IsLeapYear(year) ? 366L : 365L);
                yearStart += oneYear;

                if (yearStart <= localInstant.Ticks)
                {
                    // Didn't go too far, so actually add one year.
                    year++;
                }
            }

            return year;
        }

        internal virtual int GetDaysInYearMax()
        {
            return 366;
        }

        internal virtual int GetDaysInYear(int year)
        {
            return IsLeapYear(year) ? 366 : 365;
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

        /// <summary>
        /// All basic calendars have the same number of months regardless of the year.
        /// (Different calendars can have a different number of months, but it doesn't vary by time.)
        /// </summary>
        public override int GetMaxMonth(int year)
        {
            return GetMaxMonth();
        }

        internal virtual int GetMaxMonth()
        {
            return 12;
        }

        internal long GetTickOfDay(LocalInstant localInstant)
        {
            long ticks = localInstant.Ticks;
            return ticks >= 0 ? ticks % NodaConstants.TicksPerStandardDay : (NodaConstants.TicksPerStandardDay - 1) + ((ticks + 1) % NodaConstants.TicksPerStandardDay);
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

        internal int GetWeekYear(LocalInstant localInstant)
        {
            int year = GetYear(localInstant);
            int week = GetWeekOfWeekYear(localInstant, year);
            if (week == 1)
            {
                return GetYear(localInstant + Duration.OneStandardWeek);
            }
            else if (week > 51)
            {
                return GetYear(localInstant - Duration.FromStandardWeeks(2));
            }
            else
            {
                return year;
            }
        }

        internal int GetWeekOfWeekYear(LocalInstant localInstant)
        {
            return GetWeekOfWeekYear(localInstant, GetYear(localInstant));
        }

        internal int GetWeekOfWeekYear(LocalInstant localInstant, int year)
        {
            long firstWeekTicks1 = GetFirstWeekOfYearTicks(year);
            if (localInstant.Ticks < firstWeekTicks1)
            {
                return GetWeeksInYear(year - 1);
            }
            long firstWeekTicks2 = GetFirstWeekOfYearTicks(year + 1);
            if (localInstant.Ticks >= firstWeekTicks2)
            {
                return 1;
            }
            return (int)((localInstant.Ticks - firstWeekTicks1) / NodaConstants.TicksPerStandardWeek) + 1;
        }

        internal int GetWeeksInYear(int year)
        {
            long firstWeekTicks1 = GetFirstWeekOfYearTicks(year);
            long firstWeekTicks2 = GetFirstWeekOfYearTicks(year + 1);
            return (int)((firstWeekTicks2 - firstWeekTicks1) / NodaConstants.TicksPerStandardWeek);
        }

        private long GetFirstWeekOfYearTicks(int year)
        {
            long jan1Millis = GetYearTicks(year);
            int jan1DayOfWeek = GetDayOfWeek(new LocalInstant(jan1Millis));

            if (jan1DayOfWeek > (8 - minDaysInFirstWeek))
            {
                // First week is end of previous year because it doesn't have enough days.
                return jan1Millis + (8 - jan1DayOfWeek) * NodaConstants.TicksPerStandardDay;
            }
            else
            {
                // First week is start of this year because it has enough days.
                return jan1Millis - (jan1DayOfWeek - 1) * NodaConstants.TicksPerStandardDay;
            }
        }

        protected virtual long GetDateMidnightTicks(int year, int monthOfYear, int dayOfMonth)
        {
            Preconditions.CheckArgumentRange("year", year, MinYear, MaxYear);
            Preconditions.CheckArgumentRange("monthOfYear", monthOfYear, 1, GetMaxMonth());
            Preconditions.CheckArgumentRange("dayOfMonth", dayOfMonth, 1, GetDaysInMonth(year, monthOfYear));
            return GetYearMonthDayTicks(year, monthOfYear, dayOfMonth);
        }

        internal override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour)
        {
            Preconditions.CheckArgumentRange("hourOfDay", hourOfDay, 0, 23);
            Preconditions.CheckArgumentRange("minuteOfHour", minuteOfHour, 0, 59);

            return
                new LocalInstant(GetDateMidnightTicks(year, monthOfYear, dayOfMonth) + hourOfDay * NodaConstants.TicksPerHour +
                                 minuteOfHour * NodaConstants.TicksPerMinute);
        }

        internal override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute)
        {
            Preconditions.CheckArgumentRange("hourOfDay", hourOfDay, 0, 23);
            Preconditions.CheckArgumentRange("minuteOfHour", minuteOfHour, 0, 59);
            Preconditions.CheckArgumentRange("secondOfMinute", secondOfMinute, 0, 59);

            return
                new LocalInstant(GetDateMidnightTicks(year, monthOfYear, dayOfMonth) + hourOfDay * NodaConstants.TicksPerHour +
                                 minuteOfHour * NodaConstants.TicksPerMinute + secondOfMinute * NodaConstants.TicksPerSecond);
        }

        internal override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute,
                                                       int millisecondOfSecond, int tickOfMillisecond)
        {
            Preconditions.CheckArgumentRange("hourOfDay", hourOfDay, 0, 23);
            Preconditions.CheckArgumentRange("minuteOfHour", minuteOfHour, 0, 59);
            Preconditions.CheckArgumentRange("secondOfMinute", secondOfMinute, 0, 59);
            Preconditions.CheckArgumentRange("millisecondOfSecond", millisecondOfSecond, 0, 999);
            Preconditions.CheckArgumentRange("tickOfMillisecond", tickOfMillisecond, 0, NodaConstants.TicksPerMillisecond - 1);

            return
                new LocalInstant(GetDateMidnightTicks(year, monthOfYear, dayOfMonth) + hourOfDay * NodaConstants.TicksPerHour +
                                 minuteOfHour * NodaConstants.TicksPerMinute + secondOfMinute * NodaConstants.TicksPerSecond +
                                 millisecondOfSecond * NodaConstants.TicksPerMillisecond + tickOfMillisecond);
        }

        internal override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, long tickOfDay)
        {
            Preconditions.CheckArgumentRange("tickOfDay", tickOfDay, 0, NodaConstants.TicksPerStandardDay - 1);
            return new LocalInstant(GetDateMidnightTicks(year, monthOfYear, dayOfMonth) + tickOfDay);
        }
    }
}