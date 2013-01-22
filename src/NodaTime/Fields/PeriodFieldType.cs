// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Fields
{
    /// <summary>
    /// Indicates the type of a value represented by a period field.
    /// </summary>
    /// <remarks>
    /// If another type is added after Ticks, you must edit <see cref="UnsupportedPeriodField"/> appropriately.
    /// </remarks>
    internal enum PeriodFieldType
    {
        /// <summary>
        /// PeriodFieldType for eras.
        /// </summary>
        Eras,
        /// <summary>
        /// PeriodFieldType for centuries.
        /// </summary>
        Centuries,
        /// <summary>
        /// PeriodFieldType for week-years.
        /// </summary>
        WeekYears,
        /// <summary>
        /// PeriodFieldType for years.
        /// </summary>
        Years,
        /// <summary>
        /// PeriodFieldType for months.
        /// </summary>
        Months,
        /// <summary>
        /// PeriodFieldType for weeks.
        /// </summary>
        Weeks,
        /// <summary>
        /// PeriodFieldType for days.
        /// </summary>
        Days,
        /// <summary>
        /// PeriodFieldType for half days.
        /// </summary>
        HalfDays,
        /// <summary>
        /// PeriodFieldType for hours.
        /// </summary>
        Hours,
        /// <summary>
        /// PeriodFieldType for minutes.
        /// </summary>
        Minutes,
        /// <summary>
        /// PeriodFieldType for seconds.
        /// </summary>
        Seconds,
        /// <summary>
        /// PeriodFieldType for milliseconds.
        /// </summary>
        Milliseconds,
        /// <summary>
        /// PeriodFieldType for ticks.
        /// </summary>
        Ticks,
    }
}