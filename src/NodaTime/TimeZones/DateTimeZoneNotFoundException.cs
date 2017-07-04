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
    /// This type only exists as <c>TimeZoneNotFoundException</c> doesn't exist in netstandard1.x.
    /// By creating an exception which derives from <c>TimeZoneNotFoundException</c> on the desktop version
    /// and <c>Exception</c> on the .NET Standard 1.3 version, we achieve reasonable consistency while remaining
    /// backwardly compatible with Noda Time v1 (which was desktop-only, and threw <c>TimeZoneNotFoundException</c>).
    /// </remarks>
    /// <threadsafety>Any public static members of this type are thread safe. Any instance members are not guaranteed to be thread safe.
    /// See the thread safety section of the user guide for more information.
    /// </threadsafety>
#if !NETSTANDARD1_3
    [Serializable]
#endif
    [Mutable] // Exception itself is mutable
    public sealed class DateTimeZoneNotFoundException
#if NETSTANDARD1_3
        : Exception
#else
        : TimeZoneNotFoundException
#endif
    {
        /// <summary>
        /// Creates an instance with the given message.
        /// </summary>
        /// <param name="message">The message for the exception.</param>
        public DateTimeZoneNotFoundException(string message) : base(message) { }
    }
}
