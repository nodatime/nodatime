// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Fields
{
    /// <summary>
    /// Period field class representing a field with a fixed unit length.
    /// </summary>
    internal sealed class FixedLengthPeriodField : PeriodField
    {
        internal static readonly FixedLengthPeriodField Milliseconds = new FixedLengthPeriodField(PeriodFieldType.Milliseconds, NodaConstants.TicksPerMillisecond);
        internal static readonly FixedLengthPeriodField Seconds = new FixedLengthPeriodField(PeriodFieldType.Seconds, NodaConstants.TicksPerSecond);
        internal static readonly FixedLengthPeriodField Minutes = new FixedLengthPeriodField(PeriodFieldType.Minutes, NodaConstants.TicksPerMinute);
        internal static readonly FixedLengthPeriodField Hours = new FixedLengthPeriodField(PeriodFieldType.Hours, NodaConstants.TicksPerHour);
        internal static readonly FixedLengthPeriodField HalfDays = new FixedLengthPeriodField(PeriodFieldType.HalfDays, NodaConstants.TicksPerStandardDay / 2);
        internal static readonly FixedLengthPeriodField Days = new FixedLengthPeriodField(PeriodFieldType.Days, NodaConstants.TicksPerStandardDay);
        internal static readonly FixedLengthPeriodField Weeks = new FixedLengthPeriodField(PeriodFieldType.Weeks, NodaConstants.TicksPerStandardWeek);

        internal FixedLengthPeriodField(PeriodFieldType type, long unitTicks) : base(type, unitTicks, true, true)
        {
        }

        internal override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return new LocalInstant(localInstant.Ticks + value * UnitTicks);
        }

        internal override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return (minuendInstant.Ticks - subtrahendInstant.Ticks) / UnitTicks;
        }
    }
}