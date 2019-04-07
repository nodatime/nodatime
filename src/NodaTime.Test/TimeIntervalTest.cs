// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test
{
    public class TimeIntervalTest
    {
        private static readonly LocalTime SampleStart = new LocalTime(10, 00);
        private static readonly LocalTime SampleEnd = new LocalTime(17, 00);
        
        private static readonly LocalTime SampleStartInverted = new LocalTime(23, 00);
        private static readonly LocalTime SampleEndInverted = new LocalTime(1, 00);

        [Test]
        public void Duration_Success()
        {
            var interval = new TimeInterval(SampleStart, SampleEnd);
            Assert.AreEqual(Duration.FromHours(7), interval.Duration);
        }
        [Test]
        public void Period_Success()
        {
            var interval = new TimeInterval(SampleStart, SampleEnd);
            Assert.AreEqual(Period.FromHours(7), interval.Period);
        }
        
        [Test]
        public void InvertedTimeInterval_Duration_Success()
        {
            var interval = new TimeInterval(SampleStartInverted, SampleEndInverted);
            Assert.AreEqual(Duration.FromHours(2), interval.Duration);
        }
        
        [Test]
        public void InvertedTimeInterval_Period_Success()
        {
            var interval = new TimeInterval(SampleStartInverted, SampleEndInverted);
            Assert.AreEqual(Period.FromHours(2), interval.Period);
        }
        
        [Test]
        public void InvertedTimeInterval_Contains_Success()
        {
            var interval = new TimeInterval(SampleStartInverted, SampleEndInverted);
            Assert.IsTrue(interval.Contains(new LocalTime(23, 00)));
            Assert.IsTrue(interval.Contains(new LocalTime(23, 30)));
            Assert.IsTrue(interval.Contains(new LocalTime(00, 00)));
            Assert.IsTrue(interval.Contains(new LocalTime(00, 30)));
            Assert.IsTrue(interval.Contains(new LocalTime(00, 59)));
        }
        
        [Test]
        public void InvertedTimeInterval_NotContains_Success()
        {
            var interval = new TimeInterval(SampleStartInverted, SampleEndInverted);
            Assert.IsFalse(interval.Contains(new LocalTime(1, 00)));
            Assert.IsFalse(interval.Contains(new LocalTime(1, 01)));
            Assert.IsFalse(interval.Contains(new LocalTime(13, 00)));
            Assert.IsFalse(interval.Contains(new LocalTime(22, 30)));
            Assert.IsFalse(interval.Contains(new LocalTime(22, 59)));
        }

    }
}
