// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodaTime.Fields
{
    /// <summary>
    /// All the period fields.
    /// </summary>
    internal static class DatePeriodFields
    {
        internal static readonly IDatePeriodField DaysField = new FixedLengthDatePeriodField(1);
        internal static readonly IDatePeriodField WeeksField = new FixedLengthDatePeriodField(7);
        internal static readonly IDatePeriodField MonthsField = new MonthsPeriodField();
        internal static readonly IDatePeriodField YearsField = new YearsPeriodField();
    }
}
