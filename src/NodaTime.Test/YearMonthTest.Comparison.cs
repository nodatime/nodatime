// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NUnit.Framework;
using System;

namespace NodaTime.Test
{
    public partial class YearMonthTest
    {
        // These tests were copied from LocalDateTest and then modified for YearMonth.

        [Test]
        public void Equals_EqualValues()
        {
            CalendarSystem calendar = CalendarSystem.Julian;
            YearMonth yearMonth1 = new YearMonth(2011, 1, calendar);
            YearMonth yearMonth2 = new YearMonth(2011, 1, calendar);
            Assert.AreEqual(yearMonth1, yearMonth2);
            Assert.AreEqual(yearMonth1.GetHashCode(), yearMonth2.GetHashCode());
            Assert.IsTrue(yearMonth1 == yearMonth2);
            Assert.IsFalse(yearMonth1 != yearMonth2);
            Assert.IsTrue(yearMonth1.Equals(yearMonth2)); // IEquatable implementation
        }

        [Test]
        public void Equals_DifferentMonths()
        {
            CalendarSystem calendar = CalendarSystem.Julian;
            YearMonth yearMonth1 = new YearMonth(2011, 1, calendar);
            YearMonth yearMonth2 = new YearMonth(2011, 2, calendar);
            Assert.AreNotEqual(yearMonth1, yearMonth2);
            Assert.AreNotEqual(yearMonth1.GetHashCode(), yearMonth2.GetHashCode());
            Assert.IsFalse(yearMonth1 == yearMonth2);
            Assert.IsTrue(yearMonth1 != yearMonth2);
            Assert.IsFalse(yearMonth1.Equals(yearMonth2)); // IEquatable implementation
        }

        [Test]
        public void Equals_DifferentCalendars()
        {
            CalendarSystem calendar = CalendarSystem.Julian;
            YearMonth yearMonth1 = new YearMonth(2011, 1, calendar);
            YearMonth yearMonth2 = new YearMonth(2011, 1, CalendarSystem.Iso);
            Assert.AreNotEqual(yearMonth1, yearMonth2);
            Assert.AreNotEqual(yearMonth1.GetHashCode(), yearMonth2.GetHashCode());
            Assert.IsFalse(yearMonth1 == yearMonth2);
            Assert.IsTrue(yearMonth1 != yearMonth2);
            Assert.IsFalse(yearMonth1.Equals(yearMonth2)); // IEquatable implementation
        }

        [Test]
        public void Equals_DifferentToNull()
        {
            YearMonth date = new YearMonth(2011, 1);
            Assert.IsFalse(date.Equals(null!));
        }

        [Test]
        public void Equals_DifferentToOtherType()
        {
            YearMonth date = new YearMonth(2011, 1);
            Assert.IsFalse(date.Equals(Instant.FromUnixTimeTicks(0)));
        }

        [Test]
        public void CompareTo_SameCalendar()
        {
            YearMonth yearMonth1 = new YearMonth(2011, 1);
            YearMonth yearMonth2 = new YearMonth(2011, 1);
            YearMonth yearMonth3 = new YearMonth(2011, 2);

            TestHelper.TestOperatorComparisonEquality(yearMonth1, yearMonth2, yearMonth3);
        }

        [Test]
        public void CompareTo_DifferentCalendars_Throws()
        {
            CalendarSystem islamic = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Astronomical);
            YearMonth yearMonth1 = new YearMonth(2011, 1);
            YearMonth yearMonth2 = new YearMonth(1500, 1, islamic);

            Assert.Throws<ArgumentException>(() => yearMonth1.CompareTo(yearMonth2));
            Assert.Throws<ArgumentException>(() => ((IComparable) yearMonth1).CompareTo(yearMonth2));
            Assert.Throws<ArgumentException>(() => (yearMonth1 > yearMonth2).ToString());
        }

        /// <summary>
        /// IComparable.CompareTo works properly with YearMonth inputs with same calendar.
        /// </summary>
        [Test]
        public void IComparableCompareTo_SameCalendar()
        {
            var instance = new YearMonth(2012, 3);
            var i_instance = (IComparable) instance;

            var later = new YearMonth(2012, 6);
            var earlier = new YearMonth(2012, 1);
            var same = new YearMonth(2012, 3);

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
            var instance = new YearMonth(2012, 3);
            var comparable = (IComparable) instance;
            var result = comparable.CompareTo(null!);
            Assert.Greater(result, 0);
        }

        /// <summary>
        /// IComparable.CompareTo throws an ArgumentException for non-null arguments 
        /// that are not a YearMonth.
        /// </summary>
        [Test]
        public void IComparableCompareTo_WrongType_ArgumentException()
        {
            var instance = new YearMonth(2012, 3);
            var i_instance = (IComparable) instance;
            var arg = new LocalDateTime(2012, 3, 6, 15, 42);
            Assert.Throws<ArgumentException>(() =>
            {
                i_instance.CompareTo(arg);
            });
        }
    }
}
