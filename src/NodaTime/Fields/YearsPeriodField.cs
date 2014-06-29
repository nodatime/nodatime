// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// Period field which uses a <see cref="YearMonthDayCalculator" /> to add/subtract years.
    /// </summary>
    internal sealed class YearsPeriodField : IDatePeriodField
    {
        internal YearsPeriodField()
        {
        }

        public LocalDate Add(LocalDate localDate, int value)
        {
            if (value == 0)
            {
                return localDate;
            }
            YearMonthDay yearMonthDay = localDate.YearMonthDay;
            var calendar = localDate.Calendar;
            var calculator = calendar.YearMonthDayCalculator;
            int currentYear = yearMonthDay.Year;
            // Adjust argument range based on current year
            Preconditions.CheckArgumentRange("value", value, calculator.MinYear - currentYear, calculator.MaxYear - currentYear);
            return new LocalDate(calculator.SetYear(yearMonthDay, currentYear + value), calendar);
        }

        public int Subtract(LocalDate minuendDate, LocalDate subtrahendDate)
        {
            int diff = minuendDate.Year - subtrahendDate.Year;

            // If we just add the difference in years to subtrahendInstant, what do we get?
            LocalDate simpleAddition = Add(subtrahendDate, diff);

            if (subtrahendDate <= minuendDate)
            {
                // Moving forward: if the result of the simple addition is before or equal to the minuend,
                // we're done. Otherwise, rewind a year because we've overshot.
                return simpleAddition <= minuendDate ? diff : diff - 1;
            }
            else
            {
                // Moving backward: if the result of the simple addition (of a non-positive number)
                // is after or equal to the minuend, we're done. Otherwise, increment by a year because
                // we've overshot backwards.
                return simpleAddition >= minuendDate ? diff : diff + 1;
            }
        }
    }
}