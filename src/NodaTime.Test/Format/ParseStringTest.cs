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

using NUnit.Framework;
using NodaTime.Format;

namespace NodaTime.Test.Format
{
    [TestFixture]
    [Category("Formatting")]
    [Category("Parse")]
    public class ParseStringTest : ParsableTest
    {
        internal override Parsable MakeParsable(string value)
        {
            return new ParseString(value);
        }

        [Test]
        public void TestMatch_char()
        {
            var value = new ParseString("abc");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.True(value.Match('a'), "First character");
            Assert.True(value.Match('b'), "Second character");
            Assert.True(value.Match('c'), "Third character");
            Assert.False(value.MoveNext(), "GetNext() end");
        }

        [Test]
        public void TestMatch_string()
        {
            var value = new ParseString("abc");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.True(value.Match("abc"));
            Assert.False(value.MoveNext(), "GetNext() end");
        }

        [Test]
        public void TestMatch_stringNotMatched()
        {
            var value = new ParseString("xabcdef");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.False(value.Match("abc"));
            ValidateCharacter(value, 0, 'x');
        }

        [Test]
        public void TestMatch_stringPartial()
        {
            var value = new ParseString("abcdef");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.True(value.Match("abc"));
            ValidateCharacter(value, 3, 'd');
        }

        [Test]
        public void TestParseDigit_failureTooFewDigits()
        {
            var value = new ParseString("a12b");
            Assert.True(value.MoveNext());
            ValidateCharacter(value, 0, 'a');
            Assert.True(value.MoveNext());
            int actual;
            Assert.False(value.ParseDigits(3, 3, out actual));
            ValidateCharacter(value, 1, '1');
        }

        [Test]
        public void TestParseDigit_noNumber()
        {
            var value = new ParseString("abc");
            Assert.True(value.MoveNext());
            int actual;
            Assert.False(value.ParseDigits(1, 2, out actual));
            ValidateCharacter(value, 0, 'a');
        }

        [Test]
        public void TestParseDigit_successMaximum()
        {
            var value = new ParseString("12");
            Assert.True(value.MoveNext());
            int actual;
            Assert.True(value.ParseDigits(1, 2, out actual));
            Assert.AreEqual(12, actual);
        }

        [Test]
        public void TestParseDigit_successMaximumMoreDigits()
        {
            var value = new ParseString("1234");
            Assert.True(value.MoveNext());
            int actual;
            Assert.True(value.ParseDigits(1, 2, out actual));
            Assert.AreEqual(12, actual);
            ValidateCharacter(value, 2, '3');
        }

        [Test]
        public void TestParseDigit_successMinimum()
        {
            var value = new ParseString("1");
            value.MoveNext();
            int actual;
            Assert.True(value.ParseDigits(1, 2, out actual));
            Assert.AreEqual(1, actual);
            ValidateEndOfString(value);
        }

        [Test]
        public void TestParseDigit_successMinimumNonDigits()
        {
            var value = new ParseString("1abc");
            Assert.True(value.MoveNext());
            int actual;
            Assert.True(value.ParseDigits(1, 2, out actual));
            Assert.AreEqual(1, actual);
            ValidateCharacter(value, 1, 'a');
        }
    }
}