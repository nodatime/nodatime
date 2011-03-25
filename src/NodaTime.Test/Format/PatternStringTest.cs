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
    public class PatternStringTest
    {
        [Test]
        public void TestConstructor()
        {
            Assert.DoesNotThrow(() => new PatternString("this is a test"));
        }

        [Test]
        public void TestConstructor_empty()
        {
            Assert.Throws<ArgumentException>(() => new PatternString(""));
        }

        [Test]
        public void TestConstructor_null()
        {
            Assert.Throws<ArgumentNullException>(() => new PatternString(null));
        }

        [Test]
        public void TestGetNextCharacter()
        {
            var pattern = new PatternString("a");
            char actual = pattern.GetNextCharacter();
            Assert.AreEqual('a', actual);
            Assert.Throws<FormatException>(() => pattern.GetNextCharacter());
        }

        [Test]
        public void TestGetQuotedString_EscapeAtEnd()
        {
            var pattern = new PatternString("'abc\\");
            char openQuote;
            Assert.True(pattern.TryGetNextCharacter(out openQuote));
            Assert.AreEqual('\'', openQuote);
            Assert.Throws<FormatException>(() => pattern.GetQuotedString(openQuote));
        }

        [Test]
        public void TestGetQuotedString_empty()
        {
            var pattern = new PatternString("''");
            char openQuote;
            Assert.True(pattern.TryGetNextCharacter(out openQuote));
            Assert.AreEqual('\'', openQuote);
            string actual = pattern.GetQuotedString(openQuote);
            Assert.AreEqual(string.Empty, actual);
            Assert.False(pattern.HasMoreCharacters);
        }

        [Test]
        public void TestGetQuotedString_handlesDoubleQuote()
        {
            var pattern = new PatternString("\"abc\"");
            char openQuote;
            Assert.True(pattern.TryGetNextCharacter(out openQuote));
            Assert.AreEqual('"', openQuote);
            string actual = pattern.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            Assert.False(pattern.HasMoreCharacters);
        }

        [Test]
        public void TestGetQuotedString_handlesEscape()
        {
            var pattern = new PatternString("'ab\\c'");
            char openQuote;
            Assert.True(pattern.TryGetNextCharacter(out openQuote));
            Assert.AreEqual('\'', openQuote);
            string actual = pattern.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            Assert.False(pattern.HasMoreCharacters);
        }

        [Test]
        public void TestGetQuotedString_handlesEscapedCloseQuote()
        {
            var pattern = new PatternString("'ab\\'c'");
            char openQuote;
            Assert.True(pattern.TryGetNextCharacter(out openQuote));
            Assert.AreEqual('\'', openQuote);
            string actual = pattern.GetQuotedString(openQuote);
            Assert.AreEqual("ab'c", actual);
            Assert.False(pattern.HasMoreCharacters);
        }

        [Test]
        public void TestGetQuotedString_handlesOtherQuote()
        {
            var pattern = new PatternString("[abc]");
            char openQuote;
            Assert.True(pattern.TryGetNextCharacter(out openQuote));
            Assert.AreEqual('[', openQuote);
            string actual = pattern.GetQuotedString(']');
            Assert.AreEqual("abc", actual);
            Assert.False(pattern.HasMoreCharacters);
        }

        [Test]
        public void TestGetQuotedString_missingCloseQuote()
        {
            var pattern = new PatternString("'abc");
            char openQuote;
            Assert.True(pattern.TryGetNextCharacter(out openQuote));
            Assert.AreEqual('\'', openQuote);
            Assert.Throws<FormatException>(() => pattern.GetQuotedString(openQuote));
        }

        [Test]
        public void TestGetQuotedString_notAtEnd()
        {
            var pattern = new PatternString("'abc'more");
            char openQuote;
            Assert.True(pattern.TryGetNextCharacter(out openQuote));
            Assert.AreEqual('\'', openQuote);
            string actual = pattern.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            Assert.True(pattern.HasMoreCharacters);
        }

        [Test]
        public void TestGetQuotedString_simple()
        {
            var pattern = new PatternString("'abc'");
            char openQuote;
            Assert.True(pattern.TryGetNextCharacter(out openQuote));
            Assert.AreEqual('\'', openQuote);
            string actual = pattern.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            Assert.False(pattern.HasMoreCharacters);
        }

        [Test]
        public void TestGetRepeatCount_exceedsMax()
        {
            var pattern = new PatternString("aaa");
            char ch;
            Assert.True(pattern.TryGetNextCharacter(out ch));
            Assert.Throws<FormatException>(() => pattern.GetRepeatCount(ch, 2));
        }

        [Test]
        public void TestGetRepeatCount_one()
        {
            var pattern = new PatternString("a");
            char ch;
            Assert.True(pattern.TryGetNextCharacter(out ch));
            int actual = pattern.GetRepeatCount(ch, 10);
            Assert.AreEqual(1, actual);
            Assert.False(pattern.HasMoreCharacters);
        }

        [Test]
        public void TestGetRepeatCount_stopsOnNonMatch()
        {
            var pattern = new PatternString("aaadaa");
            char ch;
            Assert.True(pattern.TryGetNextCharacter(out ch));
            int actual = pattern.GetRepeatCount(ch, 10);
            Assert.AreEqual(3, actual);
            Assert.True(pattern.HasMoreCharacters);
        }

        [Test]
        public void TestGetRepeatCount_three()
        {
            var pattern = new PatternString("aaa");
            char ch;
            Assert.True(pattern.TryGetNextCharacter(out ch));
            int actual = pattern.GetRepeatCount(ch, 10);
            Assert.AreEqual(3, actual);
            Assert.False(pattern.HasMoreCharacters);
        }

        [Test]
        public void TestTryGetNextCharacter()
        {
            var pattern = new PatternString("a");
            char actual;
            Assert.True(pattern.TryGetNextCharacter(out actual));
            Assert.AreEqual('a', actual);
            Assert.False(pattern.TryGetNextCharacter(out actual));
            Assert.AreEqual('\u0000', actual);
        }
    }
}