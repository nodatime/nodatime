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
    /// A placeholder implementation to use when a period field is not supported.
    /// <para>
    /// UnsupportedPeriodField is thread-safe and immutable.
    /// </para>
    /// </summary>
    internal sealed class UnsupportedPeriodField : PeriodField
    {
        private static readonly UnsupportedPeriodField[] cache = Array.ConvertAll((PeriodFieldType[])Enum.GetValues(typeof(PeriodFieldType)),
                                                                                    type => new UnsupportedPeriodField(type));

        // Convenience fields
        public static readonly UnsupportedPeriodField Eras = cache[(int)PeriodFieldType.Eras];
        public static readonly UnsupportedPeriodField Centuries = cache[(int)PeriodFieldType.Centuries];
        public static readonly UnsupportedPeriodField WeekYears = cache[(int)PeriodFieldType.WeekYears];
        public static readonly UnsupportedPeriodField Years = cache[(int)PeriodFieldType.Years];
        public static readonly UnsupportedPeriodField Months = cache[(int)PeriodFieldType.Months];
        public static readonly UnsupportedPeriodField Weeks = cache[(int)PeriodFieldType.Weeks];
        public static readonly UnsupportedPeriodField Days = cache[(int)PeriodFieldType.Days];
        public static readonly UnsupportedPeriodField HalfDays = cache[(int)PeriodFieldType.HalfDays];
        public static readonly UnsupportedPeriodField Hours = cache[(int)PeriodFieldType.Hours];
        public static readonly UnsupportedPeriodField Minutes = cache[(int)PeriodFieldType.Minutes];
        public static readonly UnsupportedPeriodField Seconds = cache[(int)PeriodFieldType.Seconds];
        public static readonly UnsupportedPeriodField Milliseconds = cache[(int)PeriodFieldType.Milliseconds];
        public static readonly UnsupportedPeriodField Ticks = cache[(int)PeriodFieldType.Ticks];

        private UnsupportedPeriodField(PeriodFieldType fieldType) : base(fieldType, 0, true, false)
        {
        }

        /// <summary>
        /// Gets an instance of UnsupportedPeriodField for a specific named field.
        /// The returned instance is cached.
        /// </summary>
        /// <param name="fieldType">The type to obtain</param>
        /// <returns>The instance</returns>
        public static UnsupportedPeriodField ForFieldType(PeriodFieldType fieldType)
        {
            if (!IsTypeValid(fieldType))
            {
                throw new ArgumentOutOfRangeException("fieldType");
            }
            return cache[(int)fieldType];
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="duration">The duration to query, which may be negative</param>
        /// <returns>The value of the field, in the units of the field, which may be negative</returns>
        internal override int GetValue(Duration duration)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="duration">The duration to query, which may be negative</param>
        /// <returns>The value of the field, in the units of the field, which may be negative</returns>
        internal override long GetInt64Value(Duration duration)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="duration">The duration to query, which may be negative</param>
        /// <param name="localInstant">The start instant to calculate relative to</param>
        /// <returns>The value of the field, in the units of the field, which may be negative</returns>
        internal override int GetValue(Duration duration, LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="duration">The duration to query, which may be negative</param>
        /// <param name="localInstant">The start instant to calculate relative to</param>
        /// <returns>The value of the field, in the units of the field, which may be negative</returns>
        internal override long GetInt64Value(Duration duration, LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="value">The value of the field, which may be negative</param>
        /// <returns>The duration that the field represents, which may be negative</returns>
        internal override Duration GetDuration(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="value">The value of the field, which may be negative</param>
        /// <param name="localInstant">The instant to calculate relative to</param>
        /// <returns>The duration that the field represents, which may be negative</returns>
        internal override Duration GetDuration(long value, LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="localInstant">The local instant to add to</param>
        /// <param name="value">The value to add, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        internal override LocalInstant Add(LocalInstant localInstant, int value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="localInstant">The local instant to add to</param>
        /// <param name="value">The value to add, in the units of the field</param>
        /// <returns>The updated local instant</returns>
        internal override LocalInstant Add(LocalInstant localInstant, long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="minuendInstant">The local instant to subtract from</param>
        /// <param name="subtrahendInstant">The local instant to subtract from minuendInstant</param>
        /// <returns>The difference in the units of this field</returns>
        internal override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Always throws NotSupportedException
        /// </summary>
        /// <param name="minuendInstant">The local instant to subtract from</param>
        /// <param name="subtrahendInstant">The local instant to subtract from minuendInstant</param>
        /// <returns>The difference in the units of this field</returns>
        internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            throw new NotSupportedException();
        }
    }
}