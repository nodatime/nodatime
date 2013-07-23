// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using NodaTime.Text;
using NodaTime.Text.Patterns;

namespace NodaTime.Test.Text.Patterns
{
    [TestFixture, Category("Formatting"), Category("Format"), Category("Parse")]
    public class PatternCursorTest : TextCursorTestBase
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
        public void TestGetQuotedString_Empty()
        {
            var cursor = new PatternCursor("''");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote);
            Assert.AreEqual(string.Empty, actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_HandlesDoubleQuote()
        {
            var cursor = new PatternCursor("\"abc\"");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_HandlesEscape()
        {
            var cursor = new PatternCursor("'ab\\c'");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_HandlesEscapedCloseQuote()
        {
            var cursor = new PatternCursor("'ab\\'c'");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote);
            Assert.AreEqual("ab'c", actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_HandlesOtherQuote()
        {
            var cursor = new PatternCursor("[abc]");
            GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(']');
            Assert.AreEqual("abc", actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetQuotedString_MissingCloseQuote()
        {
            var cursor = new PatternCursor("'abc");
            char openQuote = GetNextCharacter(cursor);
            Assert.Throws<InvalidPatternException>(() => cursor.GetQuotedString(openQuote));
        }

        [Test]
        public void TestGetQuotedString_NotAtEnd()
        {
            var cursor = new PatternCursor("'abc'more");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            ValidateCurrentCharacter(cursor, 4, '\'');

            Assert.AreEqual('m', GetNextCharacter(cursor));
        }

        [Test]
        public void TestGetQuotedString_Simple()
        {
            var cursor = new PatternCursor("'abc'");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void TestGetRepeatCount_Current()
        {
            var cursor = new PatternCursor("aaa");
            GetNextCharacter(cursor);
            int actual = cursor.GetRepeatCount(10);
            Assert.AreEqual(3, actual);
            ValidateCurrentCharacter(cursor, 2, 'a');
        }

        [Test]
        public void TestGetRepeatCount_ExceedsMax()
        {
            var cursor = new PatternCursor("aaa");
            Assert.IsTrue(cursor.MoveNext());
            Assert.Throws<InvalidPatternException>(() => cursor.GetRepeatCount(2));
        }

        [Test]
        public void TestGetRepeatCount_One()
        {
            var cursor = new PatternCursor("a");
            Assert.IsTrue(cursor.MoveNext());
            int actual = cursor.GetRepeatCount(10);
            Assert.AreEqual(1, actual);
            ValidateCurrentCharacter(cursor, 0, 'a');
        }

        [Test]
        public void TestGetRepeatCount_StopsOnNonMatch()
        {
            var cursor = new PatternCursor("aaadaa");
            Assert.IsTrue(cursor.MoveNext());
            int actual = cursor.GetRepeatCount(10);
            Assert.AreEqual(3, actual);
            ValidateCurrentCharacter(cursor, 2, 'a');
        }

        [Test]
        public void TestGetRepeatCount_Three()
        {
            var cursor = new PatternCursor("aaa");
            Assert.IsTrue(cursor.MoveNext());
            int actual = cursor.GetRepeatCount(10);
            Assert.AreEqual(3, actual);
            ValidateCurrentCharacter(cursor, 2, 'a');
        }

        [Test]
        public void TestGetEmbeddedPattern_Valid()
        {
            var cursor = new PatternCursor("x<HH:mm>y");
            cursor.MoveNext();
            string embedded = cursor.GetEmbeddedPattern('<', '>');
            Assert.AreEqual("HH:mm", embedded);
            ValidateCurrentCharacter(cursor, 7, '>');
        }

        [Test]
        public void TestGetEmbeddedPattern_Valid_WithQuoting()
        {
            var cursor = new PatternCursor("x<HH:'T'mm>y");
            cursor.MoveNext();
            string embedded = cursor.GetEmbeddedPattern('<', '>');
            Assert.AreEqual("HH:'T'mm", embedded);
            Assert.AreEqual('>', cursor.Current);
        }

        [Test]
        public void TestGetEmbeddedPattern_Valid_WithEscaping()
        {
            var cursor = new PatternCursor(@"x<HH:\Tmm>y");
            cursor.MoveNext();
            string embedded = cursor.GetEmbeddedPattern('<', '>');
            Assert.AreEqual(@"HH:\Tmm", embedded);
            Assert.AreEqual('>', cursor.Current);
        }

        [Test]
        public void TestGetEmbeddedPattern_WrongOpenCharacter()
        {
            var cursor = new PatternCursor("x(oops)");
            cursor.MoveNext();
            Assert.Throws<InvalidPatternException>(() => cursor.GetEmbeddedPattern('<', '>'));
        }

        [Test]
        public void TestGetEmbeddedPattern_NoCloseCharacter()
        {
            var cursor = new PatternCursor("x<oops)");
            cursor.MoveNext();
            Assert.Throws<InvalidPatternException>(() => cursor.GetEmbeddedPattern('<', '>'));
        }

        [Test]
        public void TestGetEmbeddedPattern_EscapedCloseCharacter()
        {
            var cursor = new PatternCursor(@"x<oops\>");
            cursor.MoveNext();
            Assert.Throws<InvalidPatternException>(() => cursor.GetEmbeddedPattern('<', '>'));
        }

        [Test]
        public void TestGetEmbeddedPattern_QuotedCloseCharacter()
        {
            var cursor = new PatternCursor("x<oops'>'");
            cursor.MoveNext();
            Assert.Throws<InvalidPatternException>(() => cursor.GetEmbeddedPattern('<', '>'));
        }
    }
}
