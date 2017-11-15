// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Demo
{
    public class PeriodDemo
    {
        [Test]
        public void ConstructionFromYears()
        {
            Period period = Snippet.For(Period.FromYears(27));
            Assert.AreEqual(27, period.Years);
            Assert.AreEqual("P27Y", period.ToString());
        }

        [Test]
        public void ConstructionFromMonths()
        {
            Period period = Snippet.For(Period.FromMonths(10));
            Assert.AreEqual(10, period.Months);
            Assert.AreEqual("P10M", period.ToString());
        }

        [Test]
        public void ConstructionFromWeeks()
        {
            Period period = Snippet.For(Period.FromWeeks(1));
            Assert.AreEqual(1, period.Weeks);
            Assert.AreEqual("P1W", period.ToString());
        }

        [Test]
        public void ConstructionFromDays()
        {
            Period period = Snippet.For(Period.FromDays(3));
            Assert.AreEqual(3, period.Days);
            Assert.AreEqual("P3D", period.ToString());
        }

        [Test]
        public void ConstructionFromHours()
        {
            Period period = Snippet.For(Period.FromHours(5));
            Assert.AreEqual(5, period.Hours);
            Assert.AreEqual("PT5H", period.ToString());
        }

        [Test]
        public void ConstructionFromMinutes()
        {
            Period period = Snippet.For(Period.FromMinutes(15));
            Assert.AreEqual(15, period.Minutes);
            Assert.AreEqual("PT15M", period.ToString());
        }

        [Test]
        public void ConstructionFromSeconds()
        {
            Period period = Snippet.For(Period.FromSeconds(70));
            Assert.AreEqual(70, period.Seconds);
            Assert.AreEqual("PT70S", period.ToString());
        }

        [Test]
        public void ConstructionFromMilliseconds()
        {
            Period period = Snippet.For(Period.FromMilliseconds(1500));
            Assert.AreEqual(1500, period.Milliseconds);
            Assert.AreEqual("PT1500s", period.ToString());
        }

        [Test]
        public void ConstructionFromTicks()
        {
            Period period = Snippet.For(Period.FromTicks(42));
            Assert.AreEqual(42, period.Ticks);
            Assert.AreEqual("PT42t", period.ToString());
        }

        [Test]
        public void ConstructionFromNanoseconds()
        {
            Period period = Snippet.For(Period.FromNanoseconds(42));
            Assert.AreEqual(42, period.Nanoseconds);
            Assert.AreEqual("PT42n", period.ToString());
        }

        [Test]
        public void Between_LocalDatesDaysApart()
        {
            Period period = Snippet.For(Period.Between(
                new LocalDate(2017, 11, 10),
                new LocalDate(2017, 11, 15)));

            Assert.Multiple(() =>
            {
                Assert.AreEqual(5, period.Days);
                Assert.AreEqual("P5D", period.ToString());
            });
        }

        [Test]
        public void Between_LocalDatesYearsApart()
        {
            Period period = Snippet.For(Period.Between(
                new LocalDate(1990, 6, 26),
                new LocalDate(2017, 11, 15)));

            Assert.Multiple(() =>
            {
                Assert.AreEqual(27, period.Years);
                Assert.AreEqual(4, period.Months);
                Assert.AreEqual(20, period.Days);
                Assert.AreEqual("P27Y4M20D", period.ToString());
            });
        }

        [TestCase(PeriodUnits.Months, 5, 0, 0, "P5M")]
        [TestCase(PeriodUnits.Weeks, 0, 22, 0, "P22W")]
        [TestCase(PeriodUnits.Months | PeriodUnits.Weeks, 5, 1, 0, "P5M1W")]
        [TestCase(PeriodUnits.Months | PeriodUnits.Days, 5, 0, 7, "P5M7D")]
        [TestCase(PeriodUnits.Days, 0, 0, 160, "P160D")]
        public void Between_LocalDatesWithGivenUnits(PeriodUnits units, int expectedMonths, int expectedWeeks, int expectedDays, string expectedPattern)
        {
            Period period = Snippet.For(Period.Between(
                new LocalDate(2017, 6, 14),
                new LocalDate(2017, 11, 21),
                units));

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedMonths, period.Months);
                Assert.AreEqual(expectedWeeks, period.Weeks);
                Assert.AreEqual(expectedDays, period.Days);
                Assert.AreEqual(expectedPattern, period.ToString());
            });
        }

        [Test]
        public void Between_LocalTimes()
        {
            Period period = Snippet.For(Period.Between(new LocalTime(10, 10), new LocalTime(13, 15)));

            Assert.Multiple(() =>
            {
                Assert.AreEqual(3, period.Hours);
                Assert.AreEqual(5, period.Minutes);
                Assert.AreEqual("PT3H5M", period.ToString());
            });
        }

        [TestCase(PeriodUnits.Hours, 3, 0, 0, "PT3H")]
        [TestCase(PeriodUnits.Hours | PeriodUnits.Minutes, 3, 5, 0, "PT3H5M")]
        [TestCase(PeriodUnits.AllTimeUnits, 3, 5, 47, "PT3H5M47S")]
        [TestCase(PeriodUnits.Seconds, 0, 0, 11147, "PT11147S")]
        public void Between_LocalTimesWithGivenUnits(
            PeriodUnits units,
            int expectedHour,
            int expectedMinute,
            int expectedSecond,
            string expectedPeriodPattern)
        {
            Period period = Snippet.For(Period.Between(
                new LocalTime(10, 10, 2),
                new LocalTime(13, 15, 49),
                units));

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedHour, period.Hours);
                Assert.AreEqual(expectedMinute, period.Minutes);
                Assert.AreEqual(expectedSecond, period.Seconds);
                Assert.AreEqual(expectedPeriodPattern, period.ToString());
            });
        }

        [Test]
        public void Between_LocalDateTimes()
        {
            Period period = Snippet.For(Period.Between(
                new LocalDateTime(2015, 1, 23, 21, 30, 15),
                new LocalDateTime(2017, 10, 15, 21, 02, 17)));

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, period.Years);
                Assert.AreEqual(8, period.Months);
                Assert.AreEqual(21, period.Days);
                Assert.AreEqual(23, period.Hours);
                Assert.AreEqual(32, period.Minutes);
                Assert.AreEqual(2, period.Seconds);
                Assert.AreEqual("P2Y8M21DT23H32M2S", period.ToString());
            });
        }

        [TestCase(PeriodUnits.Years, 2, 0, 0, 0, 0, 0, "P2Y")]
        [TestCase(PeriodUnits.Months, 0, 32, 0, 0, 0, 0, "P32M")]
        [TestCase(PeriodUnits.Days, 0, 0, 995, 0, 0, 0, "P995D")]
        [TestCase(PeriodUnits.Years | PeriodUnits.Months | PeriodUnits.Days, 2, 8, 21, 0, 0, 0, "P2Y8M21D")]
        [TestCase(PeriodUnits.Hours | PeriodUnits.Minutes | PeriodUnits.Seconds, 0, 0, 0, 23903, 32, 2, "PT23903H32M2S")]
        public void Between_LocalDateTimesWithGivenUnits(
            PeriodUnits units,
            int expectedYears,
            int expectedMonths,
            int expectedDays,
            int expectedHours,
            int expectedMinutes,
            int expectedSeconds,
            string expectedPattern)
        {
            Period period = Snippet.For(Period.Between(
                new LocalDateTime(2015, 1, 23, 21, 30, 15),
                new LocalDateTime(2017, 10, 15, 21, 02, 17),
                units));

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedYears, period.Years);
                Assert.AreEqual(expectedMonths, period.Months);
                Assert.AreEqual(expectedDays, period.Days);
                Assert.AreEqual(expectedHours, period.Hours);
                Assert.AreEqual(expectedMinutes, period.Minutes);
                Assert.AreEqual(expectedSeconds, period.Seconds);
                Assert.AreEqual(expectedPattern, period.ToString());
            });
        }
    }
}