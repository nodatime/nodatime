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
    /// A placeholder implementation to use when a datetime field is not supported.
    /// Operations which can be performed solely on the duration field delegate to that; most
    /// just throw <see cref="NotSupportedException" />.
    /// TODO: See whether we really need the delegation, or whether DurationField could just throw.
    /// </summary>
    internal class UnsupportedDateTimeField : IDateTimeField
    {
        private static readonly object cacheLock = new object();
        private static readonly UnsupportedDateTimeField[] cache = new UnsupportedDateTimeField[DateTimeFieldType.MaxOrdinal + 1];

        private readonly DateTimeFieldType fieldType;
        private readonly IDurationField durationField;

        /// <summary>
        /// Returns an instance for the specified field type and duration field.
        /// The returned value is cached.
        /// TODO: Potentially use ReaderWriterLockSlim? Assess performance of caching in the first place...
        /// </summary>
        public static UnsupportedDateTimeField GetInstance(DateTimeFieldType fieldType, IDurationField durationField)
        {
            if (fieldType == null)
            {
                throw new ArgumentNullException("fieldType");
            }
            if (durationField == null)
            {
                throw new ArgumentNullException("durationField");
            }
            lock (cacheLock)
            {
                UnsupportedDateTimeField cached = cache[fieldType.Ordinal];
                if (cached == null || !object.ReferenceEquals(cached.DurationField, durationField))
                {
                    cached = new UnsupportedDateTimeField(fieldType, durationField);
                    cache[fieldType.Ordinal] = cached;
                }
                return cached;
            }
            
        }

        private UnsupportedDateTimeField(DateTimeFieldType fieldType, IDurationField durationField)
        {
            this.fieldType = fieldType;
            this.durationField = durationField;
        }

        public DateTimeFieldType FieldType { get { return fieldType; } }

        public string Name { get { return FieldType.ToString(); } }

        public IDurationField DurationField { get { return durationField; } }
        
        public IDurationField RangeDurationField { get { return null; } }

        public IDurationField LeapDurationField { get { return null; } }

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

        public long GetMaximumValue(IPartial instant, int[] values)
        {
            throw new NotImplementedException();
        }

        public long GetMinimumValue(IPartial instant, int[] values)
        {
            throw new NotImplementedException();
        }


        public long GetMaximumValue(IPartial instant)
        {
            throw new NotImplementedException();
        }

        public long GetMinimumValue(IPartial instant)
        {
            throw new NotImplementedException();
        }


        public string GetAsText(LocalInstant localInstant, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string GetAsText(LocalInstant localInstant)
        {
            throw new NotImplementedException();
        }

        public string GetAsText(IPartial partial, int fieldValue, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string GetAsText(IPartial partial, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string GetAsText(int fieldValue, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string GetAsShortText(LocalInstant localInstant, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string GetAsShortText(LocalInstant localInstant)
        {
            throw new NotImplementedException();
        }

        public string GetAsShortText(IPartial partial, int fieldValue, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string GetAsShortText(IPartial partial, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string GetAsShortText(int fieldValue, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }


        public int[] Add(IPartial instant, int fieldIndex, int[] values, int valueToAdd)
        {
            throw new NotImplementedException();
        }

        public int[] AddWrapPartial(IPartial instant, int fieldIndex, int[] values, int valueToAdd)
        {
            throw new NotImplementedException();
        }

        public int[] SetValue(IPartial instant, int fieldIndex, int[] values, int newValue)
        {
            throw new NotImplementedException();
        }


        public LocalInstant AddWrapField(LocalInstant localInstant, int value)
        {
            throw new NotImplementedException();
        }


        public LocalInstant SetValue(LocalInstant instant, string text, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public LocalInstant SetValue(LocalInstant instant, string text)
        {
            throw new NotImplementedException();
        }

        public int[] SetValue(IPartial instant, int fieldIndex, int[] values, string text, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }




        public int GetMaximumTextLength(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int GetMaximumShortTextLength(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}
