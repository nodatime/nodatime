// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
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
        public void EraProperty()
        {
            CalendarSystem calendar = CalendarSystem.Gregorian;
            LocalDateTime startOfEra = new LocalDateTime(1, 1, 1, 0, 0, 0, calendar);
            Assert.AreEqual(Era.Common, startOfEra.Era);
            Assert.AreEqual(Era.BeforeCommon, startOfEra.PlusTicks(-1).Era);
        }

        [Test]
        public void AddMonths_BoundaryCondition()
        {
            var start = new LocalDate(2017, 8, 20);
            var end = start.PlusMonths(-19);
            var expected = new LocalDate(2016, 1, 20);
            Assert.AreEqual(expected, end);
        }
    }
}
