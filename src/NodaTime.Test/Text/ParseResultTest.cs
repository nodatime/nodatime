// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Text
{
    [TestFixture]
    public class ParseResultTest
    {
        private static readonly ParseResult<int> FailureResult = ParseResult<int>.ForInvalidValue(new ValueCursor("text"), "text");

        [Test]
        public void Value_Success()
        {
            ParseResult<int> result = ParseResult<int>.ForValue(5);
            Assert.AreEqual(5, result.Value);
        }

        [Test]
        public void Value_Failure()
        {
            Assert.Throws<UnparsableValueException>(() => FailureResult.Value.GetHashCode());
        }

        [Test]
        public void Exception_Success()
        {
            ParseResult<int> result = ParseResult<int>.ForValue(5);
            Assert.Throws<InvalidOperationException>(() => result.Exception.GetHashCode());
        }

        [Test]
        public void Exception_Failure()
        {
            Assert.IsInstanceOf<UnparsableValueException>(FailureResult.Exception);
        }

        [Test]
        public void GetValueOrThrow_Success()
        {
            ParseResult<int> result = ParseResult<int>.ForValue(5);
            Assert.AreEqual(5, result.GetValueOrThrow());
        }

        [Test]
        public void GetValueOrThrow_Failure()
        {
            Assert.Throws<UnparsableValueException>(() => FailureResult.GetValueOrThrow());
        }

        [Test]
        public void TryGetValue_Success()
        {
            ParseResult<int> result = ParseResult<int>.ForValue(5);
            int actual;
            Assert.IsTrue(result.TryGetValue(-1, out actual));
            Assert.AreEqual(5, actual);
        }

        [Test]
        public void TryGetValue_Failure()
        {
            int actual;
            Assert.IsFalse(FailureResult.TryGetValue(-1, out actual));
            Assert.AreEqual(-1, actual);
        }

        [Test]
        public void Convert_ForFailureResult()
        {
            ParseResult<string> converted = FailureResult.Convert(x => "xx" + x + "xx");
            Assert.Throws<UnparsableValueException>(() => converted.GetValueOrThrow());
        }

        [Test]
        public void Convert_ForSuccessResult()
        {
            ParseResult<int> original = ParseResult<int>.ForValue(10);
            ParseResult<string> converted = original.Convert(x => "xx" + x + "xx");
            Assert.AreEqual("xx10xx", converted.Value);
        }

        [Test]
        public void ConvertError_ForFailureResult()
        {
            ParseResult<string> converted = FailureResult.ConvertError<string>();
            Assert.Throws<UnparsableValueException>(() => converted.GetValueOrThrow());
        }

        [Test]
        public void ConvertError_ForSuccessResult()
        {
            ParseResult<int> original = ParseResult<int>.ForValue(10);
            Assert.Throws<InvalidOperationException>(() => original.ConvertError<string>());            
        }
    }
}
