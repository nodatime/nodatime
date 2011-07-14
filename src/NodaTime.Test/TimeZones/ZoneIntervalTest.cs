using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class ZoneIntervalTest
    {
        private static readonly Instant SampleStart =  Instant.FromUtc(2011, 6, 3, 10, 15);
        private static readonly Instant SampleEnd =  Instant.FromUtc(2011, 8, 2, 13, 45);

        private static readonly ZoneInterval SampleInterval =
            new ZoneInterval("TestTime", SampleStart, SampleEnd,
                Offset.ForHours(9), Offset.ForHours(1));

        [Test]
        public void PassthroughProperties()
        {
            Assert.AreEqual("TestTime", SampleInterval.Name);
            Assert.AreEqual(Offset.ForHours(8), SampleInterval.BaseOffset);
            Assert.AreEqual(Offset.ForHours(1), SampleInterval.Savings);
            Assert.AreEqual(Offset.ForHours(9), SampleInterval.Offset);
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
    }
}
