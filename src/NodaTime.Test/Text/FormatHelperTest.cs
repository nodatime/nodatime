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
using System.Text;
using NUnit.Framework;
using NodaTime.Text;

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
            FormatHelper.RightPadTruncate(1, 3, 3, ".", builder);
            Assert.AreEqual("001", builder.ToString());
        }

        [Test]
        public void TestRightPadTruncate_valueSmallerLengthSmaller()
        {
            var builder = new StringBuilder();
            FormatHelper.RightPadTruncate(1, 2, 3, ".", builder);
            Assert.AreEqual("", builder.ToString());
        }

        [Test]
        public void TestRightPadTruncate_valueSmallerRemoveDecimal()
        {
            var builder = new StringBuilder();
            builder.Append(".");
            FormatHelper.RightPadTruncate(1, 2, 3, ".", builder);
            Assert.AreEqual("", builder.ToString());
        }

        [Test]
        public void TestRightPadTruncate_valueSmallerScaleLarger()
        {
            var builder = new StringBuilder();
            Assert.Throws<FormatException>(() => FormatHelper.RightPadTruncate(1, 4, 3, ".", builder));
        }

        [Test]
        public void TestRightPadTruncate_scaleTooSmall()
        {
            var builder = new StringBuilder();
            Assert.Throws<FormatException>(() => FormatHelper.RightPadTruncate(1, 2, 0, ".", builder));
        }
    }
}
