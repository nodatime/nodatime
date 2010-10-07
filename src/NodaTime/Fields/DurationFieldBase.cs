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
    /// Defines the calculation engine for duration fields.
    /// The abstract class defines a set of methods that manipulate a tick duration
    /// with regards to a single field, such as months or seconds. This class is
    /// threadsafe, and all subclasses must be too.
    /// </summary>
    public abstract class DurationFieldBase
    {
        public static bool IsTypeValid(DurationFieldType type)
        {
            return type >= 0 && type <= DurationFieldType.Ticks;
        }

        private readonly DurationFieldType fieldType;

        protected DurationFieldBase(DurationFieldType fieldType)
        {
            if (!IsTypeValid(fieldType))
            {
                throw new ArgumentOutOfRangeException("fieldType");
            }
            this.fieldType = fieldType;
        }

        /// <summary>
        /// Get the type of the field.
        /// </summary>
        public DurationFieldType FieldType { get { return fieldType; } }

        /// <summary>
        /// Returns true if this field is supported.
        /// </summary>
        public abstract bool IsSupported { get; }

        /// <summary>
        /// Is this field precise. A precise field can calculate its value from
        /// milliseconds without needing a reference date. Put another way, a
        /// precise field's unit size is not variable.
        /// </summary>
        public abstract bool IsPrecise { get; }

        /// <summary>
        /// Returns the amount of ticks per unit value of this field.
        /// For example, if this field represents "seconds", then this returns the
        /// ticks in one second.
        /// </summary>
        public abstract long UnitTicks { get; }

        #region Extract field value from a duration
        /// <summary>
        /// Get the value of this field from the ticks, which is approximate
        /// if this field is imprecise.
        /// </summary>
        /// <param name="duration">The duration to query, which may be negative</param>
        /// <returns>The value of the field, in the units of the field, which may be negative</returns>
        public virtual int GetValue(Duration duration)
        {
            return (int)GetInt64Value(duration);
        }

        /// <summary>
        /// Get the value of this field from the ticks, which is approximate
        /// if this field is imprecise.
        /// </summary>
        /// <param name="duration">The duration to query, which may be negative</param>
        /// <returns>The value of the field, in the units of the field, which may be negative</returns>
        public virtual long GetInt64Value(Duration duration)
        {
            return duration.Ticks / UnitTicks;
        }

        /// <summary>
        /// Get the value of this field from the duration relative to an
        /// instant. For precise fields this method produces the same result as for
        /// the single argument get method.
        /// <para>
        /// If the duration is positive, then the instant is treated as a "start instant". 
        /// If negative, the instant is treated as an "end instant".
        /// </para>
        /// </summary>
        /// <param name="duration">The duration to query, which may be negative</param>
        /// <param name="localInstant">The start instant to calculate relative to</param>
        /// <returns>The value of the field, in the units of the field, which may be negative</returns>
        public virtual int GetValue(Duration duration, LocalInstant localInstant)
        {
            return (int)GetInt64Value(duration, localInstant);
        }

        /// <summary>
        /// Get the value of this field from the duration relative to an
        /// instant. For precise fields this method produces the same result as for
        /// the single argument get method.
        /// <para>
        /// If the duration is positive, then the instant is treated as a "start instant". 
        /// If negative, the instant is treated as an "end instant".
        /// </para>
        /// </summary>
        /// <param name="duration">The duration to query, which may be negative</param>
        /// <param name="localInstant">the start instant to calculate relative to</param>
        /// <returns>The value of the field, in the units of the field, which may be negative</returns>
        public abstract long GetInt64Value(Duration duration, LocalInstant localInstant);
        #endregion

        #region Create a duration from a field value
        /// <summary>
        /// Get the duration of this field from its value, which is
        /// approximate if this field is imprecise.
        /// </summary>
        /// <param name="value">The value of the field, which may be negative</param>
        /// <returns>The duration that the field represents, which may be negative</returns>
        public virtual Duration GetDuration(long value)
        {
            return new Duration(value * UnitTicks);
        }

        /// <summary>
        /// Get the duration of this field from its value relative to an instant.
        /// For precise fields this method produces the same result as for
        /// the single argument GetDuration method.
        /// <para>
        /// If the value is positive, then the instant is treated as a "start
        /// instant". If negative, the instant is treated as an "end instant".
        /// </para>
        /// </summary>
        /// <param name="value">The value of the field, which may be negative</param>
        /// <param name="localInstant">The instant to calculate relative to</param>
        /// <returns>The duration that the field represents, which may be negative</returns>
        public abstract Duration GetDuration(long value, LocalInstant localInstant);
        #endregion

        #region Add, subtract, difference
        /// <summary>
        /// Adds a duration value (which may be negative) to the instant.
        /// </summary>
        /// <param name="localInstant">The local instant to add to</param>
        /// <param name="value">The value to add, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        public abstract LocalInstant Add(LocalInstant localInstant, int value);

        /// <summary>
        /// Adds a duration value (which may be negative) to the instant.
        /// </summary>
        /// <param name="localInstant">The local instant to add to</param>
        /// <param name="value">The value to add, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        public abstract LocalInstant Add(LocalInstant localInstant, long value);

        /// <summary>
        /// Subtracts a duration value (which may be negative) from the instant.
        /// </summary>
        /// <param name="localInstant">The local instant to subtract from</param>
        /// <param name="value">The value to subtract, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        public LocalInstant Subtract(LocalInstant localInstant, int value)
        {
            if (value == int.MinValue)
            {
                return Subtract(localInstant, (long)value);
            }
            return Add(localInstant, -value);
        }

        /// <summary>
        /// Subtracts a duration value (which may be negative) from the instant.
        /// </summary>
        /// <param name="localInstant">The local instant to subtract from</param>
        /// <param name="value">The value to subtract, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        public LocalInstant Subtract(LocalInstant localInstant, long value)
        {
            if (value == long.MinValue)
            {
                throw new ArithmeticException("Int64.MinValue cannot be negated");
            }
            return Add(localInstant, -value);
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
        public virtual int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return (int)GetInt64Difference(minuendInstant, subtrahendInstant);
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
        public abstract long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant);
        #endregion

        public int CompareTo(DurationFieldBase other)
        {
            // cannot do (thisMillis - otherMillis) as can overflow

            long otherMillis = other.UnitTicks;
            long thisMillis = UnitTicks;

            return thisMillis == otherMillis ? 0 : thisMillis < otherMillis ? -1 : 1;
        }

        public override string ToString()
        {
            return FieldType.ToString();
        }
    }
}