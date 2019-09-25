// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NodaTime.Test
{
    public partial class YearMonthTest
    {
        [Test]
        [TestCase(2000, 1)]
        [TestCase(2000, 12)]
        [TestCase(-9998, 1)]
        [TestCase(9999, 12)]
        public void ValidConstruction(int year, int month)
        {
            var yearMonth = new YearMonth(year, month);
            Assert.AreEqual(year, yearMonth.Year);
            Assert.AreEqual(month, yearMonth.Month);
            Assert.AreEqual(CalendarSystem.Iso, yearMonth.Calendar);
        }

        [Test]
        [TestCase(-9999, 1)]
        [TestCase(10000, 1)]
        [TestCase(2000, 0)]
        [TestCase(2000, 13)]
        public void InvalidConstruction(int year, int month)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new YearMonth(year, month));
        }

        [Test]
        [TestCase(2000, 1, (int) CalendarOrdinal.Iso)]
        [TestCase(2000, 12, (int) CalendarOrdinal.Iso)]
        [TestCase(-9998, 1, (int) CalendarOrdinal.Iso)]
        [TestCase(9999, 12, (int) CalendarOrdinal.Iso)]
        [TestCase(2000, 1, (int) CalendarOrdinal.Julian)]
        [TestCase(-9997, 1, (int) CalendarOrdinal.Julian)]
        [TestCase(9998, 12, (int) CalendarOrdinal.Julian)]
        [TestCase(5403, 1, (int) CalendarOrdinal.HebrewCivil)]
        [TestCase(5403, 12, (int) CalendarOrdinal.HebrewCivil)]
        // 5404 is a leap year
        [TestCase(5404, 1, (int) CalendarOrdinal.HebrewCivil)]
        [TestCase(5404, 13, (int) CalendarOrdinal.HebrewCivil)]
        public void ValidConstruction_WithCalendar(int year, int month, int ordinal)
        {
            CalendarSystem calendar = CalendarSystem.ForOrdinal((CalendarOrdinal) ordinal);
            var yearMonth = new YearMonth(year, month, calendar);
            Assert.AreEqual(year, yearMonth.Year);
            Assert.AreEqual(month, yearMonth.Month);
            Assert.AreEqual(calendar, yearMonth.Calendar);
        }

        [Test]
        [TestCase(-9999, 1, (int) CalendarOrdinal.Iso)]
        [TestCase(10000, 1, (int) CalendarOrdinal.Iso)]
        [TestCase(2000, 0, (int) CalendarOrdinal.Iso)]
        [TestCase(2000, 13, (int) CalendarOrdinal.Iso)]
        // 5403 is *not* a leap year
        [TestCase(5403, 13, (int) CalendarOrdinal.HebrewCivil)]
        [TestCase(5404, 14, (int) CalendarOrdinal.HebrewCivil)]
        // The Noda Time Julian calendar system runs from -9997 to 9998
        [TestCase(-9998, 12, (int) CalendarOrdinal.Julian)]
        [TestCase(9999, 11, (int) CalendarOrdinal.Julian)]
        public void InvalidConstruction_WithCalendar(int year, int month, int ordinal)
        {
            CalendarSystem calendar = CalendarSystem.ForOrdinal((CalendarOrdinal) ordinal);
            Assert.Throws<ArgumentOutOfRangeException>(() => new YearMonth(year, month, calendar));
        }

        [Test]
        [TestCase(100, 1, nameof(Era.BeforeCommon))]
        [TestCase(2000, 1, nameof(Era.Common))]
        public void ValidConstruction_WithIsoEra(int yearOfEra, int month, string eraName)
        {
            Era era = GetEra(eraName);
            var yearMonth = new YearMonth(era, yearOfEra, month);
            Assert.AreEqual(era, yearMonth.Era);
            Assert.AreEqual(yearOfEra, yearMonth.YearOfEra);
            Assert.AreEqual(month, yearMonth.Month);
            Assert.AreEqual(CalendarSystem.Iso, yearMonth.Calendar);
        }

        [Test]
        [TestCase(0, 1, nameof(Era.BeforeCommon))]
        [TestCase(10000, 1, nameof(Era.BeforeCommon))]
        [TestCase(10000, 1, nameof(Era.Common))]
        [TestCase(100, 13, nameof(Era.Common))]
        [TestCase(100, 1, nameof(Era.AnnoMundi))]
        public void InvalidConstruction_WithIsoEra(int yearOfEra, int month, string eraName)
        {
            Era era = GetEra(eraName);
            // We'll assume it throws the correct exact exception type. It varies by test case.
            Assert.That(() => new YearMonth(era, yearOfEra, month), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        [TestCase(100, 1, nameof(Era.BeforeCommon), (int) CalendarOrdinal.Iso)]
        [TestCase(2000, 1, nameof(Era.Common), (int) CalendarOrdinal.Iso)]
        [TestCase(100, 1, nameof(Era.BeforeCommon), (int) CalendarOrdinal.Julian)]
        [TestCase(2000, 1, nameof(Era.Common), (int) CalendarOrdinal.Julian)]
        [TestCase(5403, 1, nameof(Era.AnnoMundi), (int) CalendarOrdinal.HebrewCivil)]
        public void ValidConstruction_WithEraAndCalendar(int yearOfEra, int month, string eraName, int calendarOrdinal)
        {
            Era era = GetEra(eraName);
            CalendarSystem calendar = CalendarSystem.ForOrdinal((CalendarOrdinal) calendarOrdinal);
            var yearMonth = new YearMonth(era, yearOfEra, month, calendar);
            Assert.AreEqual(era, yearMonth.Era);
            Assert.AreEqual(yearOfEra, yearMonth.YearOfEra);
            Assert.AreEqual(month, yearMonth.Month);
            Assert.AreEqual(calendar, yearMonth.Calendar);
        }

        [Test]
        [TestCase(0, 1, nameof(Era.BeforeCommon), (int) CalendarOrdinal.Iso)]
        [TestCase(10000, 1, nameof(Era.BeforeCommon), (int) CalendarOrdinal.Iso)]
        [TestCase(10000, 1, nameof(Era.Common), (int) CalendarOrdinal.Iso)]
        [TestCase(100, 13, nameof(Era.Common), (int) CalendarOrdinal.Iso)]
        [TestCase(100, 1, nameof(Era.AnnoMundi), (int) CalendarOrdinal.Iso)]
        [TestCase(100, 1, nameof(Era.Common), (int) CalendarOrdinal.HebrewScriptural)]
        public void InvalidConstruction_WithEraAndCalendar(int yearOfEra, int month, string eraName, int calendarOrdinal)
        {
            Era era = GetEra(eraName);
            CalendarSystem calendar = CalendarSystem.ForOrdinal((CalendarOrdinal) calendarOrdinal);
            // We'll assume it throws the correct exact exception type. It varies by test case.
            Assert.That(() => new YearMonth(era, yearOfEra, month, calendar), Throws.InstanceOf<ArgumentException>());
        }

        // We may want to put this somewhere in the production code; I'm surprised we
        // haven't needed it before.
        private static Era GetEra(string eraName)
        {
            return (Era) typeof(Era).GetProperty(eraName, BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!;
        }
    }
}
