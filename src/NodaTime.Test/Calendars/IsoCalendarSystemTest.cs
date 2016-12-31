// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
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
            Assert.AreEqual(1970, WeekYearRules.Iso.GetWeekYear(epoch.Date));
            Assert.AreEqual(1, WeekYearRules.Iso.GetWeekOfWeekYear(epoch.Date));
            Assert.AreEqual(1, epoch.Month);
            Assert.AreEqual(1, epoch.Day);
            Assert.AreEqual(1, epoch.DayOfYear);
            Assert.AreEqual(IsoDayOfWeek.Thursday, epoch.DayOfWeek);
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
            LocalDateTime now = Instant.FromUnixTimeTicks((TimeOfGreatAchievement - UnixEpochDateTime).Ticks).InUtc().LocalDateTime;

            Assert.AreEqual(2009, now.Year);
            Assert.AreEqual(2009, now.YearOfEra);
            Assert.AreEqual(2009, WeekYearRules.Iso.GetWeekYear(now.Date));
            Assert.AreEqual(48, WeekYearRules.Iso.GetWeekOfWeekYear(now.Date));
            Assert.AreEqual(11, now.Month);
            Assert.AreEqual(27, now.Day);
            Assert.AreEqual(TimeOfGreatAchievement.DayOfYear, now.DayOfYear);
            Assert.AreEqual(IsoDayOfWeek.Friday, now.DayOfWeek);
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
            LocalInstant localAchievement = new LocalDateTime(2009, 11, 27, 18, 38, 25, 345).PlusTicks(8765).ToLocalInstant();
            long bclTicks = (TimeOfGreatAchievement - UnixEpochDateTime).Ticks;
            int bclDays = (int) (bclTicks / NodaConstants.TicksPerDay);
            long bclTickOfDay = bclTicks % NodaConstants.TicksPerDay;
            Assert.AreEqual(bclDays, localAchievement.DaysSinceEpoch);
            Assert.AreEqual(bclTickOfDay, localAchievement.NanosecondOfDay / NodaConstants.NanosecondsPerTick);
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