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
using NodaTime.Text;
using NodaTime.Text.Patterns;

namespace NodaTime.Test.Text.Patterns
{
    [TestFixture, Category("Formatting"), Category("Format"), Category("Parse")]
    public class PatternTest : TextCursorTestBase
    {
        // Used all over the place... might as well have a single declaration
        private PatternParseResult<int> failure;

        [SetUp]
        public void SetUp()
        {
            failure = null;
        }

        internal override TextCursor MakeCursor(string value)
        {
            return new PatternCursor(value);
        }

        [Test]
        public void TestGetQuotedString_EscapeAtEnd()
        {
            var cursor = new PatternCursor("'abc\\");
            Assert.AreEqual('\'', GetNextCharacter(cursor));
            cursor.GetQuotedString('\'', ref failure);
            Assert.Throws<InvalidPatternException>(() => failure.GetResultOrThrow());
        }

        [Test]
        public void TestGetQuotedString_current()
        {
            var cursor = new PatternCursor("'abc'");
            Assert.AreEqual('\'', GetNextCharacter(cursor));
            string actual = cursor.GetQuotedString(ref failure);
            AssertNoFailure();
            Assert.AreEqual("abc", actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_empty()
        {
            var cursor = new PatternCursor("''");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote, ref failure);
            Assert.AreEqual(string.Empty, actual);
            AssertNoFailure();
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_handlesDoubleQuote()
        {
            var cursor = new PatternCursor("\"abc\"");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote, ref failure);
            Assert.AreEqual("abc", actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_handlesEscape()
        {
            var cursor = new PatternCursor("'ab\\c'");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote, ref failure);
            Assert.AreEqual("abc", actual);
            AssertNoFailure();
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_handlesEscapedCloseQuote()
        {
            var cursor = new PatternCursor("'ab\\'c'");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote, ref failure);
            Assert.AreEqual("ab'c", actual);
            AssertNoFailure();
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_handlesOtherQuote()
        {
            var cursor = new PatternCursor("[abc]");
            GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(']', ref failure);
            AssertNoFailure();
            Assert.AreEqual("abc", actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_missingCloseQuote()
        {
            var cursor = new PatternCursor("'abc");
            char openQuote = GetNextCharacter(cursor);
            cursor.GetQuotedString(openQuote, ref failure);
            Assert.Throws<InvalidPatternException>(() => failure.GetResultOrThrow());
        }

        [Test]
        public void TestGetQuotedString_notAtEnd()
        {
            var cursor = new PatternCursor("'abc'more");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote, ref failure);
            AssertNoFailure();
            Assert.AreEqual("abc", actual);
            ValidateCurrentCharacter(cursor, 4, '\'');

            Assert.AreEqual('m', GetNextCharacter(cursor));
        }

        [Test]
        public void TestGetQuotedString_simple()
        {
            var cursor = new PatternCursor("'abc'");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote, ref failure);
            AssertNoFailure();
            Assert.AreEqual("abc", actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetRepeatCount_current()
        {
            var cursor = new PatternCursor("aaa");
            GetNextCharacter(cursor);
            int actual = cursor.GetRepeatCount(10, ref failure);
            AssertNoFailure();
            Assert.AreEqual(3, actual);
            ValidateCurrentCharacter(cursor, 2, 'a');
        }

        [Test]
        public void TestGetRepeatCount_exceedsMax()
        {
            var cursor = new PatternCursor("aaa");
            char ch = GetNextCharacter(cursor);
            cursor.GetRepeatCount(2, ch, ref failure);
            AssertInvalidPatternFailure();
        }

        [Test]
        public void TestGetRepeatCount_one()
        {
            var cursor = new PatternCursor("a");
            char ch = GetNextCharacter(cursor);
            int actual = cursor.GetRepeatCount(10, ch, ref failure);
            AssertNoFailure();
            Assert.AreEqual(1, actual);
            ValidateCurrentCharacter(cursor, 0, 'a');
        }

        [Test]
        public void TestGetRepeatCount_stopsOnNonMatch()
        {
            var cursor = new PatternCursor("aaadaa");
            char ch = GetNextCharacter(cursor);
            int actual = cursor.GetRepeatCount(10, ch, ref failure);
            AssertNoFailure();
            Assert.AreEqual(3, actual);
            ValidateCurrentCharacter(cursor, 2, 'a');
        }

        [Test]
        public void TestGetRepeatCount_three()
        {
            var cursor = new PatternCursor("aaa");
            char ch = GetNextCharacter(cursor);
            int actual = cursor.GetRepeatCount(10, ch, ref failure);
            AssertNoFailure();
            Assert.AreEqual(3, actual);
            ValidateCurrentCharacter(cursor, 2, 'a');
        }

        private void AssertInvalidPatternFailure()
        {
            Assert.Throws<InvalidPatternException>(() => failure.GetResultOrThrow());
        }

        private void AssertNoFailure()
        {
            Assert.IsNull(failure);
        }

    }
}
