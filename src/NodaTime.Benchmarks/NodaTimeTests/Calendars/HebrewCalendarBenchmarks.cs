// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Benchmarks.Framework;
using NodaTime.Calendars;

namespace NodaTime.Benchmarks.NodaTimeTests.Calendars
{
    internal class HebrewCalendarBenchmarks
    {
        private static readonly CalendarSystem ScripturalCalendar = CalendarSystem.GetHebrewCalendar(HebrewMonthNumbering.Scriptural);
        private static readonly CalendarSystem CivilCalendar = CalendarSystem.GetHebrewCalendar(HebrewMonthNumbering.Civil);

        [Benchmark]
        public void ScripturalConversion()
        {
            TestLeapCycle(ScripturalCalendar);
        }

        [Benchmark]
        public void CivilConversion()
        {
            TestLeapCycle(CivilCalendar);
        }

        /// <summary>
        /// Converts each day in a full leap cycle (for coverage of different scenarios) to the ISO
        /// calendar and back. This exercises fetching the number of days since the epoch and getting
        /// a year/month/day *from* a number of days.
        /// </summary>
        private static void TestLeapCycle(CalendarSystem calendar)
        {
            
            for (int year = 5400; year < 5419; year++)
            {
                int maxMonth = calendar.GetMonthsInYear(year);
                for (int month = 1; month <= maxMonth; month++)
                {
                    int maxDay = calendar.GetDaysInMonth(year, month);
                    for (int day = 1; day <= maxDay; day++)
                    {
                        var date = new LocalDate(year, month, day, calendar);
                        date.WithCalendar(CalendarSystem.Iso).WithCalendar(calendar).Consume();
                    }
                }
            }
        }
    }
}
