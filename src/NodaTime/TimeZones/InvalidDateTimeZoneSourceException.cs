// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Annotations;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Exception thrown to indicate that a time zone source has violated the contract of <see cref="IDateTimeZoneSource"/>.
    /// This exception is primarily intended to be thrown from <see cref="DateTimeZoneCache"/>, and only in the face of a buggy
    /// source; user code should not usually need to be aware of this or catch it.
    /// </summary>
    /// <threadsafety>Any public static members of this type are thread safe. Any instance members are not guaranteed to be thread safe.
    /// See the thread safety section of the user guide for more information.
    /// </threadsafety>
#if !PCL
    [Serializable]
#endif
    [Mutable] // Exception itself is mutable
    public sealed class InvalidDateTimeZoneSourceException : Exception
    {
        /// <summary>
        /// Creates a new instance with the given message.
        /// </summary>
        /// <param name="message">The message for the exception.</param>
        public InvalidDateTimeZoneSourceException(string message)
            : base(message)
        {
        }
    }
}
