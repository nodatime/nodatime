// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Period field which uses a <see cref="YearMonthDayCalculator" /> to add/subtract months.
    /// </summary>
    internal sealed class MonthsPeriodField : IPeriodField
    {
        private readonly YearMonthDayCalculator calculator;

        internal MonthsPeriodField(YearMonthDayCalculator calculator)
        {
            this.calculator = calculator;
        }

        public LocalInstant Add(LocalInstant localInstant, long value)
        {
            // We don't try to work out the actual bounds, but we can easily tell
            // that we're out of range. Anything not in the range of an int is definitely broken.
            if (value < Int32.MinValue || value > Int32.MaxValue)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            return calculator.AddMonths(localInstant, (int)value);
        }

        public long Subtract(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return calculator.MonthsBetween(minuendInstant, subtrahendInstant);
        }
    }
}