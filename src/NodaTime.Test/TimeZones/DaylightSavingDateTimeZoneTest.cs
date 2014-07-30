// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using NUnit.Framework;

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
        public void MapLocal_WithinFirstSummer()
        {
            var early = new LocalDateTime(2000, 6, 1, 0, 0);
            CheckMapping(TestZone.MapLocal(early), "Summer", "Summer", 1);
        }

        [Test]
        public void MapLocal_WithinFirstWinter()
        {
            var winter = new LocalDateTime(2000, 12, 1, 0, 0);
            CheckMapping(TestZone.MapLocal(winter), "Winter", "Winter", 1);
        }

        [Test]
        public void MapLocal_AtFirstGapStart()
        {
            var startOfFirstGap = new LocalDateTime(2000, 3, 10, 1, 0);
            CheckMapping(TestZone.MapLocal(startOfFirstGap), "Winter", "Summer", 0);
        }

        [Test]
        public void MapLocal_WithinFirstGap()
        {
            var middleOfFirstGap = new LocalDateTime(2000, 3, 10, 1, 30);
            CheckMapping(TestZone.MapLocal(middleOfFirstGap), "Winter", "Summer", 0);
        }

        [Test]
        public void MapLocal_EndOfFirstGap()
        {
            var endOfFirstGap = new LocalDateTime(2000, 3, 10, 2, 0);
            CheckMapping(TestZone.MapLocal(endOfFirstGap), "Summer", "Summer", 1);
        }

        [Test]
        public void MapLocal_StartOfFirstAmbiguity()
        {
            var firstAmbiguity = new LocalDateTime(2000, 10, 5, 1, 0);
            CheckMapping(TestZone.MapLocal(firstAmbiguity), "Summer", "Winter", 2);
        }

        [Test]
        public void MapLocal_MiddleOfFirstAmbiguity()
        {
            var firstAmbiguity = new LocalDateTime(2000, 10, 5, 1, 30);
            CheckMapping(TestZone.MapLocal(firstAmbiguity), "Summer", "Winter", 2);
        }

        [Test]
        public void MapLocal_AfterFirstAmbiguity()
        {
            var unambiguousWinter = new LocalDateTime(2000, 10, 5, 2, 0);
            CheckMapping(TestZone.MapLocal(unambiguousWinter), "Winter", "Winter", 1);
        }

        [Test]
        public void MapLocal_WithinArbitrarySummer()
        {
            var summer = new LocalDateTime(2010, 6, 1, 0, 0);
            CheckMapping(TestZone.MapLocal(summer), "Summer", "Summer", 1);
        }

        [Test]
        public void MapLocal_WithinArbitraryWinter()
        {
            var winter = new LocalDateTime(2010, 12, 1, 0, 0);
            CheckMapping(TestZone.MapLocal(winter), "Winter", "Winter", 1);
        }

        [Test]
        public void MapLocal_AtArbitraryGapStart()
        {
            var startOfGap = new LocalDateTime(2010, 3, 10, 1, 0);
            CheckMapping(TestZone.MapLocal(startOfGap), "Winter", "Summer", 0);
        }

        [Test]
        public void MapLocal_WithinArbitraryGap()
        {
            var middleOfGap = new LocalDateTime(2010, 3, 10, 1, 30);
            CheckMapping(TestZone.MapLocal(middleOfGap), "Winter", "Summer", 0);
        }

        [Test]
        public void MapLocal_EndOfArbitraryGap()
        {
            var endOfGap = new LocalDateTime(2010, 3, 10, 2, 0);
            CheckMapping(TestZone.MapLocal(endOfGap), "Summer", "Summer", 1);
        }

        [Test]
        public void MapLocal_StartOfArbitraryAmbiguity()
        {
            var ambiguity = new LocalDateTime(2010, 10, 5, 1, 0);
            CheckMapping(TestZone.MapLocal(ambiguity), "Summer", "Winter", 2);
        }

        [Test]
        public void MapLocal_MiddleOfArbitraryAmbiguity()
        {
            var ambiguity = new LocalDateTime(2010, 10, 5, 1, 30);
            CheckMapping(TestZone.MapLocal(ambiguity), "Summer", "Winter", 2);
        }

        [Test]
        public void MapLocal_AfterArbitraryAmbiguity()
        {
            var unambiguousWinter = new LocalDateTime(2010, 10, 5, 2, 0);
            CheckMapping(TestZone.MapLocal(unambiguousWinter), "Winter", "Winter", 1);
        }

        private void CheckMapping(ZoneLocalMapping mapping, string earlyIntervalName, string lateIntervalName, int count)
        {
            Assert.AreEqual(earlyIntervalName, mapping.EarlyInterval.Name);
            Assert.AreEqual(lateIntervalName, mapping.LateInterval.Name);
            Assert.AreEqual(count, mapping.Count);
        }
    }
}
