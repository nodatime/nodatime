#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Globalization;
using NUnit.Framework;
using NodaTime.Calendars;
using System;

namespace NodaTime.Test.Calendars
{
    [TestFixture]
    public class IslamicCalendarTest
    {
        private static readonly CalendarSystem SampleCalendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Civil);

        [Test]
        public void SampleDate1()
        {
            // Note: field checks removed from the tests.
            LocalDateTime ldt = new LocalDateTime(1945, 11, 12, 0, 0, 0, 0, CalendarSystem.Iso);

            ldt = ldt.WithCalendar(SampleCalendar);
            Assert.AreEqual(Era.AnnoHegirae, ldt.Era);
            Assert.AreEqual(14, ldt.CenturyOfEra);  // TODO confirm
            Assert.AreEqual(64, ldt.YearOfCentury);
            Assert.AreEqual(1364, ldt.YearOfEra);

            Assert.AreEqual(1364, ldt.Year);
            Assert.AreEqual(12, ldt.MonthOfYear);
            Assert.AreEqual(6, ldt.DayOfMonth);
            Assert.AreEqual(IsoDayOfWeek.Monday, ldt.IsoDayOfWeek);
            Assert.AreEqual(6 * 30 + 5 * 29 + 6, ldt.DayOfYear);

            Assert.AreEqual(0, ldt.HourOfDay);
            Assert.AreEqual(0, ldt.MinuteOfHour);
            Assert.AreEqual(0, ldt.SecondOfMinute);
            Assert.AreEqual(0, ldt.TickOfSecond);
        }

        [Test]
        public void SampleDate2()
        {
            LocalDateTime ldt = new LocalDateTime(2005, 11, 26, 0, 0, 0, 0, CalendarSystem.Iso);
            ldt = ldt.WithCalendar(SampleCalendar);
            Assert.AreEqual(Era.AnnoHegirae, ldt.Era);
            Assert.AreEqual(15, ldt.CenturyOfEra);  // TODO confirm
            Assert.AreEqual(26, ldt.YearOfCentury);
            Assert.AreEqual(1426, ldt.YearOfEra);

            Assert.AreEqual(1426, ldt.Year);
            Assert.AreEqual(10, ldt.MonthOfYear);
            Assert.AreEqual(24, ldt.DayOfMonth);
            Assert.AreEqual(IsoDayOfWeek.Saturday, ldt.IsoDayOfWeek);
            Assert.AreEqual(5 * 30 + 4 * 29 + 24, ldt.DayOfYear);
            Assert.AreEqual(0, ldt.HourOfDay);
            Assert.AreEqual(0, ldt.MinuteOfHour);
            Assert.AreEqual(0, ldt.SecondOfMinute);
            Assert.AreEqual(0, ldt.TickOfSecond);
        }

        [Test]
        public void SampleDate3()
        {
            LocalDateTime ldt = new LocalDateTime(1426, 12, 24, 0, 0, 0, 0, SampleCalendar);
            Assert.AreEqual(Era.AnnoHegirae, ldt.Era);

            Assert.AreEqual(1426, ldt.Year);
            Assert.AreEqual(12, ldt.MonthOfYear);
            Assert.AreEqual(24, ldt.DayOfMonth);
            Assert.AreEqual(IsoDayOfWeek.Tuesday, ldt.IsoDayOfWeek);
            Assert.AreEqual(6 * 30 + 5 * 29 + 24, ldt.DayOfYear);
            Assert.AreEqual(0, ldt.HourOfDay);
            Assert.AreEqual(0, ldt.MinuteOfHour);
            Assert.AreEqual(0, ldt.SecondOfMinute);
            Assert.AreEqual(0, ldt.TickOfSecond);
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
            CalendarSystem thursdayEpochCalendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Astronomical);
            CalendarSystem julianCalendar = CalendarSystem.GetJulianCalendar(4);

            LocalDate thursdayEpoch = new LocalDate(1, 1, 1, thursdayEpochCalendar);
            LocalDate thursdayEpochJulian = new LocalDate(622, 7, 15, julianCalendar);
            Assert.AreEqual(thursdayEpochJulian, thursdayEpoch.WithCalendar(julianCalendar));
        }

        [Test]
        public void FridayEpoch()
        {
            CalendarSystem fridayEpochCalendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Civil);
            CalendarSystem julianCalendar = CalendarSystem.GetJulianCalendar(4);

            LocalDate fridayEpoch = new LocalDate(1, 1, 1, fridayEpochCalendar);
            LocalDate fridayEpochJulian = new LocalDate(622, 7, 16, julianCalendar);
            Assert.AreEqual(fridayEpochJulian, fridayEpoch.WithCalendar(julianCalendar));
        }

        [Test]
        public void BclUsesAstronomicalEpoch()
        {
            Calendar hijri = new HijriCalendar();
            DateTime bclDirect = new DateTime(1, 1, 1, 0, 0, 0, 0, hijri, DateTimeKind.Unspecified);

            CalendarSystem julianCalendar = CalendarSystem.GetJulianCalendar(4);
            LocalDate julianIslamicEpoch = new LocalDate(622, 7, 15, julianCalendar);
            LocalDate isoIslamicEpoch = julianIslamicEpoch.WithCalendar(CalendarSystem.Iso);
            DateTime bclFromNoda = isoIslamicEpoch.LocalDateTime.ToDateTimeUnspecified();
            Assert.AreEqual(bclDirect, bclFromNoda);
        }

        [Test]
        public void SampleDateBclCompatibility()
        {
            Calendar hijri = new HijriCalendar();
            DateTime bclDirect = new DateTime(1302, 10, 15, 0, 0, 0, 0, hijri, DateTimeKind.Unspecified);

            CalendarSystem islamicCalendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Astronomical);
            LocalDate iso = new LocalDate(1302, 10, 15, islamicCalendar);
            DateTime bclFromNoda = iso.LocalDateTime.ToDateTimeUnspecified();
            Assert.AreEqual(bclDirect, bclFromNoda);
        }

        /// <summary>
        /// This tests every day for 9000 (ISO) years, to check that it always matches the year, month and day.
        /// </summary>
        [Test, Ignore("Takes a long time")]
        public void BclThroughHistory()
        {
            Calendar hijri = new HijriCalendar();
            DateTime bclDirect = new DateTime(1, 1, 1, 0, 0, 0, 0, hijri, DateTimeKind.Unspecified);

            CalendarSystem islamicCalendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base16, IslamicEpoch.Astronomical);
            CalendarSystem julianCalendar = CalendarSystem.GetJulianCalendar(4);
            LocalDate julianIslamicEpoch = new LocalDate(622, 7, 15, julianCalendar);
            LocalDate islamicDate = julianIslamicEpoch.WithCalendar(islamicCalendar);

            for (int i = 0; i < 9000 * 365; i++)
            {
                Assert.AreEqual(bclDirect, islamicDate.LocalDateTime.ToDateTimeUnspecified());
                Assert.AreEqual(hijri.GetYear(bclDirect), islamicDate.Year, i.ToString());
                Assert.AreEqual(hijri.GetMonth(bclDirect), islamicDate.MonthOfYear);
                Assert.AreEqual(hijri.GetDayOfMonth(bclDirect), islamicDate.DayOfMonth);
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
    }
}
