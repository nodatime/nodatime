// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Linq;
using NUnit.Framework;
using NodaTime.Calendars;

namespace NodaTime.Test.Calendars
{
    /// <summary>
    /// Tests for <see cref="JulianCalendarSystem"/>.
    /// </summary>
    [TestFixture]
    public partial class JulianCalendarSystemTest
    {
        private static readonly CalendarSystem Julian = JulianCalendarSystem.GetInstance(4);

        /// <summary>
        /// The Unix epoch is equivalent to December 19th 1969 in the Julian calendar.
        /// </summary>
        [Test]
        public void Epoch()
        {
            LocalDateTime julianEpoch = new LocalDateTime(LocalInstant.LocalUnixEpoch, Julian);
            Assert.AreEqual(1969, julianEpoch.Year);
            Assert.AreEqual(12, julianEpoch.Month);
            Assert.AreEqual(19, julianEpoch.Day);
        }

        [Test]
        public void LeapYears()
        {
            Assert.IsTrue(Julian.IsLeapYear(1900)); // No 100 year rule...
            Assert.IsFalse(Julian.IsLeapYear(1901));
            Assert.IsTrue(Julian.IsLeapYear(1904));
            Assert.IsTrue(Julian.IsLeapYear(2000));
            Assert.IsTrue(Julian.IsLeapYear(2100)); // No 100 year rule...
            Assert.IsTrue(Julian.IsLeapYear(2400));
            // Check 1BC, 5BC etc...
            Assert.IsTrue(Julian.IsLeapYear(0));
            Assert.IsTrue(Julian.IsLeapYear(-4));
        }

        [Test]
        public void GetInstance_UniqueIds()
        {
            Assert.AreEqual(7, Enumerable.Range(1, 7).Select(x => JulianCalendarSystem.GetInstance(x).Id).Distinct().Count());
        }

        [Test]
        public void GetInstance_InvalidMinDaysInFirstWeek()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => JulianCalendarSystem.GetInstance(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => JulianCalendarSystem.GetInstance(8));
        }

        [Test]
        public void GetInstance_MinDaysInFirstWeekIsRespected()
        {
            // Seems the simplest way to test this... yes, it seems somewhat wasteful, but hey...
            for (int i = 1; i < 7; i++)
            {
                JulianCalendarSystem calendar = JulianCalendarSystem.GetInstance(i);

                int actualMin = Enumerable.Range(1900, 400)
                                          .Select(year => GetDaysInFirstWeek(year, calendar))
                                          .Min();
                Assert.AreEqual(i, actualMin);
            }
        }

        private int GetDaysInFirstWeek(int year, JulianCalendarSystem calendar)
        {
            // Some of the first few days of the week year may be in the previous week year.
            // However, the whole of the first week of the week year definitely occurs
            // within the first 13 days of January.
            return Enumerable.Range(1, 13)
                             .Count(day => new LocalDate(year, 1, day, calendar).WeekOfWeekYear == 1);
        }
    }
}
