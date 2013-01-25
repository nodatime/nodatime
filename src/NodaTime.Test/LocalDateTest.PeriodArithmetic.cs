// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalDateTest
    {
        [Test]
        public void Addition_WithPeriod()
        {
            LocalDate start = new LocalDate(2010, 6, 19);
            Period period = Period.FromMonths(3) + Period.FromDays(10);
            LocalDate expected = new LocalDate(2010, 9, 29);
            Assert.AreEqual(expected, start + period);
        }

        [Test]
        public void Addition_TruncatesOnShortMonth()
        {
            LocalDate start = new LocalDate(2010, 1, 30);
            Period period = Period.FromMonths(1);
            LocalDate expected = new LocalDate(2010, 2, 28);
            Assert.AreEqual(expected, start + period);
        }

        [Test]
        public void Addition_WithNullPeriod_ThrowsArgumentNullException()
        {
            LocalDate date = new LocalDate(2010, 1, 1);
            // Call to ToString just to make it a valid statement
            Assert.Throws<ArgumentNullException>(() => (date + (Period)null).ToString());
        }

        [Test]
        public void Subtraction_WithPeriod()
        {
            LocalDate start = new LocalDate(2010, 9, 29);
            Period period = Period.FromMonths(3) + Period.FromDays(10);
            LocalDate expected = new LocalDate(2010, 6, 19);
            Assert.AreEqual(expected, start - period);
        }

        [Test]
        public void Subtraction_TruncatesOnShortMonth()
        {
            LocalDate start = new LocalDate(2010, 3, 30);
            Period period = Period.FromMonths(1);
            LocalDate expected = new LocalDate(2010, 2, 28);
            Assert.AreEqual(expected, start - period);
        }

        [Test]
        public void Subtraction_WithNullPeriod_ThrowsArgumentNullException()
        {
            LocalDate date = new LocalDate(2010, 1, 1);
            // Call to ToString just to make it a valid statement
            Assert.Throws<ArgumentNullException>(() => (date - (Period)null).ToString());
        }

        [Test]
        public void Addition_PeriodWithTime()
        {
            LocalDate date = new LocalDate(2010, 1, 1);
            Period period = Period.FromHours(1);
            // Use method not operator here to form a valid statement
            Assert.Throws<ArgumentException>(() => LocalDate.Add(date, period));
        }

        [Test]
        public void Subtraction_PeriodWithTime()
        {
            LocalDate date = new LocalDate(2010, 1, 1);
            Period period = Period.FromHours(1);
            // Use method not operator here to form a valid statement
            Assert.Throws<ArgumentException>(() => LocalDate.Subtract(date, period));
        }

        [Test]
        public void PeriodAddition_MethodEquivalents()
        {
            LocalDate start = new LocalDate(2010, 6, 19);
            Period period = Period.FromMonths(3) + Period.FromDays(10);
            Assert.AreEqual(start + period, LocalDate.Add(start, period));
            Assert.AreEqual(start + period, start.Plus(period));
        }

        [Test]
        public void PeriodSubtraction_MethodEquivalents()
        {
            LocalDate start = new LocalDate(2010, 6, 19);
            Period period = Period.FromMonths(3) + Period.FromDays(10);
            Assert.AreEqual(start - period, LocalDate.Subtract(start, period));
            Assert.AreEqual(start - period, start.Minus(period));
        }
    }
}
