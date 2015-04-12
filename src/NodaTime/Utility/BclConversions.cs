// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Utility
{
    /// <summary>
    /// Conversion methods which don't naturally fit into any other types - for example, for
    /// enums which can't specify any other code. In most cases, conversions to and from BCL types
    /// are provided within the type itself - such as <see cref="LocalDateTime.ToDateTimeUnspecified"/>
    /// and <see cref="LocalDateTime.FromDateTime(System.DateTime)"/>.
    /// </summary>
    /// <remarks>
    /// These methods are also available in the form of extension methods in the <c>NodaTime.Extensions</c> namespace.
    /// </remarks>
    /// <threadsafety>All members of this type are thread-safe. See the thread safety section of the user guide for more information.</threadsafety>
    public static class BclConversions
    {
        /// <summary>
        /// Converts from the Noda Time <see cref="IsoDayOfWeek"/> enum to the equivalent BCL
        /// <see cref="DayOfWeek"/> value. Other than Sunday, the BCL and ISO values are the same -
        /// but ISO 8601 defines Sunday as day 7, and the BCL defines it as day 0.
        /// </summary>
        /// <param name="isoDayOfWeek">ISO day of week value to convert.</param>
        /// <returns>The ISO day of week value equivalent to the one passed in.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="isoDayOfWeek"/> parameter
        /// is not a valid day of the week.</exception>
        public static DayOfWeek ToDayOfWeek(IsoDayOfWeek isoDayOfWeek)
        {
            if (isoDayOfWeek < IsoDayOfWeek.Monday || isoDayOfWeek > IsoDayOfWeek.Sunday)
            {
                throw new ArgumentOutOfRangeException(nameof(isoDayOfWeek));
            }
            return isoDayOfWeek == IsoDayOfWeek.Sunday ? DayOfWeek.Sunday : (DayOfWeek)isoDayOfWeek;
        }

        /// <summary>
        /// Converts from the BCL <see cref="DayOfWeek"/> enum to the equivalent Noda Time <see cref="IsoDayOfWeek"/> value.
        /// Other than Sunday, the BCL and ISO values are the same - but ISO 8601 defines
        /// Sunday as day 7, and the BCL defines it as day 0.
        /// </summary>
        /// <param name="dayOfWeek">ISO day of week value to convert.</param>
        /// <returns>The BCL day of week value equivalent to the one passed in.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="dayOfWeek"/> parameter
        /// is not a valid day of the week.</exception>
        public static IsoDayOfWeek ToIsoDayOfWeek(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
            {
                throw new ArgumentOutOfRangeException(nameof(dayOfWeek));
            }
            return dayOfWeek == DayOfWeek.Sunday ? IsoDayOfWeek.Sunday : (IsoDayOfWeek)dayOfWeek;
        }
    }
}
