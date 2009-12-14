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

namespace NodaTime.Periods
{
    /// <summary>
    /// An immutable time period specifying a set of duration field values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A time period is divided into a number of fields, such as hours and seconds.
    /// Which fields are supported is defined by the PeriodType class.
    /// The default is the standard period type, which supports years, months, weeks, days,
    /// hours, minutes, seconds and milliseconds.
    /// </para>
    /// </remarks>
    public sealed class Period : PeriodBase
    {
        #region Static creation methods and properties

        /// <summary>
        /// A period of zero length and standard period type.
        /// </summary>
        public static Period Zero
        {
            get { return new Period(); }
        }

        /// <summary>
        /// Create a period with a specified number of years.
        /// </summary>
        /// <param name="years">The amount of years in this period</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// The standard period type is used, thus you can add other fields such
        /// as months or days using the <code>WithXxx()</code> methods.
        /// For example, <code>Period.Years(2).WithMonths(6);</code>
        /// </para>
        /// <para>
        /// If you want a year-based period that cannot have other fields added,
        /// then you should consider using <see cref="Years"/>.
        /// </para>
        /// </remarks>
        public static Period Years(int years)
        {
            return new Period(new int[] { years, 0, 0, 0, 0, 0, 0, 0}, PeriodType.Standart);
        }

        /// <summary>
        /// Create a period with a specified number of months.
        /// </summary>
        /// <param name="months">The amount of months in this period</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// The standard period type is used, thus you can add other fields such
        /// as years or days using the <code>withXxx()</code> methods.
        /// For example, <code>Period.months(2).withDays(6);</code>
        /// </para>
        /// <para>
        /// If you want a month-based period that cannot have other fields added,
        /// then you should consider using <see cref="Months"/>.
        /// </para>
        /// </remarks>
        public static Period Months(int months)
        {
            return new Period(new int[] { 0, months, 0, 0, 0, 0, 0, 0}, PeriodType.Standart);
        }

        /// <summary>
        /// Create a period with a specified number of weeks.
        /// </summary>
        /// <param name="weeks">The amount of months in this period</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// The standard period type is used, thus you can add other fields such
        /// as months or days using the <code>withXxx()</code> methods.
        /// For example, <code>Period.weeks(2).withDays(6);</code>
        /// </para>
        /// <para>
        /// If you want a week-based period that cannot have other fields added,
        /// then you should consider using <see cref="Weeks"/>.
        /// </para>
        /// </remarks>
        public static Period Weeks(int weeks)
        {
            return new Period(new int[] { 0, 0, weeks, 0, 0, 0, 0, 0}, PeriodType.Standart);
        }

        /// <summary>
        /// Create a period with a specified number of days.
        /// </summary>
        /// <param name="days">The amount of days in this period</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// The standard period type is used, thus you can add other fields such
        /// as months or weeks using the <code>withXxx()</code> methods.
        /// For example, <code>Period.days(2).withHours(6);</code>
        /// </para>
        /// <para>
        /// If you want a day-based period that cannot have other fields added,
        /// then you should consider using <see cref="Days"/>.
        /// </para>
        /// </remarks>
        public static Period Days(int days)
        {
            return new Period(new int[] { 0, 0, 0, days, 0, 0, 0, 0}, PeriodType.Standart);
        }

        /// <summary>
        /// Create a period with a specified number of hours.
        /// </summary>
        /// <param name="hours">The amount of days in this period</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// The standard period type is used, thus you can add other fields such
        /// as months or days using the <code>withXxx()</code> methods.
        /// For example, <code>Period.hours(2).withMinutes(30);</code>
        /// </para>
        /// <para>
        /// If you want a hour-based period that cannot have other fields added,
        /// then you should consider using <see cref="Hours"/>.
        /// </para>
        /// </remarks>
        public static Period Hours(int hours)
        {
            return new Period(new int[] { 0, 0, 0, 0, hours, 0, 0, 0}, PeriodType.Standart);
        }

        /// <summary>
        /// Create a period with a specified number of minutes.
        /// </summary>
        /// <param name="minutes">The amount of minutes in this period</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// The standard period type is used, thus you can add other fields such
        /// as days or hours using the <code>withXxx()</code> methods.
        /// For example, <code>Period.minutes(2).withSeconds(30);</code>
        /// </para>
        /// <para>
        /// If you want a minute-based period that cannot have other fields added,
        /// then you should consider using <see cref="Minutes"/>.
        /// </para>
        /// </remarks>
        public static Period Minutes(int minutes)
        {
            return new Period(new int[] { 0, 0, 0, 0, 0, minutes, 0, 0}, PeriodType.Standart);
        }

        /// <summary>
        /// Create a period with a specified number of seconds.
        /// </summary>
        /// <param name="seconds">The amount of minutes in this period</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// The standard period type is used, thus you can add other fields such
        /// as days or hours using the <code>withXxx()</code> methods.
        /// For example, <code>Period.seconds(2).withMillis(30);</code>
        /// </para>
        /// <para>
        /// If you want a second-based period that cannot have other fields added,
        /// then you should consider using <see cref="Seconds"/>.
        /// </para>
        /// </remarks>
        public static Period Seconds(int seconds)
        {
            return new Period(new int[] { 0, 0, 0, 0, 0, 0, seconds, 0}, PeriodType.Standart);
        }

        /// <summary>
        /// Create a period with a specified number of seconds.
        /// </summary>
        /// <param name="seconds">The amount of minutes in this period</param>
        /// <returns>The period</returns>
        /// <remarks>
        /// <para>
        /// The standard period type is used, thus you can add other fields such
        /// as days or hours using the <code>withXxx()</code> methods.
        /// For example, <code>Period.seconds(2).withMillis(30);</code>
        /// </para>
        /// <para>
        /// If you want a second-based period that cannot have other fields added,
        /// then you should consider using <see cref="Seconds"/>.
        /// </para>
        /// </remarks>
        public static Period Milliseconds(int milliseconds)
        {
            return new Period(new int[] { 0, 0, 0, 0, 0, 0, 0, milliseconds}, PeriodType.Standart);
        }

        #endregion

        #region Construction

        private Period(int[] values, PeriodType periodType)
            : base(values, periodType) { }


        /// <summary>
        /// Creates a new empty period with the standard set of fields.
        /// </summary>
        /// <remarks>
        /// One way to initialise a period is as follows:
        /// <code>
        /// Period = new Period().WithYears(6).WithMonths(3).WithSeconds(23);
        /// </code>
        /// Bear in mind that this creates four period instances in total, three of
        /// which are immediately discarded.
        /// The alternative is more efficient, but less readable:
        /// <code>
        /// Period = new Period(6, 3, 0, 0, 0, 0, 23, 0);
        /// </code>
        /// The following is also slightly less wasteful:
        /// <code>
        /// Period = Period.Years(6).WithMonths(3).WithSeconds(23);
        /// </code>
        /// </remarks>
        public Period()
            :base(Duration.Zero, null, null)
        {
        }

        /// <summary>
        /// Create a period from a set of field values.
        /// </summary>
        /// <param name="years">Amount of years in this period, which must be zero if unsupported</param>
        /// <param name="months">Amount of months in this period, which must be zero if unsupported</param>
        /// <param name="weeks">Amount of weeks in this period, which must be zero if unsupported</param>
        /// <param name="days">Amount of days in this period, which must be zero if unsupported</param>
        /// <param name="hours">Amount of hours in this period, which must be zero if unsupported</param>
        /// <param name="minutes">Amount of minutes in this period, which must be zero if unsupported</param>
        /// <param name="seconds">Amount of seconds in this period, which must be zero if unsupported</param>
        /// <param name="milliseconds">Amount of milliseconds in this period, which must be zero if unsupported</param>
        /// <param name="periodType">Which set of fields this period supports, null means AllType</param>
        /// <remarks>
        /// There is usually little need to use this constructor.
        /// The period type is used primarily to define how to split an interval into a period.
        /// As this constructor already is split, the period type does no real work.
        /// </remarks>
        /// <exception cref="ArgumentException">If an unsupported field's value is non-zero</exception>
        public Period(
            int years, int months, int weeks, int days,
            int hours, int minutes, int seconds, int milliseconds,
            PeriodType periodType)
            : base(years, months, weeks, days, hours, minutes, seconds, milliseconds, periodType)
        {
        }

        /// <summary>
        /// Create a period from a set of field values using the standard set of fields.
        /// </summary>
        /// <param name="years">Amount of years in this period</param>
        /// <param name="months">Amount of months in this period</param>
        /// <param name="weeks">Amount of weeks in this period</param>
        /// <param name="days">Amount of days in this period</param>
        /// <param name="hours">Amount of hours in this period</param>
        /// <param name="minutes">Amount of minutes in this period</param>
        /// <param name="seconds">Amount of seconds in this period</param>
        /// <param name="millis">Amount of milliseconds in this period</param>
        public Period(
            int years, int months, int weeks, int days,
            int hours, int minutes, int seconds, int millis)
            :base(years, months, weeks, days, hours, minutes, seconds, millis, PeriodType.Standart)
        {
        }

        /// <summary>
        /// Create a period from a set of field values using the standard set of fields.
        /// </summary>
        /// <param name="hours">Amount of hours in this period</param>
        /// <param name="minutes">Amount of minutes in this period</param>
        /// <param name="seconds">Amount of seconds in this period</param>
        /// <param name="millis">Amount of milliseconds in this period</param>
        /// <remarks>
        /// Note that the parameters specify the time fields hours, minutes,
        /// seconds and millis, not the date fields.
        /// </remarks>
        public Period(int hours, int minutes, int seconds, int millis)
            : base(0, 0, 0, 0, hours, minutes, seconds, millis, PeriodType.Standart)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the years field part of the period.
        /// </summary>
        /// <returns>The number of years in the period, zero if unsupported</returns>
        public int Years()
        {
            return GetIndexedField(PeriodType.Index.Year);
        }

        /// <summary>
        /// Gets the months field part of the period.
        /// </summary>
        /// <returns>The number of months in the period, zero if unsupported</returns>
        public int Months()
        {
            return GetIndexedField(PeriodType.Index.Month);
        }

        /// <summary>
        /// Gets the weeks field part of the period.
        /// </summary>
        /// <returns>The number of weeks in the period, zero if unsupported</returns>
        public int Weeks()
        {
            return GetIndexedField(PeriodType.Index.Week);
        }

        /// <summary>
        /// Gets the days field part of the period.
        /// </summary>
        /// <returns>The number of days in the period, zero if unsupported</returns>
        public int Days()
        {
            return GetIndexedField(PeriodType.Index.Day);
        }

        /// <summary>
        /// Gets the hours field part of the period.
        /// </summary>
        /// <returns>The number of hours in the period, zero if unsupported</returns>
        public int Hours()
        {
            return GetIndexedField(PeriodType.Index.Hour);
        }

        /// <summary>
        /// Gets the minutes field part of the period.
        /// </summary>
        /// <returns>The number of minutes in the period, zero if unsupported</returns>
        public int Minutes()
        {
            return GetIndexedField(PeriodType.Index.Minute);
        }

        /// <summary>
        /// Gets the seconds field part of the period.
        /// </summary>
        /// <returns>The number of seconds in the period, zero if unsupported</returns>
        public int Seconds()
        {
            return GetIndexedField(PeriodType.Index.Second);
        }

        /// <summary>
        /// Gets the milliseconds field part of the period.
        /// </summary>
        /// <returns>The number of milliseconds in the period, zero if unsupported</returns>
        public int Milliseconds()
        {
            return GetIndexedField(PeriodType.Index.Millisecond);
        }

        private int GetIndexedField(PeriodType.Index index)
        {
            int realIndex = PeriodType.GetRealIndex(index);
            return GetValue(realIndex);
        }

        #endregion
    }
}