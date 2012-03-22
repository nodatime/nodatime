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

using System.Globalization;
using NUnit.Framework;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    [TestFixture, Category("Formatting"), Category("Parse")]
    public class ValueCursorTest : TextCursorTestBase
    {
        internal override TextCursor MakeCursor(string value)
        {
            return new ValueCursor(value);
        }

        [Test]
        public void TestMatch_char()
        {
            var value = new ValueCursor("abc");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.True(value.Match('a'), "First character");
            Assert.True(value.Match('b'), "Second character");
            Assert.True(value.Match('c'), "Third character");
            Assert.False(value.MoveNext(), "GetNext() end");
        }

        [Test]
        public void TestMatch_string()
        {
            var value = new ValueCursor("abc");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.True(value.Match("abc"));
            Assert.False(value.MoveNext(), "GetNext() end");
        }

        [Test]
        public void TestMatch_stringNotMatched()
        {
            var value = new ValueCursor("xabcdef");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.False(value.Match("abc"));
            ValidateCurrentCharacter(value, 0, 'x');
        }

        [Test]
        public void Match_stringOverLongStringToMatch()
        {
            var value = new ValueCursor("x");
            Assert.True(value.MoveNext());
            Assert.False(value.Match("long string"));
            ValidateCurrentCharacter(value, 0, 'x');
        }

        [Test]
        public void TestMatchCaseInsensitive_string()
        {
            var value = new ValueCursor("abc");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.True(value.MatchCaseInsensitive("AbC", CultureInfo.InvariantCulture.CompareInfo));
            Assert.False(value.MoveNext(), "GetNext() end");
        }

        [Test]
        public void TestMatchCaseInsensitive_stringNotMatched()
        {
            var value = new ValueCursor("xabcdef");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.False(value.MatchCaseInsensitive("abc", CultureInfo.InvariantCulture.CompareInfo));
            ValidateCurrentCharacter(value, 0, 'x');
        }

        [Test]
        public void MatchCaseInsensitive_stringOverLongStringToMatch()
        {
            var value = new ValueCursor("x");
            Assert.True(value.MoveNext());
            Assert.False(value.MatchCaseInsensitive("long string", CultureInfo.InvariantCulture.CompareInfo));
            ValidateCurrentCharacter(value, 0, 'x');
        }
        [Test]
        public void TestMatch_stringPartial()
        {
            var value = new ValueCursor("abcdef");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.True(value.Match("abc"));
            ValidateCurrentCharacter(value, 3, 'd');
        }

        [Test]
        public void TestParseDigit_failureTooFewDigits()
        {
            var value = new ValueCursor("a12b");
            Assert.True(value.MoveNext());
            ValidateCurrentCharacter(value, 0, 'a');
            Assert.True(value.MoveNext());
            int actual;
            Assert.False(value.ParseDigits(3, 3, out actual));
            ValidateCurrentCharacter(value, 1, '1');
        }

        [Test]
        public void TestParseDigit_noNumber()
        {
            var value = new ValueCursor("abc");
            Assert.True(value.MoveNext());
            int actual;
            Assert.False(value.ParseDigits(1, 2, out actual));
            ValidateCurrentCharacter(value, 0, 'a');
        }

        [Test]
        public void TestParseDigit_successMaximum()
        {
            var value = new ValueCursor("12");
            Assert.True(value.MoveNext());
            int actual;
            Assert.True(value.ParseDigits(1, 2, out actual));
            Assert.AreEqual(12, actual);
        }

        [Test]
        public void TestParseDigit_successMaximumMoreDigits()
        {
            var value = new ValueCursor("1234");
            Assert.True(value.MoveNext());
            int actual;
            Assert.True(value.ParseDigits(1, 2, out actual));
            Assert.AreEqual(12, actual);
            ValidateCurrentCharacter(value, 2, '3');
        }

        [Test]
        public void TestParseDigit_successMinimum()
        {
            var value = new ValueCursor("1");
            value.MoveNext();
            int actual;
            Assert.True(value.ParseDigits(1, 2, out actual));
            Assert.AreEqual(1, actual);
            ValidateEndOfString(value);
        }

        [Test]
        public void TestParseDigit_successMinimumNonDigits()
        {
            var value = new ValueCursor("1abc");
            Assert.True(value.MoveNext());
            int actual;
            Assert.True(value.ParseDigits(1, 2, out actual));
            Assert.AreEqual(1, actual);
            ValidateCurrentCharacter(value, 1, 'a');
        }

        [Test]
        public void TestParseDigit_nonASCII_NeverMatches()
        {
            // Arabic-Indic digits 0 and 1. See
            // http://www.unicode.org/charts/PDF/U0600.pdf
            var value = new ValueCursor("\u0660\u0661");
            Assert.True(value.MoveNext());
            int actual;
            Assert.False(value.ParseDigits(1, 2, out actual));
        }

        [Test]
        public void TestParseFraction_nonASCII_NeverMatches()
        {
            // Arabic-Indic digits 0 and 1. See
            // http://www.unicode.org/charts/PDF/U0600.pdf
            var value = new ValueCursor("\u0660\u0661");
            Assert.True(value.MoveNext());
            int actual;
            Assert.False(value.ParseFraction(2, 2, out actual, true));
        }

        [Test]
        public void ParseInt64_Simple()
        {
            var value = new ValueCursor("56x");
            Assert.True(value.MoveNext());
            long result;
            Assert.IsNull(value.ParseInt64<string>(out result));
            Assert.AreEqual(56L, result);
            // Cursor ends up post-number
            Assert.AreEqual(2, value.Index);
        }

        [Test]
        public void ParseInt64_Negative()
        {
            var value = new ValueCursor("-56x");
            Assert.True(value.MoveNext());
            long result;
            Assert.IsNull(value.ParseInt64<string>(out result));
            Assert.AreEqual(-56L, result);
        }

        [Test]
        public void ParseInt64_NonNumber()
        {
            var value = new ValueCursor("xyz");
            Assert.True(value.MoveNext());
            long result;
            Assert.IsNotNull(value.ParseInt64<string>(out result));
            // Cursor has not moved
            Assert.AreEqual(0, value.Index);
        }

        [Test]
        public void ParseInt64_DoubleNegativeSign()
        {
            var value = new ValueCursor("--10xyz");
            Assert.True(value.MoveNext());
            long result;
            Assert.IsNotNull(value.ParseInt64<string>(out result));
            // Cursor has not moved
            Assert.AreEqual(0, value.Index);
        }

        [Test]
        public void ParseInt64_NumberOutOfRange_LowLeadingDigits()
        {
            var value = new ValueCursor("1000000000000000000000000");
            Assert.True(value.MoveNext());
            long result;
            Assert.IsNotNull(value.ParseInt64<string>(out result));
            // Cursor has not moved
            Assert.AreEqual(0, value.Index);
        }

        [Test]
        public void ParseInt64_NumberOutOfRange_HighLeadingDigits()
        {
            var value = new ValueCursor("999999999999999999999999");
            Assert.True(value.MoveNext());
            long result;
            Assert.IsNotNull(value.ParseInt64<string>(out result));
            // Cursor has not moved
            Assert.AreEqual(0, value.Index);
        }

        [Test]
        public void ParseInt64_NumberOutOfRange_MaxValueLeadingDigits()
        {
            var value = new ValueCursor("9223372036854775808");
            Assert.True(value.MoveNext());
            long result;
            Assert.IsNotNull(value.ParseInt64<string>(out result));
            // Cursor has not moved
            Assert.AreEqual(0, value.Index);
        }

        [Test]
        public void ParseInt64_NumberOutOfRange_MinValueLeadingDigits()
        {
            var value = new ValueCursor("-9223372036854775809");
            Assert.True(value.MoveNext());
            long result;
            Assert.IsNotNull(value.ParseInt64<string>(out result));
            // Cursor has not moved
            Assert.AreEqual(0, value.Index);
        }

        [Test]
        public void ParseInt64_MaxValue()
        {
            var value = new ValueCursor("9223372036854775807");
            Assert.True(value.MoveNext());
            long result;
            Assert.IsNull(value.ParseInt64<string>(out result));
            Assert.AreEqual(long.MaxValue, result);
        }

        [Test]
        public void ParseInt64_MinValue()
        {
            var value = new ValueCursor("-9223372036854775808");
            Assert.True(value.MoveNext());
            long result;
            Assert.IsNull(value.ParseInt64<string>(out result));
            Assert.AreEqual(long.MinValue, result);
        }
    }
}
