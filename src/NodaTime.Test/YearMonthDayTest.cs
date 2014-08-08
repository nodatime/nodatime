// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class YearMonthDayTest
    {
        [Test]
        public void AllYears()
        {
            // Range of years we actually care about. We support more, but that's okay.
            for (int year = -99999; year <= 99999; year++)
            {
                var ymd = new YearMonthDay(year, 5, 20);
                Assert.AreEqual(year, ymd.Year);
                Assert.AreEqual(5, ymd.Month);
                Assert.AreEqual(20, ymd.Day);
            }
        }

        [Test]
        public void AllMonths()
        {
            // We'll never actually need 16 months, but we support that many...
            for (int month = 1; month < 16; month++)
            {
                var ymd = new YearMonthDay(-123, month, 20);
                Assert.AreEqual(-123, ymd.Year);
                Assert.AreEqual(month, ymd.Month);
                Assert.AreEqual(20, ymd.Day);
            }
        }

        [Test]
        public void AllDays()
        {
            // We'll never actually need 64 days, but we support that many...
            for (int day = 1; day < 64; day++)
            {
                var ymd = new YearMonthDay(-123, 12, day);
                Assert.AreEqual(-123, ymd.Year);
                Assert.AreEqual(12, ymd.Month);
                Assert.AreEqual(day, ymd.Day);
            }
        }

        [Test]
        [TestCase("1000-01-01", "1000-01-02")]
        [TestCase("1000-01-01", "1000-02-01")]
        [TestCase("999-16-64", "1000-01-01")]
        [TestCase("-1-01-01", "-1-01-02")]
        [TestCase("-1-01-01", "-1-02-01")]
        [TestCase("-2-16-64", "-1-01-01")]
        [TestCase("-1-16-64", "0-01-01")]
        [TestCase("-1-16-64", "1-01-01")]
        public void Comparisons(string smallerText, string greaterText)
        {
            var smaller = YearMonthDay.Parse(smallerText);
            var greater = YearMonthDay.Parse(greaterText);
            TestHelper.TestCompareToStruct(smaller, smaller, greater);
            TestHelper.TestOperatorComparisonEquality(smaller, smaller, greater);
        }
    }
}
