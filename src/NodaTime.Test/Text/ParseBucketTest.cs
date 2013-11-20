// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NodaTime.Text.Patterns;
using NUnit.Framework;

namespace NodaTime.Test.Text
{
    [TestFixture]
    public class ParseBucketTest
    {
        [Test]
        public void IsUsed_NoMatch()
        {
            Assert.IsFalse(ParseBucket<Offset>.IsFieldUsed(PatternFields.Hours12 | PatternFields.Minutes, PatternFields.Hours24));
        }

        [Test]
        public void IsUsed_SingleValueMatch()
        {
            Assert.IsTrue(ParseBucket<Offset>.IsFieldUsed(PatternFields.Hours24, PatternFields.Hours24));
        }

        [Test]
        public void IsFieldUsed_MultiValueMatch()
        {
            Assert.IsTrue(ParseBucket<Offset>.IsFieldUsed(PatternFields.Hours24 | PatternFields.Minutes, PatternFields.Hours24));
        }

        [Test]
        public void AllAreUsed_NoMatch()
        {
            Assert.IsFalse(ParseBucket<Offset>.AreAllFieldsUsed(PatternFields.Hours12 | PatternFields.Minutes,
                                                                PatternFields.Hours24 | PatternFields.Seconds));
        }

        [Test]
        public void AllAreUsed_PartialMatch()
        {
            Assert.IsFalse(ParseBucket<Offset>.AreAllFieldsUsed(PatternFields.Hours12 | PatternFields.Minutes,
                                                                PatternFields.Hours12 | PatternFields.Seconds));
        }

        [Test]
        public void AllAreUsed_CompleteMatch()
        {
            Assert.IsTrue(ParseBucket<Offset>.AreAllFieldsUsed(PatternFields.Hours12 | PatternFields.Minutes,
                                                               PatternFields.Hours12 | PatternFields.Minutes));
        }

        [Test]
        public void AllAreUsed_CompleteMatchWithMore()
        {
            Assert.IsTrue(ParseBucket<Offset>.IsFieldUsed(PatternFields.Hours24 | PatternFields.Minutes | PatternFields.Hours12,
                                                          PatternFields.Hours24 | PatternFields.Minutes));
        }
    }
}
