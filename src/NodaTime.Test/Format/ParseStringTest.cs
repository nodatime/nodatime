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
using NodaTime.Format;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    [TestFixture]
    public class ParseStringTest
    {
        [Test]
        public void TestConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new ParseString(null));
            var value = new ParseString("abcd");
            Assert.AreEqual('\u0000', value.Current);
        }

        [Test]
        public void TestGetNext()
        {
            var value = new ParseString("abc");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.AreEqual('a', value.Current, "First character");
            Assert.True(value.MoveNext(), "GetNext() 2");
            Assert.AreEqual('b', value.Current, "Second character");
            Assert.True(value.MoveNext(), "GetNext() 3");
            Assert.AreEqual('c', value.Current, "Third character");
            Assert.False(value.MoveNext(), "GetNext() end");
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
            Assert.AreEqual('x', value.Current);
        }

        [Test]
        public void TestMatch_stringPartial()
        {
            var value = new ParseString("abcdef");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.True(value.Match("abc"));
            Assert.AreEqual('d', value.Current);
        }

        [Test]
        public void TestParseDigit_failureTooFewDigits()
        {
            var value = new ParseString("a1b");
            Assert.True(value.MoveNext());
            Assert.AreEqual('a', value.Current);
            Assert.True(value.MoveNext());
            int actual;
            Assert.False(value.ParseDigits(2, 2, out actual));
            Assert.AreEqual('1', value.Current);
        }

        [Test]
        public void TestParseDigit_noNumber()
        {
            var value = new ParseString("abc");
            int actual;
            Assert.False(value.ParseDigits(1, 2, out actual));
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
            Assert.AreEqual('3', value.Current);
        }

        [Test]
        public void TestParseDigit_successMinimum()
        {
            var value = new ParseString("1");
            value.MoveNext();
            int actual;
            Assert.True(value.ParseDigits(1, 2, out actual));
            Assert.AreEqual(1, actual);
        }

        [Test]
        public void TestParseDigit_successMinimumNonDigits()
        {
            var value = new ParseString("1abc");
            Assert.True(value.MoveNext());
            int actual;
            Assert.True(value.ParseDigits(1, 2, out actual));
            Assert.AreEqual(1, actual);
            Assert.AreEqual('1', value.Current);
        }
    }
}