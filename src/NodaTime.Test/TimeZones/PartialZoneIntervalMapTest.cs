// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Linq;
using NodaTime.Testing.TimeZones;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    public class PartialZoneIntervalMapTest
    {
        // Arbitrary instants which are useful for the tests. They happen to be a year
        // apart, but nothing in these tests actually cares.
        // Various tests use a time zone with transitions at C and G.
        // Using letter is (IMO) slightly more readable than just having an array and using indexes.
        private static readonly Dictionary<char, Instant> Instants = new Dictionary<char, Instant>
        {
            {'A', Instant.FromUtc(2000, 1, 1, 0, 0) },
            {'B', Instant.FromUtc(2001, 1, 1, 0, 0) },
            {'C', Instant.FromUtc(2002, 1, 1, 0, 0) },
            {'D', Instant.FromUtc(2003, 1, 1, 0, 0) },
            {'E', Instant.FromUtc(2004, 1, 1, 0, 0) },
            {'F', Instant.FromUtc(2005, 1, 1, 0, 0) },
            {'G', Instant.FromUtc(2006, 1, 1, 0, 0) },
            {'H', Instant.FromUtc(2007, 1, 1, 0, 0) },
            {'I', Instant.FromUtc(2008, 1, 1, 0, 0) },
        };

        private static readonly DateTimeZone ExpectedZone = new MultiTransitionDateTimeZone.Builder(-2, "Start")
        {
            { Instants['C'], 2, 1, "Middle" },
            { Instants['G'], 1, 0, "End" }
        }.Build();

        // This is just a variety of interesting tests, hopefully covering everything we need. Imagine a time line,
        // and the letters in the string break it up into partial maps (all based on the original zone). We should
        // be able to break it up anywhere and still get back to something equivalent to the original zone.
        [Test]
        [TestCase("")]
        [TestCase("A")]
        [TestCase("C")]
        [TestCase("E")]
        [TestCase("G")]
        [TestCase("H")]
        [TestCase("AB")]
        [TestCase("AC")]
        [TestCase("AD")]
        [TestCase("AG")]
        [TestCase("AH")]
        [TestCase("CG")]
        [TestCase("CH")]
        [TestCase("ACD")]
        [TestCase("ACG")]
        [TestCase("ACH")]
        [TestCase("DEF")]
        [TestCase("ABCDEFGHI")]
        public void ConvertToFullMap(string intervalBreaks)
        {
            var maps = new List<PartialZoneIntervalMap>();
            // We just reuse ExpectedZone as the IZoneIntervalMap; PartialZoneIntervalMap itself will clamp the ends.
            var current = Instant.BeforeMinValue;
            foreach (var instant in intervalBreaks.Select(c => Instants[c]))
            {
                maps.Add(new PartialZoneIntervalMap(current, instant, ExpectedZone));
                current = instant;
            }
            maps.Add(new PartialZoneIntervalMap(current, Instant.AfterMaxValue, ExpectedZone));

            var converted = PartialZoneIntervalMap.ConvertToFullMap(maps);
            CollectionAssert.AreEqual(GetZoneIntervals(ExpectedZone), GetZoneIntervals(converted));
        }

        // TODO: Consider making this part of the NodaTime assembly.
        // It's just a copy from DateTimeZone, with the interval taken out.
        // It could be an extension method on IZoneIntervalMap, with optional interval.
        // On the other hand, IZoneIntervalMap is internal, so it would only be used by us.
        private static IEnumerable<ZoneInterval> GetZoneIntervals(IZoneIntervalMap map)
        {
            var current = Instant.MinValue;
            while (current < Instant.AfterMaxValue)
            {
                var zoneInterval = map.GetZoneInterval(current);
                yield return zoneInterval;
                // If this is the end of time, this will just fail on the next comparison.
                current = zoneInterval.RawEnd;
            }
        }
    }
}
