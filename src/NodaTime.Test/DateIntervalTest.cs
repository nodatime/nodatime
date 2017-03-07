// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NodaTime.Text;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.Test
{
    public class DateIntervalTest
    {
        private static readonly CalendarSystem JulianCalendar = CalendarSystem.Julian;
        private static readonly List<CalendarSystem> SupportedCalendars = CalendarSystem.Ids.ToList().Select(CalendarSystem.ForId).ToList();
        private static readonly LocalDate MinIsoDate = new LocalDate(-9998, 1, 1);
        private static readonly LocalDate MaxIsoDate = new LocalDate(9999, 12, 31);

        [Test]
        public void Construction_DifferentCalendars()
        {
            LocalDate start = new LocalDate(1600, 1, 1);
            LocalDate end = new LocalDate(1800, 1, 1, JulianCalendar);
            Assert.Throws<ArgumentException>(() => new DateInterval(start, end));
        }

        [Test]
        public void Construction_EndBeforeStart()
        {
            LocalDate start = new LocalDate(1600, 1, 1);
            LocalDate end = new LocalDate(1500, 1, 1);
            Assert.Throws<ArgumentException>(() => new DateInterval(start, end));
        }

        [Test]
        public void Construction_EqualStartAndEnd()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            Assert.DoesNotThrow(() => new DateInterval(start, start));
        }

        [Test]
        public void Construction_Properties()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval = new DateInterval(start, end);
            Assert.AreEqual(start, interval.Start);
            Assert.AreEqual(end, interval.End);
        }

        [Test]
        public void Equals_SameInstance()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval = new DateInterval(start, end);

            Assert.AreEqual(interval, interval);
            Assert.AreEqual(interval.GetHashCode(), interval.GetHashCode());
            // CS1718: Comparison made to same variable.  This is intentional to test operator ==.
#pragma warning disable 1718
            Assert.IsTrue(interval == interval);
            Assert.IsFalse(interval != interval);
#pragma warning restore 1718
            Assert.IsTrue(interval.Equals(interval)); // IEquatable implementation
        }

        [Test]
        public void Equals_EqualValues()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval1 = new DateInterval(start, end);
            var interval2 = new DateInterval(start, end);

            Assert.AreEqual(interval1, interval2);
            Assert.AreEqual(interval1.GetHashCode(), interval2.GetHashCode());
            Assert.IsTrue(interval1 == interval2);
            Assert.IsFalse(interval1 != interval2);
            Assert.IsTrue(interval1.Equals(interval2)); // IEquatable implementation
        }

        [Test]
        public void Equals_DifferentCalendars()
        {
            LocalDate start1 = new LocalDate(2000, 1, 1);
            LocalDate end1 = new LocalDate(2001, 6, 19);
            // This is a really, really similar calendar to ISO, but we do distinguish.
            LocalDate start2 = start1.WithCalendar(CalendarSystem.Gregorian);
            LocalDate end2 = end1.WithCalendar(CalendarSystem.Gregorian);
            var interval1 = new DateInterval(start1, end1);
            var interval2 = new DateInterval(start2, end2);

            Assert.AreNotEqual(interval1, interval2);
            Assert.AreNotEqual(interval1.GetHashCode(), interval2.GetHashCode());
            Assert.IsFalse(interval1 == interval2);
            Assert.IsTrue(interval1 != interval2);
            Assert.IsFalse(interval1.Equals(interval2)); // IEquatable implementation
        }

        [Test]
        public void Equals_DifferentStart()
        {
            LocalDate start1 = new LocalDate(2000, 1, 1);
            LocalDate start2 = new LocalDate(2000, 1, 2);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval1 = new DateInterval(start1, end);
            var interval2 = new DateInterval(start2, end);

            Assert.AreNotEqual(interval1, interval2);
            Assert.AreNotEqual(interval1.GetHashCode(), interval2.GetHashCode());
            Assert.IsFalse(interval1 == interval2);
            Assert.IsTrue(interval1 != interval2);
            Assert.IsFalse(interval1.Equals(interval2)); // IEquatable implementation
        }

        [Test]
        public void Equals_DifferentEnd()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end1 = new LocalDate(2001, 6, 19);
            LocalDate end2 = new LocalDate(2001, 6, 20);
            var interval1 = new DateInterval(start, end1);
            var interval2 = new DateInterval(start, end2);

            Assert.AreNotEqual(interval1, interval2);
            Assert.AreNotEqual(interval1.GetHashCode(), interval2.GetHashCode());
            Assert.IsFalse(interval1 == interval2);
            Assert.IsTrue(interval1 != interval2);
            Assert.IsFalse(interval1.Equals(interval2)); // IEquatable implementation
        }

        [Test]
        public void Equals_DifferentToNull()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval = new DateInterval(start, end);

            Assert.IsFalse(interval.Equals(null));
        }

        [Test]
        public void Equals_DifferentToOtherType()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval = new DateInterval(start, end);
            Assert.IsFalse(interval.Equals(Instant.FromUnixTimeTicks(0)));
        }

        [Test]
        public void StringRepresentation()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval = new DateInterval(start, end);
            Assert.AreEqual("[2000-01-01, 2001-06-19]", interval.ToString());
        }

        [Test]
        public void Length()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2000, 2, 10);
            var interval = new DateInterval(start, end);
            Assert.AreEqual(41, interval.Length);
        }

        [Test]
        [TestCase("1999-12-31", false, TestName = "Before start")]
        [TestCase("2000-01-01", true, TestName = "On start")]
        [TestCase("2005-06-06", true, TestName = "In middle")]
        [TestCase("2014-06-30", true, TestName = "On end")]
        [TestCase("2014-07-01", false, TestName = "After end")]
        public void Contains(string candidateText, bool expected)
        {
            var start = new LocalDate(2000, 1, 1);
            var end = new LocalDate(2014, 06, 30);
            var candidate = LocalDatePattern.Iso.Parse(candidateText).Value;
            var interval = new DateInterval(start, end);
            Assert.AreEqual(expected, interval.Contains(candidate));
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
