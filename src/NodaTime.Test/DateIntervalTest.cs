// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Security.Cryptography;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class DateIntervalTest
    {
        private static readonly CalendarSystem JulianCalendar = CalendarSystem.GetJulianCalendar(4);

        [Test]
        public void Construction_DifferentCalendars()
        {
            LocalDate start = new LocalDate(1600, 1, 1);
            LocalDate end = new LocalDate(1800, 1, 1, JulianCalendar);
            Assert.Throws<ArgumentException>(() => new DateInterval(start, end));
            Assert.Throws<ArgumentException>(() => new DateInterval(start, end, true));
        }

        [Test]
        public void Construction_EndBeforeStart()
        {
            LocalDate start = new LocalDate(1600, 1, 1);
            LocalDate end = new LocalDate(1500, 1, 1);
            Assert.Throws<ArgumentException>(() => new DateInterval(start, end));
            Assert.Throws<ArgumentException>(() => new DateInterval(start, end, true));
        }

        [Test]
        public void Construction_EqualStartAndEnd()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            Assert.DoesNotThrow(() => new DateInterval(start, start));
            Assert.DoesNotThrow(() => new DateInterval(start, start, true));
        }

        [Test]
        public void Construction_DefaultToInclusive()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval = new DateInterval(start, end);
            Assert.IsTrue(interval.Inclusive);
        }

        [Test]
        public void Construction_Properties()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval = new DateInterval(start, end, false);
            Assert.AreEqual(start, interval.Start);
            Assert.AreEqual(end, interval.End);
            Assert.IsFalse(interval.Inclusive);
        }

        [Test]
        public void Length_Inclusive()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2000, 2, 10);
            var interval = new DateInterval(start, end, true);
            Assert.AreEqual(41, interval.Length);
        }

        [Test]
        public void Length_Exclusive()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2000, 2, 10);
            var interval = new DateInterval(start, end, false);
            Assert.AreEqual(40, interval.Length);
        }

        [Test]
        [TestCase("1999-12-31", false, false, TestName = "Before start")]
        [TestCase("2000-01-01", true, true, TestName = "On start")]
        [TestCase("2005-06-06", true, true, TestName = "In middle")]
        [TestCase("2014-06-30", true, false, TestName = "On end")]
        [TestCase("2014-07-01", false, false, TestName = "After end")]
        public void Contains(string candidateText, bool expectedInclusive, bool expectedExclusive)
        {
            var start = new LocalDate(2000, 1, 1);
            var end = new LocalDate(2014, 06, 30);
            var candidate = LocalDatePattern.IsoPattern.Parse(candidateText).Value;
            var interval = new DateInterval(start, end, true);
            Assert.AreEqual(expectedInclusive, interval.Contains(candidate));
            interval = new DateInterval(start, end, false);
            Assert.AreEqual(expectedExclusive, interval.Contains(candidate));
        }

        [Test]
        public void Contains_DifferentCalendar()
        {
            var start = new LocalDate(2000, 1, 1);
            var end = new LocalDate(2014, 06, 30);
            var interval = new DateInterval(start, end);
            var candidate = new LocalDate(2000, 1, 1, JulianCalendar);
            Assert.Throws<ArgumentException>(() => interval.Contains(candidate));
        }
    }
}
