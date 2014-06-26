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
        private readonly int unitDays;

        internal FixedLengthDatePeriodField(int unitDays)
        {
            this.unitDays = unitDays;
        }

        public LocalDate Add(LocalDate localDate, int value)
        {
            if (value == 0)
            {
                return localDate;
            }
            int days = localDate.DaysSinceEpoch + value * unitDays;
            return new LocalDate(days, localDate.Calendar);
        }

        public int Subtract(LocalDate minuendDate, LocalDate subtrahendDate)
        {
            if (minuendDate == subtrahendDate)
            {
                return 0;
            }
            int minuendDays = minuendDate.DaysSinceEpoch;
            int subtrahendDays = subtrahendDate.DaysSinceEpoch;
            return (minuendDays - subtrahendDays) / unitDays;
        }
    }
}
