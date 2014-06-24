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
    internal sealed class MonthsPeriodField : IDatePeriodField
    {
        internal MonthsPeriodField()
        {
        }

        public LocalDate Add(LocalDate localDate, int value)
        {
            // TODO(2.0): Change the signature of AddMonths to be more date-friendly.
            var calculator = localDate.Calendar.YearMonthDayCalculator;
            LocalInstant newInstant = calculator.AddMonths(localDate.LocalInstant, value);
            return new LocalDate(newInstant, localDate.Calendar);
        }

        public int Subtract(LocalDate minuendDate, LocalDate subtrahendDate)
        {
            // TODO(2.0): Change the signature of MonthsBetween to be more date-friendly.
            var calculator = minuendDate.Calendar.YearMonthDayCalculator;
            return calculator.MonthsBetween(minuendDate.LocalInstant, subtrahendDate.LocalInstant);
        }
    }
}