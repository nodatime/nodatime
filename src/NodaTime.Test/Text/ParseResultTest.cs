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
        [Test]
        public void Value_Success()
        {
            ParseResult<int> result = ParseResult<int>.ForValue(5);
            Assert.AreEqual(5, result.Value);
        }

        [Test]
        public void Value_Failure()
        {
            ParseResult<int> result = ParseResult<int>.ForInvalidValue("text");
            Assert.Throws<UnparsableValueException>(() => result.Value.GetHashCode());
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
            ParseResult<int> result = ParseResult<int>.ForInvalidValue("text");
            Assert.IsInstanceOf<UnparsableValueException>(result.Exception);
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
            ParseResult<int> result = ParseResult<int>.ForInvalidValue("text");
            Assert.Throws<UnparsableValueException>(() => result.GetValueOrThrow());
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
            ParseResult<int> result = ParseResult<int>.ForInvalidValue("text");
            int actual;
            Assert.IsFalse(result.TryGetValue(-1, out actual));
            Assert.AreEqual(-1, actual);
        }

        [Test]
        public void Convert_ForFailureResult()
        {
            ParseResult<int> original = ParseResult<int>.ForInvalidValue("text");
            ParseResult<string> converted = original.Convert(x => "xx" + x + "xx");
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
            ParseResult<int> original = ParseResult<int>.ForInvalidValue("text");
            ParseResult<string> converted = original.ConvertError<string>();
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
