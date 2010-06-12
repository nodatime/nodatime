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

using System;
using NodaTime.Calendars;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class ChronologyTest
    {
        private static readonly IDateTimeZone testZone = new FixedDateTimeZone("tmp", Offset.Zero);

        [Test]
        public void Construction_WithNullArguments_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Chronology(null, IsoCalendarSystem.Instance));
            Assert.Throws<ArgumentNullException>(() => new Chronology(DateTimeZones.Utc, null));
        }

        [Test]
        public void Properties_Return_ConstructorArguments()
        {
            Chronology subject = new Chronology(testZone, GregorianCalendarSystem.Default);
            Assert.AreSame(testZone, subject.Zone);
            Assert.AreSame(GregorianCalendarSystem.Default, subject.Calendar);
        }

        [Test]
        public void IsoForZone()
        {
            Chronology subject = Chronology.IsoForZone(testZone);
            Assert.AreSame(testZone, subject.Zone);
            Assert.AreSame(IsoCalendarSystem.Instance, subject.Calendar);
        }

        [Test]
        public void IsoForZone_WithNullZone_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Chronology.IsoForZone(null));
        }

        [Test]
        public void IsoUtc()
        {
            Assert.AreSame(DateTimeZones.Utc, Chronology.IsoUtc.Zone);
            Assert.AreSame(IsoCalendarSystem.Instance, Chronology.IsoUtc.Calendar);
        }
    }
}