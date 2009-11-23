using System;
using System.Collections.Generic;
using System.Text;

namespace NodaTime.Fields
{
    /// <summary>
    /// Derives from <see cref="DateTimeFieldBase" />, implementing
    /// only the minimum required set of methods. These implemented methods
    /// delegate to a wrapped field.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This design allows new DateTimeField types to be defined that piggyback
    /// on top of another, inheriting all the safe method implementations from
    /// BaseDateTimeField. Should any method require pure delegation to the wrapped
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
        private readonly IDateTimeField wrappedField;

        protected DecoratedDateTimeField(IDateTimeField wrappedField, DateTimeFieldType fieldType) 
            : base(fieldType)
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

        public IDateTimeField WrappedField { get { return wrappedField; } }

        public override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return wrappedField.RoundFloor(localInstant);
        }

        public override bool IsLenient { get { return wrappedField.IsLenient; } }

        public override long GetInt64Value(LocalInstant localInstant)
        {
            return wrappedField.GetInt64Value(localInstant);
        }

        public override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            return wrappedField.SetValue(localInstant, value);
        }

        public override DurationField DurationField { get { return wrappedField.DurationField; } }

        public override DurationField RangeDurationField { get { return wrappedField.RangeDurationField; } }

        public override long GetMaximumValue()
        {
            return wrappedField.GetMaximumValue();
        }

        public override long GetMinimumValue()
        {
            return wrappedField.GetMinimumValue();
        }
    }
}
