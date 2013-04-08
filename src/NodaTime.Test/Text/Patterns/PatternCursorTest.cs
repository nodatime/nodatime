// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using NodaTime.Text;
using NodaTime.Text.Patterns;

namespace NodaTime.Test.Text.Patterns
{
    [TestFixture, Category("Formatting"), Category("Format"), Category("Parse")]
    public class PatternTest : TextCursorTestBase
    {
        internal override TextCursor MakeCursor(string value)
        {
            return new PatternCursor(value);
        }

        [Test]
        public void TestGetQuotedString_EscapeAtEnd()
        {
            var cursor = new PatternCursor("'abc\\");
            Assert.AreEqual('\'', GetNextCharacter(cursor));            
            Assert.Throws<InvalidPatternException>(() => cursor.GetQuotedString('\''));
        }

        [Test]
        public void TestGetQuotedString()
        {
            var cursor = new PatternCursor("'abc'");
            Assert.AreEqual('\'', GetNextCharacter(cursor));
            string actual = cursor.GetQuotedString('\'');
            Assert.AreEqual("abc", actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_empty()
        {
            var cursor = new PatternCursor("''");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote);
            Assert.AreEqual(string.Empty, actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_handlesDoubleQuote()
        {
            var cursor = new PatternCursor("\"abc\"");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_handlesEscape()
        {
            var cursor = new PatternCursor("'ab\\c'");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_handlesEscapedCloseQuote()
        {
            var cursor = new PatternCursor("'ab\\'c'");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote);
            Assert.AreEqual("ab'c", actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_handlesOtherQuote()
        {
            var cursor = new PatternCursor("[abc]");
            GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(']');
            Assert.AreEqual("abc", actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_missingCloseQuote()
        {
            var cursor = new PatternCursor("'abc");
            char openQuote = GetNextCharacter(cursor);
            Assert.Throws<InvalidPatternException>(() => cursor.GetQuotedString(openQuote));
        }

        [Test]
        public void TestGetQuotedString_notAtEnd()
        {
            var cursor = new PatternCursor("'abc'more");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            ValidateCurrentCharacter(cursor, 4, '\'');

            Assert.AreEqual('m', GetNextCharacter(cursor));
        }

        [Test]
        public void TestGetQuotedString_simple()
        {
            var cursor = new PatternCursor("'abc'");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetRepeatCount_current()
        {
            var cursor = new PatternCursor("aaa");
            GetNextCharacter(cursor);
            int actual = cursor.GetRepeatCount(10);
            Assert.AreEqual(3, actual);
            ValidateCurrentCharacter(cursor, 2, 'a');
        }

        [Test]
        public void TestGetRepeatCount_exceedsMax()
        {
            var cursor = new PatternCursor("aaa");
            char ch = GetNextCharacter(cursor);
            Assert.Throws<InvalidPatternException>(() => cursor.GetRepeatCount(2, ch));
        }

        [Test]
        public void TestGetRepeatCount_one()
        {
            var cursor = new PatternCursor("a");
            char ch = GetNextCharacter(cursor);
            int actual = cursor.GetRepeatCount(10, ch);
            Assert.AreEqual(1, actual);
            ValidateCurrentCharacter(cursor, 0, 'a');
        }

        [Test]
        public void TestGetRepeatCount_stopsOnNonMatch()
        {
            var cursor = new PatternCursor("aaadaa");
            char ch = GetNextCharacter(cursor);
            int actual = cursor.GetRepeatCount(10, ch);
            Assert.AreEqual(3, actual);
            ValidateCurrentCharacter(cursor, 2, 'a');
        }

        [Test]
        public void TestGetRepeatCount_three()
        {
            var cursor = new PatternCursor("aaa");
            char ch = GetNextCharacter(cursor);
            int actual = cursor.GetRepeatCount(10, ch);
            Assert.AreEqual(3, actual);
            ValidateCurrentCharacter(cursor, 2, 'a');
        }
    }
}
