// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class DaylightSavingDateTimeZoneTest
    {
        private static readonly ZoneRecurrence Winter = new ZoneRecurrence("Winter", Offset.Zero,
            new ZoneYearOffset(TransitionMode.Wall, 10, 5, 0, false, new LocalTime(2, 0)), 2000, int.MaxValue);

        private static readonly ZoneRecurrence Summer = new ZoneRecurrence("Summer", Offset.FromHours(1),
            new ZoneYearOffset(TransitionMode.Wall, 3, 10, 0, false, new LocalTime(1, 0)), 2000, int.MaxValue);

        /// <summary>
        /// Time zone with the following characteristics:
        /// - Only valid from March 10th 2000
        /// - Standard offset of +5 (so 4am UTC = 9am local)
        /// - Summer time (DST = 1 hour) always starts at 1am local time on March 10th (skips to 2am)
        /// - Winter time (DST = 0) always starts at 2am local time on October 5th (skips to 1am)
        /// </summary>
        private static readonly DaylightSavingsDateTimeZone TestZone = new DaylightSavingsDateTimeZone("Test",
            Offset.FromHours(5), Winter, Summer);

        [Test]
        public void MinMaxOffsets()
        {
            Assert.AreEqual(Offset.FromHours(6), TestZone.MaxOffset);
            Assert.AreEqual(Offset.FromHours(5), TestZone.MinOffset);
        }

        [Test]
        public void GetZoneInterval_Instant_Summer()
        {
            var interval = TestZone.GetZoneInterval(Instant.FromUtc(2010, 6, 1, 0, 0));
            Assert.AreEqual("Summer", interval.Name);
            Assert.AreEqual(Offset.FromHours(6), interval.WallOffset);
            Assert.AreEqual(Offset.FromHours(5), interval.StandardOffset);
            Assert.AreEqual(Offset.FromHours(1), interval.Savings);
            Assert.AreEqual(new LocalInstant(2010, 3, 10, 2, 0), interval.LocalStart);
            Assert.AreEqual(new LocalInstant(2010, 10, 5, 2, 0), interval.LocalEnd);
        }

        [Test]
        public void GetZoneInterval_Instant_Winter()
        {
            var interval = TestZone.GetZoneInterval(Instant.FromUtc(2010, 11, 1, 0, 0));
            Assert.AreEqual("Winter", interval.Name);
            Assert.AreEqual(Offset.FromHours(5), interval.WallOffset);
            Assert.AreEqual(Offset.FromHours(5), interval.StandardOffset);
            Assert.AreEqual(Offset.FromHours(0), interval.Savings);
            Assert.AreEqual(new LocalInstant(2010, 10, 5, 1, 0), interval.LocalStart);
            Assert.AreEqual(new LocalInstant(2011, 3, 10, 1, 0), interval.LocalEnd);
        }

        [Test]
        public void GetZoneInterval_Instant_StartOfFirstSummer()
        {
            // This is only just about valid
            var firstSummer = Instant.FromUtc(2000, 3, 9, 20, 0);
            var interval = TestZone.GetZoneInterval(firstSummer);
            Assert.AreEqual("Summer", interval.Name);
        }

        [Test]
        public void GetZoneIntervals_WithinFirstSummer()
        {
            var early = new LocalInstant(2000, 6, 1, 0, 0);
            var pair = TestZone.GetZoneIntervalPair(early);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_WithinFirstWinter()
        {
            var winter = new LocalInstant(2000, 12, 1, 0, 0);
            var pair = TestZone.GetZoneIntervalPair(winter);
            Assert.AreEqual("Winter", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_AtFirstGapStart()
        {
            var startOfFirstGap = new LocalInstant(2000, 3, 10, 1, 0);
            var actual = TestZone.GetZoneIntervalPair(startOfFirstGap);
            var expected = ZoneIntervalPair.NoMatch;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetZoneIntervals_WithinFirstGap()
        {
            var middleOfFirstGap = new LocalInstant(2000, 3, 10, 1, 30);
            var pair = TestZone.GetZoneIntervalPair(middleOfFirstGap);
            Assert.AreEqual(0, pair.MatchingIntervals);
        }

        [Test]
        public void GetZoneIntervals_EndOfFirstGap()
        {
            var endOfFirstGap = new LocalInstant(2000, 3, 10, 2, 0);
            var pair = TestZone.GetZoneIntervalPair(endOfFirstGap);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_StartOfFirstAmbiguity()
        {
            var firstAmbiguity = new LocalInstant(2000, 10, 5, 1, 0);
            var pair = TestZone.GetZoneIntervalPair(firstAmbiguity);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.AreEqual("Winter", pair.LateInterval.Name);
        }

        [Test]
        public void GetZoneIntervals_MiddleOfFirstAmbiguity()
        {
            var firstAmbiguity = new LocalInstant(2000, 10, 5, 1, 30);
            var pair = TestZone.GetZoneIntervalPair(firstAmbiguity);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.AreEqual("Winter", pair.LateInterval.Name);
        }

        [Test]
        public void GetZoneIntervals_AfterFirstAmbiguity()
        {
            var unambiguousWinter = new LocalInstant(2000, 10, 5, 2, 0);
            var pair = TestZone.GetZoneIntervalPair(unambiguousWinter);
            Assert.AreEqual("Winter", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_WithinArbitrarySummer()
        {
            var summer = new LocalInstant(2010, 6, 1, 0, 0);
            var pair = TestZone.GetZoneIntervalPair(summer);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_WithinArbitraryWinter()
        {
            var winter = new LocalInstant(2010, 12, 1, 0, 0);
            var pair = TestZone.GetZoneIntervalPair(winter);
            Assert.AreEqual("Winter", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_AtArbitraryGapStart()
        {
            var startOfGap = new LocalInstant(2010, 3, 10, 1, 0);
            var pair = TestZone.GetZoneIntervalPair(startOfGap);
            Assert.AreEqual(0, pair.MatchingIntervals);
        }

        [Test]
        public void GetZoneIntervals_WithinArbitraryGap()
        {
            var middleOfGap = new LocalInstant(2010, 3, 10, 1, 30);
            var pair = TestZone.GetZoneIntervalPair(middleOfGap);
            Assert.AreEqual(0, pair.MatchingIntervals);
        }

        [Test]
        public void GetZoneIntervals_EndOfArbitraryGap()
        {
            var endOfGap = new LocalInstant(2010, 3, 10, 2, 0);
            var pair = TestZone.GetZoneIntervalPair(endOfGap);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_StartOfArbitraryAmbiguity()
        {
            var ambiguity = new LocalInstant(2010, 10, 5, 1, 0);
            var pair = TestZone.GetZoneIntervalPair(ambiguity);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.AreEqual("Winter", pair.LateInterval.Name);
        }

        [Test]
        public void GetZoneIntervals_MiddleOfArbitraryAmbiguity()
        {
            var ambiguity = new LocalInstant(2010, 10, 5, 1, 30);
            var pair = TestZone.GetZoneIntervalPair(ambiguity);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.AreEqual("Winter", pair.LateInterval.Name);
        }

        [Test]
        public void GetZoneIntervals_AfterArbitraryAmbiguity()
        {
            var unambiguousWinter = new LocalInstant(2010, 10, 5, 2, 0);
            var pair = TestZone.GetZoneIntervalPair(unambiguousWinter);
            Assert.AreEqual("Winter", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }
    }
}
