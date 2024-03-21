// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Fields
{
    /// <summary>
    /// All the period fields.
    /// </summary>
    internal static class DatePeriodFields
    {
        internal static IDatePeriodField DaysField { get; } = new FixedLengthDatePeriodField(1);
        internal static IDatePeriodField WeeksField { get; } = new FixedLengthDatePeriodField(7);
        internal static IDatePeriodField MonthsField { get; } = new MonthsPeriodField();
        internal static IDatePeriodField YearsField { get; } = new YearsPeriodField();
    }
}
