// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using NodaTime.Calendars;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalDateTest
    {
        /// <summary>
        /// Using the default constructor is equivalent to January 1st 1970, UTC, ISO calendar
        /// </summary>
        [Test]
        public void DefaultConstructor()
        {
            var actual = new LocalDate();
            Assert.AreEqual(new LocalDate(1, 1, 1), actual);
        }

        [Test]
        public void CombinationWithTime()
        {
            // Test all three approaches in the same test - they're logically equivalent.
            var calendar = CalendarSystem.Julian;
            LocalDate date = new LocalDate(2014, 3, 28, calendar);
            LocalTime time = new LocalTime(20, 17, 30);
            LocalDateTime expected = new LocalDateTime(2014, 3, 28, 20, 17, 30, calendar);
            Assert.AreEqual(expected, date + time);
            Assert.AreEqual(expected, date.At(time));
            Assert.AreEqual(expected, time.On(date));
        }

        [Test]
        public void XmlSerialization_Iso()
        {
            var value = new LocalDate(2013, 4, 12);
            TestHelper.AssertXmlRoundtrip(value, "<value>2013-04-12</value>");
        }

        [Test]
        public void XmlSerialization_NonIso()
        {
            var value = new LocalDate(2013, 4, 12, CalendarSystem.Julian);
            TestHelper.AssertXmlRoundtrip(value, "<value calendar=\"Julian\">2013-04-12</value>");
        }

        [Test]
        [TestCase("<value calendar=\"Rubbish\">2013-06-12</value>", typeof(KeyNotFoundException), Description = "Unknown calendar system")]
        [TestCase("<value>2013-15-12</value>", typeof(UnparsableValueException), Description = "Invalid month")]
        public void XmlSerialization_Invalid(string xml, Type expectedExceptionType)
        {
            TestHelper.AssertXmlInvalid<LocalDate>(xml, expectedExceptionType);
        }

        [Test]
        public void Construction_NullCalendar_Throws()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentNullException>(() => new LocalDate(2017, 11, 7, calendar: null!), "Basic construction.");
                Assert.Throws<ArgumentNullException>(() => new LocalDate(Era.Common, 2017, 11, 7, calendar: null!), "Construction including era.");
            });
        }

        [Test]
        public void MaxIsoValue()
        {
            var value = LocalDate.MaxIsoValue;
            Assert.AreEqual(CalendarSystem.Iso, value.Calendar);
            Assert.Throws<OverflowException>(() => value.PlusDays(1));
        }

        [Test]
        public void MinIsoValue()
        {
            var value = LocalDate.MinIsoValue;
            Assert.AreEqual(CalendarSystem.Iso, value.Calendar);
            Assert.Throws<OverflowException>(() => value.PlusDays(-1));
        }

        [Test]
        public void Deconstruction()
        {
            var value = new LocalDate(2017, 11, 7);
            var expectedYear = 2017;
            var expectedMonth = 11;
            var expectedDay = 7;
            var (actualYear, actualMonth, actualDay) = value;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedYear, actualYear);
                Assert.AreEqual(expectedMonth, actualMonth);
                Assert.AreEqual(expectedDay, actualDay);
            });
        }

        [Test]
        public void Deconstruction_IncludingCalendar()
        {
            var value = new LocalDate(2017, 11, 7, CalendarSystem.Gregorian);
            var expectedYear = 2017;
            var expectedMonth = 11;
            var expectedDay = 7;
            var expectedCalendar = CalendarSystem.Gregorian;
            var (actualYear, actualMonth, actualDay, actualCalendar) = value;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedYear, actualYear);
                Assert.AreEqual(expectedMonth, actualMonth);
                Assert.AreEqual(expectedDay, actualDay);
                Assert.AreEqual(expectedCalendar, actualCalendar);
            });
        }
    }
}
