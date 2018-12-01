// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NodaTime.Utility;
using NUnit.Framework;
using System;

namespace NodaTime.Test.Calendars
{
    /// <summary>
    /// Tests for the Badíʿ calendar system.
    /// </summary>
    public class BadiCalendarSystemTest
    {
        // For use with CreateBadíʿDate, this is a notional "month"
        // containing Ayyam-i-Ha. The days here are represented in month
        // 18 in LocalDate etc.
        const int AyyamiHaMonth = 0;

        [Test]
        public void BadiEpoch()
        {
            CalendarSystem badi = CalendarSystem.Badi;
            LocalDate badiEpoch = CreateBadiDate(1, 1, 1);

            CalendarSystem gregorian = CalendarSystem.Gregorian;
            LocalDate converted = badiEpoch.WithCalendar(gregorian);

            LocalDate expected = new LocalDate(1844, 3, 21);

            Assert.AreEqual(expected.ToString(), converted.ToString());
        }

        [Test]
        public void UnixEpoch()
        {
            CalendarSystem badi = CalendarSystem.Badi;
            LocalDate unixEpochInBadiCalendar = NodaConstants.UnixEpoch.InZone(DateTimeZone.Utc, badi).LocalDateTime.Date;
            LocalDate expected = CreateBadiDate(126, 16, 2);
            Assert.AreEqual(expected, unixEpochInBadiCalendar);
        }

        [Test]
        public void SampleDate()
        {
            CalendarSystem badiCalendar = CalendarSystem.Badi;
            LocalDate iso = new LocalDate(2017, 3, 4);
            LocalDate badi = iso.WithCalendar(badiCalendar);

            Assert.AreEqual(19, BadiMonth(badi));

            Assert.AreEqual(Era.Bahai, badi.Era);
            Assert.AreEqual(173, badi.YearOfEra);

            Assert.AreEqual(173, badi.Year);
            Assert.IsFalse(badiCalendar.IsLeapYear(173));

            Assert.AreEqual(4, BadiDay(badi));

            Assert.AreEqual(IsoDayOfWeek.Saturday, badi.DayOfWeek);
        }

        [Test]
        [TestCase(2016, 2, 26, 172, AyyamiHaMonth, 1)]
        [TestCase(2016, 2, 29, 172, AyyamiHaMonth, 4)]
        [TestCase(2016, 3, 1, 172, 19, 1)]
        [TestCase(2016, 3, 20, 173, 1, 1)]
        [TestCase(2016, 3, 20, 173, 1, 1)]
        [TestCase(2016, 3, 21, 173, 1, 2)]
        [TestCase(2016, 5, 26, 173, 4, 11)]
        [TestCase(2017, 3, 20, 174, 1, 1)]
        [TestCase(2018, 3, 21, 175, 1, 1)]
        [TestCase(2019, 3, 21, 176, 1, 1)]
        [TestCase(2020, 3, 20, 177, 1, 1)]
        [TestCase(2021, 3, 20, 178, 1, 1)]
        [TestCase(2022, 3, 21, 179, 1, 1)]
        [TestCase(2023, 3, 21, 180, 1, 1)]
        [TestCase(2024, 3, 20, 181, 1, 1)]
        [TestCase(2025, 3, 20, 182, 1, 1)]
        [TestCase(2026, 3, 21, 183, 1, 1)]
        [TestCase(2027, 3, 21, 184, 1, 1)]
        [TestCase(2028, 3, 20, 185, 1, 1)]
        [TestCase(2029, 3, 20, 186, 1, 1)]
        [TestCase(2030, 3, 20, 187, 1, 1)]
        [TestCase(2031, 3, 21, 188, 1, 1)]
        [TestCase(2032, 3, 20, 189, 1, 1)]
        [TestCase(2033, 3, 20, 190, 1, 1)]
        [TestCase(2034, 3, 20, 191, 1, 1)]
        [TestCase(2035, 3, 21, 192, 1, 1)]
        [TestCase(2036, 3, 20, 193, 1, 1)]
        [TestCase(2037, 3, 20, 194, 1, 1)]
        [TestCase(2038, 3, 20, 195, 1, 1)]
        [TestCase(2039, 3, 21, 196, 1, 1)]
        [TestCase(2040, 3, 20, 197, 1, 1)]
        [TestCase(2041, 3, 20, 198, 1, 1)]
        [TestCase(2042, 3, 20, 199, 1, 1)]
        [TestCase(2043, 3, 21, 200, 1, 1)]
        [TestCase(2044, 3, 20, 201, 1, 1)]
        [TestCase(2045, 3, 20, 202, 1, 1)]
        [TestCase(2046, 3, 20, 203, 1, 1)]
        [TestCase(2047, 3, 21, 204, 1, 1)]
        [TestCase(2048, 3, 20, 205, 1, 1)]
        [TestCase(2049, 3, 20, 206, 1, 1)]
        [TestCase(2050, 3, 20, 207, 1, 1)]
        [TestCase(2051, 3, 21, 208, 1, 1)]
        [TestCase(2052, 3, 20, 209, 1, 1)]
        [TestCase(2053, 3, 20, 210, 1, 1)]
        [TestCase(2054, 3, 20, 211, 1, 1)]
        [TestCase(2055, 3, 21, 212, 1, 1)]
        [TestCase(2056, 3, 20, 213, 1, 1)]
        [TestCase(2057, 3, 20, 214, 1, 1)]
        [TestCase(2058, 3, 20, 215, 1, 1)]
        [TestCase(2059, 3, 20, 216, 1, 1)]
        [TestCase(2060, 3, 20, 217, 1, 1)]
        [TestCase(2061, 3, 20, 218, 1, 1)]
        [TestCase(2062, 3, 20, 219, 1, 1)]
        [TestCase(2063, 3, 20, 220, 1, 1)]
        [TestCase(2064, 3, 20, 221, 1, 1)]
        public void GeneralConversionNearNawRuz(int gYear, int gMonth, int gDay, int bYear, int bMonth, int bDay)
        {
            // create in the Badíʿ calendar
            var bDate = CreateBadiDate(bYear, bMonth, bDay);
            var gDate = bDate.WithCalendar(CalendarSystem.Gregorian);
            Assert.AreEqual(gYear, gDate.Year);
            Assert.AreEqual(gMonth, gDate.Month);
            Assert.AreEqual(gDay, gDate.Day);

            // convert to the Badíʿ calendar
            var bDate2 = new LocalDate(gYear, gMonth, gDay).WithCalendar(CalendarSystem.Badi);
            Assert.AreEqual(bYear, bDate2.Year);
            Assert.AreEqual(bMonth, BadiMonth(bDate2));
            Assert.AreEqual(bDay, BadiDay(bDate2));
        }

        [Test]
        [TestCase(2012, 2, 29, 168, AyyamiHaMonth, 4)]
        [TestCase(2012, 3, 1, 168, AyyamiHaMonth, 5)]
        [TestCase(2015, 3, 1, 171, AyyamiHaMonth, 4)]
        [TestCase(2015, 3, 1, 171, AyyamiHaMonth, 4)]
        [TestCase(2016, 3, 1, 172, 19, 1)]
        [TestCase(2016, 3, 19, 172, 19, 19)]
        [TestCase(2017, 3, 1, 173, 19, 1)]
        [TestCase(2017, 3, 19, 173, 19, 19)]
        [TestCase(2018, 2, 24, 174, 18, 19)]
        [TestCase(2018, 2, 25, 174, AyyamiHaMonth, 1)]
        [TestCase(2018, 3, 1, 174, AyyamiHaMonth, 5)]
        [TestCase(2018, 3, 2, 174, 19, 1)]
        [TestCase(2018, 3, 19, 174, 19, 18)]
        public void SpecialCases(int gYear, int gMonth, int gDay, int bYear, int bMonth, int bDay)
        {
            // create in test calendar
            var bDate = CreateBadiDate(bYear, bMonth, bDay);

            // convert to gregorian
            var gDate = bDate.WithCalendar(CalendarSystem.Gregorian);

            Assert.AreEqual($"{gYear}-{gMonth}-{gDay}", $"{gDate.Year}-{gDate.Month}-{gDate.Day}");

            // create in gregorian
            // convert to test calendar
            var gDate2 = new LocalDate(gYear, gMonth, gDay);
            var bDate2 = gDate2.WithCalendar(CalendarSystem.Badi);

            Assert.AreEqual($"{bYear}-{bMonth}-{bDay}", $"{bDate2.Year}-{BadiMonth(bDate2)}-{BadiDay(bDate2)}");
        }

        [Test]
        [TestCase(1, 1, 1, 1844, 3, 21)]
        [TestCase(169, 1, 1, 2012, 3, 21)]
        [TestCase(170, 1, 1, 2013, 3, 21)]
        [TestCase(171, 1, 1, 2014, 3, 21)]
        [TestCase(171, 1, 1, 2014, 3, 21)]
        [TestCase(172, AyyamiHaMonth, 1, 2016, 2, 26)]
        [TestCase(172, AyyamiHaMonth, 2, 2016, 2, 27)]
        [TestCase(172, AyyamiHaMonth, 3, 2016, 2, 28)]
        [TestCase(172, AyyamiHaMonth, 4, 2016, 2, 29)]
        [TestCase(172, 1, 1, 2015, 3, 21)]
        [TestCase(172, 1, 1, 2015, 3, 21)]
        [TestCase(172, 17, 18, 2016, 2, 5)]
        [TestCase(172, 18, 17, 2016, 2, 23)]
        [TestCase(172, 18, 19, 2016, 2, 25)]
        [TestCase(172, 19, 1, 2016, 3, 1)]
        [TestCase(173, 1, 1, 2016, 3, 20)]
        [TestCase(173, 1, 1, 2016, 3, 20)]
        [TestCase(174, 1, 1, 2017, 3, 20)]
        [TestCase(175, 1, 1, 2018, 3, 21)]
        [TestCase(176, 1, 1, 2019, 3, 21)]
        [TestCase(177, 1, 1, 2020, 3, 20)]
        [TestCase(178, 1, 1, 2021, 3, 20)]
        [TestCase(179, 1, 1, 2022, 3, 21)]
        [TestCase(180, 1, 1, 2023, 3, 21)]
        [TestCase(181, 1, 1, 2024, 3, 20)]
        [TestCase(182, 1, 1, 2025, 3, 20)]
        [TestCase(183, 1, 1, 2026, 3, 21)]
        [TestCase(184, 1, 1, 2027, 3, 21)]
        [TestCase(185, 1, 1, 2028, 3, 20)]
        [TestCase(186, 1, 1, 2029, 3, 20)]
        [TestCase(187, 1, 1, 2030, 3, 20)]
        [TestCase(188, 1, 1, 2031, 3, 21)]
        [TestCase(189, 1, 1, 2032, 3, 20)]
        [TestCase(190, 1, 1, 2033, 3, 20)]
        [TestCase(191, 1, 1, 2034, 3, 20)]
        [TestCase(192, 1, 1, 2035, 3, 21)]
        [TestCase(193, 1, 1, 2036, 3, 20)]
        [TestCase(194, 1, 1, 2037, 3, 20)]
        [TestCase(195, 1, 1, 2038, 3, 20)]
        [TestCase(196, 1, 1, 2039, 3, 21)]
        [TestCase(197, 1, 1, 2040, 3, 20)]
        [TestCase(198, 1, 1, 2041, 3, 20)]
        [TestCase(199, 1, 1, 2042, 3, 20)]
        [TestCase(200, 1, 1, 2043, 3, 21)]
        [TestCase(201, 1, 1, 2044, 3, 20)]
        [TestCase(202, 1, 1, 2045, 3, 20)]
        [TestCase(203, 1, 1, 2046, 3, 20)]
        [TestCase(204, 1, 1, 2047, 3, 21)]
        [TestCase(205, 1, 1, 2048, 3, 20)]
        [TestCase(206, 1, 1, 2049, 3, 20)]
        [TestCase(207, 1, 1, 2050, 3, 20)]
        [TestCase(208, 1, 1, 2051, 3, 21)]
        [TestCase(209, 1, 1, 2052, 3, 20)]
        [TestCase(210, 1, 1, 2053, 3, 20)]
        [TestCase(211, 1, 1, 2054, 3, 20)]
        [TestCase(212, 1, 1, 2055, 3, 21)]
        [TestCase(213, 1, 1, 2056, 3, 20)]
        [TestCase(214, 1, 1, 2057, 3, 20)]
        [TestCase(215, 1, 1, 2058, 3, 20)]
        [TestCase(216, 1, 1, 2059, 3, 20)]
        [TestCase(217, 1, 1, 2060, 3, 20)]
        [TestCase(218, 1, 1, 2061, 3, 20)]
        [TestCase(219, 1, 1, 2062, 3, 20)]
        [TestCase(220, 1, 1, 2063, 3, 20)]
        [TestCase(221, 1, 1, 2064, 3, 20)]
        public void GeneralWtoG(int bYear, int bMonth, int bDay, int gYear, int gMonth, int gDay)
        {
            // create in this calendar
            var bDate = CreateBadiDate(bYear, bMonth, bDay);
            var gDate = bDate.WithCalendar(CalendarSystem.Gregorian);
            Assert.AreEqual(gYear, gDate.Year);
            Assert.AreEqual(gMonth, gDate.Month);
            Assert.AreEqual(gDay, gDate.Day);

            // convert to this calendar
            var bDate2 = new LocalDate(gYear, gMonth, gDay).WithCalendar(CalendarSystem.Badi);
            Assert.AreEqual(bYear, bDate2.Year);
            Assert.AreEqual(bMonth, BadiMonth(bDate2));
            Assert.AreEqual(bDay, BadiDay(bDate2));
        }

        [Test]
        [TestCase(172, 4)]
        [TestCase(173, 4)]
        [TestCase(174, 5)]
        [TestCase(175, 4)]
        [TestCase(176, 4)]
        [TestCase(177, 4)]
        [TestCase(178, 5)]
        [TestCase(179, 4)]
        [TestCase(180, 4)]
        [TestCase(181, 4)]
        [TestCase(182, 5)]
        [TestCase(183, 4)]
        [TestCase(184, 4)]
        [TestCase(185, 4)]
        [TestCase(186, 4)]
        [TestCase(187, 5)]
        [TestCase(188, 4)]
        [TestCase(189, 4)]
        [TestCase(190, 4)]
        [TestCase(191, 5)]
        [TestCase(192, 4)]
        [TestCase(193, 4)]
        [TestCase(194, 4)]
        [TestCase(195, 5)]
        [TestCase(196, 4)]
        [TestCase(197, 4)]
        [TestCase(198, 4)]
        [TestCase(199, 5)]
        [TestCase(200, 4)]
        [TestCase(201, 4)]
        [TestCase(202, 4)]
        [TestCase(203, 5)]
        [TestCase(204, 4)]
        [TestCase(205, 4)]
        [TestCase(206, 4)]
        [TestCase(207, 5)]
        [TestCase(208, 4)]
        [TestCase(209, 4)]
        [TestCase(210, 4)]
        [TestCase(211, 5)]
        [TestCase(212, 4)]
        [TestCase(213, 4)]
        [TestCase(214, 4)]
        [TestCase(215, 4)]
        [TestCase(216, 5)]
        [TestCase(217, 4)]
        [TestCase(218, 4)]
        [TestCase(219, 4)]
        [TestCase(220, 5)]
        [TestCase(221, 4)]
        public void DaysInAyyamiHa(int bYear, int days)
        {
            Assert.AreEqual(days, BadiYearMonthDayCalculator.GetDaysInAyyamiHa(bYear));
        }

        [Test]
        [TestCase(165, 1, 1, 1)]
        [TestCase(170, 1, 1, 1)]
        [TestCase(172, 1, 1, 1)]
        [TestCase(175, 1, 1, 1)]
        [TestCase(173, 18, 1, 17 * 19 + 1)]
        [TestCase(173, 18, 19, 18 * 19)]
        [TestCase(173, AyyamiHaMonth, 1, 18 * 19 + 1)]
        [TestCase(173, 19, 1, 18 * 19 + 5)]
        [TestCase(220, AyyamiHaMonth, 1, 18 * 19 + 1)]
        [TestCase(220, AyyamiHaMonth, 5, 18 * 19 + 5)]
        [TestCase(220, 19, 1, 18 * 19 + 6)]
        public void DayOfYear(int bYear, int bMonth, int bDay, int dayOfYear)
        {
            var badi = new BadiYearMonthDayCalculator();
            Assert.AreEqual(dayOfYear, badi.GetDayOfYear(CreateBadiDate(bYear, bMonth, bDay).YearMonthDay));
        }

        // Cannot use EndOfMonth with Ayyam-i-Ha because they are internally stored as days in month 18.
        // In Ayyam-i-Ha, EndOfMonth should throw an exception or return the last day of Ayyam-i-Ha.
        // In this implementation, it will always return the last day of the month 18.
        [Test]
        [TestCase(173, 1, 1, 1, 19)]
        [TestCase(173, 18, 1, AyyamiHaMonth, 4)]
        [TestCase(173, AyyamiHaMonth, 1, AyyamiHaMonth, 4)]
        [TestCase(173, 19, 1, 19, 19)]
        [TestCase(220, 19, 1, 19, 19)]
        [TestCase(220, 4, 5, 4, 19)]
        [TestCase(220, 18, 1, AyyamiHaMonth, 5)]
        [TestCase(220, AyyamiHaMonth, 1, AyyamiHaMonth, 5)]
        public void EndOfMonth(int year, int month, int day, int eomMonth, int eomDay)
        {
            var start = CreateBadiDate(year, month, day);
            var end = CreateBadiDate(year, eomMonth, eomDay);
            Assert.AreEqual(AsBadiString(end), AsBadiString(DateAdjusters.EndOfMonth(start)));
        }

        [Test]
        public void LeapYear()
        {
            var calendar = CalendarSystem.Badi;
            Assert.IsFalse(calendar.IsLeapYear(172));
            Assert.IsFalse(calendar.IsLeapYear(173));
            Assert.IsTrue(calendar.IsLeapYear(207));
            Assert.IsTrue(calendar.IsLeapYear(220));
        }

        [Test]
        public void GetMonthsInYear()
        {
            var calendar = CalendarSystem.Badi;
            Assert.AreEqual(19, calendar.GetMonthsInYear(180));
        }

        [Test]
        [TestCase(180, 1, 19)]
        [TestCase(180, 18, 23)]
        public void GetDaysInMonth(int year, int month, int expectedDays)
        {
            var calendar = CalendarSystem.Badi;
            Assert.AreEqual(expectedDays, calendar.GetDaysInMonth(year, month));
        }

        [Test]
        public void CreateDate_InAyyamiHa()
        {
            var d1 = CreateBadiDate(180, 0, 3);
            var d3 = CreateBadiDate(180, 18, 22);

            Assert.AreEqual(d1, d3);
        }

        [Test]
        [TestCase(180, -1, 1)]
        [TestCase(180, 1, -1)]
        [TestCase(180, 0, 0)]
        [TestCase(180, 0, 5)]
        [TestCase(182, 0, 6)]
        [TestCase(180, 1, 0)]
        [TestCase(180, 1, 20)]
        [TestCase(180, 20, 1)]
        public void CreateDate_Invalid(int year, int month, int day)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateBadiDate(year, month, day));
        }

        // Period related tests
        private static readonly LocalDate TestDate1_167_5_15 = CreateBadiDate(167, 5, 15);
        private static readonly LocalDate TestDate1_167_6_7 = CreateBadiDate(167, 6, 7);
        private static readonly LocalDate TestDate2_167_Ayyam_4 = CreateBadiDate(167, AyyamiHaMonth, 4);
        private static readonly LocalDate TestDate3_168_Ayyam_5 = CreateBadiDate(168, AyyamiHaMonth, 5);

        [Test]
        public void BetweenLocalDates_InvalidUnits()
        {
            Assert.Throws<ArgumentException>(() => Period.Between(TestDate1_167_5_15, TestDate2_167_Ayyam_4, 0));
            Assert.Throws<ArgumentException>(() => Period.Between(TestDate1_167_5_15, TestDate2_167_Ayyam_4, (PeriodUnits)(-1)));
            Assert.Throws<ArgumentException>(() => Period.Between(TestDate1_167_5_15, TestDate2_167_Ayyam_4, PeriodUnits.AllTimeUnits));
            Assert.Throws<ArgumentException>(() => Period.Between(TestDate1_167_5_15, TestDate2_167_Ayyam_4, PeriodUnits.Years | PeriodUnits.Hours));
        }

        [Test]
        public void SetYear()
        {
            // crafted to test SetYear with 0 
            var d1 = CreateBadiDate(180, 1, 1);
            LocalDate result = d1 + Period.FromYears(0);
            Assert.AreEqual(180, result.Year);
        }

        [Test]
        public void BetweenLocalDates_MovingForwardNoLeapYears_WithExactResults()
        {
            Period actual = Period.Between(TestDate1_167_5_15, TestDate1_167_6_7);
            Period expected = Period.FromDays(11);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingForwardNoLeapYears_WithExactResults_2()
        {
            Period actual = Period.Between(TestDate1_167_5_15, TestDate2_167_Ayyam_4);
            Period expected = Period.FromMonths(13) + Period.FromDays(8);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingForwardInLeapYear_WithExactResults()
        {
            Period actual = Period.Between(TestDate1_167_5_15, TestDate3_168_Ayyam_5);
            Period expected = Period.FromYears(1) + Period.FromMonths(13) + Period.FromDays(9);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingBackwardNoLeapYears_WithExactResults()
        {
            Period actual = Period.Between(TestDate2_167_Ayyam_4, TestDate1_167_5_15);
            Period expected = Period.FromMonths(-13) + Period.FromDays(-8);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingBackward_WithExactResults()
        {
            // should be -1y -13m -9d
            // but system first moves back a year, and in that year, the last day of Ayyam-i-Ha is day 4
            // from there, it is -13m -8d

            Period expected = Period.FromYears(-1) + Period.FromMonths(-13) + Period.FromDays(-8);
            Period actual = Period.Between(TestDate3_168_Ayyam_5, TestDate1_167_5_15);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingForward_WithJustMonths()
        {
            Period actual = Period.Between(TestDate1_167_5_15, TestDate3_168_Ayyam_5, PeriodUnits.Months);
            Period expected = Period.FromMonths(32);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_MovingBackward_WithJustMonths()
        {
            Period actual = Period.Between(TestDate3_168_Ayyam_5, TestDate1_167_5_15, PeriodUnits.Months);
            Period expected = Period.FromMonths(-32);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BetweenLocalDates_AsymmetricForwardAndBackward()
        {
            LocalDate d1 = CreateBadiDate(166, 18, 4);
            LocalDate d2 = CreateBadiDate(167, 1, 10);

            // spanning Ayyam-i-Ha - not counted as a month
            Assert.AreEqual(Period.FromMonths(2) + Period.FromDays(6), Period.Between(d1, d2));
            Assert.AreEqual(Period.FromMonths(-2) + Period.FromDays(-6), Period.Between(d2, d1));
        }

        [Test]
        public void BetweenLocalDates_EndOfMonth()
        {
            LocalDate d1 = CreateBadiDate(171, 5, 19);
            LocalDate d2 = CreateBadiDate(171, 6, 19);
            Assert.AreEqual(Period.FromMonths(1), Period.Between(d1, d2));
            Assert.AreEqual(Period.FromMonths(-1), Period.Between(d2, d1));
        }

        [Test]
        public void BetweenLocalDates_OnLeapYear()
        {
            LocalDate d1 = new LocalDate(2012, 2, 29).WithCalendar(CalendarSystem.Badi);
            LocalDate d2 = new LocalDate(2013, 2, 28).WithCalendar(CalendarSystem.Badi);

            Assert.AreEqual("168-0-4", AsBadiString(d1));
            Assert.AreEqual("169-0-3", AsBadiString(d2));

            Assert.AreEqual(Period.FromMonths(19) + Period.FromDays(18), Period.Between(d1, d2));
        }

        [Test]
        public void BetweenLocalDates_AfterLeapYear()
        {
            LocalDate d1 = CreateBadiDate(180, 19, 5);
            LocalDate d2 = CreateBadiDate(181, 19, 5);
            Assert.AreEqual(Period.FromYears(1), Period.Between(d1, d2));
            Assert.AreEqual(Period.FromYears(-1), Period.Between(d2, d1));
        }


        [Test]
        public void Addition_DayCrossingMonthBoundary()
        {
            LocalDate start = CreateBadiDate(182, 4, 13);
            LocalDate result = start + Period.FromDays(10);
            Assert.AreEqual(CreateBadiDate(182, 5, 4), result);
        }

        [Test]
        public void Addition()
        {
            var start = CreateBadiDate(182, 1, 1);

            var result = start + Period.FromDays(3);
            Assert.AreEqual(CreateBadiDate(182, 1, 4), result);

            result = start + Period.FromDays(20);
            Assert.AreEqual(CreateBadiDate(182, 2, 2), result);
        }

        [Test]
        public void Addition_DayCrossingMonthBoundaryFromAyyamiHa()
        {
            var start = CreateBadiDate(182, AyyamiHaMonth, 3);

            var result = start + Period.FromDays(10);
            // in 182, Ayyam-i-Ha has 5 days
            Assert.AreEqual(CreateBadiDate(182, 19, 8), result);
        }

        [Test]
        public void Addition_OneYearOnLeapDay()
        {
            LocalDate start = CreateBadiDate(182, AyyamiHaMonth, 5);
            LocalDate result = start + Period.FromYears(1);
            // Ayyam-i-Ha 5 becomes Ayyam-i-Ha 4
            Assert.AreEqual(CreateBadiDate(183, AyyamiHaMonth, 4), result);
        }

        [Test]
        public void Addition_FiveYearsOnLeapDay()
        {
            LocalDate start = CreateBadiDate(182, AyyamiHaMonth, 5);
            LocalDate result = start + Period.FromYears(5);
            Assert.AreEqual(CreateBadiDate(187, AyyamiHaMonth, 5), result);
        }

        [Test]
        public void Addition_YearMonthDay()
        {
            // One year, one month, two days
            Period period = Period.FromYears(1) + Period.FromMonths(1) + Period.FromDays(2);
            LocalDate start = CreateBadiDate(171, 1, 19);
            // Periods are added in order, so this becomes...
            // Add one year: 172.1.19
            // Add one month: 172.2.19
            // Add two days: 172.3.2
            LocalDate result = start + period;
            Assert.AreEqual(CreateBadiDate(172, 3, 2), result);
        }

        [Test]
        public void PlusMonths_Overflow()
        {
            var calendar = CalendarSystem.Badi;
            var earlyDate = new LocalDate(calendar.MinYear, 1, 1, calendar);
            var lateDate = new LocalDate(calendar.MaxYear, 19, 1, calendar);

            Assert.Throws<OverflowException>(() => earlyDate.PlusMonths(-1));
            Assert.Throws<OverflowException>(() => lateDate.PlusMonths(1));
        }        

        /// <summary>
        /// Create a <see cref="LocalDate"/> in the Badíʿ calendar, treating 0
        /// as the month containing Ayyam-i-Ha.
        /// </summary>
        /// <param name="year">Year in the Badíʿ calendar</param>
        /// <param name="month">Month (use 0 for Ayyam-i-Ha)</param>
        /// <param name="day">Day in month</param>
        private static LocalDate CreateBadiDate(int year, int month, int day)
        {
            if (month == AyyamiHaMonth)
            {
                Preconditions.CheckArgumentRange(nameof(day), day, 1, BadiYearMonthDayCalculator.GetDaysInAyyamiHa(year));
                // Move Ayyam-i-Ha days to fall after the last day of month 18.
                month = BadiYearMonthDayCalculator.Month18;
                day += BadiYearMonthDayCalculator.DaysInMonth;
            }
            return new LocalDate(year, month, day, CalendarSystem.Badi);
        }

        /// <summary>
        /// Return the day of this month, treating Ayyam-i-Ha as a separate month.
        /// </summary>
        internal static int BadiDay(LocalDate input)
        {
            Preconditions.CheckArgument(input.Calendar == CalendarSystem.Badi, nameof(input), "Only valid when using the Badíʿ calendar");

            if (input.Month == BadiYearMonthDayCalculator.Month18 &&
                input.Day > BadiYearMonthDayCalculator.DaysInMonth)
            {
                return input.Day - BadiYearMonthDayCalculator.DaysInMonth;
            }
            return input.Day;
        }

        /// <summary>
        /// Return the month of this date. If in Ayyam-i-Ha, returns 0.
        /// </summary>
        internal static int BadiMonth(LocalDate input)
        {
            Preconditions.CheckArgument(input.Calendar == CalendarSystem.Badi, nameof(input), "Only valid when using the Badíʿ calendar");

            if (input.Month == BadiYearMonthDayCalculator.Month18 &&
                input.Day > BadiYearMonthDayCalculator.DaysInMonth)
            {
                return AyyamiHaMonth;
            }
            return input.Month;
        }

        /// <summary>
        /// Get a text representation of the date.
        /// </summary>
        internal static string AsBadiString(LocalDate input)
        {
            var year = input.Year;
            var month = BadiCalendarSystemTest.BadiMonth(input);
            var day = BadiCalendarSystemTest.BadiDay(input);

            return $"{year}-{month}-{day}";
        }

        [Test]
        public void HelperMethod_BadiDay()
        {
            // ensure that this helper method is working
            Assert.AreEqual(BadiDay(CreateBadiDate(180, 10, 10)), 10);
            Assert.AreEqual(BadiDay(CreateBadiDate(180, 18, 19)), 19);
            Assert.AreEqual(BadiDay(CreateBadiDate(180, 0, 3)), 3);
            Assert.AreEqual(BadiDay(CreateBadiDate(180, 19, 1)), 1);
        }

        [Test]
        public void HelperMethod_BadiMonth()
        {
            // ensure that this helper method is working
            Assert.AreEqual(BadiMonth(CreateBadiDate(180, 10, 10)), 10);
            Assert.AreEqual(BadiMonth(CreateBadiDate(180, 18, 19)), 18);
            Assert.AreEqual(BadiMonth(CreateBadiDate(180, 0, 3)), 0);
            Assert.AreEqual(BadiMonth(CreateBadiDate(180, 19, 1)), 19);
        }

        [Test]
        public void HelperMethod_AsBadiString()
        {
            // ensure that this helper method is working
            Assert.AreEqual(AsBadiString(CreateBadiDate(180, 10, 10)), "180-10-10");
            Assert.AreEqual(AsBadiString(CreateBadiDate(180, 18, 19)), "180-18-19");
            Assert.AreEqual(AsBadiString(CreateBadiDate(180, 0, 3)), "180-0-3");
            Assert.AreEqual(AsBadiString(CreateBadiDate(180, 19, 1)), "180-19-1");
        }

    }
}
