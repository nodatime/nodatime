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
    /// DurationFieldBase provides the common behaviour for DurationField implementations.
    /// <para>
    /// DurationFieldBase is thread-safe and immutable, and its subclasses must be as well.
    /// </para>
    /// </summary>
    public abstract class DurationFieldBase : IDurationField
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
        public virtual int GetValue(Duration duration)
        {
            return (int) GetInt64Value(duration);
        }

        public virtual long GetInt64Value(Duration duration)
        {
            return duration.Ticks / UnitTicks;
        }

        public virtual int GetValue(Duration duration, LocalInstant localInstant)
        {
            return (int) GetInt64Value(duration, localInstant);
        }

        public abstract long GetInt64Value(Duration duration, LocalInstant localInstant);
        #endregion

        #region Create a duration from a field value
        public virtual Duration GetDuration(long value)
        {
            return new Duration(value * UnitTicks);
        }

        public abstract Duration GetDuration(long value, LocalInstant localInstant);
        #endregion

        #region Add, subtract, difference
        public abstract LocalInstant Add(LocalInstant localInstant, int value);

        public abstract LocalInstant Add(LocalInstant localInstant, long value);

        public LocalInstant Subtract(LocalInstant localInstant, int value)
        {
            if (value == int.MinValue)
            {
                return Subtract(localInstant, (long) value);
            }
            return Add(localInstant, -value);
        }

        public LocalInstant Subtract(LocalInstant instant, long value)
        {
            if (value == long.MinValue)
            {
                throw new ArithmeticException("Int64.MinValue cannot be negated");
            }
            return Add(instant, -value);
        }

        public virtual int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return (int) GetInt64Difference(minuendInstant, subtrahendInstant);
        }

        public abstract long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant);
        #endregion

        public int CompareTo(IDurationField other)
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