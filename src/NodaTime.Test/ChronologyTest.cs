#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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
using NUnit.Framework;
using NodaTime.Calendars;
using NodaTime.TimeZones;

namespace NodaTime.Test
{
    [TestFixture]
    public class ChronologyTest
    {
        private static readonly DateTimeZone TestZone = new FixedDateTimeZone("tmp", Offset.Zero);

        [Test]
        public void Construction_WithNullArguments_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Chronology(null, CalendarSystem.Iso));
            Assert.Throws<ArgumentNullException>(() => new Chronology(DateTimeZone.Utc, null));
        }

        [Test]
        public void Properties_Return_ConstructorArguments()
        {
            GregorianCalendarSystem calendar = GregorianCalendarSystem.GetInstance(4);
            var subject = new Chronology(TestZone, calendar);
            Assert.AreSame(TestZone, subject.Zone);
            Assert.AreSame(calendar, subject.Calendar);
        }

        [Test]
        public void IsoUtc()
        {
            Assert.AreSame(DateTimeZone.Utc, Chronology.IsoUtc.Zone);
            Assert.AreSame(CalendarSystem.Iso, Chronology.IsoUtc.Calendar);
        }
    }
}