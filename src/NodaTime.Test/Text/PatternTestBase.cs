// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NUnit.Framework;
using System;

namespace NodaTime.Test.Text
{
    /// <summary>
    /// Base class for all the pattern tests (when we've migrated OffsetPattern off FormattingTestSupport).
    /// Derived classes should have internal static fields with the names listed in the TestCaseSource
    /// attributes here: InvalidPatternData, ParseFailureData, ParseData, FormatData. Any field
    /// which is missing causes that test to be "not runnable" for that concrete subclass.
    /// If a test isn't appropriate (e.g. there's no configurable pattern) just provide a property with
    /// an array containing a null value - that will be ignored.
    /// </summary>
    public abstract class PatternTestBase<T>
    {
        [Test]
        [TestCaseSource("InvalidPatternData")]
        public void InvalidPatterns(PatternTestData<T> data)
        {
            data?.TestInvalidPattern();
        }

        [Test]
        [TestCaseSource("ParseFailureData")]
        public void ParseFailures(PatternTestData<T> data)
        {
            data?.TestParseFailure();
        }

        [Test]
        [TestCaseSource("ParseData")]
        public void Parse(PatternTestData<T> data)
        {
            data?.TestParse();
        }

        [Test]
        [TestCaseSource("FormatData")]
        public void Format(PatternTestData<T> data)
        {
            data?.TestFormat();
        }

        // Testing this for every item is somewhat overkill, but not too slow.
        [Test]
        [TestCaseSource("FormatData")]
        public void AppendFormat(PatternTestData<T> data)
        {
            data?.TestAppendFormat();
        }

        protected void AssertRoundTrip(T value, IPattern<T> pattern)
        {
            string text = pattern.Format(value);
            var parseResult = pattern.Parse(text);
            Assert.AreEqual(value, parseResult.Value);            
        }

        protected void AssertParseNull(IPattern<T> pattern)
        {
            var result = pattern.Parse(null);
            Assert.IsFalse(result.Success);
            Assert.IsInstanceOf<ArgumentNullException>(result.Exception);
        }
    }
}
