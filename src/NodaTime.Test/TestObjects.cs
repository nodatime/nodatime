#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
