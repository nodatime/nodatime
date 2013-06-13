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
        internal static readonly FieldSet TimeFields = new FieldSet.Builder
        {
            Ticks = new TicksPeriodField(),
            Milliseconds = SimplePeriodField.Milliseconds,
            Seconds = SimplePeriodField.Seconds,
            Minutes = SimplePeriodField.Minutes,
            Hours = SimplePeriodField.Hours,
        }.Build();

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
                return ticks >= 0 ? ticks % NodaConstants.TicksPerStandardDay : (NodaConstants.TicksPerStandardDay - 1) + ((ticks + 1) % NodaConstants.TicksPerStandardDay);                
            }
        }

        internal static int GetTickOfSecond(LocalInstant localInstant)
        {
            return ComputeDividedValue(localInstant, 1, NodaConstants.TicksPerSecond);
        }

        internal static int GetTickOfMillisecond(LocalInstant localInstant)
        {
            return ComputeDividedValue(localInstant, 1, NodaConstants.TicksPerMillisecond);
        }

        internal static int GetMillisecondOfSecond(LocalInstant localInstant)
        {
            return ComputeDividedValue(localInstant, NodaConstants.TicksPerMillisecond, NodaConstants.MillisecondsPerSecond);
        }

        internal static int GetMillisecondOfDay(LocalInstant localInstant)
        {
            return ComputeDividedValue(localInstant, NodaConstants.TicksPerMillisecond, NodaConstants.MillisecondsPerStandardDay);
        }

        internal static int GetSecondOfMinute(LocalInstant localInstant)
        {
            return ComputeDividedValue(localInstant, NodaConstants.TicksPerSecond, NodaConstants.SecondsPerMinute);
        }

        internal static int GetSecondOfDay(LocalInstant localInstant)
        {
            return ComputeDividedValue(localInstant, NodaConstants.TicksPerSecond, NodaConstants.SecondsPerStandardDay);
        }

        internal static int GetMinuteOfHour(LocalInstant localInstant)
        {
            return ComputeDividedValue(localInstant, NodaConstants.TicksPerMinute, NodaConstants.MinutesPerHour);
        }

        internal static int GetMinuteOfDay(LocalInstant localInstant)
        {
            return ComputeDividedValue(localInstant, NodaConstants.TicksPerMinute, NodaConstants.MinutesPerStandardDay);
        }

        internal static int GetHourOfDay(LocalInstant localInstant)
        {
            return ComputeDividedValue(localInstant, NodaConstants.TicksPerHour, NodaConstants.HoursPerStandardDay);
        }

        internal static int GetHourOfHalfDay(LocalInstant localInstant)
        {
            return ComputeDividedValue(localInstant, NodaConstants.TicksPerHour, NodaConstants.HoursPerStandardDay / 2);
        }

        internal static int GetClockHourOfHalfDay(LocalInstant localInstant)
        {
            int hourOfHalfDay = GetHourOfHalfDay(localInstant);
            return hourOfHalfDay == 0 ? 12 : hourOfHalfDay;
        }

        private static int ComputeDividedValue(LocalInstant localInstant, long unitTicks, long upperBound)
        {
            // This is guaranteed not to overflow based on the operations we'll be performing.
            unchecked
            {
                long ticks = localInstant.Ticks;
                long longResult = ticks >= 0 ? (ticks / unitTicks) % upperBound
                                             : upperBound - 1 + (((ticks + 1) / unitTicks) % upperBound);
                return (int)longResult;                
            }
        }

    }
}
