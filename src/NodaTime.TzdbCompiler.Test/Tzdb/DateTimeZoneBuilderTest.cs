// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using NodaTime.TimeZones;
using NodaTime.TzdbCompiler.Tzdb;
using System.Collections.Generic;

namespace NodaTime.TzdbCompiler.Test.Tzdb
{
    /// <summary>
    /// Tests for DateTimeZoneBuilder; currently only scant coverage based on bugs which have
    /// previously been found.
    /// </summary>
    public class DateTimeZoneBuilderTest
    {
        [Test]
        public void FixedZone_Western()
        {
            var offset = Offset.FromHours(-5);
            var rules = new List<ZoneRuleSet> { new ZoneRuleSet("GMT+5", offset, Offset.Zero, int.MaxValue, null) };
            var zone = DateTimeZoneBuilder.Build("GMT+5", rules);
            FixedDateTimeZone fixedZone = (FixedDateTimeZone)zone;
            Assert.AreEqual(offset, fixedZone.Offset);
        }

        [Test]
        public void FixedZone_Eastern()
        {
            var offset = Offset.FromHours(5);
            var rules = new List<ZoneRuleSet> { new ZoneRuleSet("GMT-5", offset, Offset.Zero, int.MaxValue, null) };
            var zone = DateTimeZoneBuilder.Build("GMT-5", rules);
            FixedDateTimeZone fixedZone = (FixedDateTimeZone)zone;
            Assert.AreEqual(offset, fixedZone.Offset);
        }
    }
}
