// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test
{
    public class IntervalTest
    {
        private static readonly Instant SampleStart = NodaConstants.UnixEpoch.PlusNanoseconds(-30001);
        private static readonly Instant SampleEnd = NodaConstants.UnixEpoch.PlusNanoseconds(40001);

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
            Assert.AreEqual(NodaTime.Duration.Zero, interval.Duration);
        }

        [Test]
        public void Construction_EndBeforeStart()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Interval(SampleEnd, SampleStart));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Interval((Instant?) SampleEnd, (Instant?) SampleStart));
        }

        [Test]
        public void Equals()
        {
            TestHelper.TestEqualsStruct(
                new Interval(SampleStart, SampleEnd),
                new Interval(SampleStart, SampleEnd),
                new Interval(NodaConstants.UnixEpoch, SampleEnd));
            TestHelper.TestEqualsStruct(
                new Interval(null, SampleEnd),
                new Interval(null, SampleEnd),
                new Interval(NodaConstants.UnixEpoch, SampleEnd));
            TestHelper.TestEqualsStruct(
                new Interval(SampleStart, SampleEnd),
                new Interval(SampleStart, SampleEnd),
                new Interval(NodaConstants.UnixEpoch, SampleEnd));
            TestHelper.TestEqualsStruct(
                new Interval(null, null),
                new Interval(null, null),
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
            Assert.AreEqual(NodaTime.Duration.FromNanoseconds(70002), interval.Duration);
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
            TestHelper.AssertBinaryRoundtrip(new Interval(null, SampleEnd));
            TestHelper.AssertBinaryRoundtrip(new Interval(SampleStart, null));
            TestHelper.AssertBinaryRoundtrip(new Interval(null, null));
        }

        [Test]
        public void ToStringUsesExtendedIsoFormat()
        {
            var start = new LocalDateTime(2013, 4, 12, 17, 53, 23).PlusNanoseconds(123456789).InUtc().ToInstant();
            var end = new LocalDateTime(2013, 10, 12, 17, 1, 2, 120).InUtc().ToInstant();
            var value = new Interval(start, end);
            Assert.AreEqual("2013-04-12T17:53:23.123456789Z/2013-10-12T17:01:02.12Z", value.ToString());
        }

        [Test]
        public void ToString_Infinite()
        {
            var value = new Interval(null, null);
            Assert.AreEqual("StartOfTime/EndOfTime", value.ToString());
        }

        [Test]
        public void XmlSerialization()
        {
            var start = new LocalDateTime(2013, 4, 12, 17, 53, 23).PlusNanoseconds(123456789).InUtc().ToInstant();
            var end = new LocalDateTime(2013, 10, 12, 17, 1, 2).InUtc().ToInstant();
            var value = new Interval(start, end);
            TestHelper.AssertXmlRoundtrip(value, "<value start=\"2013-04-12T17:53:23.123456789Z\" end=\"2013-10-12T17:01:02Z\" />");
        }

        [Test]
        public void XmlSerialization_Extremes()
        {
            var value = new Interval(Instant.MinValue, Instant.MaxValue);
            TestHelper.AssertXmlRoundtrip(value, "<value start=\"-9998-01-01T00:00:00Z\" end=\"9999-12-31T23:59:59.999999999Z\" />");
        }

        [Test]
        public void XmlSerialization_ExtraContent()
        {
            var start = new LocalDateTime(2013, 4, 12, 17, 53, 23).PlusNanoseconds(123456789).InUtc().ToInstant();
            var end = new LocalDateTime(2013, 10, 12, 17, 1, 2).InUtc().ToInstant();
            var value = new Interval(start, end);
            TestHelper.AssertParsableXml(value,
                "<value start=\"2013-04-12T17:53:23.123456789Z\" end=\"2013-10-12T17:01:02Z\">Text<child attr=\"value\"/>Text 2</value>");
        }

        [Test]
        public void XmlSerialization_SwapAttributeOrder()
        {
            var start = new LocalDateTime(2013, 4, 12, 17, 53, 23).PlusNanoseconds(123456789).InUtc().ToInstant();
            var end = new LocalDateTime(2013, 10, 12, 17, 1, 2).InUtc().ToInstant();
            var value = new Interval(start, end);
            TestHelper.AssertParsableXml(value, "<value end=\"2013-10-12T17:01:02Z\" start=\"2013-04-12T17:53:23.123456789Z\" />");
        }

        [Test]
        public void XmlSerialization_FromBeginningOfTime()
        {
            var end = new LocalDateTime(2013, 10, 12, 17, 1, 2).InUtc().ToInstant();
            var value = new Interval(null, end);
            TestHelper.AssertXmlRoundtrip(value, "<value end=\"2013-10-12T17:01:02Z\" />");
        }

        [Test]
        public void XmlSerialization_ToEndOfTime()
        {
            var start = new LocalDateTime(2013, 10, 12, 17, 1, 2).InUtc().ToInstant();
            var value = new Interval(start, null);
            TestHelper.AssertXmlRoundtrip(value, "<value start=\"2013-10-12T17:01:02Z\" />");
        }

        [Test]
        public void XmlSerialization_AllOfTime()
        {
            var value = new Interval(null, null);
            TestHelper.AssertXmlRoundtrip(value, "<value />");
        }

        [Test]
        [TestCase("<value start=\"2013-15-12T17:53:23Z\" end=\"2013-11-12T17:53:23Z\"/>",
            typeof(UnparsableValueException), Description = "Invalid month in start")]
        [TestCase("<value start=\"2013-11-12T17:53:23Z\" end=\"2013-15-12T17:53:23Z\"/>",
            typeof(UnparsableValueException), Description = "Invalid month in end")]
        [TestCase("<value start=\"2013-11-12T17:53:23Z\" end=\"2013-11-12T16:53:23Z\"/>",
            typeof(ArgumentOutOfRangeException), Description = "End before start")]
        public void XmlSerialization_Invalid(string xml, Type expectedExceptionType)
        {
            TestHelper.AssertXmlInvalid<Interval>(xml, expectedExceptionType);
        }

#if !NETCORE

        [Test]
        [TestCase(typeof(OverflowException), Instant.MinDays - 1, 0L, 0, 0L)]
        [TestCase(typeof(OverflowException), Instant.MaxDays + 1, 0L, 0, 0L)]
        [TestCase(typeof(ArgumentException), 0, -1L, 0, 0L)]
        [TestCase(typeof(ArgumentException), 0, NodaConstants.NanosecondsPerDay, 0, 0L)]
        [TestCase(typeof(OverflowException), 0, 0L, Instant.MinDays - 1, 0L)]
        [TestCase(typeof(OverflowException), 0, 0L, Instant.MaxDays + 1, 0L)]
        [TestCase(typeof(ArgumentException), 0, 0L, 0, -1L)]
        [TestCase(typeof(ArgumentException), 0, 0L, 0, NodaConstants.NanosecondsPerDay)]
        [TestCase(typeof(ArgumentException), 0, 0L, -1, 0L)] // End before start
        public void InvalidBinaryData(Type expectedExceptionType, int startDays, long startNanoOfDay, int endDays, long endNanoOfDay) =>
            TestHelper.AssertBinaryDeserializationFailure<Interval>(expectedExceptionType, info =>
            {
                info.AddValue(BinaryFormattingConstants.PresenceName, 3);
                info.AddValue(BinaryFormattingConstants.StartDaysSerializationName, startDays);
                info.AddValue(BinaryFormattingConstants.StartNanosecondOfDaySerializationName, startNanoOfDay);
                info.AddValue(BinaryFormattingConstants.EndDaysSerializationName, endDays);
                info.AddValue(BinaryFormattingConstants.EndNanosecondOfDaySerializationName, endNanoOfDay);
            });
#endif

        [Test]
        [TestCase("1990-01-01T00:00:00Z", false, Description = "Before interval")]
        [TestCase("2000-01-01T00:00:00Z", true, Description = "Start of interval")]
        [TestCase("2010-01-01T00:00:00Z", true, Description = "Within interval")]
        [TestCase("2020-01-01T00:00:00Z", false, Description = "End instant of interval")]
        [TestCase("2030-01-01T00:00:00Z", false, Description = "After interval")]
        public void Contains(string candidateText, bool expectedResult)
        {
            var start = Instant.FromUtc(2000, 1, 1, 0, 0);
            var end = Instant.FromUtc(2020, 1, 1, 0, 0);
            var interval = new Interval(start, end);
            var candidate = InstantPattern.ExtendedIso.Parse(candidateText).Value;
            Assert.AreEqual(expectedResult, interval.Contains(candidate));
        }

        [Test]
        public void Contains_Infinite()
        {
            var interval = new Interval(null, null);
            Assert.IsTrue(interval.Contains(Instant.MaxValue));
            Assert.IsTrue(interval.Contains(Instant.MinValue));
        }

        [Test]
        public void HasStart()
        {
            Assert.IsTrue(new Interval(Instant.MinValue, null).HasStart);
            Assert.IsFalse(new Interval(null, Instant.MinValue).HasStart);
        }

        [Test]
        public void HasEnd()
        {
            Assert.IsTrue(new Interval(null, Instant.MaxValue).HasEnd);
            Assert.IsFalse(new Interval(Instant.MaxValue, null).HasEnd);
        }

        [Test]
        public void Start()
        {
            Assert.AreEqual(NodaConstants.UnixEpoch, new Interval(NodaConstants.UnixEpoch, null).Start);
            Interval noStart = new Interval(null, NodaConstants.UnixEpoch);
            Assert.Throws<InvalidOperationException>(() => noStart.Start.ToString());
        }

        [Test]
        public void End()
        {
            Assert.AreEqual(NodaConstants.UnixEpoch, new Interval(null, NodaConstants.UnixEpoch).End);
            Interval noEnd = new Interval(NodaConstants.UnixEpoch, null);
            Assert.Throws<InvalidOperationException>(() => noEnd.End.ToString());
        }

        [Test]
        public void Contains_EmptyInterval()
        {
            var instant = NodaConstants.UnixEpoch;
            var interval = new Interval(instant, instant);
            Assert.IsFalse(interval.Contains(instant));
        }

        [Test]
        public void Contains_EmptyInterval_MaxValue()
        {
            var instant = Instant.MaxValue;
            var interval = new Interval(instant, instant);
            // This would have been true under Noda Time 1.x
            Assert.IsFalse(interval.Contains(instant));
        }

        [Test]
        public void Deconstruction()
        {
            var start = new Instant();
            var end = start.PlusTicks(1_000_000);
            var value = new Interval(start, end);

            (Instant? actualStart, Instant? actualEnd) = value;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(start, actualStart);
                Assert.AreEqual(end, actualEnd);
            });
        }
        
        [Test]
        public void Deconstruction_IntervalWithoutStart()
        {
            Instant? start = null;
            var end = new Instant(1500, 1_000_000);
            var value = new Interval(start, end);

            (Instant? actualStart, Instant? actualEnd) = value;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(start, actualStart);
                Assert.AreEqual(end, actualEnd);
            });
        }

        [Test]
        public void Deconstruction_IntervalWithoutEnd()
        {
            var start = new Instant(1500, 1_000_000);
            Instant? end = null;
            var value = new Interval(start, end);

            (Instant? actualStart, Instant? actualEnd) = value;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(start, actualStart);
                Assert.AreEqual(end, actualEnd);
            });
        }
    }
}
