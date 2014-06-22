// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Linq;
using NodaTime.Testing.TimeZones;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.Testing.TimeZones
{
    // Test as much to demonstrate a couple of extremes of building as much as anything else...
    [TestFixture]
    public class MultiTransitionDateTimeZoneTest
    {
        [Test]
        public void SimpleBuilding()
        {
            var transition1 = Instant.FromTicksSinceUnixEpoch(0L);
            var transition2 = Instant.FromTicksSinceUnixEpoch(100000L);
            var zone = new MultiTransitionDateTimeZone.Builder
            {
                { transition1, 5 },
                { transition2, 3 }
            }.Build();
            var intervals = zone.GetZoneIntervals(transition1 - Duration.Epsilon, transition2 + Duration.Epsilon).ToList();
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
            var transition1 = Instant.FromTicksSinceUnixEpoch(0L);
            var transition2 = Instant.FromTicksSinceUnixEpoch(100000L);
            var zone = new MultiTransitionDateTimeZone.Builder(2, 1, "X")
            {
                { transition1, 2, 0, "Y" },
                { transition2, 1, 1, "Z" }
            }.Build();
            var actual = zone.GetZoneIntervals(transition1 - Duration.Epsilon, transition2 + Duration.Epsilon).ToList();
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
