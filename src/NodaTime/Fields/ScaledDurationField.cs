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
    /// TODO: Decide whether this wouldn't be better as a DelegatedDurationField...
    /// </summary>
    internal sealed class ScaledDurationField : DecoratedDurationField
    {
        private readonly int scale;

        public ScaledDurationField(DurationField wrappedField, DurationFieldType fieldType, int scale)
            : base(wrappedField, fieldType)
        {
            if (scale == 0 || scale == 1) {
                throw new ArgumentOutOfRangeException("scale", "The scale must not be 0 or 1");
            }
            this.scale = scale;
        }

        public override int GetValue(Duration duration)
        {
            return WrappedField.GetValue(duration) / scale;
        }

        public override int GetValue(Duration duration, LocalInstant localInstant)
        {
            return WrappedField.GetValue(duration, localInstant) / scale;
        }

        public override long GetInt64Value(Duration duration)
        {
            return WrappedField.GetInt64Value(duration) / scale;
        }

        public override long GetInt64Value(Duration duration, LocalInstant localInstant)
        {
            return WrappedField.GetInt64Value(duration, localInstant) / scale;
        }

        public override Duration GetDuration(long value)
        {
            return WrappedField.GetDuration(value * scale);
        }

        public override Duration GetDuration(long value, LocalInstant localInstant)
        {
            return WrappedField.GetDuration(value * scale, localInstant);
        }

        public override LocalInstant Add(LocalInstant localInstant, int value)
        {
            return WrappedField.Add(localInstant, value * scale);
        }

        public override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return WrappedField.Add(localInstant, value * scale);
        }

        public override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return WrappedField.GetDifference(minuendInstant, subtrahendInstant) / scale;
        }

        public override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return WrappedField.GetInt64Difference(minuendInstant, subtrahendInstant) / scale;
        }

        public override long UnitTicks { get { return WrappedField.UnitTicks * scale; } }
    }
}
