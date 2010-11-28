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
using NodaTime.Format;
using NodaTime.Utility;

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
    /// <para>
    /// TODO: Consider making WithDays etc work with *any* period instead of just those
    /// defined with a standard period type. This should just be a matter of checking whether
    /// the field is supported, updating it if so, and constructing a new period type and values otherwise.
    /// </para>
    /// </remarks>
    public sealed partial class Period : IPeriod, IEquatable<Period>
    {
        private readonly PeriodType periodType;
        private readonly int[] fieldValues;

        #region Construction
        internal Period(int[] values, PeriodType periodType)
        {
            this.periodType = periodType;
            fieldValues = values;
        }

        /// <summary>
        /// Initializes a new empty period with the standard set of fields.
        /// </summary>
        private Period() : this(new[] { 0, 0, 0, 0, 0, 0, 0, 0 }, PeriodType.Standard)
        {
        }

        /// <summary>
        /// Initializes a period from a set of field values.
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
        /// <exception cref="NotSupportedException">If an unsupported field's value is non-zero</exception>
        public Period(int years, int months, int weeks, int days, int hours, int minutes, int seconds, int milliseconds, PeriodType periodType)
        {
            this.periodType = periodType ?? PeriodType.Standard;

            int[] newValues = new int[Size];
            PeriodType.UpdateIndexedField(newValues, PeriodType.Index.Year, years, false);
            PeriodType.UpdateIndexedField(newValues, PeriodType.Index.Month, months, false);
            PeriodType.UpdateIndexedField(newValues, PeriodType.Index.Week, weeks, false);
            PeriodType.UpdateIndexedField(newValues, PeriodType.Index.Day, days, false);
            PeriodType.UpdateIndexedField(newValues, PeriodType.Index.Hour, hours, false);
            PeriodType.UpdateIndexedField(newValues, PeriodType.Index.Minute, minutes, false);
            PeriodType.UpdateIndexedField(newValues, PeriodType.Index.Second, seconds, false);
            PeriodType.UpdateIndexedField(newValues, PeriodType.Index.Millisecond, milliseconds, false);
            fieldValues = newValues;
        }

        /// <summary>
        /// Initializes a period from a set of field values using the standard set of fields.
        /// </summary>
        /// <param name="years">Amount of years in this period</param>
        /// <param name="months">Amount of months in this period</param>
        /// <param name="weeks">Amount of weeks in this period</param>
        /// <param name="days">Amount of days in this period</param>
        /// <param name="hours">Amount of hours in this period</param>
        /// <param name="minutes">Amount of minutes in this period</param>
        /// <param name="seconds">Amount of seconds in this period</param>
        /// <param name="millis">Amount of milliseconds in this period</param>
        public Period(int years, int months, int weeks, int days, int hours, int minutes, int seconds, int millis)
            : this(years, months, weeks, days, hours, minutes, seconds, millis, PeriodType.Standard)
        {
        }

        /// <summary>
        /// Initializes a period from a set of field values using the standard set of fields.
        /// </summary>
        /// <param name="hours">Amount of hours in this period</param>
        /// <param name="minutes">Amount of minutes in this period</param>
        /// <param name="seconds">Amount of seconds in this period</param>
        /// <param name="millis">Amount of milliseconds in this period</param>
        /// <remarks>
        /// Note that the parameters specify the time fields hours, minutes,
        /// seconds and millis, not the date fields.
        /// </remarks>
        public Period(int hours, int minutes, int seconds, int millis) : this(0, 0, 0, 0, hours, minutes, seconds, millis, PeriodType.Standard)
        {
        }
        #endregion

        #region IPeriod Members
        /// <summary>
        /// Gets the period type that defines which fields are included in the period.
        /// </summary>
        public PeriodType PeriodType { get { return periodType; } }

        /// <summary>
        /// Gets the number of fields this period supports.
        /// </summary>
        public int Size { get { return periodType.Size; } }

        /// <summary>
        /// Gets the field type at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get</param>
        /// <returns>The field at the specified index</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">if the index is invalid</exception>
        public DurationFieldType GetFieldType(int index)
        {
            return periodType.GetFieldType(index);
        }

        /// <summary>
        /// Checks whether the field type specified is supported by this period.
        /// </summary>
        /// <param name="field">The field to check, may be null which returns false</param>
        /// <returns>True if the field is supported</returns>
        public bool IsSupported(DurationFieldType field)
        {
            return PeriodType.IsSupported(field);
        }

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get</param>
        /// <returns>The value of the field at the specified index</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">if <c>index &lt; 0 || Size &lt;= index</c></exception>
        public int this[int index] { get { return fieldValues[index]; } }

        /// <summary>
        /// Gets the value of one of the fields.
        /// </summary>
        /// <param name="field">The field type to query, null return zero</param>
        /// <returns>The value of that field, zero if field not supported</returns>
        /// <remarks>
        /// If the field type specified is not supported by the period then zero is returned.
        /// </remarks>
        public int this[DurationFieldType field]
        {
            get
            {
                int index = PeriodType.IndexOf(field);
                if (index == -1)
                {
                    return 0;
                }
                return this[index];
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the years field part of the period.
        /// </summary>
        /// <returns>The number of years in the period, zero if unsupported</returns>
        public int Years { get { return GetIndexedField(PeriodType.Index.Year); } }

        /// <summary>
        /// Gets the months field part of the period.
        /// </summary>
        /// <returns>The number of months in the period, zero if unsupported</returns>
        public int Months { get { return GetIndexedField(PeriodType.Index.Month); } }

        /// <summary>
        /// Gets the weeks field part of the period.
        /// </summary>
        /// <returns>The number of weeks in the period, zero if unsupported</returns>
        public int Weeks { get { return GetIndexedField(PeriodType.Index.Week); } }

        /// <summary>
        /// Gets the days field part of the period.
        /// </summary>
        /// <returns>The number of days in the period, zero if unsupported</returns>
        public int Days { get { return GetIndexedField(PeriodType.Index.Day); } }

        /// <summary>
        /// Gets the hours field part of the period.
        /// </summary>
        /// <returns>The number of hours in the period, zero if unsupported</returns>
        public int Hours { get { return GetIndexedField(PeriodType.Index.Hour); } }

        /// <summary>
        /// Gets the minutes field part of the period.
        /// </summary>
        /// <returns>The number of minutes in the period, zero if unsupported</returns>
        public int Minutes { get { return GetIndexedField(PeriodType.Index.Minute); } }

        /// <summary>
        /// Gets the seconds field part of the period.
        /// </summary>
        /// <returns>The number of seconds in the period, zero if unsupported</returns>
        public int Seconds { get { return GetIndexedField(PeriodType.Index.Second); } }

        /// <summary>
        /// Gets the milliseconds field part of the period.
        /// </summary>
        /// <returns>The number of milliseconds in the period, zero if unsupported</returns>
        public int Milliseconds { get { return GetIndexedField(PeriodType.Index.Millisecond); } }

        private int GetIndexedField(PeriodType.Index index)
        {
            int realIndex = PeriodType.GetRealIndex(index);
            return realIndex == -1 ? 0 : this[realIndex];
        }
        #endregion

        #region Creation methods
        private int[] WithIndexedField(PeriodType.Index index, int newValue)
        {
            return UpdateIndexedField(index, newValue, false);
        }

        private int[] UpdateIndexedField(PeriodType.Index index, int newValue, bool add)
        {
            var values = CloneValues();

            //change field value
            PeriodType.UpdateIndexedField(values, index, newValue, add);

            return values;
        }

        private int[] UpdateAnyField(DurationFieldType fieldType, int newValue, bool add)
        {
            var values = CloneValues();

            PeriodType.UpdateAnyField(values, fieldType, newValue, add);

            return values;
        }

        private int[] UpdateAllFields(IPeriod period, bool add)
        {
            var values = CloneValues();

            for (int i = 0, isize = period.Size; i < isize; i++)
            {
                DurationFieldType type = period.GetFieldType(i);
                int value = period[i];
                PeriodType.UpdateAnyField(values, type, value, add);
            }
            return values;
        }

        /// <summary>
        /// Creates a new Period instance with the fields from the specified period
        /// copied on top of those from this period.
        /// </summary>
        /// <param name="period">The period to copy from, null ignored</param>
        /// <returns>The new period instance</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If some of the fields type is unsupported</exception>
        public Period With(IPeriod period)
        {
            if (period == null)
            {
                return this;
            }

            var values = UpdateAllFields(period, false);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Creates a new Period instance with the specified field set to a new value.
        /// </summary>
        /// <param name="fieldType">The field to set to</param>
        /// <param name="value">The value to set</param>
        /// <returns>The new period instance</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the field type is unsupported</exception>
        public Period WithField(DurationFieldType fieldType, int value)
        {
            var values = UpdateAnyField(fieldType, value, false);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of years.
        /// </summary>
        /// <param name="years">The amount of years to set, may be negative</param>
        /// <returns>The new period with the changed years</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the years field type is unsupported</exception>
        public Period WithYears(int years)
        {
            var values = WithIndexedField(PeriodType.Index.Year, years);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of months.
        /// </summary>
        /// <param name="months">The amount of months to set, may be negative</param>
        /// <returns>The new period with the changed months</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the months field type is unsupported</exception>
        public Period WithMonths(int months)
        {
            var values = WithIndexedField(PeriodType.Index.Month, months);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of weeks.
        /// </summary>
        /// <param name="weeks">The amount of weeks to set, may be negative</param>
        /// <returns>The new period with the changed weeks</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the weeks field type is unsupported</exception>
        public Period WithWeeks(int weeks)
        {
            var values = WithIndexedField(PeriodType.Index.Week, weeks);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of days.
        /// </summary>
        /// <param name="days">The amount of days to set, may be negative</param>
        /// <returns>The new period with the changed days</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the days field type is unsupported</exception>
        public Period WithDays(int days)
        {
            var values = WithIndexedField(PeriodType.Index.Day, days);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of hours.
        /// </summary>
        /// <param name="hours">The amount of hours to set, may be negative</param>
        /// <returns>The new period with the changed hours</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the hours field type is unsupported</exception>
        public Period WithHours(int hours)
        {
            var values = WithIndexedField(PeriodType.Index.Hour, hours);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of minutes.
        /// </summary>
        /// <param name="minutes">The amount of minutes to set, may be negative</param>
        /// <returns>The new period with the changed minutes</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the minutes field type is unsupported</exception>
        public Period WithMinutes(int minutes)
        {
            var values = WithIndexedField(PeriodType.Index.Minute, minutes);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of seconds.
        /// </summary>
        /// <param name="seconds">The amount of seconds to set, may be negative</param>
        /// <returns>The new period with the changed seconds</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the seconds field type is unsupported</exception>
        public Period WithSeconds(int seconds)
        {
            var values = WithIndexedField(PeriodType.Index.Second, seconds);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The amount of milliseconds to set, may be negative</param>
        /// <returns>The new period with the changed milliseconds</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the milliseconds field type is unsupported</exception>
        public Period WithMilliseconds(int milliseconds)
        {
            var values = WithIndexedField(PeriodType.Index.Millisecond, milliseconds);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified period added.
        /// </summary>
        /// <param name="period">The period to add, null adds zero and returns this</param>
        /// <returns>The new updated period</returns>
        /// <remarks>
        /// <para>
        /// Each field of the period is added separately. Thus a period of
        /// 2 hours 30 minutes plus 3 hours 40 minutes will produce a result
        /// of 5 hours 70 minutes - see <see cref="NormalizeStandard"/>.
        /// </para>
        /// <para>
        /// If the period being added contains a non-zero amount for a field that
        /// is not supported in this period then an exception is thrown.
        /// </para>
        /// <para>
        /// This period instance is immutable and unaffected by this method call.
        /// </para>
        /// </remarks>
        /// <exception cref="NotSupportedException">If some of the fields type is unsupported</exception>
        public Period Add(IPeriod period)
        {
            if (period == null)
            {
                return this;
            }

            var values = UpdateAllFields(period, true);

            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Creates a new Period instance with the new value added to the specified field
        /// </summary>
        /// <param name="fieldType">The field to add value to</param>
        /// <param name="value">The value to add</param>
        /// <returns>The new period instance</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the field type is unsupported</exception>
        public Period AddField(DurationFieldType fieldType, int value)
        {
            if (value == 0)
            {
                return this;
            }

            var values = UpdateAnyField(fieldType, value, true);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of years added.
        /// </summary>
        /// <param name="years">The amount of years to add, may be negative</param>
        /// <returns>The new period with the increased years</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the years field type is unsupported</exception>
        public Period AddYears(int years)
        {
            if (years == 0)
            {
                return this;
            }

            var values = UpdateIndexedField(PeriodType.Index.Year, years, true);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of months added.
        /// </summary>
        /// <param name="months">The amount of months to add, may be negative</param>
        /// <returns>The new period with the increased months</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the months field type is unsupported</exception>
        public Period AddMonths(int months)
        {
            if (months == 0)
            {
                return this;
            }

            var values = UpdateIndexedField(PeriodType.Index.Month, months, true);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of weeks added.
        /// </summary>
        /// <param name="weeks">The amount of weeks to add, may be negative</param>
        /// <returns>The new period with the increased weeks</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the weeks field type is unsupported</exception>
        public Period AddWeeks(int weeks)
        {
            if (weeks == 0)
            {
                return this;
            }

            var values = UpdateIndexedField(PeriodType.Index.Week, weeks, true);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of days added.
        /// </summary>
        /// <param name="days">The amount of days to add, may be negative</param>
        /// <returns>The new period with the increased days</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the days field type is unsupported</exception>
        public Period AddDays(int days)
        {
            if (days == 0)
            {
                return this;
            }

            var values = UpdateIndexedField(PeriodType.Index.Day, days, true);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of hours added.
        /// </summary>
        /// <param name="hours">The amount of hours to add, may be negative</param>
        /// <returns>The new period with the added hours</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the hours field type is unsupported</exception>
        public Period AddHours(int hours)
        {
            if (hours == 0)
            {
                return this;
            }

            var values = UpdateIndexedField(PeriodType.Index.Hour, hours, true);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of minutes added.
        /// </summary>
        /// <param name="minutes">The amount of minutes to add, may be negative</param>
        /// <returns>The new period with the increased minutes</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the minutes field type is unsupported</exception>
        public Period AddMinutes(int minutes)
        {
            if (minutes == 0)
            {
                return this;
            }

            var values = UpdateIndexedField(PeriodType.Index.Minute, minutes, true);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of seconds added.
        /// </summary>
        /// <param name="seconds">The amount of seconds to add, may be negative</param>
        /// <returns>The new period with the increased seconds</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the seconds field type is unsupported</exception>
        public Period AddSeconds(int seconds)
        {
            if (seconds == 0)
            {
                return this;
            }

            var values = UpdateIndexedField(PeriodType.Index.Second, seconds, true);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of milliseconds added.
        /// </summary>
        /// <param name="milliseconds">The amount of milliseconds to add, may be negative</param>
        /// <returns>The new period with the increased milliseconds</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the milliseconds field type is unsupported</exception>       
        public Period AddMilliseconds(int milliseconds)
        {
            if (milliseconds == 0)
            {
                return this;
            }

            var values = UpdateIndexedField(PeriodType.Index.Millisecond, milliseconds, true);
            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified period subtracted.
        /// </summary>
        /// <param name="period">The period to subtract, null subtracts zero and returns this</param>
        /// <returns>The new updated period</returns>
        /// <remarks>
        /// <para>
        /// Each field of the period is subtracted separately. Thus a period of
        /// 3 hours 30 minutes minus 2 hours 40 minutes will produce a result
        /// of 1 hour and -10 minutes - see <see cref="NormalizeStandard"/>.
        /// </para>
        /// <para>
        /// If the period being subtracted contains a non-zero amount for a field that
        /// is not supported in this period then an exception is thrown.
        /// </para>
        /// <para>
        /// This period instance is immutable and unaffected by this method call.
        /// </para>
        /// </remarks>
        /// <exception cref="NotSupportedException">If some of the fields type is unsupported</exception>
        public Period Subtract(IPeriod period)
        {
            if (period == null)
            {
                return this;
            }

            var values = UpdateAllFields(NegateImpl(period), true);

            return new Period(values, PeriodType);
        }

        /// <summary>
        /// Returns a new period with the specified number of years taken away.
        /// </summary>
        /// <param name="years">The amount of years to take away, may be negative</param>
        /// <returns>The new period with the decreased years</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the years field type is unsupported</exception>        
        public Period SubtractYears(int years)
        {
            return AddYears(-years);
        }

        /// <summary>
        /// Returns a new period with the specified number of months taken away.
        /// </summary>
        /// <param name="months">The amount of months to take away, may be negative</param>
        /// <returns>The new period with the decreased months</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the months field type is unsupported</exception>
        public Period SubtractMonths(int months)
        {
            return AddMonths(-months);
        }

        /// <summary>
        /// Returns a new period with the specified number of weeks taken away.
        /// </summary>
        /// <param name="weeks">The amount of weeks to take away, may be negative</param>
        /// <returns>The new period with the decreased weeks</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the weeks field type is unsupported</exception>
        public Period SubtractWeeks(int weeks)
        {
            return AddWeeks(-weeks);
        }

        /// <summary>
        /// Returns a new period with the specified number of days taken away.
        /// </summary>
        /// <param name="days">The amount of days to take away, may be negative</param>
        /// <returns>The new period with the decreased days</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the days field type is unsupported</exception>
        public Period SubtractDays(int days)
        {
            return AddDays(-days);
        }

        /// <summary>
        /// Returns a new period with the specified number of hours taken away.
        /// </summary>
        /// <param name="hours">The amount of hours to take away, may be negative</param>
        /// <returns>The new period with the decreased hours</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the hours field type is unsupported</exception>
        public Period SubtractHours(int hours)
        {
            return AddHours(-hours);
        }

        /// <summary>
        /// Returns a new period with the specified number of minutes taken away.
        /// </summary>
        /// <param name="minutes">The amount of minutes to take away, may be negative</param>
        /// <returns>The new period with the decreased minutes</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the minutes field type is unsupported</exception>
        public Period SubtractMinutes(int minutes)
        {
            return AddMinutes(-minutes);
        }

        /// <summary>
        /// Returns a new period with the specified number of seconds taken away.
        /// </summary>
        /// <param name="seconds">The amount of seconds to take away, may be negative</param>
        /// <returns>The new period with the decreased seconds</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the seconds field type is unsupported</exception>
        public Period SubtractSeconds(int seconds)
        {
            return AddSeconds(-seconds);
        }

        /// <summary>
        /// Returns a new period with the specified number of milliseconds taken away.
        /// </summary>
        /// <param name="milliseconds">The amount of milliseconds to take away, may be negative</param>
        /// <returns>The new period with the decreased milliseconds</returns>
        /// <remarks>
        /// This period instance is immutable and unaffected by this method call.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the milliseconds field type is unsupported</exception>        
        public Period SubtractMilliseconds(int milliseconds)
        {
            return AddMilliseconds(-milliseconds);
        }
        #endregion

        #region Normalization
        private void VerifyAbsenceOfYearsAndMonths(string destinationType)
        {
            if (Years != 0)
            {
                throw new NotSupportedException("Cannot convert to " + destinationType + " as this period contains years and years vary in length");
            }
            else if (Months != 0)
            {
                throw new NotSupportedException("Cannot convert to " + destinationType + " as this period contains months and months vary in length");
            }
        }

        private Duration ToStandardDurationUnchecked()
        {
            return Duration.FromStandardWeeks(Weeks) + Duration.FromStandardDays(Days) + Duration.FromHours(Hours) + Duration.FromMinutes(Minutes) +
                   Duration.FromSeconds(Seconds) + Duration.FromMilliseconds(Milliseconds);
        }

        /// <summary>
        /// Converts this period to a duration assuming a
        /// 7 day week, 24 hour day, 60 minute hour and 60 second minute.
        /// </summary>
        /// <returns>A duration equivalent to this period</returns>
        /// <remarks>
        /// This method allows you to convert from a period to a duration.
        /// However to achieve this it makes the assumption that all
        /// weeks are 7 days, all days are 24 hours, all hours are 60 minutes and
        /// all minutes are 60 seconds. This is not true when daylight savings time
        /// is considered, and may also not be true for some unusual chronologies.
        /// However, it is included as it is a useful operation for many
        /// applications and business rules.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the period contains years or months</exception>
        public Duration ToStandardDuration()
        {
            VerifyAbsenceOfYearsAndMonths("Duration");

            return ToStandardDurationUnchecked();
        }

        /*
        /// <summary>
        /// Converts this period to a period in seconds assuming a
        /// 7 day week, 24 hour day, 60 minute hour and 60 second minute.
        /// </summary>
        /// <returns>A period representing the number of standard seconds in this period</returns>
        /// <remarks>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all
        /// weeks are 7 days, all days are 24 hours, all hours are 60 minutes and
        /// all minutes are 60 seconds. This is not true when daylight savings time
        /// is considered, and may also not be true for some unusual chronologies.
        /// However, it is included as it is a useful operation for many
        /// applications and business rules.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the period contains years or months</exception>
        public Seconds ToStandardSeconds()
        {
            VerifyAbsenceOfYearsAndMonths("Seconds");

            var seconds = ToStandardDuration().Ticks / NodaConstants.TicksPerSecond;

            return Periods.Seconds.From((int)seconds);
        }

        /// <summary>
        /// Converts this period to a period in minutes assuming a
        /// 7 day week, 24 hour day, 60 minute hour and 60 second minute.
        /// </summary>
        /// <returns>A period representing the number of standard minutes in this period</returns>
        /// <remarks>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all
        /// weeks are 7 days, all days are 24 hours, all hours are 60 minutes and
        /// all minutes are 60 seconds. This is not true when daylight savings time
        /// is considered, and may also not be true for some unusual chronologies.
        /// However, it is included as it is a useful operation for many
        /// applications and business rules.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the period contains years or months</exception>
        public Minutes ToStandardMinutes()
        {
            VerifyAbsenceOfYearsAndMonths("Minutes");

            var minutes = ToStandardDuration().Ticks / NodaConstants.TicksPerMinute;

            return Periods.Minutes.From((int)minutes);
        }

        /// <summary>
        /// Converts this period to a period in hours assuming a
        /// 7 day week, 24 hour day, 60 minute hour and 60 second minute.
        /// </summary>
        /// <returns>A period representing the number of standard hours in this period</returns>
        /// <remarks>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all
        /// weeks are 7 days, all days are 24 hours, all hours are 60 minutes and
        /// all minutes are 60 seconds. This is not true when daylight savings time
        /// is considered, and may also not be true for some unusual chronologies.
        /// However, it is included as it is a useful operation for many
        /// applications and business rules.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the period contains years or months</exception>
        public Hours ToStandardHours()
        {
            VerifyAbsenceOfYearsAndMonths("Hours");

            var hours = ToStandardDuration().Ticks / NodaConstants.TicksPerHour;

            return Periods.Hours.From((int)hours);
        }

        /// <summary>
        /// Converts this period to a period in days assuming a
        /// 7 day week, 24 hour day, 60 minute hour and 60 second minute.
        /// </summary>
        /// <returns>A period representing the number of standard days in this period</returns>
        /// <remarks>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all
        /// weeks are 7 days, all days are 24 hours, all hours are 60 minutes and
        /// all minutes are 60 seconds. This is not true when daylight savings time
        /// is considered, and may also not be true for some unusual chronologies.
        /// However, it is included as it is a useful operation for many
        /// applications and business rules.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the period contains years or months</exception>
        public Days ToStandardDays()
        {
            VerifyAbsenceOfYearsAndMonths("Days");

            var days = ToStandardDuration().Ticks / NodaConstants.TicksPerDay;

            return Periods.Days.From((int)days);
        }

        /// <summary>
        /// Converts this period to a period in weeks assuming a
        /// 7 day week, 24 hour day, 60 minute hour and 60 second minute.
        /// </summary>
        /// <returns>A period representing the number of standard weeks in this period</returns>
        /// <remarks>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all
        /// weeks are 7 days, all days are 24 hours, all hours are 60 minutes and
        /// all minutes are 60 seconds. This is not true when daylight savings time
        /// is considered, and may also not be true for some unusual chronologies.
        /// However, it is included as it is a useful operation for many
        /// applications and business rules.
        /// </remarks>
        /// <exception cref="NotSupportedException">If the period contains years or months</exception>
        public Weeks ToStandardWeeks()
        {
            VerifyAbsenceOfYearsAndMonths("Weeks");

            var weeks = ToStandardDuration().Ticks / NodaConstants.TicksPerWeek;

            return Periods.Weeks.From((int)weeks);
        }*/

        /// <summary>
        /// Normalizes this period using standard rules, assuming a 12 month year,
        /// 7 day week, 24 hour day, 60 minute hour and 60 second minute,
        /// providing control over how the result is split into fields.
        /// </summary>
        /// <param name="periodType">The period type of the new period</param>
        /// <returns>A normalized period equivalent to this period</returns>
        /// <remarks>
        /// <para>
        /// This method allows you to normalize a period.
        /// However to achieve this it makes the assumption that all years are
        /// 12 months, all weeks are 7 days, all days are 24 hours,
        /// all hours are 60 minutes and all minutes are 60 seconds. This is not
        /// true when daylight savings time is considered, and may also not be true
        /// for some chronologies. However, it is included as it is a useful operation
        /// for many applications and business rules.
        /// </para>
        /// <para>
        /// If the period contains years or months, then the months will be
        /// normalized to be between 0 and 11. The days field and below will be
        /// normalized as necessary, however this will not overflow into the months
        /// field. Thus a period of 1 year 15 months will normalize to 2 years 3 months.
        /// But a period of 1 month 40 days will remain as 1 month 40 days.
        /// </para>
        /// <para>
        /// The PeriodType parameter controls how the result is created. It allows
        /// you to omit certain fields from the result if desired. For example,
        /// you may not want the result to include weeks, in which case you pass
        /// in <code>PeriodType.YearMonthDayTime</code>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">If the periodType argument is null</exception>
        /// <exception cref="NotSupportedException">if this period contains non-zero
        /// years or months but the specified period type does not support them</exception>
        public Period Normalize(PeriodType periodType)
        {
            if (periodType == null)
            {
                throw new ArgumentNullException("periodType");
            }

            var duration = ToStandardDurationUnchecked();

            var period = Period.From(duration, CalendarSystem.Iso, periodType);

            int years = Years;
            int months = Months;
            if (years != 0 || months != 0)
            {
                years += months / 12;
                months = months % 12;
                period = period.WithYears(years).WithMonths(months);
            }
            return period;
        }

        /// <summary>
        /// Normalizes this period using standard rules, assuming a 12 month year,
        /// 7 day week, 24 hour day, 60 minute hour and 60 second minute,
        /// providing control over how the result is split into fields.
        /// </summary>
        /// <returns>A normalized period equivalent to this period</returns>
        /// <remarks>
        /// <para>
        /// This method allows you to normalize a period.
        /// However to achieve this it makes the assumption that all years are
        /// 12 months, all weeks are 7 days, all days are 24 hours,
        /// all hours are 60 minutes and all minutes are 60 seconds. This is not
        /// true when daylight savings time is considered, and may also not be true
        /// for some chronologies. However, it is included as it is a useful operation
        /// for many applications and business rules.
        /// </para>
        /// <para>
        /// If the period contains years or months, then the months will be
        /// normalized to be between 0 and 11. The days field and below will be
        /// normalized as necessary, however this will not overflow into the months
        /// field. Thus a period of 1 year 15 months will normalize to 2 years 3 months.
        /// But a period of 1 month 40 days will remain as 1 month 40 days.
        /// </para>
        /// <para>
        /// The result will always have a <code>PeriodType</code> of standard.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">If the periodType argument is null</exception>
        /// <exception cref="NotSupportedException">if this period contains non-zero
        /// years or months but the specified period type does not support them</exception>
        public Period NormalizeStandard()
        {
            return Normalize(PeriodType.Standard);
        }
        #endregion

        /// <summary>
        /// Gets an array of the value of each of the fields that this period supports.
        /// </summary>
        /// <remarks>
        /// The fields are returned largest to smallest, for example Hours, Minutes, Seconds.
        /// Each value corresponds to the same array index as <code>GetFieldTypes()</code>
        /// </remarks>
        /// <returns>The current values of each field in an array that may be altered, largest to smallest</returns>
        public int[] ToArray()
        {
            return CloneValues();
        }

        private int[] CloneValues()
        {
            return (int[])fieldValues.Clone();
        }

        #region Equality
        /// <summary>
        /// Returns a value indicating whether the current <see cref="Period"/> object 
        /// and a specified <see cref="Period"/> object represent the same value.
        /// </summary>
        /// <param name="other">A <see cref="Period"/> object to compare to the current Version object, or null</param>
        /// <returns>True if every component of the current <see cref="Period"/> object 
        /// matches the corresponding component of the other parameter; otherwise, false.</returns>
        public bool Equals(Period other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            if (PeriodType != other.PeriodType)
            {
                return false;
            }

            if (fieldValues.Length != other.fieldValues.Length)
            {
                return false;
            }

            // Check for elements equality
            for (int i = 0; i < fieldValues.Length; i++)
            {
                if (fieldValues[i] != other.fieldValues[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a value indicating whether the current <see cref="Period"/> object 
        /// is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with the current <see cref="Period"/> object, or null.</param>
        /// <returns>True if the current <see cref="Period"/> object and obj 
        /// are both <see cref="Period"/> objects, and every component of the current <see cref="Period"/> object 
        /// matches the corresponding component of obj; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Period);
        }

        /// <summary>
        /// Returns a hash code for the current <see cref="Period"/> object.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();

            HashCodeHelper.Hash(hash, PeriodType);
            for (int i = 0; i < fieldValues.Length; i++)
            {
                hash = HashCodeHelper.Hash(hash, fieldValues[i]);
            }

            return hash;
        }

        /// <summary>
        /// Determines whether two specified <see cref="Period"/> objects are equal.
        /// </summary>
        /// <param name="left">The first <see cref="Period"/> object</param>
        /// <param name="right">The second <see cref="Period"/> object</param>
        /// <returns>True if left equals right; otherwise, false.</returns>
        public static bool operator ==(Period left, Period right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether two specified <see cref="Period"/> objects are not equal.
        /// </summary>
        /// <param name="left">The first <see cref="Period"/> object</param>
        /// <param name="right">The second <see cref="Period"/> object</param>
        /// <returns>True if left does not equal right; otherwise, false.</returns>
        public static bool operator !=(Period left, Period right)
        {
            return !Equals(left, right);
        }
        #endregion

        #region Operators
        /// <summary>
        /// Implements the operator + (addition).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Period"/> representing the sum of the given periods.</returns>
        public static Period operator +(Period left, Period right)
        {
            if (left == null)
            {
                return right;
            }
            else
            {
                return left.Add(right);
            }
        }

        /// <summary>
        /// Adds one period to another. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Period"/> representing the sum of the given values.</returns>
        public static Period Add(Period left, Period right)
        {
            return left + right;
        }

        /// <summary>
        /// Implements the operator - (subtraction).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Period"/> representing the difference of the given values.</returns>
        public static Period operator -(Period left, Period right)
        {
            if (left == null)
            {
                return right;
            }
            else
            {
                return left.Subtract(right);
            }
        }

        /// <summary>
        /// Subtracts one period from another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Period"/> representing the difference of the given values.</returns>
        public static Period Subtract(Period left, Period right)
        {
            return left - right;
        }

        private static Period NegateImpl(IPeriod period)
        {
            if (period == null)
            {
                return null;
            }

            var values = new int[period.Size];
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = -period[i];
            }

            return new Period(values, period.PeriodType);
        }

        /// <summary>
        /// Negates the value of the specified <see cref="Period"/> operand.
        /// </summary>
        /// <param name="period">The <see cref="Period"/> operand.</param>
        /// <returns>The negated period</returns>
        public static Period operator -(Period period)
        {
            return Negate(period);
        }

        /// <summary>
        /// Negates the value of the specified <see cref="Period"/> operand.
        /// </summary>
        /// <param name="period">The <see cref="Period"/> operand.</param>
        /// <returns>The negated period</returns>
        public static Period Negate(Period period)
        {
            return NegateImpl(period);
        }

        /// <summary>
        /// Implements the unary operator + .
        /// </summary>
        /// <param name="period">The <see cref="Period"/> operand.</param>
        /// <returns>The same <see cref="Period"/> instance</returns>
        public static Period operator +(Period period)
        {
            return period;
        }

        /// <summary>
        /// Returns the same instance. Friendly alternative for <c>Period.operator +(Period)</c> operator.
        /// </summary>
        /// <param name="period">The <see cref="Period"/> operand.</param>
        /// <returns>The same <see cref="Period"/> instance</returns>
        public static Period Plus(Period period)
        {
            return period;
        }
        #endregion

        /*
        /// <summary>
        /// Gets the value as a String in the ISO8601 duration format.
        /// </summary>
        /// <returns>The value as an ISO8601 string</returns>
        /// <example>"P6H3M7S" represents 6 hours, 3 minutes, 7 seconds.</example>
        public override string ToString()
        {
            return IsoPeriodFormats.Standard.Print(this);
        }

        /// <summary>
        /// Uses the specified formatter to convert this period to a String.
        /// </summary>
        /// <param name="formatter">The formatter to use, null means use <code>ToString()</code>.</param>
        /// <returns>The formatted string</returns>
        public string ToString(PeriodFormatter formatter)
        {
            return formatter == null ? ToString() : formatter.Print(this);
        }*/
    }
}