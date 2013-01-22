using System;
using NUnit.Framework;
using NodaTime.Calendars;

namespace NodaTime.Test
{
    public partial class LocalDateTest
    {
        [Test]
        public void Constructor_CalendarDefaultsToIso()
        {
            LocalDate date = new LocalDate(2000, 1, 1);
            Assert.AreEqual(CalendarSystem.Iso, date.Calendar);
        }

        [Test]
        public void Constructor_PropertiesRoundTrip()
        {
            LocalDate date = new LocalDate(2023, 7, 27);
            Assert.AreEqual(2023, date.Year);
            Assert.AreEqual(7, date.Month);
            Assert.AreEqual(27, date.Day);
        }

        [Test]
        public void Constructor_PropertiesRoundTrip_CustomCalendar()
        {
            LocalDate date = new LocalDate(2023, 7, 27, CalendarSystem.GetJulianCalendar(4));
            Assert.AreEqual(2023, date.Year);
            Assert.AreEqual(7, date.Month);
            Assert.AreEqual(27, date.Day);
        }

        [Test]
        public void Constructor_InvalidMonth()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(2010, 13, 1));
        }

        [Test]
        public void Constructor_InvalidDay()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(2010, 1, 100));
        }

        [Test]
        public void Constructor_InvalidDayWithinMonth()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(2010, 2, 30));
        }

        [Test]
        public void Constructor_InvalidYear()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(CalendarSystem.Iso.MaxYear + 1, 1, 1));
        }

        [Test]
        public void Constructor_InvalidYearOfEra()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(Era.Common, 0, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(Era.BeforeCommon, 0, 1, 1));
            // We go further in AD than in BC
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(Era.BeforeCommon, CalendarSystem.Iso.MaxYear, 1, 1));
        }

        [Test]
        public void Constructor_WithYearOfEra_BC()
        {
            LocalDate absolute = new LocalDate(-10, 1, 1);
            LocalDate withEra = new LocalDate(Era.BeforeCommon, 11, 1, 1);
            Assert.AreEqual(absolute, withEra);
        }

        [Test]
        public void Constructor_WithYearOfEra_AD()
        {
            LocalDate absolute = new LocalDate(50, 6, 19);
            LocalDate withEra = new LocalDate(Era.Common, 50, 6, 19);
            Assert.AreEqual(absolute, withEra);
        }

        [Test]
        public void Constructor_WithYearOfEra_NonIsoCalendar()
        {
            var calendar = CalendarSystem.GetCopticCalendar(4);
            LocalDate absolute = new LocalDate(50, 6, 19, calendar);
            LocalDate withEra = new LocalDate(Era.AnnoMartyrm, 50, 6, 19, calendar);
            Assert.AreEqual(absolute, withEra);
        }

        [Test]
        public void FromWeekYearWeekAndDay_BeforeEpoch()
        {
            // January 1st 1960 was a Friday. Therefore the first week of week year started
            // on Monday January 4th.
            var expected = new LocalDate(1960, 1, 19);
            var actual = LocalDate.FromWeekYearWeekAndDay(1960, 3, IsoDayOfWeek.Tuesday);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromWeekYearWeekAndDay_AfterEpoch()
        {
            // According to http://whatweekisit.com anyway...
            var expected = new LocalDate(2012, 10, 19);
            var actual = LocalDate.FromWeekYearWeekAndDay(2012, 42, IsoDayOfWeek.Friday);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromWeekYearWeekAndDay_EarlierWeekYearThanNormalYear()
        {
            // Saturday January 1st of 2011 is part of week 52 of week-year 2010.
            var expected = new LocalDate(2011, 1, 1);
            var actual = LocalDate.FromWeekYearWeekAndDay(2010, 52, IsoDayOfWeek.Saturday);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromWeekYearWeekAndDay_LaterWeekYearThanNormalYear()
        {
            // Monday December 31st of 2012 is part of week 1 of week-year 2013.
            var expected = new LocalDate(2012, 12, 31);
            var actual = LocalDate.FromWeekYearWeekAndDay(2013, 1, IsoDayOfWeek.Monday);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromWeekYearWeekAndDay_ValidWeek53()
        {
            // Sunday 2nd January 2005 is part of week 53 of week year 2004
            var expected = new LocalDate(2005, 1, 2);
            var actual = LocalDate.FromWeekYearWeekAndDay(2004, 53, IsoDayOfWeek.Sunday);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromWeekYearWeekAndDay_InvalidWeek53()
        {
            // Week year 2005 only has 52 weeks
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalDate.FromWeekYearWeekAndDay(2005, 53, IsoDayOfWeek.Sunday));
        }
    }
}
