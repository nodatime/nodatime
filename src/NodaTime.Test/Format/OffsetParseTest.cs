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
    public class OffsetParseTest
    {
        private static readonly NodaFormatInfo FormatInfo = new NodaFormatInfo(CultureInfo.GetCultureInfo("en-US"));
        private static readonly FailureInfo Success = new FailureInfo();
        private static readonly string Nul = null;

        private object[] tryParseExactFailure = {
            new TestCaseData(Nul, "g", new FailureInfo("value")).SetName("value is null"),
            new TestCaseData("123", Nul, new FailureInfo("format")).SetName("format is null"),
            new TestCaseData("", "g", new FailureInfo(ParseFailureKind.Format, "TryParse_Value_Empty")).SetName("value is empty"),
            new TestCaseData("123", "", new FailureInfo(ParseFailureKind.Format, "TryParse_Format_Empty")).SetName("format is empty"),
            new TestCaseData("123", "!", new FailureInfo(ParseFailureKind.Format, "Format_InvalidString")).SetName("standard format is invalid"),
        };

        private static OffsetParseInfo MakeParseInfo(bool throwImmediate)
        {
            return new OffsetParseInfo(FormatInfo, throwImmediate, DateTimeParseStyles.None);
        }

        private object[] tryParseExact = {
            new TestCaseData("2", "%h", Offset.Create(2, 0, 0, 0)).SetName("pattern '%h', one digit"),
            new TestCaseData("12", "%h", Offset.Create(12, 0, 0, 0)).SetName("pattern '%h', two digits"),
            new TestCaseData("2", "%m", Offset.Create(0, 2, 0, 0)).SetName("pattern '%m', one digit"),
            new TestCaseData("12", "%m", Offset.Create(0, 12, 0, 0)).SetName("pattern '%m', two digits"),
            new TestCaseData("2", "%s", Offset.Create(0, 0, 2, 0)).SetName("pattern '%s', one digit"),
            new TestCaseData("12", "%s", Offset.Create(0, 0, 12, 0)).SetName("pattern '%s', two digits"),
        };

        [Test]
        public void TestTryParseExact_success()
        {
            TestTryParseExact_failure(Nul, "g", new FailureInfo("value"));

            TestTryParseExact("2", "%f", Offset.Create(0, 0, 0, 2));
            TestTryParseExact("12", "%f", Offset.Create(0, 0, 0, 12));
            TestTryParseExact("123", "%f", Offset.Create(0, 0, 0, 123));
        }

        [Test]
        [TestCaseSource("tryParseExact")]
        public void TestTryParseExact(string value, string format, Offset expected)
        {
            var parseInfo = MakeParseInfo(true);
            Assert.IsTrue(OffsetParse.TryParseExact(value, format, parseInfo));
            Assert.AreEqual(expected, parseInfo.Value);
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
            private string FailureMessage { get; set; }
            private object FailureMessageFormatArgument { get; set; }
            private string FailureArgumentName { get; set; }

            internal void Validate(ParseInfo parseInfo)
            {
                Assert.AreEqual(Failure, parseInfo.Failure, "Failure kind mismatch");
                if (Failure != ParseFailureKind.None)
                {
                    Assert.AreEqual(FailureMessage, parseInfo.FailureMessage, "Failure message id mismatch");
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

        [Test]
        [TestCaseSource("tryParseExactFailure")]
        public void TestTryParseExact_failure(string value, string format, FailureInfo failureInfo)
        {
            var parseInfo = MakeParseInfo(false);
            Assert.IsFalse(OffsetParse.TryParseExact(value, format, parseInfo));
            var exception = parseInfo.GetFailureException();
            failureInfo.Validate(parseInfo);
        }
    }
}
