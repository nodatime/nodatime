using System;
using System.Collections.Generic;
using System.Text;

namespace NodaTime.Fields
{
    /// <summary>
    /// TODO: Implement.
    /// </summary>
    internal sealed class OffsetDateTimeField : DecoratedDateTimeField
    {
        private readonly int offset;
        private readonly int min;
        private readonly int max;
        internal OffsetDateTimeField(IDateTimeField field, int offset)
            // If the field is null, we want to let the 
            // base constructor throw the exception, rather than
            // fail to dereference it properly here.
            : this(field, field == null ? 0 : field.FieldType,
                offset, int.MinValue, int.MaxValue)
        {
        }

        internal OffsetDateTimeField(IDateTimeField field, 
            DateTimeFieldType fieldType, int offset)
            : this(field, fieldType, offset, int.MinValue, int.MaxValue)
        {
        }

        public OffsetDateTimeField(IDateTimeField field, 
            DateTimeFieldType fieldType, int offset, int minValue, int maxValue)
            : base(field, fieldType)
        {
            if (offset == 0)
            {
                throw new ArgumentOutOfRangeException("offset", "The offset cannot be zero");
            }

            this.offset = offset;
            // This field is only really used for weeks etc - not ticks -
            // so casting the min and max to int should be fine.
            this.min = Math.Max(minValue, (int) field.GetMinimumValue() + offset);
            this.max = Math.Min(maxValue, (int) field.GetMaximumValue() + offset);
        }
    }
}
