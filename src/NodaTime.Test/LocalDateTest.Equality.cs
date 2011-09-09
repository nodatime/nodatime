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

using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalDateTest
    {
        [Test]
        public void Equals_EqualValues()
        {
            CalendarSystem calendar = CalendarSystem.GetJulianCalendar(4);
            LocalDate date1 = new LocalDate(2011, 1, 2, calendar);
            LocalDate date2 = new LocalDate(2011, 1, 2, calendar);
            Assert.AreEqual(date1, date2);
            Assert.AreEqual(date1.GetHashCode(), date2.GetHashCode());
            Assert.IsTrue(date1 == date2);
            Assert.IsFalse(date1 != date2);
            Assert.IsTrue(date1.Equals(date2)); // IEquatable implementation
        }

        [Test]
        public void Equals_DifferentDates()
        {
            CalendarSystem calendar = CalendarSystem.GetJulianCalendar(4);
            LocalDate date1 = new LocalDate(2011, 1, 2, calendar);
            LocalDate date2 = new LocalDate(2011, 1, 3, calendar);
            Assert.AreNotEqual(date1, date2);
            Assert.AreNotEqual(date1.GetHashCode(), date2.GetHashCode());
            Assert.IsFalse(date1 == date2);
            Assert.IsTrue(date1 != date2);
            Assert.IsFalse(date1.Equals(date2)); // IEquatable implementation
        }

        [Test]
        public void Equals_DifferentCalendars()
        {
            CalendarSystem calendar = CalendarSystem.GetJulianCalendar(4);
            LocalDate date1 = new LocalDate(2011, 1, 2, calendar);
            LocalDate date2 = new LocalDate(2011, 1, 2, CalendarSystem.Iso);
            Assert.AreNotEqual(date1, date2);
            Assert.AreNotEqual(date1.GetHashCode(), date2.GetHashCode());
            Assert.IsFalse(date1 == date2);
            Assert.IsTrue(date1 != date2);
            Assert.IsFalse(date1.Equals(date2)); // IEquatable implementation
        }

        [Test]
        public void Equals_DifferentToNull()
        {
            LocalDate date = new LocalDate(2011, 1, 2);
            Assert.IsFalse(date.Equals(null));
        }

        [Test]
        public void Equals_DifferentToOtherType()
        {
            LocalDate date = new LocalDate(2011, 1, 2);
            Assert.IsFalse(date.Equals(new Instant(0)));
        }
    }
}
