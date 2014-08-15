// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Linq;
using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    [TestFixture]
    public class GregorianCalendarSystemTest
    {
        [Test]
        public void LeapYears()
        {
            var calendar = CalendarSystem.Gregorian;
            Assert.IsFalse(calendar.IsLeapYear(1900));
            Assert.IsFalse(calendar.IsLeapYear(1901));
            Assert.IsTrue(calendar.IsLeapYear(1904));
            Assert.IsTrue(calendar.IsLeapYear(1996));
            Assert.IsTrue(calendar.IsLeapYear(2000));
            Assert.IsFalse(calendar.IsLeapYear(2100));
            Assert.IsTrue(calendar.IsLeapYear(2400));
        }

        [Test]
        public void GetInstance_UniqueIds()
        {
            Assert.AreEqual(7, Enumerable.Range(1, 7).Select(x => CalendarSystem.GetGregorianCalendar(x).Id).Distinct().Count());
        }

        [Test]
        public void GetInstance_InvalidMinDaysInFirstWeek()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CalendarSystem.GetGregorianCalendar(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => CalendarSystem.GetGregorianCalendar(8));
        }

        [Test]
        public void GetInstance_MinDaysInFirstWeekIsRespected()
        {
            // Seems the simplest way to test this... yes, it seems somewhat wasteful, but hey...
            for (int i = 1; i < 7; i++)
            {
                CalendarSystem calendar = CalendarSystem.GetGregorianCalendar(i);

                int actualMin = Enumerable.Range(1900, 400)
                                          .Select(year => GetDaysInFirstWeek(year, calendar))
                                          .Min();
                Assert.AreEqual(i, actualMin);
            }
        }

        [Test]
        public void EraProperty()
        {
            CalendarSystem calendar = CalendarSystem.Gregorian;
            LocalDateTime startOfEra = new LocalDateTime(1, 1, 1, 0, 0, 0, calendar);
            Assert.AreEqual(Era.Common, startOfEra.Era);
            Assert.AreEqual(Era.BeforeCommon, startOfEra.PlusTicks(-1).Era);
        }

        private int GetDaysInFirstWeek(int year, CalendarSystem calendar)
        {
            // Some of the first few days of the week year may be in the previous week year.
            // However, the whole of the first week of the week year definitely occurs
            // within the first 13 days of January.
            return Enumerable.Range(1, 13)
                             .Count(day => new LocalDate(year, 1, day, calendar).WeekOfWeekYear == 1);
        }
    }
}
