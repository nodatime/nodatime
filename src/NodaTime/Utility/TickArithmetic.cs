// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Annotations;
using static NodaTime.NodaConstants;

namespace NodaTime.Utility
{
    /// <summary>
    /// Common operations on ticks.
    /// </summary>
    internal static class TickArithmetic
    {
        /// <summary>
        /// Cautiously converts a number of ticks (which can have any value) into a number of 
        /// days and a tick within that day.
        /// </summary>
        /// <remarks>
        /// Used by <see cref="Duration.FromTicks(long)"/>.
        /// </remarks>
        internal static int TicksToDaysAndTickOfDay(long ticks, out long tickOfDay)
        {
            unchecked
            {
                // First work out the number of days, always rounding down (so that ticks * TicksPerDay is always the
                // start of the day).
                // The shift approach here is equivalent to dividing by NodaConstants.TicksPerDay, but appears to be
                // very significantly faster under the x64 JIT (and no slower under the x86 JIT).
                // See http://stackoverflow.com/questions/22258070 for the inspiration.
                if (ticks >= 0)
                {
                    int days = (int)((ticks >> 14) / 52734375L);
                    tickOfDay = ticks - days * TicksPerDay;
                    return days;
                }
                else
                {
                    // TODO(optimization): Optimize with shifting at some point. Note that this must *not* subtract from ticks,
                    // as it could already be long.MinValue.
                    int days = (int)((ticks + 1) / TicksPerDay) - 1;
                    // We need to be careful as ticks might be close to long.MinValue, at which point
                    // days * TicksPerDay would overflow. We could validate that, but it only
                    // saves two additions, and introduces a branch.
                    tickOfDay = ticks - (days + 1) * TicksPerDay + TicksPerDay;
                    return days;
                }
            }
        }

        /// <summary>
        /// Similar to <see cref="TicksToDaysAndTickOfDay(long, out long)"/> but
        /// trusting that the input is non-negative, which can be proved in certain cases.
        /// </summary>
        /// <remarks>
        /// Used by <see cref="LocalDateTime.FromDateTime(System.DateTime)"/> and 
        /// <see cref="LocalDateTime.FromDateTime(System.DateTime, CalendarSystem)"/>.
        /// </remarks>
        internal static int NonNegativeTicksToDaysAndTickOfDay([Trusted] long ticks, out long tickOfDay)
        {
            unchecked
            {
                Preconditions.DebugCheckArgument(ticks >= 0, nameof(ticks), "Ticks must be non-negative");
                int days = (int) ((ticks >> 14) / 52734375L);
                tickOfDay = ticks - days * TicksPerDay;
                return days;
            }
        }

        /// <summary>
        /// Cautiously computes a number of ticks from day/tick-of-day value. This may overflow,
        /// but will only do so if it has to.
        /// </summary>
        internal static long DaysAndTickOfDayToTicks(int days, long tickOfDay) =>
            days >= (int) (long.MinValue / TicksPerDay)
                ? days * TicksPerDay + tickOfDay
                : (days + 1) * TicksPerDay + tickOfDay - TicksPerDay;

        /// <summary>
        /// Computes a number of ticks from a day/tick-of-day value which is trusted not to overflow,
        /// even when computed in the simplest way. Only call this method from places where there
        /// are suitable constraints on the input.
        /// </summary>
        internal static long BoundedDaysAndTickOfDayToTicks([Trusted] int days, [Trusted] long tickOfDay)
        {
            Preconditions.DebugCheckArgumentRange(nameof(days), days,
                (int) (long.MinValue / TicksPerDay),
                (int) (long.MaxValue / TicksPerDay));
            unchecked
            {
                return days * TicksPerDay + tickOfDay;
            }
        }
    }
}
