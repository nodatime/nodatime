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

using NUnit.Framework;
using NodaTime.Calendars;
using NodaTime.Fields;

namespace NodaTime.Test.Calendars
{
    /// <summary>
    /// Tests for <see cref="CopticCalendarSystem"/>.
    /// </summary>
    [TestFixture]
    public class CopticCalendarSystemTest
    {
        [Test]
        public void CopticEpoch()
        {
            CalendarSystem coptic = CopticCalendarSystem.GetInstance(4);
            LocalDateTime copticEpoch = new LocalDateTime(1, 1, 1, 0, 0, coptic);

            CalendarSystem julian = JulianCalendarSystem.GetInstance(4);
            LocalDateTime converted = new LocalDateTime(copticEpoch.LocalInstant, julian);

            LocalDateTime expected = new LocalDateTime(284, 8, 29, 0, 0, julian);
            Assert.AreEqual(expected, converted);
        }

        [Test]
        public void UnixEpoch()
        {
            CalendarSystem coptic = CopticCalendarSystem.GetInstance(4);
            LocalDateTime unixEpochInCopticCalendar = new LocalDateTime(LocalInstant.LocalUnixEpoch, coptic);
            LocalDateTime expected = new LocalDateTime(1686, 4, 23, 0, 0, coptic);
            Assert.AreEqual(expected, unixEpochInCopticCalendar);
        }

        [Test]
        public void SampleDate()
        {
            CalendarSystem copticCalendar = CopticCalendarSystem.GetInstance(4);
            LocalDateTime iso = new LocalDateTime(2004, 6, 9, 0, 0, 0, 0);
            LocalDateTime coptic = new LocalDateTime(iso.LocalInstant, copticCalendar);

            Assert.AreEqual(CopticCalendarSystem.AnnoMartyrm, coptic.Era);
            Assert.AreEqual(18, coptic.CenturyOfEra);
            Assert.AreEqual(20, coptic.YearOfCentury);
            Assert.AreEqual(1720, coptic.YearOfEra);

            Assert.AreEqual(1720, coptic.Year);
            Assert.IsFalse(copticCalendar.IsLeapYear(1720));
        
            Assert.AreEqual(10, coptic.MonthOfYear);
            Assert.AreEqual(2, coptic.DayOfMonth);
            
            // TODO: Determine whether we should consider the Coptic calendar to use ISO
            // days of the week.
            Assert.AreEqual(IsoDayOfWeek.Wednesday, coptic.IsoDayOfWeek);

            Assert.AreEqual(9 * 30 + 2, coptic.DayOfYear);

            Assert.AreEqual(0, coptic.HourOfDay);
            Assert.AreEqual(0, coptic.MinuteOfHour);
            Assert.AreEqual(0, coptic.SecondOfMinute);
            Assert.AreEqual(0, coptic.MillisecondOfSecond);
            Assert.AreEqual(0, coptic.TickOfMillisecond);
        }
    }
}
