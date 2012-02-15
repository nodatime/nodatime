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
using NUnit.Framework;
using NodaTime.Calendars;

namespace NodaTime.Test
{
    public partial class LocalDateTest
    {
        [Test]
        public void Constructor_CalendarDefaultsToIso()
        {
            LocalDate date = new LocalDate(2000, 1, 1);
            Assert.AreEqual(CalendarSystem.Iso, date.Calendar);
        }

        [Test]
        public void Constructor_PropertiesRoundTrip()
        {
            LocalDate date = new LocalDate(2023, 7, 27);
            Assert.AreEqual(2023, date.Year);
            Assert.AreEqual(7, date.MonthOfYear);
            Assert.AreEqual(27, date.DayOfMonth);
        }

        [Test]
        public void Constructor_PropertiesRoundTrip_CustomCalendar()
        {
            LocalDate date = new LocalDate(2023, 7, 27, CalendarSystem.GetJulianCalendar(4));
            Assert.AreEqual(2023, date.Year);
            Assert.AreEqual(7, date.MonthOfYear);
            Assert.AreEqual(27, date.DayOfMonth);
        }

        [Test]
        public void Constructor_InvalidMonth()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(2010, 13, 1));
        }

        [Test]
        public void Constructor_InvalidDay()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(2010, 1, 100));
        }

        [Test]
        public void Constructor_InvalidDayWithinMonth()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(2010, 2, 30));
        }

        [Test]
        public void Constructor_InvalidYear()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(CalendarSystem.Iso.MaxYear + 1, 1, 1));
        }

        [Test]
        public void Constructor_WithYearOfEra_BC()
        {
            LocalDate absolute = new LocalDate(-10, 1, 1);
            LocalDate withEra = new LocalDate(Era.BeforeCommon, 11, 1, 1);
            Assert.AreEqual(absolute, withEra);
        }

        [Test]
        public void Constructor_WithYearOfEra_AD()
        {
            LocalDate absolute = new LocalDate(50, 6, 19);
            LocalDate withEra = new LocalDate(Era.Common, 50, 6, 19);
            Assert.AreEqual(absolute, withEra);
        }

        [Test]
        public void Constructor_WithYearOfEra_NonIsoCalendar()
        {
            var calendar = CalendarSystem.GetCopticCalendar(4);
            LocalDate absolute = new LocalDate(50, 6, 19, calendar);
            LocalDate withEra = new LocalDate(Era.AnnoMartyrm, 50, 6, 19, calendar);
            Assert.AreEqual(absolute, withEra);
        }
    }
}
