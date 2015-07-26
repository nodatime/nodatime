// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using NodaTime.TzdbCompiler.Tzdb;
using NUnit.Framework;
using System.Linq;

namespace NodaTime.TzdbCompiler.Test.Tzdb
{
    [TestFixture]
    public class ZoneRuleTest
    {
        [Test]
        public void WriteRead()
        {
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, yearOffset, 1971, 2009);
            var actual = new ZoneRule(recurrence, "D", null);
            var expected = new ZoneRule(recurrence, "D", null);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatName_PercentZ()
        {
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int) IsoDayOfWeek.Wednesday, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("Rule", Offset.FromHoursAndMinutes(5, 30), yearOffset, 1971, 2009);
            var rule = new ZoneRule(recurrence, "D", null);
            
            var zoneRecurrence = rule.GetRecurrences(GetZone("X%zY", Offset.Zero)).Single();
            Assert.AreEqual("X+0530Y", zoneRecurrence.Name);

            zoneRecurrence = rule.GetRecurrences(GetZone("X%zY", Offset.FromHoursAndMinutes(0, 30))).Single();
            Assert.AreEqual("X+06Y", zoneRecurrence.Name);

            zoneRecurrence = rule.GetRecurrences(GetZone("X%zY", Offset.FromHoursAndMinutes(-6, -30))).Single();
            Assert.AreEqual("X-01Y", zoneRecurrence.Name);
        }

        [Test]
        public void FormatName_PercentS()
        {
            // Note that the offset is irrelevant here - the "daylight saving indicator" is replaced either way.
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int) IsoDayOfWeek.Wednesday, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("ignored", Offset.Zero, yearOffset, 1971, 2009);
            var rule = new ZoneRule(recurrence, "!", null);
            var zone = GetZone("X%sY", Offset.Zero);

            var zoneRecurrence = rule.GetRecurrences(zone).Single();
            Assert.AreEqual("X!Y", zoneRecurrence.Name);
        }

        [Test]
        public void FormatName_Slash_Standard()
        {
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int) IsoDayOfWeek.Wednesday, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("ignored", Offset.Zero, yearOffset, 1971, 2009);
            var rule = new ZoneRule(recurrence, "!", null);
            var zone = GetZone("X/Y", Offset.Zero);

            var zoneRecurrence = rule.GetRecurrences(zone).Single();
            Assert.AreEqual("X", zoneRecurrence.Name);
        }

        [Test]
        public void FormatName_Slash_Daylight()
        {
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int) IsoDayOfWeek.Wednesday, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("ignored", Offset.FromHours(1), yearOffset, 1971, 2009);
            var rule = new ZoneRule(recurrence, "!", null);
            var zone = GetZone("X/Y", Offset.FromHours(1));

            var zoneRecurrence = rule.GetRecurrences(zone).Single();
            Assert.AreEqual("Y", zoneRecurrence.Name);
        }

        private static Zone GetZone(string nameFormat, Offset standardOffset)
        {
            return new Zone("Zone", standardOffset, "Rule", nameFormat, 2000, ZoneYearOffset.StartOfYear);
        }
    }
}
