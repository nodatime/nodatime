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
        YearTwoDigits = 1 << 8, // Actually year of *era* as two ditits...
        YearOfEra = 1 << 9,
        MonthOfYearNumeric = 1 << 10,
        MonthOfYearText = 1 << 11,
        DayOfMonth = 1 << 12,
        DayOfWeek = 1 << 13,
        Era = 1 << 14,
        Calendar = 1 << 15,
        Zone = 1 << 16,
        ZoneAbbreviation = 1 << 17,
        EmbeddedOffset = 1 << 18,
        TotalDuration = 1 << 19, // D, H, M, or S in a DurationPattern.
        EmbeddedDate = 1 << 20, // No other date fields permitted; use calendar/year/month/day from bucket
        EmbeddedTime = 1 << 21, // No other time fields permitted; user hours24/minutes/seconds/fractional seconds from bucket

        AllTimeFields = Hours12 | Hours24 | Minutes | Seconds | FractionalSeconds | AmPm | EmbeddedTime,
        AllDateFields = Year | YearTwoDigits | YearOfEra | MonthOfYearNumeric | MonthOfYearText | DayOfMonth | DayOfWeek | Era | Calendar | EmbeddedDate
    }

    /// <summary>
    /// Extension methods on PatternFields; nothing PatternFields-specific here, but we
    /// can't write this generically due to limitations in C#. (See Unconstrained Melody for details...)
    /// </summary>
    internal static class PatternFieldsExtensions
    {
        /// <summary>
        /// Returns true if the given set of fields contains any of the target fields.
        /// </summary>
        internal static bool HasAny(this PatternFields fields, PatternFields target) => (fields & target) != 0;

        /// <summary>
        /// Returns true if the given set of fields contains all of the target fields.
        /// </summary>
        internal static bool HasAll(this PatternFields fields, PatternFields target) => (fields & target) == target;
    }
}
