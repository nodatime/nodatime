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
    /// A placeholder implementation to use when a duration field is not supported.
    /// <para>
    /// UnsupportedDurationField is thread-safe and immutable.
    /// </para>
    /// </summary>
    internal sealed class UnsupportedDurationField : DurationField
    {
        private static readonly UnsupportedDurationField[] cache = Array.ConvertAll
            ((DurationFieldType[]) Enum.GetValues(typeof(DurationFieldType)),
             type => new UnsupportedDurationField(type));

        // Convenience fields
        public static readonly UnsupportedDurationField Eras = cache[(int) DurationFieldType.Eras];
        public static readonly UnsupportedDurationField Centuries = cache[(int) DurationFieldType.Centuries];
        public static readonly UnsupportedDurationField WeekYears = cache[(int) DurationFieldType.WeekYears];
        public static readonly UnsupportedDurationField Years = cache[(int) DurationFieldType.Years];
        public static readonly UnsupportedDurationField Months = cache[(int) DurationFieldType.Months];
        public static readonly UnsupportedDurationField Weeks = cache[(int) DurationFieldType.Weeks];
        public static readonly UnsupportedDurationField Days = cache[(int) DurationFieldType.Days];
        public static readonly UnsupportedDurationField HalfDays = cache[(int) DurationFieldType.HalfDays];
        public static readonly UnsupportedDurationField Hours = cache[(int) DurationFieldType.Hours];
        public static readonly UnsupportedDurationField Minutes = cache[(int) DurationFieldType.Minutes];
        public static readonly UnsupportedDurationField Seconds = cache[(int) DurationFieldType.Seconds];
        public static readonly UnsupportedDurationField Milliseconds = cache[(int) DurationFieldType.Milliseconds];
        public static readonly UnsupportedDurationField Ticks = cache[(int) DurationFieldType.Ticks];

        private readonly DurationFieldType fieldType;

        private UnsupportedDurationField(DurationFieldType fieldType)
        {
            this.fieldType = fieldType;
        }

        /// <summary>
        /// Gets an instance of UnsupportedDurationField for a specific named field.
        /// The returned instance is cached.
        /// </summary>
        /// <param name="fieldType">The type to obtain</param>
        /// <returns>The instance</returns>
        public static UnsupportedDurationField ForFieldType(DurationFieldType fieldType)
        {
            if (!DurationFieldBase.IsTypeValid(fieldType))
            {
                throw new ArgumentOutOfRangeException("fieldType");
            }
            return cache[(int) fieldType];
        }

        /// <summary>
        /// Get the type of the field.
        /// </summary>
        public override DurationFieldType FieldType { get { return fieldType; } }

        /// <summary>
        /// This field is not supported, always returns false
        /// </summary>
        public override bool IsSupported { get { return false; } }

        /// <summary>
        /// This field is precise, always returns true
        /// </summary>
        public override bool IsPrecise { get { return true; } }

        /// <summary>
        /// Always returns zero.
        /// </summary>
        public override long UnitTicks { get { return 0; } }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="duration">The duration to query, which may be negative</param>
        /// <returns>The value of the field, in the units of the field, which may be negative</returns>
        public override int GetValue(Duration duration)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="duration">The duration to query, which may be negative</param>
        /// <returns>The value of the field, in the units of the field, which may be negative</returns>
        public override long GetInt64Value(Duration duration)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="duration">The duration to query, which may be negative</param>
        /// <param name="localInstant">The start instant to calculate relative to</param>
        /// <returns>The value of the field, in the units of the field, which may be negative</returns>
        public override int GetValue(Duration duration, LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="duration">The duration to query, which may be negative</param>
        /// <param name="localInstant">The start instant to calculate relative to</param>
        /// <returns>The value of the field, in the units of the field, which may be negative</returns>
        public override long GetInt64Value(Duration duration, LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="value">The value of the field, which may be negative</param>
        /// <returns>The duration that the field represents, which may be negative</returns>
        public override Duration GetDuration(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="value">The value of the field, which may be negative</param>
        /// <param name="localInstant">The instant to calculate relative to</param>
        /// <returns>The duration that the field represents, which may be negative</returns>
        public override Duration GetDuration(long value, LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="localInstant">The local instant to add to</param>
        /// <param name="value">The value to add, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        public override LocalInstant Add(LocalInstant localInstant, int value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="localInstant">The local instant to add to</param>
        /// <param name="value">The value to add, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        public override LocalInstant Add(LocalInstant localInstant, long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="minuendInstant">The local instant to subtract from</param>
        /// <param name="subtrahendInstant">The local instant to subtract from minuendInstant</param>
        /// <returns>The difference in the units of this field</returns>
        public override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="minuendInstant">The local instant to subtract from</param>
        /// <param name="subtrahendInstant">The local instant to subtract from minuendInstant</param>
        /// <returns>The difference in the units of this field</returns
        public override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            throw new NotSupportedException();
        }
    }
}
