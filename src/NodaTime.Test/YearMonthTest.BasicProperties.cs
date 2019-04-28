// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NodaTime.Test
{
    public partial class YearMonthTest
    {
        [Test]
        public void Properties()
        {
            // Some of these are covered in other tests too, but it doesn't hurt
            // to repeat ourselves here.
            var yearMonth = new YearMonth(2000, 1);
            Assert.AreEqual(2000, yearMonth.Year);
            Assert.AreEqual(1, yearMonth.Month);
            Assert.AreEqual(CalendarSystem.Iso, yearMonth.Calendar);
            Assert.AreEqual(2000, yearMonth.YearOfEra);
            Assert.AreEqual(Era.Common, yearMonth.Era);

            Assert.AreEqual(new LocalDate(2000, 1, 1), yearMonth.StartDate);
            Assert.AreEqual(new LocalDate(2000, 1, 31), yearMonth.EndDate);
        }

        [Test]
        public void ToDateInterval()
        {
            var yearMonth = new YearMonth(2000, 1);

            var interval = new DateInterval(new LocalDate(2000, 1, 1), new LocalDate(2000, 1, 31));
            Assert.AreEqual(interval, yearMonth.ToDateInterval());
        }
    }
}
