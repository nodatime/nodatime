// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class LocalTimeTest
    {
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
            TestHelper.AssertBinaryRoundtrip(new LocalTime(12, 34, 56, 123, 4567));
        }

        [Test]
        public void XmlSerialization()
        {
            var value = new LocalTime(17, 53, 23, 123, 4567);
            TestHelper.AssertXmlRoundtrip(value, "<value>17:53:23.1234567</value>");
        }

        [Test]
        [TestCase("<value>25:53:23</value>", typeof(UnparsableValueException), Description = "Invalid hour")]
        public void XmlSerialization_Invalid(string xml, Type expectedExceptionType)
        {
            TestHelper.AssertXmlInvalid<LocalTime>(xml, expectedExceptionType);
        }
    }
}