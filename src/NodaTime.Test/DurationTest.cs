// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class DurationTest
    {
        private readonly Duration threeMillion = new Duration(3000000L);
        private readonly Duration negativeFiftyMillion = new Duration(-50000000L);
        private readonly Duration negativeEpsilon = new Duration(-1L);

        /// <summary>
        /// Using the default constructor is equivalent to Duration.Zero.
        /// </summary>
        [Test]
        public void DefaultConstructor()
        {
            var actual = new Duration();
            Assert.AreEqual(Duration.Zero, actual);
        }

        [Test]
        public void BinarySerialization()
        {
            TestHelper.AssertBinaryRoundtrip(Duration.FromTicks(12345L));
        }

        [Test]
        public void XmlSerialization()
        {
            Duration value = new PeriodBuilder { Days = 5, Hours = 3, Minutes = 20, Seconds = 35, Ticks = 1234500 }.Build().ToDuration();
            TestHelper.AssertXmlRoundtrip(value, "<value>5:03:20:35.12345</value>");
            TestHelper.AssertXmlRoundtrip(-value, "<value>-5:03:20:35.12345</value>");
        }

        [Test]
        [TestCase("<value>XYZ</value>", typeof(UnparsableValueException), Description = "Completely unparsable")]
        public void XmlSerialization_Invalid(string xml, Type expectedExceptionType)
        {
            TestHelper.AssertXmlInvalid<Duration>(xml, expectedExceptionType);
        }
    }
}
