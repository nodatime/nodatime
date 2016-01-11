// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NodaTime.Text.Patterns;
using NUnit.Framework;

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
        [TestCase(@"'abc\", Description = "Escape at end")]
        [TestCase(@"'abc", Description = "Missing close quote")]
        public void GetQuotedString_Invalid(string pattern)
        {
            var cursor = new PatternCursor(pattern);
            Assert.AreEqual('\'', GetNextCharacter(cursor));
            Assert.Throws<InvalidPatternException>(() => cursor.GetQuotedString('\''));
        }

        [Test]
        [TestCase("'abc'", "abc")]
        [TestCase("''", "")]
        [TestCase("'\"abc\"'", "\"abc\"", Description = "Double quotes")]
        [TestCase("'ab\\c'", "abc", Description = "Escaped backslash")]
        [TestCase("'ab\\'c'", "ab'c", Description = "Escaped close quote")]
        public void GetQuotedString_Valid(string pattern, string expected)
        {
            var cursor = new PatternCursor(pattern);
            Assert.AreEqual('\'', GetNextCharacter(cursor));
            string actual = cursor.GetQuotedString('\'');
            Assert.AreEqual(expected, actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void GetQuotedString_HandlesOtherQuote()
        {
            var cursor = new PatternCursor("[abc]");
            GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(']');
            Assert.AreEqual("abc", actual);
            Assert.IsFalse(cursor.MoveNext());
        }

        [Test]
        public void GetQuotedString_NotAtEnd()
        {
            var cursor = new PatternCursor("'abc'more");
            char openQuote = GetNextCharacter(cursor);
            string actual = cursor.GetQuotedString(openQuote);
            Assert.AreEqual("abc", actual);
            ValidateCurrentCharacter(cursor, 4, '\'');

            Assert.AreEqual('m', GetNextCharacter(cursor));
        }

        [Test]
        [TestCase("aaa", 3)]
        [TestCase("a", 1)]
        [TestCase("aaadaa", 3)]
        public void GetRepeatCount_Valid(string text, int expectedCount)
        {
            var cursor = new PatternCursor(text);
            Assert.IsTrue(cursor.MoveNext());
            int actual = cursor.GetRepeatCount(10);
            Assert.AreEqual(expectedCount, actual);
            ValidateCurrentCharacter(cursor, expectedCount - 1, 'a');
        }

        [Test]
        public void GetRepeatCount_ExceedsMax()
        {
            var cursor = new PatternCursor("aaa");
            Assert.IsTrue(cursor.MoveNext());
            Assert.Throws<InvalidPatternException>(() => cursor.GetRepeatCount(2));
        }

        [Test]
        [TestCase("x<HH:mm>y", "HH:mm", Description = "Simple")]
        [TestCase("x<HH:'T'mm>y", "HH:'T'mm", Description = "Quoting")]
        [TestCase(@"x<HH:\Tmm>y", @"HH:\Tmm", Description = "Escaping")]
        [TestCase("x<a<b>c>y", "a<b>c", Description = "Simple nesting")]
        [TestCase("x<a'<'bc>y", "a'<'bc", Description = "Quoted start embedded")]
        [TestCase("x<a'>'bc>y", "a'>'bc", Description = "Quoted end embedded")]
        [TestCase(@"x<a\<bc>y", @"a\<bc", Description = "Escaped start embedded")]
        [TestCase(@"x<a\>bc>y", @"a\>bc", Description = "Escaped end embedded")]
        public void GetEmbeddedPattern_Valid(string pattern, string expectedEmbedded)
        {
            var cursor = new PatternCursor(pattern);
            cursor.MoveNext();
            string embedded = cursor.GetEmbeddedPattern();
            Assert.AreEqual(expectedEmbedded, embedded);
            ValidateCurrentCharacter(cursor, expectedEmbedded.Length + 2, '>');
        }

        [Test]
        [TestCase("x(oops)", Description = "Wrong start character")]
        [TestCase("x<oops)", Description = "No end")]
        [TestCase(@"x<oops\>", Description = "Escaped end")]
        [TestCase("x<oops'>'", Description = "Quoted end")]
        [TestCase("x<oops<nested>", Description = "Incomplete after nesting")]
        public void GetEmbeddedPattern_Invalid(string text)
        {
            var cursor = new PatternCursor(text);
            cursor.MoveNext();
            Assert.Throws<InvalidPatternException>(() => cursor.GetEmbeddedPattern());
        }
    }
}
