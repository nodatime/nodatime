// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;

namespace NodaTime.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="TimeSpan"/>.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Converts a <see cref="TimeSpan"/> into a <see cref="Duration"/>.
        /// </summary>
        /// <remarks>This is a convenience method which calls <see cref="Duration.FromTimeSpan"/>.</remarks>        
        /// <param name="timeSpan">The <c>TimeSpan</c> to convert.</param>
        /// <returns>A <c>Duration</c> representing the same length of time as <paramref name="timeSpan"/>.</returns>
        public static Duration ToDuration(this TimeSpan timeSpan)
        {
            return Duration.FromTimeSpan(timeSpan);
        }

        /// <summary>
        /// Converts a <see cref="TimeSpan"/> into an <see cref="Offset"/>.
        /// </summary>
        /// <remarks>This is a convenience method which calls <see cref="Offset.FromTimeSpan"/>.</remarks>        
        /// <param name="timeSpan">The <c>TimeSpan</c> to convert.</param>
        /// <returns>An <c>Offset</c> representing the same length of time as <paramref name="timeSpan"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeSpan"/> is too large or small to
        /// be represented as an <c>Offset</c>.</exception>
        public static Offset ToOffset(this TimeSpan timeSpan)
        {
            return Offset.FromTimeSpan(timeSpan);
        }
    }
}
