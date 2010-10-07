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

namespace NodaTime.Fields
{
    /// <summary>
    /// Abstract datetime field class that defines its own DurationField, which
    /// delegates back into this ImpreciseDateTimeField.
    /// </summary>
    /// <remarks>
    /// This DateTimeField is useful for defining DateTimeFields that are composed
    /// of imprecise durations. If both duration fields are precise, then a
    /// <see cref="PreciseDurationDateTimeField"/> should be used instead.
    /// <para>
    /// When defining imprecise DateTimeFields where a matching DurationField is
    /// already available, just extend BaseDateTimeField directly so as not to
    /// create redundant DurationField instances.
    /// </para>
    /// <para>
    /// ImpreciseDateTimeField is thread-safe and immutable, and its subclasses must
    /// be as well.
    /// </para>
    /// </remarks>
    internal abstract class ImpreciseDateTimeField : DateTimeField
    {
        private readonly long unitTicks;
        private readonly DurationField durationField;

        protected ImpreciseDateTimeField(DateTimeFieldType fieldType, long unitTicks) : base(fieldType)
        {
            this.unitTicks = unitTicks;
            durationField = new LinkedDurationField(this, fieldType.DurationFieldType);
        }

        public long UnitTicks { get { return unitTicks; } }

        public override DurationField DurationField { get { return durationField; } }

        public abstract override DurationField RangeDurationField { get; }
        public abstract override int GetValue(LocalInstant localInstant);
        public abstract override LocalInstant Add(LocalInstant localInstant, int value);
        public abstract override LocalInstant Add(LocalInstant localInstant, long value);
        public abstract override LocalInstant SetValue(LocalInstant localInstant, long value);

        public override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return (int)GetInt64Difference(minuendInstant, subtrahendInstant);
        }

        public override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            if (minuendInstant < subtrahendInstant)
            {
                return -GetInt64Difference(subtrahendInstant, minuendInstant);
            }

            long difference = (minuendInstant.Ticks - subtrahendInstant.Ticks) / unitTicks;

            // TODO: Check this; it's assymetric (< then <=, but > then > for if/while)
            if (Add(subtrahendInstant, difference) < minuendInstant)
            {
                do
                {
                    difference++;
                } while (Add(subtrahendInstant, difference) <= minuendInstant);
                difference--;
            }
            else if (Add(subtrahendInstant, difference) > minuendInstant)
            {
                do
                {
                    difference--;
                } while (Add(subtrahendInstant, difference) > minuendInstant);
            }
            return difference;
        }

        public abstract override LocalInstant RoundFloor(LocalInstant localInstant);

        private class LinkedDurationField : DurationField
        {
            private readonly ImpreciseDateTimeField linkedField;

            internal LinkedDurationField(ImpreciseDateTimeField linkedField, DurationFieldType fieldType) : base(fieldType)
            {
                this.linkedField = linkedField;
            }

            internal override bool IsSupported { get { return true; } }

            internal override bool IsPrecise { get { return false; } }

            internal override long UnitTicks { get { return linkedField.UnitTicks; } }

            internal override int GetValue(Duration duration, LocalInstant localInstant)
            {
                return linkedField.GetDifference(localInstant + duration, localInstant);
            }

            internal override long GetInt64Value(Duration duration, LocalInstant localInstant)
            {
                return linkedField.GetInt64Difference(localInstant + duration, localInstant);
            }

            internal override Duration GetDuration(long value, LocalInstant localInstant)
            {
                return linkedField.Add(localInstant, value) - localInstant;
            }

            // Note: no GetDuration(int, LocalInstant) to override

            internal override LocalInstant Add(LocalInstant localInstant, int value)
            {
                return linkedField.Add(localInstant, value);
            }

            internal override LocalInstant Add(LocalInstant localInstant, long value)
            {
                return linkedField.Add(localInstant, value);
            }

            internal override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
            {
                return linkedField.GetDifference(minuendInstant, subtrahendInstant);
            }

            internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
            {
                return linkedField.GetInt64Difference(minuendInstant, subtrahendInstant);
            }
        }
    }
}