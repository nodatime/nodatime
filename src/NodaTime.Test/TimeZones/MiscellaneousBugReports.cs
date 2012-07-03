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

using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Fixture for miscellaneous bug reports and oddities which don't really fit anywhere else.
    /// Quite often the cause of a problem is nowhere near the test code; it's still useful
    /// to have the original test which showed up the problem, as a small contribution
    /// to regression testing.
    /// </summary>
    [TestFixture]
    public class MiscellaneousBugReports
    {
        [Test]
        public void Niue()
        {
            DateTimeZone niue = DateTimeZoneFactory.Tzdb["Pacific/Niue"];
            var offset = niue.GetOffsetFromUtc(niue.AtStrictly(new LocalDateTime(2010, 1, 1, 0, 0, 0)).ToInstant());
            Assert.AreEqual(Offset.FromHours(-11), offset);
        }

        [Test]
        public void Kiritimati()
        {
            DateTimeZone kiritimati = DateTimeZoneFactory.Tzdb["Pacific/Kiritimati"];
            var offset = kiritimati.GetOffsetFromUtc(kiritimati.AtStrictly(new LocalDateTime(2010, 1, 1, 0, 0, 0)).ToInstant());
            Assert.AreEqual(Offset.FromHours(14), offset);
        }

        [Test]
        public void Pyongyang()
        {
            DateTimeZone pyongyang = DateTimeZoneFactory.Tzdb["Asia/Pyongyang"];
            var offset = pyongyang.GetOffsetFromUtc(pyongyang.AtStrictly(new LocalDateTime(2010, 1, 1, 0, 0, 0)).ToInstant());
            Assert.AreEqual(Offset.FromHours(9), offset);
        }

        [Test]
        public void Khartoum()
        {
            DateTimeZone khartoum = DateTimeZoneFactory.Tzdb["Africa/Khartoum"];
            Assert.IsNotNull(khartoum);
            Instant utc = Instant.FromUtc(2000, 1, 1, 0, 0, 0);
            ZonedDateTime inKhartoum = new ZonedDateTime(utc, khartoum);
            LocalDateTime expectedLocal = new LocalDateTime(2000, 1, 1, 2, 0);
            Assert.AreEqual(expectedLocal, inKhartoum.LocalDateTime);

            // Khartoum changed from +2 to +3 on January 15th 2000
            utc = Instant.FromUtc(2000, 1, 16, 0, 0, 0);
            inKhartoum = new ZonedDateTime(utc, khartoum);
            expectedLocal = new LocalDateTime(2000, 1, 16, 3, 0);
            Assert.AreEqual(expectedLocal, inKhartoum.LocalDateTime);
        }

        /// <summary>
        /// Tbilisi used daylight saving time for winter 1996/1997 too.
        /// </summary>
        [Test]
        public void Tbilisi()
        {
            var zone = DateTimeZoneFactory.Tzdb["Asia/Tbilisi"];
            Instant summer1996 = Instant.FromUtc(1996, 6, 1, 0, 0);
            var interval = zone.GetZoneInterval(summer1996);
            Assert.AreEqual(new LocalDateTime(1996, 3, 31, 1, 0), interval.IsoLocalStart);
            Assert.AreEqual(new LocalDateTime(1997, 10, 26, 0, 0), interval.IsoLocalEnd);
        }
    }
}