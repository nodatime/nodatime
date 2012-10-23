#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
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
using System.Linq;
using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class BclDateTimeZoneSourceTest
    {
        [Test]
        public void AllZonesMapToTheirId()
        {
            BclDateTimeZoneSource source = new BclDateTimeZoneSource();
            foreach (var zone in TimeZoneInfo.GetSystemTimeZones())
            {
                Assert.AreEqual(zone.Id, source.MapTimeZoneId(zone));
            }
        }

        [Test]
        public void UtcDoesNotEqualBuiltIn()
        {
            var zone = new BclDateTimeZoneSource().ForId("UTC");
            Assert.AreNotEqual(DateTimeZone.Utc, zone);
        }

        [Test]
        public void FixedOffsetDoesNotEqualBuiltIn()
        {
            // Only a few fixed zones are advertised by Windows. We happen to know this one
            // is wherever we run tests :)
            // Unfortunately, it doesn't always exist on Mono (at least not on the Raspberry Pi...)
            string id = "UTC-02";
            var source = new BclDateTimeZoneSource();
            if (!source.GetIds().Contains(id)) {
                Assert.Ignore("Test assumes existence of BCL zone with ID: " + id);
            }
            var zone = source.ForId(id);
            Assert.AreNotEqual(DateTimeZone.ForOffset(Offset.FromHours(-2)), zone);
            Assert.AreEqual(id, zone.Id);
            Assert.AreEqual(Offset.FromHours(-2), zone.GetZoneInterval(NodaConstants.UnixEpoch).WallOffset);
        }
    }
}
