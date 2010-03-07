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

namespace NodaTime.Test.Format
{
    public partial class DateTimeFormatterTest
    {
        #region WithProvider

        [Test]
        public void WithProvider_CreatesNewInstance_ForDifferentProvider()
        {
            var sutWithProvider = fullFormatterWithOffset.WithProvider(provider2);

            Assert.That(sutWithProvider, Is.Not.SameAs(fullFormatterWithOffset));
            Assert.That(sutWithProvider.Provider, Is.SameAs(provider2));

            Assert.That(sutWithProvider.CalendarSystem, Is.SameAs(fullFormatterWithOffset.CalendarSystem));
            Assert.That(sutWithProvider.Zone, Is.SameAs(fullFormatterWithOffset.Zone));
            Assert.That(sutWithProvider.PivotYear, Is.EqualTo(fullFormatterWithOffset.PivotYear));
            Assert.That(sutWithProvider.IsOffsetParsed, Is.EqualTo(fullFormatterWithOffset.IsOffsetParsed));
        }

        [Test]
        public void WithProvider_ReturnsTheSameInstance_ForTheSameProvider()
        {
            var sutWithProvider = fullFormatterWithOffset.WithProvider(provider1);

            Assert.That(sutWithProvider, Is.SameAs(fullFormatterWithOffset));
        }

        #endregion

        #region WithOffsetParsed

        [Test]
        public void WithOffsetParsed_CreatesNewInstance_ForDifferentProvider()
        {
            var sutWithOffset = fullFormatterWithoutOffset.WithOffsetParsed();

            Assert.That(sutWithOffset, Is.Not.SameAs(fullFormatterWithoutOffset));
            Assert.That(sutWithOffset.IsOffsetParsed, Is.True);

            Assert.That(sutWithOffset.CalendarSystem, Is.SameAs(fullFormatterWithoutOffset.CalendarSystem));
            Assert.That(sutWithOffset.Zone, Is.SameAs(fullFormatterWithoutOffset.Zone));
            Assert.That(sutWithOffset.PivotYear, Is.EqualTo(fullFormatterWithoutOffset.PivotYear));
            Assert.That(sutWithOffset.Provider, Is.SameAs(fullFormatterWithoutOffset.Provider));

        }

        [Test]
        public void WithOffsetParsed_ReturnsTheSameInstance_IfAlreadySet()
        {
            var sutWithOffset = fullFormatterWithOffset.WithOffsetParsed();

            Assert.That(sutWithOffset, Is.SameAs(fullFormatterWithOffset));
        }

        #endregion

        #region WithCalendarSystem

        [Test]
        public void WithCalendarSystem_CreatesNewInstance_ForDifferentCalendarSystem()
        {
            var sutWithCalendar = fullFormatterWithOffset.WithCalendarSystem(calendar2);

            Assert.That(sutWithCalendar, Is.Not.SameAs(fullFormatterWithOffset));
            Assert.That(sutWithCalendar.CalendarSystem, Is.SameAs(calendar2));

            Assert.That(sutWithCalendar.Provider, Is.SameAs(fullFormatterWithOffset.Provider));
            Assert.That(sutWithCalendar.Zone, Is.SameAs(fullFormatterWithOffset.Zone));
            Assert.That(sutWithCalendar.PivotYear, Is.EqualTo(fullFormatterWithOffset.PivotYear));
            Assert.That(sutWithCalendar.IsOffsetParsed, Is.EqualTo(fullFormatterWithOffset.IsOffsetParsed));
        }

        [Test]
        public void WithCalendarSystem_ReturnsTheSameInstance_ForTheSameCalendarSystem()
        {
            var sutWithCalendar = fullFormatterWithOffset.WithCalendarSystem(calendar1);

            Assert.That(sutWithCalendar, Is.SameAs(fullFormatterWithOffset));
        }

        #endregion

        #region WithZone

        [Test]
        public void WithZone_CreatesNewInstance_ForDifferentZone()
        {
            var sutWithZone = fullFormatterWithOffset.WithZone(zone2);

            Assert.That(sutWithZone, Is.Not.SameAs(fullFormatterWithOffset));
            Assert.That(sutWithZone.Zone, Is.SameAs(zone2));

            Assert.That(sutWithZone.Provider, Is.SameAs(fullFormatterWithOffset.Provider));
            Assert.That(sutWithZone.CalendarSystem, Is.SameAs(fullFormatterWithOffset.CalendarSystem));
            Assert.That(sutWithZone.PivotYear, Is.EqualTo(fullFormatterWithOffset.PivotYear));
            Assert.That(sutWithZone.IsOffsetParsed, Is.EqualTo(fullFormatterWithOffset.IsOffsetParsed));
        }

        [Test]
        public void WithZone_ReturnsTheSameInstance_ForTheSameZone()
        {
            var sutWithZone = fullFormatterWithOffset.WithZone(zone1);

            Assert.That(sutWithZone, Is.SameAs(fullFormatterWithOffset));
        }

        #endregion

        #region WithPivotYear

        [Test]
        public void WithPivotYear_CreatesNewInstance_ForDifferentPivotYear()
        {
            var sutWithPivotYear = fullFormatterWithOffset.WithPivotYear(pivotYear2);

            Assert.That(sutWithPivotYear, Is.Not.SameAs(fullFormatterWithOffset));
            Assert.That(sutWithPivotYear.PivotYear, Is.EqualTo(pivotYear2));

            Assert.That(sutWithPivotYear.Provider, Is.SameAs(fullFormatterWithOffset.Provider));
            Assert.That(sutWithPivotYear.CalendarSystem, Is.SameAs(fullFormatterWithOffset.CalendarSystem));
            Assert.That(sutWithPivotYear.Zone, Is.SameAs(fullFormatterWithOffset.Zone));
            Assert.That(sutWithPivotYear.IsOffsetParsed, Is.EqualTo(fullFormatterWithOffset.IsOffsetParsed));
        }

        [Test]
        public void WithPivotYear_ReturnsTheSameInstance_ForTheSamePivotYear()
        {
            var sutWithPivotYear = fullFormatterWithOffset.WithPivotYear(pivotYear1);

            Assert.That(sutWithPivotYear, Is.SameAs(fullFormatterWithOffset));
        }

        #endregion
    }
}
