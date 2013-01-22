// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class IntervalTest
    {
        private static readonly Instant SampleStart = new Instant(-300);
        private static readonly Instant SampleEnd = new Instant(400);

        [Test]
        public void Construction_Success()
        {
            var interval = new Interval(SampleStart, SampleEnd);
            Assert.AreEqual(SampleStart, interval.Start);
            Assert.AreEqual(SampleEnd, interval.End);
        }

        [Test]
        public void Construction_EqualStartAndEnd()
        {
            var interval = new Interval(SampleStart, SampleStart);
            Assert.AreEqual(SampleStart, interval.Start);
            Assert.AreEqual(SampleStart, interval.End);
            Assert.AreEqual(new Duration(0), interval.Duration);
        }

        [Test]
        public void Construction_EndBeforeStart()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Interval(SampleEnd, SampleStart));
        }

        [Test]
        public void Equals()
        {
            TestHelper.TestEqualsStruct(
                new Interval(SampleStart, SampleEnd),
                new Interval(SampleStart, SampleEnd),
                new Interval(NodaConstants.UnixEpoch, SampleEnd));
        }

        [Test]
        public void Operators()
        {
            TestHelper.TestOperatorEquality(
                new Interval(SampleStart, SampleEnd),
                new Interval(SampleStart, SampleEnd),
                new Interval(NodaConstants.UnixEpoch, SampleEnd));
        }

        [Test]
        public void Duration()
        {
            var interval = new Interval(SampleStart, SampleEnd);
            Assert.AreEqual(new Duration(700), interval.Duration);
        }

        /// <summary>
        ///   Using the default constructor is equivalent to a zero duration.
        /// </summary>
        [Test]
        public void DefaultConstructor()
        {
            var actual = new Interval();
            Assert.AreEqual(NodaTime.Duration.Zero, actual.Duration);
        }

    }
}
