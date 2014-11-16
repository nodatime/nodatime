// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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

        private readonly long unitNanoseconds;

        public long UnitsPerDay { get; }

        private TimePeriodField(long unitNanoseconds)
        {
            this.unitNanoseconds = unitNanoseconds;
            UnitsPerDay = NodaConstants.NanosecondsPerDay / unitNanoseconds;
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
                    if (value >= UnitsPerDay)
                    {
                        value = value % UnitsPerDay;
                    }
                    long nanosToAdd = value * unitNanoseconds;
                    long newNanos = localTime.NanosecondOfDay + nanosToAdd;
                    if (newNanos >= NodaConstants.NanosecondsPerDay)
                    {
                        newNanos -= NodaConstants.NanosecondsPerDay;
                    }
                    return new LocalTime(newNanos);
                }
                else
                {
                    if (value <= -UnitsPerDay)
                    {
                        value = value % UnitsPerDay;
                    }
                    long nanosToAdd = value * unitNanoseconds;
                    long newNanos = localTime.NanosecondOfDay + nanosToAdd;
                    if (newNanos < 0)
                    {
                        newNanos += NodaConstants.NanosecondsPerDay;
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
                    if (value >= UnitsPerDay)
                    {
                        long longDays = value / UnitsPerDay;
                        // If this overflows, that's fine. (An OverflowException is a reasonable outcome.)
                        days = checked ((int) longDays);
                        value = value % UnitsPerDay;
                    }
                    long nanosToAdd = value * unitNanoseconds;
                    long newNanos = localTime.NanosecondOfDay + nanosToAdd;
                    if (newNanos >= NodaConstants.NanosecondsPerDay)
                    {
                        newNanos -= NodaConstants.NanosecondsPerDay;
                        days = checked(days + 1);
                    }
                    extraDays = checked(extraDays + days);
                    return new LocalTime(newNanos);
                }
                else
                {
                    if (value <= -UnitsPerDay)
                    {
                        long longDays = value / UnitsPerDay;
                        // If this overflows, that's fine. (An OverflowException is a reasonable outcome.)
                        days = checked((int) longDays);
                        value = value % UnitsPerDay;
                    }
                    long nanosToAdd = value * unitNanoseconds;
                    long newNanos = localTime.NanosecondOfDay + nanosToAdd;
                    if (newNanos < 0)
                    {
                        newNanos += NodaConstants.NanosecondsPerDay;
                        days = checked(days - 1);
                    }
                    extraDays = checked(days + extraDays);
                    return new LocalTime(newNanos);
                }
            }
        }

        public long Subtract(LocalTime minuendTime, LocalTime subtrahendTime)
        {
            if (minuendTime.NanosecondOfDay < subtrahendTime.NanosecondOfDay)
            {
                return -Subtract(subtrahendTime, minuendTime);
            }
            unchecked
            {
                // We know this won't overflow, as the result must be smallish and positive.
                long nanoseconds = (minuendTime.NanosecondOfDay - subtrahendTime.NanosecondOfDay);
                // This will naturally truncate towards 0, which is what we want.
                return nanoseconds / unitNanoseconds;
            }
        }
    }
}