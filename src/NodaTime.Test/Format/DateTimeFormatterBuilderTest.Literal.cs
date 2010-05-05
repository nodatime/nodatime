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

using System;
using System.IO;
using NodaTime.Format;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    public partial class DateTimeFormatterBuilderTest
    {
        #region CharacterLiteral

        [Test]
        public void CharacterLiteral_EstimatesPrintedLengthAs1Always()
        {
            const char value = 'a';
            var literal = new DateTimeFormatterBuilder.CharacterLiteral(value);

            var actualLength = literal.EstimatedPrintedLength;

            Assert.That(actualLength, Is.EqualTo(1));
        }

        [Test]
        public void CharacterLiteral_PrintsCharacter_ForAnyLocalInstant()
        {
            const char value = 'c';
            var literal = new DateTimeFormatterBuilder.CharacterLiteral(value);
            var writer = new StringWriter();

            literal.PrintTo(writer, LocalInstant.Now, null, Offset.Zero, null, null);

            Assert.That(writer.ToString(), Is.EqualTo(value.ToString()));
        }

        [Test]
        public void CharacterLiteral_PrintsCharacter_ForAnyPartial()
        {
            const char value = 'z';
            var literal = new DateTimeFormatterBuilder.CharacterLiteral(value);
            var writer = new StringWriter();

            literal.PrintTo(writer, null, null);

            Assert.That(writer.ToString(), Is.EqualTo(value.ToString()));
        }

        [Test]
        public void CharacterLiteral_EstimatesParsedLengthAs1Always()
        {
            const char value = 'x';
            var literal = new DateTimeFormatterBuilder.CharacterLiteral(value);

            var actualLength = literal.EstimatedParsedLength;

            Assert.That(actualLength, Is.EqualTo(1));
        }

        object[] CharacterLiteralParseGoodTestData =
        {
            new TestCaseData("a", 0, 'a').SetName("a -> a : commom"),
            new TestCaseData("B", 0, 'b').SetName("B -> b : case insensitive"),
            new TestCaseData("c", 0, 'C').SetName("c -> C : case insensitive"),
            new TestCaseData("bar", 0, 'b').SetName("bar -> b: zero position in large string"),
            new TestCaseData("hello", 2, 'l').SetName("hello[2] -> l: non-zero average position"),
            new TestCaseData("foo", 2, 'o').SetName("foo[2] -> o: non-zero last position"),
        };
        [Test]
        [TestCaseSource("CharacterLiteralParseGoodTestData")]
        public void CharacterLiteral_ParsesCharacter(string text, int position, char value)
        {
            var literal = new DateTimeFormatterBuilder.CharacterLiteral(value);

            var newPosition = literal.ParseInto(null, text, position);

            Assert.That(newPosition, Is.EqualTo(position + 1));
        }

        object[] CharacterLiteralParseBadTestData =
        {
            new TestCaseData("a", 0, 'b').SetName("a -> b : non equal characters"),
            new TestCaseData("oops", 4, 's').SetName("oops[4] -> s : position outside of length"),
        };
        [Test]
        [TestCaseSource("CharacterLiteralParseBadTestData")]
        public void CharacterLiteral_Fails_ForWrongArguments(string text, int position, char value)
        {
            var literal = new DateTimeFormatterBuilder.CharacterLiteral(value);

            var newPosition = literal.ParseInto(null, text, position);

            Assert.That(newPosition, Is.EqualTo(~position));
        }

        #endregion

        #region StringLiteral

        [Test]
        public void StringLiteral_EstimatesPrintedLengthAsStringLength()
        {
            const string value = "hi";
            var literal = new DateTimeFormatterBuilder.StringLiteral(value);

            var actualLength = literal.EstimatedPrintedLength;

            Assert.That(actualLength, Is.EqualTo(value.Length));
        }

        [Test]
        public void StringLiteral_PrintsString_ForAnyLocalInstant()
        {
            const string value = "hi";
            var literal = new DateTimeFormatterBuilder.StringLiteral(value);
            var writer = new StringWriter();

            literal.PrintTo(writer, LocalInstant.Now, null, Offset.Zero, null, null);

            Assert.That(writer.ToString(), Is.EqualTo(value));
        }

        [Test]
        public void StringLiteral_PrintsString_ForAnyPartial()
        {
            const string value = "hi";
            var literal = new DateTimeFormatterBuilder.StringLiteral(value);
            var writer = new StringWriter();

            literal.PrintTo(writer, null, null);

            Assert.That(writer.ToString(), Is.EqualTo(value));
        }

        [Test]
        public void StringLiteral_EstimatesParsedLengthAsStringLength()
        {
            const string value = "hi";
            var literal = new DateTimeFormatterBuilder.StringLiteral(value);

            var actualLength = literal.EstimatedParsedLength;

            Assert.That(actualLength, Is.EqualTo(value.Length));
        }


        object[] StringLiteralParseGoodTestData =
        {
            new TestCaseData("abc", 0, "abc").SetName("abc -> abc : commom"),
            new TestCaseData("aBc", 0, "abc").SetName("aBc -> abc : case insensitive"),
            new TestCaseData("abc", 0, "ABC").SetName("abc -> ABC : case insensitive"),
            new TestCaseData("bar", 0, "ba").SetName("bar -> ba: zero position in large string"),
            new TestCaseData("hello", 2, "ll").SetName("hello[2] -> ll: non-zero average position"),
            new TestCaseData("foo", 1, "oo").SetName("foo[2] -> oo: non-zero last position"),
        };
        [Test]
        [TestCaseSource("StringLiteralParseGoodTestData")]
        public void StringLiteral_ParsesString(string text, int position, string value)
        {
            var literal = new DateTimeFormatterBuilder.StringLiteral(value);

            var newPosition = literal.ParseInto(null, text, position);

            Assert.That(newPosition, Is.EqualTo(position + value.Length));
        }

        object[] StringLiteralParseBadTestData =
        {
            new TestCaseData("abc", 0, "def").SetName("abc -> def : non equal string"),
            new TestCaseData("abc", 0, "abd").SetName("abc -> abd : non equal string"),
            new TestCaseData("abc", 0, "zbc").SetName("abc -> zbc : non equal string"),
            new TestCaseData("oops", 4, "s").SetName("oops[4] -> s : position outside of length"),
        };
        [Test]
        [TestCaseSource("StringLiteralParseBadTestData")]
        public void StringLiteral_Fails_ForWrongArguments(string text, int position, string value)
        {
            var literal = new DateTimeFormatterBuilder.StringLiteral(value);

            var newPosition = literal.ParseInto(null, text, position);

            Assert.That(newPosition, Is.EqualTo(~position));
        }


        #endregion

        [Test]
        public void AppendLiteral_ThrowsArgumentNull_ForNullTextArg()
        {
            Assert.That(() => (new DateTimeFormatterBuilder().AppendLiteral(null)), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void AppendLiteral_ThrowsArgumentNull_ForEmptyTextArg()
        {
            Assert.That(() => (new DateTimeFormatterBuilder().AppendLiteral(String.Empty)), Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
