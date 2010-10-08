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

namespace NodaTime.Fields
{
    /// <summary>
    /// Defines the calculation engine for date and time fields.
    /// The interface defines a set of methods that manipulate a LocalInstant
    /// with regards to a single field, such as monthOfYear or secondOfMinute.
    /// </summary>
    internal abstract class DateTimeField
    {
        private readonly DateTimeFieldType fieldType;

        protected DateTimeField(DateTimeFieldType fieldType)
        {
            if (fieldType == null)
            {
                throw new ArgumentNullException("fieldType");
            }
            this.fieldType = fieldType;
        }

        /// <summary>
        /// Get the type of the field.
        /// </summary>
        internal DateTimeFieldType FieldType { get { return fieldType; } }

        /// <summary>
        /// Get the name of the field.
        /// </summary>
        /// <remarks>
        /// By convention, names follow a pattern of "dddOfRrr", where "ddd" represents
        /// the (singular) duration unit field name and "Rrr" represents the (singular)
        /// duration range field name. If the range field is not applicable, then
        /// the name of the field is simply the (singular) duration field name.
        /// </remarks>
        internal String Name { get { return FieldType.ToString(); } }

        /// <summary>
        /// Gets the duration per unit value of this field, or UnsupportedDurationField if field has no duration.
        /// For example, if this
        /// field represents "hour of day", then the duration is an hour.
        /// </summary>
        internal abstract DurationField DurationField { get; }

        /// <summary>
        /// Returns the range duration of this field. For example, if this field
        /// represents "hour of day", then the range duration is a day.
        /// </summary>
        internal abstract DurationField RangeDurationField { get; }

        /// <summary>
        /// Defaults to fields being supported
        /// </summary>
        internal virtual bool IsSupported { get { return true; } }

        /// <summary>
        /// Returns true if the set method is lenient. If so, it accepts values that
        /// are out of bounds. For example, a lenient day of month field accepts 32
        /// for January, converting it to February 1.
        /// </summary>
        internal abstract bool IsLenient { get; }

        #region Values
        /// <summary>
        /// Get the value of this field from the local instant.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The value of the field, in the units of the field</returns>
        internal virtual int GetValue(LocalInstant localInstant)
        {
            return (int)GetInt64Value(localInstant);
        }

        /// <summary>
        /// Get the value of this field from the local instant.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The value of the field, in the units of the field</returns>
        internal abstract long GetInt64Value(LocalInstant localInstant);

        /// <summary>
        /// Adds a value (which may be negative) to the local instant value.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value will be added to this field. If the value is too large to be
        /// added solely to this field, larger fields will increase as required.
        /// Smaller fields should be unaffected, except where the result would be
        /// an invalid value for a smaller field. In this case the smaller field is
        /// adjusted to be in range. For example, in the ISO chronology:
        /// </para>
        /// <list type="bullet">
        /// <item>2000-08-20 add six months is 2001-02-20</item>
        /// <item>2000-08-20 add twenty months is 2002-04-20</item>
        /// <item>2000-08-20 add minus nine months is 1999-11-20</item>
        /// <item>2001-01-31 add one month  is 2001-02-28</item>
        /// <item>2001-01-31 add two months is 2001-03-31</item>
        /// </list>
        /// </remarks>
        /// <param name="localInstant">The local instant to add to</param>
        /// <param name="value">The value to add, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        internal virtual LocalInstant Add(LocalInstant localInstant, int value)
        {
            return DurationField.Add(localInstant, value);
        }

        /// <summary>
        /// Adds a value (which may be negative) to the local instant value.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value will be added to this field. If the value is too large to be
        /// added solely to this field, larger fields will increase as required.
        /// Smaller fields should be unaffected, except where the result would be
        /// an invalid value for a smaller field. In this case the smaller field is
        /// adjusted to be in range. For example, in the ISO chronology:
        /// </para>
        /// <list type="bullet">
        /// <item>2000-08-20 add six months is 2001-02-20</item>
        /// <item>2000-08-20 add twenty months is 2002-04-20</item>
        /// <item>2000-08-20 add minus nine months is 1999-11-20</item>
        /// <item>2001-01-31 add one month  is 2001-02-28</item>
        /// <item>2001-01-31 add two months is 2001-03-31</item>
        /// </list>
        /// </remarks>
        /// <param name="localInstant">The local instant to add to</param>
        /// <param name="value">The value to add, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        internal virtual LocalInstant Add(LocalInstant localInstant, long value)
        {
            return DurationField.Add(localInstant, value);
        }

        /// <summary>
        /// Adds a value (which may be negative) to the local instant,
        /// wrapping within this field.
        /// </summary>
        /// <param name="localInstant">The local instant to add to</param>
        /// <param name="value">The value to add, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        /// <remarks>
        /// <para>
        /// The value will be added to this field. If the value is too large to be
        /// added solely to this field then it wraps. Larger fields are always
        /// unaffected. Smaller fields should be unaffected, except where the
        /// result would be an invalid value for a smaller field. In this case the
        /// smaller field is adjusted to be in range. For example, in the ISO chronology:
        /// </para>
        /// <list type="bullet">
        /// <item>2000-08-20 AddWrapField six months is 2000-02-20</item>
        /// <item>2000-08-20 AddWrapField twenty months is 2000-04-20</item>
        /// <item>2000-08-20 AddWrapField minus nine months is 2000-11-20</item>
        /// <item>2001-01-31 AddWrapField one month  is 2001-02-28</item>
        /// <item>2001-01-31 AddWrapField two months is 2001-03-31</item>
        /// </list>
        /// </remarks>
        internal virtual LocalInstant AddWrapField(LocalInstant localInstant, int value)
        {
            int current = GetValue(localInstant);
            int wrapped = FieldUtils.GetWrappedValue(current, value, (int)GetMinimumValue(localInstant), (int)GetMaximumValue(localInstant));
            return SetValue(localInstant, wrapped);
        }

        /// <summary>
        /// Adds a value (which may be negative) to the partial instant,
        /// throwing an exception if the maximum size of the instant is reached.
        /// </summary>
        /// <param name="instant">The partial instant</param>
        /// <param name="fieldIndex">The index of this field in the instant</param>
        /// <param name="values">The values of the partial instant which should be updated</param>
        /// <param name="valueToAdd">The value to add, in the units of the field</param>
        /// <returns>The passed in values</returns>
        /// <remarks>
        /// <para>
        /// The value will be added to this field, overflowing into larger fields
        /// if necessary. Smaller fields should be unaffected, except where the
        /// result would be an invalid value for a smaller field. In this case the
        /// smaller field is adjusted to be in range.
        /// </para>
        /// <para>
        /// Partial instants only contain some fields. This may result in a maximum
        /// possible value, such as TimeOfDay being limited to 23:59:59:999. If this
        /// limit is reached by the add an exception is thrown. For example, in the
        /// ISO chronology:
        /// </para>
        /// <list type="bullet">
        /// <item>2000-08-20 add six months is 2000-02-20</item>
        /// <item>2000-08-20 add twenty months is 2000-04-20</item>
        /// <item>2000-08-20 add minus nine months is 2000-11-20</item>
        /// <item>2001-01-31 add one month  is 2001-02-28</item>
        /// <item>2001-01-31 add two months is 2001-03-31</item>
        /// </list>
        /// </remarks>
        internal virtual int[] Add(IPartial instant, int fieldIndex, int[] values, int valueToAdd)
        {
            if (valueToAdd == 0)
            {
                return values;
            }

            // there are more efficient algorithms than this (especially for time only fields)
            // trouble is when dealing with days and months, so we use this technique of
            // adding/removing one from the larger field at a time
            DateTimeField nextField = null;
            while (valueToAdd > 0)
            {
                long max = GetMaximumValue(instant, values);
                long proposed = values[fieldIndex] + valueToAdd;
                if (proposed <= max)
                {
                    values[fieldIndex] = (int)proposed;
                    break;
                }
                if (nextField == null)
                {
                    if (fieldIndex == 0)
                    {
                        throw new ArgumentException("Maximum value exceeded for add");
                    }
                    nextField = instant.GetField(fieldIndex - 1);
                    // test only works if this field is UTC (ie. local)
                    if (RangeDurationField.FieldType != nextField.DurationField.FieldType)
                    {
                        throw new ArgumentException("Fields invalid for add");
                    }
                }
                valueToAdd -= ((int)max + 1) - values[fieldIndex]; // reduce the amount to add
                values = nextField.Add(instant, fieldIndex - 1, values, 1); // add 1 to next bigger field
                values[fieldIndex] = (int)GetMinimumValue(instant, values); // reset this field to zero
            }
            while (valueToAdd < 0)
            {
                long min = GetMinimumValue(instant, values);
                long proposed = values[fieldIndex] + valueToAdd;
                if (proposed >= min)
                {
                    values[fieldIndex] = (int)proposed;
                    break;
                }
                if (nextField == null)
                {
                    if (fieldIndex == 0)
                    {
                        throw new ArgumentException("Maximum value exceeded for add");
                    }
                    nextField = instant.GetField(fieldIndex - 1);
                    if (RangeDurationField.FieldType != nextField.DurationField.FieldType)
                    {
                        throw new ArgumentException("Fields invalid for add");
                    }
                }
                valueToAdd -= ((int)min - 1) - values[fieldIndex]; // reduce the amount to add
                values = nextField.Add(instant, fieldIndex - 1, values, -1); // subtract 1 from next bigger field
                values[fieldIndex] = (int)GetMaximumValue(instant, values); // reset this field to max value
            }

            return SetValue(instant, fieldIndex, values, values[fieldIndex]); // adjusts smaller fields
        }

        /// <summary>
        /// Adds a value (which may be negative) to the partial instant,
        /// wrapping the whole partial if the maximum size of the partial is reached.
        /// </summary>
        /// <param name="instant">The partial instant</param>
        /// <param name="fieldIndex">The index of this field in the partial</param>
        /// <param name="values">The values of the partial instant which should be updated</param>
        /// <param name="valueToAdd">The value to add, in the units of the field</param>
        /// <returns>The passed in values</returns>
        /// <remarks>
        /// <para>
        /// The value will be added to this field, overflowing into larger fields
        /// if necessary. Smaller fields should be unaffected, except where the
        /// result would be an invalid value for a smaller field. In this case the
        /// smaller field is adjusted to be in range.
        /// </para>
        /// <para>
        /// Partial instants only contain some fields. This may result in a maximum
        /// possible value, such as TimeOfDay normally being limited to 23:59:59:999.
        /// If ths limit is reached by the addition, this method will wrap back to
        /// 00:00:00.000. In fact, you would generally only use this method for
        /// classes that have a limitation such as this. For example, in the ISO chronology:
        /// </para>
        /// <list type="bullet">
        /// <item>10:20:30 add 20 minutes is 10:40:30</item>
        /// <item>10:20:30 add 45 minutes is 11:05:30</item>
        /// <item>10:20:30 add 16 hours is 02:20:30</item>
        /// </list>
        /// </remarks>
        internal virtual int[] AddWrapPartial(IPartial instant, int fieldIndex, int[] values, int valueToAdd)
        {
            if (valueToAdd == 0)
            {
                return values;
            }

            // there are more efficient algorithms than this (especially for time only fields)
            // trouble is when dealing with days and months, so we use this technique of
            // adding/removing one from the larger field at a time
            DateTimeField nextField = null;
            while (valueToAdd > 0)
            {
                int max = (int)GetMaximumValue(instant, values);
                long proposed = values[fieldIndex] + valueToAdd;
                if (proposed <= max)
                {
                    values[fieldIndex] = (int)proposed;
                    break;
                }
                if (nextField == null)
                {
                    if (fieldIndex == 0)
                    {
                        valueToAdd -= (max + 1) - values[fieldIndex];
                        values[fieldIndex] = (int)GetMinimumValue(instant, values);
                        continue;
                    }
                    nextField = instant.GetField(fieldIndex - 1);
                    // test only works if this field is UTC (ie. local)
                    if (RangeDurationField.FieldType != nextField.DurationField.FieldType)
                    {
                        throw new ArgumentException("Fields invalid for add");
                    }
                }
                valueToAdd -= (max + 1) - values[fieldIndex]; // reduce the amount to add
                values = nextField.AddWrapPartial(instant, fieldIndex - 1, values, 1); // add 1 to next bigger field
                values[fieldIndex] = (int)GetMinimumValue(instant, values); // reset this field to zero
            }
            while (valueToAdd < 0)
            {
                int min = (int)GetMinimumValue(instant, values);
                long proposed = values[fieldIndex] + valueToAdd;
                if (proposed >= min)
                {
                    values[fieldIndex] = (int)proposed;
                    break;
                }
                if (nextField == null)
                {
                    if (fieldIndex == 0)
                    {
                        valueToAdd -= (min - 1) - values[fieldIndex];
                        values[fieldIndex] = (int)GetMaximumValue(instant, values);
                        continue;
                    }
                    nextField = instant.GetField(fieldIndex - 1);
                    if (RangeDurationField.FieldType != nextField.DurationField.FieldType)
                    {
                        throw new ArgumentException("Fields invalid for add");
                    }
                }
                valueToAdd -= (min - 1) - values[fieldIndex]; // reduce the amount to add
                values = nextField.AddWrapPartial(instant, fieldIndex - 1, values, -1); // subtract 1 from next bigger field
                values[fieldIndex] = (int)GetMaximumValue(instant, values); // reset this field to max value
            }

            return SetValue(instant, fieldIndex, values, values[fieldIndex]); // adjusts smaller fields
        }

        /// <summary>
        /// Computes the difference between two instants, as measured in the units
        /// of this field. Any fractional units are dropped from the result. Calling
        /// GetDifference reverses the effect of calling add. In the following code:
        /// <code>
        /// LocalInstant instant = ...
        /// int v = ...
        /// int age = GetDifference(Add(instant, v), instant);
        /// </code>
        /// The value 'age' is the same as the value 'v'.
        /// </summary>
        /// <param name="minuendInstant">The local instant to subtract from</param>
        /// <param name="subtrahendInstant">The local instant to subtract from minuendInstant</param>
        /// <returns>The difference in the units of this field</returns>
        internal virtual int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return DurationField.GetDifference(minuendInstant, subtrahendInstant);
        }

        /// <summary>
        /// Computes the difference between two instants, as measured in the units
        /// of this field. Any fractional units are dropped from the result. Calling
        /// GetDifference reverses the effect of calling add. In the following code:
        /// <code>
        /// LocalInstant instant = ...
        /// int v = ...
        /// int age = GetDifference(Add(instant, v), instant);
        /// </code>
        /// The value 'age' is the same as the value 'v'.
        /// </summary>
        /// <param name="minuendInstant">The local instant to subtract from</param>
        /// <param name="subtrahendInstant">The local instant to subtract from minuendInstant</param>
        /// <returns>The difference in the units of this field</returns>
        internal virtual long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return DurationField.GetInt64Difference(minuendInstant, subtrahendInstant);
        }

        /// <summary>
        /// Sets a value in the local instant supplied.
        /// <para>
        /// The value of this field will be set.
        /// If the value is invalid, an exception if thrown.
        /// </para>
        /// <para>
        /// If setting this field would make other fields invalid, then those fields
        /// may be changed. For example if the current date is the 31st January, and
        /// the month is set to February, the day would be invalid. Instead, the day
        /// would be changed to the closest value - the 28th/29th February as appropriate.
        /// </para>
        /// </summary>
        /// <param name="localInstant">The local instant to set in</param>
        /// <param name="value">The value to set, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        internal abstract LocalInstant SetValue(LocalInstant localInstant, long value);

        /// <summary>
        /// Sets a value in the local instant supplied from a human-readable, text value.
        /// </summary>
        /// <param name="instant">The local instant to set in</param>
        /// <param name="text">The text value to set</param>
        /// <param name="provider">The format provider to use</param>
        /// <returns>The updated local instant</returns>
        /// <remarks>
        /// If setting this field would make other fields invalid, then those fields
        /// may be changed. For example if the current date is the 31st January, and
        /// the month is set to February, the day would be invalid. Instead, the day
        /// would be changed to the closest value - the 28th/29th February as appropriate.
        /// </remarks>
        internal virtual LocalInstant SetValue(LocalInstant instant, string text, IFormatProvider provider)
        {
            int value = ConvertText(text, provider);
            return SetValue(instant, value);
        }

        /// <summary>
        /// Sets a value in the local instant supplied from a human-readable, text value.
        /// </summary>
        /// <param name="instant">The local instant to set in</param>
        /// <param name="text">The text value to set</param>
        /// <returns>The updated local instant</returns>
        /// <remarks>
        /// If setting this field would make other fields invalid, then those fields
        /// may be changed. For example if the current date is the 31st January, and
        /// the month is set to February, the day would be invalid. Instead, the day
        /// would be changed to the closest value - the 28th/29th February as appropriate.
        /// </remarks>        
        internal virtual LocalInstant SetValue(LocalInstant instant, String text)
        {
            return SetValue(instant, text, null);
        }

        /// <summary>
        /// Sets a value using the specified partial instant.
        /// </summary>
        /// <param name="instant">The partial instant</param>
        /// <param name="fieldIndex">The index of this field in the instant</param>
        /// <param name="values">The values of the partial instant which should be updated</param>
        /// <param name="newValue">The value to set, in the units of the field</param>
        /// <returns>The passed in values</returns>
        /// <remarks>
        /// <para>
        /// The value of this field (specified by the index) will be set.
        /// If the value is invalid, an exception if thrown.
        /// </para>
        /// <para>
        /// If setting this field would make other fields invalid, then those fields
        /// may be changed. For example if the current date is the 31st January, and
        /// the month is set to February, the day would be invalid. Instead, the day
        /// would be changed to the closest value - the 28th/29th February as appropriate.
        /// </para>
        /// </remarks>
        internal virtual int[] SetValue(IPartial instant, int fieldIndex, int[] values, int newValue)
        {
            FieldUtils.VerifyValueBounds(this, newValue, GetMinimumValue(instant, values), GetMaximumValue(instant, values));
            values[fieldIndex] = newValue;

            // may need to adjust smaller fields
            for (int i = fieldIndex + 1; i < instant.Size; i++)
            {
                DateTimeField field = instant.GetField(i);
                if (values[i] > field.GetMaximumValue(instant, values))
                {
                    values[i] = (int)field.GetMaximumValue(instant, values);
                }
                if (values[i] < (int)field.GetMinimumValue(instant, values))
                {
                    values[i] = (int)field.GetMinimumValue(instant, values);
                }
            }
            return values;
        }

        /// <summary>
        /// Sets a value using the specified  partial instant supplied from a human-readable, text value.
        /// </summary>
        /// <param name="instant">The partial instant</param>
        /// <param name="fieldIndex">The index of this field in the instant</param>
        /// <param name="values">The values of the partial instant which should be updated</param>
        /// <param name="text">The text value to set</param>
        /// <param name="provider">The format provider to use</param>
        /// <returns>The passed in values</returns>
        internal virtual int[] SetValue(IPartial instant, int fieldIndex, int[] values, String text, IFormatProvider provider)
        {
            int value = ConvertText(text, provider);
            return SetValue(instant, fieldIndex, values, value);
        }
        #endregion

        #region Leap
        /// <summary>
        /// Defaults to non-leap.
        /// </summary>
        internal virtual bool IsLeap(LocalInstant localInstant)
        {
            return false;
        }

        /// <summary>
        /// Defaults to 0.
        /// </summary>
        internal virtual int GetLeapAmount(LocalInstant localInstant)
        {
            return 0;
        }

        /// <summary>
        /// Defaults to null, i.e. no leap duration field.
        /// </summary>
        internal virtual DurationField LeapDurationField { get { return null; } }
        #endregion

        #region Ranges
        /// <summary>
        /// Defaults to the absolute maximum for the field.
        /// </summary>
        internal virtual long GetMaximumValue(LocalInstant localInstant)
        {
            return GetMaximumValue();
        }

        /// <summary>
        /// Defaults to the absolute maximum for the field.
        /// </summary>
        /// <param name="instant"></param>
        /// <returns></returns>
        internal virtual long GetMaximumValue(IPartial instant)
        {
            return GetMaximumValue();
        }

        /// <summary>
        /// Defaults to the absolute maximum for the field.
        /// </summary>
        /// <param name="instant"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal virtual long GetMaximumValue(IPartial instant, int[] values)
        {
            return GetMaximumValue();
        }

        /// <summary>
        /// Get the maximum allowable value for this field.
        /// </summary>
        /// <returns>The maximum valid value for this field, in the units of the field</returns>
        internal abstract long GetMaximumValue();

        /// <summary>
        /// Defaults to the absolute minimum for the field.
        /// </summary>
        internal virtual long GetMinimumValue(LocalInstant localInstant)
        {
            return GetMinimumValue();
        }

        /// <summary>
        /// Defaults to the absolute minimum for the field.
        /// </summary>
        /// <param name="instant"></param>
        /// <returns></returns>
        internal virtual long GetMinimumValue(IPartial instant)
        {
            return GetMinimumValue();
        }

        /// <summary>
        /// Defaults to the absolute minimum for the field.
        /// </summary>
        /// <param name="instant"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal virtual long GetMinimumValue(IPartial instant, int[] values)
        {
            return GetMinimumValue();
        }

        /// <summary>
        /// Get the minimum allowable value for this field.
        /// </summary>
        /// <returns>The minimum valid value for this field, in the units of the field</returns>
        internal abstract long GetMinimumValue();
        #endregion

        #region Rounding
        /// <summary>
        /// Round to the lowest whole unit of this field. After rounding, the value
        /// of this field and all fields of a higher magnitude are retained. The
        /// fractional ticks that cannot be expressed in whole increments of this
        /// field are set to minimum.
        /// <para>
        /// For example, a datetime of 2002-11-02T23:34:56.789, rounded to the
        /// lowest whole hour is 2002-11-02T23:00:00.000.
        /// </para>
        /// </summary>
        /// <param name="localInstant">The local instant to round</param>
        /// <returns>Rounded local instant</returns>
        internal abstract LocalInstant RoundFloor(LocalInstant localInstant);

        /// <summary>
        /// Round to the highest whole unit of this field. The value of this field
        /// and all fields of a higher magnitude may be incremented in order to
        /// achieve this result. The fractional ticks that cannot be expressed in
        /// whole increments of this field are set to minimum.
        /// <para>
        /// For example, a datetime of 2002-11-02T23:34:56.789, rounded to the
        /// highest whole hour is 2002-11-03T00:00:00.000.
        /// </para>
        /// </summary>
        /// <param name="localInstant">The local instant to round</param>
        /// <returns>Rounded local instant</returns>
        internal virtual LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            LocalInstant newInstant = RoundFloor(localInstant);
            if (newInstant != localInstant)
            {
                newInstant = Add(newInstant, 1);
            }
            return newInstant;
        }

        /// <summary>
        /// Round to the nearest whole unit of this field. If the given local instant
        /// is closer to the floor or is exactly halfway, this function
        /// behaves like RoundFloor. If the local instant is closer to the
        /// ceiling, this function behaves like RoundCeiling.
        /// </summary>
        /// <param name="localInstant">The local instant to round</param>
        /// <returns>Rounded local instant</returns>
        internal virtual LocalInstant RoundHalfFloor(LocalInstant localInstant)
        {
            LocalInstant floor = RoundFloor(localInstant);
            LocalInstant ceiling = RoundCeiling(localInstant);

            Duration diffFromFloor = localInstant - floor;
            Duration diffToCeiling = ceiling - localInstant;

            // Closer to the floor, or halfway - round floor
            return diffFromFloor <= diffToCeiling ? floor : ceiling;
        }

        /// <summary>
        /// Round to the nearest whole unit of this field. If the given local instant
        /// is closer to the floor, this function behaves like RoundFloor. If
        /// the local instant is closer to the ceiling or is exactly halfway,
        /// this function behaves like RoundCeiling.
        /// </summary>
        /// <param name="localInstant">The local instant to round</param>
        /// <returns>Rounded local instant</returns>
        internal virtual LocalInstant RoundHalfCeiling(LocalInstant localInstant)
        {
            LocalInstant floor = RoundFloor(localInstant);
            LocalInstant ceiling = RoundCeiling(localInstant);

            long diffFromFloor = localInstant.Ticks - floor.Ticks;
            long diffToCeiling = ceiling.Ticks - localInstant.Ticks;

            // Closer to the ceiling, or halfway - round ceiling
            return diffToCeiling <= diffFromFloor ? ceiling : floor;
        }

        /// <summary>
        /// Round to the nearest whole unit of this field. If the given local instant
        /// is closer to the floor, this function behaves like RoundFloor. If
        /// the local instant is closer to the ceiling, this function behaves
        /// like RoundCeiling.
        /// </summary>
        /// <param name="localInstant">The local instant to round</param>
        /// <returns>Rounded local instant</returns>
        internal virtual LocalInstant RoundHalfEven(LocalInstant localInstant)
        {
            LocalInstant floor = RoundFloor(localInstant);
            LocalInstant ceiling = RoundCeiling(localInstant);

            Duration diffFromFloor = localInstant - floor;
            Duration diffToCeiling = ceiling - localInstant;

            // Closer to the floor - round floor
            if (diffFromFloor < diffToCeiling)
            {
                return floor;
            }
                // Closer to the ceiling - round ceiling
            else if (diffToCeiling < diffFromFloor)
            {
                return ceiling;
            }
            else
            {
                // Round to the instant that makes this field even. If both values
                // make this field even (unlikely), favor the ceiling.
                return (GetInt64Value(ceiling) & 1) == 0 ? ceiling : floor;
            }
        }

        /// <summary>
        /// Returns the fractional duration of this field. In other words, 
        /// calling Remainder returns the duration that RoundFloor would subtract.
        /// <para>
        /// For example, on a datetime of 2002-11-02T23:34:56.789, the remainder by
        /// hour is 34 minutes and 56.789 seconds.
        /// </para>
        /// </summary>
        /// <param name="localInstant">The local instant to get the remainder</param>
        /// <returns>Remainder duration</returns>
        internal virtual Duration Remainder(LocalInstant localInstant)
        {
            return localInstant - RoundFloor(localInstant);
        }
        #endregion

        #region Text
        /// <summary>
        /// Get the maximum text value for this field.
        /// </summary>
        /// <param name="provider">The format provider to use</param>
        /// <returns>The maximum text length</returns>
        /// <remarks>
        /// The default implementation returns the equivalent of 
        /// GetMaximumValue().ToString().Length.
        /// </remarks>
        internal virtual int GetMaximumTextLength(IFormatProvider provider)
        {
            int max = (int)GetMaximumValue();
            if (max >= 0)
            {
                if (max < 10)
                {
                    return 1;
                }
                else if (max < 100)
                {
                    return 2;
                }
                else if (max < 1000)
                {
                    return 3;
                }
            }
            return max.ToString(provider).Length;
        }

        /// <summary>
        /// Get the maximum short text value for this field.
        /// </summary>
        /// <param name="provider">The format provider to use</param>
        /// <returns>The maximum short text length</returns>
        /// <remarks>
        /// The default implementation returns GetMaximumTextLength().
        /// </remarks>
        internal virtual int GetMaximumShortTextLength(IFormatProvider provider)
        {
            return GetMaximumTextLength(provider);
        }

        /// <summary>
        /// Get the human-readable, text value of this field from the local instant.
        /// <para>
        /// The default implementation calls <see cref="GetAsText(int, IFormatProvider)"/>.
        /// </para>
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <param name="provider">Format provider to use</param>
        /// <returns>The text value of the field</returns>
        internal virtual string GetAsText(LocalInstant localInstant, IFormatProvider provider)
        {
            return GetAsText(GetValue(localInstant), provider);
        }

        /// <summary>
        /// Get the human-readable, text value of this field from the local instant.
        /// <para>
        /// The default implementation calls <see cref="GetAsText(int, IFormatProvider)"/>.
        /// </para>
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The text value of the field</returns>
        internal virtual string GetAsText(LocalInstant localInstant)
        {
            return GetAsText(localInstant, null);
        }

        /// <summary>
        /// Get the human-readable, text value of this field from a partial instant.
        /// <para>
        /// The default implementation returns GetAsText(fieldValue, provider).
        /// </para>
        /// </summary>
        /// <param name="partial">The partial instant to query</param>
        /// <param name="fieldValue">The field value of this field, provided for performance</param>
        /// <param name="provider">Format provider to use</param>
        /// <returns>The text value of the field</returns>
        internal virtual string GetAsText(IPartial partial, int fieldValue, IFormatProvider provider)
        {
            return GetAsText(fieldValue, provider);
        }

        /// <summary>
        /// Get the human-readable, text value of this field from a partial instant.
        /// <para>
        /// The default implementation calls <see cref="IPartial.Get(DateTimeFieldType)"/>
        /// and <see cref="GetAsText(IPartial, int, IFormatProvider)"/>
        /// </para>
        /// </summary>
        /// <param name="partial">The partial instant to query</param>
        /// <param name="provider">Format provider to use</param>
        /// <returns>The text value of the field</returns>
        internal virtual string GetAsText(IPartial partial, IFormatProvider provider)
        {
            return GetAsText(partial, partial.Get(FieldType), provider);
        }

        /// <summary>
        /// Get the human-readable, text value of this field from the field value.
        /// <para>
        /// The default implementation returns fieldValue.ToString(provider).
        /// </para>
        /// <para>
        /// Note: subclasses that override this method should also override
        /// GetMaximumTextLength.
        /// </para>
        /// </summary>
        /// <param name="fieldValue">the numeric value to convert to text</param>
        /// <param name="provider">Format provider to use</param>
        /// <returns>The text value of the field</returns>
        internal virtual string GetAsText(int fieldValue, IFormatProvider provider)
        {
            return fieldValue.ToString(provider);
        }

        /// <summary>
        /// Get the human-readable, short text value of this field from the local instant.
        /// <para>
        /// The default implementation calls <see cref="GetAsShortText(int, IFormatProvider)"/>.
        /// </para>
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <param name="provider">Format provider to use</param>
        /// <returns>The text value of the field</returns>
        internal virtual string GetAsShortText(LocalInstant localInstant, IFormatProvider provider)
        {
            return GetAsShortText(GetValue(localInstant), provider);
        }

        /// <summary>
        /// Get the human-readable, short text value of this field from the local instant.
        /// <para>
        /// The default implementation calls <see cref="GetAsShortText(int, IFormatProvider)"/>.
        /// </para>
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The text value of the field</returns>
        internal virtual string GetAsShortText(LocalInstant localInstant)
        {
            return GetAsShortText(localInstant, null);
        }

        /// <summary>
        /// Get the human-readable, short text value of this field from a partial instant.
        /// <para>
        /// The default implementation returns GetAsShortText(fieldValue, provider).
        /// </para>
        /// </summary>
        /// <param name="partial">The partial instant to query</param>
        /// <param name="fieldValue">The field value of this field, provided for performance</param>
        /// <param name="provider">Format provider to use</param>
        /// <returns>The text value of the field</returns>
        internal virtual string GetAsShortText(IPartial partial, int fieldValue, IFormatProvider provider)
        {
            return GetAsShortText(fieldValue, provider);
        }

        /// <summary>
        /// Get the human-readable, short text value of this field from a partial instant.
        /// <para>
        /// The default implementation calls <see cref="IPartial.Get(DateTimeFieldType)"/>
        /// and <see cref="GetAsShortText(IPartial, int, IFormatProvider)"/>
        /// </para>
        /// </summary>
        /// <param name="partial">The partial instant to query</param>
        /// <param name="provider">Format provider to use</param>
        /// <returns>The text value of the field</returns>
        internal virtual string GetAsShortText(IPartial partial, IFormatProvider provider)
        {
            return GetAsShortText(partial, partial.Get(FieldType), provider);
        }

        /// <summary>
        /// Get the human-readable, short text value of this field from the field value.
        /// <para>
        /// The default implementation calls <see cref="GetAsText(int, IFormatProvider)"/>.
        /// </para>
        /// <para>
        /// Note: subclasses that override this method should also override
        /// GetMaximumShortTextLength.
        /// </para>
        /// </summary>
        /// <param name="fieldValue">the numeric value to convert to text</param>
        /// <param name="provider">Format provider to use</param>
        /// <returns>The text value of the field</returns>
        internal virtual string GetAsShortText(int fieldValue, IFormatProvider provider)
        {
            return GetAsText(fieldValue, provider);
        }
        #endregion

        protected int ConvertText(String text, IFormatProvider provider)
        {
            int result = 0;
            if (Int32.TryParse(text, out result))
            {
                return result;
            }
            else
            {
                throw new FieldValueException(FieldType, text);
            }
        }

        public override string ToString()
        {
            return fieldType.ToString();
        }
    }
}