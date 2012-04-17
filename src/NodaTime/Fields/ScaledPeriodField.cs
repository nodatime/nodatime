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

using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// A period field which is simply scaled by a fixed amount.
    /// </summary>
    internal sealed class ScaledPeriodField : PeriodField
    {
        private readonly int scale;
        private readonly PeriodField wrappedField;

        internal ScaledPeriodField(PeriodField wrappedField, PeriodFieldType fieldType, int scale)
            : base(fieldType, ValidateWrappedField(wrappedField).UnitTicks * scale, wrappedField.IsFixedLength, true)
        {
            Preconditions.CheckArgumentRange("scale", scale, 2, int.MaxValue);
            this.scale = scale;
            this.wrappedField = wrappedField;
        }

        private static PeriodField ValidateWrappedField(PeriodField wrappedField)
        {
            Preconditions.CheckNotNull(wrappedField, "wrappedField");
            Preconditions.CheckArgument(wrappedField.IsSupported, "wrappedField", "Wrapped field must be supported");
            return wrappedField;
        }

        internal override int GetValue(Duration duration)
        {
            return wrappedField.GetValue(duration) / scale;
        }

        internal override int GetValue(Duration duration, LocalInstant localInstant)
        {
            return wrappedField.GetValue(duration, localInstant) / scale;
        }

        internal override long GetInt64Value(Duration duration)
        {
            return wrappedField.GetInt64Value(duration) / scale;
        }

        internal override long GetInt64Value(Duration duration, LocalInstant localInstant)
        {
            return wrappedField.GetInt64Value(duration, localInstant) / scale;
        }

        internal override Duration GetDuration(long value)
        {
            return wrappedField.GetDuration(value * scale);
        }

        internal override Duration GetDuration(long value, LocalInstant localInstant)
        {
            return wrappedField.GetDuration(value * scale, localInstant);
        }

        internal override LocalInstant Add(LocalInstant localInstant, int value)
        {
            return wrappedField.Add(localInstant, value * scale);
        }

        internal override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return wrappedField.Add(localInstant, value * scale);
        }

        internal override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return wrappedField.GetDifference(minuendInstant, subtrahendInstant) / scale;
        }

        internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return wrappedField.GetInt64Difference(minuendInstant, subtrahendInstant) / scale;
        }
    }
}