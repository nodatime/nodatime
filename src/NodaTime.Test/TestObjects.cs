// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;

namespace NodaTime.Test
{
    /// <summary>
    /// Helper methods for creating objects of various kinds.
    /// </summary>
    internal static class TestObjects
    {
        /// <summary>
        /// Creates a positive offset from the given values.
        /// </summary>
        /// <param name="hours">The number of hours, in the range [0, 24).</param>
        /// <param name="minutes">The number of minutes, in the range [0, 60).</param>
        /// <param name="seconds">The number of seconds, in the range [0, 60).</param>
        /// <returns>A new <see cref="Offset"/> representing the given values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        public static Offset CreatePositiveOffset(int hours, int minutes, int seconds)
        {
            Preconditions.CheckArgumentRange(nameof(hours), hours, 0, 23);
            Preconditions.CheckArgumentRange(nameof(minutes), minutes, 0, 59);
            Preconditions.CheckArgumentRange(nameof(seconds), seconds, 0, 59);
            seconds += minutes * NodaConstants.SecondsPerMinute;
            seconds += hours * NodaConstants.SecondsPerHour;
            return Offset.FromSeconds(seconds);
        }

        /// <summary>
        /// Creates a negative offset from the given values.
        /// </summary>
        /// <param name="hours">The number of hours, in the range [0, 24).</param>
        /// <param name="minutes">The number of minutes, in the range [0, 60).</param>
        /// <param name="seconds">The number of seconds, in the range [0, 60).</param>
        /// <returns>A new <see cref="Offset"/> representing the given values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        public static Offset CreateNegativeOffset(int hours, int minutes, int seconds)
        {
            return Offset.FromSeconds(-CreatePositiveOffset(hours, minutes, seconds).Seconds);
        }
    }
}
