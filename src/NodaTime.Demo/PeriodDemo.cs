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

            Assert.AreEqual(5, period.Days);
            Assert.AreEqual("P5D", period.ToString());
        }

        [Test]
        public void Between_LocalDatesYearsApart()
        {
            Period period = Snippet.For(Period.Between(
                new LocalDate(1990, 6, 26),
                new LocalDate(2017, 11, 15)));

            Assert.AreEqual(27, period.Years);
            Assert.AreEqual(4, period.Months);
            Assert.AreEqual(20, period.Days);
            Assert.AreEqual("P27Y4M20D", period.ToString());
        }

        [Test]
        public void Between_LocalDatesWithGivenUnits()
        {
            Period period = Snippet.For(Period.Between(
                new LocalDate(2016, 11, 14),
                new LocalDate(2017, 11, 21),
                PeriodUnits.Years | PeriodUnits.Days));

            Assert.AreEqual(1, period.Years);
            Assert.AreEqual(7, period.Days);
            Assert.AreEqual("P1Y7D", period.ToString());
        }

        [Test]
        public void Between_LocalTimes()
        {
            Period period = Snippet.For(Period.Between(new LocalTime(10, 10), new LocalTime(13, 15)));

            Assert.AreEqual(3, period.Hours);
            Assert.AreEqual(5, period.Minutes);
            Assert.AreEqual("PT3H5M", period.ToString());
        }

        [Test]
        public void Between_LocalTimesWithGivenUnits()
        {
            Period period = Snippet.For(Period.Between(
                new LocalTime(10, 10, 2),
                new LocalTime(13, 15, 49),
                PeriodUnits.Hours | PeriodUnits.Seconds));

            Assert.AreEqual(3, period.Hours);
            Assert.AreEqual(347, period.Seconds);
            Assert.AreEqual("PT3H347S", period.ToString());
        }

        [Test]
        public void Between_LocalDateTimes()
        {
            Period period = Snippet.For(Period.Between(
                new LocalDateTime(2015, 1, 23, 21, 30, 15),
                new LocalDateTime(2017, 10, 15, 21, 02, 17)));

            Assert.AreEqual(2, period.Years);
            Assert.AreEqual(8, period.Months);
            Assert.AreEqual(21, period.Days);
            Assert.AreEqual(23, period.Hours);
            Assert.AreEqual(32, period.Minutes);
            Assert.AreEqual(2, period.Seconds);
            Assert.AreEqual("P2Y8M21DT23H32M2S", period.ToString());
        }

        [Test]
        public void Between_LocalDateTimesWithGivenUnits()
        {
            Period period = Snippet.For(Period.Between(
                new LocalDateTime(2015, 1, 23, 21, 30, 15),
                new LocalDateTime(2017, 10, 15, 21, 02, 17),
                PeriodUnits.Years | PeriodUnits.Days | PeriodUnits.Hours));

            Assert.AreEqual(2, period.Years);
            Assert.AreEqual(264, period.Days);
            Assert.AreEqual(23, period.Hours);
            Assert.AreEqual("P2Y264DT23H", period.ToString());
        }
    }
}