// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using NodaTime.Calendars;

namespace NodaTime.Demo
{
    public class YearMonthDemo
    {
        [Test]
        public void ConstructionWithExplicitYearAndMonth()
        {
            YearMonth yearMonth = Snippet.For(new YearMonth(2019, 5));
            Assert.AreEqual(2019, yearMonth.Year);
            Assert.AreEqual(5, yearMonth.Month);
            Assert.AreEqual(CalendarSystem.Iso, yearMonth.Calendar);
        }

        [Test]
        public void ConstructionFromEra()
        {
            YearMonth yearMonth = Snippet.For(new YearMonth(Era.Common, 1994, 5));
            Assert.AreEqual(1994, yearMonth.Year);
            Assert.AreEqual(5, yearMonth.Month);
            Assert.AreEqual(CalendarSystem.Iso, yearMonth.Calendar);
            Assert.AreEqual(Calendars.Era.Common, yearMonth.Era);
        }

        [Test]
        public void ConstructionWithExplicitCalendar()
        {
            YearMonth yearMonth = Snippet.For(new YearMonth(2014, 3, CalendarSystem.Julian));
            Assert.AreEqual(2014, yearMonth.Year);
            Assert.AreEqual(3, yearMonth.Month);
            Assert.AreEqual(CalendarSystem.Julian, yearMonth.Calendar);
        }

        [Test]
        public void ConstructionWithExplicitCalendar2()
        {
            YearMonth yearMonth = Snippet.For(new YearMonth(Era.Common, 2019, 5, CalendarSystem.Gregorian));
            Assert.AreEqual(2019, yearMonth.Year);
            Assert.AreEqual(5, yearMonth.Month);
            Assert.AreEqual(CalendarSystem.Gregorian, yearMonth.Calendar);
            Assert.AreEqual(Era.Common, yearMonth.Era);
        }

        [Test]
        public void ToDateInterval()
        {
            YearMonth yearMonth = new YearMonth(2019, 5);
            DateInterval interval = Snippet.For(yearMonth.ToDateInterval());
            Assert.AreEqual(new LocalDate(2019, 5, 1), interval.Start);
            Assert.AreEqual(new LocalDate(2019, 5, 31), interval.End);
        }
    }
}
