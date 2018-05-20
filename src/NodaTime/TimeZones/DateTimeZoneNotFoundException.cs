// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Annotations;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Exception thrown when time zone is requested from an <see cref="IDateTimeZoneProvider"/>,
    /// but the specified ID is invalid for that provider.
    /// </summary>
    /// <remarks>
    /// This type originally existed as <c>TimeZoneNotFoundException</c> doesn't exist in framework versions
    /// targeted by earlier versions of Noda Time. It is present now solely to avoid unnecessary
    /// backward incompatibility. While it could be used to distinguish between exceptions thrown by
    /// Noda Time and those thrown by <c>TimeZoneInfo</c>, we recommend that you don't use it that way.
    /// </remarks>
    /// <threadsafety>Any public static members of this type are thread safe. Any instance members are not guaranteed to be thread safe.
    /// See the thread safety section of the user guide for more information.
    /// </threadsafety>
    [Mutable] // Exception itself is mutable
    public sealed class DateTimeZoneNotFoundException : TimeZoneNotFoundException
    {
        /// <summary>
        /// Creates an instance with the given message.
        /// </summary>
        /// <param name="message">The message for the exception.</param>
        public DateTimeZoneNotFoundException(string message) : base(message) { }
    }
}
