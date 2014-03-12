// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Fields;
using NodaTime.Utility;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Calculator to handle time-of-day related fields.
    /// This is a static class because we don't intend to model
    /// different lengths of days, or hours etc. (We have no state
    /// at all, and need no polymorphism.)
    /// </summary>
    internal static class TimeOfDayCalculator
    {
        internal static readonly PeriodFieldSet TimeFields = new PeriodFieldSet.Builder
        {
            Ticks = FixedDurationPeriodField.Ticks,
            Milliseconds = FixedDurationPeriodField.Milliseconds,
            Seconds = FixedDurationPeriodField.Seconds,
            Minutes = FixedDurationPeriodField.Minutes,
            Hours = FixedDurationPeriodField.Hours,
        }.Build();

        internal static long GetTicks(int hourOfDay, int minuteOfHour)
        {
            Preconditions.CheckArgumentRange("hourOfDay", hourOfDay, 0, NodaConstants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minuteOfHour", minuteOfHour, 0, NodaConstants.MinutesPerHour - 1);
            return unchecked(hourOfDay * NodaConstants.TicksPerHour +
                 minuteOfHour * NodaConstants.TicksPerMinute);
        }

        internal static long GetTicks(int hourOfDay, int minuteOfHour, int secondOfMinute)
        {
            Preconditions.CheckArgumentRange("hourOfDay", hourOfDay, 0, NodaConstants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minuteOfHour", minuteOfHour, 0, NodaConstants.MinutesPerHour - 1);
            Preconditions.CheckArgumentRange("secondOfMinute", secondOfMinute, 0, NodaConstants.SecondsPerMinute - 1);
            return unchecked(hourOfDay * NodaConstants.TicksPerHour +
                 minuteOfHour * NodaConstants.TicksPerMinute +
                 secondOfMinute * NodaConstants.TicksPerSecond);
        }

        internal static long GetTicks(int hourOfDay, int minuteOfHour, int secondOfMinute, int millisecondOfSecond,
                                      int tickOfMillisecond)
        {
            Preconditions.CheckArgumentRange("hourOfDay", hourOfDay, 0, NodaConstants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minuteOfHour", minuteOfHour, 0, NodaConstants.MinutesPerHour - 1);
            Preconditions.CheckArgumentRange("secondOfMinute", secondOfMinute, 0, NodaConstants.SecondsPerMinute - 1);
            Preconditions.CheckArgumentRange("millisecondOfSecond", millisecondOfSecond, 0, NodaConstants.MillisecondsPerSecond - 1);
            Preconditions.CheckArgumentRange("tickOfMillisecond", tickOfMillisecond, 0, NodaConstants.TicksPerMillisecond - 1);
            return unchecked(hourOfDay * NodaConstants.TicksPerHour +
                 minuteOfHour * NodaConstants.TicksPerMinute +
                 secondOfMinute * NodaConstants.TicksPerSecond +
                 millisecondOfSecond * NodaConstants.TicksPerMillisecond +
                 tickOfMillisecond);
        }

        internal static long GetTickOfDay(LocalInstant localInstant)
        {
            // This is guaranteed not to overflow based on the operations we'll be performing.
            unchecked
            {
                long ticks = localInstant.Ticks;
                if (ticks >= 0)
                {
                    // Surprisingly enough, this is faster than (but equivalent to)
                    // return ticks % NodaConstants.TicksPerStandardDay;
                    int days = TickArithmetic.TicksToDays(ticks);
                    return ticks - ((days * 52734375L) << 14);
                }
                else
                {
                    // I'm sure this can be optimized using shifting, but it's complicated enough as it is...
                    return (NodaConstants.TicksPerStandardDay - 1) + ((ticks + 1) % NodaConstants.TicksPerStandardDay);
                }
            }
        }

        internal static int GetTickOfSecond(LocalInstant localInstant)
        {
            return (int) (GetTickOfDay(localInstant) % NodaConstants.TicksPerSecond);
        }

        internal static int GetTickOfMillisecond(LocalInstant localInstant)
        {
            return (int) (GetTickOfDay(localInstant) % NodaConstants.TicksPerMillisecond);
        }

        internal static int GetMillisecondOfSecond(LocalInstant localInstant)
        {
            return GetMillisecondOfDay(localInstant) % NodaConstants.MillisecondsPerSecond;
        }

        internal static int GetMillisecondOfDay(LocalInstant localInstant)
        {
            return (int) (GetTickOfDay(localInstant) / NodaConstants.TicksPerMillisecond);
        }

        internal static int GetSecondOfMinute(LocalInstant localInstant)
        {
            return GetSecondOfDay(localInstant) % NodaConstants.SecondsPerMinute;
        }

        internal static int GetSecondOfDay(LocalInstant localInstant)
        {
            return (int) (GetTickOfDay(localInstant) / NodaConstants.TicksPerSecond);
        }

        internal static int GetMinuteOfHour(LocalInstant localInstant)
        {
            return GetMinuteOfDay(localInstant) % NodaConstants.MinutesPerHour;
        }

        internal static int GetMinuteOfDay(LocalInstant localInstant)
        {
            return (int) (GetTickOfDay(localInstant) / NodaConstants.TicksPerMinute);
        }

        internal static int GetHourOfDay(LocalInstant localInstant)
        {
            return (int) (GetTickOfDay(localInstant) / NodaConstants.TicksPerHour);
        }

        internal static int GetHourOfHalfDay(LocalInstant localInstant)
        {
            return GetHourOfDay(localInstant) % 12;
        }

        internal static int GetClockHourOfHalfDay(LocalInstant localInstant)
        {
            int hourOfHalfDay = GetHourOfHalfDay(localInstant);
            return hourOfHalfDay == 0 ? 12 : hourOfHalfDay;
        }
    }
}
