﻿// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NUnit.Framework;
using System;

namespace NodaTime.Test
{
    /// <summary>
    /// Tests for exceptions - we don't typically call the serialization code anywhere else, for example.
    /// </summary>
    public class ExceptionConstructorTests
    {
        // We never actually use this constructor, but it's in the public API and it's harmless...
        [Test]
        public void InvalidPatternExceptionParameterlessConstructor()
        {
            var exception = new InvalidPatternException();
            Assert.AreEqual(new FormatException().Message, exception.Message);
        }

        // We never actually use these constructors, but they're in the public API and we don't want to create
        // a breaking change. They're obsolete to discourage external use though, as we want an UnparsableValueException
        // to always have a value.
        [Test, Obsolete]
        public void UnparsableValueObsoleteConstructors()
        {
            var exceptions = new[]
            {
                new UnparsableValueException(),
                new UnparsableValueException("message"),
                new UnparsableValueException("message", new Exception())
            };

            foreach (var exception in exceptions)
            {
                Assert.IsEmpty(exception.Value);
                Assert.AreEqual(-1, exception.Index);
            }
        }
    }
}
