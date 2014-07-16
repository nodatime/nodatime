// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System.Globalization;
using NodaTime.Calendars;

namespace NodaTime
{
    /// <summary>
    /// Simple properties to obtain commonly-used calendars. These are entirely equivalent to calling
    /// the factory methods on <see cref="CalendarSystem"/>, but more concise.
    /// </summary>
    public static class CommonCalendars
    {
        /// <summary>
        /// Returns the ISO calendar system.
        /// </summary>
        /// <seealso cref="CalendarSystem.Iso"/>
        /// <returns>The ISO calendar system.</returns>
        public static CalendarSystem Iso { get { return CalendarSystem.Iso; } }

        /// <summary>
        /// Returns a Gregorian calendar system with at least 4 days in the first week of a week-year.
        /// </summary>
        /// <seealso cref="CalendarSystem.GetGregorianCalendar"/>
        /// <returns>A Gregorian calendar system with at least 4 days in the first week of a week-year.</returns>
        public static CalendarSystem Gregorian { get { return CalendarSystem.GetGregorianCalendar(4); } }

        /// <summary>
        /// Returns a Julian calendar system with at least 4 days in the first week of a week-year.
        /// </summary>
        /// <seealso cref="CalendarSystem.GetJulianCalendar"/>
        /// <returns>A Julian calendar system with at least 4 days in the first week of a week-year.</returns>
        public static CalendarSystem Julian { get { return CalendarSystem.GetJulianCalendar(4); } }

        /// <summary>
        /// Returns a Coptic calendar system with at least 4 days in the first week of a week-year.
        /// </summary>
        /// <seealso cref="CalendarSystem.GetCopticCalendar"/>
        /// <returns>A Coptic calendar system with at least 4 days in the first week of a week-year.</returns>
        public static CalendarSystem Coptic { get { return CalendarSystem.GetCopticCalendar(4); } }

        /// <summary>
        /// Returns an Islamic calendar system equivalent to the one used by the BCL <see cref="HijriCalendar"/>.
        /// </summary>
        /// <remarks>
        /// This uses the <see cref="IslamicLeapYearPattern.Base16"/> leap year pattern and the
        /// <see cref="IslamicEpoch.Astronomical"/> epoch.
        /// </remarks>
        /// <seealso cref="CalendarSystem.GetIslamicCalendar"/>
        /// <returns>An Islamic calendar system equivalent to the one used by the BCL.</returns>
        public static CalendarSystem BclIslamic
        {
            get
            {
                return CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Astronomical);
            }
        }

        /// <summary>
        /// Returns a Persian calendar system.
        /// </summary>
        /// <seealso cref="CalendarSystem.GetPersianCalendar"/>
        /// <returns>A Persian calendar system.</returns>
        public static CalendarSystem Persian { get { return CalendarSystem.GetPersianCalendar(); } }

        /// <summary>
        /// Returns a Hebrew calendar system using the civil month numbering.
        /// </summary>
        /// <remarks>This calendar system is compatible with the BCL <see cref="HebrewCalendar"/>.</remarks>
        /// <seealso cref="CalendarSystem.GetHebrewCalendar"/>
        /// <returns>A Hebrew calendar system using the civil month numbering.</returns>
        public static CalendarSystem CivilHebrew { get { return CalendarSystem.GetHebrewCalendar(HebrewMonthNumbering.Civil); } }

        /// <summary>
        /// Returns a Hebrew calendar system using the scriptural month numbering.
        /// </summary>
        /// <seealso cref="CalendarSystem.GetHebrewCalendar"/>
        /// <returns>A Hebrew calendar system using the civil month numbering.</returns>
        public static CalendarSystem ScripturalHebrew { get { return CalendarSystem.GetHebrewCalendar(HebrewMonthNumbering.Scriptural); } }
    }
}
