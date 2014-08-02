// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Linq;
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
            Assert.AreEqual(new LocalDateTime(2010, 3, 10, 2, 0), interval.IsoLocalStart);
            Assert.AreEqual(new LocalDateTime(2010, 10, 5, 2, 0), interval.IsoLocalEnd);
        }

        [Test]
        public void GetZoneInterval_Instant_Winter()
        {
            var interval = TestZone.GetZoneInterval(Instant.FromUtc(2010, 11, 1, 0, 0));
            Assert.AreEqual("Winter", interval.Name);
            Assert.AreEqual(Offset.FromHours(5), interval.WallOffset);
            Assert.AreEqual(Offset.FromHours(5), interval.StandardOffset);
            Assert.AreEqual(Offset.FromHours(0), interval.Savings);
            Assert.AreEqual(new LocalDateTime(2010, 10, 5, 1, 0), interval.IsoLocalStart);
            Assert.AreEqual(new LocalDateTime(2011, 3, 10, 1, 0), interval.IsoLocalEnd);
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

        [Test]
        public void Extremes()
        {
            ZoneRecurrence winter = new ZoneRecurrence("Winter", Offset.Zero,
                new ZoneYearOffset(TransitionMode.Wall, 10, 5, 0, false, new LocalTime(2, 0)), int.MinValue, int.MaxValue);

            ZoneRecurrence summer = new ZoneRecurrence("Summer", Offset.FromHours(1),
                new ZoneYearOffset(TransitionMode.Wall, 3, 10, 0, false, new LocalTime(1, 0)), int.MinValue, int.MaxValue);

            var zone = new DaylightSavingsDateTimeZone("infinite", Offset.Zero, winter, summer);

            var firstSpring = Instant.FromUtc(-9998, 3, 10, 1, 0);
            var firstAutumn = Instant.FromUtc(-9998, 10, 5, 1, 0); // 1am UTC = 2am wall

            var lastSpring = Instant.FromUtc(9999, 3, 10, 1, 0);
            var lastAutumn = Instant.FromUtc(9999, 10, 5, 1, 0); // 1am UTC = 2am wall

            var dstOffset = Offset.FromHours(1);

            // Check both year -9998 and 9999, both the infinite interval and the next one in
            var firstWinter = new ZoneInterval("Winter", Instant.BeforeMinValue, firstSpring, Offset.Zero, Offset.Zero);
            var firstSummer = new ZoneInterval("Summer", firstSpring, firstAutumn, dstOffset, dstOffset);
            var lastSummer = new ZoneInterval("Summer", lastSpring, lastAutumn, dstOffset, dstOffset);
            var lastWinter = new ZoneInterval("Winter", lastAutumn, Instant.AfterMaxValue, Offset.Zero, Offset.Zero);

            Assert.AreEqual(firstWinter, zone.GetZoneInterval(Instant.MinValue));
            Assert.AreEqual(firstWinter, zone.GetZoneInterval(Instant.FromUtc(-9998, 2, 1, 0, 0)));
            Assert.AreEqual(firstSummer, zone.GetZoneInterval(firstSpring));
            Assert.AreEqual(firstSummer, zone.GetZoneInterval(Instant.FromUtc(-9998, 5, 1, 0, 0)));

            Assert.AreEqual(lastSummer, zone.GetZoneInterval(lastSpring));
            Assert.AreEqual(lastSummer, zone.GetZoneInterval(Instant.FromUtc(9999, 5, 1, 0, 0)));
            Assert.AreEqual(lastWinter, zone.GetZoneInterval(lastAutumn));
            Assert.AreEqual(lastWinter, zone.GetZoneInterval(Instant.FromUtc(9999, 11, 1, 0, 0)));
            Assert.AreEqual(lastWinter, zone.GetZoneInterval(Instant.MaxValue));

            // And just for kicks, let's check we can get them all with GetZoneIntervals.
            IEnumerable<ZoneInterval> intervals = zone.GetZoneIntervals(new Interval(null, null)).ToList();
            Assert.AreEqual(firstWinter, intervals.First());
            Assert.AreEqual(firstSummer, intervals.Skip(1).First());
            Assert.AreEqual(lastSummer, intervals.Reverse().Skip(1).First());
            Assert.AreEqual(lastWinter, intervals.Last());
        }

        private void CheckMapping(ZoneLocalMapping mapping, string earlyIntervalName, string lateIntervalName, int count)
        {
            Assert.AreEqual(earlyIntervalName, mapping.EarlyInterval.Name);
            Assert.AreEqual(lateIntervalName, mapping.LateInterval.Name);
            Assert.AreEqual(count, mapping.Count);
        }
    }
}
