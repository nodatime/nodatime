// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Fields
{
    /// <summary>
    /// Date period field for fixed-length periods (weeks and days).
    /// </summary>
    internal sealed class FixedLengthDatePeriodField : IDatePeriodField
    {
        private readonly long halfUnitTicks;
        private readonly int unitDays;

        internal FixedLengthDatePeriodField(int unitDays)
        {
            this.unitDays = unitDays;
            this.halfUnitTicks = unitDays * NodaConstants.TicksPerStandardDay / 2;
        }

        public LocalDate Add(LocalDate localDate, int value)
        {
            if (value == 0)
            {
                return localDate;
            }
            // TODO(2.0): This will become simpler when we're not using LocalInstant for dates.
            // For the moment, just perform two half-additions to avoid overflowing.
            return new LocalDate(localDate.LocalInstant
                                          .PlusTicks(value * halfUnitTicks)
                                          .PlusTicks(value * halfUnitTicks),
                                 localDate.Calendar);
        }

        public int Subtract(LocalDate minuendDate, LocalDate subtrahendDate)
        {
            // The tick values are guaranteed to be exact multiples, because the source values are dates.
            int minuendDays = (int) (minuendDate.LocalInstant.Ticks / NodaConstants.TicksPerStandardDay);
            int subtrahendDays = (int) (subtrahendDate.LocalInstant.Ticks / NodaConstants.TicksPerStandardDay);
            return (minuendDays - subtrahendDays) / unitDays;
        }
    }
}
