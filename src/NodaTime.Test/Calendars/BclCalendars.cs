// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace NodaTime.Test.Calendars
{
    /// <summary>
    /// In netstandard, we can't access specific subclasses of System.Globalization.Calendar.
    /// We can fetch them from cultures though... and maybe use reflection.
    /// </summary>
    public class BclCalendars
    {
        /// <summary>
        /// Returns a sequence of all the BCL calendar systems for which we have a
        /// mapping in Noda Time. The first entry is Gregorian so that it's easy to
        /// comment out the rest for initial testing of a new feature (where the
        /// Gregorian calendar is typically easy to reason about).
        /// </summary>
        public static IEnumerable<Calendar> MappedCalendars =>
            new[] { Gregorian, Hebrew, UmAlQura, Persian, Julian, Hijri };

        public static Calendar Hebrew { get; } = new HebrewCalendar();
        public static Calendar Gregorian { get; } = new GregorianCalendar();
        public static Calendar UmAlQura { get; } = new UmAlQuraCalendar();
        public static Calendar Persian { get; } = new PersianCalendar();
        public static Calendar Julian { get; } = new JulianCalendar();
        public static Calendar Hijri { get; } = new HijriCalendar();

        /// <summary>
        /// Tries to work out a roughly-matching calendar system for the given BCL calendar.
        /// This is needed where we're testing whether days of the week match - even if we can
        /// get day/month/year values to match without getting the calendar right, the calendar
        /// affects the day of week.
        /// </summary>
        internal static CalendarSystem? CalendarSystemForCalendar(Calendar bcl) =>
            bcl switch
            {
                null => throw new ArgumentNullException(nameof(bcl)),
                GregorianCalendar _ => CalendarSystem.Iso,
                HijriCalendar _ => CalendarSystem.IslamicBcl,
                HebrewCalendar _ => CalendarSystem.HebrewCivil,
                PersianCalendar _ => bcl.IsLeapYear(1) ? CalendarSystem.PersianSimple : CalendarSystem.PersianAstronomical,
                UmAlQuraCalendar _ => CalendarSystem.UmAlQura,
                JulianCalendar _ => CalendarSystem.Julian,
                _ => null // No idea - we can't test with this calendar...
            };
    }
}
