// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NodaTime.Text;
using NUnit.Framework;
using System;

namespace NodaTime.Test
{
    public class DateIntervalTest
    {
        private static readonly CalendarSystem JulianCalendar = CalendarSystem.Julian;

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
        public void Equals_EqualValues()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval1 = new DateInterval(start, end, false);
            var interval2 = new DateInterval(start, end, false);
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
            // TODO: Should we?
            LocalDate start2 = start1.WithCalendar(CalendarSystem.Gregorian);
            LocalDate end2 = end1.WithCalendar(CalendarSystem.Gregorian);
            var interval1 = new DateInterval(start1, end1, false);
            var interval2 = new DateInterval(start2, end2, false);
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
            var interval1 = new DateInterval(start1, end, false);
            var interval2 = new DateInterval(start2, end, false);
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
            var interval1 = new DateInterval(start, end1, false);
            var interval2 = new DateInterval(start, end2, false);
            Assert.AreNotEqual(interval1, interval2);
            Assert.AreNotEqual(interval1.GetHashCode(), interval2.GetHashCode());
            Assert.IsFalse(interval1 == interval2);
            Assert.IsTrue(interval1 != interval2);
            Assert.IsFalse(interval1.Equals(interval2)); // IEquatable implementation
        }

        [Test]
        public void Equals_DifferentInclusivity()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval1 = new DateInterval(start, end, false);
            var interval2 = new DateInterval(start, end, true);
            Assert.AreNotEqual(interval1, interval2);
            Assert.AreNotEqual(interval1.GetHashCode(), interval2.GetHashCode());
            Assert.IsFalse(interval1 == interval2);
            Assert.IsTrue(interval1 != interval2);
            Assert.IsFalse(interval1.Equals(interval2)); // IEquatable implementation
        }

        [Test]
        public void Equals_SameRangeButNotEquivalent()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end1 = new LocalDate(2001, 6, 19);
            LocalDate end2 = new LocalDate(2001, 6, 20);
            var interval1 = new DateInterval(start, end1, true);
            var interval2 = new DateInterval(start, end2, false);
            Assert.AreEqual(interval1.Length, interval2.Length);

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
            var interval = new DateInterval(start, end, false);
            Assert.IsFalse(interval.Equals(null));
        }

        [Test]
        public void Equals_DifferentToOtherType()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval = new DateInterval(start, end, false);
            Assert.IsFalse(interval.Equals(Instant.FromUnixTimeTicks(0)));
        }

        [Test]
        public void ToString_Inclusive()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval = new DateInterval(start, end, true);
            Assert.AreEqual("[2000-01-01, 2001-06-19]", interval.ToString());
        }

        [Test]
        public void ToString_Exclusive()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval = new DateInterval(start, end, false);
            Assert.AreEqual("[2000-01-01, 2001-06-19)", interval.ToString());
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
