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
        private const DateTimeParseStyles LeadingSpace = DateTimeParseStyles.AllowLeadingWhite;
        private const DateTimeParseStyles TrailingSpace = DateTimeParseStyles.AllowTrailingWhite;
        private const DateTimeParseStyles InnerSpace = DateTimeParseStyles.AllowInnerWhite;
        private const DateTimeParseStyles SurroundingSpace = DateTimeParseStyles.AllowLeadingWhite | DateTimeParseStyles.AllowTrailingWhite;
        private const DateTimeParseStyles AllSpace = DateTimeParseStyles.AllowWhiteSpaces;

        private static readonly NodaFormatInfo FormatInfo = new NodaFormatInfo(CultureInfo.GetCultureInfo("en-US"));

        private object[] tryParseExact = {
            new ParseData<Offset>("2", "%H", Offset.Create(2, 0, 0, 0), "one digit"),
            new ParseData<Offset>("2", "%H", Offset.Create(2, 0, 0, 0), "one digit"),
            new ParseData<Offset>("12", "%H", Offset.Create(12, 0, 0, 0), "two digits"),
            new ParseData<Offset>("12", "HH", Offset.Create(12, 0, 0, 0), "two digits"),
            new ParseData<Offset>("2", "%m", Offset.Create(0, 2, 0, 0), "one digit"),
            new ParseData<Offset>("12", "%m", Offset.Create(0, 12, 0, 0), "two digits"),
            new ParseData<Offset>("12", "mm", Offset.Create(0, 12, 0, 0), "two digits"),
            new ParseData<Offset>("2", "%s", Offset.Create(0, 0, 2, 0), "one digit"),
            new ParseData<Offset>("12", "%s", Offset.Create(0, 0, 12, 0), "two digits"),
            new ParseData<Offset>("12", "ss", Offset.Create(0, 0, 12, 0), "two digits"),
            new ParseData<Offset>("1", "%f", Offset.Create(0, 0, 0, 100), "one digit"),
            new ParseData<Offset>("12", "ff", Offset.Create(0, 0, 0, 120), "two digits"),
            new ParseData<Offset>("123", "fff", Offset.Create(0, 0, 0, 123), "two digits"),
            new ParseData<Offset>("1", "%F", Offset.Create(0, 0, 0, 100), "one digit"),
            new ParseData<Offset>("1", "FF", Offset.Create(0, 0, 0, 100), "one digit"),
            new ParseData<Offset>("12", "FF", Offset.Create(0, 0, 0, 120), "two digits"),
            new ParseData<Offset>("1", "FFF", Offset.Create(0, 0, 0, 100), "two digits"),
            new ParseData<Offset>("12", "FFF", Offset.Create(0, 0, 0, 120), "two digits"),
            new ParseData<Offset>("123", "FFF", Offset.Create(0, 0, 0, 123), "three digits"),
            new ParseData<Offset>(":", "%:", Offset.Create(0, 0, 0, 0), "date separator"),
            new ParseData<Offset>(".", "%.", Offset.Create(0, 0, 0, 0), "decimal separator"),
            new ParseData<Offset>(".6", ".f", Offset.Create(0, 0, 0, 600), "one digit"),
            new ParseData<Offset>(".678", ".fff", Offset.Create(0, 0, 0, 678), "three digits"),
            new ParseData<Offset>(".6", ".F", Offset.Create(0, 0, 0, 600), "one digit"),
            new ParseData<Offset>(".6", ".FFF", Offset.Create(0, 0, 0, 600), "three digits, elided zeros"),
            new ParseData<Offset>(".678", ".FFF", Offset.Create(0, 0, 0, 678), "three digits"),
            new ParseData<Offset>("1", "H.FFF", Offset.Create(1, 0, 0, 0), "missing fraction"),
            new ParseData<Offset>("H", "\\H", Offset.Create(0, 0, 0, 0), "escaped character"),
            new ParseData<Offset>("HHss", "'HHss'", Offset.Create(0, 0, 0, 0), "quoted string"),
            // *************************************
            new ParseData<Offset>("12:34", "HH:mm", Offset.Create(12, 34, 0, 0), LeadingSpace, "allow leading spaces"),
            new ParseData<Offset>("  12:34", "HH:mm", Offset.Create(12, 34, 0, 0), LeadingSpace, "allow leading spaces"),
            new ParseData<Offset>("12:34", "  HH:mm", Offset.Create(12, 34, 0, 0), LeadingSpace, "allow leading spaces"),
            new ParseData<Offset>("12:34", "'  'HH:mm", Offset.Create(12, 34, 0, 0), LeadingSpace, "allow leading spaces"),
            new ParseData<Offset>("12:34", "  '  'HH:mm", Offset.Create(12, 34, 0, 0), LeadingSpace, "allow leading spaces"),
            new ParseData<Offset>("  12:34", "  '  'HH:mm", Offset.Create(12, 34, 0, 0), LeadingSpace, "allow leading spaces"),
            // *************************************
            new ParseData<Offset>("12:34", "HH:mm", Offset.Create(12, 34, 0, 0), TrailingSpace, "allow trailing spaces"),
            new ParseData<Offset>("12:34  ", "HH:mm", Offset.Create(12, 34, 0, 0), TrailingSpace, "allow trailing spaces"),
            new ParseData<Offset>("12:34", "HH:mm  ", Offset.Create(12, 34, 0, 0), TrailingSpace, "allow trailing spaces"),
            new ParseData<Offset>("12:34", "HH:mm'  '", Offset.Create(12, 34, 0, 0), TrailingSpace, "allow trailing spaces"),
            new ParseData<Offset>("12:34", "HH:mm'  '  ", Offset.Create(12, 34, 0, 0), TrailingSpace, "allow trailing spaces"),
            new ParseData<Offset>("12:34  ", "HH:mm'  '  ", Offset.Create(12, 34, 0, 0), TrailingSpace, "allow trailing spaces"),
            // *************************************
            new ParseData<Offset>("12:34", "HH:mm", Offset.Create(12, 34, 0, 0), SurroundingSpace, "allow leading and trailing spaces"),
            new ParseData<Offset>("  12:34  ", "HH:mm", Offset.Create(12, 34, 0, 0), SurroundingSpace, "allow leading and trailing spaces"),
            new ParseData<Offset>("12:34", "  HH:mm  ", Offset.Create(12, 34, 0, 0), SurroundingSpace, "allow leading and trailing spaces"),
            new ParseData<Offset>("12:34", "'  'HH:mm'  '", Offset.Create(12, 34, 0, 0), SurroundingSpace, "allow leading and trailing spaces"),
            new ParseData<Offset>("12:34", "  '  'HH:mm'  '  ", Offset.Create(12, 34, 0, 0), SurroundingSpace, "allow leading and trailing spaces"),
            new ParseData<Offset>("  12:34  ", "  '  'HH:mm'  '  ", Offset.Create(12, 34, 0, 0), SurroundingSpace, "allow leading and trailing spaces"),
            // *************************************
            new ParseData<Offset>("12:34", "HH:mm", Offset.Create(12, 34, 0, 0), InnerSpace, "allow inner spaces"),
            new ParseData<Offset>(" 12:34", "HH:mm", Offset.Create(12, 34, 0, 0), InnerSpace, "allow inner spaces"),
            new ParseData<Offset>("12  :34", "HH:mm", Offset.Create(12, 34, 0, 0), InnerSpace, "allow inner spaces"),
            new ParseData<Offset>("12:34", " HH:mm", Offset.Create(12, 34, 0, 0), InnerSpace, "allow inner spaces"),
            new ParseData<Offset>("12:34", "HH  :mm", Offset.Create(12, 34, 0, 0), InnerSpace, "allow inner spaces"),
            new ParseData<Offset>(" 12:34", "HH :mm", Offset.Create(12, 34, 0, 0), InnerSpace, "allow inner spaces"),
            // *************************************
            new ParseData<Offset>("12:34", "HH:mm", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("  12:34", "HH:mm", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("12:34", "  HH:mm", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("12:34", "'  'HH:mm", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("12:34", "  '  'HH:mm", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("  12:34", "  '  'HH:mm", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            // *************************************
            new ParseData<Offset>("12:34", "HH:mm", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("12:34  ", "HH:mm", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("12:34", "HH:mm  ", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("12:34", "HH:mm'  '", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("12:34", "HH:mm'  '  ", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("12:34  ", "HH:mm'  '  ", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            // *************************************
            new ParseData<Offset>("12:34", "HH:mm", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("  12:34  ", "HH:mm", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("12:34", "  HH:mm  ", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("12:34", "'  'HH:mm'  '", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("12:34", "  '  'HH:mm'  '  ", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("  12:34  ", "  '  'HH:mm'  '  ", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            // *************************************
            new ParseData<Offset>("12:34", "HH:mm", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>(" 12:34", "HH:mm", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("12  :34", "HH:mm", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("12:34", " HH:mm", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>("12:34", "HH  :mm", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
            new ParseData<Offset>(" 12:34", "HH :mm", Offset.Create(12, 34, 0, 0), AllSpace, "allow all spaces"),
        };

        private object[] tryParseExactFailure = {
            new ParseFailureNull(null, "g", "value"),
            new ParseFailureNull("123", null, "format"),
            new ParseFailureData("", "g", ParseFailureKind.ParseValueStringEmpty, "value is empty"),
            new ParseFailureData("123", "", ParseFailureKind.ParseValueStringEmpty, "format is empty"),
            new ParseFailureData("123", "!", ParseFailureKind.ParseUnknownStandardFormat, "standard format is invalid", '!', typeof(Offset).FullName),
            new ParseFailureData("2:", "%H", ParseFailureKind.ParseExtraValueCharacters, "% at end of format", ":"),
            new ParseFailureData("123", "H%", ParseFailureKind.ParsePercentAtEndOfString, "% at end of format"),
            new ParseFailureData("123", "%%H", ParseFailureKind.ParsePercentDoubled, "% doubled"),
            new ParseFailureData("axc", "'abc'", ParseFailureKind.ParseQuotedStringMismatch, "quoted string mismatch"),
            new ParseFailureData("axc", "%\\", ParseFailureKind.ParseEscapeAtEndOfString, "\\ at end of string"),
            new ParseFailureData("a", "\\'", ParseFailureKind.ParseEscapedCharacterMismatch, "escaped character mismatch", '\''),
            new ParseFailureData("a", "%.", ParseFailureKind.ParseMissingDecimalSeparator, "decimal separator missing"),
            new ParseFailureData("a", ".H", ParseFailureKind.ParseMissingDecimalSeparator, "decimal separator missing, no F"),
            new ParseFailureData("a", "%:", ParseFailureKind.ParseTimeSeparatorMismatch, "time separator mismatch"),
            new ParseFailureData("a", "%H", ParseFailureKind.ParseMismatchedNumber, "no digits", "H"),
            new ParseFailureData("1", "HH", ParseFailureKind.ParseMismatchedNumber, "not enough digits", "HH"),
            new ParseFailureData("a", "%m", ParseFailureKind.ParseMismatchedNumber, "no digits", "m"),
            new ParseFailureData("1", "mm", ParseFailureKind.ParseMismatchedNumber, "not enough digits", "mm"),
            new ParseFailureData("a", "%s", ParseFailureKind.ParseMismatchedNumber, "no digits", "s"),
            new ParseFailureData("1", "ss", ParseFailureKind.ParseMismatchedNumber, "not enough digits", "ss"),
            new ParseFailureData("12", "%f", ParseFailureKind.ParseExtraValueCharacters, "too many digits", "2"),
            new ParseFailureData("123", "ff", ParseFailureKind.ParseExtraValueCharacters, "too many digits", "3"),
            new ParseFailureData("1234", "fff", ParseFailureKind.ParseExtraValueCharacters, "too many digits", "4"),
            new ParseFailureData("12", "%F", ParseFailureKind.ParseExtraValueCharacters, "too many digits", "2"),
            new ParseFailureData("123", "FF", ParseFailureKind.ParseExtraValueCharacters, "too many digits", "3"),
            new ParseFailureData("1234", "FFF", ParseFailureKind.ParseExtraValueCharacters, "too many digits", "4"),
            new ParseFailureData("1a", "H ", ParseFailureKind.ParseMismatchedSpace, "no spaces"),
            new ParseFailureData("z", "%y", ParseFailureKind.ParseMismatchedCharacter, "no matching character", 'y'),
            new ParseFailureData("12:34 ", "HH:mm", ParseFailureKind.ParseExtraValueCharacters, "value has trailing space", " "),
        };

        private static OffsetParseInfo MakeParseInfo(bool throwImmediate, DateTimeParseStyles styles = DateTimeParseStyles.None)
        {
            return new OffsetParseInfo(FormatInfo, throwImmediate, styles);
        }

        [Test]
        public void TestAbc()
        {
        }

        [Test]
        [TestCaseSource("tryParseExact")]
        public void TestTryParseExact(string value, string format, Offset expected, DateTimeParseStyles styles)
        {
            var parseInfo = MakeParseInfo(true, styles);
            Assert.IsTrue(OffsetParse.TryParseExact(value, format, parseInfo));
            Assert.AreEqual(expected, parseInfo.Value);
        }

        [Test]
        [TestCaseSource("tryParseExactFailure")]
        public void TestTryParseExact_failure(string value, string format, DateTimeParseStyles styles, ParseFailureInfo failureInfo)
        {
            var parseInfo = MakeParseInfo(false, styles);
            Assert.IsFalse(OffsetParse.TryParseExact(value, format, parseInfo));
            failureInfo.Validate(parseInfo);
        }
    }
}