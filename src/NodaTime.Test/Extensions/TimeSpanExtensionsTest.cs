// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Extensions;
using NUnit.Framework;
using System;

namespace NodaTime.Test.Extensions
{
    public class TimeSpanExtensionsTest
    {
        [Test]
        public void ToDuration()
        {
            var timeSpan = TimeSpan.FromTicks(123456789012345L);
            var duration = timeSpan.ToDuration();
            Assert.AreEqual(123456789012345, duration.BclCompatibleTicks);
        }
    }
}
