// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Calendars
{
    /// <summary>
    /// Common operations on ticks.
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
        internal static int FastTicksToDays(long ticks)
        {
            return unchecked((int) ((ticks >> 14) / 52734375L));
        }

        /// <summary>
        /// Converts a number of ticks into days, rounding down. This method works with any number of
        /// ticks.
        /// </summary>
        internal static int TicksToDays(long ticks)
        {
            unchecked
            {
                return ticks >= 0
                    ? FastTicksToDays(ticks)
                // TODO: Optimize with shifting at some point. Note that this must *not* subtract from ticks,
                // as it could already be long.MinValue.
                : (int) ((ticks + 1) / NodaConstants.TicksPerStandardDay) - 1;
            }
        }

        internal static int TicksToDaysAndTickOfDay(long ticks, out long tickOfDay)
        {
            int ret = TicksToDays(ticks);
            // We're almost always fine to do this...
            if (ticks >= long.MinValue + NodaConstants.TicksPerStandardDay)
            {
                tickOfDay = ticks - ret * NodaConstants.TicksPerStandardDay;
            }
            else
            {
                // Make sure the multiplication doesn't overflow...
                tickOfDay = ticks - (ret + 1) * NodaConstants.TicksPerStandardDay + NodaConstants.TicksPerStandardDay;
            }
            return ret;
        }

        internal static long DaysAndTickOfDayToTicks(int days, long tickOfDay)
        {
            return days >= (int) (long.MinValue / NodaConstants.TicksPerStandardDay)
                ? days * NodaConstants.TicksPerStandardDay + tickOfDay
                : (days + 1) * NodaConstants.TicksPerStandardDay + tickOfDay - NodaConstants.TicksPerStandardDay;

        }
    }
}
