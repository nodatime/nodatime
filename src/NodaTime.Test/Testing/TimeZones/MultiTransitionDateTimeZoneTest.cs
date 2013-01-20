#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2013 Jon Skeet
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NodaTime.Testing.TimeZones;
using NodaTime.TimeZones;

namespace NodaTime.Test.Testing.TimeZones
{
    // Test as much to demonstrate a couple of extremes of building as much as anything else...
    [TestFixture]
    public class MultiTransitionDateTimeZoneTest
    {
        [Test]
        public void SimpleBuilding()
        {
            var transition1 = new Instant(0L);
            var transition2 = new Instant(100000L);
            var zone = new MultiTransitionDateTimeZone.Builder
            {
                { transition1, 5 },
                { transition2, 3 }
            }.Build();
            var intervals = zone.GetAllZoneIntervals(transition1 - Duration.Epsilon, transition2 + Duration.Epsilon).ToList();
            Assert.AreEqual(3, intervals.Count);
            Assert.AreEqual(Offset.Zero, intervals[0].WallOffset);
            Assert.AreEqual(Instant.MinValue, intervals[0].Start);
            Assert.AreEqual(transition1, intervals[0].End);

            Assert.AreEqual(Offset.FromHours(5), intervals[1].WallOffset);
            Assert.AreEqual(transition1, intervals[1].Start);
            Assert.AreEqual(transition2, intervals[1].End);

            Assert.AreEqual(Offset.FromHours(3), intervals[2].WallOffset);
            Assert.AreEqual(transition2, intervals[2].Start);
            Assert.AreEqual(Instant.MaxValue, intervals[2].End);
        }

        [Test]
        public void ComplexBuilding()
        {
            var transition1 = new Instant(0L);
            var transition2 = new Instant(100000L);
            var zone = new MultiTransitionDateTimeZone.Builder(2, 1, "X")
            {
                { transition1, 2, 0, "Y" },
                { transition2, 1, 1, "Z" }
            }.Build();
            var actual = zone.GetAllZoneIntervals(transition1 - Duration.Epsilon, transition2 + Duration.Epsilon).ToList();
            // ZoneInterval uses wall offset and savings...
            var expected = new[]
            {
                new ZoneInterval("X", Instant.MinValue, transition1, Offset.FromHours(3), Offset.FromHours(1)),
                new ZoneInterval("Y", transition1, transition2, Offset.FromHours(2), Offset.FromHours(0)),
                new ZoneInterval("Z", transition2, Instant.MaxValue, Offset.FromHours(2), Offset.FromHours(1)),
            };

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
