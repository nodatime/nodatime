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
    /// Derives from <see cref="DateTimeFieldBase" />, implementing
    /// only the minimum required set of methods. These implemented methods
    /// delegate to a wrapped field.
    /// Porting status: Done.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This design allows new DateTimeField types to be defined that piggyback
    /// on top of another, inheriting all the safe method implementations from
    /// DateTimeFieldBase. Should any method require pure delegation to the wrapped
    /// field, simply override and use the provided WrappedField property.
    /// </para>
    /// <para>
    /// Note that currently following the Joda Time model, this type does not delegate
    /// leap-related methods and properties - those would need to be overridden directly.
    /// However, presumably that's not required as Joda doesn't use it...
    /// </para>
    /// </remarks>
    public abstract class DecoratedDateTimeField : DateTimeFieldBase
    {
        private readonly DateTimeFieldBase wrappedField;

        protected DecoratedDateTimeField(DateTimeFieldBase wrappedField, DateTimeFieldType fieldType) : base(fieldType)
        {
            if (wrappedField == null)
            {
                throw new ArgumentNullException("wrappedField");
            }
            if (!wrappedField.IsSupported)
            {
                throw new ArgumentException("The wrapped field must be supported");
            }
            this.wrappedField = wrappedField;
        }

        /// <summary>
        /// Gets the wrapped date time field.
        /// </summary>
        public DateTimeFieldBase WrappedField { get { return wrappedField; } }

        public override DurationField DurationField { get { return wrappedField.DurationField; } }

        public override DurationField RangeDurationField { get { return wrappedField.RangeDurationField; } }

        public override bool IsLenient { get { return wrappedField.IsLenient; } }

        public override long GetInt64Value(LocalInstant localInstant)
        {
            return wrappedField.GetInt64Value(localInstant);
        }

        public override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            return wrappedField.SetValue(localInstant, value);
        }

        public override long GetMaximumValue()
        {
            return wrappedField.GetMaximumValue();
        }

        public override long GetMinimumValue()
        {
            return wrappedField.GetMinimumValue();
        }

        public override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return wrappedField.RoundFloor(localInstant);
        }
    }
}