// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Test.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NodaTime.Test.Calendars
{
    /// <summary>
    /// In the PCL, we can't access specific subclasses of System.Globalization.Calendar.
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
            Calendar calendar;
            if (calendars.TryGetValue(name, out calendar))
            {
                return calendar;
            }
            // Try to initialize by reflection instead...
            Type type = typeof(Calendar).GetTypeInfo().Assembly.GetType($"System.Globalization.{name}");
            return type == null ? null : (Calendar) Activator.CreateInstance(type);
        }

        public static Calendar Hebrew => GetCalendar("HebrewCalendar");
        public static Calendar Gregorian => GetCalendar("GregorianCalendar");
        public static Calendar UmAlQura => GetCalendar("UmAlQuraCalendar");
        public static Calendar Persian => GetCalendar("PersianCalendar");
        public static Calendar Julian => GetCalendar("JulianCalendar");
        public static Calendar Hijri => GetCalendar("HijriCalendar");
    }
}
