// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime
{
#if !NETSTANDARD1_3
    /// <summary>
    /// Names used by binary formatting, to make it easier to avoid any collisions.
    /// </summary>
    internal static class BinaryFormattingConstants
    {
        // LocalDate
        // Used as part of LocalDateTime, OffsetDateTime, ZonedDateTime
        internal const string YearSerializationName = "year";
        internal const string MonthSerializationName = "month";
        internal const string DaySerializationName = "day";
        internal const string CalendarSerializationName = "calendar";

        // LocalTime
        // Used as part of LocalDateTime, OffsetDateTime, ZonedDateTime
        internal const string NanoOfDaySerializationName = "nanoOfDay";

        // Duration
        internal const string DurationDefaultDaysSerializationName = "days";
        internal const string DurationDefaultNanosecondOfDaySerializationName = "nanoOfDay";

        // Offset
        // Used as part of OffsetDateTime, ZonedDateTime
        internal const string OffsetSecondsSerializationName = "offsetSeconds";

        // Interval
        internal const string StartDaysSerializationName = "startDays";
        internal const string EndDaysSerializationName = "endDays";
        internal const string StartNanosecondOfDaySerializationName = "startNanoOfDay";
        internal const string EndNanosecondOfDaySerializationName = "endNanoOfDay";
        internal const string PresenceName = "presence";

        // Period
        internal const string YearsSerializationName = "years";
        internal const string MonthsSerializationName = "months";
        internal const string WeeksSerializationName = "weeks";
        internal const string DaysSerializationName = "days";
        internal const string HoursSerializationName = "hours";
        internal const string MinutesSerializationName = "minutes";
        internal const string SecondsSerializationName = "seconds";
        internal const string MillisecondsSerializationName = "milliseconds";
        internal const string TicksSerializationName = "ticks";
        internal const string NanosecondsSerializationName = "nanosDays";

        // ZonedDateTime
        internal const string ZoneIdSerializationName = "zone";
    }
#endif
}
