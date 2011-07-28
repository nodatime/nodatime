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

using System;
using System.Globalization;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class DurationTest
    {
        private readonly Duration threeMillion = new Duration(3000000L);
        private readonly Duration negativeFiftyMillion = new Duration(-50000000L);

        [Test]
        public void TestToString_InvalidFormat()
        {
            Assert.Throws<FormatException>(() => Duration.Zero.ToString("A"));
        }

        [Test]
        public void TestToString_MinValue()
        {
            TestToStringBase(Duration.MinValue, "-PT2562047788015H12M55.807S", "-PT2562047788015H12M55.807S", "-PT2562047788015H12M");
        }

        [Test]
        public void TestToString_MaxValue()
        {
            TestToStringBase(Duration.MaxValue, "+PT2562047788015H12M55.807S", "+PT2562047788015H12M55.807S", "+PT2562047788015H12M");
        }

        [Test]
        public void TestToString_Zero()
        {
            TestToStringBase(Duration.Zero, "+PT0H", "+PT0H00M00.000S", "+PT0H00M");
        }

        private static void TestToStringBase(Duration value, string gvalue, string lvalue, string svalue)
        {
            var actual = value.ToString();
            Assert.AreEqual(gvalue, actual);
            actual = value.ToString("G");
            Assert.AreEqual(gvalue, actual);
            actual = value.ToString("L");
            Assert.AreEqual(lvalue, actual);
            actual = value.ToString("S");
            Assert.AreEqual(svalue, actual);
            actual = value.ToString("S", CultureInfo.InvariantCulture);
            Assert.AreEqual(svalue, actual);
            actual = value.ToString(CultureInfo.InvariantCulture);
            Assert.AreEqual(gvalue, actual);

            actual = string.Format("{0}", value);
            Assert.AreEqual(gvalue, actual);
            actual = string.Format("{0:G}", value);
            Assert.AreEqual(gvalue, actual);
            actual = string.Format("{0:L}", value);
            Assert.AreEqual(lvalue, actual);
            actual = string.Format("{0:S}", value);
            Assert.AreEqual(svalue, actual);
        }

        private object[] parseBadTestData = {
            new TestCaseData("PT0").SetName("PT0"), new TestCaseData("12.345S").SetName("12.345S"),
            new TestCaseData("P2Y6M9DXYZ").SetName("P2Y6M9DXYZ"), new TestCaseData("PTS").SetName("PTS"), new TestCaseData("XT0S").SetName("XT0S"),
            new TestCaseData("PX0S").SetName("PX0S"), new TestCaseData("PT0X").SetName("PT0X"), new TestCaseData("PTXS").SetName("PTXS"),
            new TestCaseData("PT0.0.0S").SetName("PT0.0.0S"), new TestCaseData("PT0-00S").SetName("PT0-00S"), new TestCaseData("PT0-00S").SetName("PT0-00S"),
        };

        private object[] parseGoodTestData = {
            new TestCaseData("PT0S", 0).SetName("PT0S => 0"),
            new TestCaseData("PT12.3450000S", 12345 * NodaConstants.TicksPerMillisecond).SetName("PT12.3450000S => 12.345 * 10 000"),
            new TestCaseData("PT12.3456789S", 12 * NodaConstants.TicksPerSecond + 3456789).SetName("PT12.3456789S => 12*10 000 000 + 3456789"),
            new TestCaseData("pt12.3450000s", 12345 * NodaConstants.TicksPerMillisecond).SetName("pt12.345s => 12.345 * 10 000"),
            new TestCaseData("pt12s", 12000 * NodaConstants.TicksPerMillisecond).SetName("pt12s => 12.000 * 10 000"),
            new TestCaseData("pt12.s", 12000 * NodaConstants.TicksPerMillisecond).SetName("pt12.s => 12.000 * 10 000"),
            new TestCaseData("pt-12.320000s", -12320 * NodaConstants.TicksPerMillisecond).SetName("pt-12.32s => -12.320 * 10 000"),
            new TestCaseData("pt12.34567891s", 12 * NodaConstants.TicksPerSecond + 3456789).SetName("PT12.34567891S => 12*10 000 000 + 3456789"),
        };

        #region Parse
        [Test]
        public void Parse_ThrowsArgumentNull_IfValueIsNull()
        {
            Assert.That(() => Duration.Parse(null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource("parseBadTestData")]
        public void Parse_TrowsFormat_IfStringInBadFormat(string durationText)
        {
            Assert.That(() => Duration.Parse(durationText), Throws.InstanceOf<FormatException>());
        }

        [Test]
        [TestCaseSource("parseGoodTestData")]
        public void Parse_ReturnsDurationFromStringValue(string durationText, long ticks)
        {
            var sut = Duration.Parse(durationText);
            Assert.That(sut.Ticks, Is.EqualTo(ticks));
        }
        #endregion

        #region TryParse
        [Test]
        public void TryParse_ThrowsArgumentNull_IfValueIsNull()
        {
            Duration result;
            Assert.That(() => Duration.TryParse(null, out result), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource("parseBadTestData")]
        public void TryParse_ReturnsFalse_IfStringInBadFormat(string durationText)
        {
            Duration result;
            var flag = Duration.TryParse(durationText, out result);
            Assert.That(result.Ticks, Is.EqualTo(0));
            Assert.That(flag, Is.False);
        }

        [Test]
        [TestCaseSource("parseGoodTestData")]
        public void TryParse_ReturnsTrueAndSetDurationFromStringValue(string durationText, long ticks)
        {
            Duration result;
            var flag = Duration.TryParse(durationText, out result);
            Assert.That(result.Ticks, Is.EqualTo(ticks));
            Assert.That(flag, Is.True);
        }
        #endregion
    }
}