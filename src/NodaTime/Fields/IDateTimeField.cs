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
namespace NodaTime.Fields
{
    /// <summary>
    /// Defines the calculation engine for date and time fields.
    /// The interface defines a set of methods that manipulate a millisecond datetime
    /// with regards to a single field, such as monthOfYear or secondOfMinute.
    /// </summary>
    public interface IDateTimeField
    {
        /// <summary>
        /// Get the type of the field.
        /// </summary>
        DateTimeFieldType FieldType { get; }

        /// <summary>
        /// Get the name of the field.
        /// </summary>
        /// <remarks>
        /// By convention, names follow a pattern of "dddOfRrr", where "ddd" represents
        /// the (singular) duration unit field name and "Rrr" represents the (singular)
        /// duration range field name. If the range field is not applicable, then
        /// the name of the field is simply the (singular) duration field name.
        /// </remarks>
        String Name { get; }

        /// <summary>
        /// Gets the duration per unit value of this field, or UnsupportedDurationField if field has no duration.
        /// For example, if this
        /// field represents "hour of day", then the duration is an hour.
        /// </summary>
        DurationField DurationField { get; }

        /// <summary>
        /// Returns the range duration of this field. For example, if this field
        /// represents "hour of day", then the range duration is a day.
        /// </summary>
        DurationField RangeDurationField { get; }

        /// <summary>
        /// Returns true if this field is supported.
        /// </summary>
        bool IsSupported { get; }

        /// <summary>
        /// Returns true if the set method is lenient. If so, it accepts values that
        /// are out of bounds. For example, a lenient day of month field accepts 32
        /// for January, converting it to February 1.
        /// </summary>
        bool IsLenient { get; }

        #region Values

        /// <summary>
        /// Get the value of this field from the local instant.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The value of the field, in the units of the field</returns>
        int GetValue(LocalInstant localInstant);

        /// <summary>
        /// Get the value of this field from the local instant.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The value of the field, in the units of the field</returns>
        long GetInt64Value(LocalInstant localInstant);

        /// <summary>
        /// Adds a value (which may be negative) to the local instant value.
        /// <para>
        /// The value will be added to this field. If the value is too large to be
        /// added solely to this field, larger fields will increase as required.
        /// Smaller fields should be unaffected, except where the result would be
        /// an invalid value for a smaller field. In this case the smaller field is
        /// adjusted to be in range.
        /// </para>
        /// For example, in the ISO chronology:<br>
        /// 2000-08-20 add six months is 2001-02-20<br>
        /// 2000-08-20 add twenty months is 2002-04-20<br>
        /// 2000-08-20 add minus nine months is 1999-11-20<br>
        /// 2001-01-31 add one month  is 2001-02-28<br>
        /// 2001-01-31 add two months is 2001-03-31<br>
        /// </summary>
        /// <param name="localInstant">The local instant to add to</param>
        /// <param name="value">The value to add, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        LocalInstant Add(LocalInstant localInstant, int value);

        /// <summary>
        /// Adds a value (which may be negative) to the local instant value.
        /// <para>
        /// The value will be added to this field. If the value is too large to be
        /// added solely to this field, larger fields will increase as required.
        /// Smaller fields should be unaffected, except where the result would be
        /// an invalid value for a smaller field. In this case the smaller field is
        /// adjusted to be in range.
        /// </para>
        /// For example, in the ISO chronology:<br>
        /// 2000-08-20 add six months is 2001-02-20<br>
        /// 2000-08-20 add twenty months is 2002-04-20<br>
        /// 2000-08-20 add minus nine months is 1999-11-20<br>
        /// 2001-01-31 add one month  is 2001-02-28<br>
        /// 2001-01-31 add two months is 2001-03-31<br>
        /// </summary>
        /// <param name="localInstant">The local instant to add to</param>
        /// <param name="value">The value to add, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        LocalInstant Add(LocalInstant localInstant, long value);

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
        /// smaller field is adjusted to be in range.
        /// </para>
        /// <example>
        /// For example, in the ISO chronology:<br>
        /// 2000-08-20 AddWrapField six months is 2000-02-20<br>
        /// 2000-08-20 AddWrapField twenty months is 2000-04-20<br>
        /// 2000-08-20 AddWrapField minus nine months is 2000-11-20<br>
        /// 2001-01-31 AddWrapField one month  is 2001-02-28<br>
        /// 2001-01-31 AddWrapField two months is 2001-03-31<br>
        /// </example>
        /// </remarks>
        LocalInstant AddWrapField(LocalInstant localInstant, int value);

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
        /// limit is reached by the add an exception is thrown.
        /// </para>
        /// <example>
        /// For example, in the ISO chronology:<br>
        /// 2000-08-20 add six months is 2000-02-20<br>
        /// 2000-08-20 add twenty months is 2000-04-20<br>
        /// 2000-08-20 add minus nine months is 2000-11-20<br>
        /// 2001-01-31 add one month  is 2001-02-28<br>
        /// 2001-01-31 add two months is 2001-03-31<br>
        /// </example>
        /// </remarks>
        int[] Add(IPartial instant, int fieldIndex, int[] values, int valueToAdd);

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
        /// classes that have a limitation such as this.
        /// </para>
        /// <example>
        /// For example, in the ISO chronology:<br>
        /// 10:20:30 add 20 minutes is 10:40:30<br>
        /// 10:20:30 add 45 minutes is 11:05:30<br>
        /// 10:20:30 add 16 hours is 02:20:30<br>
        /// </example>
        /// </remarks>
        int[] AddWrapPartial(IPartial instant, int fieldIndex, int[] values, int valueToAdd);

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
        int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant);

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
        long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant);

        /// <summary>
        /// Sets a value in the milliseconds supplied.
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
        LocalInstant SetValue(LocalInstant localInstant, long value);

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
        LocalInstant SetValue(LocalInstant instant, string text, IFormatProvider provider);

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
        LocalInstant SetValue(LocalInstant instant, String text);

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
        int[] SetValue(IPartial instant, int fieldIndex, int[] values, int newValue);

        /// <summary>
        /// Sets a value using the specified  partial instant supplied from a human-readable, text value.
        /// </summary>
        /// <param name="instant">The partial instant</param>
        /// <param name="fieldIndex">The index of this field in the instant</param>
        /// <param name="values">The values of the partial instant which should be updated</param>
        /// <param name="text">The text value to set</param>
        /// <param name="provider">The format provider to use</param>
        /// <returns>The passed in values</returns>
        int[] SetValue(IPartial instant, int fieldIndex, int[] values, String text, IFormatProvider provider);

        #endregion

        #region Leap

        /// <summary>
        /// Returns whether this field is 'leap' for the specified instant.
        /// <para>
        /// For example, a leap year would return true, a non leap year would return false.
        /// </para>
        /// </summary>
        /// <param name="localInstant">The local instant to check for "leap" status</param>
        /// <returns>True if the field is 'leap', false otherwise</returns>
        bool IsLeap(LocalInstant localInstant);

        /// <summary>
        /// Gets the amount by which this field is 'leap' for the specified instant.
        /// <para>
        /// For example, a leap year would return one, a non leap year would return zero.
        /// </para>
        /// </summary>
        /// <param name="localInstant">The local instant to check for "leap" status</param>
        /// <returns>The amount, in units of the leap duration field, that the field is leap</returns>
        int GetLeapAmount(LocalInstant localInstant);

        /// <summary>
        /// If this field were to leap, then it would be in units described by the
        /// returned duration. If this field doesn't ever leap, null is returned.
        /// </summary>
        DurationField LeapDurationField { get; }

        #endregion

        #region Ranges

        /// <summary>
        /// Get the maximum allowable value for this field.
        /// </summary>
        /// <returns>The maximum valid value for this field, in the units of the field</returns>
        long GetMaximumValue();

        /// <summary>
        /// Get the maximum value for this field evaluated at the specified time.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The maximum value for this field, in the units of the field</returns>
        long GetMaximumValue(LocalInstant localInstant);

        /// <summary>
        /// Get the maximum value for this field evaluated at the specified time.
        /// </summary>
        /// <param name="instant">The partial instant to query</param>
        /// <returns>The maximum value for this field, in the units of the field</returns>
        long GetMaximumValue(IPartial instant);

        /// <summary>
        /// Get the maximum value for this field using the partial instant and
        /// the specified values.
        /// </summary>
        /// <param name="instant">The partial instant to query</param>
        /// <param name="values">The values to use</param>
        /// <returns>The maximum value for this field, in the units of the field</returns>
        long GetMaximumValue(IPartial instant, int[] values);

        /// <summary>
        /// Get the minimum allowable value for this field.
        /// </summary>
        /// <returns>The minimum valid value for this field, in the units of the field</returns>
        long GetMinimumValue();

        /// <summary>
        /// Get the minimum value for this field evaluated at the specified time.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The minimum value for this field, in the units of the field</returns>
        long GetMinimumValue(LocalInstant localInstant);

        /// <summary>
        /// Get the minimum value for this field evaluated at the specified time.
        /// </summary>
        /// <param name="instant">The partial instant to query</param>
        /// <returns>The minimum value for this field, in the units of the field</returns>
        long GetMinimumValue(IPartial instant);

        /// <summary>
        /// Get the minimum value for this field using the partial instant and the specified values.
        /// </summary>
        /// <param name="instant">The partial instant to query</param>
        /// <param name="values">The values to use</param>
        /// <returns>The minimum value for this field, in the units of the field</returns>
        long GetMinimumValue(IPartial instant, int[] values);

        #endregion

        #region Rounding

        /// <summary>
        /// Round to the lowest whole unit of this field. After rounding, the value
        /// of this field and all fields of a higher magnitude are retained. The
        /// fractional millis that cannot be expressed in whole increments of this
        /// field are set to minimum.
        /// <para>
        /// For example, a datetime of 2002-11-02T23:34:56.789, rounded to the
        /// lowest whole hour is 2002-11-02T23:00:00.000.
        /// </para>
        /// </summary>
        /// <param name="localInstant">The local instant to round</param>
        /// <returns>Rounded local instant</returns>
        LocalInstant RoundFloor(LocalInstant localInstant);

        /// <summary>
        /// Round to the highest whole unit of this field. The value of this field
        /// and all fields of a higher magnitude may be incremented in order to
        /// achieve this result. The fractional millis that cannot be expressed in
        /// whole increments of this field are set to minimum.
        /// <para>
        /// For example, a datetime of 2002-11-02T23:34:56.789, rounded to the
        /// highest whole hour is 2002-11-03T00:00:00.000.
        /// </para>
        /// </summary>
        /// <param name="localInstant">The local instant to round</param>
        /// <returns>Rounded local instant</returns>
        LocalInstant RoundCeiling(LocalInstant localInstant);

        /// <summary>
        /// Round to the nearest whole unit of this field. If the given local instant
        /// is closer to the floor or is exactly halfway, this function
        /// behaves like RoundFloor. If the local instant is closer to the
        /// ceiling, this function behaves like RoundCeiling.
        /// </summary>
        /// <param name="localInstant">The local instant to round</param>
        /// <returns>Rounded local instant</returns>
        LocalInstant RoundHalfFloor(LocalInstant localInstant);

        /// <summary>
        /// Round to the nearest whole unit of this field. If the given local instant
        /// is closer to the floor, this function behaves like RoundFloor. If
        /// the local instant is closer to the ceiling or is exactly halfway,
        /// this function behaves like RoundCeiling.
        /// </summary>
        /// <param name="localInstant">The local instant to round</param>
        /// <returns>Rounded local instant</returns>
        LocalInstant RoundHalfCeiling(LocalInstant localInstant);

        /// <summary>
        /// Round to the nearest whole unit of this field. If the given local instant
        /// is closer to the floor, this function behaves like RoundFloor. If
        /// the local instant is closer to the ceiling, this function behaves
        /// like RoundCeiling.
        /// </summary>
        /// <param name="localInstant">The local instant to round</param>
        /// <returns>Rounded local instant</returns>
        LocalInstant RoundHalfEven(LocalInstant localInstant);

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
        Duration Remainder(LocalInstant localInstant);

        #endregion

        #region Text

        /// <summary>
        /// Get the maximum text value for this field.
        /// </summary>
        /// <param name="provider">The format provider to use</param>
        /// <returns>The maximum text length</returns>
        int GetMaximumTextLength(IFormatProvider provider);

        /// <summary>
        /// Get the maximum short text value for this field.
        /// </summary>
        /// <param name="provider">The format provider to use</param>
        /// <returns>The maximum short text length</returns>
        int GetMaximumShortTextLength(IFormatProvider provider);

        /// <summary>
        /// Get the human-readable, text value of this field from the milliseconds.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <param name="provider">Format provider to use</param>
        /// <returns>The text value of the field</returns>
        string GetAsText(LocalInstant localInstant, IFormatProvider provider);

        /// <summary>
        /// Get the human-readable, text value of this field from the milliseconds.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The text value of the field</returns>
        string GetAsText(LocalInstant localInstant);

        /// <summary>
        /// Get the human-readable, text value of this field from a partial instant.
        /// </summary>
        /// <param name="partial">The partial instant to query</param>
        /// <param name="fieldValue">The field value of this field, provided for performance</param>
        /// <param name="provider">Format provider to use</param>
        /// <returns>The text value of the field</returns>
        string GetAsText(IPartial partial, int fieldValue, IFormatProvider provider);

        /// <summary>
        /// Get the human-readable, text value of this field from a partial instant.
        /// </summary>
        /// <param name="partial">The partial instant to query</param>
        /// <param name="provider">Format provider to use</param>
        /// <returns>The text value of the field</returns>
        string GetAsText(IPartial partial, IFormatProvider provider);

        /// <summary>
        /// Get the human-readable, text value of this field from the field value.
        /// </summary>
        /// <param name="fieldValue">the numeric value to convert to text</param>
        /// <param name="provider">Format provider to use</param>
        /// <returns>The text value of the field</returns>
        string GetAsText(int fieldValue, IFormatProvider provider);

        /// <summary>
        /// Get the human-readable, short text value of this field from the milliseconds.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <param name="provider">Format provider to use</param>
        /// <returns>The text value of the field</returns>
        string GetAsShortText(LocalInstant localInstant, IFormatProvider provider);

        /// <summary>
        /// Get the human-readable, short text value of this field from the milliseconds.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The text value of the field</returns>
        string GetAsShortText(LocalInstant localInstant);

        /// <summary>
        /// Get the human-readable, short text value of this field from a partial instant.
        /// </summary>
        /// <param name="partial">The partial instant to query</param>
        /// <param name="fieldValue">The field value of this field, provided for performance</param>
        /// <param name="provider">Format provider to use</param>
        /// <returns>The text value of the field</returns>
        string GetAsShortText(IPartial partial, int fieldValue, IFormatProvider provider);

        /// <summary>
        /// Get the human-readable, short text value of this field from a partial instant.
        /// </summary>
        /// <param name="partial">The partial instant to query</param>
        /// <param name="provider">Format provider to use</param>
        /// <returns>The text value of the field</returns>
        string GetAsShortText(IPartial partial, IFormatProvider provider);

        /// <summary>
        /// Get the human-readable, short text value of this field from the field value.
        /// </summary>
        /// <param name="fieldValue">the numeric value to convert to text</param>
        /// <param name="provider">Format provider to use</param>
        /// <returns>The text value of the field</returns>
        string GetAsShortText(int fieldValue, IFormatProvider provider);

        #endregion
    }
}