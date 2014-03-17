// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Text;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Text
{
    [TestFixture]
    public class FormatHelperTest
    {
        [Test]
        public void TestLeftPad_valueSmaller()
        {
            AssertLeftPad(123, 5, "00123");
            AssertLeftPad(123, 3, "123");
            AssertLeftPad(123, 1, "123");
        }

        [Test]
        public void TestLeftPad_lengthTooLarge()
        {
            var builder = new StringBuilder();
            Assert.Throws<FormatException>(() => FormatHelper.LeftPad(123456, 3000, builder));
        }

        [Test]
        public void TestLeftPad_Negative()
        {
            AssertLeftPad(-123, 5, "-00123");
            AssertLeftPad(-123, 3, "-123");
            AssertLeftPad(-123, 1, "-123");
        }

        [Test]
        public void TestLeftPad_MinValue()
        {
            AssertLeftPad(int.MinValue, 15, "-000002147483648");
            AssertLeftPad(int.MinValue, 10, "-2147483648");
            AssertLeftPad(int.MinValue, 3, "-2147483648");
        }

        private static void AssertLeftPad(int value, int length, string expected)
        {
            var builder = new StringBuilder();
            FormatHelper.LeftPad(value, length, builder);
            Assert.AreEqual(expected, builder.ToString());
        }

        [Test]
        public void TestRightPad_valueSmaller()
        {
            var builder = new StringBuilder();
            FormatHelper.RightPad(1, 3, 3, builder);
            Assert.AreEqual("001", builder.ToString());
        }

        [Test]
        public void TestRightPad_valueSmallerLengthSmaller()
        {
            var builder = new StringBuilder();
            FormatHelper.RightPad(1, 2, 3, builder);
            Assert.AreEqual("00", builder.ToString());
        }

        [Test]
        public void TestRightPad_valueSmallerScaleLarger()
        {
            var builder = new StringBuilder();
            Assert.Throws<FormatException>(() => FormatHelper.RightPad(1, 4, 3, builder));
        }

        [Test]
        public void TestRightPad_scaleTooSmall()
        {
            var builder = new StringBuilder();
            Assert.Throws<FormatException>(() => FormatHelper.RightPad(1, 2, 0, builder));
        }

        [Test]
        public void TestRightPadTruncate_valueSmaller()
        {
            var builder = new StringBuilder();
            FormatHelper.RightPadTruncate(1, 3, 3, builder);
            Assert.AreEqual("001", builder.ToString());
        }

        [Test]
        public void TestRightPadTruncate_valueSmallerLengthSmaller()
        {
            var builder = new StringBuilder();
            FormatHelper.RightPadTruncate(1, 2, 3, builder);
            Assert.AreEqual("", builder.ToString());
        }

        [Test]
        public void TestRightPadTruncate_valueSmallerRemoveDecimal()
        {
            var builder = new StringBuilder();
            builder.Append(".");
            FormatHelper.RightPadTruncate(1, 2, 3, builder);
            Assert.AreEqual("", builder.ToString());
        }

        [Test]
        public void TestRightPadTruncate_valueSmallerScaleLarger()
        {
            var builder = new StringBuilder();
            Assert.Throws<FormatException>(() => FormatHelper.RightPadTruncate(1, 4, 3, builder));
        }

        [Test]
        public void TestRightPadTruncate_scaleTooSmall()
        {
            var builder = new StringBuilder();
            Assert.Throws<FormatException>(() => FormatHelper.RightPadTruncate(1, 2, 0, builder));
        }

        [Test]
        public void FormatInvariant_Zero()
        {
            var builder = new StringBuilder("x");
            FormatHelper.FormatInvariant(0, builder);
            Assert.AreEqual("x0", builder.ToString());
        }

        [Test]
        public void FormatInvariant_Negative()
        {
            var builder = new StringBuilder("x");
            FormatHelper.FormatInvariant(-1230, builder);
            Assert.AreEqual("x-1230", builder.ToString());
        }

        [Test]
        public void FormatInvariant_Positive()
        {
            var builder = new StringBuilder("x");
            FormatHelper.FormatInvariant(1230, builder);
            Assert.AreEqual("x1230", builder.ToString());
        }

        [Test]
        public void FormatInvariant_MinValue()
        {
            var builder = new StringBuilder("x");
            FormatHelper.FormatInvariant(long.MinValue, builder);
            Assert.AreEqual("x-9223372036854775808", builder.ToString());
        }
    }
}
