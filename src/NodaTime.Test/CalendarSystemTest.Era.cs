// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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
