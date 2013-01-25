// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using NodaTime.Testing.TimeZones;
using NodaTime.TimeZones;

namespace NodaTime.Test
{
    // Tests for GetZoneIntervals within DateTimeZone.
    // We have two zones, each with a single transition at midnight January 1st 2000.
    // One goes from -5 to +10, i.e. skips from 7pm Dec 31st to 10am Jan 1st
    // The other goes from +10 to -5, i.e. goes from 10am Jan 1st back to 7pm Dec 31st.
    // Both zones are tested for the zone interval pairs at:
    // - The start of time
    // - The end of time
    // - A local time well before the transition
    // - A local time well after the transition
    // - An unambiguous local time shortly before the transition
    // - An unambiguous local time shortly after the transition
    // - The start of the transition
    // - In the middle of the gap / ambiguity
    // - The last local instant of the gap / ambiguity
    // - The local instant immediately after the gap / ambiguity
    public partial class DateTimeZoneTest
    {
        private static readonly Instant Transition = Instant.FromUtc(2000, 1, 1, 0, 0);

        private static readonly Offset Minus5 = Offset.FromHours(-5);
        private static readonly Offset Plus10 = Offset.FromHours(10);

        private static readonly LocalInstant TransitionMinus5 = Transition.Plus(Minus5);
        private static readonly LocalInstant TransitionPlus10 = Transition.Plus(Plus10);
        private static readonly LocalInstant MidTransition = Transition.Plus(Offset.Zero);

        private static readonly LocalInstant YearBeforeTransition = new LocalInstant(1999, 1, 1, 0, 0);
        private static readonly LocalInstant YearAfterTransition = new LocalInstant(2001, 1, 1, 0, 0);

        private static readonly SingleTransitionDateTimeZone ZoneWithGap = new SingleTransitionDateTimeZone(Transition, Minus5, Plus10);
        private static readonly ZoneInterval IntervalBeforeGap = ZoneWithGap.EarlyInterval;
        private static readonly ZoneInterval IntervalAfterGap = ZoneWithGap.LateInterval;

        private static readonly SingleTransitionDateTimeZone ZoneWithAmbiguity = new SingleTransitionDateTimeZone(Transition, Plus10, Minus5);
        private static readonly ZoneInterval IntervalBeforeAmbiguity = ZoneWithAmbiguity.EarlyInterval;
        private static readonly ZoneInterval IntervalAfterAmbiguity = ZoneWithAmbiguity.LateInterval;

        // Time zone with an ambiguity
        [Test]
        public void ZoneWithAmbiguity_StartOfTime()
        {
            var actual = ZoneWithAmbiguity.GetZoneIntervals(LocalInstant.MinValue);
            var expected = ZoneIntervalPair.Unambiguous(IntervalBeforeAmbiguity);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithAmbiguity_EndOfTime()
        {
            var actual = ZoneWithAmbiguity.GetZoneIntervals(LocalInstant.MaxValue);
            var expected = ZoneIntervalPair.Unambiguous(IntervalAfterAmbiguity);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithAmbiguity_WellBeforeTransition()
        {
            var actual = ZoneWithAmbiguity.GetZoneIntervals(YearBeforeTransition);
            var expected = ZoneIntervalPair.Unambiguous(IntervalBeforeAmbiguity);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithAmbiguity_WellAfterTransition()
        {
            var actual = ZoneWithAmbiguity.GetZoneIntervals(YearAfterTransition);
            var expected = ZoneIntervalPair.Unambiguous(IntervalAfterAmbiguity);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithAmbiguity_JustBeforeAmbiguity()
        {
            var actual = ZoneWithAmbiguity.GetZoneIntervals(TransitionMinus5 - Duration.Epsilon);
            var expected = ZoneIntervalPair.Unambiguous(IntervalBeforeAmbiguity);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithAmbiguity_JustAfterTransition()
        {
            var actual = ZoneWithAmbiguity.GetZoneIntervals(TransitionPlus10 + Duration.Epsilon);
            var expected = ZoneIntervalPair.Unambiguous(IntervalAfterAmbiguity);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithAmbiguity_StartOfTransition()
        {
            var actual = ZoneWithAmbiguity.GetZoneIntervals(TransitionMinus5);
            var expected = ZoneIntervalPair.Ambiguous(IntervalBeforeAmbiguity, IntervalAfterAmbiguity);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithAmbiguity_MidTransition()
        {
            var actual = ZoneWithAmbiguity.GetZoneIntervals(MidTransition);
            var expected = ZoneIntervalPair.Ambiguous(IntervalBeforeAmbiguity, IntervalAfterAmbiguity);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithAmbiguity_LastTickOfTransition()
        {
            var actual = ZoneWithAmbiguity.GetZoneIntervals(TransitionPlus10 - Duration.Epsilon);
            var expected = ZoneIntervalPair.Ambiguous(IntervalBeforeAmbiguity, IntervalAfterAmbiguity);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithAmbiguity_FirstTickAfterTransition()
        {
            var actual = ZoneWithAmbiguity.GetZoneIntervals(TransitionPlus10);
            var expected = ZoneIntervalPair.Unambiguous(IntervalAfterAmbiguity);
            Assert.AreEqual(expected, actual);
        }

        // Time zone with a gap
        [Test]
        public void ZoneWithGap_StartOfTime()
        {
            var actual = ZoneWithGap.GetZoneIntervals(LocalInstant.MinValue);
            var expected = ZoneIntervalPair.Unambiguous(IntervalBeforeGap);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithGap_EndOfTime()
        {
            var actual = ZoneWithGap.GetZoneIntervals(LocalInstant.MaxValue);
            var expected = ZoneIntervalPair.Unambiguous(IntervalAfterGap);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithGap_WellBeforeTransition()
        {
            var actual = ZoneWithGap.GetZoneIntervals(YearBeforeTransition);
            var expected = ZoneIntervalPair.Unambiguous(IntervalBeforeGap);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithGap_WellAfterTransition()
        {
            var actual = ZoneWithGap.GetZoneIntervals(YearAfterTransition);
            var expected = ZoneIntervalPair.Unambiguous(IntervalAfterGap);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithGap_JustBeforeGap()
        {
            var actual = ZoneWithGap.GetZoneIntervals(TransitionMinus5 - Duration.Epsilon);
            var expected = ZoneIntervalPair.Unambiguous(IntervalBeforeGap);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithGap_JustAfterTransition()
        {
            var actual = ZoneWithGap.GetZoneIntervals(TransitionPlus10 + Duration.Epsilon);
            var expected = ZoneIntervalPair.Unambiguous(IntervalAfterGap);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithGap_StartOfTransition()
        {
            var actual = ZoneWithGap.GetZoneIntervals(TransitionMinus5);
            var expected = ZoneIntervalPair.NoMatch;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithGap_MidTransition()
        {
            var actual = ZoneWithGap.GetZoneIntervals(MidTransition);
            var expected = ZoneIntervalPair.NoMatch;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithGap_LastTickOfTransition()
        {
            var actual = ZoneWithGap.GetZoneIntervals(TransitionPlus10 - Duration.Epsilon);
            var expected = ZoneIntervalPair.NoMatch;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ZoneWithGap_FirstTickAfterTransition()
        {
            var actual = ZoneWithGap.GetZoneIntervals(TransitionPlus10);
            var expected = ZoneIntervalPair.Unambiguous(IntervalAfterGap);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Case added to cover everything: we want our initial guess to hit the
        /// *later* zone, which doesn't actually include the local instant. However,
        /// we want the *earlier* zone to include it. So, we want a zone with two
        /// positive offsets.
        /// </summary>
        [Test]
        public void TrickyCase()
        {
            // 1am occurs unambiguously in the early zone.
            var zone = new SingleTransitionDateTimeZone(Transition, Offset.FromHours(3), Offset.FromHours(5));
            var actual = zone.GetZoneIntervals(new LocalInstant(2000, 1, 1, 1, 0));
            var expected = ZoneIntervalPair.Unambiguous(zone.EarlyInterval);
            Assert.AreEqual(expected, actual);
        }
    }
}
