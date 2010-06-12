#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
using NodaTime.Calendars;
using NodaTime.Fields;
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

        private static readonly ICalendarSystem Iso = IsoCalendarSystem.Instance;

        private static readonly FieldSet isoFields = IsoCalendarSystem.Instance.Fields;

        [Test]
        public void FieldsOf_UnixEpoch()
        {
            // It's easiest to test this using a LocalDateTime in the ISO calendar system.
            // LocalDateTime just passes everything through anyway.
            LocalDateTime epoch = new LocalDateTime(LocalInstant.LocalUnixEpoch, IsoCalendarSystem.Instance);

            Assert.AreEqual(1970, epoch.Year);
            Assert.AreEqual(1970, epoch.YearOfEra);
            Assert.AreEqual(70, epoch.YearOfCentury);
            Assert.AreEqual(19, epoch.CenturyOfEra);
            Assert.AreEqual(1970, epoch.WeekYear);
            Assert.AreEqual(1, epoch.WeekOfWeekYear);
            Assert.AreEqual(1, epoch.MonthOfYear);
            Assert.AreEqual(1, epoch.DayOfMonth);
            Assert.AreEqual(1, epoch.DayOfYear);
            Assert.AreEqual((int)DayOfWeek.Thursday, epoch.DayOfWeek);
            Assert.AreEqual(NodaConstants.CommonEra, epoch.Era);
            Assert.AreEqual(0, epoch.HourOfDay);
            Assert.AreEqual(0, epoch.MinuteOfHour);
            Assert.AreEqual(0, epoch.SecondOfMinute);
            Assert.AreEqual(0, epoch.SecondOfDay);
            Assert.AreEqual(0, epoch.MillisecondOfSecond);
            Assert.AreEqual(0, epoch.MillisecondOfDay);
            Assert.AreEqual(0, epoch.TickOfDay);
            Assert.AreEqual(0, epoch.TickOfMillisecond);
        }

        [Test]
        public void FieldsOf_GreatAchievement()
        {
            LocalDateTime now = new LocalDateTime(new LocalInstant((TimeOfGreatAchievement - UnixEpochDateTime).Ticks), IsoCalendarSystem.Instance);

            Assert.AreEqual(2009, now.Year);
            Assert.AreEqual(2009, now.YearOfEra);
            Assert.AreEqual(9, now.YearOfCentury);
            Assert.AreEqual(20, now.CenturyOfEra);
            Assert.AreEqual(2009, now.WeekYear);
            Assert.AreEqual(48, now.WeekOfWeekYear);
            Assert.AreEqual(11, now.MonthOfYear);
            Assert.AreEqual(27, now.DayOfMonth);
            Assert.AreEqual(TimeOfGreatAchievement.DayOfYear, now.DayOfYear);
            Assert.AreEqual((int)DayOfWeek.Friday, now.DayOfWeek);
            Assert.AreEqual(NodaConstants.CommonEra, now.Era);
            Assert.AreEqual(18, now.HourOfDay);
            Assert.AreEqual(38, now.MinuteOfHour);
            Assert.AreEqual(25, now.SecondOfMinute);
            Assert.AreEqual((18 * 60 * 60) + (38 * 60) + 25, now.SecondOfDay);
            Assert.AreEqual(345, now.MillisecondOfSecond);
            Assert.AreEqual(345 + now.SecondOfDay * 1000, now.MillisecondOfDay);
            Assert.AreEqual(now.MillisecondOfDay * 10000L + 8765, now.TickOfDay);
            Assert.AreEqual(8765, now.TickOfMillisecond);
        }

        [Test]
        public void GetLocalInstant_WithAllFields()
        {
            LocalInstant localAchievement = IsoCalendarSystem.Instance.GetLocalInstant(2009, 11, 27, 18, 38, 25, 345, 8765);
            Assert.AreEqual((TimeOfGreatAchievement - UnixEpochDateTime).Ticks, localAchievement.Ticks);
        }
    }
}