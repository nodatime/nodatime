// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Test.TimeZones.IO;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class ZoneYearOffsetTest
    {
        [Test]
        public void Construct_InvalidMonth_Exception()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 0, 1, 1, true, LocalTime.Midnight), "Month 0");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 34, 1, 1, true, LocalTime.Midnight), "Month 34");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, -3, 1, 1, true, LocalTime.Midnight), "Month -3");
        }

        [Test]
        public void Construct_InvalidDayOfMonth_Exception()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, 0, 1, true, LocalTime.Midnight), "Day of Month 0");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, 32, 1, true, LocalTime.Midnight), "Day of Month 32");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, 475, 1, true, LocalTime.Midnight),
                          "Day of Month 475");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, -32, 1, true, LocalTime.Midnight),
                          "Day of Month -32");
        }

        [Test]
        public void Construct_InvalidDayOfWeek_Exception()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, 3, -1, true, LocalTime.Midnight), "Day of Week -1");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, 3, 8, true, LocalTime.Midnight), "Day of Week 8");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, 3, 5756, true, LocalTime.Midnight),
                          "Day of Week 5856");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, 3, -347, true, LocalTime.Midnight),
                          "Day of Week -347");
        }

        [Test]
        public void Construct_ValidMonths()
        {
            for (int month = 1; month <= 12; month++)
            {
                Assert.NotNull(new ZoneYearOffset(TransitionMode.Standard, month, 1, 1, true, LocalTime.Midnight), "Month " + month);
            }
        }

        [Test]
        public void Construct_ValidDays()
        {
            for (int day = 1; day <= 31; day++)
            {
                Assert.NotNull(new ZoneYearOffset(TransitionMode.Standard, 1, day, 1, true, LocalTime.Midnight), "Day " + day);
            }
            for (int day = -1; day >= -31; day--)
            {
                Assert.NotNull(new ZoneYearOffset(TransitionMode.Standard, 1, day, 1, true, LocalTime.Midnight), "Day " + day);
            }
        }

        [Test]
        public void Construct_ValidDaysOfWeek()
        {
            for (int dayOfWeek = 0; dayOfWeek <= 7; dayOfWeek++)
            {
                Assert.NotNull(new ZoneYearOffset(TransitionMode.Standard, 1, 1, dayOfWeek, true, LocalTime.Midnight), "Day of week " + dayOfWeek);
            }
        }

        [Test]
        public void GetOccurrenceForYear_Defaults_Epoch()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, LocalTime.Midnight);
            var actual = offset.GetOccurrenceForYear(1970);
            var expected = new LocalDateTime(1970, 1, 1, 0, 0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetOccurrenceForYear_Year_1971()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, LocalTime.Midnight);
            var actual = offset.GetOccurrenceForYear(1971);
            var expected = new LocalDateTime(1971, 1, 1, 0, 0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetOccurrenceForYear_SavingOffsetIgnored_Epoch()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, LocalTime.Midnight);
            var actual = offset.GetOccurrenceForYear(1970, Offset.FromHours(2), Offset.FromHours(1));
            var expected = new LocalDateTime(1970, 1, 1, 0, 0).WithOffset(Offset.Zero); // UTC transition
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetOccurrenceForYear_SavingIgnored()
        {
            var offset = new ZoneYearOffset(TransitionMode.Standard, 1, 1, 0, true, LocalTime.Midnight);
            var actual = offset.GetOccurrenceForYear(1970, Offset.FromHours(2), Offset.FromHours(1));
            var expected = new LocalDateTime(1970, 1, 1, 0, 0).WithOffset(Offset.FromHours(2)); // Standard transition
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetOccurrenceForYear_SavingAndOffset()
        {
            var offset = new ZoneYearOffset(TransitionMode.Wall, 1, 1, 0, true, LocalTime.Midnight);
            var actual = offset.GetOccurrenceForYear(1970, Offset.FromHours(2), Offset.FromHours(1));
            var expected = new LocalDateTime(1970, 1, 1, 0, 0).WithOffset(Offset.FromHours(3)); // Wall transition
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetOccurrenceForYear_Milliseconds()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, new LocalTime(0, 0, 0, 1));
            var actual = offset.GetOccurrenceForYear(1970);
            var expected = new LocalDateTime(1970, 1, 1, 0, 0, 0, 1);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetOccurrenceForYear_WednesdayForward()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, (int)DayOfWeek.Wednesday, true, LocalTime.Midnight);
            var actual = offset.GetOccurrenceForYear(1970);
            var expected = new LocalDateTime(1970, 1, 7, 0, 0); // 1970-01-01 was a Thursday
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetOccurrenceForYear_WednesdayBackward()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 15, (int)DayOfWeek.Wednesday, false, LocalTime.Midnight);
            var actual = offset.GetOccurrenceForYear(1970);
            var expected = new LocalDateTime(1970, 1, 14, 0, 0); // 1970-01-15 was a Thursday
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetOccurrenceForYear_JanMinusTwo()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, -2, 0, true, LocalTime.Midnight);
            var actual = offset.GetOccurrenceForYear(1970);
            var expected = new LocalDateTime(1970, 1, 30, 0, 0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetOccurrenceForYear_JanFive()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 5, 0, true, LocalTime.Midnight);
            var actual = offset.GetOccurrenceForYear(1970);
            var expected = new LocalDateTime(1970, 1, 5, 0, 0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetOccurrenceForYear_Feb()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 2, 1, 0, true, LocalTime.Midnight);
            var actual = offset.GetOccurrenceForYear(1970);
            var expected = new LocalDateTime(1970, 2, 1, 0, 0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetOccurrenceForYear_LastSundayInOctober()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 10, -1, (int)IsoDayOfWeek.Sunday, false, LocalTime.Midnight);
            var actual = offset.GetOccurrenceForYear(1996);
            var expected = new LocalDateTime(1996, 10, 27, 0, 0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetOccurrenceForYear_ExactlyFeb29th_LeapYear()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 2, 29, 0, false, LocalTime.Midnight);
            var actual = offset.GetOccurrenceForYear(2012);
            var expected = new LocalDateTime(2012, 2, 29, 0, 0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetOccurrenceForYear_ExactlyFeb29th_NotLeapYear()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 2, 29, 0, false, LocalTime.Midnight);
            Assert.Throws<InvalidOperationException>(() => offset.GetOccurrenceForYear(2013));
        }

        [Test]
        public void GetOccurrenceForYear_AtLeastFeb29th_LeapYear()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 2, 29, (int) IsoDayOfWeek.Sunday, true, LocalTime.Midnight);
            var actual = offset.GetOccurrenceForYear(2012);
            var expected = new LocalDateTime(2012, 3, 4, 0, 0); // March 4th is the first Sunday after 2012-02-29
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetOccurrenceForYear_AtLeastFeb29th_NotLeapYear()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 2, 29, (int) IsoDayOfWeek.Sunday, true, LocalTime.Midnight);
            Assert.Throws<InvalidOperationException>(() => offset.GetOccurrenceForYear(2013));
        }

        [Test]
        public void GetOccurrenceForYear_AtMostFeb29th_LeapYear()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 2, 29, (int) IsoDayOfWeek.Sunday, false, LocalTime.Midnight);
            var actual = offset.GetOccurrenceForYear(2012);
            var expected = new LocalDateTime(2012, 2, 26, 0, 0); // Feb 26th is the last Sunday before 2012-02-29
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetOccurrenceForYear_AtMostFeb29th_NotLeapYear()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 2, 29, (int) IsoDayOfWeek.Sunday, false, LocalTime.Midnight);
            var actual = offset.GetOccurrenceForYear(2013);
            var expected = new LocalDateTime(2013, 2, 24, 0, 0); // Feb 24th is the last Sunday is February 2013
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetOccurrenceForYear_WithAddDay()
        {
            // Last Thursday in October, then add 24 hours. The last Thursday in October 2013 is the 31st, so
            // we should get the start of November 1st.
            var offset = new ZoneYearOffset(TransitionMode.Utc, 10, -1, (int) IsoDayOfWeek.Thursday, false, LocalTime.Midnight, true);
            var actual = offset.GetOccurrenceForYear(2013);
            var expected = new LocalDateTime(2013, 11, 1, 0, 0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Serialization()
        {
            var dio = DtzIoHelper.CreateNoStringPool();
            var expected = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true,
                new LocalTime(12, 34, 45, 678));
            dio.TestZoneYearOffset(expected);

            dio.Reset();
            expected = new ZoneYearOffset(TransitionMode.Utc, 10, -31, (int)IsoDayOfWeek.Wednesday, true, LocalTime.Midnight);
            dio.TestZoneYearOffset(expected);
        }

        [Test]
        public void IEquatable_Tests()
        {
            var value = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true, LocalTime.Midnight);
            var equalValue = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true, LocalTime.Midnight);
            var unequalValue = new ZoneYearOffset(TransitionMode.Utc, 9, 31, (int)IsoDayOfWeek.Wednesday, true, LocalTime.Midnight);

            TestHelper.TestEqualsClass(value, equalValue, unequalValue);
        }
    }
}
