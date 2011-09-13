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
    /// A placeholder implementation to use when a datetime field is not supported.
    /// Operations which can be performed solely on the duration field delegate to that; most
    /// just throw <see cref="NotSupportedException" />.
    /// TODO: See whether we really need the delegation, or whether DurationField could just throw.
    /// </summary>
    internal class UnsupportedDateTimeField : DateTimeField
    {
        private static readonly object cacheLock = new object();
        private static readonly UnsupportedDateTimeField[] cache = new UnsupportedDateTimeField[DateTimeFieldType.MaxOrdinal + 1];

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

        private UnsupportedDateTimeField(DateTimeFieldType fieldType, DurationField durationField) : base(fieldType, durationField, false, false)
        {
        }

        internal override DurationField RangeDurationField { get { return null; } }

        internal override DurationField LeapDurationField { get { return null; } }

        internal override int GetValue(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            throw new NotSupportedException();
        }

        internal override bool IsLeap(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override int GetLeapAmount(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override long GetMaximumValue()
        {
            throw new NotSupportedException();
        }

        internal override long GetMaximumValue(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override long GetMinimumValue()
        {
            throw new NotSupportedException();
        }

        internal override long GetMinimumValue(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override LocalInstant RoundHalfFloor(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override LocalInstant RoundHalfCeiling(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override LocalInstant RoundHalfEven(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override Duration Remainder(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override LocalInstant AddWrapField(LocalInstant localInstant, int value)
        {
            throw new NotImplementedException();
        }
    }
}