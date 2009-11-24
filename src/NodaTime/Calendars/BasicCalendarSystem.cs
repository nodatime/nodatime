#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
    public abstract class BasicCalendarSystem : AssembledCalendarSystem
    {
        private readonly static FieldSet preciseFields = CreatePreciseFields();

        const int yearCacheSize = 1 << 10;
        const int yearCacheMask = yearCacheSize - 1;
        private static readonly YearInfo[] yearCache = new YearInfo[yearCacheSize];

        private readonly int minDaysInFirstWeek;

        public abstract long AverageTicksPerYear { get; }
        public abstract long AverageTicksPerYearDividedByTwo { get; }

        private static FieldSet CreatePreciseFields()
        {
            // First create the simple durations, then fill in date/time fields,
            // which rely on the other properties
            FieldSet.Builder builder = new FieldSet.Builder()
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

        protected BasicCalendarSystem(ICalendarSystem baseCalendar, 
            int minDaysInFirstWeek)
            : base(baseCalendar)
        {
            if (minDaysInFirstWeek < 1 || minDaysInFirstWeek > 7)
            {
                throw new ArgumentException("Invalid min days in first week: " + minDaysInFirstWeek);
            }
            this.minDaysInFirstWeek = minDaysInFirstWeek;
            // Effectively invalidate the first cache entry.
            // Every other cache entry will automatically be invalid,
            // by having year 0.
            yearCache[0] = new YearInfo(1, LocalInstant.LocalUnixEpoch);
        }

        protected abstract LocalInstant CalculateStartOfYear(int year);

        protected abstract bool IsLeapYear(int year);

        protected override void AssembleFields(FieldSet.Builder builder)
        {
            // First copy the fields that are the same for all basic
            // calendars
            builder.WithSupportedFieldsFrom(preciseFields);

            // Now create fields that have unique behavior for Gregorian and Julian
            // calendars.

            builder.Year = new BasicYearDateTimeField(this);
            builder.YearOfEra = new GJYearOfEraDateTimeField(builder.Year, this);

            /* TODO: Uncomment this lot!
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
            builder.WeekYear = new BasicWeekyearDateTimeField(this);
            builder.WeekOfWeekYear = new BasicWeekOfWeekyearDateTimeField(this, builder.Weeks);
            
            field = new RemainderDateTimeField(builder.WeekYear, DateTimeFieldType.WeekYearOfCentury, 100);
            builder.WeekYearOfCentury = new OffsetDateTimeField(field, DateTimeFieldType.WeekYearOfCentury, 1);
            */
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
        internal LocalInstant GetStartOfYear(int year)
        {
            YearInfo info = yearCache[year & yearCacheMask];
            if (info.Year != year)
            {
                info = new YearInfo(year, CalculateStartOfYear(year));
                yearCache[year & yearCacheMask] = info;
            }
            return info.StartOfYear;
        }

        /// <summary>
        /// Immutable struct containing a year and the first tick of that year.
        /// This is cached to avoid it being calculated more often than is necessary.
        /// </summary>
        private struct YearInfo
        {
            private readonly int year;
            private readonly LocalInstant startOfYear;
            internal YearInfo(int year, LocalInstant startOfYear)
            {
                this.year = year;
                this.startOfYear = startOfYear;
            }

            internal int Year { get { return year; } }
            internal LocalInstant StartOfYear { get { return startOfYear; } }
        }
    }
}