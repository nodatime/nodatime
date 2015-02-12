// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using static NodaTime.NodaConstants;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Common operations on ticks.
    /// TODO(2.0): Move into Utility? It's no longer used for calendars at all...
    /// </summary>
    internal static class TickArithmetic
    {
        internal static int TicksToDaysAndTickOfDay(long ticks, out long tickOfDay)
        {
            // First work out the number of days, always rounding down (so that ticks * TicksPerDay is always the
            // start of the day).
            // The shift approach here is equivalent to dividing by NodaConstants.TicksPerDay, but appears to be
            // very significantly faster under the x64 JIT (and no slower under the x86 JIT).
            // See http://stackoverflow.com/questions/22258070 for the inspiration.
            int days = ticks >= 0 ? unchecked((int) ((ticks >> 14) / 52734375L))
                    // TODO: Optimize with shifting at some point. Note that this must *not* subtract from ticks,
                    // as it could already be long.MinValue.
                    : (int) ((ticks + 1) / TicksPerDay) - 1;
            // We're almost always fine to do this...
            if (ticks >= long.MinValue + TicksPerDay)
            {
                tickOfDay = ticks - days * TicksPerDay;
            }
            else
            {
                // Make sure the multiplication doesn't overflow...
                tickOfDay = ticks - (days + 1) * TicksPerDay + TicksPerDay;
            }
            return days;
        }

        internal static long DaysAndTickOfDayToTicks(int days, long tickOfDay) =>
            days >= (int) (long.MinValue / TicksPerDay)
                ? days * TicksPerDay + tickOfDay
                : (days + 1) * TicksPerDay + tickOfDay - TicksPerDay;
    }
}
