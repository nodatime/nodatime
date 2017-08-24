// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;
using NUnit.Framework;
using NodaTime.Extensions;

namespace NodaTime.Test.Utility
{
    public class BclConversionsTest
    {
        // This tests both directions for all valid values.
        // Alternatively, we could have checked names...
        // it doesn't matter much.
        [TestCase(DayOfWeek.Sunday, IsoDayOfWeek.Sunday)]
        [TestCase(DayOfWeek.Monday, IsoDayOfWeek.Monday)]
        [TestCase(DayOfWeek.Tuesday, IsoDayOfWeek.Tuesday)]
        [TestCase(DayOfWeek.Wednesday, IsoDayOfWeek.Wednesday)]
        [TestCase(DayOfWeek.Thursday, IsoDayOfWeek.Thursday)]
        [TestCase(DayOfWeek.Friday, IsoDayOfWeek.Friday)]
        [TestCase(DayOfWeek.Saturday, IsoDayOfWeek.Saturday)]
        public void DayOfWeek_BothWaysValid(DayOfWeek bcl, IsoDayOfWeek noda)
        {
            Assert.AreEqual(bcl, BclConversions.ToDayOfWeek(noda));
            Assert.AreEqual(noda, BclConversions.ToIsoDayOfWeek(bcl));
            Assert.AreEqual(bcl, noda.ToDayOfWeek());
#pragma warning disable CS0618 // Type or member is obsolete
            Assert.AreEqual(bcl, noda.ToIsoDayOfWeek());
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.AreEqual(noda, bcl.ToIsoDayOfWeek());
        }

        [TestCase(0)]
        [TestCase(8)]
        public void ToDayOfWeek_InvalidValues(IsoDayOfWeek noda)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => BclConversions.ToDayOfWeek(noda));
            Assert.Throws<ArgumentOutOfRangeException>(() => noda.ToDayOfWeek());
#pragma warning disable CS0618 // Type or member is obsolete
            Assert.Throws<ArgumentOutOfRangeException>(() => noda.ToIsoDayOfWeek());
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [TestCase(-1)]
        [TestCase(7)]
        public void ToIsoDayOfWeek_InvalidValues(DayOfWeek bcl)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => BclConversions.ToIsoDayOfWeek(bcl));
            Assert.Throws<ArgumentOutOfRangeException>(() => bcl.ToIsoDayOfWeek());
        }
    }
}
