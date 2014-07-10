// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using NodaTime.Utility;
using NUnit.Framework;

namespace NodaTime.Test.Utility
{
    [TestFixture]
    public class PreconditionsTest
    {
#if DEBUG        
        [Test]
        public void DebugCheckArgumentRange_Debug()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Preconditions.DebugCheckArgumentRange("ignore", 10, 0, 5));
            Assert.Throws<ArgumentOutOfRangeException>(() => Preconditions.DebugCheckArgumentRange("ignore", 10L, 0L, 5L));
        }
#else
        [Test]
        public void DebugCheckArgumentRange_NotDebug()
        {
            Preconditions.DebugCheckArgumentRange("ignore", 10, 0, 5);
            Preconditions.DebugCheckArgumentRange("ignore", 10L, 0L, 5L);
        }
#endif
    }
}
