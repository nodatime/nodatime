// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Linq;
using NodaTime.Testing.TimeZones;
using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Test
{
    // Tests for DateTimeZoneTest.GetZoneIntervals. The calls to ToList()
    // in the assertions are to make the actual values get dumped on failure, instead of
    // just <NodaTime.DateTimeZone+<GetZoneIntervalsImpl>d__0>
    public partial class DateTimeZoneTest
    {
        private static readonly SingleTransitionDateTimeZone TestZone = new SingleTransitionDateTimeZone
            (Instant.FromUtc(2000, 1, 1, 0, 0), -3, +4);

        [Test]
        public void GetZoneIntervals_EndBeforeStart()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DateTimeZone.Utc.GetZoneIntervals(Instant.FromUnixTimeTicks(100L), Instant.FromUnixTimeTicks(99L)));
        }

        [Test]
        public void GetZoneIntervals_EndEqualToStart()
        {
            CollectionAssert.IsEmpty(DateTimeZone.Utc.GetZoneIntervals(Instant.FromUnixTimeTicks(100L), Instant.FromUnixTimeTicks(100L)));
        }

        [Test]
        public void GetZoneIntervals_InvalidOptions()
        {
            var zone = DateTimeZone.Utc;
            var interval = new Interval(Instant.FromUtc(2000, 1, 1, 0, 0), Instant.FromUtc(2001, 1, 1, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => zone.GetZoneIntervals(interval, (ZoneEqualityComparer.Options) 1234567));
        }

        [Test]
        public void GetZoneIntervals_FixedZone()
        {
            var zone = DateTimeZone.ForOffset(Offset.FromHours(3));
            var expected = new[] { zone.GetZoneInterval(Instant.MinValue) };
            // Give a reasonably wide interval...
            var actual = zone.GetZoneIntervals(Instant.FromUtc(1900, 1, 1, 0, 0), Instant.FromUtc(2100, 1, 1, 0, 0));
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [Test]
        public void GetZoneIntervals_SingleTransitionZone_IntervalCoversTransition()
        {
            Instant start = TestZone.Transition - Duration.FromDays(5);
            Instant end = TestZone.Transition + Duration.FromDays(5);
            var expected = new[] { TestZone.EarlyInterval, TestZone.LateInterval };
            var actual = TestZone.GetZoneIntervals(start, end);
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [Test]
        public void GetZoneIntervals_SingleTransitionZone_IntervalDoesNotCoverTransition()
        {
            Instant start = TestZone.Transition - Duration.FromDays(10);
            Instant end = TestZone.Transition - Duration.FromDays(5);
            var expected = new[] { TestZone.EarlyInterval };
            var actual = TestZone.GetZoneIntervals(start, end);
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [Test]
        public void GetZoneIntervals_IncludesStart()
        {
            Instant start = TestZone.Transition - Duration.Epsilon;
            Instant end = TestZone.Transition + Duration.FromDays(5);
            var expected = new[] { TestZone.EarlyInterval, TestZone.LateInterval };
            var actual = TestZone.GetZoneIntervals(start, end);
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [Test]
        public void GetZoneIntervals_ExcludesEnd()
        {
            Instant start = TestZone.Transition - Duration.FromDays(10);
            Instant end = TestZone.Transition;
            var expected = new[] { TestZone.EarlyInterval };
            var actual = TestZone.GetZoneIntervals(start, end);
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [Test]
        public void GetZoneIntervals_Complex()
        {
            var london = DateTimeZoneProviders.Tzdb["Europe/London"];
            // Transitions are always Spring/Autumn, so June and January should be clear.
            var expected = new[] { 
                london.GetZoneInterval(Instant.FromUtc(1999, 6, 1, 0, 0)),
                london.GetZoneInterval(Instant.FromUtc(2000, 1, 1, 0, 0)),
                london.GetZoneInterval(Instant.FromUtc(2000, 6, 1, 0, 0)),
                london.GetZoneInterval(Instant.FromUtc(2001, 1, 1, 0, 0)),
                london.GetZoneInterval(Instant.FromUtc(2001, 6, 1, 0, 0)),
                london.GetZoneInterval(Instant.FromUtc(2002, 1, 1, 0, 0)),
            };
            // After the instant we used to fetch the expected zone interval, but that's fine:
            // it'll be the same one, as there's no transition within June.
            var start = Instant.FromUtc(1999, 6, 19, 0, 0);
            var end = Instant.FromUtc(2002, 2, 4, 0, 0);
            var actual = london.GetZoneIntervals(start, end);
            CollectionAssert.AreEqual(expected, actual.ToList());
            // Just to exercise the other overload
            actual = london.GetZoneIntervals(new Interval(start, end));
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [Test]
        public void GetZoneIntervals_WithOptions_NoCoalescing()
        {
            // We'll ask for 1999-2003, so there are three transitions within that.
            var transition1 = Instant.FromUtc(2000, 1, 1, 0, 0);
            var transition2 = Instant.FromUtc(2001, 1, 1, 0, 0);
            var transition3 = Instant.FromUtc(2002, 1, 1, 0, 0);
            // And one transition afterwards.
            var transition4 = Instant.FromUtc(2004, 1, 1, 0, 0);
            var zone = new MultiTransitionDateTimeZone.Builder(0, "0+0")
            {
                { transition1, 1, 1, "1+1" },
                { transition2, 0, 2, "0+2" },
                { transition3, 0, 1, "0+1" },
                { transition4, 0, 0, "0+0" }
            }.Build();
            var interval = new Interval(
                Instant.FromUtc(1999, 1, 1, 0, 0),
                Instant.FromUtc(2003, 1, 1, 0, 0));
            // No coalescing required, as the names are different.
            var zoneIntervals = zone.GetZoneIntervals(interval, ZoneEqualityComparer.Options.MatchNames).ToList();
            Assert.AreEqual(4, zoneIntervals.Count);
            CollectionAssert.AreEqual(new[] { transition1, transition2, transition3, transition4 },
                zoneIntervals.Select(zi => zi.End));
        }

        [Test]
        public void GetZoneIntervals_WithOptions_Coalescing()
        {
            // We'll ask for 1999-2003, so there are three transitions within that.
            var transition1 = Instant.FromUtc(2000, 1, 1, 0, 0);
            var transition2 = Instant.FromUtc(2001, 1, 1, 0, 0);
            var transition3 = Instant.FromUtc(2002, 1, 1, 0, 0);
            // And one transition afterwards.
            var transition4 = Instant.FromUtc(2004, 1, 1, 0, 0);
            var zone = new MultiTransitionDateTimeZone.Builder(0, "0+0")
            {
                { transition1, 1, 1, "1+1" },
                { transition2, 0, 2, "0+2" },
                { transition3, 0, 1, "0+1" },
                { transition4, 0, 0, "0+0" }
            }.Build();
            var interval = new Interval(
                Instant.FromUtc(1999, 1, 1, 0, 0),
                Instant.FromUtc(2003, 1, 1, 0, 0));
            // The zone intervals abutting at transition2 are coalesced,
            // because that only changes the name and standard/daylight split.
            var zoneIntervals = zone.GetZoneIntervals(interval, ZoneEqualityComparer.Options.OnlyMatchWallOffset).ToList();
            Assert.AreEqual(3, zoneIntervals.Count);
            CollectionAssert.AreEqual(new[] { transition1, transition3, transition4 },
                zoneIntervals.Select(zi => zi.End));
            CollectionAssert.AreEqual(new[] { Instant.BeforeMinValue, transition1, transition3 },
                zoneIntervals.Select(zi => zi.RawStart));
            CollectionAssert.AreEqual(new[] { "0+0", "1+1", "0+1" },
                zoneIntervals.Select(zi => zi.Name));
        }
    }
}
