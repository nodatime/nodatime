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

using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class IsoCalendarSystemTest
    {
        [Test]
        public void DateFields_Era()
        {
            var sut = isoFields.Era;

            Assert.That(sut.ToString(), Is.EqualTo("Era"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void DateFields_CenturyOfEra()
        {
            var sut = isoFields.CenturyOfEra;

            Assert.That(sut.ToString(), Is.EqualTo("CenturyOfEra"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void DateFields_YearOfCentury()
        {
            var sut = isoFields.YearOfCentury;

            Assert.That(sut.ToString(), Is.EqualTo("YearOfCentury"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void DateFields_YearOfEra()
        {
            var sut = isoFields.YearOfEra;

            Assert.That(sut.ToString(), Is.EqualTo("YearOfEra"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void DateFields_Year()
        {
            var sut = isoFields.Year;

            Assert.That(sut.ToString(), Is.EqualTo("Year"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void DateFields_MonthOfYear()
        {
            var sut = isoFields.MonthOfYear;

            Assert.That(sut.ToString(), Is.EqualTo("MonthOfYear"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void DateFields_WeekYearOfCentury()
        {
            var sut = isoFields.WeekYearOfCentury;

            Assert.That(sut.ToString(), Is.EqualTo("WeekYearOfCentury"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void DateFields_WeekYear()
        {
            var sut = isoFields.WeekYear;

            Assert.That(sut.ToString(), Is.EqualTo("WeekYear"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void DateFields_WeekOfWeekYear()
        {
            var sut = isoFields.WeekOfWeekYear;

            Assert.That(sut.ToString(), Is.EqualTo("WeekOfWeekYear"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void DateFields_DayOfYear()
        {
            var sut = isoFields.DayOfYear;

            Assert.That(sut.ToString(), Is.EqualTo("DayOfYear"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void DateFields_DayOfMonth()
        {
            var sut = isoFields.DayOfMonth;

            Assert.That(sut.ToString(), Is.EqualTo("DayOfMonth"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void DateFields_DayOfWeek()
        {
            var sut = isoFields.DayOfWeek;

            Assert.That(sut.ToString(), Is.EqualTo("DayOfWeek"));
            Assert.That(sut.IsSupported, Is.True);
        }

    }
}
