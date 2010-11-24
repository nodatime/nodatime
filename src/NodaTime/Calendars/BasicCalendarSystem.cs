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
using NodaTime.Fields;

namespace NodaTime.Calendars
{
    // TODO: Optimisations of GetLocalInstant etc.
    public abstract class BasicCalendarSystem : AssembledCalendarSystem
    {
        private static readonly FieldSet preciseFields = CreatePreciseFields();

        private const int YearCacheSize = 1 << 10;
        private const int YearCacheMask = YearCacheSize - 1;
        private static readonly YearInfo[] yearCache = new YearInfo[YearCacheSize];

        private readonly int minDaysInFirstWeek;

        /// <summary>
        /// Returns the number of ticks from the start of the given year to the start of the given month.
        /// TODO: We always add this to the ticks at the start of the year. Why not just do it?
        /// </summary>
        protected abstract long GetTotalTicksByYearMonth(int year, int month);

        public abstract int MinYear { get; }
        public abstract int MaxYear { get; }
        public abstract long AverageTicksPerMonth { get; }
        public abstract long AverageTicksPerYear { get; }
        public abstract long AverageTicksPerYearDividedByTwo { get; }
        public abstract long ApproxTicksAtEpochDividedByTwo { get; }
        public abstract int GetDaysInYearMonth(int year, int month);
        protected abstract LocalInstant CalculateStartOfYear(int year);
        protected internal abstract bool IsLeapYear(int year);
        protected internal abstract int GetMonthOfYear(LocalInstant localInstant, int year);
        internal abstract int GetDaysInMonthMax(int month);
        internal abstract long GetYearDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant);
        internal abstract LocalInstant SetYear(LocalInstant localInstant, int year);

        private static FieldSet CreatePreciseFields()
        {
            // First create the simple durations, then fill in date/time fields,
            // which rely on the other properties
            FieldSet.Builder builder = new FieldSet.Builder
                                       {
                                           Ticks = TicksDurationField.Instance,
                                           Milliseconds = PreciseDurationField.Milliseconds,
                                           Seconds = PreciseDurationField.Seconds,
                                           Minutes = PreciseDurationField.Minutes,
                                           Hours = PreciseDurationField.Hours,
                                           HalfDays = PreciseDurationField.HalfDays,
                                           Days = PreciseDurationField.Days,
                                           Weeks = PreciseDurationField.Weeks
                                       };
            builder.TickOfMillisecond = new PreciseDateTimeField(DateTimeFieldType.TickOfMillisecond, builder.Ticks, builder.Milliseconds);
            builder.TickOfDay = new PreciseDateTimeField(DateTimeFieldType.TickOfDay, builder.Ticks, builder.Days);
            builder.MillisecondOfSecond = new PreciseDateTimeField(DateTimeFieldType.MillisecondOfSecond, builder.Milliseconds, builder.Seconds);
            builder.MillisecondOfDay = new PreciseDateTimeField(DateTimeFieldType.MillisecondOfDay, builder.Milliseconds, builder.Days);
            builder.SecondOfMinute = new PreciseDateTimeField(DateTimeFieldType.SecondOfMinute, builder.Seconds, builder.Minutes);
            builder.SecondOfDay = new PreciseDateTimeField(DateTimeFieldType.SecondOfDay, builder.Seconds, builder.Days);
            builder.MinuteOfHour = new PreciseDateTimeField(DateTimeFieldType.MinuteOfHour, builder.Minutes, builder.Hours);
            builder.MinuteOfDay = new PreciseDateTimeField(DateTimeFieldType.MinuteOfDay, builder.Minutes, builder.Days);
            builder.HourOfDay = new PreciseDateTimeField(DateTimeFieldType.HourOfDay, builder.Hours, builder.Days);
            builder.HourOfHalfDay = new PreciseDateTimeField(DateTimeFieldType.HourOfHalfDay, builder.Hours, builder.HalfDays);
            builder.ClockHourOfDay = new ZeroIsMaxDateTimeField(builder.HourOfDay, DateTimeFieldType.ClockHourOfDay);
            builder.ClockHourOfHalfDay = new ZeroIsMaxDateTimeField(builder.HourOfHalfDay, DateTimeFieldType.ClockHourOfHalfDay);
            // TODO: This was a separate subclass in Joda, for i18n purposes
            builder.HalfDayOfDay = new PreciseDateTimeField(DateTimeFieldType.HalfDayOfDay, builder.HalfDays, builder.Days);
            return builder.Build();
        }

        protected BasicCalendarSystem(string name, ICalendarSystem baseCalendar, int minDaysInFirstWeek) : base(name, baseCalendar)
        {
            if (minDaysInFirstWeek < 1 || minDaysInFirstWeek > 7)
            {
                throw new ArgumentException("Invalid min days in first week: " + minDaysInFirstWeek);
            }
            this.minDaysInFirstWeek = minDaysInFirstWeek;
            // Effectively invalidate the first cache entry.
            // Every other cache entry will automatically be invalid,
            // by having year 0.
            yearCache[0] = new YearInfo(1, LocalInstant.LocalUnixEpoch.Ticks);
        }

        protected override void AssembleFields(FieldSet.Builder builder)
        {
            // First copy the fields that are the same for all basic
            // calendars
            builder.WithSupportedFieldsFrom(preciseFields);

            // Now create fields that have unique behavior for Gregorian and Julian
            // calendars.

            builder.Year = new BasicYearDateTimeField(this);
            builder.YearOfEra = new GJYearOfEraDateTimeField(builder.Year, this);

            // Define one-based centuryOfEra and yearOfCentury.
            IDateTimeField field = new OffsetDateTimeField(builder.YearOfEra, 99);
            builder.CenturyOfEra = new DividedDateTimeField(field, DateTimeFieldType.CenturyOfEra, 100);

            field = new RemainderDateTimeField((DividedDateTimeField)builder.CenturyOfEra);
            builder.YearOfCentury = new OffsetDateTimeField(field, DateTimeFieldType.YearOfCentury, 1);

            builder.Era = new GJEraDateTimeField(this);
            builder.DayOfWeek = new GJDayOfWeekDateTimeField(this, builder.Days);
            builder.DayOfMonth = new BasicDayOfMonthDateTimeField(this, builder.Days);
            builder.DayOfYear = new BasicDayOfYearDateTimeField(this, builder.Days);
            builder.MonthOfYear = new GJMonthOfYearDateTimeField(this);
            builder.WeekYear = new BasicWeekYearDateTimeField(this);
            builder.WeekOfWeekYear = new BasicWeekOfWeekYearDateTimeField(this, builder.Weeks);

            field = new RemainderDateTimeField(builder.WeekYear, DateTimeFieldType.WeekYearOfCentury, 100);
            builder.WeekYearOfCentury = new OffsetDateTimeField(field, DateTimeFieldType.WeekYearOfCentury, 1);
            // The remaining (imprecise) durations are available from the newly
            // created datetime fields.

            builder.Years = builder.Year.DurationField;
            builder.Centuries = builder.CenturyOfEra.DurationField;
            builder.Months = builder.MonthOfYear.DurationField;
            builder.WeekYears = builder.WeekYear.DurationField;
        }

        /// <summary>
        /// Fetches the start of the year from the cache, or calculates
        /// and caches it.
        /// </summary>
        internal long GetYearTicks(int year)
        {
            YearInfo info = yearCache[year & YearCacheMask];
            if (info.Year != year)
            {
                info = new YearInfo(year, CalculateStartOfYear(year).Ticks);
                yearCache[year & YearCacheMask] = info;
            }
            return info.StartOfYearTicks;
        }

        internal int GetDayOfWeek(LocalInstant localInstant)
        {
            // 1970-01-01 is day of week 4, Thursday.

            long daysSince19700101;
            long ticks = localInstant.Ticks;
            if (ticks >= 0)
            {
                daysSince19700101 = ticks / NodaConstants.TicksPerDay;
            }
            else
            {
                daysSince19700101 = (ticks - (NodaConstants.TicksPerDay - 1)) / NodaConstants.TicksPerDay;
                if (daysSince19700101 < -3)
                {
                    return 7 + (int)((daysSince19700101 + 4) % 7);
                }
            }

            return 1 + (int)((daysSince19700101 + 3) % 7);
        }

        internal int GetDayOfMonth(LocalInstant localInstant)
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
            long dateTicks = GetYearTicks(year);
            dateTicks += GetTotalTicksByYearMonth(year, month);
            return (int)((localInstant.Ticks - dateTicks) / NodaConstants.TicksPerDay) + 1;
        }

        internal int GetDaysInMonthMax()
        {
            return 31;
        }

        internal int GetMonthOfYear(LocalInstant localInstant)
        {
            return GetMonthOfYear(localInstant, GetYear(localInstant));
        }

        internal int GetDaysInMonthMax(LocalInstant instant)
        {
            int thisYear = GetYear(instant);
            int thisMonth = GetMonthOfYear(instant, thisYear);
            return GetDaysInYearMonth(thisYear, thisMonth);
        }

        internal int GetYear(LocalInstant instant)
        {
            long ticks = instant.Ticks;
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
            else if (diff >= NodaConstants.TicksPerDay * 365L)
            {
                // One year may need to be added to fix estimate.
                long oneYear = NodaConstants.TicksPerDay * (IsLeapYear(year) ? 366L : 365L);
                yearStart += oneYear;

                if (yearStart <= instant.Ticks)
                {
                    // Didn't go too far, so actually add one year.
                    year++;
                }
            }

            return year;
        }

        internal int GetDaysInYearMax()
        {
            return 366;
        }

        internal int GetDaysInYearMax(int year)
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
            return (int)((localInstant.Ticks - yearStart) / NodaConstants.TicksPerDay) + 1;
        }

        // Note: no overload taking the year, as it's never used in Joda
        internal int GetMaxMonth()
        {
            return 12;
        }

        internal long GetTickOfDay(LocalInstant localInstant)
        {
            long ticks = localInstant.Ticks;
            return ticks >= 0 ? ticks % NodaConstants.TicksPerDay : (NodaConstants.TicksPerDay - 1) + ((ticks + 1) % NodaConstants.TicksPerDay);
        }

        internal long GetYearMonthDayTicks(int year, int month, int dayOfMonth)
        {
            long ticks = GetYearTicks(year);
            ticks += GetTotalTicksByYearMonth(year, month);
            return ticks + (dayOfMonth - 1) * NodaConstants.TicksPerDay;
        }

        internal long GetYearMonthTicks(int year, int month)
        {
            long ticks = GetYearTicks(year);
            ticks += GetTotalTicksByYearMonth(year, month);
            return ticks;
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
                return GetYear(localInstant + Duration.OneWeek);
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
            return (int)((localInstant.Ticks - firstWeekTicks1) / NodaConstants.TicksPerWeek) + 1;
        }

        internal int GetWeeksInYear(int year)
        {
            long firstWeekTicks1 = GetFirstWeekOfYearTicks(year);
            long firstWeekTicks2 = GetFirstWeekOfYearTicks(year + 1);
            return (int)((firstWeekTicks2 - firstWeekTicks1) / NodaConstants.TicksPerWeek);
        }

        private long GetFirstWeekOfYearTicks(int year)
        {
            long jan1Millis = GetYearTicks(year);
            int jan1DayOfWeek = GetDayOfWeek(LocalInstant.FromTicks(jan1Millis));

            if (jan1DayOfWeek > (8 - minDaysInFirstWeek))
            {
                // First week is end of previous year because it doesn't have enough days.
                return jan1Millis + (8 - jan1DayOfWeek) * NodaConstants.TicksPerDay;
            }
            else
            {
                // First week is start of this year because it has enough days.
                return jan1Millis - (jan1DayOfWeek - 1) * NodaConstants.TicksPerDay;
            }
        }

        protected virtual long GetDateMidnightTicks(int year, int monthOfYear, int dayOfMonth)
        {
            FieldUtils.VerifyValueBounds(DateTimeFieldType.Year, year, MinYear, MaxYear);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.MonthOfYear, monthOfYear, 1, GetMaxMonth());
            FieldUtils.VerifyValueBounds(DateTimeFieldType.DayOfMonth, dayOfMonth, 1, GetDaysInYearMonth(year, monthOfYear));
            return GetYearMonthDayTicks(year, monthOfYear, dayOfMonth);
        }

        // TODO: Override the remaining GetLocalInstant overload?

        public override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour)
        {
            if (BaseCalendar != null)
            {
                return BaseCalendar.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour);
            }
            FieldUtils.VerifyValueBounds(DateTimeFieldType.HourOfDay, hourOfDay, 0, 23);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.MinuteOfHour, minuteOfHour, 0, 59);

            return
                LocalInstant.FromTicks(GetDateMidnightTicks(year, monthOfYear, dayOfMonth) + hourOfDay * NodaConstants.TicksPerHour +
                                 minuteOfHour * NodaConstants.TicksPerMinute);
        }

        public override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute)
        {
            if (BaseCalendar != null)
            {
                return BaseCalendar.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute);
            }
            FieldUtils.VerifyValueBounds(DateTimeFieldType.HourOfDay, hourOfDay, 0, 23);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.MinuteOfHour, minuteOfHour, 0, 59);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.SecondOfMinute, secondOfMinute, 0, 59);

            return
                LocalInstant.FromTicks(GetDateMidnightTicks(year, monthOfYear, dayOfMonth) + hourOfDay * NodaConstants.TicksPerHour +
                                 minuteOfHour * NodaConstants.TicksPerMinute + secondOfMinute * NodaConstants.TicksPerSecond);
        }

        public override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute,
                                                     int millisecondOfSecond, int tickOfMillisecond)
        {
            if (BaseCalendar != null)
            {
                return BaseCalendar.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute, millisecondOfSecond,
                                                    tickOfMillisecond);
            }
            FieldUtils.VerifyValueBounds(DateTimeFieldType.HourOfDay, hourOfDay, 0, 23);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.MinuteOfHour, minuteOfHour, 0, 59);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.SecondOfMinute, secondOfMinute, 0, 59);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.MillisecondOfSecond, millisecondOfSecond, 0, 999);
            FieldUtils.VerifyValueBounds(DateTimeFieldType.TickOfMillisecond, tickOfMillisecond, 0, NodaConstants.TicksPerMillisecond - 1);

            return
                LocalInstant.FromTicks(GetDateMidnightTicks(year, monthOfYear, dayOfMonth) + hourOfDay * NodaConstants.TicksPerHour +
                                 minuteOfHour * NodaConstants.TicksPerMinute + secondOfMinute * NodaConstants.TicksPerSecond +
                                 millisecondOfSecond * NodaConstants.TicksPerMillisecond + tickOfMillisecond);
        }

        public override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, long tickOfDay)
        {
            if (BaseCalendar != null)
            {
                return BaseCalendar.GetLocalInstant(year, monthOfYear, dayOfMonth, tickOfDay);
            }
            // TODO: Report bug in Joda Time, which doesn't have the - 1 here.
            FieldUtils.VerifyValueBounds(DateTimeFieldType.TickOfDay, tickOfDay, 0, NodaConstants.TicksPerDay - 1);
            return LocalInstant.FromTicks(GetDateMidnightTicks(year, monthOfYear, dayOfMonth) + tickOfDay);
        }
    }
}