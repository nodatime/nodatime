// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Fields
{
    /// <summary>
    /// Period field class representing a field with a fixed duration regardless of when it occurs.
    /// </summary>
    /// <remarks>
    /// 2014-06-29: Tried optimizing time period calculations by making these static methods accepting
    /// the number of ticks. I'd expected that to be really significant, given that it would avoid
    /// finding the object etc. It turned out to make about 10% difference, at the cost of quite a bit
    /// of code elegance.
    /// </remarks>
    internal sealed class TimePeriodField
    {
        internal static readonly TimePeriodField Nanoseconds = new TimePeriodField(1L);
        internal static readonly TimePeriodField Ticks = new TimePeriodField(NodaConstants.NanosecondsPerTick);
        internal static readonly TimePeriodField Milliseconds = new TimePeriodField(NodaConstants.NanosecondsPerMillisecond);
        internal static readonly TimePeriodField Seconds = new TimePeriodField(NodaConstants.NanosecondsPerSecond);
        internal static readonly TimePeriodField Minutes = new TimePeriodField(NodaConstants.NanosecondsPerMinute);
        internal static readonly TimePeriodField Hours = new TimePeriodField(NodaConstants.NanosecondsPerHour);

        private readonly ulong unitNanoseconds;
        private readonly long unitsPerDay;

        public long UnitsPerDay { get { return unitsPerDay; } }

        private TimePeriodField(ulong unitNanoseconds)
        {
            this.unitNanoseconds = unitNanoseconds;
            unitsPerDay = NodaConstants.NanosecondsPerStandardDay / (long) unitNanoseconds;
        }

        internal LocalDateTime Add(LocalDateTime start, long units)
        {
            // TODO(2.0): Consider expanding code below, to avoid all the division etc.
            // Probably not worth doing when the date/time separation is firmer.
            int extraDays = 0;
            LocalTime time = Add(start.TimeOfDay, units, ref extraDays);
            // Even though PlusDays optimizes for "value == 0", it's still quicker not to call it.
            LocalDate date = extraDays == 0 ? start.Date :  start.Date.PlusDays(extraDays);
            return new LocalDateTime(date, time);
        }

        internal LocalTime Add(LocalTime localTime, long value)
        {
            unchecked
            {
                // Arithmetic with a LocalTime wraps round, and every unit divides exactly
                // into a day, so we can make sure we add a value which is less than a day.
                if (value >= 0)
                {
                    if (value >= unitsPerDay)
                    {
                        value = value % unitsPerDay;
                    }
                    long nanosToAdd = value * (long) unitNanoseconds;
                    long newNanos = localTime.NanosecondOfDay + nanosToAdd;
                    if (newNanos >= NodaConstants.NanosecondsPerStandardDay)
                    {
                        newNanos -= NodaConstants.NanosecondsPerStandardDay;
                    }
                    return new LocalTime(newNanos);
                }
                else
                {
                    if (value <= -unitsPerDay)
                    {
                        value = value % unitsPerDay;
                    }
                    long nanosToAdd = value * (long) unitNanoseconds;
                    long newNanos = localTime.NanosecondOfDay + nanosToAdd;
                    if (newNanos < 0)
                    {
                        newNanos += NodaConstants.NanosecondsPerStandardDay;
                    }
                    return new LocalTime(newNanos);
                }
            }
        }

        internal LocalTime Add(LocalTime localTime, long value, ref int extraDays)
        {
            if (value == 0)
            {
                return localTime;
            }
            // It's possible that there are better ways to do this, but this at least feels simple.
            if (value >= 0)
            {
                ulong startNanosecondOfDay = (ulong) localTime.NanosecondOfDay;
                // Check that we wouldn't wrap round *more* than once, by performing
                // this multiplication in a checked context.
                ulong nanoseconds = (ulong) value * unitNanoseconds;

                // Now add in an unchecked context...
                ulong newNanoseconds;
                unchecked
                {
                    newNanoseconds = startNanosecondOfDay + nanoseconds;
                    // And check that we're not earlier than we should be.
                    if (newNanoseconds < startNanosecondOfDay)
                    {
                        throw new OverflowException("Period addition overflowed.");
                    }
                    // If we're still in the same day, we're done.
                    if (newNanoseconds < (ulong) NodaConstants.NanosecondsPerStandardDay)
                    {
                        return LocalTime.FromNanosecondsSinceMidnight((long) newNanoseconds);
                    }
                    // This can never actually overflow, as NodaConstants.NanosecondsPerStandardDay is more than int.MaxValue.
                    extraDays += (int) (newNanoseconds / (ulong) NodaConstants.NanosecondsPerStandardDay);
                    return LocalTime.FromNanosecondsSinceMidnight((long) (newNanoseconds % (ulong) NodaConstants.NanosecondsPerStandardDay));
                }
            }
            else
            {
                ulong positiveValue = value == Int64.MinValue ? Int64.MaxValue + 1UL : (ulong) Math.Abs(value);
                // Check that we wouldn't wrap round *more* than once, by performing
                // this multiplication in a checked context.
                ulong nanoseconds = positiveValue * unitNanoseconds;

                // Now add in an unchecked context...
                long newNanoseconds;
                unchecked
                {
                    newNanoseconds = localTime.NanosecondOfDay - (long) nanoseconds;
                    // And check that we're not later than we should be.
                    if (newNanoseconds > localTime.NanosecondOfDay)
                    {
                        throw new OverflowException("Period addition overflowed.");
                    }
                    if (newNanoseconds < 0)
                    {
                        long remainderNanoseconds = NodaConstants.NanosecondsPerStandardDay + (newNanoseconds % NodaConstants.NanosecondsPerStandardDay);
                        extraDays += (int) ((newNanoseconds + 1) / NodaConstants.NanosecondsPerStandardDay) - 1;
                        return LocalTime.FromNanosecondsSinceMidnight(remainderNanoseconds);
                    }
                    else
                    {
                        // We know we haven't wrapped, so it's easy.
                        return LocalTime.FromNanosecondsSinceMidnight(newNanoseconds);
                    }
                }
            }
        }

        public long Subtract(LocalTime minuendTime, LocalTime subtrahendTime)
        {
            ulong nanoseconds;
            if (minuendTime.NanosecondOfDay < subtrahendTime.NanosecondOfDay)
            {
                return -Subtract(subtrahendTime, minuendTime);
            }
            unchecked
            {
                // We know this won't overflow, as the result must be smallish and positive.
                nanoseconds = (ulong) (minuendTime.NanosecondOfDay - subtrahendTime.NanosecondOfDay);
                // This will naturally truncate towards 0, which is what we want.
                return (long) (nanoseconds / unitNanoseconds);
            }
        }
    }
}