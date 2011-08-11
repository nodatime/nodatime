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
    /// DateTimeField which simply delegates every call to another field, allowing
    /// selective members to be overridden.
    /// </summary>
    internal abstract class DelegatedDateTimeField : DateTimeField
    {
        private readonly DateTimeField wrappedField;
        private readonly DateTimeFieldType type;

        internal DelegatedDateTimeField(DateTimeField wrappedField) : this(wrappedField, wrappedField.FieldType)
        {
        }

        internal DelegatedDateTimeField(DateTimeField wrappedField, DateTimeFieldType type) : base(type)
        {
            // No validation: this is internal, and only ever constructed when creating calendar systems.
            // In other words, we'll see it go bang soon enough if we screw up...
            this.wrappedField = wrappedField;
            this.type = type;
        }

        internal DateTimeField WrappedField { get { return wrappedField; } }
        internal override bool IsSupported { get { return wrappedField.IsSupported; } }
        internal override bool IsLenient { get { return wrappedField.IsLenient; } }

        internal override int GetValue(LocalInstant localInstant)
        {
            return wrappedField.GetValue(localInstant);
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return wrappedField.GetInt64Value(localInstant);
        }

        internal override LocalInstant Add(LocalInstant localInstant, int value)
        {
            return wrappedField.Add(localInstant, value);
        }

        internal override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return wrappedField.Add(localInstant, value);
        }

        internal override LocalInstant AddWrapField(LocalInstant localInstant, int value)
        {
            return wrappedField.AddWrapField(localInstant, value);
        }

        internal override DurationField DurationField { get { return wrappedField.DurationField; } }

        internal override string GetAsShortText(int fieldValue, IFormatProvider provider)
        {
            return wrappedField.GetAsShortText(fieldValue, provider);
        }

        internal override string GetAsShortText(LocalInstant localInstant)
        {
            return wrappedField.GetAsShortText(localInstant);
        }

        internal override string GetAsShortText(LocalInstant localInstant, IFormatProvider provider)
        {
            return wrappedField.GetAsShortText(localInstant, provider);
        }

        internal override string GetAsText(int fieldValue, IFormatProvider provider)
        {
            return wrappedField.GetAsText(fieldValue, provider);
        }

        internal override string GetAsText(LocalInstant localInstant)
        {
            return wrappedField.GetAsText(localInstant);
        }

        internal override string GetAsText(LocalInstant localInstant, IFormatProvider provider)
        {
            return wrappedField.GetAsText(localInstant, provider);
        }

        internal override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return wrappedField.GetDifference(minuendInstant, subtrahendInstant);
        }

        internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return wrappedField.GetInt64Difference(minuendInstant, subtrahendInstant);
        }

        internal override int GetLeapAmount(LocalInstant localInstant)
        {
            return wrappedField.GetLeapAmount(localInstant);
        }

        internal override int GetMaximumShortTextLength(IFormatProvider provider)
        {
            return wrappedField.GetMaximumShortTextLength(provider);
        }

        internal override int GetMaximumTextLength(IFormatProvider provider)
        {
            return wrappedField.GetMaximumTextLength(provider);
        }

        internal override long GetMaximumValue()
        {
            return wrappedField.GetMaximumValue();
        }

        internal override long GetMaximumValue(LocalInstant localInstant)
        {
            return wrappedField.GetMaximumValue(localInstant);
        }

        internal override long GetMinimumValue()
        {
            return wrappedField.GetMinimumValue();
        }

        internal override long GetMinimumValue(LocalInstant localInstant)
        {
            return wrappedField.GetMinimumValue(localInstant);
        }

        internal override bool IsLeap(LocalInstant localInstant)
        {
            return wrappedField.IsLeap(localInstant);
        }

        internal override DurationField LeapDurationField { get { return wrappedField.LeapDurationField; } }

        internal override DurationField RangeDurationField { get { return wrappedField.RangeDurationField; } }

        internal override Duration Remainder(LocalInstant localInstant)
        {
            return wrappedField.Remainder(localInstant);
        }

        internal override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return wrappedField.RoundCeiling(localInstant);
        }

        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return wrappedField.RoundFloor(localInstant);
        }

        internal override LocalInstant RoundHalfCeiling(LocalInstant localInstant)
        {
            return wrappedField.RoundHalfCeiling(localInstant);
        }

        internal override LocalInstant RoundHalfEven(LocalInstant localInstant)
        {
            return wrappedField.RoundHalfEven(localInstant);
        }

        internal override LocalInstant RoundHalfFloor(LocalInstant localInstant)
        {
            return wrappedField.RoundHalfFloor(localInstant);
        }

        internal override LocalInstant SetValue(LocalInstant instant, string text)
        {
            return wrappedField.SetValue(instant, text);
        }

        internal override LocalInstant SetValue(LocalInstant instant, string text, IFormatProvider provider)
        {
            return wrappedField.SetValue(instant, text, provider);
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            return wrappedField.SetValue(localInstant, value);
        }
    }
}
