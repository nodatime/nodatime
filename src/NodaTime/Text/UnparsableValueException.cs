// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Annotations;
using System;

namespace NodaTime.Text
{
    /// <summary>
    /// Exception thrown to indicate that the specified value could not be parsed.
    /// </summary>
    /// <threadsafety>Any public static members of this type are thread safe. Any instance members are not guaranteed to be thread safe.
    /// See the thread safety section of the user guide for more information.
    /// </threadsafety>
    [Mutable] // Exception is Mutable
    public sealed class UnparsableValueException : FormatException
    {
        /// <summary>
        /// Creates a new UnparsableValueException with no message, value or index.
        /// <see cref="Value"/> will be an empty string, and <see cref="Index"/> will have a value of -1.
        /// </summary>
        [Obsolete("Use constructors accepting a value")]
        public UnparsableValueException()
        {
            Value = "";
            Index = -1;
        }

        /// <summary>
        /// Creates a new UnparsableValueException with the given message, but no value or index.
        /// <see cref="Value"/> will be an empty string, and <see cref="Index"/> will have a value of -1.
        /// </summary>
        /// <param name="message">The failure message</param>
        [Obsolete("Use constructors accepting a value")]
        public UnparsableValueException(string message) : base(message)
        {
            Value = "";
            Index = -1;
        }

        /// <summary>
        /// Creates a new UnparsableValueException with the given message and base exception, but no value or index.
        /// <see cref="Value"/> will be an empty string, and <see cref="Index"/> will have a value of -1.
        /// </summary>
        /// <param name="message">The failure message</param>
        /// <param name="innerException">The inner exception</param>
        [Obsolete("Use constructors accepting a value")]
        public UnparsableValueException(string message, Exception innerException) : base(message, innerException)
        {
            Value = "";
            Index = -1;
        }

        /// <summary>
        /// Creates a new UnparsableValueException with the given message, value and index
        /// at which parsing failed.
        /// </summary>
        /// <param name="message">The failure message</param>
        /// <param name="value">The value which could not be parsed</param>
        /// <param name="index">The index within the value where parsing failed</param>
        public UnparsableValueException(string message, string value, int index)
            : base(message)
        {
            Value = value;
            Index = index;
        }

        /// <summary>
        /// Creates a new UnparsableValueException with the given message, value, index and inner exception.
        /// </summary>
        /// <param name="message">The failure message</param>
        /// <param name="value">The value which could not be parsed</param>
        /// <param name="index">The index within the value where parsing failed</param>
        /// <param name="innerException">The inner exception</param>
        public UnparsableValueException(string message, string value, int index, Exception innerException) : base(message, innerException)
        {
            Value = value;
            Index = index;
        }
        
        /// <summary>
        /// The value which could not be parsed.
        /// </summary>
        public string Value { get; private set; }
        
        /// <summary>
        /// The index within the value where parsing failed.
        /// This will be -1 if parsing failed before examining any text,
        /// for example because parsing was requested on a format-only pattern, or
        /// the value to be parsed was empty.
        /// </summary>
        public int Index { get; private set; }
    }
}
