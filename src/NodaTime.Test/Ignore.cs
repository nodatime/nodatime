// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using System;

namespace NodaTime.Test
{
    /// <summary>
    /// Convenience class for ignoring tests based on conditions at execution time.
    /// </summary>
    public static class Ignore
    {
        public static void When(bool condition, string message)
        {
            if (condition)
            {
                Assert.Ignore(message);
            }
        }

        /// <summary>
        /// Calls Assert.Ignore, which always throws an exception - but with a return type
        /// of T, which makes it easier to call in expression-bodied members.
        /// </summary>
        public static T Throw<T>(string message)
        {
            Assert.Ignore(message);
            throw new InvalidOperationException($"Expected Assert.Ignore to throw");
        }
    }
}
