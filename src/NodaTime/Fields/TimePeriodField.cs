// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using static NodaTime.NodaConstants;

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
        internal static readonly TimePeriodField Ticks = new TimePeriodField(NanosecondsPerTick);
        internal static readonly TimePeriodField Milliseconds = new TimePeriodField(NanosecondsPerMillisecond);
        internal static readonly TimePeriodField Seconds = new TimePeriodField(NanosecondsPerSecond);
        internal static readonly TimePeriodField Minutes = new TimePeriodField(NanosecondsPerMinute);
        internal static readonly TimePeriodField Hours = new TimePeriodField(NanosecondsPerHour);

        private readonly long unitNanoseconds;
        // The largest number of units (positive or negative) we can multiply unitNanoseconds by without overflowing a long.
        private readonly long maxLongUnits;
        private readonly long unitsPerDay;

        private TimePeriodField(long unitNanoseconds)
        {
            this.unitNanoseconds = unitNanoseconds;
            maxLongUnits = long.MaxValue / unitNanoseconds;
            unitsPerDay = NanosecondsPerDay / unitNanoseconds;
        }

        internal LocalDateTime Add(LocalDateTime start, long units)
        {
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
                    long nanosToAdd = value * unitNanoseconds;
                    long newNanos = localTime.NanosecondOfDay + nanosToAdd;
                    if (newNanos >= NanosecondsPerDay)
                    {
                        newNanos -= NanosecondsPerDay;
                    }
                    return new LocalTime(newNanos);
                }
                else
                {
                    if (value <= -unitsPerDay)
                    {
                        value = value % unitsPerDay;
                    }
                    long nanosToAdd = value * unitNanoseconds;
                    long newNanos = localTime.NanosecondOfDay + nanosToAdd;
                    if (newNanos < 0)
                    {
                        newNanos += NanosecondsPerDay;
                    }
                    return new LocalTime(newNanos);
                }
            }
        }

        internal LocalTime Add(LocalTime localTime, long value, ref int extraDays)
        {
            unchecked
            {
                if (value == 0)
                {
                    return localTime;
                }
                int days = 0;
                // It's possible that there are better ways to do this, but this at least feels simple.
                if (value >= 0)
                {
                    if (value >= unitsPerDay)
                    {
                        long longDays = value / unitsPerDay;
                        // If this overflows, that's fine. (An OverflowException is a reasonable outcome.)
                        days = checked ((int) longDays);
                        value = value % unitsPerDay;
                    }
                    long nanosToAdd = value * unitNanoseconds;
                    long newNanos = localTime.NanosecondOfDay + nanosToAdd;
                    if (newNanos >= NanosecondsPerDay)
                    {
                        newNanos -= NanosecondsPerDay;
                        days = checked(days + 1);
                    }
                    extraDays = checked(extraDays + days);
                    return new LocalTime(newNanos);
                }
                else
                {
                    if (value <= -unitsPerDay)
                    {
                        long longDays = value / unitsPerDay;
                        // If this overflows, that's fine. (An OverflowException is a reasonable outcome.)
                        days = checked((int) longDays);
                        value = value % unitsPerDay;
                    }
                    long nanosToAdd = value * unitNanoseconds;
                    long newNanos = localTime.NanosecondOfDay + nanosToAdd;
                    if (newNanos < 0)
                    {
                        newNanos += NanosecondsPerDay;
                        days = checked(days - 1);
                    }
                    extraDays = checked(days + extraDays);
                    return new LocalTime(newNanos);
                }
            }
        }

        internal long UnitsBetween(LocalDateTime start, LocalDateTime end)
        {
            LocalInstant startLocalInstant = start.ToLocalInstant();
            LocalInstant endLocalInstant = end.ToLocalInstant();
            Duration duration = endLocalInstant.TimeSinceLocalEpoch - startLocalInstant.TimeSinceLocalEpoch;
            return GetUnitsInDuration(duration);
        }

        /// <summary>
        /// Returns the number of units in the given duration, rounding towards zero.
        /// </summary>
        internal long GetUnitsInDuration(Duration duration) =>
            duration.IsInt64Representable
            ? duration.ToInt64Nanoseconds() / unitNanoseconds
            : (long)(duration.ToDecimalNanoseconds() / unitNanoseconds);

        /// <summary>
        /// Returns a <see cref="Duration"/> representing the given number of units.
        /// </summary>
        internal Duration ToDuration(long units) =>
            units >= -maxLongUnits && units <= maxLongUnits
            ? Duration.FromNanoseconds(units * unitNanoseconds)
            : Duration.FromNanoseconds(units * (decimal)unitNanoseconds);
    }
}