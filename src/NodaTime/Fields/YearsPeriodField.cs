// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NodaTime.Utility;

namespace NodaTime.Fields
{
    internal sealed class YearsPeriodField : IPeriodField
    {
        private readonly YearMonthDayCalculator calculator;

        internal YearsPeriodField(YearMonthDayCalculator calculator)
        {
            this.calculator = calculator;
        }

        public LocalInstant Add(LocalInstant localInstant, long value)
        {
            int currentYear = calculator.GetYear(localInstant);
            // Adjust argument range based on current year
            Preconditions.CheckArgumentRange("value", value, calculator.MinYear - currentYear, calculator.MaxYear - currentYear);
            // If we got this far, the conversion to int must be fine.
            int intValue = (int)value;
            return calculator.SetYear(localInstant, intValue + currentYear);
        }

        public long Subtract(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            int minuendYear = calculator.GetYear(minuendInstant);
            int subtrahendYear = calculator.GetYear(subtrahendInstant);

            int diff = minuendYear - subtrahendYear;

            // If we just add the difference in years to subtrahendInstant, what do we get?
            LocalInstant simpleAddition = Add(subtrahendInstant, diff);

            if (subtrahendInstant <= minuendInstant)
            {
                // Moving forward: if the result of the simple addition is before or equal to the minuend,
                // we're done. Otherwise, rewind a year because we've overshot.
                return simpleAddition <= minuendInstant ? diff : diff - 1;
            }
            else
            {
                // Moving backward: if the result of the simple addition (of a non-positive number)
                // is after or equal to the minuend, we're done. Otherwise, increment by a year because
                // we've overshot backwards.
                return simpleAddition >= minuendInstant ? diff : diff + 1;
            }
        }
    }
}