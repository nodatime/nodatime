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
    /// Precise datetime field, which has a precise unit duration field.
    /// </summary>
    /// <remarks>
    /// </remarks>
    internal abstract class PreciseDurationDateTimeField : DateTimeFieldBase
    {        
        /// <summary>
        /// The fractional unit in ticks
        /// </summary>
        private readonly long unitTicks;

        private readonly DurationField unitField;

        protected PreciseDurationDateTimeField(DateTimeFieldType fieldType, DurationField unit)
            : base(fieldType)
        {
            if (unit == null)
            {
                throw new ArgumentNullException("unit");
            }
            if (!unit.IsPrecise)
            {
                throw new ArgumentException("Unit duration field must be precise");
            }

            unitTicks = unit.UnitTicks;
            if (unitTicks < 1)
            {
                throw new ArgumentException("The unit ticks must be at least one");
            }
            unitField = unit;
        }

        public override bool IsLenient { get { return false; } }

        public override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            FieldUtils.VerifyValueBounds(this, value, GetMinimumValue(),
                                         GetMaximumValueForSet(localInstant, value));
            return new LocalInstant(localInstant.Ticks + (value - GetInt64Value(localInstant)) * unitTicks);
        }

        public override long GetMinimumValue()
        {
            return 0;
        }

        public override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            long ticks = localInstant.Ticks;
            if (ticks >= 0)
            {
                return new LocalInstant(ticks - (ticks % unitTicks));
            }
            else
            {
                ticks++;
                return new LocalInstant(ticks - (ticks % unitTicks) - unitTicks);
            }
        }

        public override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            long ticks = localInstant.Ticks;
            if (ticks > 0)
            {
                ticks--;
                return new LocalInstant(ticks - (ticks % unitTicks) + unitTicks);
            }
            else
            {
                return new LocalInstant(ticks - ticks % unitTicks);
            }
        }

        public override Duration Remainder(LocalInstant localInstant)
        {
            long ticks = localInstant.Ticks;
            return new Duration(ticks >= 0 ? ticks % unitTicks : ((ticks + 1) % unitTicks) + unitTicks - 1);
        }

        public override DurationField DurationField { get { return unitField; } }

        internal long UnitTicks { get { return unitTicks; } }

        /// <summary>
        /// Called by <see cref="DateTimeFieldBase.SetValue" /> to get the maximum allowed
        /// value. By default, returns GetMaximumValue(localInstant). Override to provide
        /// a faster implementation.
        /// </summary>
        protected long GetMaximumValueForSet(LocalInstant localInstant, long value)
        {
            return GetMaximumValue(localInstant);
        }
    }
}
