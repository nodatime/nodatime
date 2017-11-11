// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalTimeTest
    {
        [Test]
        public void MinValueEqualToMidnight()
        {
            Assert.AreEqual(LocalTime.Midnight, LocalTime.MinValue);
        }

        [Test]
        public void MaxValue()
        {
            Assert.AreEqual(NodaConstants.NanosecondsPerDay - 1, LocalTime.MaxValue.NanosecondOfDay);
        }

        [Test]
        public void ClockHourOfHalfDay()
        {
            Assert.AreEqual(12, new LocalTime(0, 0).ClockHourOfHalfDay);
            Assert.AreEqual(1, new LocalTime(1, 0).ClockHourOfHalfDay);
            Assert.AreEqual(12, new LocalTime(12, 0).ClockHourOfHalfDay);
            Assert.AreEqual(1, new LocalTime(13, 0).ClockHourOfHalfDay);
            Assert.AreEqual(11, new LocalTime(23, 0).ClockHourOfHalfDay);
        }

        /// <summary>
        ///   Using the default constructor is equivalent to midnight
        /// </summary>
        [Test]
        public void DefaultConstructor()
        {
            var actual = new LocalTime();
            Assert.AreEqual(LocalTime.Midnight, actual);
        }

        [Test]
        public void BinarySerialization()
        {
            TestHelper.AssertBinaryRoundtrip(LocalTime.FromHourMinuteSecondNanosecond(12, 34, 56, 123456789));
        }

#if !NETCORE

        [Test]
        [TestCase(typeof(ArgumentException), -1L)]
        [TestCase(typeof(ArgumentException), NodaConstants.NanosecondsPerDay)]
        public void InvalidBinaryData(Type expectedExceptionType, long nanoOfDay) =>
            TestHelper.AssertBinaryDeserializationFailure<LocalTime>(expectedExceptionType, info =>
            {
                info.AddValue(BinaryFormattingConstants.NanoOfDaySerializationName, nanoOfDay);
            });
#endif

        [Test]
        public void XmlSerialization()
        {
            var value = LocalTime.FromHourMinuteSecondNanosecond(17, 53, 23, 123456789);
            TestHelper.AssertXmlRoundtrip(value, "<value>17:53:23.123456789</value>");
        }

        [Test]
        [TestCase("<value>25:53:23</value>", typeof(UnparsableValueException), Description = "Invalid hour")]
        public void XmlSerialization_Invalid(string xml, Type expectedExceptionType)
        {
            TestHelper.AssertXmlInvalid<LocalTime>(xml, expectedExceptionType);
        }

        [Test]
        public void Max()
        {
            LocalTime x = new LocalTime(5, 10);
            LocalTime y = new LocalTime(6, 20);
            Assert.AreEqual(y, LocalTime.Max(x, y));
            Assert.AreEqual(y, LocalTime.Max(y, x));
            Assert.AreEqual(x, LocalTime.Max(x, LocalTime.MinValue));
            Assert.AreEqual(x, LocalTime.Max(LocalTime.MinValue, x));
            Assert.AreEqual(LocalTime.MaxValue, LocalTime.Max(LocalTime.MaxValue, x));
            Assert.AreEqual(LocalTime.MaxValue, LocalTime.Max(x, LocalTime.MaxValue));
        }

        [Test]
        public void Min()
        {
            LocalTime x = new LocalTime(5, 10);
            LocalTime y = new LocalTime(6, 20);
            Assert.AreEqual(x, LocalTime.Min(x, y));
            Assert.AreEqual(x, LocalTime.Min(y, x));
            Assert.AreEqual(LocalTime.MinValue, LocalTime.Min(x, LocalTime.MinValue));
            Assert.AreEqual(LocalTime.MinValue, LocalTime.Min(LocalTime.MinValue, x));
            Assert.AreEqual(x, LocalTime.Min(LocalTime.MaxValue, x));
            Assert.AreEqual(x, LocalTime.Min(x, LocalTime.MaxValue));
        }

        [Test]
        public void Deconstruction()
        {
            var value = new LocalTime(15, 8, 20);
            var expectedHour = 15;
            var expectedMinute = 8;
            var expectedSecond = 20;

            var (actualHour, actualMinute, actualSecond) = value;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedHour, actualHour);
                Assert.AreEqual(expectedMinute, actualMinute);
                Assert.AreEqual(expectedSecond, actualSecond);
            });
        }

        [Test]
        public void WithOffset()
        {
            var time = new LocalTime(3, 45, 12, 34);
            var offset = Offset.FromHours(5);
            var expected = new OffsetTime(time, offset);
            Assert.AreEqual(expected, time.WithOffset(offset));
        }
    }
}