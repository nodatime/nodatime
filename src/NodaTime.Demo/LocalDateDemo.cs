// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using System.Globalization;
using NodaTime.Calendars;
using System;

namespace NodaTime.Demo
{
    public class LocalDateDemo
    {
        [Test]
        public void SimpleConstruction()
        {
            LocalDate date = Snippet.For(new LocalDate(2010, 6, 16));
            Assert.AreEqual("2010-06-16", date.ToString("uuuu-MM-dd", CultureInfo.InvariantCulture));
            Assert.AreEqual(2010, date.Year);
            Assert.AreEqual(6, date.Month);
            Assert.AreEqual(16, date.Day);
        }

        [Test]
        public void ExplicitCalendar()
        {
            LocalDate date = Snippet.For(new LocalDate(2010, 6, 16, CalendarSystem.Iso));
            Assert.AreEqual(new LocalDate(2010, 6, 16), date);
        }

        [Test]
        public void CombineWithTime()
        {
            LocalDate date = new LocalDate(2010, 6, 16);
            LocalTime time = new LocalTime(16, 20);
            LocalDateTime dateTime = Snippet.For(date + time);
            Assert.AreEqual(new LocalDateTime(2010, 6, 16, 16, 20, 0), dateTime);
        }

        [Test]
        public void Day()
        {
            LocalDate date = new LocalDate(2010, 6, 16);
            int result = Snippet.For(date.Day);
            Assert.AreEqual(16, result);
        }
        
        [Test]
        public void DayOfWeek()
        {
            LocalDate date = new LocalDate(2010, 6, 16);
            IsoDayOfWeek result = Snippet.For(date.DayOfWeek);
            Assert.AreEqual(IsoDayOfWeek.Wednesday, result);
        }

        [Test]
        public void DayOfYear()
        {
            LocalDate date = new LocalDate(2010, 6, 16);
            int result = Snippet.For(date.DayOfYear);
            Assert.AreEqual(167, result);
        }

        [Test]
        public void Add()
        {
            LocalDate date = new LocalDate(2010, 6, 16);
            LocalDate result = Snippet.For(LocalDate.Add(date, Period.FromDays(3)));
            Assert.AreEqual(new LocalDate(2010, 6, 19), result);
        }

        [Test]
        public void At()
        {
            LocalDate date = new LocalDate(2010, 6, 16);
            LocalTime time = new LocalTime(16, 20);
            LocalDateTime dateTime = Snippet.For(date.At(time));
            Assert.AreEqual(new LocalDateTime(2010, 6, 16, 16, 20, 0), dateTime);
        }

        [Test]
        public void AtMidnight()
        {
            LocalDate date = new LocalDate(2010, 6, 16);
            LocalDateTime dateTime = Snippet.For(date.AtMidnight());
            Assert.AreEqual(new LocalDateTime(2010, 6, 16, 0, 0, 0), dateTime);
        }

        [Test]
        public void CompareTo()
        {
            LocalDate date1 = new LocalDate(2010, 6, 16);
            LocalDate date2 = new LocalDate(2010, 6, 16);
            int result = Snippet.For(date1.CompareTo(date2));
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Equals()
        {
            LocalDate date1 = new LocalDate(2010, 6, 16);
            LocalDate date2 = new LocalDate(2010, 6, 16);
            bool result = Snippet.For(date1.Equals(date2));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void EqualsObject()
        {
            LocalDate date = new LocalDate(2010, 6, 16);
            object dateAsObject = new LocalDate(2010, 6, 16);
            bool result = Snippet.For(date.Equals(dateAsObject));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void Max()
        {
            LocalDate earlyJune = new LocalDate(2010, 6, 5);
            LocalDate lateJune = new LocalDate(2010, 6, 25);
            LocalDate max = Snippet.For(LocalDate.Max(earlyJune, lateJune));
            Assert.AreEqual(new LocalDate(2010, 6, 25), max);
        }

        [Test]
        public void Min()
        {
            LocalDate earlyJune = new LocalDate(2010, 6, 5);
            LocalDate lateJune = new LocalDate(2010, 6, 25);
            LocalDate min = Snippet.For(LocalDate.Min(earlyJune, lateJune));
            Assert.AreEqual(new LocalDate(2010, 6, 5), min);
        }

        [Test]
        public void FromDateTime()
        {
            DateTime earlyJune = new DateTime(2010, 6, 5);
            LocalDate date = Snippet.For(LocalDate.FromDateTime(earlyJune));
            Assert.AreEqual(new LocalDate(2010, 6, 5), date);
        }

        [Test]
        public void FromDateTimeWithCalendarSystem()
        {
            DateTime earlyJune = new DateTime(2010, 6, 5);
            CalendarSystem calendar = CalendarSystem.ForId("Julian");
            LocalDate date = Snippet.For(LocalDate.FromDateTime(earlyJune, calendar));
            // Between the years 2000 and 2099, the Julian calendar is 13 days behind the Gregorian calendar.
            Assert.AreEqual(2010, date.Year);
            Assert.AreEqual(5, date.Month);
            Assert.AreEqual(23, date.Day);
        }

        [Test]
        public void BeforeCommonEra()
        {
            LocalDate date = Snippet.For(new LocalDate(Era.BeforeCommon, 2010, 6, 16));
            Assert.AreEqual(new LocalDate(Era.BeforeCommon, 2010, 6, 16), date);
        }

        [Test]
        public void FromWeekYearWeekAndDay()
        {
            LocalDate date = Snippet.For(LocalDate.FromWeekYearWeekAndDay(2010, 24, IsoDayOfWeek.Wednesday));
            Assert.AreEqual(new LocalDate(2010, 6, 16), date);
        }

        [Test]
        public void FromYearMonthWeekAndDay()
        {
            LocalDate date = Snippet.For(LocalDate.FromYearMonthWeekAndDay(2010, 6, 3, IsoDayOfWeek.Wednesday));
            Assert.AreEqual(new LocalDate(2010, 6, 16), date);
        }

        [Test]
        public void Next()
        {
            LocalDate date = new LocalDate(2010, 6, 16);
            LocalDate result = Snippet.For(date.Next(IsoDayOfWeek.Thursday));
            Assert.AreEqual(new LocalDate(2010, 6, 17), result);
        }

        [Test]
        public void ToYearMonth()
        {
            YearMonth yearMonth = Snippet.For(new LocalDate(2010, 6, 16).ToYearMonth());
            Assert.AreEqual(new YearMonth(2010, 6), yearMonth);
        }

        [Test]
        public void Plus()
        {
            LocalDate date = Snippet.For(new LocalDate(2010, 1, 30).Plus(Period.FromMonths(1)));
            Assert.AreEqual(new LocalDate(2010, 2, 28), date);
        }

        [Test]
        public void Subtract()
        {
            LocalDate date = Snippet.For(LocalDate.Subtract(new LocalDate(2010, 2, 28), Period.FromMonths(1)));
            Assert.AreEqual(new LocalDate(2010, 1, 28), date);
        }

        [Test]
        public void Minus()
        {
            LocalDate date = Snippet.For(new LocalDate(2010, 6, 16).Minus(Period.FromDays(1)));
            Assert.AreEqual(new LocalDate(2010, 6, 15), date);
        }

    }
}
