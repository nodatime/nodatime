// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    public class ZoneIntervalTest
    {
        private static readonly Instant SampleStart = Instant.FromUtc(2011, 6, 3, 10, 15);
        private static readonly Instant SampleEnd = Instant.FromUtc(2011, 8, 2, 13, 45);

        private static readonly ZoneInterval SampleInterval =
            new ZoneInterval("TestTime", SampleStart, SampleEnd,
                Offset.FromHours(9), Offset.FromHours(1));

        [Test]
        public void PassthroughProperties()
        {
            Assert.AreEqual("TestTime", SampleInterval.Name);
            Assert.AreEqual(Offset.FromHours(8), SampleInterval.StandardOffset);
            Assert.AreEqual(Offset.FromHours(1), SampleInterval.Savings);
            Assert.AreEqual(Offset.FromHours(9), SampleInterval.WallOffset);
            Assert.AreEqual(SampleStart, SampleInterval.Start);
            Assert.AreEqual(SampleEnd, SampleInterval.End);
        }

        // Having one test per property feels like a waste of time to me (Jon)...
        // If any of them fail, I'm going to be looking here anyway, and they're
        // fairly interrelated anyway.
        [Test]
        public void ComputedProperties()
        {
            LocalDateTime start = new LocalDateTime(2011, 6, 3, 19, 15);
            LocalDateTime end = new LocalDateTime(2011, 8, 2, 22, 45);
            Assert.AreEqual(start, SampleInterval.IsoLocalStart);
            Assert.AreEqual(end, SampleInterval.IsoLocalEnd);
            Assert.AreEqual(SampleEnd - SampleStart, SampleInterval.Duration);
        }

        [Test]
        public void Contains_Instant_Normal()
        {
            Assert.IsTrue(SampleInterval.Contains(SampleStart));
            Assert.IsFalse(SampleInterval.Contains(SampleEnd));
            Assert.IsFalse(SampleInterval.Contains(Instant.MinValue));
            Assert.IsFalse(SampleInterval.Contains(Instant.MaxValue));
        }

        [Test]
        public void Contains_Instant_WholeOfTime_ViaNullity()
        {
            ZoneInterval interval = new ZoneInterval("All Time", null, null,
                Offset.FromHours(9), Offset.FromHours(1));
            Assert.IsTrue(interval.Contains(SampleStart));
            Assert.IsTrue(interval.Contains(Instant.MinValue));
            Assert.IsTrue(interval.Contains(Instant.MaxValue));
        }

        [Test]
        public void Contains_Instant_WholeOfTime_ViaSpecialInstants()
        {
            ZoneInterval interval = new ZoneInterval("All Time", Instant.BeforeMinValue, Instant.AfterMaxValue,
                Offset.FromHours(9), Offset.FromHours(1));
            Assert.IsTrue(interval.Contains(SampleStart));
            Assert.IsTrue(interval.Contains(Instant.MinValue));
            Assert.IsTrue(interval.Contains(Instant.MaxValue));
        }

        [Test]
        public void Contains_LocalInstant_WholeOfTime()
        {
            ZoneInterval interval = new ZoneInterval("All Time", Instant.BeforeMinValue, Instant.AfterMaxValue,
                Offset.FromHours(9), Offset.FromHours(1));
            Assert.IsTrue(interval.Contains(SampleStart.Plus(Offset.Zero)));
            Assert.IsTrue(interval.Contains(Instant.MinValue.Plus(Offset.Zero)));
            Assert.IsTrue(interval.Contains(Instant.MaxValue.Plus(Offset.Zero)));
        }

        [Test]
        public void Contains_OutsideLocalInstantange()
        {
            ZoneInterval veryEarly = new ZoneInterval("Very early", Instant.BeforeMinValue, Instant.MinValue + Duration.FromHours(8), Offset.FromHours(-9), Offset.Zero);
            ZoneInterval veryLate = new ZoneInterval("Very late", Instant.MaxValue - Duration.FromHours(8), Instant.AfterMaxValue, Offset.FromHours(9), Offset.Zero);
            // The instants are contained...
            Assert.IsTrue(veryEarly.Contains(Instant.MinValue + Duration.FromHours(4)));
            Assert.IsTrue(veryLate.Contains(Instant.MaxValue - Duration.FromHours(4)));
            // But there are no valid local instants
            Assert.IsFalse(veryEarly.Contains(Instant.MinValue.Plus(Offset.Zero)));
            Assert.IsFalse(veryLate.Contains(Instant.MaxValue.Plus(Offset.Zero)));
        }

        [Test]
        public void IsoLocalStartAndEnd_Infinite()
        {
            var interval = new ZoneInterval("All time", null, null, Offset.Zero, Offset.Zero);
            Assert.Throws<InvalidOperationException>(() => interval.IsoLocalStart.ToString());
            Assert.Throws<InvalidOperationException>(() => interval.IsoLocalEnd.ToString());
        }

        [Test]
        public void IsoLocalStartAndEnd_OutOfRange()
        {
            var interval = new ZoneInterval("All time", Instant.MinValue, null, Offset.FromHours(-1), Offset.Zero);
            Assert.Throws<OverflowException>(() => interval.IsoLocalStart.ToString());
            interval = new ZoneInterval("All time", null, Instant.MaxValue, Offset.FromHours(11), Offset.Zero);
            Assert.Throws<OverflowException>(() => interval.IsoLocalEnd.ToString());
        }

        [Test]
        public void Equality()
        {
            TestHelper.TestEqualsClass(
                // Equal values
                new ZoneInterval("name", SampleStart, SampleEnd, Offset.FromHours(1), Offset.FromHours(2)),
                new ZoneInterval("name", SampleStart, SampleEnd, Offset.FromHours(1), Offset.FromHours(2)),
                // Unequal values
                new ZoneInterval("name2", SampleStart, SampleEnd, Offset.FromHours(1), Offset.FromHours(2)),
                new ZoneInterval("name", SampleStart.PlusNanoseconds(1), SampleEnd, Offset.FromHours(1), Offset.FromHours(2)),
                new ZoneInterval("name", SampleStart, SampleEnd.PlusNanoseconds(1), Offset.FromHours(1), Offset.FromHours(2)),
                new ZoneInterval("name", SampleStart, SampleEnd, Offset.FromHours(2), Offset.FromHours(2)),
                new ZoneInterval("name", SampleStart, SampleEnd, Offset.FromHours(1), Offset.FromHours(3)));
        }
    }
}
