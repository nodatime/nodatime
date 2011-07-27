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
using NodaTime.Format;
using NUnit.Framework;
using System;
#endregion

namespace NodaTime.Test.Format
{
    [TestFixture]
    [Category("Formating")]
    [Category("Format")]
    [Category("Parse")]
    public class PatternTest : ParsableTest
    {
        internal override Parsable MakeParsable(string value)
        {
            return new Pattern(value);
        }

        [Test]
        public void TestGetQuotedString_EscapeAtEnd()
        {
            var pattern = new Pattern("'abc\\");
            char openQuote = pattern.GetNextCharacter();
            Assert.Throws<FormatException>(() => pattern.GetQuotedString(openQuote));
        }

        [Test]
        public void TestGetQuotedString_current()
        {
            var pattern = new Pattern("'abc'");
            pattern.GetNextCharacter();
            string actual = pattern.GetQuotedString();
            Assert.AreEqual("abc", actual);
            ValidateEndOfString(pattern);
        }

        [Test]
        public void TestGetQuotedString_empty()
        {
            var pattern = new Pattern("''");
            char openQuote = pattern.GetNextCharacter();
            string actual = pattern.GetQuotedString(openQuote);
            Assert.AreEqual(string.Empty, actual);
            ValidateEndOfString(pattern);
        }

        [Test]
        public void TestGetQuotedString_handlesDoubleQuote()
        {
            var pattern = new Pattern("\"abc\"");
            char openQuote = pattern.GetNextCharacter();
            string actual = pattern.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            ValidateEndOfString(pattern);
        }

        [Test]
        public void TestGetQuotedString_handlesEscape()
        {
            var pattern = new Pattern("'ab\\c'");
            char openQuote = pattern.GetNextCharacter();
            string actual = pattern.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            ValidateEndOfString(pattern);
        }

        [Test]
        public void TestGetQuotedString_handlesEscapedCloseQuote()
        {
            var pattern = new Pattern("'ab\\'c'");
            char openQuote = pattern.GetNextCharacter();
            string actual = pattern.GetQuotedString(openQuote);
            Assert.AreEqual("ab'c", actual);
            ValidateEndOfString(pattern);
        }

        [Test]
        public void TestGetQuotedString_handlesOtherQuote()
        {
            var pattern = new Pattern("[abc]");
            pattern.GetNextCharacter();
            string actual = pattern.GetQuotedString(']');
            Assert.AreEqual("abc", actual);
            ValidateEndOfString(pattern);
        }

        [Test]
        public void TestGetQuotedString_missingCloseQuote()
        {
            var pattern = new Pattern("'abc");
            char openQuote = pattern.GetNextCharacter();
            Assert.Throws<FormatException>(() => pattern.GetQuotedString(openQuote));
        }

        [Test]
        public void TestGetQuotedString_notAtEnd()
        {
            var pattern = new Pattern("'abc'more");
            char openQuote = pattern.GetNextCharacter();
            string actual = pattern.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            ValidateCharacter(pattern, 5, 'm');
        }

        [Test]
        public void TestGetQuotedString_simple()
        {
            var pattern = new Pattern("'abc'");
            char openQuote = pattern.GetNextCharacter();
            string actual = pattern.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            ValidateEndOfString(pattern);
        }

        [Test]
        public void TestGetRepeatCount_current()
        {
            var pattern = new Pattern("aaa");
            pattern.GetNextCharacter();
            int actual = pattern.GetRepeatCount(10);
            Assert.AreEqual(3, actual);
            ValidateEndOfString(pattern);
        }

        [Test]
        public void TestGetRepeatCount_exceedsMax()
        {
            var pattern = new Pattern("aaa");
            char ch = pattern.GetNextCharacter();
            Assert.Throws<FormatError.FormatValueException>(() => pattern.GetRepeatCount(2, ch));
        }

        [Test]
        public void TestGetRepeatCount_one()
        {
            var pattern = new Pattern("a");
            char ch = pattern.GetNextCharacter();
            int actual = pattern.GetRepeatCount(10, ch);
            Assert.AreEqual(1, actual);
            ValidateEndOfString(pattern);
        }

        [Test]
        public void TestGetRepeatCount_stopsOnNonMatch()
        {
            var pattern = new Pattern("aaadaa");
            char ch = pattern.GetNextCharacter();
            int actual = pattern.GetRepeatCount(10, ch);
            Assert.AreEqual(3, actual);
            ValidateCharacter(pattern, 2, 'a');
        }

        [Test]
        public void TestGetRepeatCount_three()
        {
            var pattern = new Pattern("aaa");
            char ch = pattern.GetNextCharacter();
            int actual = pattern.GetRepeatCount(10, ch);
            Assert.AreEqual(3, actual);
            ValidateEndOfString(pattern);
        }
    }
}