// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalDateTest
    {
        [Test]
        public void Equals_EqualValues()
        {
            CalendarSystem calendar = CalendarSystem.Julian;
            LocalDate date1 = new LocalDate(2011, 1, 2, calendar);
            LocalDate date2 = new LocalDate(2011, 1, 2, calendar);
            Assert.AreEqual(date1, date2);
            Assert.AreEqual(date1.GetHashCode(), date2.GetHashCode());
            Assert.IsTrue(date1 == date2);
            Assert.IsFalse(date1 != date2);
            Assert.IsTrue(date1.Equals(date2)); // IEquatable implementation
        }

        [Test]
        public void Equals_DifferentDates()
        {
            CalendarSystem calendar = CalendarSystem.Julian;
            LocalDate date1 = new LocalDate(2011, 1, 2, calendar);
            LocalDate date2 = new LocalDate(2011, 1, 3, calendar);
            Assert.AreNotEqual(date1, date2);
            Assert.AreNotEqual(date1.GetHashCode(), date2.GetHashCode());
            Assert.IsFalse(date1 == date2);
            Assert.IsTrue(date1 != date2);
            Assert.IsFalse(date1.Equals(date2)); // IEquatable implementation
        }

        [Test]
        public void Equals_DifferentCalendars()
        {
            CalendarSystem calendar = CalendarSystem.Julian;
            LocalDate date1 = new LocalDate(2011, 1, 2, calendar);
            LocalDate date2 = new LocalDate(2011, 1, 2, CalendarSystem.Iso);
            Assert.AreNotEqual(date1, date2);
            Assert.AreNotEqual(date1.GetHashCode(), date2.GetHashCode());
            Assert.IsFalse(date1 == date2);
            Assert.IsTrue(date1 != date2);
            Assert.IsFalse(date1.Equals(date2)); // IEquatable implementation
        }

        [Test]
        public void Equals_DifferentToNull()
        {
            LocalDate date = new LocalDate(2011, 1, 2);
            Assert.IsFalse(date.Equals(null));
        }

        [Test]
        public void Equals_DifferentToOtherType()
        {
            LocalDate date = new LocalDate(2011, 1, 2);
            Assert.IsFalse(date.Equals(Instant.FromTicksSinceUnixEpoch(0)));
        }

        [Test]
        public void ComparisonOperators_SameCalendar()
        {
            LocalDate date1 = new LocalDate(2011, 1, 2);
            LocalDate date2 = new LocalDate(2011, 1, 2);
            LocalDate date3 = new LocalDate(2011, 1, 5);

            Assert.IsFalse(date1 < date2);
            Assert.IsTrue(date1 < date3);
            Assert.IsFalse(date2 < date1);
            Assert.IsFalse(date3 < date1);

            Assert.IsTrue(date1 <= date2);
            Assert.IsTrue(date1 <= date3);
            Assert.IsTrue(date2 <= date1);
            Assert.IsFalse(date3 <= date1);

            Assert.IsFalse(date1 > date2);
            Assert.IsFalse(date1 > date3);
            Assert.IsFalse(date2 > date1);
            Assert.IsTrue(date3 > date1);

            Assert.IsTrue(date1 >= date2);
            Assert.IsFalse(date1 >= date3);
            Assert.IsTrue(date2 >= date1);
            Assert.IsTrue(date3 >= date1);
        }

        [Test]
        public void ComparisonOperators_DifferentCalendars_Throws()
        {
            LocalDate date1 = new LocalDate(2011, 1, 2);
            LocalDate date2 = new LocalDate(2011, 1, 3, CalendarSystem.Julian);

            Assert.Throws<ArgumentException>(() => (date1 < date2).ToString());
            Assert.Throws<ArgumentException>(() => (date1 <= date2).ToString());
            Assert.Throws<ArgumentException>(() => (date1 > date2).ToString());
            Assert.Throws<ArgumentException>(() => (date1 >= date2).ToString());
        }

        [Test]
        public void CompareTo_SameCalendar()
        {
            LocalDate date1 = new LocalDate(2011, 1, 2);
            LocalDate date2 = new LocalDate(2011, 1, 2);
            LocalDate date3 = new LocalDate(2011, 1, 5);

            Assert.That(date1.CompareTo(date2), Is.EqualTo(0));
            Assert.That(date1.CompareTo(date3), Is.LessThan(0));
            Assert.That(date3.CompareTo(date2), Is.GreaterThan(0));
        }

        [Test]
        public void CompareTo_DifferentCalendars_Throws()
        {
            CalendarSystem islamic = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Astronomical);
            LocalDate date1 = new LocalDate(2011, 1, 2);
            LocalDate date2 = new LocalDate(1500, 1, 1, islamic);

            Assert.Throws<ArgumentException>(() => date1.CompareTo(date2));
            Assert.Throws<ArgumentException>(() => ((IComparable) date1).CompareTo(date2));
        }

        /// <summary>
        /// IComparable.CompareTo works properly with LocalDate inputs with same calendar.
        /// </summary>
        [Test]
        public void IComparableCompareTo_SameCalendar()
        {
            var instance = new LocalDate(2012, 3, 5);
            var i_instance = (IComparable)instance;
            
            var later = new LocalDate(2012, 6, 4);
            var earlier = new LocalDate(2012, 1, 4);
            var same = new LocalDate(2012, 3, 5);

            Assert.That(i_instance.CompareTo(later), Is.LessThan(0));
            Assert.That(i_instance.CompareTo(earlier), Is.GreaterThan(0));
            Assert.That(i_instance.CompareTo(same), Is.EqualTo(0));
        }

        /// <summary>
        /// IComparable.CompareTo returns a positive number for a null input.
        /// </summary>
        [Test]
        public void IComparableCompareTo_Null_Positive()
        {
            var instance = new LocalDate(2012, 3, 5);
            var i_instance = (IComparable)instance;
            object arg = null;
            var result = i_instance.CompareTo(arg);
            Assert.Greater(result, 0);
        }

        /// <summary>
        /// IComparable.CompareTo throws an ArgumentException for non-null arguments 
        /// that are not a LocalDate.
        /// </summary>
        [Test]
        public void IComparableCompareTo_WrongType_ArgumentException()
        {
            var instance = new LocalDate(2012, 3, 5);
            var i_instance = (IComparable)instance;
            var arg = new LocalDateTime(2012, 3, 6, 15, 42);
            Assert.Throws<ArgumentException>(() =>
            {
                i_instance.CompareTo(arg);
            });
        }
    }
}
