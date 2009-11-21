using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
