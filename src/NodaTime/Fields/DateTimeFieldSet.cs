using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodaTime.Fields
{
    /// <summary>
    /// An immutable collection of DateTimeFields
    /// </summary>
    public sealed class DateTimeFieldSet
    {
        public DateTimeField this[DateTimeFieldType fieldType] { get { throw new NotImplementedException(); } }
    }
}
