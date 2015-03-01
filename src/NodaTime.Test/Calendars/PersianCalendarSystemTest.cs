// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Globalization;
using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    /// <summary>
    /// Tests for PersianYearMonthDayCalculator via the Persian CalendarSystem.
    /// </summary>
    [TestFixture]
    public class PersianCalendarSystemTest
    {
        [Test, Timeout(300000)] // Can take a long time under NCrunch.
        public void BclThroughHistory()
        {
            Calendar bcl = new PersianCalendar();
            // The "right" BCL equivalent to use depends on the version of .NET... pick it appropriately here.
            CalendarSystem noda = bcl.IsLeapYear(1) ? CalendarSystem.PersianSimple : CalendarSystem.PersianAstronomical;
            // Note: Noda Time stops in 9377, whereas the BCL goes into the start of 9378. This is because
            // Noda Time ensures that the whole year is valid.
            BclEquivalenceHelper.AssertEquivalent(bcl, noda, noda.MinYear, noda.MaxYear);
        }

        /// <summary>
        /// Use the examples in Calendrical Calculations for where the arithmetic calendar differs
        /// from the astronomical one.
        /// </summary>
        [Test]
        [TestCase(1016, 1637, 21)]
        [TestCase(1049, 1670, 21)]
        [TestCase(1078, 1699, 21)]
        [TestCase(1082, 1703, 22)]
        [TestCase(1111, 1732, 21)]
        [TestCase(1115, 1736, 21)]
        [TestCase(1144, 1765, 21)]
        [TestCase(1177, 1798, 21)]
        [TestCase(1210, 1831, 22)]
        [TestCase(1243, 1864, 21)]
        [TestCase(1404, 2025, 20)]
        [TestCase(1437, 2058, 20)]
        [TestCase(1532, 2153, 20)]
        [TestCase(1565, 2186, 20)]
        [TestCase(1569, 2190, 20)]
        [TestCase(1598, 2219, 21)]
        [TestCase(1631, 2252, 20)]
        [TestCase(1660, 2281, 20)]
        [TestCase(1664, 2285, 20)]
        [TestCase(1693, 2314, 21)]
        [TestCase(1697, 2318, 21)]
        [TestCase(1726, 2347, 21)]
        [TestCase(1730, 2351, 21)]
        [TestCase(1759, 2380, 20)]
        [TestCase(1763, 2384, 20)]
        [TestCase(1788, 2409, 20)]
        [TestCase(1792, 2413, 20)]
        [TestCase(1796, 2417, 20)]

        public void ArithmeticExamples(int persianYear, int gregorianYear, int gregorianDayOfMarch)
        {
            var persian = new LocalDate(persianYear, 1, 1, CalendarSystem.PersianArithmetic);
            var gregorian = persian.WithCalendar(CalendarSystem.Gregorian);
            Assert.AreEqual(gregorianYear, gregorian.Year);
            Assert.AreEqual(3, gregorian.Month);
            Assert.AreEqual(gregorianDayOfMarch, gregorian.Day);
        }

#if DEBUG && !PCL
        // Only really present to make it easy to regenerate the astronomical leap year data
        [Test]
        public void GenerateLeapYearData()
        {
            string data = PersianYearMonthDayCalculator.Astronomical.GenerateLeapYearData();
            Assert.IsNotEmpty(data);
        }
#endif
    }
}
