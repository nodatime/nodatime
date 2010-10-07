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
    internal class UnsupportedDateTimeField : DateTimeFieldBase
    {
        private static readonly object cacheLock = new object();
        private static readonly UnsupportedDateTimeField[] cache = new UnsupportedDateTimeField[DateTimeFieldType.MaxOrdinal + 1];

        private readonly DurationField durationField;

        /// <summary>
        /// Returns an instance for the specified field type and duration field.
        /// The returned value is cached.
        /// TODO: Potentially use ReaderWriterLockSlim? Assess performance of caching in the first place...
        /// </summary>
        public static UnsupportedDateTimeField GetInstance(DateTimeFieldType fieldType, DurationField durationField)
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
                if (cached == null || !ReferenceEquals(cached.DurationField, durationField))
                {
                    cached = new UnsupportedDateTimeField(fieldType, durationField);
                    cache[fieldType.Ordinal] = cached;
                }
                return cached;
            }
        }

        private UnsupportedDateTimeField(DateTimeFieldType fieldType, DurationField durationField) : base(fieldType)
        {
            this.durationField = durationField;
        }


        public override DurationField DurationField { get { return durationField; } }

        public override DurationField RangeDurationField { get { return null; } }

        public override DurationField LeapDurationField { get { return null; } }

        public override bool IsSupported { get { return false; } }

        public override bool IsLenient { get { return false; } }

        public override int GetValue(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public override long GetInt64Value(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public override LocalInstant Add(LocalInstant localInstant, int value)
        {
            return durationField.Add(localInstant, value);
        }

        public override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return durationField.Add(localInstant, value);
        }

        public override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return durationField.GetDifference(minuendInstant, subtrahendInstant);
        }

        public override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return durationField.GetInt64Difference(minuendInstant, subtrahendInstant);
        }

        public override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            throw new NotSupportedException();
        }

        public override bool IsLeap(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public override int GetLeapAmount(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public override long GetMaximumValue()
        {
            throw new NotSupportedException();
        }

        public override long GetMaximumValue(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public override long GetMinimumValue()
        {
            throw new NotSupportedException();
        }

        public override long GetMinimumValue(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public override LocalInstant RoundHalfFloor(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public override LocalInstant RoundHalfCeiling(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public override LocalInstant RoundHalfEven(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public override Duration Remainder(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        public override long GetMaximumValue(IPartial instant, int[] values)
        {
            throw new NotImplementedException();
        }

        public override long GetMinimumValue(IPartial instant, int[] values)
        {
            throw new NotImplementedException();
        }

        public override long GetMaximumValue(IPartial instant)
        {
            throw new NotImplementedException();
        }

        public override long GetMinimumValue(IPartial instant)
        {
            throw new NotImplementedException();
        }

        public override string GetAsText(LocalInstant localInstant, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override string GetAsText(LocalInstant localInstant)
        {
            throw new NotImplementedException();
        }

        public override string GetAsText(IPartial partial, int fieldValue, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override string GetAsText(IPartial partial, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override string GetAsText(int fieldValue, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override string GetAsShortText(LocalInstant localInstant, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override string GetAsShortText(LocalInstant localInstant)
        {
            throw new NotImplementedException();
        }

        public override string GetAsShortText(IPartial partial, int fieldValue, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override string GetAsShortText(IPartial partial, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override string GetAsShortText(int fieldValue, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override int[] Add(IPartial instant, int fieldIndex, int[] values, int valueToAdd)
        {
            throw new NotImplementedException();
        }

        public override int[] AddWrapPartial(IPartial instant, int fieldIndex, int[] values, int valueToAdd)
        {
            throw new NotImplementedException();
        }

        public override int[] SetValue(IPartial instant, int fieldIndex, int[] values, int newValue)
        {
            throw new NotImplementedException();
        }

        public override LocalInstant AddWrapField(LocalInstant localInstant, int value)
        {
            throw new NotImplementedException();
        }

        public override LocalInstant SetValue(LocalInstant instant, string text, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override LocalInstant SetValue(LocalInstant instant, string text)
        {
            throw new NotImplementedException();
        }

        public override int[] SetValue(IPartial instant, int fieldIndex, int[] values, string text, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override int GetMaximumTextLength(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public override int GetMaximumShortTextLength(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}