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
using System.Linq;
using NUnit.Framework;
using NodaTime.Calendars;

namespace NodaTime.Test.Calendars
{
    [TestFixture]
    public class GregorianCalendarSystemTest
    {
        [Test]
        public void LeapYears()
        {
            var calendar = CalendarSystem.GetGregorianCalendar(4);
            Assert.IsFalse(calendar.IsLeapYear(1900));
            Assert.IsFalse(calendar.IsLeapYear(1901));
            Assert.IsTrue(calendar.IsLeapYear(1904));
            Assert.IsTrue(calendar.IsLeapYear(1996));
            Assert.IsTrue(calendar.IsLeapYear(2000));
            Assert.IsFalse(calendar.IsLeapYear(2100));
            Assert.IsTrue(calendar.IsLeapYear(2400));
        }

        [Test]
        public void GetInstance_UniqueIds()
        {
            Assert.AreEqual(7, Enumerable.Range(1, 7).Select(x => GregorianCalendarSystem.GetInstance(x).Id).Distinct().Count());
        }

        [Test]
        public void GetInstance_InvalidMinDaysInFirstWeek()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => GregorianCalendarSystem.GetInstance(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => GregorianCalendarSystem.GetInstance(8));
        }

        [Test]
        public void GetInstance_MinDaysInFirstWeekIsRespected()
        {
            // Seems the simplest way to test this... yes, it seems somewhat wasteful, but hey...
            for (int i = 1; i < 7; i++)
            {
                GregorianCalendarSystem calendar = GregorianCalendarSystem.GetInstance(i);

                int actualMin = Enumerable.Range(1900, 400)
                                          .Select(year => GetDaysInFirstWeek(year, calendar))
                                          .Min();
                Assert.AreEqual(i, actualMin);
            }
        }

        private int GetDaysInFirstWeek(int year, GregorianCalendarSystem calendar)
        {
            // Some of the first few days of the week year may be in the previous week year.
            // However, the whole of the first week of the week year definitely occurs
            // within the first 13 days of January.
            return Enumerable.Range(1, 13)
                             .Count(day => new LocalDate(year, 1, day, calendar).WeekOfWeekYear == 1);
        }
    }
}
