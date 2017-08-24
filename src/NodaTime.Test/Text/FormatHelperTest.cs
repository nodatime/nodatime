// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NUnit.Framework;
using System.Text;

namespace NodaTime.Test.Text
{
    public class FormatHelperTest
    {
        [Test]
        [TestCase(123, 1, "123")]
        [TestCase(123, 3, "123")]
        [TestCase(123, 4, "0123")]
        [TestCase(123, 5, "00123")]
        [TestCase(123, 6, "000123")]
        [TestCase(123, 7, "0000123")]
        [TestCase(123, 15, "000000000000123")]
        [TestCase(-123, 1, "-123")]
        [TestCase(-123, 3, "-123")]
        [TestCase(-123, 4, "-0123")]
        [TestCase(-123, 5, "-00123")]
        [TestCase(-123, 6, "-000123")]
        [TestCase(-123, 7, "-0000123")]
        [TestCase(-123, 15, "-000000000000123")]
        [TestCase(int.MinValue, 15, "-000002147483648")]
        [TestCase(int.MinValue, 10, "-2147483648")]
        [TestCase(int.MinValue, 3, "-2147483648")]
        public void TestLeftPad(int value, int length, string expected)
        {
            var builder = new StringBuilder();
            FormatHelper.LeftPad(value, length, builder);
            Assert.AreEqual(expected, builder.ToString());
        }
        
        [Test]
        [TestCase(123L, 1, "123")]
        [TestCase(123L, 3, "123")]
        [TestCase(123L, 4, "0123")]
        [TestCase(123L, 5, "00123")]
        [TestCase(123L, 6, "000123")]
        [TestCase(123L, 7, "0000123")]
        [TestCase(123L, 15, "000000000000123")]
        public void TestLeftPadNonNegativeInt64(long value, int length, string expected)
        {
            var builder = new StringBuilder();
            FormatHelper.LeftPadNonNegativeInt64(value, length, builder);
            Assert.AreEqual(expected, builder.ToString());
        }

        [Test]
        [TestCase(1, 3, 3, "001")]
        [TestCase(1200, 4, 5, "0120")]
        [TestCase(1, 2, 3, "00")]
        public void TestAppendFraction(int value, int length, int scale, string expected)
        {
            var builder = new StringBuilder();
            FormatHelper.AppendFraction(value, length, scale, builder);
            Assert.AreEqual(expected, builder.ToString());
        }
        
        [Test]
        [TestCase("x", 1, 3, 3, "x001")]
        [TestCase("x", 1200, 4, 5, "x012")]
        [TestCase("x", 1, 2, 3, "x")]
        [TestCase("1.", 1, 2, 3, "1")]
        public void TestAppendFractionTruncate(string initial, int value, int length, int scale, string expected)
        {
            var builder = new StringBuilder(initial);
            FormatHelper.AppendFractionTruncate(value, length, scale, builder);
            Assert.AreEqual(expected, builder.ToString());
        }

        [Test]
        [TestCase(0, "x0")]
        [TestCase(-1230, "x-1230")]
        [TestCase(1230, "x1230")]
        [TestCase(long.MinValue, "x-9223372036854775808")]
        public void FormatInvariant(long value, string expected)
        {
            var builder = new StringBuilder("x");
            FormatHelper.FormatInvariant(value, builder);
            Assert.AreEqual(expected, builder.ToString());
        }        
    }
}
