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
using NodaTime.Format;

namespace NodaTime.Test.Format
{
    [TestFixture]
    public class FormatHelperTest
    {
        private readonly ISignedValue positive = new PositiveSignedValue();
        private readonly ISignedValue negative = new NegativeSignedValue();

        private class PositiveSignedValue : ISignedValue
        {
            #region ISignedValue Members
            public bool IsNegative { get { return false; } }

            public string Sign { get { return "+"; } }
            #endregion
        }

        private class NegativeSignedValue : ISignedValue
        {
            #region ISignedValue Members
            public bool IsNegative { get { return true; } }

            public string Sign { get { return "-"; } }
            #endregion
        }

        [Test]
        public void TestFormatSign_requiredPositive()
        {
            var builder = new StringBuilder();
            FormatHelper.FormatSign(positive, true, builder);
            Assert.AreEqual("+", builder.ToString());
        }

        [Test]
        public void TestFormatSign_requiredNegative()
        {
            var builder = new StringBuilder();
            FormatHelper.FormatSign(negative, true, builder);
            Assert.AreEqual("-", builder.ToString());
        }

        [Test]
        public void TestFormatSign_notRequiredPositive()
        {
            var builder = new StringBuilder();
            FormatHelper.FormatSign(positive, false, builder);
            Assert.AreEqual("", builder.ToString());
        }

        [Test]
        public void TestFormatSign_notRequiredNegative()
        {
            var builder = new StringBuilder();
            FormatHelper.FormatSign(negative, false, builder);
            Assert.AreEqual("-", builder.ToString());
        }

        [Test]
        public void TestLeftPad_valueSmaller()
        {
            var builder = new StringBuilder();
            FormatHelper.LeftPad(1, 3, builder);
            Assert.AreEqual("001", builder.ToString());
        }

        [Test]
        public void TestLeftPad_valueSame()
        {
            var builder = new StringBuilder();
            FormatHelper.LeftPad(123, 3, builder);
            Assert.AreEqual("123", builder.ToString());
        }

        [Test]
        public void TestLeftPad_valueLarger()
        {
            var builder = new StringBuilder();
            FormatHelper.LeftPad(123456, 3, builder);
            Assert.AreEqual("123456", builder.ToString());
        }

        [Test]
        public void TestLeftPad_lengthTooLarge()
        {
            var builder = new StringBuilder();
            Assert.Throws<FormatException>(() => FormatHelper.LeftPad(123456, 3000, builder));
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