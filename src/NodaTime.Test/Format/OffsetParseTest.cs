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
#region usings
using System;
using System.Globalization;
using NodaTime.Format;
using NodaTime.Globalization;
using NUnit.Framework;
#endregion

namespace NodaTime.Test.Format
{
    [TestFixture]
    [Category("Formating")]
    [Category("Parse")]
    public class OffsetParseTest
    {
        private static readonly CultureInfo EnUs = new CultureInfo("en-US");

        private const DateTimeParseStyles LeadingSpace = DateTimeParseStyles.AllowLeadingWhite;
        private const DateTimeParseStyles TrailingSpace = DateTimeParseStyles.AllowTrailingWhite;
        private const DateTimeParseStyles InnerSpace = DateTimeParseStyles.AllowInnerWhite;
        private const DateTimeParseStyles SurroundingSpace = DateTimeParseStyles.AllowLeadingWhite | DateTimeParseStyles.AllowTrailingWhite;
        private const DateTimeParseStyles AllSpace = DateTimeParseStyles.AllowWhiteSpaces;

        public sealed class OffsetData : AbstractFormattingData<Offset>
        {
            protected override string ValueLabel(Offset value)
            {
                return string.Format("Make({0}{1}, {2}, {3}, {4})",
                    value.IsNegative ? "-" : "",
                    value.Hours,
                    value.Minutes,
                    value.Seconds,
                    value.FractionalSeconds);
            }
        }

        private static Offset Make(int hours, int minutes, int seconds, int fractional)
        {
            return Offset.Create(hours, minutes, seconds, fractional);
        }

        private object[] parseExactCommon = {
            new OffsetData { C = EnUs, V = Make(2, 0, 0, 0), S = "2", F = "%H" },
            new OffsetData { C = EnUs, V = Make(2, 0, 0, 0), S = "2", F = "%H" },
            new OffsetData { C = EnUs, V = Make(12, 0, 0, 0), S = "12", F = "%H" },
            new OffsetData { C = EnUs, V = Make(12, 0, 0, 0), S = "12", F = "HH" },
            new OffsetData { C = EnUs, V = Make(0, 2, 0, 0), S = "2", F = "%m" },
            new OffsetData { C = EnUs, V = Make(0, 12, 0, 0), S = "12", F = "%m" },
            new OffsetData { C = EnUs, V = Make(0, 12, 0, 0), S = "12", F = "mm" },
            new OffsetData { C = EnUs, V = Make(0, 0, 2, 0), S = "2", F = "%s" },
            new OffsetData { C = EnUs, V = Make(0, 0, 12, 0), S = "12", F = "%s" },
            new OffsetData { C = EnUs, V = Make(0, 0, 12, 0), S = "12", F = "ss" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 100), S = "1", F = "%f" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 120), S = "12", F = "ff" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 123), S = "123", F = "fff" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 100), S = "1", F = "%F" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 100), S = "1", F = "FF" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 120), S = "12", F = "FF" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 100), S = "1", F = "FFF" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 120), S = "12", F = "FFF" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 123), S = "123", F = "FFF" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 0), S = ":", F = "%:",  Name = "date separator" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 0), S = ".", F = "%.",  Name = "decimal separator" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 600), S = ".6", F = ".f" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 678), S = ".678", F = ".fff" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 600), S = ".6", F = ".F" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 600), S = ".6", F = ".FFF",  Name = "elided zeros" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 678), S = ".678", F = ".FFF" },
            new OffsetData { C = EnUs, V = Make(1, 0, 0, 0), S = "1", F = "H.FFF",  Name = "missing fraction" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 0), S = "H", F = "\\H" },
            new OffsetData { C = EnUs, V = Make(0, 0, 0, 0), S = "HHss", F = "'HHss'" },
            // *************************************
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "HH:mm", Styles = LeadingSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "  12:34", F = "HH:mm", Styles = LeadingSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "  HH:mm", Styles = LeadingSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "'  'HH:mm", Styles = LeadingSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "  '  'HH:mm", Styles = LeadingSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "  12:34", F = "  '  'HH:mm", Styles = LeadingSpace },
            // *************************************
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "HH:mm", Styles = TrailingSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34  ", F = "HH:mm", Styles = TrailingSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "HH:mm  ", Styles = TrailingSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "HH:mm'  '", Styles = TrailingSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "HH:mm'  '  ", Styles = TrailingSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34  ", F = "HH:mm'  '  ", Styles = TrailingSpace },
            // *************************************
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "HH:mm", Styles = SurroundingSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "  12:34  ", F = "HH:mm", Styles = SurroundingSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "  HH:mm  ", Styles = SurroundingSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "'  'HH:mm'  '", Styles = SurroundingSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "  '  'HH:mm'  '  ", Styles = SurroundingSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "  12:34  ", F = "  '  'HH:mm'  '  ", Styles = SurroundingSpace },
            // *************************************
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "HH:mm", Styles = InnerSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = " 12:34", F = "HH:mm", Styles = InnerSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12  :34", F = "HH:mm", Styles = InnerSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = " HH:mm", Styles = InnerSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "HH  :mm", Styles = InnerSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = " 12:34", F = "HH :mm", Styles = InnerSpace },
            // *************************************
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "  12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "  HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "'  'HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "  '  'HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "  12:34", F = "  '  'HH:mm", Styles = AllSpace },
            // *************************************
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34  ", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "HH:mm  ", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "HH:mm'  '", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "HH:mm'  '  ", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34  ", F = "HH:mm'  '  ", Styles = AllSpace },
            // *************************************
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "  12:34  ", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "  HH:mm  ", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "'  'HH:mm'  '", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "  '  'HH:mm'  '  ", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "  12:34  ", F = "  '  'HH:mm'  '  ", Styles = AllSpace },
            // *************************************
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = " 12:34", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12  :34", F = "HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = " HH:mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = "12:34", F = "HH  :mm", Styles = AllSpace },
            new OffsetData { C = EnUs, V = Make(12, 34, 0, 0), S = " 12:34", F = "HH :mm", Styles = AllSpace },
            // *************************************
            new OffsetData { C = EnUs, S = "", F = "g", Kind = ParseFailureKind.ParseValueStringEmpty },
            new OffsetData { C = EnUs, S = null, F = "g", Kind = ParseFailureKind.ArgumentNull, ArgumentName = "value" },
        };

        private object[] parseExactSingle = {
            new OffsetData { C = EnUs, S = "123", F = "!", Kind = ParseFailureKind.ParseUnknownStandardFormat, Parameters = {'!', typeof(Offset).FullName}},
            new OffsetData { C = EnUs, S = "2:", F = "%H", Kind = ParseFailureKind.ParseExtraValueCharacters, Parameters = {":"}},
            new OffsetData { C = EnUs, S = "123", F = "H%", Kind = ParseFailureKind.ParsePercentAtEndOfString },
            new OffsetData { C = EnUs, S = "123", F = "%%H", Kind = ParseFailureKind.ParsePercentDoubled },
            new OffsetData { C = EnUs, S = "axc", F = "'abc'", Kind = ParseFailureKind.ParseQuotedStringMismatch },
            new OffsetData { C = EnUs, S = "axc", F = "%\\", Kind = ParseFailureKind.ParseEscapeAtEndOfString },
            new OffsetData { C = EnUs, S = "a", F = "\\'", Kind = ParseFailureKind.ParseEscapedCharacterMismatch, Parameters = {'\''} },
            new OffsetData { C = EnUs, S = "a", F = "%.", Kind = ParseFailureKind.ParseMissingDecimalSeparator },
            new OffsetData { C = EnUs, S = "a", F = ".H", Kind = ParseFailureKind.ParseMissingDecimalSeparator },
            new OffsetData { C = EnUs, S = "a", F = "%:", Kind = ParseFailureKind.ParseTimeSeparatorMismatch },
            new OffsetData { C = EnUs, S = "a", F = "%H", Kind = ParseFailureKind.ParseMismatchedNumber, Parameters = {"H"} },
            new OffsetData { C = EnUs, S = "1", F = "HH", Kind = ParseFailureKind.ParseMismatchedNumber, Parameters = {"HH"} },
            new OffsetData { C = EnUs, S = "a", F = "%m", Kind = ParseFailureKind.ParseMismatchedNumber, Parameters = {"m"} },
            new OffsetData { C = EnUs, S = "1", F = "mm", Kind = ParseFailureKind.ParseMismatchedNumber, Parameters = {"mm"} },
            new OffsetData { C = EnUs, S = "a", F = "%s", Kind = ParseFailureKind.ParseMismatchedNumber, Parameters = {"s"} },
            new OffsetData { C = EnUs, S = "1", F = "ss", Kind = ParseFailureKind.ParseMismatchedNumber, Parameters = {"ss"} },

            new OffsetData { C = EnUs, S = "12", F = "%f", Kind = ParseFailureKind.ParseExtraValueCharacters, Parameters = {"2"} },
            new OffsetData { C = EnUs, S = "123", F = "ff", Kind = ParseFailureKind.ParseExtraValueCharacters, Parameters = {"3"} },
            new OffsetData { C = EnUs, S = "1234", F = "fff", Kind = ParseFailureKind.ParseExtraValueCharacters, Parameters = {"4"} },
            new OffsetData { C = EnUs, S = "12", F = "%F", Kind = ParseFailureKind.ParseExtraValueCharacters, Parameters = {"2"} },
            new OffsetData { C = EnUs, S = "123", F = "FF", Kind = ParseFailureKind.ParseExtraValueCharacters, Parameters = {"3"} },
            new OffsetData { C = EnUs, S = "1234", F = "FFF", Kind = ParseFailureKind.ParseExtraValueCharacters, Parameters = {"4"} },

            new OffsetData { C = EnUs, S = "1a", F = "H ", Kind = ParseFailureKind.ParseMismatchedSpace },
            new OffsetData { C = EnUs, S = "z", F = "%y", Kind = ParseFailureKind.ParseMismatchedCharacter, Parameters = {'y'} },
            new OffsetData { C = EnUs, S = "12:34 ", F = "HH:mm", Kind = ParseFailureKind.ParseExtraValueCharacters, Parameters = {" "} },
            new OffsetData { C = EnUs, S = "123", F = "", Kind = ParseFailureKind.ParseFormatStringEmpty },
            new OffsetData { C = EnUs, S = "123", F = null, Kind = ParseFailureKind.ArgumentNull, ArgumentName = "format" },
        };

        private object[] parseExactMultiple = {
            new OffsetData { V = Make(0, 1, 23, 0), C = EnUs, S = "1:23", F = "HH:mm\0m:ss"},
            new OffsetData { C = EnUs, S = "123", F = "", Kind = ParseFailureKind.ParseFormatElementInvalid },
            new OffsetData { C = EnUs, S = "123", F = null, Kind = ParseFailureKind.ArgumentNull, ArgumentName = "formats" },
        };

        [Test]
        [TestCaseSource("parseExactCommon")]
        [TestCaseSource("parseExactMultiple")]
        public void TestParseExact_multiple(OffsetData data)
        {
            using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
            {
                bool isSuccess = data.Kind == ParseFailureKind.None;
                var formatInfo = new NodaFormatInfo(data.C);
                string[] formats = null;
                if (data.F != null)
                {
                    formats = data.F.Split('\0');
                }
                if (isSuccess)
                {
                    Assert.AreEqual(data.V, OffsetParse.ParseExact(data.S, formats, formatInfo, data.Styles));
                }
                else
                {
                    TestDelegate test = () => OffsetParse.ParseExact(data.S, formats, formatInfo, data.Styles);
                    if (data.Kind == ParseFailureKind.ArgumentNull)
                    {
                        Assert.Throws<ArgumentNullException>(test);
                    }
                    else
                    {
                        Assert.Throws(Is.TypeOf<ParseException>().And.Property("Kind").EqualTo(data.Kind), test);
                    }
                }
            }
        }

        [Test]
        [TestCaseSource("parseExactCommon")]
        [TestCaseSource("parseExactSingle")]
        public void TestParseExact_single(OffsetData data)
        {
            using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
            {
                bool isSuccess = data.Kind == ParseFailureKind.None;
                var formatInfo = new NodaFormatInfo(data.C);
                if (isSuccess)
                {
                    Assert.AreEqual(data.V, OffsetParse.ParseExact(data.S, data.F, formatInfo, data.Styles));
                }
                else
                {
                    TestDelegate test = () => OffsetParse.ParseExact(data.S, data.F, formatInfo, data.Styles);
                    if (data.Kind == ParseFailureKind.ArgumentNull)
                    {
                        Assert.Throws<ArgumentNullException>(test);
                    }
                    else
                    {
                        Assert.Throws(Is.TypeOf<ParseException>().And.Property("Kind").EqualTo(data.Kind), test);
                    }
                }
            }
        }

        [Test]
        [TestCaseSource("parseExactCommon")]
        [TestCaseSource("parseExactSingle")]
        public void TestTryParseExact(OffsetData data)
        {
            using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
            {
                bool isSuccess = data.Kind == ParseFailureKind.None;
                var formatInfo = new NodaFormatInfo(data.C);
                Offset result;
                Assert.AreEqual(isSuccess, OffsetParse.TryParseExact(data.S, data.F, formatInfo, data.Styles, out result));
                if (isSuccess)
                {
                    Assert.AreEqual(data.V, result);
                }
            }
        }

        [Test]
        [TestCaseSource("parseExactCommon")]
        [TestCaseSource("parseExactMultiple")]
        public void TestTryParseExactMultiple(OffsetData data)
        {
            using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
            {
                bool isSuccess = data.Kind == ParseFailureKind.None;
                var formatInfo = new NodaFormatInfo(data.C);
                Offset result;
                string[] formats = null;
                if (data.F != null)
                {
                    formats = data.F.Split('\0');
                }
                Assert.AreEqual(isSuccess, OffsetParse.TryParseExactMultiple(data.S, formats, formatInfo, data.Styles, out result));
                if (isSuccess)
                {
                    Assert.AreEqual(data.V, result);
                }
            }
        }

        [Test]
        [TestCaseSource("parseExactCommon")]
        [TestCaseSource("parseExactMultiple")]
        public void TestTryParseExactMultiple_internal(OffsetData data)
        {
            using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
            {
                bool isSuccess = data.Kind == ParseFailureKind.None;
                var formatInfo = new NodaFormatInfo(data.C);
                var parseInfo = new OffsetParseInfo(formatInfo, false, data.Styles);
                string[] formats = null;
                if (data.F != null)
                {
                    formats = data.F.Split('\0');
                }
                Assert.AreEqual(isSuccess, OffsetParse.TryParseExactMultiple(data.S, formats, parseInfo));
                data.Validate(parseInfo);
                if (isSuccess)
                {
                    Assert.AreEqual(data.V, parseInfo.Value);
                }
            }
        }

        [Test]
        [TestCaseSource("parseExactCommon")]
        [TestCaseSource("parseExactSingle")]
        public void TestTryParseExact_internal(OffsetData data)
        {
            using (CultureSaver.SetCultures(data.ThreadCulture, data.ThreadUiCulture))
            {
                bool isSuccess = data.Kind == ParseFailureKind.None;
                var formatInfo = new NodaFormatInfo(data.C);
                var parseInfo = new OffsetParseInfo(formatInfo, isSuccess, data.Styles);
                Assert.AreEqual(isSuccess, OffsetParse.TryParseExact(data.S, data.F, parseInfo));
                data.Validate(parseInfo);
                if (isSuccess)
                {
                    Assert.AreEqual(data.V, parseInfo.Value);
                }
            }
        }
    }
}