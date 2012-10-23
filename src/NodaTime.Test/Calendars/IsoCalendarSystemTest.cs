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

using System;
using NUnit.Framework;
using NodaTime.Calendars;
using NodaTime.Fields;

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

        private static readonly FieldSet isoFields = CalendarSystem.Iso.Fields;

        [Test]
        public void FieldsOf_UnixEpoch()
        {
            // It's easiest to test this using a LocalDateTime in the ISO calendar system.
            // LocalDateTime just passes everything through anyway.
            LocalDateTime epoch = new LocalDateTime(LocalInstant.LocalUnixEpoch, CalendarSystem.Iso);

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
            LocalDateTime now = new LocalDateTime(new LocalInstant((TimeOfGreatAchievement - UnixEpochDateTime).Ticks), CalendarSystem.Iso);

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
        public void GetLocalInstant_WithAllFields()
        {
            LocalInstant localAchievement = CalendarSystem.Iso.GetLocalInstant(2009, 11, 27, 18, 38, 25, 345, 8765);
            Assert.AreEqual((TimeOfGreatAchievement - UnixEpochDateTime).Ticks, localAchievement.Ticks);
        }

        [Test]
        public void GetLocalInstant_WithValidTickOfDay()
        {
            LocalInstant expected = new LocalInstant(2009, 11, 27, 3, 0);
            LocalInstant actual = CalendarSystem.Iso.GetLocalInstant(2009, 11, 27, NodaConstants.TicksPerHour * 3);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetLocalInstant_WithInvalidTickOfDay()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CalendarSystem.Iso.GetLocalInstant(2009, 11, 27, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => CalendarSystem.Iso.GetLocalInstant(2009, 11, 27, NodaConstants.TicksPerStandardDay));
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
    }
}