using System;
using System.Collections.Generic;
using System.Text;

namespace NodaTime.Fields
{
    /// <summary>
    /// TODO: Implement!
    /// </summary>
    internal abstract class ImpreciseDateTimeField : DateTimeFieldBase
    {
        private readonly long unitTicks;

        protected ImpreciseDateTimeField(DateTimeFieldType fieldType, long unitTicks)
            : base(fieldType)
        {
            this.unitTicks = unitTicks;
        }

        public long UnitTicks { get { return unitTicks; } }

        public override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            throw new NotImplementedException();
        }

        public override bool IsLenient
        {
            get { throw new NotImplementedException(); }
        }

        public override long GetInt64Value(LocalInstant localInstant)
        {
            throw new NotImplementedException();
        }

        public override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            throw new NotImplementedException();
        }

        public override DurationField DurationField
        {
            get { throw new NotImplementedException(); }
        }

        public override DurationField RangeDurationField
        {
            get { throw new NotImplementedException(); }
        }

        public override long GetMaximumValue()
        {
            throw new NotImplementedException();
        }

        public override long GetMinimumValue()
        {
            throw new NotImplementedException();
        }
    }
}
