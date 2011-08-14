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
    internal class BasicSingleEraDateTimeField : DateTimeField
    {
        private const int EraValue = NodaConstants.CE;

        private readonly string name;

        internal BasicSingleEraDateTimeField(string name) : base(DateTimeFieldType.Era)
        {
            this.name = name;
        }

        internal override bool IsLenient { get { return false; } }

        internal override int GetValue(LocalInstant localInstant)
        {
            return EraValue;
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return EraValue;
        }

        internal override LocalInstant SetValue(LocalInstant instant, string text)
        {
            throw new NotImplementedException();
        }

        internal override LocalInstant SetValue(LocalInstant instant, string text, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            FieldUtils.VerifyValueBounds(this, value, EraValue, EraValue);
            return localInstant;
        }

        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return LocalInstant.MinValue;
        }

        internal override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return LocalInstant.MaxValue;
        }

        internal override LocalInstant RoundHalfFloor(LocalInstant localInstant)
        {
            return LocalInstant.MinValue;
        }

        internal override LocalInstant RoundHalfCeiling(LocalInstant localInstant)
        {
            return LocalInstant.MinValue;
        }

        internal override LocalInstant RoundHalfEven(LocalInstant localInstant)
        {
            return LocalInstant.MinValue;
        }

        internal override DurationField DurationField { get { return UnsupportedDurationField.Eras; } }

        // TODO: Fix this. Joda returns a null. Could return an unsupported field?
        internal override DurationField RangeDurationField
        {
            get { throw new NotSupportedException(); }
        }

        internal override long GetMinimumValue()
        {
            return EraValue;
        }

        internal override long GetMaximumValue()
        {
            return EraValue;
        }
    }
}
