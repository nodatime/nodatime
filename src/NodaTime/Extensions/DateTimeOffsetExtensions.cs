// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;

namespace NodaTime.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="DateTimeOffset"/>.
    /// </summary>
    public static class DateTimeOffsetExtensions
    {
        /// <summary>
        /// Converts a <see cref="DateTimeOffset"/> to <see cref="OffsetDateTime"/>.
        /// </summary>
        /// <remarks>This is a convenience method which calls <see cref="OffsetDateTime.FromDateTimeOffset"/>.</remarks>        
        /// <param name="dateTimeOffset">The <c>DateTimeOffset</c> to convert.</param>
        /// <returns>A new <see cref="OffsetDateTime"/> with the same values as <paramref name="dateTimeOffset"/>.</returns>
        public static OffsetDateTime ToOffsetDateTime(this DateTime dateTimeOffset) =>
            OffsetDateTime.FromDateTimeOffset(dateTimeOffset);

        /// <summary>
        /// Converts a <see cref="DateTimeOffset"/> to <see cref="OffsetDateTime"/>.
        /// </summary>
        /// <remarks>This is a convenience method which calls <see cref="ZonedDateTime.FromDateTimeOffset"/>.</remarks>        
        /// <param name="dateTimeOffset">The <c>DateTimeOffset</c> to convert.</param>
        /// <returns>A new <see cref="ZonedDateTime"/> with the same values as <paramref name="dateTimeOffset"/>.</returns>
        public static ZonedDateTime ToZonedDateTime(this DateTime dateTimeOffset) =>
            ZonedDateTime.FromDateTimeOffset(dateTimeOffset);

        /// <summary>
        /// Converts a <see cref="DateTimeOffset"/> into an <see cref="Instant"/>.
        /// </summary>
        /// <remarks>This is a convenience method which calls <see cref="Instant.FromDateTimeOffset"/>.</remarks>        
        /// <param name="dateTimeOffset">The <c>DateTimeOffset</c> to convert.</param>
        /// <returns>An <see cref="Instant"/> value representing the same instant in time as <paramref name="dateTimeOffset"/>.</returns>
        public static Instant ToInstant(this DateTimeOffset dateTimeOffset) => Instant.FromDateTimeOffset(dateTimeOffset);
    }
}
