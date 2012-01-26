#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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

using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class ZoneIntervalTest
    {
        private static readonly Instant SampleStart =  Instant.FromUtc(2011, 6, 3, 10, 15);
        private static readonly Instant SampleEnd =  Instant.FromUtc(2011, 8, 2, 13, 45);

        private static readonly ZoneInterval SampleInterval =
            new ZoneInterval("TestTime", SampleStart, SampleEnd,
                Offset.FromHours(9), Offset.FromHours(1));

        [Test]
        public void PassthroughProperties()
        {
            Assert.AreEqual("TestTime", SampleInterval.Name);
            Assert.AreEqual(Offset.FromHours(8), SampleInterval.StandardOffset);
            Assert.AreEqual(Offset.FromHours(1), SampleInterval.Savings);
            Assert.AreEqual(Offset.FromHours(9), SampleInterval.Offset);
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
            Assert.AreEqual(start.LocalInstant, SampleInterval.LocalStart);
            Assert.AreEqual(end, SampleInterval.IsoLocalEnd);
            Assert.AreEqual(end.LocalInstant, SampleInterval.LocalEnd);
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
        public void Contains_Instant_WholeOfTime()
        {
            ZoneInterval interval = new ZoneInterval("All Time", Instant.MinValue, Instant.MaxValue,
                Offset.FromHours(9), Offset.FromHours(1));
            Assert.IsTrue(interval.Contains(SampleStart));
            Assert.IsTrue(interval.Contains(Instant.MinValue));
            Assert.IsTrue(interval.Contains(Instant.MaxValue));
        }

        [Test]
        public void Contains_LocalInstant_WholeOfTime()
        {
            ZoneInterval interval = new ZoneInterval("All Time", Instant.MinValue, Instant.MaxValue,
                Offset.FromHours(9), Offset.FromHours(1));
            Assert.IsTrue(interval.Contains(SampleStart.Plus(Offset.Zero)));
            Assert.IsTrue(interval.Contains(LocalInstant.MinValue));
            Assert.IsTrue(interval.Contains(LocalInstant.MaxValue));
        }
    }
}
