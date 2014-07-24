// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    [TestFixture]
    public partial class IsoCalendarSystemTest
    {
        private static readonly DateTime UnixEpochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        // This was when I was writing the tests, having finally made everything work - several thousand lines
        // of shockingly untested code.
        private static readonly DateTime TimeOfGreatAchievement = new DateTime(2009, 11, 27, 18, 38, 25, 345, DateTimeKind.Utc) + TimeSpan.FromTicks(8765);

        private static readonly CalendarSystem Iso = CalendarSystem.Iso;

        [Test]
        public void FieldsOf_UnixEpoch()
        {
            // It's easiest to test this using a LocalDateTime in the ISO calendar system.
            // LocalDateTime just passes everything through anyway.
            LocalDateTime epoch = NodaConstants.UnixEpoch.InUtc().LocalDateTime;

            Assert.AreEqual(1970, epoch.Year);
            Assert.AreEqual(1970, epoch.YearOfEra);
            Assert.AreEqual(70, epoch.YearOfCentury);
            Assert.AreEqual(19, epoch.CenturyOfEra);
            Assert.AreEqual(1970, epoch.WeekYear);
            Assert.AreEqual(1, epoch.WeekOfWeekYear);
            Assert.AreEqual(1, epoch.Month);
            Assert.AreEqual(1, epoch.Day);
            Assert.AreEqual(1, epoch.DayOfYear);
            Assert.AreEqual(IsoDayOfWeek.Thursday, epoch.IsoDayOfWeek);
            Assert.AreEqual(4, epoch.DayOfWeek);
            Assert.AreEqual(Era.Common, epoch.Era);
            Assert.AreEqual(0, epoch.Hour);
            Assert.AreEqual(0, epoch.Minute);
            Assert.AreEqual(0, epoch.Second);
            Assert.AreEqual(0, epoch.Millisecond);
            Assert.AreEqual(0, epoch.TickOfDay);
            Assert.AreEqual(0, epoch.TickOfSecond);
        }

        [Test]
        public void FieldsOf_GreatAchievement()
        {
            LocalDateTime now = Instant.FromTicksSinceUnixEpoch((TimeOfGreatAchievement - UnixEpochDateTime).Ticks).InUtc().LocalDateTime;

            Assert.AreEqual(2009, now.Year);
            Assert.AreEqual(2009, now.YearOfEra);
            Assert.AreEqual(9, now.YearOfCentury);
            Assert.AreEqual(20, now.CenturyOfEra);
            Assert.AreEqual(2009, now.WeekYear);
            Assert.AreEqual(48, now.WeekOfWeekYear);
            Assert.AreEqual(11, now.Month);
            Assert.AreEqual(27, now.Day);
            Assert.AreEqual(TimeOfGreatAchievement.DayOfYear, now.DayOfYear);
            Assert.AreEqual(IsoDayOfWeek.Friday, now.IsoDayOfWeek);
            Assert.AreEqual(5, now.DayOfWeek);
            Assert.AreEqual(Era.Common, now.Era);
            Assert.AreEqual(18, now.Hour);
            Assert.AreEqual(38, now.Minute);
            Assert.AreEqual(25, now.Second);
            Assert.AreEqual(345, now.Millisecond);
            Assert.AreEqual(3458765, now.TickOfSecond);
            Assert.AreEqual(18 * NodaConstants.TicksPerHour +
                            38 * NodaConstants.TicksPerMinute +
                            25 * NodaConstants.TicksPerSecond +
                            3458765,
                            now.TickOfDay);
        }

        [Test]
        public void ConstructLocalInstant_WithAllFields()
        {
            LocalInstant localAchievement = new LocalDateTime(2009, 11, 27, 18, 38, 25, 345, 8765).ToLocalInstant();
            long bclTicks = (TimeOfGreatAchievement - UnixEpochDateTime).Ticks;
            int bclDays = (int) (bclTicks / NodaConstants.TicksPerStandardDay);
            long bclTickOfDay = bclTicks % NodaConstants.TicksPerStandardDay;
            Assert.AreEqual(bclDays, localAchievement.DaysSinceEpoch);
            Assert.AreEqual(bclTickOfDay, localAchievement.NanosecondOfDay / NodaConstants.NanosecondsPerTick);
        }

        [Test]
        public void IsoCalendarUsesIsoDayOfWeek()
        {
            Assert.IsTrue(CalendarSystem.Iso.UsesIsoDayOfWeek);
        }

        [Test]
        public void IsLeapYear()
        {
            Assert.IsTrue(CalendarSystem.Iso.IsLeapYear(2012)); // 4 year rule
            Assert.IsFalse(CalendarSystem.Iso.IsLeapYear(2011)); // 4 year rule
            Assert.IsFalse(CalendarSystem.Iso.IsLeapYear(2100)); // 100 year rule
            Assert.IsTrue(CalendarSystem.Iso.IsLeapYear(2000)); // 400 year rule
        }

        [Test]
        public void GetDaysInMonth()
        {
            Assert.AreEqual(30, CalendarSystem.Iso.GetDaysInMonth(2010, 9));
            Assert.AreEqual(31, CalendarSystem.Iso.GetDaysInMonth(2010, 1));
            Assert.AreEqual(28, CalendarSystem.Iso.GetDaysInMonth(2010, 2));
            Assert.AreEqual(29, CalendarSystem.Iso.GetDaysInMonth(2012, 2));
        }

        [Test]
        public void WeekYearLessThanYear()
        {
            // January 1st 2011 was a Saturday, and therefore part of WeekYear 2010.
            LocalDate localDate = new LocalDate(2011, 1, 1);
            Assert.AreEqual(2011, localDate.Year);
            Assert.AreEqual(2010, localDate.WeekYear);
            Assert.AreEqual(52, localDate.WeekOfWeekYear);
        }

        [Test]
        public void WeekYearGreaterThanYear()
        {
            // December 31st 2012 is a Monday, and thus part of WeekYear 2013.
            LocalDate localDate = new LocalDate(2012, 12, 31);
            Assert.AreEqual(2012, localDate.Year);
            Assert.AreEqual(2013, localDate.WeekYear);
            Assert.AreEqual(1, localDate.WeekOfWeekYear);
        }

        [Test]
        public void BeforeCommonEra()
        {
            // Year -1 in absolute terms is 2BCE
            LocalDate localDate = new LocalDate(-1, 1, 1);
            Assert.AreEqual(Era.BeforeCommon, localDate.Era);
            Assert.AreEqual(-1, localDate.Year);
            Assert.AreEqual(2, localDate.YearOfEra);
        }

        [Test]
        public void BeforeCommonEra_BySpecifyingEra()
        {
            // Year -1 in absolute terms is 2BCE
            LocalDate localDate = new LocalDate(Era.BeforeCommon, 2, 1, 1);
            Assert.AreEqual(Era.BeforeCommon, localDate.Era);
            Assert.AreEqual(-1, localDate.Year);
            Assert.AreEqual(2, localDate.YearOfEra);
        }
    }
}