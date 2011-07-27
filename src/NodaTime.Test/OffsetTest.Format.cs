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
    public partial class OffsetTest
    {
        private const string Nbsp = "\u00a0";

        private object[] toStringNoFormatData = {
            new TestCaseData(EnUs, Offset.MaxValue, "+23:59:59.999").SetName("Offset.MaxValue, eu-US"),
            new TestCaseData(EnUs, Offset.MinValue, "-23:59:59.999").SetName("Offset.MinValue, en-US"),
            new TestCaseData(EnUs, HmsfOffset, "+05:12:34.567").SetName("Hours, minutes, seconds, fractions, en-US"),
            new TestCaseData(EnUs, HmsOffset, "+05:12:34").SetName("Hours, minutes, seconds, en-US"),
            new TestCaseData(EnUs, HmOffset, "+05:12").SetName("Hours, minutes, en-US"),
            new TestCaseData(EnUs, HOffset, "+05").SetName("Hours, en-US"),
            new TestCaseData(FrFr, Offset.MaxValue, "+23:59:59,999").SetName("Offset.MaxValue, fr-FR"),
            new TestCaseData(FrFr, Offset.MinValue, "-23:59:59,999").SetName("Offset.MinValue, fr-FR"),
            new TestCaseData(FrFr, HmsfOffset, "+05:12:34,567").SetName("Hours, minutes, seconds, fractions, fr-FR"),
            new TestCaseData(FrFr, HmsOffset, "+05:12:34").SetName("Hours, minutes, seconds, fr-FR"),
            new TestCaseData(FrFr, HmOffset, "+05:12").SetName("Hours, minutes, fr-FR"),
            new TestCaseData(FrFr, HOffset, "+05").SetName("Hours, fr-FR"),
            new TestCaseData(ItIt, Offset.MaxValue, "+23.59.59,999").SetName("Offset.MaxValue, it-IT"),
            new TestCaseData(ItIt, Offset.MinValue, "-23.59.59,999").SetName("Offset.MinValue, it-IT"),
            new TestCaseData(ItIt, HmsfOffset, "+05.12.34,567").SetName("Hours, minutes, seconds, fractions, it-IT"),
            new TestCaseData(ItIt, HmsOffset, "+05.12.34").SetName("Hours, minutes, seconds, it-IT"),
            new TestCaseData(ItIt, HmOffset, "+05.12").SetName("Hours, minutes, it-IT"),
            new TestCaseData(ItIt, HOffset, "+05").SetName("Hours, it-IT"),
        };

        private object[] toStringFormatData = {
            new TestCaseData(EnUs, Offset.MaxValue, null, "+23:59:59.999").SetName("Offset.MaxValue, null, eu-US"),
            new TestCaseData(EnUs, Offset.MaxValue, "", "+23:59:59.999").SetName("Offset.MaxValue, '', eu-US"),
            new TestCaseData(EnUs, Offset.MaxValue, "G", "+23:59:59.999").SetName("Offset.MaxValue, 'G', eu-US"),
            new TestCaseData(EnUs, Offset.MinValue, "G", "-23:59:59.999").SetName("Offset.MinValue, 'G', en-US"),
            new TestCaseData(EnUs, HmsfOffset, "G", "+05:12:34.567").SetName("Hours, minutes, seconds, fractions, 'G', en-US"),
            new TestCaseData(EnUs, HmsOffset, "G", "+05:12:34").SetName("Hours, minutes, seconds, 'G', en-US"),
            new TestCaseData(EnUs, HmOffset, "G", "+05:12").SetName("Hours, minutes, 'G', en-US"),
            new TestCaseData(EnUs, HOffset, "G", "+05").SetName("Hours, 'G', en-US"),
            new TestCaseData(EnUs, HmsfOffset, "N", "18,754,567").SetName("Hmsf, 'N', en-US"),
            new TestCaseData(EnUs, HmsfOffset, "F", "+05:12:34.567").SetName("Hmsf, 'F', en-US"),
            new TestCaseData(EnUs, HmsfOffset, "L", "+05:12:34").SetName("Hmsf, 'L', en-US"),
            new TestCaseData(EnUs, HmsfOffset, "M", "+05:12").SetName("Hmsf, 'M', en-US"),
            new TestCaseData(EnUs, HmsfOffset, "S", "+05").SetName("Hmsf, 'S', en-US"),
            new TestCaseData(EnUs, Offset.Zero, "F", "+00:00:00.000").SetName("Zero, 'F', en-US"),
            new TestCaseData(EnUs, Offset.Zero, "L", "+00:00:00").SetName("Zero, 'L', en-US"),
            new TestCaseData(EnUs, Offset.Zero, "M", "+00:00").SetName("Zero, 'M', en-US"),
            new TestCaseData(EnUs, Offset.Zero, "S", "+00").SetName("Zero, 'S', en-US"),
            new TestCaseData(EnUs, ThreeHours, "%+", "+").SetName("ThreeHours, '%+', en-US"),
            new TestCaseData(EnUs, ThreeHours, "%-", "").SetName("ThreeHours, '%-', en-US"),
            new TestCaseData(EnUs, NegativeThreeHours, "%+", "-").SetName("NegativeThreeHours, '%+', en-US"),
            new TestCaseData(EnUs, NegativeThreeHours, "%-", "-").SetName("NegativeThreeHours, '%-', en-US"),
            new TestCaseData(EnUs, Offset.MaxValue, "\\m", "m").SetName("Offset.MaxValue, '\\\\m', en-US"),
            new TestCaseData(EnUs, Offset.MaxValue, "'m'", "m").SetName("Offset.MaxValue, '\\\\m', en-US"),
            new TestCaseData(EnUs, Offset.MaxValue, "'mmmmmmmmmm'", "mmmmmmmmmm").SetName("Offset.MaxValue, ''mmmmmmmmmm'', en-US"),
            new TestCaseData(EnUs, Offset.MaxValue, "zqw", "zqw").SetName("Offset.MaxValue, 'zqw', en-US"),
            new TestCaseData(EnUs, Offset.MaxValue, "%z", "z").SetName("Offset.MaxValue, '%z', en-US"),
            new TestCaseData(EnUs, Full, "%h", "5").SetName("Full, '%h', en-US"),
            new TestCaseData(EnUs, Offset.MaxValue, "%h", "23").SetName("Offset.MaxValue, '%h', en-US"),
            new TestCaseData(EnUs, Full, "%H", "5").SetName("Full, '%H', en-US"),
            new TestCaseData(EnUs, Full, "HH", "05").SetName("Full, 'HH', en-US"),
            new TestCaseData(EnUs, Full, "%m", "6").SetName("Full, '%m', en-US"),
            new TestCaseData(EnUs, Offset.MaxValue, "%m", "59").SetName("Offset.MaxValue, '%m', en-US"),
            new TestCaseData(EnUs, Full, "mm", "06").SetName("Full, 'mm', en-US"),
            new TestCaseData(EnUs, Full, "%s", "7").SetName("Full, '%s', en-US"),
            new TestCaseData(EnUs, Offset.MaxValue, "%s", "59").SetName("Offset.MaxValue, '%s', en-US"),
            new TestCaseData(EnUs, Full, "ss", "07").SetName("Full, 'ss', en-US"),
            new TestCaseData(EnUs, Full, "%f", "0").SetName("Full, '%f', en-US"),
            new TestCaseData(EnUs, Full, "ff", "00").SetName("Full, 'ff', en-US"),
            new TestCaseData(EnUs, Full, "fff", "008").SetName("Full, 'fff', en-US"),
            new TestCaseData(EnUs, Full, "%F", "").SetName("Full, '%F', en-US"),
            new TestCaseData(EnUs, Full, "FF", "").SetName("Full, 'FF', en-US"),
            new TestCaseData(EnUs, Full, "FFF", "008").SetName("Full, 'FFF', en-US"),
            new TestCaseData(EnUs, OneFractional, "%f", "4").SetName("OneFractional, '%f', en-US"),
            new TestCaseData(EnUs, OneFractional, "ff", "40").SetName("OneFractional, 'ff', en-US"),
            new TestCaseData(EnUs, OneFractional, "fff", "400").SetName("OneFractional, 'fff', en-US"),
            new TestCaseData(EnUs, OneFractional, "%F", "4").SetName("OneFractional, '%F', en-US"),
            new TestCaseData(EnUs, OneFractional, "FF", "4").SetName("OneFractional, 'FF', en-US"),
            new TestCaseData(EnUs, OneFractional, "FFF", "4").SetName("OneFractional, 'FFF', en-US"),
            new TestCaseData(EnUs, TwoFractional, "%f", "4").SetName("TwoFractional, '%f', en-US"),
            new TestCaseData(EnUs, TwoFractional, "ff", "45").SetName("TwoFractional, 'ff', en-US"),
            new TestCaseData(EnUs, TwoFractional, "fff", "450").SetName("TwoFractional, 'fff', en-US"),
            new TestCaseData(EnUs, TwoFractional, "%F", "4").SetName("TwoFractional, '%F', en-US"),
            new TestCaseData(EnUs, TwoFractional, "FF", "45").SetName("TwoFractional, 'FF', en-US"),
            new TestCaseData(EnUs, TwoFractional, "FFF", "45").SetName("TwoFractional, 'FFF', en-US"),
            new TestCaseData(EnUs, ThreeFractional, "%f", "4").SetName("ThreeFractional, '%f', en-US"),
            new TestCaseData(EnUs, ThreeFractional, "ff", "45").SetName("ThreeFractional, 'ff', en-US"),
            new TestCaseData(EnUs, ThreeFractional, "fff", "456").SetName("ThreeFractional, 'fff', en-US"),
            new TestCaseData(EnUs, ThreeFractional, "%F", "4").SetName("ThreeFractional, '%F', en-US"),
            new TestCaseData(EnUs, ThreeFractional, "FF", "45").SetName("ThreeFractional, 'FF', en-US"),
            new TestCaseData(EnUs, ThreeFractional, "FFF", "456").SetName("ThreeFractional, 'FFF', en-US"),
            new TestCaseData(FrFr, Offset.MaxValue, "G", "+23:59:59,999").SetName("Offset.MaxValue, 'G', fr-FR"),
            new TestCaseData(FrFr, Offset.MinValue, "G", "-23:59:59,999").SetName("Offset.MinValue, 'G', fr-FR"),
            new TestCaseData(FrFr, HmsfOffset, "G", "+05:12:34,567").SetName("Hours, minutes, seconds, fractions, 'G', fr-FR"),
            new TestCaseData(FrFr, HmsOffset, "G", "+05:12:34").SetName("Hours, minutes, seconds, 'G', fr-FR"),
            new TestCaseData(FrFr, HmOffset, "G", "+05:12").SetName("Hours, minutes, 'G', fr-FR"),
            new TestCaseData(FrFr, HOffset, "G", "+05").SetName("Hours, 'G', fr-FR"),
            new TestCaseData(FrFr, HmsfOffset, "N", "18" + Nbsp + "754" + Nbsp + "567").SetName("Hmsf, 'N', fr-FR"),
            new TestCaseData(ItIt, Offset.MaxValue, "G", "+23.59.59,999").SetName("Offset.MaxValue, 'G', it-IT"),
            new TestCaseData(ItIt, Offset.MinValue, "G", "-23.59.59,999").SetName("Offset.MinValue, 'G', it-IT"),
            new TestCaseData(ItIt, HmsfOffset, "G", "+05.12.34,567").SetName("Hours, minutes, seconds, fractions, 'G', it-IT"),
            new TestCaseData(ItIt, HmsOffset, "G", "+05.12.34").SetName("Hours, minutes, seconds, 'G', it-IT"),
            new TestCaseData(ItIt, HmOffset, "G", "+05.12").SetName("Hours, minutes, 'G', it-IT"),
            new TestCaseData(ItIt, HOffset, "G", "+05").SetName("Hours, 'G', it-IT"),
            new TestCaseData(ItIt, HmsfOffset, "N", "18.754.567").SetName("Hmsf, 'N', it-IT"),
        };

        private object[] toStringFormatBadData = {
            new TestCaseData("z").SetName("invalid standard format"),
            new TestCaseData("\\").SetName("Escape without character"),
            new TestCaseData("%").SetName("Expansion character without character"),
            new TestCaseData("%%").SetName("Double expansion character"),
            new TestCaseData("'").SetName("Missing end quote"),
            new TestCaseData("'qwe").SetName("Missing end quote 2"),
            new TestCaseData("'qwe\\'").SetName("Escaped end quote"),
            new TestCaseData("'qwe\\").SetName("Escaped in quote missing character"),
            new TestCaseData("ffff").SetName("Too many 'f'"),
            new TestCaseData("hhh").SetName("Too many 'h'"),
            new TestCaseData("mmm").SetName("Too many 'm'"),
            new TestCaseData("sss").SetName("Too many 's'"),
            new TestCaseData("mmmmmmmmmmmmmmmmmmm").SetName("Too many 'm'"),
        };

        [Test]
        [TestCaseSource("toStringNoFormatData")]
        public void TestToString_Culture(CultureInfo cultureInfo, Offset value, string expected)
        {
            using (CultureSaver.SetUiCulture(EnUs))
            {
                string actual = value.ToString(cultureInfo);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        [TestCaseSource("toStringFormatData")]
        public void TestToString_Format(CultureInfo cultureInfo, Offset value, string format, string expected)
        {
            using (CultureSaver.SetUiCulture(cultureInfo))
            {
                string actual = value.ToString(format);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        [TestCaseSource("toStringFormatData")]
        public void TestToString_FormatCulture(CultureInfo cultureInfo, Offset value, string format, string expected)
        {
            using (CultureSaver.SetUiCulture(EnUs))
            {
                string actual = value.ToString(format, cultureInfo);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        [TestCaseSource("toStringFormatBadData")]
        public void TestToString_FormatFailure(string format)
        {
            Assert.Throws<FormatException>(() => Offset.Zero.ToString(format));
        }

        [Test]
        [TestCaseSource("toStringNoFormatData")]
        public void TestToString_NoArg(CultureInfo cultureInfo, Offset value, string expected)
        {
            using (CultureSaver.SetUiCulture(cultureInfo))
            {
                string actual = value.ToString();
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void TestToString_NullCulture()
        {
            using (CultureSaver.SetUiCulture(EnUs))
            {
                IFormatProvider formatProvider = null;
                string actual = Offset.Zero.ToString(formatProvider);
                Assert.AreEqual("+00", actual);
                actual = Offset.Zero.ToString("G", formatProvider);
                Assert.AreEqual("+00", actual);
            }
        }
    }
}