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
using NodaTime.Calendars;

namespace NodaTime.Periods
{
    public sealed partial class Period : IPeriod, IEquatable<Period>
    {
        private static readonly Period zero = new Period();

        /// <summary>
        /// Gets a period of zero length and standard period type.
        /// </summary>
        public static Period Zero { get { return zero; } }

        /// <summary>
        /// Creates a period with a specified number of years.
        /// </summary>
        /// <param name="years">The amount of years in this period</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// The standard period type is used, thus you can add other fields such
        /// as months or days using the <code>WithXxx()</code> methods.
        /// For example, <code>Period.FromYears(2).WithMonths(6);</code>
        /// </para>
        /// <para>
        /// If you want a year-based period that cannot have other fields added,
        /// then you should consider using <see cref="Years"/>.
        /// </para>
        /// </remarks>
        public static Period FromYears(int years)
        {
            return new Period(new[] {years, 0, 0, 0, 0, 0, 0, 0}, PeriodType.Standard);
        }

        /// <summary>
        /// Creates a period with a specified number of months.
        /// </summary>
        /// <param name="months">The amount of months in this period</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// The standard period type is used, thus you can add other fields such
        /// as years or days using the <code>WithXxx()</code> methods.
        /// For example, <code>Period.FromMonths(2).WithDays(6);</code>
        /// </para>
        /// <para>
        /// If you want a month-based period that cannot have other fields added,
        /// then you should consider using <see cref="Months"/>.
        /// </para>
        /// </remarks>
        public static Period FromMonths(int months)
        {
            return new Period(new[] {0, months, 0, 0, 0, 0, 0, 0}, PeriodType.Standard);
        }

        /// <summary>
        /// Creates a period with a specified number of weeks.
        /// </summary>
        /// <param name="weeks">The amount of weeks in this period</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// The standard period type is used, thus you can add other fields such
        /// as months or days using the <code>WithXxx()</code> methods.
        /// For example, <code>Period.FromWeeks(2).WithDays(6);</code>
        /// </para>
        /// <para>
        /// If you want a week-based period that cannot have other fields added,
        /// then you should consider using <see cref="Weeks"/>.
        /// </para>
        /// </remarks>
        public static Period FromWeeks(int weeks)
        {
            return new Period(new[] {0, 0, weeks, 0, 0, 0, 0, 0}, PeriodType.Standard);
        }

        /// <summary>
        /// Creates a period with a specified number of days.
        /// </summary>
        /// <param name="days">The amount of days in this period</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// The standard period type is used, thus you can add other fields such
        /// as months or weeks using the <code>WithXxx()</code> methods.
        /// For example, <code>Period.FromDays(2).WithHours(6);</code>
        /// </para>
        /// <para>
        /// If you want a day-based period that cannot have other fields added,
        /// then you should consider using <see cref="Days"/>.
        /// </para>
        /// </remarks>
        public static Period FromDays(int days)
        {
            return new Period(new[] {0, 0, 0, days, 0, 0, 0, 0}, PeriodType.Standard);
        }

        /// <summary>
        /// Creates a period with a specified number of hours.
        /// </summary>
        /// <param name="hours">The amount of days in this period</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// The standard period type is used, thus you can add other fields such
        /// as months or days using the <code>WithXxx()</code> methods.
        /// For example, <code>Period.FromHours(2).WithMinutes(30);</code>
        /// </para>
        /// <para>
        /// If you want a hour-based period that cannot have other fields added,
        /// then you should consider using <see cref="Hours"/>.
        /// </para>
        /// </remarks>
        public static Period FromHours(int hours)
        {
            return new Period(new[] {0, 0, 0, 0, hours, 0, 0, 0}, PeriodType.Standard);
        }

        /// <summary>
        /// Creates a period with a specified number of minutes.
        /// </summary>
        /// <param name="minutes">The amount of minutes in this period</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// The standard period type is used, thus you can add other fields such
        /// as days or hours using the <code>WithXxx()</code> methods.
        /// For example, <code>Period.FromMinutes(2).WithSeconds(30);</code>
        /// </para>
        /// <para>
        /// If you want a minute-based period that cannot have other fields added,
        /// then you should consider using <see cref="Minutes"/>.
        /// </para>
        /// </remarks>
        public static Period FromMinutes(int minutes)
        {
            return new Period(new[] {0, 0, 0, 0, 0, minutes, 0, 0}, PeriodType.Standard);
        }

        /// <summary>
        /// Creates a period with a specified number of seconds.
        /// </summary>
        /// <param name="seconds">The amount of seconds in this period</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// The standard period type is used, thus you can add other fields such
        /// as days or hours using the <code>WithXxx()</code> methods.
        /// For example, <code>Period.FromSeconds(2).WithMilliseconds(30);</code>
        /// </para>
        /// <para>
        /// If you want a second-based period that cannot have other fields added,
        /// then you should consider using <see cref="Seconds"/>.
        /// </para>
        /// </remarks>
        public static Period FromSeconds(int seconds)
        {
            return new Period(new[] {0, 0, 0, 0, 0, 0, seconds, 0}, PeriodType.Standard);
        }

        /// <summary>
        /// Creates a period with a specified number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The amount of milliseconds in this period</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// The standard period type is used, thus you can add other fields such
        /// as days or hours using the <code>WithXxx()</code> methods.
        /// For example, <code>Period.FromSeconds(2).WithMillis(30);</code>
        /// </para>
        /// </remarks>
        public static Period FromMilliseconds(int milliseconds)
        {
            return new Period(new[] {0, 0, 0, 0, 0, 0, 0, milliseconds}, PeriodType.Standard);
        }

        /// <summary>
        /// Creates a period from the given duration.
        /// </summary>
        /// <param name="duration">The duration</param>
        /// <param name="calendar">The calendar system to use to split the duration</param>
        /// <param name="periodType">Which set of fields this period supports</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// Only precise fields in the period type will be used.
        /// Imprecise fields will not be populated.
        /// </para>
        /// <para>
        /// If the duration is small then this method will perform
        /// as you might expect and split the fields evenly.
        /// </para>
        /// <para>
        /// If the duration is large then all the remaining duration will
        /// be stored in the largest available precise field.
        /// </para>
        /// </remarks>
        public static Period From(Duration duration, ICalendarSystem calendar, PeriodType periodType)
        {
            return From(LocalInstant.LocalUnixEpoch, LocalInstant.LocalUnixEpoch + duration, calendar, periodType);
        }

        /// <summary>
        /// Creates a period from the given duration with the standard set of fields.
        /// </summary>
        /// <param name="duration">The duration</param>
        /// <param name="calendar">The calendar system to use to split the duration</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// Only precise fields in the period type will be used.
        /// Imprecise fields will not be populated.
        /// </para>
        /// <para>
        /// If the duration is small then this method will perform
        /// as you might expect and split the fields evenly.
        /// </para>
        /// <para>
        /// If the duration is large then all the remaining duration will
        /// be stored in the largest available precise field.
        /// </para>
        /// </remarks>
        public static Period From(Duration duration, ICalendarSystem calendar)
        {
            return From(LocalInstant.LocalUnixEpoch, LocalInstant.LocalUnixEpoch + duration, calendar, PeriodType.Standard);
        }

        /// <summary>
        /// Creates a period from the given duration.
        /// </summary>
        /// <param name="duration">The duration</param>
        /// <param name="periodType">Which set of fields this period supports</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// Only precise fields in the period type will be used.
        /// Imprecise fields will not be populated.
        /// </para>
        /// <para>
        /// If the duration is small then this method will perform
        /// as you might expect and split the fields evenly.
        /// </para>
        /// <para>
        /// If the duration is large then all the remaining duration will
        /// be stored in the largest available precise field.
        /// </para>
        /// </remarks>
        public static Period From(Duration duration, PeriodType periodType)
        {
            return From(LocalInstant.LocalUnixEpoch, LocalInstant.LocalUnixEpoch + duration, IsoCalendarSystem.Instance, periodType);
        }

        /// <summary>
        /// Creates a period from the given duration.
        /// </summary>
        /// <param name="duration">The duration</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// Only precise fields in the period type will be used.
        /// Imprecise fields will not be populated.
        /// </para>
        /// <para>
        /// If the duration is small then this method will perform
        /// as you might expect and split the fields evenly.
        /// </para>
        /// <para>
        /// If the duration is large then all the remaining duration will
        /// be stored in the largest available precise field.
        /// </para>
        /// </remarks>
        public static Period From(Duration duration)
        {
            return From(LocalInstant.LocalUnixEpoch, LocalInstant.LocalUnixEpoch + duration, IsoCalendarSystem.Instance, PeriodType.Standard);
        }

        /// <summary>
        /// Creates a period from the given local interval endpoints.
        /// </summary>
        /// <param name="start">Interval start</param>
        /// <param name="end">Interval end</param>
        /// <param name="calendar">The calendar system to use</param>
        /// <param name="periodType">Which set of fields this period supports</param>
        /// <returns>The period</returns>
        public static Period From(LocalInstant start, LocalInstant end, ICalendarSystem calendar, PeriodType periodType)
        {
            if (calendar == null)
            {
                throw new ArgumentNullException("calendar");
            }
            if (periodType == null)
            {
                throw new ArgumentNullException("periodType");
            }

            return new Period(calendar.GetPeriodValues(periodType, start, end), periodType);
        }

        /// <summary>
        /// Creates a period from the given local interval endpoints with the standard set of fields.
        /// </summary>
        /// <param name="start">Interval start</param>
        /// <param name="end">Interval end</param>
        /// <param name="calendar">The calendar system to use</param>
        /// <returns>The period</returns>
        public static Period From(LocalInstant start, LocalInstant end, ICalendarSystem calendar)
        {
            return From(start, end, calendar, PeriodType.Standard);
        }

        /// <summary>
        /// Creates a period from the given local interval endpoints.
        /// </summary>
        /// <param name="start">Interval start</param>
        /// <param name="end">Interval end</param>
        /// <param name="periodType">Which set of fields this period supports</param>
        /// <returns>The period</returns>
        public static Period From(LocalInstant start, LocalInstant end, PeriodType periodType)
        {
            return From(start, end, IsoCalendarSystem.Instance, periodType);
        }

        /// <summary>
        /// Creates a period from the given local interval endpoints with the standard set of fields.
        /// </summary>
        /// <param name="start">Interval start</param>
        /// <param name="end">Interval end</param>
        /// <returns>The period</returns>
        public static Period From(LocalInstant start, LocalInstant end)
        {
            return From(start, end, IsoCalendarSystem.Instance, PeriodType.Standard);
        }
    }
}