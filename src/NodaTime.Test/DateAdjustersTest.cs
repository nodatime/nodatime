// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NUnit.Framework;
using System;

namespace NodaTime.Test
{
    public class DateAdjustersTest
    {
        [Test]
        public void StartOfMonth()
        {
            var start = new LocalDate(2014, 6, 27);
            var end = new LocalDate(2014, 6, 1);
            Assert.AreEqual(end, DateAdjusters.StartOfMonth(start));
        }

        [Test]
        public void EndOfMonth()
        {
            var start = new LocalDate(2014, 6, 27);
            var end = new LocalDate(2014, 6, 30);
            Assert.AreEqual(end, DateAdjusters.EndOfMonth(start));
        }

        [Test]
        public void DayOfMonth()
        {
            var start = new LocalDate(2014, 6, 27);
            var end = new LocalDate(2014, 6, 19);
            var adjuster = DateAdjusters.DayOfMonth(19);
            Assert.AreEqual(end, adjuster(start));
        }

        [Test]
        [TestCase(2014, 8, 18, IsoDayOfWeek.Monday, 2014, 8, 18, Description = "Same day-of-week")]
        [TestCase(2014, 8, 18, IsoDayOfWeek.Tuesday, 2014, 8, 19)]
        [TestCase(2014, 8, 18, IsoDayOfWeek.Sunday, 2014, 8, 24)]
        [TestCase(2014, 8, 31, IsoDayOfWeek.Monday, 2014, 9, 1, Description = "Wrap month")]
        public void NextOrSame(
            int year, int month, int day, IsoDayOfWeek dayOfWeek,
            int expectedYear, int expectedMonth, int expectedDay)
        {
            LocalDate start = new LocalDate(year, month, day);
            LocalDate actual = start.With(DateAdjusters.NextOrSame(dayOfWeek));
            LocalDate expected = new LocalDate(expectedYear, expectedMonth, expectedDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(2014, 8, 18, IsoDayOfWeek.Monday, 2014, 8, 18, Description = "Same day-of-week")]
        [TestCase(2014, 8, 18, IsoDayOfWeek.Tuesday, 2014, 8, 12)]
        [TestCase(2014, 8, 18, IsoDayOfWeek.Sunday, 2014, 8, 17)]
        [TestCase(2014, 8, 1, IsoDayOfWeek.Thursday, 2014, 7, 31, Description = "Wrap month")]
        public void PreviousOrSame(
            int year, int month, int day, IsoDayOfWeek dayOfWeek,
            int expectedYear, int expectedMonth, int expectedDay)
        {
            LocalDate start = new LocalDate(year, month, day);
            LocalDate actual = start.With(DateAdjusters.PreviousOrSame(dayOfWeek));
            LocalDate expected = new LocalDate(expectedYear, expectedMonth, expectedDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(2014, 8, 18, IsoDayOfWeek.Monday, 2014, 8, 25, Description = "Same day-of-week")]
        [TestCase(2014, 8, 18, IsoDayOfWeek.Tuesday, 2014, 8, 19)]
        [TestCase(2014, 8, 18, IsoDayOfWeek.Sunday, 2014, 8, 24)]
        [TestCase(2014, 8, 31, IsoDayOfWeek.Monday, 2014, 9, 1, Description = "Wrap month")]
        public void Next(
            int year, int month, int day, IsoDayOfWeek dayOfWeek,
            int expectedYear, int expectedMonth, int expectedDay)
        {
            LocalDate start = new LocalDate(year, month, day);
            LocalDate actual = start.With(DateAdjusters.Next(dayOfWeek));
            LocalDate expected = new LocalDate(expectedYear, expectedMonth, expectedDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(2014, 8, 18, IsoDayOfWeek.Monday, 2014, 8, 11, Description = "Same day-of-week")]
        [TestCase(2014, 8, 18, IsoDayOfWeek.Tuesday, 2014, 8, 12)]
        [TestCase(2014, 8, 18, IsoDayOfWeek.Sunday, 2014, 8, 17)]
        [TestCase(2014, 8, 1, IsoDayOfWeek.Thursday, 2014, 7, 31, Description = "Wrap month")]
        public void Previous(
            int year, int month, int day, IsoDayOfWeek dayOfWeek,
            int expectedYear, int expectedMonth, int expectedDay)
        {
            LocalDate start = new LocalDate(year, month, day);
            LocalDate actual = start.With(DateAdjusters.Previous(dayOfWeek));
            LocalDate expected = new LocalDate(expectedYear, expectedMonth, expectedDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Month_Valid()
        {
            var adjuster = DateAdjusters.Month(2);
            var start = new LocalDate(2017, 8, 21, CalendarSystem.Julian);
            var actual = start.With(adjuster);
            var expected = new LocalDate(2017, 2, 21, CalendarSystem.Julian);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Month_InvalidAdjustment()
        {
            var adjuster = DateAdjusters.Month(2);
            var start = new LocalDate(2017, 8, 30, CalendarSystem.Julian);
            Assert.Throws<ArgumentOutOfRangeException>(() => start.With(adjuster));
        }

        [Test]
        public void IsoDayOfWeekAdjusters_Invalid()
        {
            var invalid = (IsoDayOfWeek) 10;
            Assert.Throws<ArgumentOutOfRangeException>(() => DateAdjusters.Next(invalid));
            Assert.Throws<ArgumentOutOfRangeException>(() => DateAdjusters.NextOrSame(invalid));
            Assert.Throws<ArgumentOutOfRangeException>(() => DateAdjusters.Previous(invalid));
            Assert.Throws<ArgumentOutOfRangeException>(() => DateAdjusters.PreviousOrSame(invalid));
        }
    }
}
