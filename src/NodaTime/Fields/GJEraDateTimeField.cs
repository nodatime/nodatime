using System;
using System.Collections.Generic;
using System.Text;
using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// TODO: Implement properly, and consider moving to Calendars namespace.
    /// (To match the chrono namespace in Joda.)
    /// </summary>
    internal sealed class GJEraDateTimeField : DateTimeFieldBase
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal GJEraDateTimeField(BasicCalendarSystem calendarSystem) 
            : base(DateTimeFieldType.Era)
        {
            this.calendarSystem = calendarSystem;
        }

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
