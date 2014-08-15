// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class CalendarSystemTest
    {
        private static readonly CalendarSystem CopticCalendar = CalendarSystem.Coptic;

        // Tests using CopticCalendar as a simple example which doesn't override anything.
        [Test]
        public void GetAbsoluteYear()
        {
            Assert.AreEqual(5, CopticCalendar.GetAbsoluteYear(5, Era.AnnoMartyrum));
            // Prove it's right...
            LocalDate localDate = new LocalDate(5, 1, 1, CopticCalendar);
            Assert.AreEqual(5, localDate.Year);
            Assert.AreEqual(5, localDate.YearOfEra);
            Assert.AreEqual(Era.AnnoMartyrum, localDate.Era);
        }

        [Test]
        public void GetMinYearOfEra()
        {
            Assert.AreEqual(1, CopticCalendar.GetMinYearOfEra(Era.AnnoMartyrum));
        }

        [Test]
        public void GetMaxYearOfEra()
        {
            Assert.AreEqual(CopticCalendar.MaxYear, CopticCalendar.GetMaxYearOfEra(Era.AnnoMartyrum));
        }
    }
}
