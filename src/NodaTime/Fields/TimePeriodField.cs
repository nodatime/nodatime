// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Fields
{
    /// <summary>
    /// Period field class representing a field with a fixed duration regardless of when it occurs.
    /// </summary>
    internal sealed class TimePeriodField
    {
        internal static readonly TimePeriodField Ticks = new TimePeriodField(1);
        internal static readonly TimePeriodField Milliseconds = new TimePeriodField(NodaConstants.TicksPerMillisecond);
        internal static readonly TimePeriodField Seconds = new TimePeriodField(NodaConstants.TicksPerSecond);
        internal static readonly TimePeriodField Minutes = new TimePeriodField(NodaConstants.TicksPerMinute);
        internal static readonly TimePeriodField Hours = new TimePeriodField(NodaConstants.TicksPerHour);

        private readonly ulong unitTicks;
        private readonly long unitsPerDay;

        public long UnitsPerDay { get { return unitsPerDay; } }

        private TimePeriodField(ulong unitTicks)
        {
            this.unitTicks = unitTicks;
            unitsPerDay = NodaConstants.TicksPerStandardDay / (long) unitTicks;
        }

        internal LocalDateTime Add(LocalDateTime start, long units)
        {
            // TODO(2.0): Consider expanding code below, to avoid all the division etc.
            // Probably not worth doing when the date/time separation is firmer.
            int extraDays;
            LocalTime time = Add(start.TimeOfDay, units, out extraDays);
            LocalDate date = start.Date.PlusDays(extraDays);
            return new LocalDateTime(date, time);
        }

        internal LocalTime Add(LocalTime localTime, long value, out int extraDays)
        {
            if (value == 0)
            {
                extraDays = 0;
                return localTime;
            }
            // It's possible that there are better ways to do this, but this at least feels simple.
            if (value >= 0)
            {
                // Check that we wouldn't wrap round *more* than once, by performing
                // this multiplication in a checked context.
                ulong ticks = (ulong) value * unitTicks;

                // Now add in an unchecked context...
                ulong newTicks;
                unchecked
                {
                    newTicks = (ulong) localTime.TickOfDay + ticks;
                }
                // And check that we're not earlier than we should be.
                if (newTicks < (ulong) localTime.TickOfDay)
                {
                    throw new OverflowException("Period addition overflowed.");
                }
                // This can never actually overflow, as NodaConstants.TicksPerStandardDay is more than int.MaxValue.
                extraDays = (int) (newTicks / NodaConstants.TicksPerStandardDay);
                return LocalTime.FromTicksSinceMidnight((long) (newTicks % NodaConstants.TicksPerStandardDay));
            }
            else
            {
                ulong positiveValue = value == Int64.MinValue ? Int64.MaxValue + 1UL : (ulong) Math.Abs(value);
                // Check that we wouldn't wrap round *more* than once, by performing
                // this multiplication in a checked context.
                ulong ticks = positiveValue * unitTicks;

                // Now add in an unchecked context...
                long newTicks;
                unchecked
                {
                    newTicks = localTime.TickOfDay - (long) ticks;
                }
                // And check that we're not later than we should be.
                if (newTicks > localTime.TickOfDay)
                {
                    throw new OverflowException("Period addition overflowed.");
                }
                if (newTicks < 0)
                {
                    long remainderTicks = NodaConstants.TicksPerStandardDay + (newTicks % NodaConstants.TicksPerStandardDay);
                    extraDays = (int) ((newTicks + 1) / NodaConstants.TicksPerStandardDay) - 1;
                    return LocalTime.FromTicksSinceMidnight(remainderTicks);
                }
                else
                {
                    // We know we haven't wrapped, so it's easy.
                    extraDays = 0;
                    return LocalTime.FromTicksSinceMidnight(newTicks);
                }
            }
        }

        public long Subtract(LocalTime minuendTime, LocalTime subtrahendTime)
        {
            ulong ticks;
            if (minuendTime.TickOfDay < subtrahendTime.TickOfDay)
            {
                return -Subtract(subtrahendTime, minuendTime);
            }
            unchecked
            {
                // We know this won't overflow, as the result must be smallish and positive.
                ticks = (ulong) (minuendTime.TickOfDay - subtrahendTime.TickOfDay);
                // This will naturally truncate towards 0, which is what we want.
                return (long) (ticks / unitTicks);
            }
        }
    }
}