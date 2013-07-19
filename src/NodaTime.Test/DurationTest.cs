// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using NodaTime.Text;

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
            Duration value = new PeriodBuilder { Days = 5, Hours = 3, Seconds = 20 }.Build().ToDuration();
            TestHelper.AssertXmlRoundtrip(value, "<value>P5DT3H20S</value>");
        }

        [Test]
        [TestCase("<value>P10MT1S</value>", typeof(InvalidOperationException), Description = "Can't contain month/year")]
        [TestCase("<value>XYZ</value>", typeof(UnparsableValueException), Description = "Completely unparsable")]
        public void XmlSerialization_Invalid(string xml, Type expectedExceptionType)
        {
            TestHelper.AssertXmlInvalid<Duration>(xml, expectedExceptionType);
        }
    }
}
