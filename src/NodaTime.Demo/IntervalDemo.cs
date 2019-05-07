// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Demo
{
    public class IntervalDemo
    {
        [Test]
        public void Construction_WithNonNullableParameters()
        {
            Instant start = Instant.FromUtc(2019, 1, 1, 15, 25, 48);
            Instant end = Instant.FromUtc(2019, 1, 1, 16, 25, 48);
            Interval interval = Snippet.For(new Interval(start, end));
            Assert.AreEqual(start, interval.Start);
            Assert.AreEqual(end, interval.End);
        }

        [Test]
        public void Construction_WithNullableParameters()
        {
            Instant end = Instant.FromUtc(2019, 1, 1, 16, 25, 48);
            Interval interval = Snippet.For(new Interval(null, end));
            Assert.False(interval.HasStart);
            Assert.True(interval.HasEnd);
            Assert.AreEqual(end, interval.End);
        }
                
        [Test]
        public void Contains()
        {
            Instant start = Instant.FromUtc(2019, 1, 1, 15, 25, 48);
            Instant end = Instant.FromUtc(2019, 1, 1, 16, 25, 48);
            Interval interval = new Interval(start, end);
            Assert.True(Snippet.For(interval.Contains(Instant.FromUtc(2019, 1, 1, 15, 50, 50))));
        }
    }
}
