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
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class FixedDateTimeZoneTest
    {
        [Test]
        public void SimpleProperties_ReturnValuesFromConstructor()
        {
            Duration offset = new Duration(1000);
            FixedDateTimeZone zone = new FixedDateTimeZone("test", offset);
            Assert.AreEqual("test", zone.Id);
            // TODO: Use a real LocalDateTime when we've implemented it!
            Assert.AreEqual(offset, zone.GetOffsetFromLocal(new LocalDateTime()));
            Assert.AreEqual(offset, zone.GetOffsetFromUtc(Instant.UnixEpoch));
        }

        [Test]
        public void IsFixed_ReturnsTrue()
        {
            FixedDateTimeZone zone = new FixedDateTimeZone("test", new Duration(1000));
            Assert.IsTrue(zone.IsFixed);
        }

        [Test]
        public void NextTransition_ReturnsNull()
        {
            FixedDateTimeZone zone = new FixedDateTimeZone("test", new Duration(1000));
            Assert.IsNull(zone.NextTransition(Instant.UnixEpoch));
        }

        [Test]
        public void PreviousTransition_ReturnsNull()
        {
            FixedDateTimeZone zone = new FixedDateTimeZone("test", new Duration(1000));
            Assert.IsNull(zone.PreviousTransition(Instant.UnixEpoch));
        }
    }
}
