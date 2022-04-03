// Copyright 2022 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

#if NET6_0_OR_GREATER
using NodaTime.Extensions;
using NUnit.Framework;
using System;

namespace NodaTime.Test.Extensions
{
    public class TimeOnlyExtensionsTest
    {
        [Test]
        public void ToLocalTime()
        {
            var timeOnly = new TimeOnly(12, 34, 56, 300).Add(TimeSpan.FromTicks(4567));
            var expected = new LocalTime(12, 34, 56, 300).PlusTicks(4567);
            var actual = timeOnly.ToLocalTime();
            Assert.AreEqual(expected, actual);
        }
    }
}
#endif
