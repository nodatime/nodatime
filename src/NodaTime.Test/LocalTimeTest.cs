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
    }
}