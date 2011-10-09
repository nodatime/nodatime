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
    }
}
