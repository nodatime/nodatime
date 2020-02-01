// Copyright 2020 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using System;

namespace NodaTime.Test
{
    // Tests for YearMonth which don't naturally fall into other categories
    public partial class YearMonthTest
    {
        [Test]
        [TestCase(CalendarOrdinal.Iso, 1904, 2, 29)]
        [TestCase(CalendarOrdinal.Julian, 1900, 2, 29)]
        [TestCase(CalendarOrdinal.HebrewCivil, 5402, 2, 30)]
        [TestCase(CalendarOrdinal.HebrewCivil, 5402, 3, 30)]
        [TestCase(CalendarOrdinal.HebrewScriptural, 5402, 8, 30)]
        [TestCase(CalendarOrdinal.HebrewScriptural, 5402, 9, 30)]
        public void OnDayOfMonth_Valid(int calendarOrdinal, int year, int month, int day)
        {
            var calendar = CalendarSystem.ForOrdinal((CalendarOrdinal) calendarOrdinal);
            var yearMonth = new YearMonth(year, month, calendar);
            var actualDate = yearMonth.OnDayOfMonth(day);
            var expectedDate = new LocalDate(year, month, day, calendar);
            Assert.AreEqual(expectedDate, actualDate);
        }

        [Test]
        [TestCase(CalendarOrdinal.Iso, 1900, 2, 29)]
        [TestCase(CalendarOrdinal.Julian, 1900, 2, 30)]
        [TestCase(CalendarOrdinal.HebrewCivil, 5401, 2, 30)]
        [TestCase(CalendarOrdinal.HebrewCivil, 5401, 3, 30)]
        [TestCase(CalendarOrdinal.HebrewScriptural, 5401, 8, 30)]
        [TestCase(CalendarOrdinal.HebrewScriptural, 5401, 9, 30)]
        public void OnDayOfMonth_Invalid(int calendarOrdinal, int year, int month, int day)
        {
            var calendar = CalendarSystem.ForOrdinal((CalendarOrdinal) calendarOrdinal);
            var yearMonth = new YearMonth(year, month, calendar);
            Assert.Throws<ArgumentOutOfRangeException>(() => yearMonth.OnDayOfMonth(day));
        }
    }
}