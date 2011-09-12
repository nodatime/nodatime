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
        internal string Name { get { return FieldType.ToString(); } }

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
                newInstant = DurationField.Add(newInstant, 1);
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

        public override string ToString()
        {
            return fieldType.ToString();
        }
    }
}