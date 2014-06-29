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
            var calendar = localDate.Calendar;
            var calculator = calendar.YearMonthDayCalculator;
            var yearMonthDay = calculator.AddMonths(localDate.YearMonthDay, value);
            return new LocalDate(yearMonthDay, calendar);
        }

        public int Subtract(LocalDate minuendDate, LocalDate subtrahendDate)
        {
            var calculator = minuendDate.Calendar.YearMonthDayCalculator;
            return calculator.MonthsBetween(minuendDate.YearMonthDay, subtrahendDate.YearMonthDay);
        }
    }
}