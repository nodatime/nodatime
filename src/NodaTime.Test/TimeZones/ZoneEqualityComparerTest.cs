// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Linq;
using NodaTime.Testing.TimeZones;
using NodaTime.TimeZones;
using NUnit.Framework;
using System;

namespace NodaTime.Test.TimeZones
{
    public class ZoneEqualityComparerTest
    {
        // Sample instants for use in tests. They're on January 1st 2000...2009, midnight UTC.
        private static IList<Instant> Instants = Enumerable.Range(2000, 10)
                                                           .Select(year => Instant.FromUtc(year, 1, 1, 0, 0))
                                                           .ToList()
                                                           .AsReadOnly();

        // Various tests using a pair of zones which can demonstrate a number of
        // different features.
        [Test]
        public void Various()
        {
            // Names, some offsets, and first transition are all different.
            var zone1 = new MultiTransitionDateTimeZone.Builder
            {
                { Instants[0], 1, 0, "xx" },
                { Instants[2], 3, 0, "1b" },
                { Instants[4], 2, 1, "1c" },
                { Instants[6], 4, 0, "1d" },
            }.Build();
            var zone2 = new MultiTransitionDateTimeZone.Builder
            {
                { Instants[1], 1, 0, "xx" },
                { Instants[2], 3, 0, "2b" },
                { Instants[4], 1, 2, "2c" },
                { Instants[6], 5, 0, "2d"},
            }.Build();
            // Even though the first transition point is different, by default that's fine if
            // the start point is "inside" both.
            AssertEqual(zone1, zone2, Instants[1], Instants[5], ZoneEqualityComparer.Options.OnlyMatchWallOffset);
            // When we extend backwards a bit, we can see the difference between the two.
            AssertNotEqual(zone1, zone2, Instants[1] - Duration.Epsilon, Instants[5], ZoneEqualityComparer.Options.OnlyMatchWallOffset);
            // Or if we force the start and end transitions to be exact...
            AssertNotEqual(zone1, zone2, Instants[1], Instants[5], ZoneEqualityComparer.Options.MatchStartAndEndTransitions);
            
            // The first two transitions have the same split between standard and saving...
            AssertEqual(zone1, zone2, Instants[1], Instants[4], ZoneEqualityComparer.Options.MatchOffsetComponents);
            // The third one (at Instants[4]) doesn't...
            AssertNotEqual(zone1, zone2, Instants[1], Instants[5], ZoneEqualityComparer.Options.MatchOffsetComponents);

            // The first transition has the same name for the zone interval...
            AssertEqual(zone1, zone2, Instants[1], Instants[2], ZoneEqualityComparer.Options.MatchNames);
            // The second transition (at Instants[2]) doesn't...
            AssertNotEqual(zone1, zone2, Instants[1], Instants[3], ZoneEqualityComparer.Options.MatchNames);
        }

        [Test]
        public void ElidedTransitions()
        {
            var zone1 = new MultiTransitionDateTimeZone.Builder
            {
                { Instants[3], 0, 0, "a" },
                { Instants[4], 1, 2, "b" },
                { Instants[5], 2, 1, "b" },
                { Instants[6], 1, 0, "d" },
                { Instants[7], 1, 0, "e" },
                { Instants[8], 0, 0, "x" },
            }.Build();
            var zone2 = new MultiTransitionDateTimeZone.Builder
            {
                { Instants[3], 0, 0, "a" },
                { Instants[4], 3, 0, "b" },
                // Instants[5] isn't included here: wall offset is the same; components change in zone1
                { Instants[6], 1, 0, "d" },
                // Instants[7] isn't included here: offset components are the same; names change in zone1
                { Instants[8], 0, 0, "x" },
            }.Build();

            AssertEqual(zone1, zone2, Instant.MinValue, Instant.MaxValue, ZoneEqualityComparer.Options.OnlyMatchWallOffset);
            // BOT-Instants[6] will elide transitions when ignoring components, even if we match names
            AssertEqual(zone1, zone2, Instant.MinValue, Instants[6], ZoneEqualityComparer.Options.MatchNames);
            AssertNotEqual(zone1, zone2, Instant.MinValue, Instants[6], ZoneEqualityComparer.Options.MatchOffsetComponents);
            // Instants[6]-EOT will elide transitions when ignoring names, even if we match components
            AssertEqual(zone1, zone2, Instants[6], Instant.MaxValue, ZoneEqualityComparer.Options.MatchOffsetComponents);
            AssertNotEqual(zone1, zone2, Instants[6], Instant.MaxValue, ZoneEqualityComparer.Options.MatchNames);
            
            // But if we require the exact transitions, both fail
            AssertNotEqual(zone1, zone2, Instant.MinValue, Instants[6], ZoneEqualityComparer.Options.MatchAllTransitions);
            AssertNotEqual(zone1, zone2, Instants[6], Instant.MaxValue, ZoneEqualityComparer.Options.MatchAllTransitions);
        }

        [Test]
        public void ForInterval()
        {
            var interval = new Interval(Instants[3], Instants[5]);
            var comparer = ZoneEqualityComparer.ForInterval(interval);
            Assert.AreEqual(ZoneEqualityComparer.Options.OnlyMatchWallOffset, comparer.OptionsForTest);
            Assert.AreEqual(interval, comparer.IntervalForTest);
        }

        [Test]
        public void WithOptions()
        {
            var interval = new Interval(Instants[3], Instants[5]);
            var firstComparer = ZoneEqualityComparer.ForInterval(interval);
            var secondComparer = firstComparer.WithOptions(ZoneEqualityComparer.Options.MatchNames);

            Assert.AreEqual(ZoneEqualityComparer.Options.MatchNames, secondComparer.OptionsForTest);
            Assert.AreEqual(interval, secondComparer.IntervalForTest);

            // Validate that the first comparer hasn't changed
            Assert.AreEqual(ZoneEqualityComparer.Options.OnlyMatchWallOffset, firstComparer.OptionsForTest);
            Assert.AreEqual(interval, firstComparer.IntervalForTest);
        }

        [Test]
        public void ElidedTransitions_Degenerate()
        {
            // Transitions with *nothing* that we care about. (Normally
            // these wouldn't even be generated, but we could imagine some
            // sort of zone interval in the future which had another property...)
            var zone1 = new MultiTransitionDateTimeZone.Builder
            {
                { Instants[3], 1, 0, "a" },
                { Instants[4], 1, 0, "a" },
                { Instants[5], 1, 0, "a" },
                { Instants[6], 0 }
            }.Build();
            var zone2 = new MultiTransitionDateTimeZone.Builder
            {
                { Instants[3], 1, 0, "a" },
                { Instants[6], 0 }
            }.Build();

            // We can match *everything* except exact transitions...
            AssertEqual(zone1, zone2, Instant.MinValue, Instant.MaxValue, ZoneEqualityComparer.Options.MatchNames | ZoneEqualityComparer.Options.MatchOffsetComponents | ZoneEqualityComparer.Options.MatchStartAndEndTransitions);
            // But not the exact transitions...
            AssertNotEqual(zone1, zone2, Instant.MinValue, Instant.MaxValue, ZoneEqualityComparer.Options.MatchAllTransitions);
        }

        [Test]
        public void ReferenceComparison()
        {
            var comparer = ZoneEqualityComparer.ForInterval(new Interval(Instants[0], Instants[2]));
            var zone = DateTimeZoneProviders.Tzdb["Europe/London"];
            Assert.IsTrue(comparer.Equals(zone, zone));
        }

        [Test]
        public void NullComparison()
        {
            var comparer = ZoneEqualityComparer.ForInterval(new Interval(Instants[0], Instants[2]));
            var zone = DateTimeZoneProviders.Tzdb["Europe/London"];
            Assert.IsFalse(comparer.Equals(zone, null));
            Assert.IsFalse(comparer.Equals(null, zone));
        }

        [Test]
        public void InvalidOptions()
        {
            var comparer = ZoneEqualityComparer.ForInterval(new Interval(Instants[0], Instants[2]));
            Assert.Throws<ArgumentOutOfRangeException>(() => comparer.WithOptions((ZoneEqualityComparer.Options) 9999));
        }

        private void AssertEqual(DateTimeZone first, DateTimeZone second, 
            Instant start, Instant end, ZoneEqualityComparer.Options options)
        {
            var comparer = ZoneEqualityComparer.ForInterval(new Interval(start, end)).WithOptions(options);
            Assert.IsTrue(comparer.Equals(first, second));
            Assert.AreEqual(comparer.GetHashCode(first), comparer.GetHashCode(second));
        }

        private void AssertNotEqual(DateTimeZone first, DateTimeZone second, 
            Instant start, Instant end, ZoneEqualityComparer.Options options)
        {
            var comparer = ZoneEqualityComparer.ForInterval(new Interval(start, end)).WithOptions(options);
            Assert.IsFalse(comparer.Equals(first, second));
            // If this fails, the code *could* still be correct - but it's unlikely...
            Assert.AreNotEqual(comparer.GetHashCode(first), comparer.GetHashCode(second));
        }
    }
}
