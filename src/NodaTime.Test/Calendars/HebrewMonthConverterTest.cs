// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    public class HebrewMonthConverterTest
    {
        private const int SampleLeapYear = 5502;
        private const int SampleNonLeapYear = 5501;

        [Test]
        [TestCase(1, 7)] // Nisan
        [TestCase(2, 8)] // Iyyar
        [TestCase(3, 9)] // Sivan
        [TestCase(4, 10)] // Tammuz
        [TestCase(5, 11)] // Av
        [TestCase(6, 12)] // Elul
        [TestCase(7, 1)] // Tishri
        [TestCase(8, 2)] // Heshvan
        [TestCase(9, 3)] // Kislev
        [TestCase(10, 4)] // Teveth
        [TestCase(11, 5)] // Shevat
        [TestCase(12, 6)] // Adar
        public void NonLeapYear(int scriptural, int civil)
        {
            Assert.AreEqual(scriptural, HebrewMonthConverter.CivilToScriptural(SampleNonLeapYear, civil));
            Assert.AreEqual(civil, HebrewMonthConverter.ScripturalToCivil(SampleNonLeapYear, scriptural));
        }

        [Test]
        [TestCase(1, 8)] // Nisan
        [TestCase(2, 9)] // Iyyar
        [TestCase(3, 10)] // Sivan
        [TestCase(4, 11)] // Tammuz
        [TestCase(5, 12)] // Av
        [TestCase(6, 13)] // Elul
        [TestCase(7, 1)] // Tishri
        [TestCase(8, 2)] // Heshvan
        [TestCase(9, 3)] // Kislev
        [TestCase(10, 4)] // Teveth
        [TestCase(11, 5)] // Shevat
        [TestCase(12, 6)] // Adar I
        [TestCase(13, 7)] // Adar II
        public void LeapYear(int scriptural, int civil)
        {
            Assert.AreEqual(scriptural, HebrewMonthConverter.CivilToScriptural(SampleLeapYear, civil));
            Assert.AreEqual(civil, HebrewMonthConverter.ScripturalToCivil(SampleLeapYear, scriptural));
        }
    }
}
