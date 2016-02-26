// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    /// <summary>
    /// Tests for the Julian calendar system via JulianYearMonthDayCalculator.
    /// </summary>
    public partial class JulianCalendarSystemTest
    {
        private static readonly CalendarSystem Julian = CalendarSystem.Julian;

        /// <summary>
        /// The Unix epoch is equivalent to December 19th 1969 in the Julian calendar.
        /// </summary>
        [Test]
        public void Epoch()
        {
            LocalDateTime julianEpoch = NodaConstants.UnixEpoch.InZone(DateTimeZone.Utc, Julian).LocalDateTime;
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
    }
}
