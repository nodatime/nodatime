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
    /// A placeholder implementation to use when a datetime field is not supported.
    /// Operations which can be performed solely on the duration field delegate to that; most
    /// just throw <see cref="NotSupportedException" />.
    /// </summary>
    internal class UnsupportedDateTimeField : IDateTimeField
    {
        private static object cacheLock = new object();
        private static readonly UnsupportedDateTimeField[] cache = new UnsupportedDateTimeField[Enum.GetValues(typeof(DateTimeFieldType)).Length];

        private readonly DateTimeFieldType fieldType;
        private readonly DurationField durationField;

        /// <summary>
        /// Returns an instance for the specified field type and duration field.
        /// The returned value is cached.
        /// TODO: Potentially use ReaderWriterLockSlim? Assess performance of caching in the first place...
        /// </summary>
        public static UnsupportedDateTimeField GetInstance(DateTimeFieldType fieldType, DurationField durationField)
        {
            if (!DateTimeFieldBase.IsTypeValid(fieldType))
            {
                throw new ArgumentOutOfRangeException("fieldType");
            }
            if (durationField == null)
            {
                throw new ArgumentNullException("durationField");
            }
            lock (cacheLock)
            {
                UnsupportedDateTimeField cached = cache[(int)fieldType];
                if (cached == null || !object.ReferenceEquals(cached.DurationField, durationField))
                {
                    cached = new UnsupportedDateTimeField(fieldType, durationField);
                    cache[(int)fieldType] = cached;
                }
                return cached;
            }
            
        }

        private UnsupportedDateTimeField(DateTimeFieldType fieldType, DurationField durationField)
        {
            this.fieldType = fieldType;
            this.durationField = durationField;
        }

        public DateTimeFieldType FieldType { get { return fieldType; } }

        public DurationField DurationField { get { return durationField; } }
        
        public DurationField RangeDurationField { get { return null; } }

        public DurationField LeapDurationField { get { return null; } }

        public bool IsSupported { get { return false; } }

        public bool IsLenient { get { return false; } }

        public int GetValue(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public long GetInt64Value(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public LocalInstant Add(LocalInstant localInstant, int value)
        {
            return durationField.Add(localInstant, value);
        }

        public LocalInstant Add(LocalInstant localInstant, long value)
        {
            return durationField.Add(localInstant, value);
        }

        public int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return durationField.GetDifference(minuendInstant, subtrahendInstant);
        }

        public long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return durationField.GetInt64Difference(minuendInstant, subtrahendInstant);
        }

        public LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            throw new NotSupportedException();
        }

        public bool IsLeap(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public int GetLeapAmount(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public long GetMaximumValue()
        {
            throw new NotSupportedException();
        }

        public long GetMaximumValue(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public long GetMinimumValue()
        {
            throw new NotSupportedException();
        }

        public long GetMinimumValue(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public LocalInstant RoundFloor(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public LocalInstant RoundHalfFloor(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public LocalInstant RoundHalfCeiling(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public LocalInstant RoundHalfEven(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public Duration Remainder(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }
    }
}
