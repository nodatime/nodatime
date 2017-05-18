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
            Assert.AreEqual("2010-06-16", date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
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
        public void At()
        {
            LocalDate date = new LocalDate(2010, 6, 16);
            LocalTime time = new LocalTime(16, 20);
            LocalDateTime dateTime = Snippet.For(date.At(time));
            Assert.AreEqual(new LocalDateTime(2010, 6, 16, 16, 20, 0), dateTime);
        }
    }
}