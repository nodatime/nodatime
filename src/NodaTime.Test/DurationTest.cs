#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
using System;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class DurationTest
    {
        readonly Duration threeMillion = new Duration(3000000L);
        readonly Duration negativeFiftyMillion = new Duration(-50000000L);


        #region ToString

        object[] ToStringTestData =
        {
            new TestCaseData(0, "PT0S").SetName("0 => PT0S"),
            new TestCaseData(12 * NodaConstants.TicksPerMillisecond + 345L, "PT0.012S").SetName("12ms => PT0.012S"),
            new TestCaseData(345 * NodaConstants.TicksPerMillisecond + 678L, "PT0.345S").SetName("345ms => PT0.345S"),
            new TestCaseData(1234 * NodaConstants.TicksPerMillisecond, "PT1.234S").SetName("1234ms => PT1.234S"),
        };

        [Test]
        [TestCaseSource("ToStringTestData")]
        public void ToString_ReturnsISO8601Value(long ticks, string text)
        {
            var sut = new Duration(ticks);
            Assert.That(sut.ToString(), Is.EqualTo(text));
        }


        #endregion

        object[] ParseBadTestData =
        {
            new TestCaseData("PT0").SetName("PT0"),
            new TestCaseData("12.345S").SetName("12.345S"),
            new TestCaseData("P2Y6M9DXYZ").SetName("P2Y6M9DXYZ"),
            new TestCaseData("PTS").SetName("PTS"),
            new TestCaseData("XT0S").SetName("XT0S"),
            new TestCaseData("PX0S").SetName("PX0S"),
            new TestCaseData("PT0X").SetName("PT0X"),
            new TestCaseData("PTXS").SetName("PTXS"),
            new TestCaseData("PT0.0.0S").SetName("PT0.0.0S"),
            new TestCaseData("PT0-00S").SetName("PT0-00S"),
            new TestCaseData("PT0-00S").SetName("PT0-00S"),

        };


        object[] ParseGoodTestData =
        {
            new TestCaseData("PT0S", 0).SetName("PT0S => 0"),
            new TestCaseData("PT12.345S", 12345 * NodaConstants.TicksPerMillisecond).SetName("PT12.345S => 12.345 * 10 000"),
            new TestCaseData("pt12.345s", 12345 * NodaConstants.TicksPerMillisecond).SetName("pt12.345s => 12.345 * 10 000"),
            new TestCaseData("pt12s", 12000 * NodaConstants.TicksPerMillisecond).SetName("pt12s => 12.000 * 10 000"),
            new TestCaseData("pt12.s", 12000 * NodaConstants.TicksPerMillisecond).SetName("pt12.s => 12.000 * 10 000"),
            new TestCaseData("pt-12.32s", -12320 * NodaConstants.TicksPerMillisecond).SetName("pt-12.32s => -12.320 * 10 000"),
            new TestCaseData("pt12.3456s", 12345 * NodaConstants.TicksPerMillisecond).SetName("pt12.3456s => 12.345 * 10 000"),

        };


        #region Parse

        [Test]
        public void Parse_ThrowsArgumentNull_IfValueIsNull()
        {
            Assert.That(() => Duration.Parse(null), Throws.InstanceOf<ArgumentNullException>());
        }


        [Test]
        [TestCaseSource("ParseBadTestData")]
        public void Parse_TrowsFormat_IfStringInBadFormat(string durationText)
        {
            Assert.That(() => Duration.Parse(durationText), Throws.InstanceOf<FormatException>());
        }

        [Test]
        [TestCaseSource("ParseGoodTestData")]
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
        [TestCaseSource("ParseBadTestData")]
        public void TryParse_ReturnsFalse_IfStringInBadFormat(string durationText)
        {
            Duration result;
            var flag = Duration.TryParse(durationText, out result);
            Assert.That(result.Ticks, Is.EqualTo(0));
            Assert.That(flag, Is.False);
        }

        [Test]
        [TestCaseSource("ParseGoodTestData")]
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
