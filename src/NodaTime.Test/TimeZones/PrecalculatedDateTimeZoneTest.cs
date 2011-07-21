#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class PrecalculatedDateTimeZoneTest
    {
        private static readonly ZoneInterval FirstInterval =
            new ZoneInterval("First", Instant.MinValue, Instant.FromUtc(2000, 3, 10, 10, 0), Offset.ForHours(3), Offset.Zero);

        // Note that this is effectively UTC +3 + 1 hour DST.
        private static readonly ZoneInterval SecondInterval =
            new ZoneInterval("Second", FirstInterval.End, Instant.FromUtc(2000, 9, 15, 5, 0), Offset.ForHours(4), Offset.ForHours(1));

        private static readonly ZoneInterval ThirdInterval =
            new ZoneInterval("Third", SecondInterval.End, Instant.FromUtc(2005, 6, 20, 8, 0), Offset.ForHours(-5), Offset.Zero);

        private static readonly FixedDateTimeZone TailZone = new FixedDateTimeZone("TestFixed", Offset.ForHours(-6));

        // We don't actually want an interval from the beginning of time when we ask our composite time zone for an interval
        // - because that could give the wrong idea. So we clamp it at the end of the precalculated interval.
        private static readonly ZoneInterval ClampedTailZoneInterval = TailZone.GetZoneInterval(Instant.UnixEpoch).WithStart(ThirdInterval.End);

        private static readonly PrecalculatedDateTimeZone TestZone =
            new PrecalculatedDateTimeZone("Test", new[] { FirstInterval, SecondInterval, ThirdInterval }, TailZone);

        [Test]
        public void MinMaxOffsets()
        {
            Assert.AreEqual(Offset.ForHours(-6), TestZone.MinOffset);
            Assert.AreEqual(Offset.ForHours(4), TestZone.MaxOffset);
        }

        [Test]
        public void MinMaxOffsetsWithOtherTailZone()
        {
            var tailZone = new FixedDateTimeZone("TestFixed", Offset.ForHours(8));
            var testZone = new PrecalculatedDateTimeZone("Test",
                new[] { FirstInterval, SecondInterval, ThirdInterval }, tailZone);
            Assert.AreEqual(Offset.ForHours(-5), testZone.MinOffset);
            Assert.AreEqual(Offset.ForHours(8), testZone.MaxOffset);
        }

        [Test]
        public void MinMaxOffsetsWithNullTailZone()
        {
            var testZone = new PrecalculatedDateTimeZone("Test",
                new[] { FirstInterval, SecondInterval, ThirdInterval,
                        new ZoneInterval("Last", ThirdInterval.End, Instant.MaxValue, Offset.Zero, Offset.Zero) }, null);
            Assert.AreEqual(Offset.ForHours(-5), testZone.MinOffset);
            Assert.AreEqual(Offset.ForHours(4), testZone.MaxOffset);
        }

        [Test]
        public void GetZoneIntervalInstant_End()
        {
            Assert.AreEqual(SecondInterval, TestZone.GetZoneInterval(SecondInterval.End - Duration.One));
        }

        [Test]
        public void GetZoneIntervalInstant_Start()
        {
            Assert.AreEqual(SecondInterval, TestZone.GetZoneInterval(SecondInterval.Start));
        }

        [Test]
        public void GetZoneIntervalInstant_TailZone()
        {
            Assert.AreEqual(ClampedTailZoneInterval, TestZone.GetZoneInterval(ThirdInterval.End));
        }

        [Test]
        public void GetZoneIntervals_UnambiguousInPrecalculated()
        {
            var pair = TestZone.GetZoneIntervals(new LocalInstant(2000, 6, 1, 0, 0));
            Assert.AreEqual(SecondInterval, pair.EarlyInterval);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_UnambiguousInTailZone()
        {
            var pair = TestZone.GetZoneIntervals(new LocalInstant(2015, 1, 1, 0, 0));
            Assert.AreEqual(ClampedTailZoneInterval, pair.EarlyInterval);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_AmbiguousWithinPrecalculated()
        {
            // Transition from +4 to -5 has a 9 hour ambiguity
            var pair = TestZone.GetZoneIntervals(ThirdInterval.LocalStart);
            Assert.AreEqual(SecondInterval, pair.EarlyInterval);
            Assert.AreEqual(ThirdInterval, pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_AmbiguousAroundTailZoneTransition()
        {
            // Transition from -5 to -6 has a 1 hour ambiguity
            var pair = TestZone.GetZoneIntervals(ThirdInterval.LocalEnd - Duration.One);
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
            var tailZone = new SingleTransitionZone(ThirdInterval.End + Duration.FromHours(1), 10, 8);
            var gapZone = new PrecalculatedDateTimeZone("Test",
                new[] { FirstInterval, SecondInterval, ThirdInterval }, tailZone);
            var pair = gapZone.GetZoneIntervals(ThirdInterval.LocalEnd - Duration.FromHours(1));
            Assert.AreEqual(ThirdInterval, pair.EarlyInterval);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_GapWithinPrecalculated()
        {
            // Transition from +3 to +4 has a 1 hour gap
            Assert.IsTrue(FirstInterval.LocalEnd < SecondInterval.LocalStart);
            var pair = TestZone.GetZoneIntervals(FirstInterval.LocalEnd);
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
            var tailZone = new FixedDateTimeZone(Offset.ForHours(5));
            var gapZone = new PrecalculatedDateTimeZone("Test",
                new[] { FirstInterval, SecondInterval, ThirdInterval }, tailZone);
            var pair = gapZone.GetZoneIntervals(ThirdInterval.LocalEnd - Duration.FromHours(1));
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
            var tailZone = new FixedDateTimeZone(Offset.ForHours(5));
            var gapZone = new PrecalculatedDateTimeZone("Test", 
                new[] { FirstInterval, SecondInterval, ThirdInterval }, tailZone);
            var actual = gapZone.GetZoneIntervals(ThirdInterval.LocalEnd);
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
            var tailZone = new SingleTransitionZone(ThirdInterval.End + Duration.FromHours(1), -10, +5);
            var gapZone = new PrecalculatedDateTimeZone("Test",
                new[] { FirstInterval, SecondInterval, ThirdInterval }, tailZone);
            var pair = gapZone.GetZoneIntervals(ThirdInterval.LocalEnd + Duration.FromHours(1));
            Assert.IsNull(pair.EarlyInterval);
            Assert.IsNull(pair.LateInterval);
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

    }
}