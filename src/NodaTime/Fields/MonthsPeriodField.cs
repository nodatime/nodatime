// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Calendars;

namespace NodaTime.Fields
{
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
            int minuendYear = calculator.GetYear(minuendInstant);
            int subtrahendYear = calculator.GetYear(subtrahendInstant);
            int minuendMonth = calculator.GetMonthOfYear(minuendInstant);
            int subtrahendMonth = calculator.GetMonthOfYear(subtrahendInstant);

            int diff = (minuendYear - subtrahendYear) * calculator.MonthsInYear + minuendMonth - subtrahendMonth;

            // If we just add the difference in months to subtrahendInstant, what do we get?
            LocalInstant simpleAddition = Add(subtrahendInstant, diff);

            if (subtrahendInstant <= minuendInstant)
            {
                // Moving forward: if the result of the simple addition is before or equal to the minuend,
                // we're done. Otherwise, rewind a month because we've overshot.
                return simpleAddition <= minuendInstant ? diff : diff - 1;
            }
            else
            {
                // Moving backward: if the result of the simple addition (of a non-positive number)
                // is after or equal to the minuend, we're done. Otherwise, increment by a month because
                // we've overshot backwards.
                return simpleAddition >= minuendInstant ? diff : diff + 1;
            }
        }
    }
}