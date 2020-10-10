// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using System.Globalization;

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
    }
}