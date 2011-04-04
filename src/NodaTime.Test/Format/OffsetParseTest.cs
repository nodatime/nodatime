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
    [Category("Format")]
    public class OffsetParseTest
    {
        private static readonly NodaFormatInfo FormatInfo = new NodaFormatInfo(CultureInfo.GetCultureInfo("en-US"));
        private static readonly string Nul;
        private static readonly FailureInfo Success = new FailureInfo();

        private object[] tryParseExact = {
            new TestCaseData("2", "%H", Offset.Create(2, 0, 0, 0)).SetName("pattern '%H', one digit"),
            new TestCaseData("12", "%H", Offset.Create(12, 0, 0, 0)).SetName("pattern '%H', two digits"),
            new TestCaseData("12", "HH", Offset.Create(12, 0, 0, 0)).SetName("pattern 'HH', two digits"),
            new TestCaseData("2", "%m", Offset.Create(0, 2, 0, 0)).SetName("pattern '%m', one digit"),
            new TestCaseData("12", "%m", Offset.Create(0, 12, 0, 0)).SetName("pattern '%m', two digits"),
            new TestCaseData("12", "mm", Offset.Create(0, 12, 0, 0)).SetName("pattern 'mm', two digits"),
            new TestCaseData("2", "%s", Offset.Create(0, 0, 2, 0)).SetName("pattern '%s', one digit"),
            new TestCaseData("12", "%s", Offset.Create(0, 0, 12, 0)).SetName("pattern '%s', two digits"),
            new TestCaseData("12", "ss", Offset.Create(0, 0, 12, 0)).SetName("pattern 'ss', two digits"),
            new TestCaseData("1", "%f", Offset.Create(0, 0, 0, 100)).SetName("pattern '%f', one digit"),
            new TestCaseData("12", "ff", Offset.Create(0, 0, 0, 120)).SetName("pattern 'ff', two digits"),
            new TestCaseData("123", "fff", Offset.Create(0, 0, 0, 123)).SetName("pattern 'fff', three digits"),
            new TestCaseData("1", "%F", Offset.Create(0, 0, 0, 100)).SetName("pattern '%F', one digit"),
            new TestCaseData("1", "FF", Offset.Create(0, 0, 0, 100)).SetName("pattern 'FF', one digit"),
            new TestCaseData("12", "FF", Offset.Create(0, 0, 0, 120)).SetName("pattern 'FF', two digits"),
            new TestCaseData("1", "FFF", Offset.Create(0, 0, 0, 100)).SetName("pattern 'FFF', one digit"),
            new TestCaseData("12", "FFF", Offset.Create(0, 0, 0, 120)).SetName("pattern 'FFF', two digits"),
            new TestCaseData("123", "FFF", Offset.Create(0, 0, 0, 123)).SetName("pattern 'FFF', three digits"),
            new TestCaseData(":", "%:", Offset.Create(0, 0, 0, 0)).SetName("pattern '%:'"),
            new TestCaseData(".", "%.", Offset.Create(0, 0, 0, 0)).SetName("pattern '%.'"),
            new TestCaseData(".6", ".f", Offset.Create(0, 0, 0, 600)).SetName("pattern '.f'"),
            new TestCaseData(".678", ".fff", Offset.Create(0, 0, 0, 678)).SetName("pattern '.fff'"),
            new TestCaseData(".6", ".F", Offset.Create(0, 0, 0, 600)).SetName("pattern '.F'"),
            new TestCaseData(".6", ".FFF", Offset.Create(0, 0, 0, 600)).SetName("pattern '.FFF', elided zeros"),
            new TestCaseData(".678", ".FFF", Offset.Create(0, 0, 0, 678)).SetName("pattern '.FFF'"),
            new TestCaseData("1", "H.FFF", Offset.Create(1, 0, 0, 0)).SetName("pattern '.FFF', missing fraction"),
            new TestCaseData("H", "\\H", Offset.Create(0, 0, 0, 0)).SetName("pattern '\\H'"),
            new TestCaseData("HHss", "'HHss'", Offset.Create(0, 0, 0, 0)).SetName("pattern \"'HHss'\""),
        };

        private object[] tryParseExactWhiteSpace = {
// *************************************
            new TestCaseData("12:34", "HH:mm", DateTimeParseStyles.AllowLeadingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow leading, pattern \"HH:mm\", value has no leading spaces"),
            new TestCaseData("  12:34", "HH:mm", DateTimeParseStyles.AllowLeadingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow leading, pattern \"HH:mm\", value has leading spaces"),
            new TestCaseData("12:34", "  HH:mm", DateTimeParseStyles.AllowLeadingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow leading, pattern \"  HH:mm\", value has no leading spaces"),
            new TestCaseData("12:34", "'  'HH:mm", DateTimeParseStyles.AllowLeadingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow leading, pattern \"'  'HH:mm\", value has no leading spaces"),
            new TestCaseData("12:34", "   '  'HH:mm", DateTimeParseStyles.AllowLeadingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow leading, pattern \"   '  'HH:mm\", value has no leading spaces"),
            new TestCaseData("  12:34", "   '  'HH:mm", DateTimeParseStyles.AllowLeadingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow leading, pattern \"   '  'HH:mm\", value has leading spaces"),
// *************************************
            new TestCaseData("12:34", "HH:mm", DateTimeParseStyles.AllowTrailingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow trailing, pattern \"HH:mm\", value has no trailing spaces"),
            new TestCaseData("12:34  ", "HH:mm", DateTimeParseStyles.AllowTrailingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow trailing, pattern \"HH:mm\", value has trailing spaces"),
            new TestCaseData("12:34", "HH:mm  ", DateTimeParseStyles.AllowTrailingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow trailing, pattern \"HH:mm  \", value has no trailing spaces"),
            new TestCaseData("12:34", "HH:mm'  '  ", DateTimeParseStyles.AllowTrailingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow trailing, pattern \"HH:mm'  '  \", value has no trailing spaces"),
            new TestCaseData("12:34  ", "HH:mm'  '  ", DateTimeParseStyles.AllowTrailingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow trailing, pattern \"HH:mm'  '  \", value has trailing spaces"),
            new TestCaseData("12:34", "HH:mm", DateTimeParseStyles.AllowTrailingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow trailing, pattern \"HH:mm\", value has no trailing spaces"),
// *************************************
            new TestCaseData("12:34", "HH:mm", DateTimeParseStyles.AllowLeadingWhite | DateTimeParseStyles.AllowTrailingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow trailing, pattern \"HH:mm\", value has no trailing spaces"),
            new TestCaseData("  12:34  ", "HH:mm", DateTimeParseStyles.AllowLeadingWhite | DateTimeParseStyles.AllowTrailingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow trailing, pattern \"HH:mm\", value has trailing spaces"),
            new TestCaseData("12:34", "  HH:mm  ", DateTimeParseStyles.AllowLeadingWhite | DateTimeParseStyles.AllowTrailingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow trailing, pattern \"HH:mm  \", value has no trailing spaces"),
            new TestCaseData("12:34", "  '  'HH:mm'  '  ", DateTimeParseStyles.AllowLeadingWhite | DateTimeParseStyles.AllowTrailingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow trailing, pattern \"HH:mm'  '  \", value has no trailing spaces"),
            new TestCaseData("  12:34  ", "  '  'HH:mm'  '  ", DateTimeParseStyles.AllowLeadingWhite | DateTimeParseStyles.AllowTrailingWhite, Offset.Create(12, 34, 0, 0)).SetName("allow trailing, pattern \"HH:mm'  '  \", value has trailing spaces"),
// *************************************
        };

        private object[] tryParseExactFailure = {
            new TestCaseData(Nul, "g", new FailureInfo("value")).SetName("value is null"),
            new TestCaseData("123", Nul, new FailureInfo("format")).SetName("format is null"),
            new TestCaseData("", "g", new FailureInfo(ParseFailureKind.Format, "The value string is empty.")).SetName("value is empty"),
            new TestCaseData("123", "", new FailureInfo(ParseFailureKind.Format, "The format string is empty.")).SetName("format is empty"),
            new TestCaseData("123", "!", new FailureInfo(ParseFailureKind.Format, "The standard format \"!\" is not valid for the NodaTime.Instant type.")).
                SetName("standard format is invalid"),
            new TestCaseData("2:", "%H", new FailureInfo(ParseFailureKind.Format, "The format matches a prefix of the value string but not the entire string. Part not matching: \":\".")).SetName("extra characters in value not matched"),
            new TestCaseData("123", "H%", new FailureInfo(ParseFailureKind.Format, "A percent sign (%) appears at the end of the format string.")).
                SetName("% at end of format."),
            new TestCaseData("123", "%%H", new FailureInfo(ParseFailureKind.Format, "A percent sign (%) is followed by another percent sign in the format string.")).
                SetName("% doubled"),
            new TestCaseData("axc", "'abc'", new FailureInfo(ParseFailureKind.Format, "The value string does not match a quoted string in the pattern.")).
                SetName("quoted string mismatch"),
            new TestCaseData("axc", "%\\", new FailureInfo(ParseFailureKind.Format, "The format string has an escape character (backslash '\\') at the end of the string.")).
                SetName("\\ at end of string"),
            new TestCaseData("a", "\\'", new FailureInfo(ParseFailureKind.Format, "The value string does not match an escaped character in the format string: \"\\'\"")).
                SetName("escaped character mismatch"),
            new TestCaseData("a", "%.", new FailureInfo(ParseFailureKind.Format, "The format string contains a decimal separator that does not match the value and the decimal separator is not followed by an \"F\" pattern character.")).
                SetName("decimal separator missing"),
            new TestCaseData("a", ".H", new FailureInfo(ParseFailureKind.Format, "The format string contains a decimal separator that does not match the value and the decimal separator is not followed by an \"F\" pattern character.")).
                SetName("decimal separator missing, no F"),
            new TestCaseData("a", "%:", new FailureInfo(ParseFailureKind.Format, "The value string does not match a time separator in the format string.")).
                SetName("time separator mismatch"),
            new TestCaseData("a", "%H", new FailureInfo(ParseFailureKind.Format, "The value string does not match the required number from the format string \"H\".")).
                SetName("format '%H', no digits"),
            new TestCaseData("1", "HH", new FailureInfo(ParseFailureKind.Format, "The value string does not match the required number from the format string \"HH\".")).
                SetName("format 'HH', not enough digits"),
            new TestCaseData("a", "%m", new FailureInfo(ParseFailureKind.Format, "The value string does not match the required number from the format string \"m\".")).
                SetName("format '%m', no digits"),
            new TestCaseData("1", "mm", new FailureInfo(ParseFailureKind.Format, "The value string does not match the required number from the format string \"mm\".")).
                SetName("format 'mm', not enough digits"),
            new TestCaseData("a", "%s", new FailureInfo(ParseFailureKind.Format, "The value string does not match the required number from the format string \"s\".")).
                SetName("format '%s', no digits"),
            new TestCaseData("1", "ss", new FailureInfo(ParseFailureKind.Format, "The value string does not match the required number from the format string \"ss\".")).
                SetName("format 'ss', not enough digits"),
            new TestCaseData("12", "%f", new FailureInfo(ParseFailureKind.Format, "The format matches a prefix of the value string but not the entire string. Part not matching: \"2\".")).
                SetName("format '%f', too many digits"),
            new TestCaseData("123", "ff", new FailureInfo(ParseFailureKind.Format, "The format matches a prefix of the value string but not the entire string. Part not matching: \"3\".")).
                SetName("format 'ff', too many digits"),
            new TestCaseData("1234", "fff", new FailureInfo(ParseFailureKind.Format, "The format matches a prefix of the value string but not the entire string. Part not matching: \"4\".")).
                SetName("format 'fff', too many digits"),
            new TestCaseData("12", "%F", new FailureInfo(ParseFailureKind.Format, "The format matches a prefix of the value string but not the entire string. Part not matching: \"2\".")).
                SetName("format '%F', too many digits"),
            new TestCaseData("123", "FF", new FailureInfo(ParseFailureKind.Format, "The format matches a prefix of the value string but not the entire string. Part not matching: \"3\".")).
                SetName("format 'FF', too many digits"),
            new TestCaseData("1234", "FFF", new FailureInfo(ParseFailureKind.Format, "The format matches a prefix of the value string but not the entire string. Part not matching: \"4\".")).
                SetName("format 'FFF', too many digits"),
            new TestCaseData("1a", "H ", new FailureInfo(ParseFailureKind.Format, "The value string does not match a space in the format string.")).
                SetName("format 'H ', no spaces"),
            new TestCaseData("z", "%y", new FailureInfo(ParseFailureKind.Format, "The value string does not match a simple character in the format string \"y\".")).
                SetName("format '%y', no matching character"),
        };

        [Test]
        public void TestAbc()
        {
            TestTryParseExact_whiteSpace("   12:34", "HH:mm", DateTimeParseStyles.AllowLeadingWhite, Offset.Create(12, 34, 0, 0));
        }

        [Test]
        [TestCaseSource("tryParseExactWhiteSpace")]
        public void TestTryParseExact_whiteSpace(string value, string format, DateTimeParseStyles styles, Offset expected)
        {
            var parseInfo = MakeParseInfo(true, styles);
            Assert.IsTrue(OffsetParse.TryParseExact(value, format, parseInfo));
            Assert.AreEqual(expected, parseInfo.Value);
        }

        [Test]
        [TestCaseSource("tryParseExact")]
        public void TestTryParseExact(string value, string format, Offset expected)
        {
            var parseInfo = MakeParseInfo(true);
            Assert.IsTrue(OffsetParse.TryParseExact(value, format, parseInfo));
            Assert.AreEqual(expected, parseInfo.Value);
        }

        [Test]
        [TestCaseSource("tryParseExactFailure")]
        public void TestTryParseExact_failure(string value, string format, FailureInfo failureInfo)
        {
            var parseInfo = MakeParseInfo(false);
            Assert.IsFalse(OffsetParse.TryParseExact(value, format, parseInfo));
            failureInfo.Validate(parseInfo);
        }

        private static OffsetParseInfo MakeParseInfo(bool throwImmediate, DateTimeParseStyles styles = DateTimeParseStyles.None)
        {
            return new OffsetParseInfo(FormatInfo, throwImmediate, styles);
        }

        public sealed class FailureInfo
        {
            public FailureInfo()
            {
                Failure = ParseFailureKind.None;
            }

            internal FailureInfo(ParseFailureKind failure, string message)
            {
                Failure = failure;
                FailureMessage = message;
            }

            internal FailureInfo(string argumentName)
            {
                Failure = ParseFailureKind.ArgumentNull;
                FailureMessage = "Argument cannot be null.";
                FailureArgumentName = argumentName;
            }

            private ParseFailureKind Failure { get; set; }
            private string FailureArgumentName { get; set; }
            private string FailureMessage { get; set; }

            internal void Validate(ParseInfo parseInfo)
            {
                Assert.AreEqual(Failure, parseInfo.Failure, "Failure kind mismatch");
                if (Failure != ParseFailureKind.None)
                {
                    Assert.AreEqual(FailureMessage, parseInfo.FailureMessage, "Failure message mismatch");
                    if (FailureArgumentName == null)
                    {
                        Assert.IsNull(parseInfo.FailureArgumentName, "failure argument name should be null");
                    }
                    else
                    {
                        Assert.AreEqual(FailureArgumentName, parseInfo.FailureArgumentName, "Failure argument name mismatch");
                    }
                }
            }
        }
    }
}