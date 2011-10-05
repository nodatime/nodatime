#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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
using System.Collections;
using System.Collections.Generic;
using NodaTime.Fields;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// Represents a period of time expressed in human chronological terms: hours, days,
    /// weeks, months and so on. All implementations in Noda Time are immutable, and return fields
    /// in descending size order: hours before minutes, for example.
    /// </summary>
    /// <remarks>
    /// Periods operate on calendar-related types such as
    /// <see cref="LocalDateTime" /> whereas <see cref="Duration"/> operates on instants
    /// on the time line. Although <see cref="ZonedDateTime" /> includes both concepts, it is generally
    /// simpler to consider period-based arithmetic solely on local dates and times, so only
    /// duration-based arithmetic is supported on ZonedDateTime. This avoids ambiguities
    /// and skipped date/time values becoming a problem within a series of calculations; instead,
    /// these can be considered just once, at the point of conversion to a ZonedDateTime.
    /// </remarks>
    public sealed class Period : IEnumerable<DurationFieldValue>, IEquatable<Period>
    {
        private readonly PeriodType periodType;
        private readonly long[] values;

        /// <summary>
        /// Creates a new period with the given array without copying it. The array contents must
        /// not be changed after the value has been constructed - which is why this method is private.
        /// </summary>
        /// <param name="periodType">Type of this period, describing which fields are present</param>
        /// <param name="values">Values for each field in the period type</param>
        private Period(PeriodType periodType, long[] values)
        {
            this.values = values;
            this.periodType = periodType;
        }

        /// <summary>
        /// Returns the type of this period, which describes the fields within this period.
        /// </summary>
        public PeriodType PeriodType { get { return periodType; } }

        private static Period CreateSingleFieldPeriod(PeriodType periodType, long value)
        {
            long[] values = { value };
            return new Period(periodType, values);
        }

        /// <summary>
        /// Creates a period representing the specified number of years.
        /// </summary>
        /// <param name="years">The number of years in the new period</param>
        /// <returns>A period consisting of the given number of years.</returns>
        public static Period FromYears(long years)
        {
            return CreateSingleFieldPeriod(PeriodType.Years, years);
        }

        /// <summary>
        /// Creates a period representing the specified number of months.
        /// </summary>
        /// <param name="months">The number of months in the new period</param>
        /// <returns>A period consisting of the given number of months.</returns>
        public static Period FromMonths(long months)
        {
            return CreateSingleFieldPeriod(PeriodType.Months, months);
        }

        /// <summary>
        /// Creates a period representing the specified number of days.
        /// </summary>
        /// <param name="days">The number of days in the new period</param>
        /// <returns>A period consisting of the given number of days.</returns>
        public static Period FromDays(long days)
        {
            return CreateSingleFieldPeriod(PeriodType.Days, days);
        }

        /// <summary>
        /// Creates a period representing the specified number of hours.
        /// </summary>
        /// <param name="hours">The number of hours in the new period</param>
        /// <returns>A period consisting of the given number of hours.</returns>
        public static Period FromHours(long hours)
        {
            return CreateSingleFieldPeriod(PeriodType.Hours, hours);
        }

        /// <summary>
        /// Creates a period representing the specified number of minutes.
        /// </summary>
        /// <param name="minutes">The number of minutes in the new period</param>
        /// <returns>A period consisting of the given number of minutes.</returns>
        public static Period FromMinutes(long minutes)
        {
            return CreateSingleFieldPeriod(PeriodType.Minutes, minutes);
        }

        /// <summary>
        /// Creates a period representing the specified number of seconds.
        /// </summary>
        /// <param name="seconds">The number of seconds in the new period</param>
        /// <returns>A period consisting of the given number of seconds.</returns>
        public static Period FromSeconds(long seconds)
        {
            return CreateSingleFieldPeriod(PeriodType.Seconds, seconds);
        }

        /// <summary>
        /// Creates a period representing the specified number of miliseconds.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds in the new period</param>
        /// <returns>A period consisting of the given number of milliseconds.</returns>
        public static Period FromMillseconds(long milliseconds)
        {
            return CreateSingleFieldPeriod(PeriodType.Milliseconds, milliseconds);
        }

        /// <summary>
        /// Creates a period representing the specified number of ticks.
        /// </summary>
        /// <param name="ticks">The number of ticks in the new period</param>
        /// <returns>A period consisting of the given number of ticks.</returns>
        public static Period FromTicks(long ticks)
        {
            return CreateSingleFieldPeriod(PeriodType.Ticks, ticks);
        }

        /// <summary>
        /// Adds two periods together, by simply adding the values for each field. Currently this
        /// returns a period with a period type of "all fields".
        /// </summary>
        /// <param name="left">The first period to add</param>
        /// <param name="right">The second period to add</param>
        /// <returns>The sum of the two periods.</returns>
        public static Period operator +(Period left, Period right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }
            PeriodType all = PeriodType.AllFields;
            long[] newValues = new long[all.Size];
            // TODO: Make this a lot faster :)
            for (int i = 0; i < all.Size; i++)
            {
                DurationFieldType fieldType = all[i];
                newValues[i] = left[fieldType] + right[fieldType];
            }
            return new Period(PeriodType.AllFields, newValues);
        }

        /// <summary>
        /// Subtracts one periods from another, by simply subtracting each field value. Currently this
        /// returns a period with a period type of "all fields".
        /// </summary>
        /// <param name="minuend">The period to subtract the second operand from</param>
        /// <param name="subtrahend">The period to subtract the first operand from</param>
        /// <returns>The result of subtracting all the values in the second operand from the values in the first.</returns>
        public static Period operator -(Period minuend, Period subtrahend)
        {
            if (minuend == null)
            {
                throw new ArgumentNullException("minuend");
            }
            if (subtrahend == null)
            {
                throw new ArgumentNullException("subtrahend");
            }
            PeriodType all = PeriodType.AllFields;
            long[] newValues = new long[all.Size];
            // TODO: Make this a lot faster :)
            for (int i = 0; i < all.Size; i++)
            {
                DurationFieldType fieldType = all[i];
                newValues[i] = minuend[fieldType] - subtrahend[fieldType];
            }
            return new Period(PeriodType.AllFields, newValues);
        }

        /// <summary>
        /// Returns the period between a start and an end date/time, using the set of fields in the given
        /// period type.
        /// </summary>
        /// <remarks>
        /// If <paramref name="end"/> is before <paramref name="start" />, each field in the returned period
        /// will be negative. If the given period type cannot exactly reach the end point (e.g. finding
        /// the difference between 1am and 3:15am in hours) the result will be such that adding it to <paramref name="start"/>
        /// will give a value between <paramref name="start"/> and <paramref name="end"/>. In other words,
        /// any rounding is "towards start"; this is true whether the resulting period is negative or positive.
        /// </remarks>
        /// <param name="start">Start date/time</param>
        /// <param name="end">End date/time</param>
        /// <param name="periodType">Period type to use for calculations</param>
        /// <exception cref="ArgumentException"><paramref name="start"/> and <paramref name="end"/> use different calendars</exception>
        /// <exception cref="ArgumentNullException"><paramref name="periodType"/> is null</exception>
        /// <returns>The period between the given date/times</returns>
        public static Period Between(LocalDateTime start, LocalDateTime end, PeriodType periodType)
        {
            if (periodType == null)
            {
                throw new ArgumentNullException("periodType");
            }
            if (!start.Calendar.Equals(end.Calendar))
            {
                throw new ArgumentException("start and end must use the same calendar system");
            }
            long[] values = start.Calendar.GetPeriodValues(start.LocalInstant, end.LocalInstant, periodType);
            return new Period(periodType, values);
        }

        /// <summary>
        /// Returns the difference between two date/times using the "all fields" period type.
        /// </summary>
        /// <param name="start">Start date/time</param>
        /// <param name="end">End date/time</param>
        /// <returns>The period between the two date and time values, using all period fields.</returns>
        public static Period Between(LocalDateTime start, LocalDateTime end)
        {
            return Between(start, end, PeriodType.AllFields);
        }

        /// <summary>
        /// Returns the period between a start and an end date, using the set of fields in the given
        /// period type.
        /// </summary>
        /// <remarks>
        /// If <paramref name="end"/> is before <paramref name="start" />, each field in the returned period
        /// will be negative. If the given period type cannot exactly reach the end point (e.g. finding
        /// the difference between 12th February and 15th March in months) the result will be such that adding it to <paramref name="start"/>
        /// will give a value between <paramref name="start"/> and <paramref name="end"/>. In other words,
        /// any rounding is "towards start"; this is true whether the resulting period is negative or positive.
        /// </remarks>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <param name="periodType">Period type to use for calculations</param>
        /// <exception cref="ArgumentException"><paramref name="periodType"/> contains time fields</exception>
        /// <exception cref="ArgumentException"><paramref name="start"/> and <paramref name="end"/> use different calendars</exception>
        /// <exception cref="ArgumentNullException"><paramref name="periodType"/> is null</exception>
        /// <returns>The period between the given dates using the specified period type</returns>
        public static Period Between(LocalDate start, LocalDate end, PeriodType periodType)
        {
            return Between(start.LocalDateTime, end.LocalDateTime, periodType);
        }

        /// <summary>
        /// Returns the difference between two dates using the "year month day" period type.
        /// </summary>
        /// <param name="start">Start date</param>
        /// <param name="end">End date</param>
        /// <returns>The period between the two dates, using year, month and day fields.</returns>
        public static Period Between(LocalDate start, LocalDate end)
        {
            return Between(start.LocalDateTime, end.LocalDateTime, PeriodType.YearMonthDay);
        }

        /// <summary>
        /// Returns the period between a start and an end time, using the set of fields in the given
        /// period type.
        /// </summary>
        /// <remarks>
        /// If <paramref name="end"/> is before <paramref name="start" />, each field in the returned period
        /// will be negative. If the given period type cannot exactly reach the end point (e.g. finding
        /// the difference between 3am and 4.30am in hours) the result will be such that adding it to <paramref name="start"/>
        /// will give a value between <paramref name="start"/> and <paramref name="end"/>. In other words,
        /// any rounding is "towards start"; this is true whether the resulting period is negative or positive.
        /// </remarks>
        /// <param name="start">Start time</param>
        /// <param name="end">End time</param>
        /// <param name="periodType">Period type to use for calculations</param>
        /// <exception cref="ArgumentException"><paramref name="periodType"/> contains time fields</exception>
        /// <exception cref="ArgumentException"><paramref name="start"/> and <paramref name="end"/> use different calendars</exception>
        /// <exception cref="ArgumentNullException"><paramref name="periodType"/> is null</exception>
        /// <returns>The period between the given times</returns>
        public static Period Between(LocalTime start, LocalTime end, PeriodType periodType)
        {
            return Between(start.LocalDateTime, end.LocalDateTime, periodType);
        }

        /// <summary>
        /// Returns the difference between two dates using the "time" period type.
        /// </summary>
        /// <param name="start">Start time</param>
        /// <param name="end">End time</param>
        /// <returns>The period between the two times, using the "time" period fields.</returns>
        public static Period Between(LocalTime start, LocalTime end)
        {
            return Between(start.LocalDateTime, end.LocalDateTime, PeriodType.Time);
        }

        /// <summary>
        /// Returns the fields and values within this period.
        /// </summary>
        /// <returns>The fields and values within this period.</returns>
        public IEnumerator<DurationFieldValue> GetEnumerator()
        {
            for (int i = 0; i < values.Length; i++)
            {
                yield return new DurationFieldValue(periodType[i], values[i]);
            }
        }

        /// <summary>
        /// Returns the fields and values within this period.
        /// </summary>
        /// <returns>The fields and values within this period.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns the value of the given field within this period. If the period does not contain
        /// the given field, 0 is returned.
        /// </summary>
        /// <param name="fieldType">The type of field to fetch the value of.</param>
        /// <returns>The value of the given field within this period, or 0 if this period does not contain the given field.</returns>
        public long this[DurationFieldType fieldType]
        {
            get
            {
                int index = periodType.IndexOf(fieldType);
                return index == -1 ? 0 : values[index];
            }
        }

        #region Helper properties
        /// <summary>
        /// Gets the number of years within this period.
        /// </summary>
        public long Years { get { return this[DurationFieldType.Years]; } }
        /// <summary>
        /// Gets the number of months within this period.
        /// </summary>
        public long Months { get { return this[DurationFieldType.Months]; } }
        /// <summary>
        /// Gets the number of weeks within this period.
        /// </summary>
        public long Weeks { get { return this[DurationFieldType.Weeks]; } }
        /// <summary>
        /// Gets the number of days within this period.
        /// </summary>
        public long Days { get { return this[DurationFieldType.Days]; } }
        /// <summary>
        /// Gets the number of hours within this period.
        /// </summary>
        public long Hours { get { return this[DurationFieldType.Hours]; } }
        /// <summary>
        /// Gets the number of minutes within this period.
        /// </summary>
        public long Minutes { get { return this[DurationFieldType.Minutes]; } }
        /// <summary>
        /// Gets the number of seconds within this period.
        /// </summary>
        public long Seconds { get { return this[DurationFieldType.Seconds]; } }
        /// <summary>
        /// Gets the number of milliseconds within this period.
        /// </summary>
        public long Milliseconds { get { return this[DurationFieldType.Milliseconds]; } }
        /// <summary>
        /// Gets the number of ticks within this period.
        /// </summary>
        public long Ticks { get { return this[DurationFieldType.Ticks]; } }
        #endregion

        #region Object overrides
        /// <summary>
        /// Compares the given object for equality with this one, as per <see cref="Equals(Period)"/>.
        /// </summary>
        /// <param name="other">The value to compare this one with.</param>
        /// <returns>true if the other object is a period equal to this one, consistent with <see cref="Equals(Period)"/></returns>
        public override bool Equals(object other)
        {
            return Equals(other as Period);
        }

        /// <summary>
        /// Returns the hash code for this period, consistent with <see cref="Equals(Period)"/>.
        /// </summary>
        /// <returns>The hash code for this period.</returns>
        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            // TODO: Make this a lot faster :)
            foreach (DurationFieldType fieldType in PeriodType.AllFields)
            {
                hash = HashCodeHelper.Hash(hash, this[fieldType]);
            }
            return hash;
        }

        /// <summary>
        /// Compares the given period for equality with this one.
        /// </summary>
        /// <remarks>
        /// Periods are equal if they contain the same values for the same fields, regardless of period type
        /// - so a period of "one hour" is the same whether or not it's potentially got other fields with
        /// a zero value. However, no normalization takes place, so "one hour" is not equal to "sixty minutes".
        /// </remarks>
        /// <param name="other">The period to compare this one with.</param>
        /// <returns>True if this period has the same values for the same fields as the one specified.</returns>
        public bool Equals(Period other)
        {
            if (other == null)
            {
                return false;
            }

            // TODO: Make this a lot faster :)
            foreach (DurationFieldType fieldType in PeriodType.AllFields)
            {
                if (this[fieldType] != other[fieldType])
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
