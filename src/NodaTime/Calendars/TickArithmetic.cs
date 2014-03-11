// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Calendars
{
    /// <summary>
    /// Just a class to hold optimization methods.
    /// </summary>
    internal static class TickArithmetic
    {
        /// <summary>
        /// Converts a number of ticks into days, rounding down. The number of ticks must be
        /// non-negative (to have an easily-predictable outcome), but this is *not* validated in this method.
        /// This method is equivalent to dividing by NodaConstants.TicksPerStandardDay, but appears to be
        /// very significantly faster under the x64 JIT (and no slower under the x86 JIT).
        /// See http://stackoverflow.com/questions/22258070 for the inspiration.
        /// </summary>
        internal static int TicksToDays(long ticks)
        {
            return unchecked((int) ((ticks >> 14) / 52734375L));
        }
    }
}
