using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace NodaTime.Format
{
    /// <summary>
    /// Exception thrown to indicate that the specified value could not be parsed.
    /// </summary>
    [Serializable]
    public class UnparsableValueException : FormatException
    {
        public UnparsableValueException()
        {
        }

        public UnparsableValueException(string message)
            : base(message)
        {
        }

        public UnparsableValueException(string format, params string[] args)
            : base(string.Format(format, args))
        {
        }

        protected UnparsableValueException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}
