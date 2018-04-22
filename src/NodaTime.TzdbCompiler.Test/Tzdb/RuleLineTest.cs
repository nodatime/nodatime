// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using NodaTime.TzdbCompiler.Tzdb;
using NUnit.Framework;

namespace NodaTime.TzdbCompiler.Test.Tzdb
{
    public class RuleLineTest
    {
        [Test]
        public void WriteRead()
        {
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, yearOffset, 1971, 2009);
            var actual = new RuleLine(recurrence, "D", null);
            var expected = new RuleLine(recurrence, "D", null);
            Assert.AreEqual(expected, actual);
        }
    }
}
