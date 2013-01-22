// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Calendars
{
    /// <summary>
    /// The pattern of leap years to use when constructing an Islamic calendar.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Islamic, or Hijri, calendar is a lunar calendar of 12 months, each of 29 or 30 days.
    /// The calendar can be defined in either observational or tabular terms; 
    /// Noda Time implements a tabular calendar, where a pattern of leap years (in which the last month has
    /// an extra day) repeats every 30 years, according to one of the patterns within this enum.
    /// </para>
    /// <para>
    /// While the patterns themselves are reasonably commonly documented (see e.g.
    /// <a href="http://en.wikipedia.org/wiki/Tabular_Islamic_calendar">Wikipedia</a>)
    /// there is little standardization in terms of naming the patterns. I hope the current names do not
    /// cause offence to anyone; suggestions for better names would be welcome.
    /// </para>
    /// <seealso cref="CalendarSystem.GetIslamicCalendar"/>
    /// </remarks>
    public enum IslamicLeapYearPattern
    {
        /// <summary>
        /// A pattern of leap years in 2, 5, 7, 10, 13, 15, 18, 21, 24, 26 and 29.
        /// This pattern and <see cref="Base16"/> are the most commonly used ones,
        /// and only differ in whether the 15th or 16th year is deemed leap.
        /// </summary>
        Base15 = 1,
        /// <summary>
        /// A pattern of leap years in 2, 5, 7, 10, 13, 16, 18, 21, 24, 26 and 29.
        /// This pattern and <see cref="Base15"/> are the most commonly used ones,
        /// and only differ in whether the 15th or 16th year is deemed leap.
        /// </summary>
        Base16 = 2,
        /// <summary>
        /// A pattern of leap years in 2, 5, 8, 10, 13, 16, 19, 21, 24, 27 and 29.
        /// </summary>
        Indian = 3,
        /// <summary>
        /// A pattern of leap years in 2, 5, 8, 11, 13, 16, 19, 21, 24, 27 and 30.
        /// </summary>
        HabashAlHasib = 4,
    }
}
