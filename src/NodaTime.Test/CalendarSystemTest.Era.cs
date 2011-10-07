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

namespace NodaTime.Test
{
    public partial class CalendarSystemTest
    {
        private static readonly CalendarSystem CopticCalendar = CalendarSystem.GetCopticCalendar(4);

        // Tests using CopticCalendar as a simple example which doesn't override anything.
        [Test]
        public void GetAbsoluteYear_NullEra()
        {
            Assert.Throws<ArgumentNullException>(() => CopticCalendar.GetAbsoluteYear(1, null));
        }

        [Test]
        public void GetAbsoluteYear_InvalidEra()
        {
            // Coptic calendar only has the AM era.
            Assert.Throws<ArgumentException>(() => CopticCalendar.GetAbsoluteYear(1, Era.Common));
        }

        [Test]
        public void GetAbsoluteYear_Valid()
        {
            Assert.AreEqual(5, CopticCalendar.GetAbsoluteYear(5, Era.AnnoMartyrm));
            // Prove it's right...
            LocalDate localDate = new LocalDate(5, 1, 1, CopticCalendar);
            Assert.AreEqual(5, localDate.Year);
            Assert.AreEqual(5, localDate.YearOfEra);
            Assert.AreEqual(Era.AnnoMartyrm, localDate.Era);
        }

        [Test]
        public void GetAbsoluteYear_YearTooBig()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CopticCalendar.GetAbsoluteYear(CopticCalendar.MaxYear + 1, Era.AnnoMartyrm));
        }

        [Test]
        public void GetAbsoluteYear_YearTooSmall()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CopticCalendar.GetAbsoluteYear(0, Era.AnnoMartyrm));
        }

        [Test]
        public void GetMinYearOfEra_NullEra()
        {
            Assert.Throws<ArgumentNullException>(() => CopticCalendar.GetMinYearOfEra(null));
        }

        [Test]
        public void GetMinYearOfEra_InvalidEra()
        {
            // Coptic calendar only has the AM era.
            Assert.Throws<ArgumentException>(() => CopticCalendar.GetMinYearOfEra(Era.Common));
        }

        [Test]
        public void GetMinYearOfEra_Valid()
        {
            Assert.AreEqual(1, CopticCalendar.GetMinYearOfEra(Era.AnnoMartyrm));
        }

        [Test]
        public void GetMaxYearOfEra_NullEra()
        {
            Assert.Throws<ArgumentNullException>(() => CopticCalendar.GetMaxYearOfEra(null));
        }

        [Test]
        public void GetMaxYearOfEra_InvalidEra()
        {
            // Coptic calendar only has the AM era.
            Assert.Throws<ArgumentException>(() => CopticCalendar.GetMaxYearOfEra(Era.Common));
        }

        [Test]
        public void GetMaxYearOfEra_Valid()
        {
            Assert.AreEqual(CopticCalendar.MaxYear, CopticCalendar.GetMaxYearOfEra(Era.AnnoMartyrm));
        }
    }
}
