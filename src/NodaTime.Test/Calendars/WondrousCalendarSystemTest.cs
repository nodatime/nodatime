// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NodaTime.Calendars.Wondrous;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    /// <summary>
    /// Tests for the Wondrous calendar system.
    /// </summary>
    public class WondrousCalendarSystemTest
    {
        const int AyyamiHa = WondrousCalendarHelper.AyyamiHaMonth0;

        [Test]
        public void WondrousEpoch()
        {
            CalendarSystem wondrous = CalendarSystem.Wondrous;
            LocalDateTime wondrousEpoch = WondrousCalendarHelper.CreateDateTime(1, 1, 1, 0, 0);

            CalendarSystem gregorian = CalendarSystem.Gregorian;
            LocalDateTime converted = wondrousEpoch.WithCalendar(gregorian);

            LocalDateTime expected = new LocalDateTime(1844, 3, 21, 0, 0);

            //Assert.AreEqual(expected, converted);
            Assert.AreEqual(expected.ToString(), converted.ToString());
        }

        [Test]
        public void UnixEpoch()
        {
            CalendarSystem wondrous = CalendarSystem.Wondrous;
            LocalDateTime unixEpochInWondrousCalendar = NodaConstants.UnixEpoch.InZone(DateTimeZone.Utc, wondrous).LocalDateTime;
            LocalDateTime expected = WondrousCalendarHelper.CreateDateTime(126, 16, 2, 0, 0);
            Assert.AreEqual(expected, unixEpochInWondrousCalendar);
        }

        [Test]
        public void SampleDate()
        {

            CalendarSystem wondrousCalendar = CalendarSystem.Wondrous;
            LocalDateTime iso = new LocalDateTime(2017, 3, 4, 0, 0, 0, 0);
            LocalDateTime wondrous = iso.WithCalendar(wondrousCalendar);

            Assert.AreEqual(19, wondrous.WondrousMonthNum());

            Assert.AreEqual(Era.BahaiEra, wondrous.Era);
            Assert.AreEqual(173, wondrous.YearOfEra);

            Assert.AreEqual(173, wondrous.Year);
            Assert.IsFalse(wondrousCalendar.IsLeapYear(173));

            Assert.AreEqual(4, wondrous.WondrousDayNum());

            Assert.AreEqual(IsoDayOfWeek.Saturday, wondrous.DayOfWeek);

            //            Assert.AreEqual(9 * 30 + 2, wondrous.DayOfYear);

            Assert.AreEqual(0, wondrous.Hour);
            Assert.AreEqual(0, wondrous.Minute);
            Assert.AreEqual(0, wondrous.Second);
            Assert.AreEqual(0, wondrous.Millisecond);
        }

        [Test]
        //[TestCase(2010, 3, 21, 167, 1, 1)]
        //[TestCase(2015, 3, 21, 172, 1, 1)]
        //[TestCase(2016, 1, 1, 172, 16, 2)]
        //[TestCase(2016, 1, 21, 172, 17, 3)]
        //[TestCase(2016, 12, 31, 173, 16, 2)]
        //[TestCase(2016, 2, 20, 172, 18, 14)]
        //[TestCase(2016, 2, 25, 172, 18, 19)]
        [TestCase(2016, 2, 26, 172, AyyamiHa, 1)]
        [TestCase(2016, 2, 29, 172, AyyamiHa, 4)]
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
        public void GeneralGtoW(int gYear, int gMonth, int gDay, int wYear, int wMonth, int wDay)
        {
            // create in this calendar
            var wDate = WondrousCalendarHelper.CreateDate(wYear, wMonth, wDay);
            var gDate = wDate.WithCalendar(CalendarSystem.Gregorian);
            Assert.AreEqual(gYear, gDate.Year);
            Assert.AreEqual(gMonth, gDate.Month);
            Assert.AreEqual(gDay, gDate.Day);

            // convert to this calendar
            var wDate2 = new LocalDate(gYear, gMonth, gDay).WithCalendar(CalendarSystem.Wondrous);
            Assert.AreEqual(wYear, wDate2.Year);
            Assert.AreEqual(wMonth, wDate2.WondrousMonthNum());
            Assert.AreEqual(wDay, wDate2.WondrousDayNum());
        }

        [Test]
        [TestCase(2012, 2, 29, 168, AyyamiHa, 4)]
        [TestCase(2012, 3, 1, 168, AyyamiHa, 5)]
        [TestCase(2015, 3, 1, 171, AyyamiHa, 4)]
        [TestCase(2015, 3, 1, 171, AyyamiHa, 4)]
        [TestCase(2016, 3, 1, 172, 19, 1)]
        [TestCase(2016, 3, 19, 172, 19, 19)]
        [TestCase(2017, 3, 1, 173, 19, 1)]
        [TestCase(2017, 3, 19, 173, 19, 19)]
        [TestCase(2018, 2, 24, 174, 18, 19)]
        [TestCase(2018, 2, 25, 174, AyyamiHa, 1)]
        [TestCase(2018, 3, 1, 174, AyyamiHa, 5)]
        [TestCase(2018, 3, 2, 174, 19, 1)]
        [TestCase(2018, 3, 19, 174, 19, 18)]
        public void SpecialCases(int gYear, int gMonth, int gDay, int wYear, int wMonth, int wDay)
        {
            //            System.Diagnostics.Debugger.Launch();

            // create in test calendar
            var wDate = WondrousCalendarHelper.CreateDate(wYear, wMonth, wDay);

            // convert to gregorian
            var gDate = wDate.WithCalendar(CalendarSystem.Gregorian);
            WondrousYearMonthDayCalculator.ConsoleWriteLine("{0}-->{1}", WondrousCalendarHelper.AsWondrousString(wDate), gDate.YearMonthDay);

            Assert.AreEqual($"{gYear}-{gMonth}-{gDay}", $"{gDate.Year}-{gDate.Month}-{gDate.Day}");

            // create in gregorian
            // convert to test calendar
            var gDate2 = new LocalDate(gYear, gMonth, gDay);
            var wDate2 = gDate2.WithCalendar(CalendarSystem.Wondrous);
            WondrousYearMonthDayCalculator.ConsoleWriteLine("{0}-->{1}", gDate2.YearMonthDay, WondrousCalendarHelper.AsWondrousString(wDate2));

            Assert.AreEqual($"{wYear}-{wMonth}-{wDay}", $"{wDate2.Year}-{wDate2.WondrousMonthNum()}-{wDate2.WondrousDayNum()}");
        }

        /// <summary>
        /// Same as GeneralGtoW but listed in other order
        /// </summary>
        [Test]
        [TestCase(1, 1, 1, 1844, 3, 21)]
        [TestCase(169, 1, 1, 2012, 3, 21)]
        [TestCase(170, 1, 1, 2013, 3, 21)]
        [TestCase(171, 1, 1, 2014, 3, 21)]
        [TestCase(171, 1, 1, 2014, 3, 21)]
        [TestCase(172, AyyamiHa, 1, 2016, 2, 26)]
        [TestCase(172, AyyamiHa, 2, 2016, 2, 27)]
        [TestCase(172, AyyamiHa, 3, 2016, 2, 28)]
        [TestCase(172, AyyamiHa, 4, 2016, 2, 29)]
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
        public void GeneralWtoG(int wYear, int wMonth, int wDay, int gYear, int gMonth, int gDay)
        {
            // create in this calendar
            var wDate = WondrousCalendarHelper.CreateDate(wYear, wMonth, wDay);
            var gDate = wDate.WithCalendar(CalendarSystem.Gregorian);
            Assert.AreEqual(gYear, gDate.Year);
            Assert.AreEqual(gMonth, gDate.Month);
            Assert.AreEqual(gDay, gDate.Day);

            // convert to this calendar
            var wDate2 = new LocalDate(gYear, gMonth, gDay).WithCalendar(CalendarSystem.Wondrous);
            Assert.AreEqual(wYear, wDate2.Year);
            Assert.AreEqual(wMonth, wDate2.WondrousMonthNum());
            Assert.AreEqual(wDay, wDate2.WondrousDayNum());
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
        public void DaysInAyyamiHa(int wYear, int days)
        {
            var wondrous = new WondrousYearMonthDayCalculator();
            Assert.AreEqual(days, wondrous.DaysInAyyamHa(wYear));
        }

        /*
            After above tests run, poor man's test coverage:

            Label	Num

            ~A	193
            ~B
            ~C	57
            ~D	284
            ~E
            ~F	1
            ~G
            ~H
            ~I
            ~J
            ~K	142
            ~L	348
            ~M	139
            ~N	755
            ~O	18
            ~P	82

        */

        /// <summary>
        /// Not covered by other tests
        /// </summary>
        [Test]
        [TestCase(165, 1, 1, 1)]
        [TestCase(170, 1, 1, 1)]
        [TestCase(172, 1, 1, 1)]
        [TestCase(175, 1, 1, 1)]
        [TestCase(173, 18, 1, 17 * 19 + 1)]
        [TestCase(173, 18, 19, 18 * 19)]
        [TestCase(173, AyyamiHa, 1, 18 * 19 + 1)]
        [TestCase(173, 19, 1, 18 * 19 + 5)]
        [TestCase(220, AyyamiHa, 1, 18 * 19 + 1)]
        [TestCase(220, AyyamiHa, 5, 18 * 19 + 5)]
        [TestCase(220, 19, 1, 18 * 19 + 6)]
        public void TestB(int wYear, int wMonth, int wDay, int dayOfYear)
        {
            var wondrous = new WondrousYearMonthDayCalculator();
            Assert.AreEqual(dayOfYear, wondrous.GetDayOfYear(WondrousCalendarHelper.CreateDate(wYear, wMonth, wDay).YearMonthDay));
        }

        /// <summary>
        /// Not covered by other tests
        /// </summary>
        [Test]
        [TestCase(173, 1, 1, 19)]
        [TestCase(173, 18, 1, 19)]
        [TestCase(173, 19, 1, 19)]
        [TestCase(220, 19, 1, 19)]
        [TestCase(220, 4, 5, 19)]
        //[TestCase(173, AyyamiHa, 1, 4)] -- can't use with Ayyam-i-Ha
        //[TestCase(220, AyyamiHa, 1, 5)]
        public void TestE(int wYear, int wMonth, int wDay, int eomDay)
        {
            var start = WondrousCalendarHelper.CreateDate(wYear, wMonth, wDay);
            var end = WondrousCalendarHelper.CreateDate(wYear, wMonth, eomDay);
            Assert.AreEqual(end.AsWondrousString(), DateAdjusters.EndOfMonth(start).AsWondrousString());
        }

        /// <summary>
        /// Not covered by other tests
        /// </summary>
        [Test]
        public void TestF()
        {
            var calendar = CalendarSystem.Wondrous;
            Assert.IsFalse(calendar.IsLeapYear(172));
            Assert.IsFalse(calendar.IsLeapYear(173));
            Assert.IsTrue(calendar.IsLeapYear(207));
            Assert.IsTrue(calendar.IsLeapYear(220));
        }

        ///// <summary>
        ///// Not covered by other tests
        ///// </summary>
        //[Test]
        //public void TestG_Pattern()
        //{
        //    var pattern = LocalDatePattern.Iso.WithCalendar(CalendarSystem.Wondrous);
        //    var value = pattern.Parse("0182-18-17").Value;
        //    Assert.AreEqual(new LocalDate(182, 17, 18, CalendarSystem.Wondrous), value);
        //}

        /// <summary>
        /// Not covered by other tests
        /// </summary>
        [Test]
        public void TestH()
        {
        }

        /// <summary>
        /// Not covered by other tests
        /// </summary>
        [Test]
        public void TestI()
        {
        }

        /// <summary>
        /// Not covered by other tests
        /// </summary>
        [Test]
        public void TestJ()
        {
        }
    }
}
