#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
using System.IO;
using NodaTime.Format;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    public partial class PeriodFormatterBuilderTest
    {
        [Test]
        public void Literal_CalculatesLengthAsLiteralLength()
        {
            const string literalText = "Hi";
            var literal = new PeriodFormatterBuilder.Literal(literalText);

            var actualLength = literal.CalculatePrintedLength(null, null);

            Assert.That(actualLength, Is.EqualTo(literalText.Length));
        }

        [Test]
        public void Literal_CountsFieldsToPrintAsZeroAlways()
        {
            const string literalText = "Hi";
            var literal = new PeriodFormatterBuilder.Literal(literalText);

            var actualFieldsCount = literal.CountFieldsToPrint(null, 0, null);

            Assert.That(actualFieldsCount, Is.EqualTo(0));
        }

        [Test]
        public void Literal_PrintsTextToTextWriter()
        {
            const string literalText = "T";
            var literal = new PeriodFormatterBuilder.Literal(literalText);
            var writer = new StringWriter();

            literal.PrintTo(writer, null, null);

            Assert.That(writer.ToString(), Is.EqualTo(literalText));
        }

        private object[] LiteralParseGoodTestData = {
            new TestCaseData("abc", 0, "abc").SetName("abc -> abc : commom"),
            new TestCaseData("aBc", 0, "abc").SetName("aBc -> abc : case insensitive"),
            new TestCaseData("abc", 0, "ABC").SetName("abc -> ABC : case insensitive"),
            new TestCaseData("bar", 0, "ba").SetName("bar -> ba: zero position in large string"),
            new TestCaseData("hello", 2, "ll").SetName("hello[2] -> ll: non-zero average position"),
            new TestCaseData("foo", 1, "oo").SetName("foo[2] -> oo: non-zero last position"),
        };

        [Test]
        [TestCaseSource("LiteralParseGoodTestData")]
        public void Literal_ParsesString(string text, int position, string value)
        {
            var literal = new PeriodFormatterBuilder.Literal(value);

            var newPosition = literal.Parse(text, position, null, null);

            Assert.That(newPosition, Is.EqualTo(position + value.Length));
        }

        private object[] LiteralParseBadTestData = {
            new TestCaseData("abc", 0, "def").SetName("abc -> def : non equal string"),
            new TestCaseData("abc", 0, "abd").SetName("abc -> abd : non equal string"),
            new TestCaseData("abc", 0, "zbc").SetName("abc -> zbc : non equal string"),
            new TestCaseData("oops", 4, "s").SetName("oops[4] -> s : position outside of length"),
        };

        [Test]
        [TestCaseSource("LiteralParseBadTestData")]
        public void Literal_Fails_ForWrongArguments(string text, int position, string value)
        {
            var literal = new PeriodFormatterBuilder.Literal(value);

            var newPosition = literal.Parse(text, position, null, null);

            Assert.That(newPosition, Is.EqualTo(~position));
        }

        [Test]
        public void AppendLiteral_ThrowsArgumentNull_ForNullTextArg()
        {
            Assert.That(() => (new PeriodFormatterBuilder().AppendLiteral(null)), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void AppendLiteral_ThrowsArgumentNull_ForEmptyTextArg()
        {
            Assert.That(() => (new PeriodFormatterBuilder().AppendLiteral(String.Empty)), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void AppendLiteral_ThrowsInvalidOperation_ForBuilderWithPrefix()
        {
            Assert.Throws<InvalidOperationException>(() => builder.AppendPrefix("prefix").AppendLiteral("literal"));
        }
    }
}