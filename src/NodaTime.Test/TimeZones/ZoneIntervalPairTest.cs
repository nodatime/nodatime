// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class ZoneIntervalPairTest
    {
        [Test]
        public void MatchingIntervals_SingleInterval()
        {
            ZoneIntervalPair pair = ZoneIntervalPair.Unambiguous(new ZoneInterval("Foo", Instant.FromTicksSinceUnixEpoch(0), Instant.FromTicksSinceUnixEpoch(10), Offset.Zero, Offset.Zero));
            Assert.AreEqual(1, pair.MatchingIntervals);
        }

        [Test]
        public void MatchingIntervals_NoIntervals()
        {
            ZoneIntervalPair pair = ZoneIntervalPair.NoMatch;
            Assert.AreEqual(0, pair.MatchingIntervals);
        }

        [Test]
        public void MatchingIntervals_TwoIntervals()
        {
            ZoneIntervalPair pair = ZoneIntervalPair.Ambiguous(
                new ZoneInterval("Foo", Instant.FromTicksSinceUnixEpoch(0), Instant.FromTicksSinceUnixEpoch(10), Offset.Zero, Offset.Zero),
                new ZoneInterval("Bar", Instant.FromTicksSinceUnixEpoch(10), Instant.FromTicksSinceUnixEpoch(20), Offset.Zero, Offset.Zero));
            Assert.AreEqual(2, pair.MatchingIntervals);
        }
    }
}
