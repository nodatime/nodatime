// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Globalization;
using NodaTime.Text;
using NUnit.Framework;

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
        public void Match_Char()
        {
            var value = new ValueCursor("abc");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.True(value.Match('a'), "First character");
            Assert.True(value.Match('b'), "Second character");
            Assert.True(value.Match('c'), "Third character");
            Assert.False(value.MoveNext(), "GetNext() end");
        }

        [Test]
        public void Match_String()
        {
            var value = new ValueCursor("abc");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.True(value.Match("abc"));
            Assert.False(value.MoveNext(), "GetNext() end");
        }

        [Test]
        public void Match_StringNotMatched()
        {
            var value = new ValueCursor("xabcdef");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.False(value.Match("abc"));
            ValidateCurrentCharacter(value, 0, 'x');
        }

        [Test]
        public void Match_StringOverLongStringToMatch()
        {
            var value = new ValueCursor("x");
            Assert.True(value.MoveNext());
            Assert.False(value.Match("long string"));
            ValidateCurrentCharacter(value, 0, 'x');
        }

        [Test]
        public void MatchCaseInsensitive_MatchAndMove()
        {
            var value = new ValueCursor("abcd");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.True(value.MatchCaseInsensitive("AbC", CultureInfo.InvariantCulture.CompareInfo, true));
            ValidateCurrentCharacter(value, 3, 'd');
        }

        [Test]
        public void MatchCaseInsensitive_MatchWithoutMoving()
        {
            var value = new ValueCursor("abcd");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.True(value.MatchCaseInsensitive("AbC", CultureInfo.InvariantCulture.CompareInfo, false));
            // We're still looking at the start
            ValidateCurrentCharacter(value, 0, 'a');
        }

        [Test]
        public void MatchCaseInsensitive_StringNotMatched()
        {
            var value = new ValueCursor("xabcdef");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.False(value.MatchCaseInsensitive("abc", CultureInfo.InvariantCulture.CompareInfo, true));
            ValidateCurrentCharacter(value, 0, 'x');
        }

        [Test]
        public void MatchCaseInsensitive_StringOverLongStringToMatch()
        {
            var value = new ValueCursor("x");
            Assert.True(value.MoveNext());
            Assert.False(value.MatchCaseInsensitive("long string", CultureInfo.InvariantCulture.CompareInfo, true));
            ValidateCurrentCharacter(value, 0, 'x');
        }

        [Test]
        public void Match_StringPartial()
        {
            var value = new ValueCursor("abcdef");
            Assert.True(value.MoveNext(), "GetNext() 1");
            Assert.True(value.Match("abc"));
            ValidateCurrentCharacter(value, 3, 'd');
        }

        [Test]
        public void ParseDigits_TooFewDigits()
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
        public void ParseDigits_NoNumber()
        {
            var value = new ValueCursor("abc");
            Assert.True(value.MoveNext());
            int actual;
            Assert.False(value.ParseDigits(1, 2, out actual));
            ValidateCurrentCharacter(value, 0, 'a');
        }

        [Test]
        public void ParseDigits_Maximum()
        {
            var value = new ValueCursor("12");
            Assert.True(value.MoveNext());
            int actual;
            Assert.True(value.ParseDigits(1, 2, out actual));
            Assert.AreEqual(12, actual);
        }

        [Test]
        public void ParseDigits_MaximumMoreDigits()
        {
            var value = new ValueCursor("1234");
            Assert.True(value.MoveNext());
            int actual;
            Assert.True(value.ParseDigits(1, 2, out actual));
            Assert.AreEqual(12, actual);
            ValidateCurrentCharacter(value, 2, '3');
        }

        [Test]
        public void ParseDigits_Minimum()
        {
            var value = new ValueCursor("1");
            value.MoveNext();
            int actual;
            Assert.True(value.ParseDigits(1, 2, out actual));
            Assert.AreEqual(1, actual);
            ValidateEndOfString(value);
        }

        [Test]
        public void ParseDigits_MinimumNonDigits()
        {
            var value = new ValueCursor("1abc");
            Assert.True(value.MoveNext());
            int actual;
            Assert.True(value.ParseDigits(1, 2, out actual));
            Assert.AreEqual(1, actual);
            ValidateCurrentCharacter(value, 1, 'a');
        }

        [Test]
        public void ParseDigits_NonAscii_NeverMatches()
        {
            // Arabic-Indic digits 0 and 1. See
            // http://www.unicode.org/charts/PDF/U0600.pdf
            var value = new ValueCursor("\u0660\u0661");
            Assert.True(value.MoveNext());
            int actual;
            Assert.False(value.ParseDigits(1, 2, out actual));
        }

        [Test]
        public void ParseInt64Digits_TooFewDigits()
        {
            var value = new ValueCursor("a12b");
            Assert.True(value.MoveNext());
            ValidateCurrentCharacter(value, 0, 'a');
            Assert.True(value.MoveNext());
            long actual;
            Assert.False(value.ParseInt64Digits(3, 3, out actual));
            ValidateCurrentCharacter(value, 1, '1');
        }

        [Test]
        public void ParseInt64Digits_NoNumber()
        {
            var value = new ValueCursor("abc");
            Assert.True(value.MoveNext());
            long actual;
            Assert.False(value.ParseInt64Digits(1, 2, out actual));
            ValidateCurrentCharacter(value, 0, 'a');
        }

        [Test]
        public void ParseInt64Digits_Maximum()
        {
            var value = new ValueCursor("12");
            Assert.True(value.MoveNext());
            long actual;
            Assert.True(value.ParseInt64Digits(1, 2, out actual));
            Assert.AreEqual(12, actual);
        }

        [Test]
        public void ParseInt64Digits_MaximumMoreDigits()
        {
            var value = new ValueCursor("1234");
            Assert.True(value.MoveNext());
            long actual;
            Assert.True(value.ParseInt64Digits(1, 2, out actual));
            Assert.AreEqual(12, actual);
            ValidateCurrentCharacter(value, 2, '3');
        }

        [Test]
        public void ParseInt64Digits_Minimum()
        {
            var value = new ValueCursor("1");
            value.MoveNext();
            long actual;
            Assert.True(value.ParseInt64Digits(1, 2, out actual));
            Assert.AreEqual(1, actual);
            ValidateEndOfString(value);
        }

        [Test]
        public void ParseInt64Digits_MinimumNonDigits()
        {
            var value = new ValueCursor("1abc");
            Assert.True(value.MoveNext());
            long actual;
            Assert.True(value.ParseInt64Digits(1, 2, out actual));
            Assert.AreEqual(1, actual);
            ValidateCurrentCharacter(value, 1, 'a');
        }

        [Test]
        public void ParseInt64Digits_NonAscii_NeverMatches()
        {
            // Arabic-Indic digits 0 and 1. See
            // http://www.unicode.org/charts/PDF/U0600.pdf
            var value = new ValueCursor("\u0660\u0661");
            Assert.True(value.MoveNext());
            long actual;
            Assert.False(value.ParseInt64Digits(1, 2, out actual));
        }

        [Test]
        public void ParseInt64Digits_LargeNumber()
        {
            var value = new ValueCursor("9999999999999");
            Assert.True(value.MoveNext());
            long actual;
            Assert.True(value.ParseInt64Digits(1, 13, out actual));
            Assert.AreEqual(actual, 9999999999999L);
            Assert.Greater(9999999999999L, int.MaxValue);
        }

        [Test]
        public void ParseFraction_NonAscii_NeverMatches()
        {
            // Arabic-Indic digits 0 and 1. See
            // http://www.unicode.org/charts/PDF/U0600.pdf
            var value = new ValueCursor("\u0660\u0661");
            Assert.True(value.MoveNext());
            int actual;
            Assert.False(value.ParseFraction(2, 2, out actual, 2));
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
        public void ParseInt64_NegativeThenNonDigit()
        {
            var value = new ValueCursor("-x");
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

        [Test]
        public void CompareOrdinal_ExactMatchToEndOfValue()
        {
            var value = new ValueCursor("xabc");
            value.Move(1);
            Assert.AreEqual(0, value.CompareOrdinal("abc"));
            Assert.AreEqual(1, value.Index); // Cursor hasn't moved
        }

        [Test]
        public void CompareOrdinal_ExactMatchValueContinues()
        {
            var value = new ValueCursor("xabc");
            value.Move(1);
            Assert.AreEqual(0, value.CompareOrdinal("ab"));
            Assert.AreEqual(1, value.Index); // Cursor hasn't moved
        }

        [Test]
        public void CompareOrdinal_ValueIsEarlier()
        {
            var value = new ValueCursor("xabc");
            value.Move(1);
            Assert.Less(value.CompareOrdinal("mm"), 0);
            Assert.AreEqual(1, value.Index); // Cursor hasn't moved
        }

        [Test]
        public void CompareOrdinal_ValueIsLater()
        {
            var value = new ValueCursor("xabc");
            value.Move(1);
            Assert.Greater(value.CompareOrdinal("aa"), 0);
            Assert.AreEqual(1, value.Index); // Cursor hasn't moved
        }

        [Test]
        public void CompareOrdinal_LongMatch_EqualToEnd()
        {
            var value = new ValueCursor("xabc");
            value.Move(1);
            Assert.Less(value.CompareOrdinal("abcd"), 0);
            Assert.AreEqual(1, value.Index); // Cursor hasn't moved
        }

        [Test]
        public void CompareOrdinal_LongMatch_ValueIsEarlier()
        {
            var value = new ValueCursor("xabc");
            value.Move(1);
            Assert.Less(value.CompareOrdinal("cccc"), 0);
            Assert.AreEqual(1, value.Index); // Cursor hasn't moved
        }

        [Test]
        public void CompareOrdinal_LongMatch_ValueIsLater()
        {
            var value = new ValueCursor("xabc");
            value.Move(1);
            Assert.Greater(value.CompareOrdinal("aaaa"), 0);
            Assert.AreEqual(1, value.Index); // Cursor hasn't moved
        }
    }
}
