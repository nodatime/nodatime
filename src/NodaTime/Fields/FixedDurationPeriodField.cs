// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Fields
{
    /// <summary>
    /// Period field class representing a field with a fixed duration regardless of when it occurs.
    /// </summary>
    internal sealed class FixedDurationPeriodField : IPeriodField
    {
        internal static readonly IPeriodField Milliseconds = new FixedDurationPeriodField(NodaConstants.TicksPerMillisecond);
        internal static readonly IPeriodField Seconds = new FixedDurationPeriodField(NodaConstants.TicksPerSecond);
        internal static readonly IPeriodField Minutes = new FixedDurationPeriodField(NodaConstants.TicksPerMinute);
        internal static readonly IPeriodField Hours = new FixedDurationPeriodField(NodaConstants.TicksPerHour);
        internal static readonly IPeriodField HalfDays = new FixedDurationPeriodField(NodaConstants.TicksPerStandardDay / 2);
        internal static readonly IPeriodField Days = new FixedDurationPeriodField(NodaConstants.TicksPerStandardDay);
        internal static readonly IPeriodField Weeks = new FixedDurationPeriodField(NodaConstants.TicksPerStandardWeek);

        private readonly long unitTicks;

        private FixedDurationPeriodField(long unitTicks)
        {
            this.unitTicks = unitTicks;
        }

        public LocalInstant Add(LocalInstant localInstant, long value)
        {
            return new LocalInstant(localInstant.Ticks + value * unitTicks);
        }

        public long Subtract(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            // This will naturally truncate towards 0, which is what we want.
            return (minuendInstant.Ticks - subtrahendInstant.Ticks) / unitTicks;
        }
    }
}