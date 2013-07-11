// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NodaTime.Testing.TimeZones;
using NodaTime.TimeZones;
using NodaTime.TimeZones.IO;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class PrecalculatedDateTimeZoneTest
    {
        private static readonly ZoneInterval FirstInterval =
            new ZoneInterval("First", Instant.MinValue, Instant.FromUtc(2000, 3, 10, 10, 0), Offset.FromHours(3), Offset.Zero);

        // Note that this is effectively UTC +3 + 1 hour DST.
        private static readonly ZoneInterval SecondInterval =
            new ZoneInterval("Second", FirstInterval.End, Instant.FromUtc(2000, 9, 15, 5, 0), Offset.FromHours(4), Offset.FromHours(1));

        private static readonly ZoneInterval ThirdInterval =
            new ZoneInterval("Third", SecondInterval.End, Instant.FromUtc(2005, 1, 20, 8, 0), Offset.FromHours(-5), Offset.Zero);

        private static readonly ZoneRecurrence Winter = new ZoneRecurrence("Winter", Offset.Zero,
            new ZoneYearOffset(TransitionMode.Wall, 10, 5, 0, false, new LocalTime(2, 0)), 1960, int.MaxValue);

        private static readonly ZoneRecurrence Summer = new ZoneRecurrence("Summer", Offset.FromHours(1),
            new ZoneYearOffset(TransitionMode.Wall, 3, 10, 0, false, new LocalTime(1, 0)), 1960, int.MaxValue);

        private static readonly DaylightSavingsDateTimeZone TailZone = new DaylightSavingsDateTimeZone(
            "TestTail", Offset.FromHours(-6), Winter, Summer);

        // We don't actually want an interval from the beginning of time when we ask our composite time zone for an interval
        // - because that could give the wrong idea. So we clamp it at the end of the precalculated interval.
        private static readonly ZoneInterval ClampedTailZoneInterval = TailZone.GetZoneInterval(ThirdInterval.End).WithStart(ThirdInterval.End);

        private static readonly PrecalculatedDateTimeZone TestZone =
            new PrecalculatedDateTimeZone("Test", new[] { FirstInterval, SecondInterval, ThirdInterval }, TailZone);

        [Test]
        public void MinMaxOffsets()
        {
            Assert.AreEqual(Offset.FromHours(-6), TestZone.MinOffset);
            Assert.AreEqual(Offset.FromHours(4), TestZone.MaxOffset);
        }

        [Test]
        public void MinMaxOffsetsWithOtherTailZone()
        {
            var tailZone = new FixedDateTimeZone("TestFixed", Offset.FromHours(8));
            var testZone = new PrecalculatedDateTimeZone("Test",
                new[] { FirstInterval, SecondInterval, ThirdInterval }, tailZone);
            Assert.AreEqual(Offset.FromHours(-5), testZone.MinOffset);
            Assert.AreEqual(Offset.FromHours(8), testZone.MaxOffset);
        }

        [Test]
        public void MinMaxOffsetsWithNullTailZone()
        {
            var testZone = new PrecalculatedDateTimeZone("Test",
                new[] { FirstInterval, SecondInterval, ThirdInterval,
                        new ZoneInterval("Last", ThirdInterval.End, Instant.MaxValue, Offset.Zero, Offset.Zero) }, null);
            Assert.AreEqual(Offset.FromHours(-5), testZone.MinOffset);
            Assert.AreEqual(Offset.FromHours(4), testZone.MaxOffset);
        }

        [Test]
        public void GetZoneIntervalInstant_End()
        {
            Assert.AreEqual(SecondInterval, TestZone.GetZoneInterval(SecondInterval.End - Duration.Epsilon));
        }

        [Test]
        public void GetZoneIntervalInstant_Start()
        {
            Assert.AreEqual(SecondInterval, TestZone.GetZoneInterval(SecondInterval.Start));
        }

        [Test]
        public void GetZoneIntervalInstant_FinalInterval_End()
        {
            Assert.AreEqual(ThirdInterval, TestZone.GetZoneInterval(ThirdInterval.End - Duration.Epsilon));
        }

        [Test]
        public void GetZoneIntervalInstant_FinalInterval_Start()
        {
            Assert.AreEqual(ThirdInterval, TestZone.GetZoneInterval(ThirdInterval.Start));
        }

        [Test]
        public void GetZoneIntervalInstant_TailZone()
        {
            Assert.AreEqual(ClampedTailZoneInterval, TestZone.GetZoneInterval(ThirdInterval.End));
        }

        [Test]
        public void GetZoneIntervals_UnambiguousInPrecalculated()
        {
            var pair = TestZone.GetZoneIntervalPair(new LocalInstant(2000, 6, 1, 0, 0));
            Assert.AreEqual(SecondInterval, pair.EarlyInterval);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_UnambiguousInTailZone()
        {
            var pair = TestZone.GetZoneIntervalPair(new LocalInstant(2005, 2, 1, 0, 0));
            Assert.AreEqual(ClampedTailZoneInterval, pair.EarlyInterval);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_AmbiguousWithinPrecalculated()
        {
            // Transition from +4 to -5 has a 9 hour ambiguity
            var pair = TestZone.GetZoneIntervalPair(ThirdInterval.LocalStart);
            Assert.AreEqual(SecondInterval, pair.EarlyInterval);
            Assert.AreEqual(ThirdInterval, pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_AmbiguousAroundTailZoneTransition()
        {
            // Transition from -5 to -6 has a 1 hour ambiguity
            var pair = TestZone.GetZoneIntervalPair(ThirdInterval.LocalEnd - Duration.Epsilon);
            Assert.AreEqual(ThirdInterval, pair.EarlyInterval);
            Assert.AreEqual(ClampedTailZoneInterval, pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_AmbiguousButTooEarlyInTailZoneTransition()
        {
            // Tail zone is +10 / +8, with the transition occurring just after
            // the transition *to* the tail zone from the precalculated zone.
            // A local instant of one hour before after the transition from the precalculated zone (which is -5)
            // will therefore be ambiguous, but the resulting instants from the ambiguity occur
            // before our transition into the tail zone, so are ignored.
            var tailZone = new SingleTransitionDateTimeZone(ThirdInterval.End + Duration.FromHours(1), 10, 8);
            var gapZone = new PrecalculatedDateTimeZone("Test",
                new[] { FirstInterval, SecondInterval, ThirdInterval }, tailZone);
            var pair = gapZone.GetZoneIntervalPair(ThirdInterval.LocalEnd - Duration.FromHours(1));
            Assert.AreEqual(ThirdInterval, pair.EarlyInterval);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_GapWithinPrecalculated()
        {
            // Transition from +3 to +4 has a 1 hour gap
            Assert.IsTrue(FirstInterval.LocalEnd < SecondInterval.LocalStart);
            var pair = TestZone.GetZoneIntervalPair(FirstInterval.LocalEnd);
            Assert.IsNull(pair.EarlyInterval);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_SingleIntervalAroundTailZoneTransition()
        {
            // Tail zone is fixed at +5. A local instant of one hour before the transition
            // from the precalculated zone (which is -5) will therefore give an instant from
            // the tail zone which occurs before the precalculated-to-tail transition,
            // and can therefore be ignored, resulting in an overall unambiguous time.
            var tailZone = new FixedDateTimeZone(Offset.FromHours(5));
            var gapZone = new PrecalculatedDateTimeZone("Test",
                new[] { FirstInterval, SecondInterval, ThirdInterval }, tailZone);
            var pair = gapZone.GetZoneIntervalPair(ThirdInterval.LocalEnd - Duration.FromHours(1));
            Assert.AreEqual(ThirdInterval, pair.EarlyInterval);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_GapAroundTailZoneTransition()
        {
            // Tail zone is fixed at +5. A local instant of one hour after the transition
            // from the precalculated zone (which is -5) will therefore give an instant from
            // the tail zone which occurs before the precalculated-to-tail transition,
            // and can therefore be ignored, resulting in an overall gap.
            var tailZone = new FixedDateTimeZone(Offset.FromHours(5));
            var gapZone = new PrecalculatedDateTimeZone("Test", 
                new[] { FirstInterval, SecondInterval, ThirdInterval }, tailZone);
            var actual = gapZone.GetZoneIntervalPair(ThirdInterval.LocalEnd);
            var expected = ZoneIntervalPair.NoMatch;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetZoneIntervals_GapAroundAndInTailZoneTransition()
        {
            // Tail zone is -10 / +5, with the transition occurring just after
            // the transition *to* the tail zone from the precalculated zone.
            // A local instant of one hour after the transition from the precalculated zone (which is -5)
            // will therefore be in the gap. No zone interval matches, so the result is
            // an empty pair.
            var tailZone = new SingleTransitionDateTimeZone(ThirdInterval.End + Duration.FromHours(1), -10, +5);
            var gapZone = new PrecalculatedDateTimeZone("Test",
                new[] { FirstInterval, SecondInterval, ThirdInterval }, tailZone);
            var pair = gapZone.GetZoneIntervalPair(ThirdInterval.LocalEnd + Duration.FromHours(1));
            Assert.IsNull(pair.EarlyInterval);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_NullTailZone_Eot()
        {
            ZoneInterval[] intervals =
            {
                new ZoneInterval("foo", Instant.MinValue, new Instant(20), Offset.Zero, Offset.Zero),
                new ZoneInterval("foo", new Instant(20), Instant.MaxValue, Offset.Zero, Offset.Zero),                                       
            };
            var zone = new PrecalculatedDateTimeZone("Test", intervals, null);
            Assert.AreEqual(intervals[1], zone.GetZoneInterval(Instant.MaxValue));
        }

        [Test]
        public void Validation_EmptyPeriodArray()
        {
            Assert.Throws<ArgumentException>(() => PrecalculatedDateTimeZone.ValidatePeriods(new ZoneInterval[0],
                DateTimeZone.Utc));
        }

        [Test]
        public void Validation_BadFirstStartingPoint()
        {
            ZoneInterval[] intervals =
            {
                new ZoneInterval("foo", new Instant(10), new Instant(20), Offset.Zero, Offset.Zero),
                new ZoneInterval("foo", new Instant(20), new Instant(30), Offset.Zero, Offset.Zero),                                       
            };
            Assert.Throws<ArgumentException>(() => PrecalculatedDateTimeZone.ValidatePeriods(intervals, DateTimeZone.Utc));
        }

        [Test]
        public void Validation_NonAdjoiningIntervals()
        {
            ZoneInterval[] intervals =
            {
                new ZoneInterval("foo", Instant.MinValue, new Instant(20), Offset.Zero, Offset.Zero),
                new ZoneInterval("foo", new Instant(25), new Instant(30), Offset.Zero, Offset.Zero),                                       
            };
            Assert.Throws<ArgumentException>(() => PrecalculatedDateTimeZone.ValidatePeriods(intervals, DateTimeZone.Utc));
        }

        [Test]
        public void Validation_Success()
        {
            ZoneInterval[] intervals =
            {
                new ZoneInterval("foo", Instant.MinValue, new Instant(20), Offset.Zero, Offset.Zero),
                new ZoneInterval("foo", new Instant(20), new Instant(30), Offset.Zero, Offset.Zero),                                       
                new ZoneInterval("foo", new Instant(30), new Instant(100), Offset.Zero, Offset.Zero),                                       
                new ZoneInterval("foo", new Instant(100), new Instant(200), Offset.Zero, Offset.Zero),                                       
            };
            PrecalculatedDateTimeZone.ValidatePeriods(intervals, DateTimeZone.Utc);
        }

        [Test]
        public void Validation_NullTailZoneWithMiddleOfTimeFinalPeriod()
        {
            ZoneInterval[] intervals =
            {
                new ZoneInterval("foo", Instant.MinValue, new Instant(20), Offset.Zero, Offset.Zero),
                new ZoneInterval("foo", new Instant(20), new Instant(30), Offset.Zero, Offset.Zero)                                      
            };
            Assert.Throws<ArgumentException>(() => PrecalculatedDateTimeZone.ValidatePeriods(intervals, null));
        }

        [Test]
        public void Validation_NullTailZoneWithEotPeriodEnd()
        {
            ZoneInterval[] intervals =
            {
                new ZoneInterval("foo", Instant.MinValue, new Instant(20), Offset.Zero, Offset.Zero),
                new ZoneInterval("foo", new Instant(20), Instant.MaxValue, Offset.Zero, Offset.Zero),                                       
            };
            PrecalculatedDateTimeZone.ValidatePeriods(intervals, null);
        }

        [Test]
        public void Equals()
        {
            TestHelper.TestEqualsClass<DateTimeZone>
                (new PrecalculatedDateTimeZone("Test", new[] { FirstInterval, SecondInterval, ThirdInterval }, TailZone),
                 new PrecalculatedDateTimeZone("Test", new[] { FirstInterval, SecondInterval, ThirdInterval }, TailZone),
                 new PrecalculatedDateTimeZone("Test other ID", new[] { FirstInterval, SecondInterval, ThirdInterval }, TailZone));
            TestHelper.TestEqualsClass<DateTimeZone>
                (new PrecalculatedDateTimeZone("Test", new[] { FirstInterval, SecondInterval, ThirdInterval }, TailZone),
                 new PrecalculatedDateTimeZone("Test", new[] { FirstInterval, SecondInterval, ThirdInterval }, TailZone),
                 new PrecalculatedDateTimeZone("Test", new[] { SecondInterval.WithStart(Instant.MinValue), ThirdInterval }, TailZone));
        }
    }
}
