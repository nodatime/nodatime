// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using NodaTime.Text;

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

        [Test]
        public void BinarySerialization()
        {
            TestHelper.AssertBinaryRoundtrip(new Interval(SampleStart, SampleEnd));
        }

        [Test]
        public void XmlSerialization()
        {
            var start = new LocalDateTime(2013, 4, 12, 17, 53, 23, 123, 4567).InUtc().ToInstant();
            var end = new LocalDateTime(2013, 10, 12, 17, 1, 2).InUtc().ToInstant();
            var value = new Interval(start, end);
            TestHelper.AssertXmlRoundtrip(value, "<value start=\"2013-04-12T17:53:23.1234567Z\" end=\"2013-10-12T17:01:02Z\" />");
        }

        [Test]
        public void XmlSerialization_ExtraContent()
        {
            var start = new LocalDateTime(2013, 4, 12, 17, 53, 23, 123, 4567).InUtc().ToInstant();
            var end = new LocalDateTime(2013, 10, 12, 17, 1, 2).InUtc().ToInstant();
            var value = new Interval(start, end);
            TestHelper.AssertParsableXml(value,
                "<value start=\"2013-04-12T17:53:23.1234567Z\" end=\"2013-10-12T17:01:02Z\">Text<child attr=\"value\"/>Text 2</value>");
        }

        [Test]
        public void XmlSerialization_SwapAttributeOrder()
        {
            var start = new LocalDateTime(2013, 4, 12, 17, 53, 23, 123, 4567).InUtc().ToInstant();
            var end = new LocalDateTime(2013, 10, 12, 17, 1, 2).InUtc().ToInstant();
            var value = new Interval(start, end);
            TestHelper.AssertParsableXml(value, "<value end=\"2013-10-12T17:01:02Z\" start=\"2013-04-12T17:53:23.1234567Z\" />");
        }

        [Test]
        [TestCase("<value start=\"2013-15-12T17:53:23Z\" end=\"2013-11-12T17:53:23Z\"/>",
            typeof(UnparsableValueException), Description = "Invalid month in start")]
        [TestCase("<value start=\"2013-11-12T17:53:23Z\" end=\"2013-15-12T17:53:23Z\"/>",
            typeof(UnparsableValueException), Description = "Invalid month in end")]
        [TestCase("<value start=\"2013-11-12T17:53:23Z\" end=\"2013-11-12T16:53:23Z\"/>",
            typeof(ArgumentOutOfRangeException), Description = "End before start")]
        [TestCase("<value start=\"2013-11-12T17:53:23Z\"/>", typeof(ArgumentException), Description = "No end")]
        [TestCase("<value end=\"2013-11-12T16:53:23Z\"/>", typeof(ArgumentException), Description = "No start")]
        public void XmlSerialization_Invalid(string xml, Type expectedExceptionType)
        {
            TestHelper.AssertXmlInvalid<Interval>(xml, expectedExceptionType);
        }
    }
}
