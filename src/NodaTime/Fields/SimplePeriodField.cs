// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Fields
{
    /// <summary>
    /// Period field class representing a field with a fixed unit length.
    /// </summary>
    internal sealed class SimplePeriodField : IPeriodField
    {
        internal static readonly IPeriodField Milliseconds = new SimplePeriodField(NodaConstants.TicksPerMillisecond);
        internal static readonly IPeriodField Seconds = new SimplePeriodField(NodaConstants.TicksPerSecond);
        internal static readonly IPeriodField Minutes = new SimplePeriodField(NodaConstants.TicksPerMinute);
        internal static readonly IPeriodField Hours = new SimplePeriodField(NodaConstants.TicksPerHour);
        internal static readonly IPeriodField HalfDays = new SimplePeriodField(NodaConstants.TicksPerStandardDay / 2);
        internal static readonly IPeriodField Days = new SimplePeriodField(NodaConstants.TicksPerStandardDay);
        internal static readonly IPeriodField Weeks = new SimplePeriodField(NodaConstants.TicksPerStandardWeek);

        private readonly long unitTicks;

        private SimplePeriodField(long unitTicks)
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