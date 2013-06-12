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
        internal static readonly FieldSet TimeFields;
        
        static TimeOfDayCalculator()
        {
            var builder = new FieldSet.Builder
            {
                HourOfDay = CreateDivRemField(NodaConstants.TicksPerHour, NodaConstants.HoursPerStandardDay),
                HourOfHalfDay = CreateDivRemField(NodaConstants.TicksPerHour, NodaConstants.HoursPerStandardDay / 2),
                MinuteOfHour = CreateDivRemField(NodaConstants.TicksPerMinute, NodaConstants.MinutesPerHour),
                MinuteOfDay = CreateDivRemField(NodaConstants.TicksPerMinute, NodaConstants.MinutesPerStandardDay),
                SecondOfMinute = CreateDivRemField(NodaConstants.TicksPerSecond, NodaConstants.SecondsPerMinute),
                SecondOfDay = CreateDivRemField(NodaConstants.TicksPerSecond, NodaConstants.SecondsPerStandardDay),
                MillisecondOfSecond = CreateDivRemField(NodaConstants.TicksPerMillisecond, NodaConstants.MillisecondsPerSecond),
                MillisecondOfDay = CreateDivRemField(NodaConstants.TicksPerMillisecond, NodaConstants.MillisecondsPerStandardDay),
                TickOfMillisecond = CreateDivRemField(1, NodaConstants.TicksPerMillisecond),
                TickOfSecond = CreateDivRemField(1, NodaConstants.TicksPerSecond),
                TickOfDay = CreateDivRemField(1, NodaConstants.TicksPerStandardDay),
            };
            builder.ClockHourOfHalfDay = CreateZeroIsMaxField(builder.HourOfHalfDay, 12);
            TimeFields = builder.Build();
        }

        private static DateTimeField CreateDivRemField(long unitTicks, long upperBound)
        {
            return new Int64DateTimeField(localInstant =>
            {
                long ticks = localInstant.Ticks;
                return ticks >= 0 ? (ticks / unitTicks) % upperBound : upperBound - 1 + (((ticks + 1) / unitTicks) % upperBound);
            });
        }

        // Note: this is inefficient for GetValue.
        private static DateTimeField CreateZeroIsMaxField(DateTimeField field, long max)
        {
            return new Int64DateTimeField(localInstant =>
            {
                long value = field.GetInt64Value(localInstant);
                return value == 0 ? max : value;
            });
        }

        internal static long GetTicks(int hourOfDay, int minuteOfHour, int secondOfMinute, int millisecondOfSecond)
        {
            Preconditions.CheckArgumentRange("hourOfDay", hourOfDay, 0, NodaConstants.HoursPerStandardDay - 1);
            Preconditions.CheckArgumentRange("minuteOfHour", minuteOfHour, 0, NodaConstants.MinutesPerHour - 1);
            Preconditions.CheckArgumentRange("secondOfMinute", secondOfMinute, 0, NodaConstants.SecondsPerMinute - 1);
            Preconditions.CheckArgumentRange("millisecondOfSecond", millisecondOfSecond, 0, NodaConstants.MillisecondsPerSecond - 1);
            return unchecked(hourOfDay * NodaConstants.TicksPerHour +
                 minuteOfHour * NodaConstants.TicksPerMinute +
                 secondOfMinute * NodaConstants.TicksPerSecond +
                 millisecondOfSecond * NodaConstants.TicksPerMillisecond);
        }
    }
}
