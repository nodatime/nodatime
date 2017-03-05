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
        public void Construction_DefaultToEndInclusive()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval = new DateInterval(start, end);
            Assert.IsTrue(interval.EndInclusive);
        }

        [Test]
        public void Construction_Properties()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval = new DateInterval(start, end, false);
            Assert.AreEqual(start, interval.Start);
            Assert.AreEqual(end, interval.End);
            Assert.IsFalse(interval.EndInclusive);
        }

        [Test]
        public void Equals_SameInstance()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval = new DateInterval(start, end, false);

            Assert.AreEqual(interval, interval);
            Assert.AreEqual(interval.GetHashCode(), interval.GetHashCode());
            // CS1718: Comparison made to same variable.  This is intentional to test operator ==.
#pragma warning disable 1718
            Assert.IsTrue(interval == interval);
            Assert.IsFalse(interval != interval);
#pragma warning restore 1718
            Assert.IsTrue(interval.Equals(interval)); // IEquatable implementation

            IEqualityComparer<DateInterval> comparer = DateInterval.NormalizingEqualityComparer;
            Assert.IsTrue(comparer.Equals(interval, interval));
            Assert.AreEqual(comparer.GetHashCode(interval), comparer.GetHashCode(interval));

            comparer = DateInterval.ContainedDatesEqualityComparer;
            Assert.IsTrue(comparer.Equals(interval, interval));
            Assert.AreEqual(comparer.GetHashCode(interval), comparer.GetHashCode(interval));
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

            var comparer = DateInterval.NormalizingEqualityComparer;
            Assert.IsTrue(comparer.Equals(interval1, interval2));
            Assert.AreEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));

            comparer = DateInterval.ContainedDatesEqualityComparer;
            Assert.IsTrue(comparer.Equals(interval1, interval2));
            Assert.AreEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));
        }

        [Test]
        public void Equals_DifferentCalendars()
        {
            LocalDate start1 = new LocalDate(2000, 1, 1);
            LocalDate end1 = new LocalDate(2001, 6, 19);
            // This is a really, really similar calendar to ISO, but we do distinguish.
            LocalDate start2 = start1.WithCalendar(CalendarSystem.Gregorian);
            LocalDate end2 = end1.WithCalendar(CalendarSystem.Gregorian);
            var interval1 = new DateInterval(start1, end1, false);
            var interval2 = new DateInterval(start2, end2, false);

            Assert.AreNotEqual(interval1, interval2);
            Assert.AreNotEqual(interval1.GetHashCode(), interval2.GetHashCode());
            Assert.IsFalse(interval1 == interval2);
            Assert.IsTrue(interval1 != interval2);
            Assert.IsFalse(interval1.Equals(interval2)); // IEquatable implementation

            var comparer = DateInterval.NormalizingEqualityComparer;
            Assert.IsFalse(comparer.Equals(interval1, interval2));
            Assert.AreNotEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));  // probably

            comparer = DateInterval.ContainedDatesEqualityComparer;
            Assert.IsFalse(comparer.Equals(interval1, interval2));
            Assert.AreNotEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));  // probably
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

            var comparer = DateInterval.NormalizingEqualityComparer;
            Assert.IsFalse(comparer.Equals(interval1, interval2));
            Assert.AreNotEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));  // probably

            comparer = DateInterval.ContainedDatesEqualityComparer;
            Assert.IsFalse(comparer.Equals(interval1, interval2));
            Assert.AreNotEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));  // probably
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

            var comparer = DateInterval.NormalizingEqualityComparer;
            Assert.IsFalse(comparer.Equals(interval1, interval2));
            Assert.AreNotEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));  // probably

            comparer = DateInterval.ContainedDatesEqualityComparer;
            Assert.IsFalse(comparer.Equals(interval1, interval2));
            Assert.AreNotEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));  // probably
        }

        [Test]
        public void Equals_DifferentEndInclusivity()
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

            var comparer = DateInterval.NormalizingEqualityComparer;
            Assert.IsFalse(comparer.Equals(interval1, interval2));
            Assert.AreNotEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));  // probably

            comparer = DateInterval.ContainedDatesEqualityComparer;
            Assert.IsFalse(comparer.Equals(interval1, interval2));
            Assert.AreNotEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));  // probably
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

            // This is the key difference between this comparer and standard equality.
            var comparer = DateInterval.NormalizingEqualityComparer;
            Assert.IsTrue(comparer.Equals(interval1, interval2));
            Assert.AreEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));

            // The same range with different inclusivity is equal, like NormalizingEqualityComparer.
            comparer = DateInterval.ContainedDatesEqualityComparer;
            Assert.IsTrue(comparer.Equals(interval1, interval2));
            Assert.AreEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));
        }

        [Test]
        public void Equals_SameRangeButNotEquivalent_Extremes_Equal()
        {
            // As above, but for an extreme range.
            var interval1 = new DateInterval(MinIsoDate, MaxIsoDate.PlusDays(-1), true);
            var interval2 = new DateInterval(MinIsoDate, MaxIsoDate, false);

            Assert.AreEqual(interval1.Length, interval2.Length);
            Assert.AreNotEqual(interval1, interval2);

            var comparer = DateInterval.NormalizingEqualityComparer;
            Assert.IsTrue(comparer.Equals(interval1, interval2));
            Assert.AreEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));

            comparer = DateInterval.ContainedDatesEqualityComparer;
            Assert.IsTrue(comparer.Equals(interval1, interval2));
            Assert.AreEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));
        }

        [Test]
        public void Equals_Extremes_Equal_BothEndInclusive()
        {
            // An extreme range where both intervals are inclusive.
            var interval1 = new DateInterval(MinIsoDate, MaxIsoDate, true);
            var interval2 = new DateInterval(MinIsoDate, MaxIsoDate, true);

            Assert.AreEqual(interval1, interval2);

            // These are equal under the regular equality operation; they should still be equal under this comparer,
            // even though neither interval can be represented as an exclusive interval.
            var comparer = DateInterval.NormalizingEqualityComparer;
            Assert.IsTrue(comparer.Equals(interval1, interval2));
            Assert.AreEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));

            comparer = DateInterval.ContainedDatesEqualityComparer;
            Assert.IsTrue(comparer.Equals(interval1, interval2));
            Assert.AreEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));
        }

        [Test]
        public void Equals_Extremes_Unequal()
        {
            // Two extreme ranges, where one interval is inclusive.
            var interval1 = new DateInterval(MinIsoDate, MaxIsoDate, true);
            var interval2 = new DateInterval(MinIsoDate, MaxIsoDate, false);

            Assert.AreNotEqual(interval1, interval2);

            var comparer = DateInterval.NormalizingEqualityComparer;
            Assert.IsFalse(comparer.Equals(interval1, interval2));
            Assert.AreNotEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));  // probably

            comparer = DateInterval.ContainedDatesEqualityComparer;
            Assert.IsFalse(comparer.Equals(interval1, interval2));
            Assert.AreNotEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));  // probably
        }

        [Test]
        public void Equals_Extremes_Unequal_DifferentCalendars()
        {
            // Two extreme ranges with different calendars but the 'same' dates are still unequal.
            LocalDate start1 = MinIsoDate;
            LocalDate end1 = MaxIsoDate;
            LocalDate start2 = start1.WithCalendar(CalendarSystem.Gregorian);
            LocalDate end2 = end1.WithCalendar(CalendarSystem.Gregorian);
            var interval1 = new DateInterval(start1, end1, true);
            var interval2 = new DateInterval(start2, end2, true);

            Assert.AreNotEqual(interval1, interval2);

            // Neither interval above can be represented as an exclusive interval, but they must still compare unequal.
            var comparer = DateInterval.NormalizingEqualityComparer;
            Assert.IsFalse(comparer.Equals(interval1, interval2));
            Assert.AreNotEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));  // probably

            comparer = DateInterval.ContainedDatesEqualityComparer;
            Assert.IsFalse(comparer.Equals(interval1, interval2));
            Assert.AreNotEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));  // probably
        }

        [Test]
        public void Equals_EmptyRange()
        {
            LocalDate date1 = new LocalDate(2000, 1, 1);
            LocalDate date2 = new LocalDate(2000, 1, 2);
            var interval1 = new DateInterval(date1, date1, false);
            var interval2 = new DateInterval(date2, date2, false);
            Assert.AreEqual(0, interval1.Length);
            Assert.AreEqual(0, interval2.Length);

            Assert.AreNotEqual(interval1, interval2);

            var comparer = DateInterval.NormalizingEqualityComparer;
            Assert.IsFalse(comparer.Equals(interval1, interval2));
            Assert.AreNotEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));  // probably

            // This is the key difference between this comparer and the normalizing comparer.
            comparer = DateInterval.ContainedDatesEqualityComparer;
            Assert.IsTrue(comparer.Equals(interval1, interval2));
            Assert.AreEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));
        }

        [Test]
        [TestCaseSource(nameof(SupportedCalendars))]
        public void ContainedDatesEqualityComparer_EmptyRange(CalendarSystem calendar)
        {
            // Similar to Equals_EmptyRange, but specifically for ContainedDatesEqualityComparer, which must be able to
            // construct a canonical empty range for every possible calendar.
            int year = (calendar.MaxYear + calendar.MinYear) / 2;
            LocalDate date1 = new LocalDate(year, 2, 1, calendar);
            LocalDate date2 = new LocalDate(year, 2, 2, calendar);
            var interval1 = new DateInterval(date1, date1, false);
            var interval2 = new DateInterval(date2, date2, false);
            Assert.AreEqual(0, interval1.Length);
            Assert.AreEqual(0, interval2.Length);
            Assert.AreNotEqual(interval1, interval2);

            var comparer = DateInterval.ContainedDatesEqualityComparer;
            Assert.IsTrue(comparer.Equals(interval1, interval2));
            Assert.AreEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));
        }

        [Test]
        public void Equals_EmptyRange_DifferentCalendars()
        {
            LocalDate date1 = new LocalDate(2000, 2, 1);
            LocalDate date2 = new LocalDate(2000, 2, 2, CalendarSystem.Gregorian);
            var interval1 = new DateInterval(date1, date1, false);
            var interval2 = new DateInterval(date2, date2, false);
            Assert.AreEqual(0, interval1.Length);
            Assert.AreEqual(0, interval2.Length);

            Assert.AreNotEqual(interval1, interval2);

            // Different calendar systems are still unequal, even if they're both empty.
            var comparer = DateInterval.NormalizingEqualityComparer;
            Assert.IsFalse(comparer.Equals(interval1, interval2));
            Assert.AreNotEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));  // probably

            comparer = DateInterval.ContainedDatesEqualityComparer;
            Assert.IsFalse(comparer.Equals(interval1, interval2));
            Assert.AreNotEqual(comparer.GetHashCode(interval1), comparer.GetHashCode(interval2));  // probably
        }

        [Test]
        public void Equals_DifferentToNull()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval = new DateInterval(start, end, false);

            Assert.IsFalse(interval.Equals(null));

            Assert.IsFalse(DateInterval.NormalizingEqualityComparer.Equals(interval, null));
            Assert.IsFalse(DateInterval.NormalizingEqualityComparer.Equals(null, interval));

            Assert.IsFalse(DateInterval.ContainedDatesEqualityComparer.Equals(interval, null));
            Assert.IsFalse(DateInterval.ContainedDatesEqualityComparer.Equals(null, interval));
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
        public void Equals_NullToNull()
        {
            Assert.IsTrue(DateInterval.NormalizingEqualityComparer.Equals(null, null));
            Assert.IsTrue(DateInterval.ContainedDatesEqualityComparer.Equals(null, null));
        }

        [Test]
        public void ToString_EndInclusive()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval = new DateInterval(start, end, true);
            Assert.AreEqual("[2000-01-01, 2001-06-19]", interval.ToString());
        }

        [Test]
        public void ToString_EndExclusive()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2001, 6, 19);
            var interval = new DateInterval(start, end, false);
            Assert.AreEqual("[2000-01-01, 2001-06-19)", interval.ToString());
        }

        [Test]
        public void Length_EndInclusive()
        {
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = new LocalDate(2000, 2, 10);
            var interval = new DateInterval(start, end, true);
            Assert.AreEqual(41, interval.Length);
        }

        [Test]
        public void Length_EndExclusive()
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
            var candidate = LocalDatePattern.Iso.Parse(candidateText).Value;
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
