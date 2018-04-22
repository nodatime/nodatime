// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Linq;
using NodaTime.TimeZones;
using NUnit.Framework;
using System.IO;
using NodaTime.TimeZones.IO;
using System;

namespace NodaTime.Test.TimeZones
{
    public class StandardDaylightAlternatingMapTest
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
        private static readonly StandardDaylightAlternatingMap TestMap =
            new StandardDaylightAlternatingMap(Offset.FromHours(5), Winter, Summer);

        private static readonly DateTimeZone TestZone =
            new PrecalculatedDateTimeZone(
                "zone",
                new[] { new ZoneInterval("Before", Instant.BeforeMinValue, Instant.FromUtc(1999, 12, 1, 0, 0), Offset.FromHours(5), Summer.Savings) },
                new StandardDaylightAlternatingMap(Offset.FromHours(5), Winter, Summer));

        [Test]
        public void MinMaxOffsets()
        {
            Assert.AreEqual(Offset.FromHours(6), TestMap.MaxOffset);
            Assert.AreEqual(Offset.FromHours(5), TestMap.MinOffset);
        }

        [Test]
        public void GetZoneInterval_Instant_Summer()
        {
            var interval = TestMap.GetZoneInterval(Instant.FromUtc(2010, 6, 1, 0, 0));
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
            var interval = TestMap.GetZoneInterval(Instant.FromUtc(2010, 11, 1, 0, 0));
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
            var interval = TestMap.GetZoneInterval(firstSummer);
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
        public void Equality()
        {
            // Order of recurrences doesn't matter
            var map1 = new StandardDaylightAlternatingMap(Offset.FromHours(1), Summer, Winter);
            var map2 = new StandardDaylightAlternatingMap(Offset.FromHours(1), Winter, Summer);
            var map3 = new StandardDaylightAlternatingMap(Offset.FromHours(1), Winter,
                // Summer, but starting from 1900
                new ZoneRecurrence("Summer", Offset.FromHours(1),
                    new ZoneYearOffset(TransitionMode.Wall, 3, 10, 0, false, new LocalTime(1, 0)), 1900, int.MaxValue));
            // Standard offset does matter
            var map4 = new StandardDaylightAlternatingMap(Offset.FromHours(0), Summer, Winter);

            TestHelper.TestEqualsClass(map1, map2, map4);
            TestHelper.TestEqualsClass(map1, map3, map4);
            
            // Recurrences like Summer, but different in one aspect each, *except* 
            var unequalMaps = new[]
            {
                new ZoneRecurrence("Different name", Offset.FromHours(1),
                    new ZoneYearOffset(TransitionMode.Wall, 3, 10, 0, false, new LocalTime(1, 0)), 2000, int.MaxValue),
                new ZoneRecurrence("Summer", Offset.FromHours(2),
                    new ZoneYearOffset(TransitionMode.Wall, 3, 10, 0, false, new LocalTime(1, 0)), 2000, int.MaxValue),
                new ZoneRecurrence("Summer", Offset.FromHours(1),
                    new ZoneYearOffset(TransitionMode.Standard, 3, 10, 0, false, new LocalTime(1, 0)), 2000, int.MaxValue),
                new ZoneRecurrence("Summer", Offset.FromHours(1),
                    new ZoneYearOffset(TransitionMode.Wall, 4, 10, 0, false, new LocalTime(1, 0)), 2000, int.MaxValue),
                new ZoneRecurrence("Summer", Offset.FromHours(1),
                    new ZoneYearOffset(TransitionMode.Wall, 3, 9, 0, false, new LocalTime(1, 0)), 2000, int.MaxValue),
                new ZoneRecurrence("Summer", Offset.FromHours(1),
                    new ZoneYearOffset(TransitionMode.Wall, 3, 10, 1, false, new LocalTime(1, 0)), 2000, int.MaxValue),
                // Advance with day-of-week 0 doesn't make any real difference, but they compare non-equal...
                new ZoneRecurrence("Summer", Offset.FromHours(1),
                    new ZoneYearOffset(TransitionMode.Wall, 3, 10, 0, true, new LocalTime(1, 0)), 2000, int.MaxValue),
                new ZoneRecurrence("Summer", Offset.FromHours(1),
                    new ZoneYearOffset(TransitionMode.Wall, 3, 10, 0, false, new LocalTime(2, 0)), 2000, int.MaxValue)
            }.Select(recurrence => new StandardDaylightAlternatingMap(Offset.FromHours(1), Winter, recurrence)).ToArray();
            TestHelper.TestEqualsClass(map1, map2, unequalMaps);
        }

        [Test]
        public void ReadWrite()
        {
            var map1 = new StandardDaylightAlternatingMap(Offset.FromHours(1), Summer, Winter);
            var stream = new MemoryStream();
            var writer = new DateTimeZoneWriter(stream, null);
            map1.Write(writer);
            stream.Position = 0;

            var reader = new DateTimeZoneReader(stream, null);
            var map2 = StandardDaylightAlternatingMap.Read(reader);
            Assert.AreEqual(map1, map2);
        }

        [Test]
        public void Extremes()
        {
            ZoneRecurrence winter = new ZoneRecurrence("Winter", Offset.Zero,
                new ZoneYearOffset(TransitionMode.Wall, 10, 5, 0, false, new LocalTime(2, 0)), int.MinValue, int.MaxValue);

            ZoneRecurrence summer = new ZoneRecurrence("Summer", Offset.FromHours(1),
                new ZoneYearOffset(TransitionMode.Wall, 3, 10, 0, false, new LocalTime(1, 0)), int.MinValue, int.MaxValue);

            var zone = new StandardDaylightAlternatingMap(Offset.Zero, winter, summer);

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
        }

        [Test]
        public void InvalidMap_SimultaneousTransition()
        {
            // Two recurrences with different savings, but which occur at the same instant in time every year.
            ZoneRecurrence r1 = new ZoneRecurrence("Recurrence1", Offset.Zero,
                new ZoneYearOffset(TransitionMode.Utc, 10, 5, 0, false, new LocalTime(2, 0)), int.MinValue, int.MaxValue);

            ZoneRecurrence r2 = new ZoneRecurrence("Recurrence2", Offset.FromHours(1),
                new ZoneYearOffset(TransitionMode.Utc, 10, 5, 0, false, new LocalTime(2, 0)), int.MinValue, int.MaxValue);

            var map = new StandardDaylightAlternatingMap(Offset.Zero, r1, r2);

            Assert.Throws<InvalidOperationException>(() => map.GetZoneInterval(Instant.FromUtc(2017, 8, 25, 0, 0, 0)));
        }

        private void CheckMapping(ZoneLocalMapping mapping, string earlyIntervalName, string lateIntervalName, int count)
        {
            Assert.AreEqual(earlyIntervalName, mapping.EarlyInterval.Name);
            Assert.AreEqual(lateIntervalName, mapping.LateInterval.Name);
            Assert.AreEqual(count, mapping.Count);
        }
    }
}
