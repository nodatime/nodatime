// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class NanosecondsTest
    {
        [Test]
        [TestCase(long.MinValue)]
        [TestCase(long.MinValue + 1)]
        [TestCase(-NodaConstants.NanosecondsPerStandardDay - 1)]
        [TestCase(-NodaConstants.NanosecondsPerStandardDay)]
        [TestCase(-NodaConstants.NanosecondsPerStandardDay + 1)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(NodaConstants.NanosecondsPerStandardDay - 1)]
        [TestCase(NodaConstants.NanosecondsPerStandardDay)]
        [TestCase(NodaConstants.NanosecondsPerStandardDay + 1)]
        [TestCase(long.MaxValue - 1)]
        [TestCase(long.MaxValue)]
        public void Int64Conversions(long int64Nanos)
        {
            var nanoseconds = (Nanoseconds) int64Nanos;
            Assert.AreEqual(int64Nanos, (long) nanoseconds);
        }

        [Test]
        [TestCase(long.MinValue)]
        [TestCase(long.MinValue + 1)]
        [TestCase(-NodaConstants.NanosecondsPerStandardDay - 1)]
        [TestCase(-NodaConstants.NanosecondsPerStandardDay)]
        [TestCase(-NodaConstants.NanosecondsPerStandardDay + 1)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(NodaConstants.NanosecondsPerStandardDay - 1)]
        [TestCase(NodaConstants.NanosecondsPerStandardDay)]
        [TestCase(NodaConstants.NanosecondsPerStandardDay + 1)]
        [TestCase(long.MaxValue - 1)]
        [TestCase(long.MaxValue)]
        public void DecimalConversions(long int64Nanos)
        {
            decimal decimalNanos = int64Nanos;
            var nanoseconds = (Nanoseconds) decimalNanos;
            Assert.AreEqual(decimalNanos, (decimal) nanoseconds);

            // And multiply it by 100, which proves we still work for values out of the range of Int64
            decimalNanos *= 100;
            nanoseconds = (Nanoseconds) decimalNanos;
            Assert.AreEqual(decimalNanos, (decimal) nanoseconds);
        }

        [Test]
        [TestCase(long.MinValue)]
        [TestCase(long.MinValue + 1)]
        [TestCase(-NodaConstants.TicksPerStandardDay - 1)]
        [TestCase(-NodaConstants.TicksPerStandardDay)]
        [TestCase(-NodaConstants.TicksPerStandardDay + 1)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(NodaConstants.TicksPerStandardDay - 1)]
        [TestCase(NodaConstants.TicksPerStandardDay)]
        [TestCase(NodaConstants.TicksPerStandardDay + 1)]
        [TestCase(long.MaxValue - 1)]
        [TestCase(long.MaxValue)]
        public void TickConversions(long ticks)
        {
            var nanoseconds = Nanoseconds.FromTicks(ticks);
            Assert.AreEqual(ticks, nanoseconds.Ticks);
        }

        [Test]
        public void ConstituentParts_Positive()
        {
            var nanos = (Nanoseconds) (NodaConstants.NanosecondsPerStandardDay * 5 + 100);
            Assert.AreEqual(5, nanos.Days);
            Assert.AreEqual(100, nanos.NanosecondOfDay);
        }


        [Test]
        public void ConstituentParts_Negative()
        {
            var nanos = (Nanoseconds) (NodaConstants.NanosecondsPerStandardDay * -5 + 100);
            Assert.AreEqual(-5, nanos.Days);
            Assert.AreEqual(100, nanos.NanosecondOfDay);
        }

        [Test]
        public void ConstituentParts_Large()
        {
            // And outside the normal range of long...
            var nanos = (Nanoseconds) (NodaConstants.NanosecondsPerStandardDay * 365000m + 500m);
            Assert.AreEqual(365000, nanos.Days);
            Assert.AreEqual(500, nanos.NanosecondOfDay);
        }

        // TODO:
        // - Comparisons and comparison operators
        // - Equality
        // - + and -
    }
}
