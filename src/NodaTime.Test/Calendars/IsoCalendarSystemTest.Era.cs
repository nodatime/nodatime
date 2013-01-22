// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using NodaTime.Calendars;

namespace NodaTime.Test.Calendars
{
    public partial class IsoCalendarSystemTest
    {
        [Test]
        public void GetMaxYearOfEra()
        {
            LocalDate date = new LocalDate(Iso.MaxYear, 1, 1);
            Assert.AreEqual(date.YearOfEra, Iso.GetMaxYearOfEra(Era.Common));
            Assert.AreEqual(Era.Common, date.Era);
            date = new LocalDate(Iso.MinYear, 1, 1);
            Assert.AreEqual(Iso.MinYear, date.Year);
            Assert.AreEqual(date.YearOfEra, Iso.GetMaxYearOfEra(Era.BeforeCommon));
            Assert.AreEqual(Era.BeforeCommon, date.Era);
        }

        [Test]
        public void GetMinYearOfEra()
        {
            LocalDate date = new LocalDate(1, 1, 1);
            Assert.AreEqual(date.YearOfEra, Iso.GetMinYearOfEra(Era.Common));
            Assert.AreEqual(Era.Common, date.Era);
            date = new LocalDate(0, 1, 1);
            Assert.AreEqual(date.YearOfEra, Iso.GetMinYearOfEra(Era.BeforeCommon));
            Assert.AreEqual(Era.BeforeCommon, date.Era);
        }

        [Test]
        public void GetAbsoluteYear()
        {
            Assert.AreEqual(1, Iso.GetAbsoluteYear(1, Era.Common));
            Assert.AreEqual(0, Iso.GetAbsoluteYear(1, Era.BeforeCommon));
            Assert.AreEqual(-1, Iso.GetAbsoluteYear(2, Era.BeforeCommon));
            Assert.AreEqual(Iso.MaxYear, Iso.GetAbsoluteYear(Iso.GetMaxYearOfEra(Era.Common), Era.Common));
            Assert.AreEqual(Iso.MinYear, Iso.GetAbsoluteYear(Iso.GetMaxYearOfEra(Era.BeforeCommon), Era.BeforeCommon));
        }
    }
}
