// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;

namespace NodaTime.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a <see cref="DateTime"/> of any kind to <see cref="LocalDateTime"/>.
        /// </summary>
        /// <remarks>This is a convenience method which calls <see cref="LocalDateTime.FromDateTime(System.DateTime)"/>.</remarks>        
        /// <param name="dateTime">The <c>DateTime</c> to convert.</param>
        /// <returns>A new <see cref="LocalDateTime"/> with the same values as <paramref name="dateTime"/>.</returns>
        public static LocalDateTime ToLocalDateTime(this DateTime dateTime) => LocalDateTime.FromDateTime(dateTime);

        /// <summary>
        /// Converts a <see cref="DateTime"/> with a kind of <see cref="DateTimeKind.Utc"/> into an <see cref="Instant"/>.
        /// </summary>
        /// <remarks>This is a convenience method which calls <see cref="Instant.FromDateTimeUtc"/>.</remarks>        
        /// <param name="dateTime">The <c>DateTime</c> to convert.</param>
        /// <returns>An <see cref="Instant"/> value representing the same instant in time as <paramref name="dateTime"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="dateTime"/> does not have a kind of <c>Utc</c>.</exception>
        public static Instant ToInstant(this DateTime dateTime) => Instant.FromDateTimeUtc(dateTime);
    }
}
