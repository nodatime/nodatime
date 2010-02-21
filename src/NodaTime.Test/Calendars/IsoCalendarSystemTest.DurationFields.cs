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

namespace NodaTime.Test.Calendars
{
    public partial class IsoCalendarSystemTest
    {
        [Test]
        public void DurationFields_Eras()
        {
            var sut = isoFields.Eras;

            Assert.That(sut.ToString(), Is.EqualTo("Eras"));
            Assert.That(sut.IsSupported, Is.False);
        }

        [Test]
        public void DurationFields_Centuries()
        {
            var sut = isoFields.Centuries;

            Assert.That(sut.ToString(), Is.EqualTo("Centuries"));
            Assert.That(sut.IsSupported, Is.True);
            Assert.That(sut.IsPrecise, Is.False);
        }

        [Test]
        public void DurationFields_Years()
        {
            var sut = isoFields.Years;

            Assert.That(sut.ToString(), Is.EqualTo("Years"));
            Assert.That(sut.IsSupported, Is.True);
            Assert.That(sut.IsPrecise, Is.False);
        }

        [Test]
        public void DurationFields_WeekYears()
        {
            var sut = isoFields.WeekYears;

            Assert.That(sut.ToString(), Is.EqualTo("WeekYears"));
            Assert.That(sut.IsSupported, Is.True);
            Assert.That(sut.IsPrecise, Is.False);
        }

        [Test]
        public void DurationFields_Months()
        {
            var sut = isoFields.Months;

            Assert.That(sut.ToString(), Is.EqualTo("Months"));
            Assert.That(sut.IsSupported, Is.True);
            Assert.That(sut.IsPrecise, Is.False);
        }

        [Test]
        public void DurationFields_Weeks()
        {
            var sut = isoFields.Weeks;

            Assert.That(sut.ToString(), Is.EqualTo("Weeks"));
            Assert.That(sut.IsSupported, Is.True);
            Assert.That(sut.IsPrecise, Is.True);
        }

        [Test]
        public void DurationFields_Days()
        {
            var sut = isoFields.Days;

            Assert.That(sut.ToString(), Is.EqualTo("Days"));
            Assert.That(sut.IsSupported, Is.True);
            Assert.That(sut.IsPrecise, Is.True);
        }

        [Test]
        public void DurationFields_Hours()
        {
            var sut = isoFields.Hours;

            Assert.That(sut.ToString(), Is.EqualTo("Hours"));
            Assert.That(sut.IsSupported, Is.True);
            Assert.That(sut.IsPrecise, Is.True);
        }

        [Test]
        public void DurationFields_Minutes()
        {
            var sut = isoFields.Minutes;

            Assert.That(sut.ToString(), Is.EqualTo("Minutes"));
            Assert.That(sut.IsSupported, Is.True);
            Assert.That(sut.IsPrecise, Is.True);
        }

        [Test]
        public void DurationFields_Seconds()
        {
            var sut = isoFields.Seconds;

            Assert.That(sut.ToString(), Is.EqualTo("Seconds"));
            Assert.That(sut.IsSupported, Is.True);
            Assert.That(sut.IsPrecise, Is.True);
        }

        [Test]
        public void DurationFields_Milliseconds()
        {
            var sut = isoFields.Milliseconds;

            Assert.That(sut.ToString(), Is.EqualTo("Milliseconds"));
            Assert.That(sut.IsSupported, Is.True);
            Assert.That(sut.IsPrecise, Is.True);
        }

        [Test]
        public void DurationFields_Ticks()
        {
            var sut = isoFields.Ticks;

            Assert.That(sut.ToString(), Is.EqualTo("Ticks"));
            Assert.That(sut.IsSupported, Is.True);
            Assert.That(sut.IsPrecise, Is.True);
        }

    }
}
