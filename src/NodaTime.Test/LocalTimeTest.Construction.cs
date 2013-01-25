// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalTimeTest
    {
        [Test]
        public void FromTicksSinceMidnight_Valid()
        {
            Assert.AreEqual(LocalTime.Midnight, LocalTime.FromTicksSinceMidnight(0));
            Assert.AreEqual(LocalTime.Midnight - Period.FromTicks(1), LocalTime.FromTicksSinceMidnight(NodaConstants.TicksPerStandardDay - 1));
        }

        [Test]
        public void FromTicksSinceMidnight_RangeChecks()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromTicksSinceMidnight(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromTicksSinceMidnight(NodaConstants.TicksPerStandardDay));
        }

        [Test]
        public void FromMillisecondsSinceMidnight_Valid()
        {
            Assert.AreEqual(LocalTime.Midnight, LocalTime.FromMillisecondsSinceMidnight(0));
            Assert.AreEqual(LocalTime.Midnight - Period.FromMilliseconds(1), LocalTime.FromMillisecondsSinceMidnight(NodaConstants.MillisecondsPerStandardDay - 1));
        }

        [Test]
        public void FromMillisecondsSinceMidnight_RangeChecks()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromMillisecondsSinceMidnight(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromMillisecondsSinceMidnight(NodaConstants.MillisecondsPerStandardDay));
        }

        [Test]
        public void FromSecondsSinceMidnight_Valid()
        {
            Assert.AreEqual(LocalTime.Midnight, LocalTime.FromSecondsSinceMidnight(0));
            Assert.AreEqual(LocalTime.Midnight - Period.FromSeconds(1), LocalTime.FromSecondsSinceMidnight(NodaConstants.SecondsPerStandardDay - 1));
        }

        [Test]
        public void FromSecondsSinceMidnight_RangeChecks()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromSecondsSinceMidnight(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromSecondsSinceMidnight(NodaConstants.SecondsPerStandardDay));
        }
    }
}
