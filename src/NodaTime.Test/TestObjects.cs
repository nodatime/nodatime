using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <param name="seconds">The number of second, in the range [0, 60).</param>
        /// <param name="fractionalSeconds">The number of milliseconds within the second,
        /// in the range [0, 1000).</param>
        /// <returns>A new <see cref="Offset"/> representing the given values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        public static Offset CreatePositiveOffset(int hours, int minutes, int seconds, int fractionalSeconds)
        {
            Preconditions.CheckArgumentRange("hours", hours, 0, 23);
            Preconditions.CheckArgumentRange("minutes", minutes, 0, 59);
            Preconditions.CheckArgumentRange("seconds", seconds, 0, 59);
            Preconditions.CheckArgumentRange("fractionalSeconds", fractionalSeconds, 0, 999);
            int milliseconds = 0;
            milliseconds += hours * NodaConstants.MillisecondsPerHour;
            milliseconds += minutes * NodaConstants.MillisecondsPerMinute;
            milliseconds += seconds * NodaConstants.MillisecondsPerSecond;
            milliseconds += fractionalSeconds;
            return Offset.FromMilliseconds(milliseconds);
        }

        /// <summary>
        /// Creates a negative offset from the given values.
        /// </summary>
        /// <param name="hours">The number of hours, in the range [0, 24).</param>
        /// <param name="minutes">The number of minutes, in the range [0, 60).</param>
        /// <param name="seconds">The number of second, in the range [0, 60).</param>
        /// <param name="fractionalSeconds">The number of milliseconds within the second,
        /// in the range [0, 1000).</param>
        /// <returns>A new <see cref="Offset"/> representing the given values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        public static Offset CreateNegativeOffset(int hours, int minutes, int seconds, int fractionalSeconds)
        {
            return Offset.FromMilliseconds(-CreatePositiveOffset(hours, minutes, seconds, fractionalSeconds).Milliseconds);
        }
    }
}
