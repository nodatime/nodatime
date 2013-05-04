// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;

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
        private readonly PeriodField periodField;
        private readonly bool isSupported;

        protected DateTimeField(DateTimeFieldType fieldType, PeriodField periodField)
            : this(fieldType, periodField, true)
        {
        }

        protected DateTimeField(DateTimeFieldType fieldType, PeriodField periodField,bool isSupported)
        {
            this.fieldType = Preconditions.CheckNotNull(fieldType, "fieldType");
            this.periodField = Preconditions.CheckNotNull(periodField, "PeriodField");
            this.isSupported = isSupported;
        }

        /// <summary>
        /// Get the type of the field.
        /// </summary>
        internal DateTimeFieldType FieldType { get { return fieldType; } }

        /// <summary>
        /// Gets the duration per unit value of this field, or UnsupportedPeriodField if field has no duration.
        /// For example, if this
        /// field represents "hour of day", then the duration is an hour.
        /// </summary>
        internal PeriodField PeriodField { get { return periodField; } }

        /// <summary>
        /// Whether or not this is a supported field.
        /// </summary>
        internal bool IsSupported { get { return isSupported; } }

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
        /// Sets a value in the local instant supplied.
        /// <para>
        /// The value of this field will be set.
        /// If the value is invalid, an exception if thrown.
        /// </para>
        /// <para>
        /// If setting this field would make other fields invalid, then "smaller" fields
        /// may be changed. For example if the current date is the 31st January, and
        /// the month is set to February, the day would be invalid. Instead, the day
        /// would be changed to the closest value - the 28th/29th February as appropriate.
        /// </para>
        /// <para>
        /// If setting this field is invalid in the context of "larger" fields, an exception
        /// is thrown. For example, if the current date is February 20th, and the day of month
        /// is set to 30, then this is invalid within the larger context.
        /// </para>
        /// </summary>
        /// <param name="localInstant">The local instant to set in</param>
        /// <param name="value">The value to set, in the units of the field</param>
        /// <exception cref="ArgumentOutOfRangeException">The field value is invalid in terms of the "larger" existing fields.</exception>
        /// <returns>The updated local instant</returns>
        internal abstract LocalInstant SetValue(LocalInstant localInstant, long value);
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
                newInstant = PeriodField.Add(newInstant, 1);
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
        #endregion

        public override string ToString()
        {
            return fieldType.ToString();
        }
    }
}
