﻿// Copyright 2013 The Noda Time Authors. All rights reserved.
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

        [Test]
        public void Deconstruction()
        {
            var start = new LocalDate(2017, 11, 6);
            var end = new LocalDate(2017, 11, 10);
            var value = new DateInterval(start, end);

            var (actualStart, actualEnd) = value;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(start, actualStart);
                Assert.AreEqual(end, actualEnd);
            });
        }

        [Test]
        public void Contains_NullInterval_Throws()
        {
            var start = new LocalDate(2017, 11, 6);
            var end = new LocalDate(2017, 11, 10);
            var value = new DateInterval(start, end);

            Assert.That(() => value.Contains(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Contains_IntervalWithinAnotherCalendar_Throws()
        {
            var value = new DateInterval(
                new LocalDate(2017, 11, 6, CalendarSystem.Gregorian),
                new LocalDate(2017, 11, 10, CalendarSystem.Gregorian));

            var other = new DateInterval(
                new LocalDate(2017, 11, 6, CalendarSystem.Coptic),
                new LocalDate(2017, 11, 10, CalendarSystem.Coptic));

            Assert.That(() => value.Contains(other), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Contains_EqualInterval()
        {
            var start = new LocalDate(2014, 3, 7);
            var end = new LocalDate(2014, 3, 7);
            var value = new DateInterval(start, end);
            var same = new DateInterval(start, end);

            Assert.True(value.Contains(same));
        }

        [Test]
        public void Contains_LaterInterval()
        {
            var value = new DateInterval(new LocalDate(2014, 3, 7), new LocalDate(2014, 3, 31));
            var other = new DateInterval(new LocalDate(2015, 1, 1), new LocalDate(2015, 4, 1));

            Assert.False(value.Contains(other));
        }

        [Test]
        public void Contains_EarlierInterval()
        {
            var value = new DateInterval(new LocalDate(2014, 3, 7), new LocalDate(2014, 3, 31));
            var other = new DateInterval(new LocalDate(2013, 6, 26), new LocalDate(2013, 7, 25));

            Assert.False(value.Contains(other));
        }

        [Test]
        public void Contains_IntervalWithSameStartDate()
        {
            var value = new DateInterval(new LocalDate(2017, 11, 1), new LocalDate(2017, 11, 29));
            var other = new DateInterval(new LocalDate(2017, 11, 1), new LocalDate(2017, 11, 15));

            Assert.True(value.Contains(other));
        }

        [Test]
        public void Contains_IntervalWithSameEndDate()
        {
            var value = new DateInterval(new LocalDate(2017, 11, 3), new LocalDate(2017, 11, 29));
            var other = new DateInterval(new LocalDate(2017, 11, 8), new LocalDate(2017, 11, 29));

            Assert.True(value.Contains(other));
        }

        [Test]
        public void Contains_IntervalFullyWithin()
        {
            var year2016 = new DateInterval(new LocalDate(2016, 1, 1), new LocalDate(2016, 12, 31));
            var june2016 = new DateInterval(new LocalDate(2016, 6, 1), new LocalDate(2016, 6, 30));

            Assert.True(year2016.Contains(june2016));
        }

        [Test]
        public void Contains_IntersectingAtTheStart()
        {
            var value = new DateInterval(new LocalDate(2017, 11, 3), new LocalDate(2017, 11, 29));
            var other = new DateInterval(new LocalDate(2017, 11, 1), new LocalDate(2017, 11, 15));

            Assert.False(value.Contains(other));
        }

        [Test]
        public void Contains_IntersectingAtTheEnd()
        {
            var value = new DateInterval(new LocalDate(2017, 11, 3), new LocalDate(2017, 11, 29));
            var other = new DateInterval(new LocalDate(2017, 11, 10), new LocalDate(2017, 11, 30));

            Assert.False(value.Contains(other));
        }

        [Test]
        public void Contains_Superset()
        {
            var november2017 = new DateInterval(new LocalDate(2017, 11, 1), new LocalDate(2017, 11, 30));
            var year2017 = new DateInterval(new LocalDate(2017, 1, 1), new LocalDate(2017, 12, 31));

            Assert.False(november2017.Contains(year2017));
        }

        [Test]
        public void Intersects_NullInterval_Throws()
        {
            var value = new DateInterval(new LocalDate(100), new LocalDate(200));
            Assert.That(() => value.Intersects(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Intersects_IntervalInDifferentCalendar_Throws()
        {
            var value = new DateInterval(
                new LocalDate(2017, 11, 6, CalendarSystem.Gregorian),
                new LocalDate(2017, 11, 10, CalendarSystem.Gregorian));

            var other = new DateInterval(
                new LocalDate(2017, 11, 6, CalendarSystem.Coptic),
                new LocalDate(2017, 11, 10, CalendarSystem.Coptic));

            Assert.That(() => value.Intersects(other), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Intersects_EarlierInterval()
        {
            var value = new DateInterval(new LocalDate(2014, 3, 7), new LocalDate(2014, 3, 31));
            var other = new DateInterval(new LocalDate(2013, 6, 26), new LocalDate(2013, 7, 25));

            Assert.False(value.Intersects(other));
        }

        [Test]
        public void Intersects_LaterInterval()
        {
            var value = new DateInterval(new LocalDate(2013, 6, 26), new LocalDate(2013, 7, 25));
            var other = new DateInterval(new LocalDate(2014, 3, 7), new LocalDate(2014, 3, 31));

            Assert.False(value.Intersects(other));
        }

        [Test]
        public void Intersects_EqualInterval()
        {
            var value = new DateInterval(new LocalDate(2013, 6, 26), new LocalDate(2013, 7, 25));
            var equal = new DateInterval(new LocalDate(2013, 6, 26), new LocalDate(2013, 7, 25));

            Assert.True(value.Intersects(equal));
        }

        [Test]
        public void Intersects_IntersectingAtTheStart()
        {
            var value = new DateInterval(new LocalDate(2017, 11, 3), new LocalDate(2017, 11, 29));
            var other = new DateInterval(new LocalDate(2017, 11, 1), new LocalDate(2017, 11, 5));

            Assert.True(value.Intersects(other));
        }

        [Test]
        public void Intersects_IntersectingAtTheEnd()
        {
            var value = new DateInterval(new LocalDate(2017, 11, 3), new LocalDate(2017, 11, 25));
            var other = new DateInterval(new LocalDate(2017, 11, 18), new LocalDate(2017, 11, 30));

            Assert.True(value.Intersects(other));
        }

        [Test]
        public void Intersects_Superset()
        {
            var november2017 = new DateInterval(new LocalDate(2017, 11, 1), new LocalDate(2017, 11, 30));
            var year2017 = new DateInterval(new LocalDate(2017, 1, 1), new LocalDate(2017, 12, 31));

            Assert.True(november2017.Intersects(year2017));
        }

        [Test]
        public void Intersects_Subset()
        {
            var year2017 = new DateInterval(new LocalDate(2017, 1, 1), new LocalDate(2017, 12, 31));
            var november2017 = new DateInterval(new LocalDate(2017, 11, 1), new LocalDate(2017, 11, 30));

            Assert.True(year2017.Intersects(november2017));
        }

        [Test]
        public void Intersection_NullInterval_Throws()
        {
            var value = new DateInterval(new LocalDate(100), new LocalDate(200));
            Assert.That(() => value.Intersection(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Intersection_IntervalInDifferentCalendar_Throws()
        {
            var value = new DateInterval(
                new LocalDate(2017, 11, 6, CalendarSystem.Gregorian),
                new LocalDate(2017, 11, 10, CalendarSystem.Gregorian));

            var other = new DateInterval(
                new LocalDate(2017, 11, 6, CalendarSystem.Coptic),
                new LocalDate(2017, 11, 10, CalendarSystem.Coptic));

            Assert.That(() => value.Intersection(other), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Intersection_EqualIntervals()
        {
            var value = new DateInterval(new LocalDate(100), new LocalDate(200));
            var other = new DateInterval(new LocalDate(100), new LocalDate(200));

            Assert.AreEqual(value, value.Intersection(other));
        }

        [Test]
        public void Intersection_EarlierInterval()
        {
            var value = new DateInterval(new LocalDate(2014, 3, 7), new LocalDate(2014, 3, 31));
            var other = new DateInterval(new LocalDate(2013, 6, 26), new LocalDate(2013, 7, 25));

            Assert.Null(value.Intersection(other));
        }

        [Test]
        public void Intersection_LaterInterval()
        {
            var value = new DateInterval(new LocalDate(2013, 6, 26), new LocalDate(2013, 7, 25));
            var other = new DateInterval(new LocalDate(2014, 12, 7), new LocalDate(2015, 1, 1));

            Assert.Null(value.Intersection(other));
        }

        [Test]
        public void Intersection_IntersectingAtTheStart()
        {
            var value = new DateInterval(new LocalDate(2017, 11, 3), new LocalDate(2017, 11, 29));
            var other = new DateInterval(new LocalDate(2017, 11, 1), new LocalDate(2017, 11, 12));
            var expected = new DateInterval(new LocalDate(2017, 11, 3), new LocalDate(2017, 11, 12));

            Assert.AreEqual(expected, value.Intersection(other));
        }

        [Test]
        public void Intersection_IntersectingAtTheEnd()
        {
            var value = new DateInterval(new LocalDate(2017, 11, 3), new LocalDate(2017, 11, 25));
            var other = new DateInterval(new LocalDate(2017, 11, 18), new LocalDate(2017, 11, 30));
            var expected = new DateInterval(new LocalDate(2017, 11, 18), new LocalDate(2017, 11, 25));

            Assert.AreEqual(expected, value.Intersection(other));
        }

        [Test]
        public void Intersection_Subset()
        {
            var year2017 = new DateInterval(new LocalDate(2017, 1, 1), new LocalDate(2017, 12, 31));
            var november2017 = new DateInterval(new LocalDate(2017, 11, 1), new LocalDate(2017, 11, 30));
            var expected = november2017;

            Assert.AreEqual(expected, year2017.Intersection(november2017));
        }
    }
}