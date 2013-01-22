// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using NodaTime.Calendars;

namespace NodaTime.Test.Calendars
{
    public partial class JulianCalendarSystemTest
    {
        [Test]
        public void GetMaxYearOfEra()
        {
            LocalDate date = new LocalDate(Julian.MaxYear, 1, 1, Julian);
            Assert.AreEqual(date.YearOfEra, Julian.GetMaxYearOfEra(Era.Common));
            Assert.AreEqual(Era.Common, date.Era);
            date = new LocalDate(Julian.MinYear, 1, 1, Julian);
            Assert.AreEqual(Julian.MinYear, date.Year);
            Assert.AreEqual(date.YearOfEra, Julian.GetMaxYearOfEra(Era.BeforeCommon));
            Assert.AreEqual(Era.BeforeCommon, date.Era);
        }

        [Test]
        public void GetMinYearOfEra()
        {
            LocalDate date = new LocalDate(1, 1, 1, Julian);
            Assert.AreEqual(date.YearOfEra, Julian.GetMinYearOfEra(Era.Common));
            Assert.AreEqual(Era.Common, date.Era);
            date = new LocalDate(0, 1, 1, Julian);
            Assert.AreEqual(date.YearOfEra, Julian.GetMinYearOfEra(Era.BeforeCommon));
            Assert.AreEqual(Era.BeforeCommon, date.Era);
        }

        [Test]
        public void GetAbsoluteYear()
        {
            Assert.AreEqual(1, Julian.GetAbsoluteYear(1, Era.Common));
            Assert.AreEqual(0, Julian.GetAbsoluteYear(1, Era.BeforeCommon));
            Assert.AreEqual(-1, Julian.GetAbsoluteYear(2, Era.BeforeCommon));
            Assert.AreEqual(Julian.MaxYear, Julian.GetAbsoluteYear(Julian.GetMaxYearOfEra(Era.Common), Era.Common));
            Assert.AreEqual(Julian.MinYear, Julian.GetAbsoluteYear(Julian.GetMaxYearOfEra(Era.BeforeCommon), Era.BeforeCommon));
        }
    }
}
