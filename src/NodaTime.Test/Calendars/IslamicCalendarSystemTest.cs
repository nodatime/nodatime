// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    [TestFixture]
    public class IslamicCalendarSystemTest
    {
        private static readonly CalendarSystem SampleCalendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Civil);

        [Test]
        public void SampleDate1()
        {
            // Note: field checks removed from the tests.
            LocalDateTime ldt = new LocalDateTime(1945, 11, 12, 0, 0, 0, 0, CalendarSystem.Iso);

            ldt = ldt.WithCalendar(SampleCalendar);
            Assert.AreEqual(Era.AnnoHegirae, ldt.Era);
            Assert.AreEqual(1364, ldt.YearOfEra);

            Assert.AreEqual(1364, ldt.Year);
            Assert.AreEqual(12, ldt.Month);
            Assert.AreEqual(6, ldt.Day);
            Assert.AreEqual(IsoDayOfWeek.Monday, ldt.IsoDayOfWeek);
            Assert.AreEqual(6 * 30 + 5 * 29 + 6, ldt.DayOfYear);

            Assert.AreEqual(0, ldt.Hour);
            Assert.AreEqual(0, ldt.Minute);
            Assert.AreEqual(0, ldt.Second);
            Assert.AreEqual(0, ldt.TickOfSecond);
        }

        [Test]
        public void SampleDate2()
        {
            LocalDateTime ldt = new LocalDateTime(2005, 11, 26, 0, 0, 0, 0, CalendarSystem.Iso);
            ldt = ldt.WithCalendar(SampleCalendar);
            Assert.AreEqual(Era.AnnoHegirae, ldt.Era);
            Assert.AreEqual(1426, ldt.YearOfEra);

            Assert.AreEqual(1426, ldt.Year);
            Assert.AreEqual(10, ldt.Month);
            Assert.AreEqual(24, ldt.Day);
            Assert.AreEqual(IsoDayOfWeek.Saturday, ldt.IsoDayOfWeek);
            Assert.AreEqual(5 * 30 + 4 * 29 + 24, ldt.DayOfYear);
            Assert.AreEqual(0, ldt.Hour);
            Assert.AreEqual(0, ldt.Minute);
            Assert.AreEqual(0, ldt.Second);
            Assert.AreEqual(0, ldt.TickOfSecond);
        }

        [Test]
        public void SampleDate3()
        {
            LocalDateTime ldt = new LocalDateTime(1426, 12, 24, 0, 0, 0, 0, SampleCalendar);
            Assert.AreEqual(Era.AnnoHegirae, ldt.Era);

            Assert.AreEqual(1426, ldt.Year);
            Assert.AreEqual(12, ldt.Month);
            Assert.AreEqual(24, ldt.Day);
            Assert.AreEqual(IsoDayOfWeek.Tuesday, ldt.IsoDayOfWeek);
            Assert.AreEqual(6 * 30 + 5 * 29 + 24, ldt.DayOfYear);
            Assert.AreEqual(0, ldt.Hour);
            Assert.AreEqual(0, ldt.Minute);
            Assert.AreEqual(0, ldt.Second);
            Assert.AreEqual(0, ldt.TickOfSecond);
        }

        [Test]
        public void InternalConsistency()
        {
            var calendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Civil);
            // Check construction and then deconstruction for every day of every year in one 30-year cycle.
            for (int year = 1; year <= 30; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    int monthLength = calendar.GetDaysInMonth(year, month);
                    for (int day = 1; day < monthLength; day++)
                    {
                        LocalDate date = new LocalDate(year, month, day, calendar);
                        Assert.AreEqual(year, date.Year, "Year of {0}-{1}-{2}", year, month, day);
                        Assert.AreEqual(month, date.Month, "Month of {0}-{1}-{2}", year, month, day);
                        Assert.AreEqual(day, date.Day, "Day of {0}-{1}-{2}", year, month, day);
                    }
                }
            }
        }

        [Test]
        public void Base15LeapYear()
        {
            CalendarSystem calendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Civil);

            Assert.AreEqual(false, calendar.IsLeapYear(1));
            Assert.AreEqual(true, calendar.IsLeapYear(2));
            Assert.AreEqual(false, calendar.IsLeapYear(3));
            Assert.AreEqual(false, calendar.IsLeapYear(4));
            Assert.AreEqual(true, calendar.IsLeapYear(5));
            Assert.AreEqual(false, calendar.IsLeapYear(6));
            Assert.AreEqual(true, calendar.IsLeapYear(7));
            Assert.AreEqual(false, calendar.IsLeapYear(8));
            Assert.AreEqual(false, calendar.IsLeapYear(9));
            Assert.AreEqual(true, calendar.IsLeapYear(10));
            Assert.AreEqual(false, calendar.IsLeapYear(11));
            Assert.AreEqual(false, calendar.IsLeapYear(12));
            Assert.AreEqual(true, calendar.IsLeapYear(13));
            Assert.AreEqual(false, calendar.IsLeapYear(14));
            Assert.AreEqual(true, calendar.IsLeapYear(15));
            Assert.AreEqual(false, calendar.IsLeapYear(16));
            Assert.AreEqual(false, calendar.IsLeapYear(17));
            Assert.AreEqual(true, calendar.IsLeapYear(18));
            Assert.AreEqual(false, calendar.IsLeapYear(19));
            Assert.AreEqual(false, calendar.IsLeapYear(20));
            Assert.AreEqual(true, calendar.IsLeapYear(21));
            Assert.AreEqual(false, calendar.IsLeapYear(22));
            Assert.AreEqual(false, calendar.IsLeapYear(23));
            Assert.AreEqual(true, calendar.IsLeapYear(24));
            Assert.AreEqual(false, calendar.IsLeapYear(25));
            Assert.AreEqual(true, calendar.IsLeapYear(26));
            Assert.AreEqual(false, calendar.IsLeapYear(27));
            Assert.AreEqual(false, calendar.IsLeapYear(28));
            Assert.AreEqual(true, calendar.IsLeapYear(29));
            Assert.AreEqual(false, calendar.IsLeapYear(30));
        }

        [Test]
        public void Base16LeapYear()
        {
            CalendarSystem calendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Civil);

            Assert.AreEqual(false, calendar.IsLeapYear(1));
            Assert.AreEqual(true, calendar.IsLeapYear(2));
            Assert.AreEqual(false, calendar.IsLeapYear(3));
            Assert.AreEqual(false, calendar.IsLeapYear(4));
            Assert.AreEqual(true, calendar.IsLeapYear(5));
            Assert.AreEqual(false, calendar.IsLeapYear(6));
            Assert.AreEqual(true, calendar.IsLeapYear(7));
            Assert.AreEqual(false, calendar.IsLeapYear(8));
            Assert.AreEqual(false, calendar.IsLeapYear(9));
            Assert.AreEqual(true, calendar.IsLeapYear(10));
            Assert.AreEqual(false, calendar.IsLeapYear(11));
            Assert.AreEqual(false, calendar.IsLeapYear(12));
            Assert.AreEqual(true, calendar.IsLeapYear(13));
            Assert.AreEqual(false, calendar.IsLeapYear(14));
            Assert.AreEqual(false, calendar.IsLeapYear(15));
            Assert.AreEqual(true, calendar.IsLeapYear(16));
            Assert.AreEqual(false, calendar.IsLeapYear(17));
            Assert.AreEqual(true, calendar.IsLeapYear(18));
            Assert.AreEqual(false, calendar.IsLeapYear(19));
            Assert.AreEqual(false, calendar.IsLeapYear(20));
            Assert.AreEqual(true, calendar.IsLeapYear(21));
            Assert.AreEqual(false, calendar.IsLeapYear(22));
            Assert.AreEqual(false, calendar.IsLeapYear(23));
            Assert.AreEqual(true, calendar.IsLeapYear(24));
            Assert.AreEqual(false, calendar.IsLeapYear(25));
            Assert.AreEqual(true, calendar.IsLeapYear(26));
            Assert.AreEqual(false, calendar.IsLeapYear(27));
            Assert.AreEqual(false, calendar.IsLeapYear(28));
            Assert.AreEqual(true, calendar.IsLeapYear(29));
            Assert.AreEqual(false, calendar.IsLeapYear(30));
        }

        [Test]
        public void IndianBasedLeapYear()
        {
            CalendarSystem calendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Indian, IslamicEpoch.Civil);

            Assert.AreEqual(false, calendar.IsLeapYear(1));
            Assert.AreEqual(true, calendar.IsLeapYear(2));
            Assert.AreEqual(false, calendar.IsLeapYear(3));
            Assert.AreEqual(false, calendar.IsLeapYear(4));
            Assert.AreEqual(true, calendar.IsLeapYear(5));
            Assert.AreEqual(false, calendar.IsLeapYear(6));
            Assert.AreEqual(false, calendar.IsLeapYear(7));
            Assert.AreEqual(true, calendar.IsLeapYear(8));
            Assert.AreEqual(false, calendar.IsLeapYear(9));
            Assert.AreEqual(true, calendar.IsLeapYear(10));
            Assert.AreEqual(false, calendar.IsLeapYear(11));
            Assert.AreEqual(false, calendar.IsLeapYear(12));
            Assert.AreEqual(true, calendar.IsLeapYear(13));
            Assert.AreEqual(false, calendar.IsLeapYear(14));
            Assert.AreEqual(false, calendar.IsLeapYear(15));
            Assert.AreEqual(true, calendar.IsLeapYear(16));
            Assert.AreEqual(false, calendar.IsLeapYear(17));
            Assert.AreEqual(false, calendar.IsLeapYear(18));
            Assert.AreEqual(true, calendar.IsLeapYear(19));
            Assert.AreEqual(false, calendar.IsLeapYear(20));
            Assert.AreEqual(true, calendar.IsLeapYear(21));
            Assert.AreEqual(false, calendar.IsLeapYear(22));
            Assert.AreEqual(false, calendar.IsLeapYear(23));
            Assert.AreEqual(true, calendar.IsLeapYear(24));
            Assert.AreEqual(false, calendar.IsLeapYear(25));
            Assert.AreEqual(false, calendar.IsLeapYear(26));
            Assert.AreEqual(true, calendar.IsLeapYear(27));
            Assert.AreEqual(false, calendar.IsLeapYear(28));
            Assert.AreEqual(true, calendar.IsLeapYear(29));
            Assert.AreEqual(false, calendar.IsLeapYear(30));
        }

        [Test]
        public void HabashAlHasibBasedLeapYear()
        {
            CalendarSystem calendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Civil);

            Assert.AreEqual(false, calendar.IsLeapYear(1));
            Assert.AreEqual(true, calendar.IsLeapYear(2));
            Assert.AreEqual(false, calendar.IsLeapYear(3));
            Assert.AreEqual(false, calendar.IsLeapYear(4));
            Assert.AreEqual(true, calendar.IsLeapYear(5));
            Assert.AreEqual(false, calendar.IsLeapYear(6));
            Assert.AreEqual(false, calendar.IsLeapYear(7));
            Assert.AreEqual(true, calendar.IsLeapYear(8));
            Assert.AreEqual(false, calendar.IsLeapYear(9));
            Assert.AreEqual(false, calendar.IsLeapYear(10));
            Assert.AreEqual(true, calendar.IsLeapYear(11));
            Assert.AreEqual(false, calendar.IsLeapYear(12));
            Assert.AreEqual(true, calendar.IsLeapYear(13));
            Assert.AreEqual(false, calendar.IsLeapYear(14));
            Assert.AreEqual(false, calendar.IsLeapYear(15));
            Assert.AreEqual(true, calendar.IsLeapYear(16));
            Assert.AreEqual(false, calendar.IsLeapYear(17));
            Assert.AreEqual(false, calendar.IsLeapYear(18));
            Assert.AreEqual(true, calendar.IsLeapYear(19));
            Assert.AreEqual(false, calendar.IsLeapYear(20));
            Assert.AreEqual(true, calendar.IsLeapYear(21));
            Assert.AreEqual(false, calendar.IsLeapYear(22));
            Assert.AreEqual(false, calendar.IsLeapYear(23));
            Assert.AreEqual(true, calendar.IsLeapYear(24));
            Assert.AreEqual(false, calendar.IsLeapYear(25));
            Assert.AreEqual(false, calendar.IsLeapYear(26));
            Assert.AreEqual(true, calendar.IsLeapYear(27));
            Assert.AreEqual(false, calendar.IsLeapYear(28));
            Assert.AreEqual(false, calendar.IsLeapYear(29));
            Assert.AreEqual(true, calendar.IsLeapYear(30));
        }

        [Test]
        public void ThursdayEpoch()
        {
            CalendarSystem thursdayEpochCalendar = CommonCalendars.BclIslamic;
            CalendarSystem julianCalendar = CommonCalendars.Julian;

            LocalDate thursdayEpoch = new LocalDate(1, 1, 1, thursdayEpochCalendar);
            LocalDate thursdayEpochJulian = new LocalDate(622, 7, 15, julianCalendar);
            Assert.AreEqual(thursdayEpochJulian, thursdayEpoch.WithCalendar(julianCalendar));
        }

        [Test]
        public void FridayEpoch()
        {
            CalendarSystem fridayEpochCalendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Civil);
            CalendarSystem julianCalendar = CommonCalendars.Julian;

            LocalDate fridayEpoch = new LocalDate(1, 1, 1, fridayEpochCalendar);
            LocalDate fridayEpochJulian = new LocalDate(622, 7, 16, julianCalendar);
            Assert.AreEqual(fridayEpochJulian, fridayEpoch.WithCalendar(julianCalendar));
        }

        [Test]
        public void BclUsesAstronomicalEpoch()
        {
            Calendar hijri = new HijriCalendar();
            DateTime bclDirect = new DateTime(1, 1, 1, 0, 0, 0, 0, hijri, DateTimeKind.Unspecified);

            CalendarSystem julianCalendar = CommonCalendars.Julian;
            LocalDate julianIslamicEpoch = new LocalDate(622, 7, 15, julianCalendar);
            LocalDate isoIslamicEpoch = julianIslamicEpoch.WithCalendar(CalendarSystem.Iso);
            DateTime bclFromNoda = isoIslamicEpoch.AtMidnight().ToDateTimeUnspecified();
            Assert.AreEqual(bclDirect, bclFromNoda);
        }

        [Test]
        public void SampleDateBclCompatibility()
        {
            Calendar hijri = new HijriCalendar();
            DateTime bclDirect = new DateTime(1302, 10, 15, 0, 0, 0, 0, hijri, DateTimeKind.Unspecified);

            CalendarSystem islamicCalendar = CommonCalendars.BclIslamic;
            LocalDate iso = new LocalDate(1302, 10, 15, islamicCalendar);
            DateTime bclFromNoda = iso.AtMidnight().ToDateTimeUnspecified();
            Assert.AreEqual(bclDirect, bclFromNoda);
        }

        /// <summary>
        /// This tests every day for 9000 (ISO) years, to check that it always matches the year, month and day.
        /// </summary>
        [Test, Timeout(180000)] // Can take a long time under NCrunch.
        public void BclThroughHistory()
        {
            Calendar hijri = new HijriCalendar();
            DateTime bclDirect = new DateTime(1, 1, 1, 0, 0, 0, 0, hijri, DateTimeKind.Unspecified);

            CalendarSystem islamicCalendar = CommonCalendars.BclIslamic;
            CalendarSystem julianCalendar = CommonCalendars.Julian;
            LocalDate julianIslamicEpoch = new LocalDate(622, 7, 15, julianCalendar);
            LocalDate islamicDate = julianIslamicEpoch.WithCalendar(islamicCalendar);

            for (int i = 0; i < 9000 * 365; i++)
            {
                Assert.AreEqual(bclDirect, islamicDate.AtMidnight().ToDateTimeUnspecified());
                Assert.AreEqual(hijri.GetYear(bclDirect), islamicDate.Year, i.ToString());
                Assert.AreEqual(hijri.GetMonth(bclDirect), islamicDate.Month);
                Assert.AreEqual(hijri.GetDayOfMonth(bclDirect), islamicDate.Day);
                bclDirect = hijri.AddDays(bclDirect, 1);
                islamicDate = islamicDate.PlusDays(1);
            }
        }

        [Test]
        public void GetDaysInMonth()
        {
            // Just check that we've got the long/short the right way round...
            CalendarSystem calendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.HabashAlHasib, IslamicEpoch.Civil);
            Assert.AreEqual(30, calendar.GetDaysInMonth(7, 1));
            Assert.AreEqual(29, calendar.GetDaysInMonth(7, 2));
            Assert.AreEqual(30, calendar.GetDaysInMonth(7, 3));
            Assert.AreEqual(29, calendar.GetDaysInMonth(7, 4));
            Assert.AreEqual(30, calendar.GetDaysInMonth(7, 5));
            Assert.AreEqual(29, calendar.GetDaysInMonth(7, 6));
            Assert.AreEqual(30, calendar.GetDaysInMonth(7, 7));
            Assert.AreEqual(29, calendar.GetDaysInMonth(7, 8));
            Assert.AreEqual(30, calendar.GetDaysInMonth(7, 9));
            Assert.AreEqual(29, calendar.GetDaysInMonth(7, 10));
            Assert.AreEqual(30, calendar.GetDaysInMonth(7, 11));
            // As noted before, 7 isn't a leap year in this calendar
            Assert.AreEqual(29, calendar.GetDaysInMonth(7, 12));
            // As noted before, 8 is a leap year in this calendar
            Assert.AreEqual(30, calendar.GetDaysInMonth(8, 12));
        }

        [Test]
        public void GetInstance_Caching()
        {
            var queue = new Queue<CalendarSystem>();
            var set = new HashSet<CalendarSystem>();
            var ids = new HashSet<string>();

            foreach (IslamicLeapYearPattern leapYearPattern in Enum.GetValues(typeof(IslamicLeapYearPattern)))
            {
                foreach (IslamicEpoch epoch in Enum.GetValues(typeof(IslamicEpoch)))
                {
                    var calendar = CalendarSystem.GetIslamicCalendar(leapYearPattern, epoch);
                    queue.Enqueue(calendar);
                    Assert.IsTrue(set.Add(calendar)); // Check we haven't already seen it...
                    Assert.IsTrue(ids.Add(calendar.Id));
                }
            }

            // Now check we get the same references again...
            foreach (IslamicLeapYearPattern leapYearPattern in Enum.GetValues(typeof(IslamicLeapYearPattern)))
            {
                foreach (IslamicEpoch epoch in Enum.GetValues(typeof(IslamicEpoch)))
                {
                    var oldCalendar = queue.Dequeue();
                    var newCalendar = CalendarSystem.GetIslamicCalendar(leapYearPattern, epoch);
                    Assert.AreSame(oldCalendar, newCalendar);
                }
            }
        }

        [Test]
        public void GetInstance_ArgumentValidation()
        {
            var epochs = Enum.GetValues(typeof(IslamicEpoch)).Cast<IslamicEpoch>();
            var leapYearPatterns = Enum.GetValues(typeof(IslamicLeapYearPattern)).Cast<IslamicLeapYearPattern>();
            Assert.Throws<ArgumentOutOfRangeException>(() => CalendarSystem.GetIslamicCalendar(leapYearPatterns.Min() - 1, epochs.Min()));
            Assert.Throws<ArgumentOutOfRangeException>(() => CalendarSystem.GetIslamicCalendar(leapYearPatterns.Min(), epochs.Min() - 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => CalendarSystem.GetIslamicCalendar(leapYearPatterns.Max() + 1, epochs.Min()));
            Assert.Throws<ArgumentOutOfRangeException>(() => CalendarSystem.GetIslamicCalendar(leapYearPatterns.Min(), epochs.Max() + 1));
        }

        [Test]
        public void PlusYears_Simple()
        {
            var calendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Civil);
            LocalDateTime start = new LocalDateTime(5, 8, 20, 2, 0, calendar);
            LocalDateTime expectedEnd = new LocalDateTime(10, 8, 20, 2, 0, calendar);
            Assert.AreEqual(expectedEnd, start.PlusYears(5));
        }

        [Test]
        public void PlusYears_TruncatesAtLeapYear()
        {
            var calendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Civil);
            Assert.IsTrue(calendar.IsLeapYear(2));
            Assert.IsFalse(calendar.IsLeapYear(3));

            LocalDateTime start = new LocalDateTime(2, 12, 30, 2, 0, calendar);
            LocalDateTime expectedEnd = new LocalDateTime(3, 12, 29, 2, 0, calendar);

            Assert.AreEqual(expectedEnd, start.PlusYears(1));
        }

        [Test]
        public void PlusYears_DoesNotTruncateFromOneLeapYearToAnother()
        {
            var calendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Civil);
            Assert.IsTrue(calendar.IsLeapYear(2));
            Assert.IsTrue(calendar.IsLeapYear(5));

            LocalDateTime start = new LocalDateTime(2, 12, 30, 2, 0, calendar);
            LocalDateTime expectedEnd = new LocalDateTime(5, 12, 30, 2, 0, calendar);

            Assert.AreEqual(expectedEnd, start.PlusYears(3));
        }

        [Test]
        public void PlusMonths_Simple()
        {
            var calendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Civil);
            Assert.IsTrue(calendar.IsLeapYear(2));

            LocalDateTime start = new LocalDateTime(2, 12, 30, 2, 0, calendar);
            LocalDateTime expectedEnd = new LocalDateTime(3, 11, 30, 2, 0, calendar);
            Assert.AreEqual(11, expectedEnd.Month);
            Assert.AreEqual(30, expectedEnd.Day);
            Assert.AreEqual(expectedEnd, start.PlusMonths(11));
        }
    }
}
