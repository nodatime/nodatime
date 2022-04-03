// Copyright 2022 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using System.Linq;
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalTimeTest
    {

#if NET6_0_OR_GREATER
        [Test]
        public void ToTimeOnly_OnTickBoundary()
        {
            var localTime = new LocalTime(12, 34, 56, 300).PlusTicks(4567);
            var expected = new TimeOnly(12, 34, 56, 300).Add(TimeSpan.FromTicks(4567));
            var actual = localTime.ToTimeOnly();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToTimeOnly_RoundsDown()
        {
            var localTime = new LocalTime(12, 34, 56, 300).PlusTicks(4567).PlusNanoseconds(89);
            var expected = new TimeOnly(12, 34, 56, 300).Add(TimeSpan.FromTicks(4567));
            var actual = localTime.ToTimeOnly();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromTimeOnly()
        {
            var timeOnly = new TimeOnly(12, 34, 56, 300).Add(TimeSpan.FromTicks(4567));
            var expected = new LocalTime(12, 34, 56, 300).PlusTicks(4567);
            var actual = LocalTime.FromTimeOnly(timeOnly);
            Assert.AreEqual(expected, actual);
        }
#endif
    }
}
