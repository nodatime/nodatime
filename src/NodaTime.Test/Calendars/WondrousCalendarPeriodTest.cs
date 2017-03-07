// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Calendars;
using NodaTime.Text;
using NUnit.Framework;
using NodaTime.Calendars.Wondrous;

namespace NodaTime.Test.Calendars
{
    public class WondrousCalendarPeriodTest
    {
        private const int AyyamiHaMonthNum = WondrousCalendarHelper.AyyamiHaMonth0;

        private static readonly LocalDate TestDate1_167_5_15 = WondrousCalendarHelper.CreateDate(167, 5, 15);

        private static readonly LocalDate TestDate1_167_6_7 = WondrousCalendarHelper.CreateDate(167, 6, 7);
        // March 1st 2011
        private static readonly LocalDate TestDate2_167_Ayyam_4 = WondrousCalendarHelper.CreateDate(167, AyyamiHaMonthNum, 4);
        // March 1st 2012
        private static readonly LocalDate TestDate3_168_Ayyam_5 = WondrousCalendarHelper.CreateDate(168, AyyamiHaMonthNum, 5);

        [Test]
        public void BetweenLocalDates_InvalidUnits()
        {
            Assert.Throws<ArgumentException>(() => Period.Between(TestDate1_167_5_15, TestDate2_167_Ayyam_4, 0));
            Assert.Throws<ArgumentException>(() => Period.Between(TestDate1_167_5_15, TestDate2_167_Ayyam_4, (PeriodUnits)(-1)));
            Assert.Throws<ArgumentException>(() => Period.Between(TestDate1_167_5_15, TestDate2_167_Ayyam_4, PeriodUnits.AllTimeUnits));
            Assert.Throws<ArgumentException>(() => Period.Between(TestDate1_167_5_15, TestDate2_167_Ayyam_4, PeriodUnits.Years | PeriodUnits.Hours));
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
        public void TestGregorianPeriod()
        {
            var d1 = new LocalDate(2012, 1, 20);
            var d2 = new LocalDate(2012, 3, 22);

            Assert.AreEqual(Period.FromMonths(2) + Period.FromDays(2), Period.Between(d1, d2));
        }

        [Test]
        public void BetweenLocalDates_AsymmetricForwardAndBackward()
        {
            LocalDate d1 = WondrousCalendarHelper.CreateDate(166, 18, 4);
            LocalDate d2 = WondrousCalendarHelper.CreateDate(167, 1, 10);

            // spanning Ayyam-i-Ha - not counted as a month
            Assert.AreEqual(Period.FromMonths(2) + Period.FromDays(6), Period.Between(d1, d2));
            Assert.AreEqual(Period.FromMonths(-2) + Period.FromDays(-6), Period.Between(d2, d1));
        }

        [Test]
        public void BetweenLocalDates_EndOfMonth()
        {
            LocalDate d1 = WondrousCalendarHelper.CreateDate(171, 5, 19);
            LocalDate d2 = WondrousCalendarHelper.CreateDate(171, 6, 19);
            Assert.AreEqual(Period.FromMonths(1), Period.Between(d1, d2));
            Assert.AreEqual(Period.FromMonths(-1), Period.Between(d2, d1));
        }

        [Test]
        public void BetweenLocalDates_OnLeapYear()
        {
            //            System.Diagnostics.Debugger.Launch();

            LocalDate d1 = new LocalDate(2012, 2, 29).WithCalendar(CalendarSystem.Wondrous);
            LocalDate d2 = new LocalDate(2013, 2, 28).WithCalendar(CalendarSystem.Wondrous);

            Assert.AreEqual("168-0-4", d1.AsWondrousString());
            Assert.AreEqual("169-0-3", d2.AsWondrousString());

            Assert.AreEqual(Period.FromMonths(19) + Period.FromDays(18), Period.Between(d1, d2));
        }

        [Test]
        public void BetweenLocalDates_AfterLeapYear()
        {
            LocalDate d1 = WondrousCalendarHelper.CreateDate(180, 19, 5);
            LocalDate d2 = WondrousCalendarHelper.CreateDate(181, 19, 5);
            Assert.AreEqual(Period.FromYears(1), Period.Between(d1, d2));
            Assert.AreEqual(Period.FromYears(-1), Period.Between(d2, d1));
        }

        [Test]
        public void CreateDate_InAyyamiHa() {
            var d1 = WondrousCalendarHelper.CreateDate(180, 0, 3);
            var d3 = WondrousCalendarHelper.CreateDate(180, 18, 22);

            Assert.AreEqual(d1, d3);
        }


        [Test]
        public void CreateDate_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => WondrousCalendarHelper.CreateDate(180,-1,1));
            Assert.Throws<ArgumentOutOfRangeException>(() => WondrousCalendarHelper.CreateDate(180,1,-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => WondrousCalendarHelper.CreateDate(180,0,0));
            Assert.Throws<ArgumentOutOfRangeException>(() => WondrousCalendarHelper.CreateDate(180,0,5));
            Assert.Throws<ArgumentOutOfRangeException>(() => WondrousCalendarHelper.CreateDate(182,0,6));
            Assert.Throws<ArgumentOutOfRangeException>(() => WondrousCalendarHelper.CreateDate(180,1,0));
            Assert.Throws<ArgumentOutOfRangeException>(() => WondrousCalendarHelper.CreateDate(180,1,20));
            Assert.Throws<ArgumentOutOfRangeException>(() => WondrousCalendarHelper.CreateDate(180,20,1));
        }

        //[Test]
        //public void BetweenLocalDateTimes_OnLeapYear()
        //{
        //    LocalDateTime dt1 = WondrousCalendarHelper.CreateDateTime(168, 18, 18, 2, 0);
        //    LocalDateTime dt2 = WondrousCalendarHelper.CreateDateTime(168, 18, 19, 4, 0);
        //    LocalDateTime dt3 = WondrousCalendarHelper.CreateDateTime(169, 18, 18, 3, 0);

        //    Assert.AreEqual(Parse("P1YT1H"), Period.Between(dt1, dt3));

        //    Assert.AreEqual(Parse("P11M29DT23H"), Period.Between(dt2, dt3));

        //    Assert.AreEqual(Parse("P-11M-28DT-1H"), Period.Between(dt3, dt1));
        //    Assert.AreEqual(Parse("P-11M-27DT-23H"), Period.Between(dt3, dt2));
        //}

        [Test]
        public void Addition_DayCrossingMonthBoundary()
        {
            LocalDate start = WondrousCalendarHelper.CreateDate(182, 4, 13);
            LocalDate result = start + Period.FromDays(10);
            Assert.AreEqual(WondrousCalendarHelper.CreateDate(182, 5, 4), result);
        }

        [Test]
        public void Addition()
        {
            var start = WondrousCalendarHelper.CreateDate(182, 1, 1);

            var result = start + Period.FromDays(3);
            Assert.AreEqual(WondrousCalendarHelper.CreateDate(182, 1, 4), result);

            result = start + Period.FromDays(20);
            Assert.AreEqual(WondrousCalendarHelper.CreateDate(182, 2, 2), result);
        }

        [Test]
        public void Addition_DayCrossingMonthBoundaryFromAyyamiHa()
        {
            var start = WondrousCalendarHelper.CreateDate(182, AyyamiHaMonthNum, 3);
            //            System.Diagnostics.Debugger.Launch();

            var result = start + Period.FromDays(10);
            // in 182, Ayyam-i-Ha has 5 days
            Assert.AreEqual(WondrousCalendarHelper.CreateDate(182, 19, 8), result);
        }

        [Test]
        public void Addition_OneYearOnLeapDay()
        {
            LocalDate start = WondrousCalendarHelper.CreateDate(182, AyyamiHaMonthNum, 5);
            LocalDate result = start + Period.FromYears(1);
            // Ayyam-i-Ha 5 becomes Ayyam-i-Ha 4 ??
            Assert.AreEqual(WondrousCalendarHelper.CreateDate(183, AyyamiHaMonthNum, 4), result);
        }

        [Test]
        public void Addition_FiveYearsOnLeapDay()
        {
            LocalDate start = WondrousCalendarHelper.CreateDate(182, AyyamiHaMonthNum, 5);
            LocalDate result = start + Period.FromYears(5);
            Assert.AreEqual(WondrousCalendarHelper.CreateDate(187, AyyamiHaMonthNum, 5), result);
        }

        [Test]
        public void Addition_YearMonthDay()
        {
            // One year, one month, two days
            Period period = Period.FromYears(1) + Period.FromMonths(1) + Period.FromDays(2);
            LocalDate start = WondrousCalendarHelper.CreateDate(171, 1, 19);
            // Periods are added in order, so this becomes...
            // Add one year: 172.1.19
            // Add one month: 172.2.19
            // Add two days: 172.3.2
            LocalDate result = start + period;
            Assert.AreEqual(WondrousCalendarHelper.CreateDate(172, 3, 2), result);
        }



        ///// <summary>
        ///// Just a simple way of parsing a period string. It's a more compact period representation.
        ///// </summary>
        //private static Period Parse(string text)
        //{
        //    return PeriodPattern.Roundtrip.Parse(text).Value;
        //}
    }
}
