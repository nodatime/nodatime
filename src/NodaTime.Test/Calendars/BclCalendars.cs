// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Test.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace NodaTime.Test.Calendars
{
    /// <summary>
    /// In netstandard, we can't access specific subclasses of System.Globalization.Calendar.
    /// We can fetch them from cultures though... and maybe use reflection.
    /// </summary>
    public class BclCalendars
    {
        private static readonly Dictionary<string, Calendar> calendars
            = Cultures.AllCultures
                .SelectMany(culture => new[] { culture.Calendar }.Concat(culture.OptionalCalendars))
                .GroupBy(calendar => calendar.GetType())
                .ToDictionary(g => g.Key.GetTypeInfo().Name, g => g.First());

        private static Calendar GetCalendar(string name)
        {
            if (calendars.TryGetValue(name, out Calendar calendar))
            {
                return calendar;
            }
            // Try to initialize by reflection instead...
            Type type = typeof(Calendar).GetTypeInfo().Assembly.GetType($"System.Globalization.{name}");
            if (type == null)
            {
                // We can start being defensive if/when we try to test on a platform where
                // this becomes a problem.
                throw new Exception($"Unable to get calendar {name}");
            }
            return (Calendar) Activator.CreateInstance(type);
        }

        public static Calendar Hebrew => GetCalendar("HebrewCalendar");
        public static Calendar Gregorian => GetCalendar("GregorianCalendar");
        public static Calendar UmAlQura => GetCalendar("UmAlQuraCalendar");
        public static Calendar Persian => GetCalendar("PersianCalendar");
        public static Calendar Julian => GetCalendar("JulianCalendar");
        public static Calendar Hijri => GetCalendar("HijriCalendar");

        /// <summary>
        /// Returns a sequence of all the BCL calendar systems for which we have a
        /// mapping in Noda Time. The first entry is Gregorian so that it's easy to
        /// comment out the rest for initial testing of a new feature (where the
        /// Gregorian calendar is typically easy to reason about).
        /// </summary>
        public static IEnumerable<Calendar> MappedCalendars =>
            new[] { Gregorian, Hebrew, UmAlQura, Persian, Julian, Hijri };

        /// <summary>
        /// Tries to work out a roughly-matching calendar system for the given BCL calendar.
        /// This is needed where we're testing whether days of the week match - even if we can
        /// get day/month/year values to match without getting the calendar right, the calendar
        /// affects the day of week.
        /// </summary>
        internal static CalendarSystem CalendarSystemForCalendar(Calendar bcl)
        {
            // Yes, this is horrible... but the specific calendars aren't available to test
            // against in netstandard
            switch (bcl.GetType().Name)
            {
                case "GregorianCalendar": return CalendarSystem.Iso;
                case "HijriCalendar": return CalendarSystem.IslamicBcl;
                case "HebrewCalendar": return CalendarSystem.HebrewCivil;
                case "PersianCalendar": return bcl.IsLeapYear(1) ? CalendarSystem.PersianSimple : CalendarSystem.PersianAstronomical;
                case "UmAlQuraCalendar": return CalendarSystem.UmAlQura;
                case "JulianCalendar":
                    return CalendarSystem.Julian;
                default:
                    // No idea - we can't test with this calendar...
                    return null;
            }
        }
    }
}
