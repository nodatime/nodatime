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
using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class BclTimeZoneSourceTest
    {
        [Test]
        public void AllZonesMapToTheirId()
        {
            BclTimeZoneSource source = new BclTimeZoneSource();
            foreach (var zone in TimeZoneInfo.GetSystemTimeZones())
            {
                Assert.AreEqual(zone.Id, source.MapTimeZoneId(zone));
            }
        }

        [Test]
        public void UtcMapping()
        {
            // Effectively check that we end up with a BclTimeZone when we use the UTC ID.
            DateTimeZoneCache provider = new DateTimeZoneCache(new BclTimeZoneSource());
            Assert.IsInstanceOf<BclDateTimeZone>(provider[DateTimeZone.UtcId]);
        }
    }
}
