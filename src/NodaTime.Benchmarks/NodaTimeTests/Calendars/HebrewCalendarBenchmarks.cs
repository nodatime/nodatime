// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using NodaTime.Calendars;

#if !V1_0 && !V1_1 && !V1_2
namespace NodaTime.Benchmarks.NodaTimeTests.Calendars
{
    public class HebrewCalendarBenchmarks
    {
        // Note: avoiding properties for backward compatibility
        private static readonly CalendarSystem ScripturalCalendar = CalendarSystem.GetHebrewCalendar(HebrewMonthNumbering.Scriptural);
        private static readonly CalendarSystem CivilCalendar = CalendarSystem.GetHebrewCalendar(HebrewMonthNumbering.Civil);

        [Benchmark]
        public LocalDate ScripturalConversion() => TestLeapCycle(ScripturalCalendar);

        [Benchmark]
        public LocalDate CivilConversion() => TestLeapCycle(CivilCalendar);

        /// <summary>
        /// Converts each day in a full leap cycle (for coverage of different scenarios) to the ISO
        /// calendar and back. This exercises fetching the number of days since the epoch and getting
        /// a year/month/day *from* a number of days.
        /// </summary>
        private static LocalDate TestLeapCycle(CalendarSystem calendar)
        {
            LocalDate returnLocalDate = new LocalDate();
            for (int year = 5400; year < 5419; year++)
            {
#if !V1
                int maxMonth = calendar.GetMonthsInYear(year);
#else
                int maxMonth = calendar.GetMaxMonth(year);
#endif
                for (int month = 1; month <= maxMonth; month++)
                {
                    int maxDay = calendar.GetDaysInMonth(year, month);
                    for (int day = 1; day <= maxDay; day++)
                    {
                        var date = new LocalDate(year, month, day, calendar);
                        returnLocalDate = date.WithCalendar(CalendarSystem.Iso).WithCalendar(calendar);
                    }
                }
            }
            return returnLocalDate;
        }
    }
}
#endif
