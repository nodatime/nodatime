using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class DaylightSavingTimeZoneTest
    {
        private static readonly ZoneRecurrence Winter = new ZoneRecurrence("Winter", Offset.Zero,
            new ZoneYearOffset(TransitionMode.Wall, 10, 5, 0, false, Offset.ForHours(2)), 2000, int.MaxValue);

        private static readonly ZoneRecurrence Summer = new ZoneRecurrence("Summer", Offset.ForHours(1),
            new ZoneYearOffset(TransitionMode.Wall, 3, 10, 0, false, Offset.ForHours(1)), 2000, int.MaxValue);

        /// <summary>
        /// Time zone with the following characteristics:
        /// - Only valid from March 10th 2000
        /// - Standard offset of +5 (so 4am UTC = 9am local)
        /// - Summer time (DST = 1 hour) always starts at 1am local time on March 10th (skips to 2am)
        /// - Winter time (DST = 0) always starts at 2am local time on October 5th (skips to 1am)
        /// </summary>
        private static readonly DaylightSavingsTimeZone TestZone = new DaylightSavingsTimeZone("Test",
            Offset.ForHours(5), Winter, Summer);

        [Test]
        public void MinMaxOffsets()
        {
            Assert.AreEqual(Offset.ForHours(6), TestZone.MaxOffset);
            Assert.AreEqual(Offset.ForHours(5), TestZone.MinOffset);
        }

        [Test]
        public void GetZoneInterval_Instant_Summer()
        {
            var interval = TestZone.GetZoneInterval(Instant.FromUtc(2010, 6, 1, 0, 0));
            Assert.AreEqual("Summer", interval.Name);
            Assert.AreEqual(Offset.ForHours(6), interval.Offset);
            Assert.AreEqual(Offset.ForHours(5), interval.BaseOffset);
            Assert.AreEqual(Offset.ForHours(1), interval.Savings);
            Assert.AreEqual(new LocalInstant(2010, 3, 10, 2, 0), interval.LocalStart);
            Assert.AreEqual(new LocalInstant(2010, 10, 5, 2, 0), interval.LocalEnd);
        }

        [Test]
        public void GetZoneInterval_Instant_Winter()
        {
            var interval = TestZone.GetZoneInterval(Instant.FromUtc(2010, 11, 1, 0, 0));
            Assert.AreEqual("Winter", interval.Name);
            Assert.AreEqual(Offset.ForHours(5), interval.Offset);
            Assert.AreEqual(Offset.ForHours(5), interval.BaseOffset);
            Assert.AreEqual(Offset.ForHours(0), interval.Savings);
            Assert.AreEqual(new LocalInstant(2010, 10, 5, 1, 0), interval.LocalStart);
            Assert.AreEqual(new LocalInstant(2011, 3, 10, 1, 0), interval.LocalEnd);
        }

        [Test]
        public void GetZoneInterval_Instant_Invalid()
        {
            // This is only just about invalid
            var invalid = Instant.FromUtc(2000, 3, 9, 19, 59);
            Assert.Throws<ArgumentOutOfRangeException>(() => TestZone.GetZoneInterval(invalid));
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
        public void GetZoneIntervals_BeforeRecurrenceStart()
        {
            var early = new LocalInstant(1990, 1, 1, 0, 0);
            var pair = TestZone.GetZoneIntervals(early);
            Assert.AreEqual(0, pair.MatchingIntervals);
        }

        [Test]
        public void GetZoneIntervals_WithinFirstSummer()
        {
            var early = new LocalInstant(2000, 6, 1, 0, 0);
            var pair = TestZone.GetZoneIntervals(early);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_WithinFirstWinter()
        {
            var winter = new LocalInstant(2000, 12, 1, 0, 0);
            var pair = TestZone.GetZoneIntervals(winter);
            Assert.AreEqual("Winter", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_AtFirstGapStart()
        {
            var startOfFirstGap = new LocalInstant(2000, 3, 10, 1, 0);
            var pair = TestZone.GetZoneIntervals(startOfFirstGap);
            Assert.AreEqual(0, pair.MatchingIntervals);
        }

        [Test]
        public void GetZoneIntervals_WithinFirstGap()
        {
            var middleOfFirstGap = new LocalInstant(2000, 3, 10, 1, 30);
            var pair = TestZone.GetZoneIntervals(middleOfFirstGap);
            Assert.AreEqual(0, pair.MatchingIntervals);
        }

        [Test]
        public void GetZoneIntervals_EndOfFirstGap()
        {
            var endOfFirstGap = new LocalInstant(2000, 3, 10, 2, 0);
            var pair = TestZone.GetZoneIntervals(endOfFirstGap);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_StartOfFirstAmbiguity()
        {
            var firstAmbiguity = new LocalInstant(2000, 10, 5, 1, 0);
            var pair = TestZone.GetZoneIntervals(firstAmbiguity);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.AreEqual("Winter", pair.LateInterval.Name);
        }

        [Test]
        public void GetZoneIntervals_MiddleOfFirstAmbiguity()
        {
            var firstAmbiguity = new LocalInstant(2000, 10, 5, 1, 30);
            var pair = TestZone.GetZoneIntervals(firstAmbiguity);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.AreEqual("Winter", pair.LateInterval.Name);
        }

        [Test]
        public void GetZoneIntervals_AfterFirstAmbiguity()
        {
            var unambiguousWinter = new LocalInstant(2000, 10, 5, 2, 0);
            var pair = TestZone.GetZoneIntervals(unambiguousWinter);
            Assert.AreEqual("Winter", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_WithinArbitrarySummer()
        {
            var summer = new LocalInstant(2010, 6, 1, 0, 0);
            var pair = TestZone.GetZoneIntervals(summer);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_WithinArbitraryWinter()
        {
            var winter = new LocalInstant(2010, 12, 1, 0, 0);
            var pair = TestZone.GetZoneIntervals(winter);
            Assert.AreEqual("Winter", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_AtArbitraryGapStart()
        {
            var startOfGap = new LocalInstant(2010, 3, 10, 1, 0);
            var pair = TestZone.GetZoneIntervals(startOfGap);
            Assert.AreEqual(0, pair.MatchingIntervals);
        }

        [Test]
        public void GetZoneIntervals_WithinArbitraryGap()
        {
            var middleOfGap = new LocalInstant(2010, 3, 10, 1, 30);
            var pair = TestZone.GetZoneIntervals(middleOfGap);
            Assert.AreEqual(0, pair.MatchingIntervals);
        }

        [Test]
        public void GetZoneIntervals_EndOfArbitraryGap()
        {
            var endOfGap = new LocalInstant(2010, 3, 10, 2, 0);
            var pair = TestZone.GetZoneIntervals(endOfGap);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_StartOfArbitraryAmbiguity()
        {
            var ambiguity = new LocalInstant(2010, 10, 5, 1, 0);
            var pair = TestZone.GetZoneIntervals(ambiguity);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.AreEqual("Winter", pair.LateInterval.Name);
        }

        [Test]
        public void GetZoneIntervals_MiddleOfArbitraryAmbiguity()
        {
            var ambiguity = new LocalInstant(2010, 10, 5, 1, 30);
            var pair = TestZone.GetZoneIntervals(ambiguity);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.AreEqual("Winter", pair.LateInterval.Name);
        }

        [Test]
        public void GetZoneIntervals_AfterArbitraryAmbiguity()
        {
            var unambiguousWinter = new LocalInstant(2010, 10, 5, 2, 0);
            var pair = TestZone.GetZoneIntervals(unambiguousWinter);
            Assert.AreEqual("Winter", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void DisparateRecurrences_SummerEarlier()
        {
            // The summer recurrence starts in 2000; the winter
            // recurrence starts in 2005. The first valid instant should be
            // the start of the summer recurrence in 2005.
            var summer = new ZoneRecurrence("Summer", Offset.ForHours(1),
                new ZoneYearOffset(TransitionMode.Wall, 3, 10, 0, false, Offset.ForHours(1)), 2000, int.MaxValue);

            var winter = new ZoneRecurrence("Winter", Offset.Zero,
                new ZoneYearOffset(TransitionMode.Wall, 10, 5, 0, false, Offset.ForHours(2)), 2005, int.MaxValue);

            var zone = new DaylightSavingsTimeZone("Test", Offset.ForHours(5), winter, summer);
            AssertFirstInstant(zone, Instant.FromUtc(2005, 3, 9, 20, 0)); // March 10th local time.
        }

        [Test]
        public void DisparateRecurrences_WinterEarlier()
        {
            // The summer recurrence starts in 2005; the winter
            // recurrence starts in 2000. The first valid instant should be
            // the start of the winter recurrence in 2004.
            var summer = new ZoneRecurrence("Summer", Offset.ForHours(1),
                new ZoneYearOffset(TransitionMode.Wall, 3, 10, 0, false, Offset.ForHours(1)), 2005, int.MaxValue);

            var winter = new ZoneRecurrence("Winter", Offset.Zero,
                new ZoneYearOffset(TransitionMode.Wall, 10, 5, 0, false, Offset.ForHours(2)), 2000, int.MaxValue);

            var zone = new DaylightSavingsTimeZone("Test", Offset.ForHours(5), winter, summer);
            AssertFirstInstant(zone, Instant.FromUtc(2004, 10, 4, 20, 0)); // October 5th local time.
        }

        /// <summary>
        /// Checks that the given instant is the first valid one within the zone - i.e.
        /// that calling zone.GetZoneInterval for that instant is okay, but with one
        /// tick earlier an exception is thrown.
        /// </summary>
        private void AssertFirstInstant(DaylightSavingsTimeZone zone, Instant expectedFirst)
        {
            Assert.IsNotNull(zone.GetZoneInterval(expectedFirst));
            Assert.Throws<ArgumentOutOfRangeException>(() => zone.GetZoneInterval(expectedFirst - Duration.One));
        }
    }
}
