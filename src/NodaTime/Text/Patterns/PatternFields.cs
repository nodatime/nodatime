// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Enum representing the fields available within patterns. This single enum is shared
    /// by all parser types for simplicity, although most fields aren't used by most parsers.
    /// Pattern fields don't necessarily have corresponding duration or date/time fields,
    /// due to concepts such as "sign".
    /// </summary>
    [Flags]
    internal enum PatternFields
    {
        None = 0,
        Sign = 1 << 0,
        Hours12 = 1 << 1,
        Hours24 = 1 << 2,
        Minutes = 1 << 3,
        Seconds = 1 << 4,
        FractionalSeconds = 1 << 5,
        AmPm = 1 << 6,
        Year = 1 << 7,
        YearTwoDigits = 1 << 8,
        YearOfEra = 1 << 9,
        MonthOfYearNumeric = 1 << 10,
        MonthOfYearText = 1 << 11,
        DayOfMonth = 1 << 12,
        DayOfWeek = 1 << 13,
        Era = 1 << 14,
        Calendar = 1 << 15,
        Zone = 1 << 16,
        EmbeddedOffset = 1 << 17,

        AllTimeFields = Hours12 | Hours24 | Minutes | Seconds | FractionalSeconds | AmPm,
        AllDateFields = Year | YearTwoDigits | YearOfEra | MonthOfYearNumeric | MonthOfYearText | DayOfMonth | DayOfWeek | Era | Calendar
    }
}
