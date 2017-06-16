// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Annotations;
#if !NETSTANDARD1_3
using System.Runtime.Serialization;
#endif

namespace NodaTime.Text
{
    /// <summary>
    /// Exception thrown to indicate that the specified value could not be parsed.
    /// </summary>
    /// <threadsafety>Any public static members of this type are thread safe. Any instance members are not guaranteed to be thread safe.
    /// See the thread safety section of the user guide for more information.
    /// </threadsafety>
#if !NETSTANDARD1_3
    [Serializable]
#endif
    [Mutable] // Exception is Mutable
    public sealed class UnparsableValueException : FormatException
    {
        /// <summary>
        /// Creates a new UnparsableValueException with no message.
        /// </summary>
        public UnparsableValueException()
        {
        }

        /// <summary>
        /// Creates a new UnparsableValueException with the given message.
        /// </summary>
        /// <param name="message">The failure message</param>
        public UnparsableValueException(string message)
            : base(message)
        {
        }
#if !NETSTANDARD1_3
        /// <summary>
        /// Creates a new UnparsableValueException from the given serialization information.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        private UnparsableValueException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
