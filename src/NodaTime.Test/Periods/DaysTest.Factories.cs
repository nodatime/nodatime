#region Copyright and license information

// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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

using NodaTime.Partials;
using NodaTime.Periods;
using NodaTime.TimeZones;

using NUnit.Framework;

namespace NodaTime.Test.Periods
{
    partial class DaysTest
    {
        [Test]
        public void Create_UpToSeven_ReturnsCached()
        {
            Assert.AreSame(Days.Zero, Days.Create(0));
            Assert.AreSame(Days.One, Days.Create(1));
            Assert.AreSame(Days.Two, Days.Create(2));
            Assert.AreSame(Days.Three, Days.Create(3));
            Assert.AreSame(Days.Four, Days.Create(4));
            Assert.AreSame(Days.Five, Days.Create(5));
            Assert.AreSame(Days.Six, Days.Create(6));
            Assert.AreSame(Days.Seven, Days.Create(7));
            Assert.AreSame(Days.MaxValue, Days.Create(Int32.MaxValue));
            Assert.AreSame(Days.MinValue, Days.Create(Int32.MinValue));
        }

        [Test]
        public void Create_GreaterThanSeven()
        {
            Assert.AreEqual(8, Days.Create(8).Value);
        }

        [Test]
        public void Create_NegativeValues()
        {
            Assert.AreEqual(-1, Days.Create(-1).Value);
        }

        [Test]
        [Ignore("Not enough code")]
        public void Between_ZonedDateTime()
        {
            var Paris = DateTimeZones.ForId("Europe/Paris");
            ZonedDateTime start = new ZonedDateTime(2006, 6, 9, 12, 0, 0, 0, Paris);
            ZonedDateTime end1 = new ZonedDateTime(2006, 6, 12, 12, 0, 0, 0, Paris);
            ZonedDateTime end2 = new ZonedDateTime(2006, 6, 15, 18, 0, 0, 0, Paris);

            Assert.AreEqual(3, Days.Between(start, end1).Value);
            Assert.AreEqual(0, Days.Between(start, start).Value);
            Assert.AreEqual(0, Days.Between(end1, end1).Value);
            Assert.AreEqual((-3), Days.Between(end1, start).Value);
            Assert.AreEqual(6, Days.Between(start, end2).Value);
        }

        [Test]
        [Ignore("Not enough code")]
        public void Between_Partial()
        {
            LocalDate start = new LocalDate(2006, 6, 9);
            LocalDate end1 = new LocalDate(2006, 6, 12);
            LocalDate end2 = new LocalDate(2006, 6, 15);

            Assert.AreEqual(3, Days.Between(start, end1).Value);
            Assert.AreEqual(0, Days.Between(start, start).Value);
            Assert.AreEqual(0, Days.Between(end1, end1).Value);
            Assert.AreEqual(-3, Days.Between(end1, start).Value);
            Assert.AreEqual(6, Days.Between(start, end2).Value);
        }

        [Test]
        [Ignore("Not enough code")]
        public void StandardDaysIn()
        {
            Assert.AreEqual(0, Days.StandardDaysIn(Period.Zero).Value);
            Assert.AreEqual(1, Days.StandardDaysIn(new Period(0, 0, 0, 1, 0, 0, 0, 0)).Value);
            Assert.AreEqual(123, Days.StandardDaysIn(Period.Days(123)).Value);
            Assert.AreEqual((-987), Days.StandardDaysIn(Period.Days(-987)).Value);
            Assert.AreEqual(1, Days.StandardDaysIn(Period.Hours(47)).Value);
            Assert.AreEqual(2, Days.StandardDaysIn(Period.Hours(48)).Value);
            Assert.AreEqual(2, Days.StandardDaysIn(Period.Hours(49)).Value);
            Assert.AreEqual(14, Days.StandardDaysIn(Period.Weeks(2)).Value);
        }

        [Test]
        [Ignore("Not enough code")]
        public void StandardDaysIn_ImpreciseFields_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Days.StandardDaysIn(Period.Months(1)));
        }

        [Test]
        [Ignore("Not enough code")]
        public void Parse()
        {
            Assert.AreEqual(0, Days.Parse(null).Value);
            Assert.AreEqual(0, Days.Parse("P0D").Value);
            Assert.AreEqual(1, Days.Parse("P1D").Value);
            Assert.AreEqual((-3), Days.Parse("P-3D").Value);
            Assert.AreEqual(2, Days.Parse("P0Y0M2D").Value);
            Assert.AreEqual(2, Days.Parse("P2DT0H0M").Value);
        }

        [Test]
        [Ignore("Not enough code")]
        public void Parse_WithImpreciseFields_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Days.Parse("P1Y1D"));
            Assert.Throws<ArgumentException>(() => Days.Parse("P1DT1H"));
        }
    }
}