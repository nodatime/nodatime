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
using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// Porting status: Done.
    /// </summary>
    internal abstract class DecoratedDurationField : DurationField
    {
        private readonly DurationField wrappedField;

        internal DecoratedDurationField(DurationField wrappedField, DurationFieldType fieldType)
            : this(Preconditions.CheckNotNull(wrappedField, "wrappedField"), fieldType, wrappedField.UnitTicks)
        {
        }

        internal DecoratedDurationField(DurationField wrappedField, DurationFieldType fieldType, long unitTicks)
            : base(fieldType, unitTicks, wrappedField.IsPrecise, true)
        {
            if (wrappedField == null)
            {
                throw new ArgumentNullException("wrappedField");
            }
            if (!wrappedField.IsSupported)
            {
                throw new ArgumentException("The field must be supported", "wrappedField");
            }
            this.wrappedField = wrappedField;
        }

        protected DurationField WrappedField { get { return wrappedField; } }

        internal override long GetInt64Value(Duration duration, LocalInstant localInstant)
        {
            return wrappedField.GetInt64Value(duration, localInstant);
        }

        internal override Duration GetDuration(long value, LocalInstant localInstant)
        {
            return wrappedField.GetDuration(value, localInstant);
        }

        internal override LocalInstant Add(LocalInstant localInstant, int value)
        {
            return wrappedField.Add(localInstant, value);
        }

        internal override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return wrappedField.Add(localInstant, value);
        }

        internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return wrappedField.GetInt64Difference(minuendInstant, subtrahendInstant);
        }
    }
}