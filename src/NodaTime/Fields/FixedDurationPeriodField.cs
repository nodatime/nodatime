// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Fields
{
    /// <summary>
    /// Period field class representing a field with a fixed duration regardless of when it occurs.
    /// </summary>
    internal sealed class FixedDurationPeriodField : IPeriodField
    {
        internal static readonly IPeriodField Ticks = new FixedDurationPeriodField(1);
        internal static readonly IPeriodField Milliseconds = new FixedDurationPeriodField(NodaConstants.TicksPerMillisecond);
        internal static readonly IPeriodField Seconds = new FixedDurationPeriodField(NodaConstants.TicksPerSecond);
        internal static readonly IPeriodField Minutes = new FixedDurationPeriodField(NodaConstants.TicksPerMinute);
        internal static readonly IPeriodField Hours = new FixedDurationPeriodField(NodaConstants.TicksPerHour);
        internal static readonly IPeriodField HalfDays = new FixedDurationPeriodField(NodaConstants.TicksPerStandardDay / 2);
        internal static readonly IPeriodField Days = new FixedDurationPeriodField(NodaConstants.TicksPerStandardDay);
        internal static readonly IPeriodField Weeks = new FixedDurationPeriodField(NodaConstants.TicksPerStandardWeek);

        private readonly ulong unitTicks;

        private FixedDurationPeriodField(ulong unitTicks)
        {
            this.unitTicks = unitTicks;
        }

        public LocalInstant Add(LocalInstant localInstant, long value)
        {
            // It's possible that there are better ways to do this, but this at least feels simple.
            if (value > 0)
            {
                // Check that we wouldn't wrap round *more* than once, by performing
                // this multiplication in a checked context.
                ulong ticks = (ulong) value * unitTicks;

                // Now add in an unchecked context...
                long newTicks;
                unchecked
                {
                    newTicks = localInstant.Ticks + (long) ticks;
                }
                // And check that we're not earlier than we should be.
                if (newTicks < localInstant.Ticks)
                {
                    throw new OverflowException("Period addition overflowed.");
                }
                return new LocalInstant(newTicks);
            }
            else
            {
                ulong positiveValue = value == long.MinValue ? long.MaxValue + 1UL : (ulong)Math.Abs(value);
                // Check that we wouldn't wrap round *more* than once, by performing
                // this multiplication in a checked context.
                ulong ticks = positiveValue * unitTicks;

                // Now add in an unchecked context...
                long newTicks;
                unchecked
                {
                    newTicks = localInstant.Ticks - (long)ticks;
                }
                // And check that we're not later than we should be.
                if (newTicks > localInstant.Ticks)
                {
                    throw new OverflowException("Period addition overflowed.");
                }
                return new LocalInstant(newTicks);                
            }
        }

        public long Subtract(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            ulong ticks;
            if (minuendInstant < subtrahendInstant)
            {
                return -Subtract(subtrahendInstant, minuendInstant);
            }
            unchecked
            {
                // This will handle overflow appropriately, so we'll end up with the right
                // positive value.
                ticks = (ulong)(minuendInstant.Ticks - subtrahendInstant.Ticks);
                // This will naturally truncate towards 0, which is what we want.
            }
            return (long) (ticks / (ulong)unitTicks);
        }
    }
}