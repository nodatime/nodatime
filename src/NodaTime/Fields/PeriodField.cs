// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// Defines the calculation engine for period fields.
    /// The abstract class defines a set of methods that manipulate a tick duration
    /// with regards to a single field, such as months or seconds. This class is
    /// threadsafe, and all subclasses must be too.
    /// </summary>
    internal abstract class PeriodField
    {
        internal static bool IsTypeValid(PeriodFieldType type)
        {
            return type >= 0 && type <= PeriodFieldType.Ticks;
        }

        private readonly PeriodFieldType fieldType;
        private readonly long unitTicks;
        private readonly bool fixedLength;
        private readonly bool supported;

        protected PeriodField(PeriodFieldType fieldType, long unitTicks, bool fixedLength, bool supported)
        {
            if (!IsTypeValid(fieldType))
            {
                throw new ArgumentOutOfRangeException("fieldType");
            }
            // TODO(V1.2): Take another look at unsupported fields. Do we really want them?
            // (Hopefully fields will die completely when the Great CalendarSystem Refactoring takes place.)
            if (supported)
            {
                Preconditions.CheckArgumentRange("unitTicks", unitTicks, 1L, long.MaxValue);
            }
            this.fieldType = fieldType;
            this.unitTicks = unitTicks;
            this.fixedLength = fixedLength;
            this.supported = supported;
        }

        /// <summary>
        /// Get the type of the field.
        /// </summary>
        internal PeriodFieldType FieldType { get { return fieldType; } }

        /// <summary>
        /// Returns true if this field is supported.
        /// </summary>
        internal bool IsSupported { get { return supported; } }

        /// <summary>
        /// Returns true if this field has a fixed length, regardless of a reference point.
        /// For example, "minute" is a fixed length field, but "month" is not.
        /// </summary>
        internal bool IsFixedLength { get { return fixedLength; } }

        /// <summary>
        /// Returns the amount of ticks per unit value of this field.
        /// For example, if this field represents "seconds", then this returns the
        /// ticks in one second.
        /// </summary>
        internal long UnitTicks { get { return unitTicks; } }

        #region Create a duration from a field value
        /// <summary>
        /// Get the duration of this field from its value, which is
        /// approximate if this field has a variable length
        /// </summary>
        /// <param name="value">The value of the field, which may be negative</param>
        /// <returns>The duration that the field represents, which may be negative</returns>
        internal virtual Duration GetDuration(long value)
        {
            return new Duration(value * UnitTicks);
        }

        /// <summary>
        /// Get the duration of this field from its value relative to an instant.
        /// For fixed length fields this method produces the same result as for
        /// the single argument GetDuration method.
        /// <para>
        /// If the value is positive, then the instant is treated as a "start
        /// instant". If negative, the instant is treated as an "end instant".
        /// </para>
        /// </summary>
        /// <param name="value">The value of the field, which may be negative</param>
        /// <param name="localInstant">The instant to calculate relative to</param>
        /// <returns>The duration that the field represents, which may be negative</returns>
        internal abstract Duration GetDuration(long value, LocalInstant localInstant);
        #endregion

        #region Add, difference
        /// <summary>
        /// Adds a duration value (which may be negative) to the instant.
        /// </summary>
        /// <param name="localInstant">The local instant to add to</param>
        /// <param name="value">The value to add, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        internal abstract LocalInstant Add(LocalInstant localInstant, int value);

        /// <summary>
        /// Adds a duration value (which may be negative) to the instant.
        /// </summary>
        /// <param name="localInstant">The local instant to add to</param>
        /// <param name="value">The value to add, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        internal abstract LocalInstant Add(LocalInstant localInstant, long value);

        /// <summary>
        /// Computes the difference between two local instants, as measured in the units
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
            return (int)GetInt64Difference(minuendInstant, subtrahendInstant);
        }

        /// <summary>
        /// Computes the difference between two local instants, as measured in the units
        /// of this field. Any fractional units are dropped from the result. Calling
        /// GetInt64Difference reverses the effect of calling Add. In the following code:
        /// <code>
        /// LocalInstant instant = ...
        /// long v = ...
        /// long age = GetInt64Difference(Add(instant, v), instant);
        /// </code>
        /// The value 'age' is the same as the value 'v'.
        /// </summary>
        /// <param name="minuendInstant">The local instant to subtract from</param>
        /// <param name="subtrahendInstant">The local instant to subtract from minuendInstant</param>
        /// <returns>The difference in the units of this field</returns>
        internal abstract long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant);
        #endregion

        public override string ToString()
        {
            return FieldType.ToString();
        }
    }
}