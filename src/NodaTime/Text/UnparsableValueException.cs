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
    }
}
