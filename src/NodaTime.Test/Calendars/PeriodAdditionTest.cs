// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    /// <summary>
    /// Tests for adding periods. These use LocalDateTime for simplicity; its + operator
    /// simply delegates to the relevant calendar. Likewise most tests use the ISO calendar.
    /// </summary>
    [TestFixture]
    public class PeriodAdditionTest
    {
        [Test]
        public void DayCrossingMonthBoundary()
        {
            LocalDateTime start = new LocalDateTime(2010, 2, 20, 10, 0);
            LocalDateTime result = start + Period.FromDays(10);
            Assert.AreEqual(new LocalDateTime(2010, 3, 2, 10, 0), result);
        }

        [Test]
        public void AddOneYearOnLeapDay()
        {
            LocalDateTime start = new LocalDateTime(2012, 2, 29, 10, 0);
            LocalDateTime result = start + Period.FromYears(1);
            // Feb 29th becomes Feb 28th
            Assert.AreEqual(new LocalDateTime(2013, 2, 28, 10, 0), result);
        }

        [Test]
        public void AddFourYearsOnLeapDay()
        {
            LocalDateTime start = new LocalDateTime(2012, 2, 29, 10, 0);
            LocalDateTime result = start + Period.FromYears(4);
            // Feb 29th is still valid in 2016
            Assert.AreEqual(new LocalDateTime(2016, 2, 29, 10, 0), result);
        }

        [Test]
        public void AddYearMonthDay()
        {
            // One year, one month, two days
            Period period = Period.FromYears(1) + Period.FromMonths(1) + Period.FromDays(2);
            LocalDateTime start = new LocalDateTime(2007, 1, 30, 0, 0);
            // Periods are added in order, so this becomes...
            // Add one year: Jan 30th 2008
            // Add one month: Feb 29th 2008
            // Add two days: March 2nd 2008
            // If we added the days first, we'd end up with March 1st instead.
            LocalDateTime result = start + period;
            Assert.AreEqual(new LocalDateTime(2008, 3, 2, 0, 0), result);
        }
    }
}