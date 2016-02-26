// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text.Patterns;
using NUnit.Framework;
using static NodaTime.Text.Patterns.PatternFields;

namespace NodaTime.Test.Text
{
    public class PatternFieldsExtensionsTest
    {
        [Test]
        public void IsUsed_NoMatch()
        {
            Assert.IsFalse((Hours12 | Minutes).HasAny(Hours24));
        }

        [Test]
        public void IsUsed_SingleValueMatch()
        {
            Assert.IsTrue(Hours24.HasAny(Hours24));
        }

        [Test]
        public void IsFieldUsed_MultiValueMatch()
        {
            Assert.IsTrue((Hours24 | Minutes).HasAny(Hours24));
        }

        [Test]
        public void AllAreUsed_NoMatch()
        {
            Assert.IsFalse((Hours12 | Minutes).HasAll(Hours24 | Seconds));
        }

        [Test]
        public void AllAreUsed_PartialMatch()
        {
            Assert.IsFalse((Hours12 | Minutes).HasAll(Hours12 | Seconds));
        }

        [Test]
        public void AllAreUsed_CompleteMatch()
        {
            Assert.IsTrue((Hours12 | Minutes).HasAll(Hours12 | Minutes));
        }

        [Test]
        public void AllAreUsed_CompleteMatchWithMore()
        {
            Assert.IsTrue((Hours24 | Minutes | Hours12).HasAll(Hours24 | Minutes));
        }
    }
}
