// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class TzdbDateTimeZoneSourceTest
    {
        [Test]
        [TestCase("UTC", "Etc/GMT")]
        [TestCase("GMT Standard Time", "Europe/London")]
        // Standard name differs from ID under Windows
        [TestCase("Israel Standard Time", "Asia/Jerusalem")]
        public void ZoneMapping(string bclId, string tzdbId)
        {
            try
            {
                var source = TzdbDateTimeZoneSource.Default;
                var bclZone = TimeZoneInfo.FindSystemTimeZoneById(bclId);
                Assert.AreEqual(tzdbId, source.MapTimeZoneId(bclZone));
            }
            catch (TimeZoneNotFoundException)
            {
                // This may occur on Mono, for example.
                Assert.Ignore("Test assumes existence of BCL zone with ID: " + bclId);
            }
        }

        /// <summary>
        /// Tests that we can load (and exercise) the binary Tzdb resource file distributed with Noda Time 1.1.0.
        /// This is effectively a black-box regression test that ensures that the stream format has not changed in a
        /// way such that a custom tzdb compiled with ZoneInfoCompiler from 1.1 would become unreadable.
        /// </summary>
        [Test]
        public void CanLoadNodaTimeResourceFromOnePointOneRelease()
        {
            var assembly = typeof(TzdbDateTimeZoneSourceTest).Assembly;
            TzdbDateTimeZoneSource source;
            using (Stream stream = assembly.GetManifestResourceStream("NodaTime.Test.TestData.Tzdb2013bFromNodaTime1.1.nzd"))
            {
                source = TzdbDateTimeZoneSource.FromStream(stream);
            }
            Assert.AreEqual("TZDB: 2013b (mapping: 8274)", source.VersionId);

            var utc = Instant.FromUtc(2007, 8, 24, 9, 30, 0);

            // Test a regular zone with rules.
            var london = source.ForId("Europe/London");
            var inLondon = new ZonedDateTime(utc, london);
            var expectedLocal = new LocalDateTime(2007, 8, 24, 10, 30);
            Assert.AreEqual(expectedLocal, inLondon.LocalDateTime);

            // Test a fixed-offset zone.
            var utcFixed = source.ForId("Etc/UTC");
            var inUtcFixed = new ZonedDateTime(utc, utcFixed);
            expectedLocal = new LocalDateTime(2007, 8, 24, 9, 30);
            Assert.AreEqual(expectedLocal, inUtcFixed.LocalDateTime);

            // Test an alias.
            var jersey = source.ForId("Japan");  // Asia/Tokyo
            var inJersey = new ZonedDateTime(utc, jersey);
            expectedLocal = new LocalDateTime(2007, 8, 24, 18, 30);
            Assert.AreEqual(expectedLocal, inJersey.LocalDateTime);

            // Test ZoneLocations.
            var france = source.ZoneLocations.Single(g => g.CountryName == "France");
            // Tolerance of about 2 seconds
            Assert.AreEqual(48.86666, france.Latitude, 0.00055);
            Assert.AreEqual(2.3333, france.Longitude, 0.00055);
            Assert.AreEqual("Europe/Paris", france.ZoneId);
            Assert.AreEqual("FR", france.CountryCode);
            Assert.AreEqual("", france.Comment);
        }

#if !PCL
        /// <summary>
        /// Tests that we can load (and exercise) the binary Tzdb resource file distributed with Noda Time 1.0.0.
        /// This is effectively a black-box regression test that ensures that the resource format has not changed in a
        /// way such that a custom resource file compiled with ZoneInfoCompiler from 1.0 would become unreadable.
        /// </summary>
        [Test]
        public void CanLoadNodaTimeResourceFromOnePointZeroRelease()
        {
#pragma warning disable 0618
            var source = new TzdbDateTimeZoneSource("NodaTime.Test.TestData.Tzdb2012iFromNodaTime1.0",
                Assembly.GetExecutingAssembly());
#pragma warning restore 0618
            Assert.AreEqual("TZDB: 2012i (mapping: 6356)", source.VersionId);

            var utc = Instant.FromUtc(2007, 8, 24, 9, 30, 0);

            // Test a regular zone with rules.
            var london = source.ForId("Europe/London");
            var inLondon = new ZonedDateTime(utc, london);
            var expectedLocal = new LocalDateTime(2007, 8, 24, 10, 30);
            Assert.AreEqual(expectedLocal, inLondon.LocalDateTime);

            // Test a fixed-offset zone.
            var utcFixed = source.ForId("Etc/UTC");
            var inUtcFixed = new ZonedDateTime(utc, utcFixed);
            expectedLocal = new LocalDateTime(2007, 8, 24, 9, 30);
            Assert.AreEqual(expectedLocal, inUtcFixed.LocalDateTime);

            // Test an alias.
            var jersey = source.ForId("Japan");  // Asia/Tokyo
            var inJersey = new ZonedDateTime(utc, jersey);
            expectedLocal = new LocalDateTime(2007, 8, 24, 18, 30);
            Assert.AreEqual(expectedLocal, inJersey.LocalDateTime);

            // Can't ask for ZoneLocations
            Assert.Throws<InvalidOperationException>(() => source.ZoneLocations.GetHashCode());
        }

        /// <summary>
        /// Make sure we can still load the latest version of TZDB as a resource file, and
        /// that the zones are equivalent to the stream version. (This requires that we
        /// replace both files each time we rebuild, of course.)
        /// </summary>
        [Test]
        public void ResourceZoneEquivalence()
        {
            var streamSource = TzdbDateTimeZoneSource.Default;
#pragma warning disable 0618
            var resourceSource = new TzdbDateTimeZoneSource("NodaTime.Test.TestData.Tzdb",
                Assembly.GetExecutingAssembly());
#pragma warning restore 0618
            Assert.AreEqual(streamSource.VersionId, resourceSource.VersionId);
            CollectionAssert.AreEquivalent(streamSource.GetIds(), resourceSource.GetIds());

            var interval = new Interval(Instant.FromUtc(1850, 1, 1, 0, 0), Instant.FromUtc(2050, 1, 1, 0, 0));
            var comparer = ZoneEqualityComparer.ForInterval(interval).WithOptions(ZoneEqualityComparer.Options.StrictestMatch);
            foreach (var id in streamSource.GetIds())
            {
                Assert.IsTrue(comparer.Equals(streamSource.ForId(id), resourceSource.ForId(id)),
                    "Zone {0} is equal under stream and resource formats", id);
            }
        }
#endif

        /// <summary>
        /// Simply tests that every ID in the built-in database can be fetched. This is also
        /// helpful for diagnostic debugging when we want to check that some potential
        /// invariant holds for all time zones...
        /// </summary>
        [Test]
        public void ForId_AllIds()
        {
            var source = TzdbDateTimeZoneSource.Default;
            foreach (string id in source.GetIds())
            {
                Assert.IsNotNull(source.ForId(id));
            }
        }

        [Test]
        public void ForId_Null()
        {
            Assert.Throws<ArgumentNullException>(() => TzdbDateTimeZoneSource.Default.ForId(null));
        }

        [Test]
        public void ForId_Unknown()
        {
            Assert.Throws<ArgumentException>(() => TzdbDateTimeZoneSource.Default.ForId("unknown"));
        }

        [Test]
        public void UtcEqualsBuiltIn()
        {
            var zone = TzdbDateTimeZoneSource.Default.ForId("UTC");
            Assert.AreEqual(DateTimeZone.Utc, zone);
        }

        // The following tests all make assumptions about the built-in TZDB data.
        // This is simpler than constructing fake data, and validates that the creation
        // mechanism matches the reading mechanism, too.
        [Test]
        public void Aliases()
        {
            var aliases = TzdbDateTimeZoneSource.Default.Aliases;
            CollectionAssert.AreEqual(new[] { "Europe/Belfast", "Europe/Guernsey", "Europe/Isle_of_Man", "Europe/Jersey", "GB", "GB-Eire" },
                                      aliases["Europe/London"].ToArray()); // ToArray call makes diagnostics more useful
            CollectionAssert.IsOrdered(aliases["Europe/London"]);
            CollectionAssert.IsEmpty(aliases["Europe/Jersey"]);
        }

        [Test]
        public void CanonicalIdMap_Contents()
        {
            var map = TzdbDateTimeZoneSource.Default.CanonicalIdMap;
            Assert.AreEqual("Europe/London", map["Europe/Jersey"]);
            Assert.AreEqual("Europe/London", map["Europe/London"]);
        }

        [Test]
        public void CanonicalIdMap_IsReadOnly()
        {
            var map = TzdbDateTimeZoneSource.Default.CanonicalIdMap;
            Assert.Throws<NotSupportedException>(() => map.Add("Foo", "Bar"));
        }

        // Sample zone location checks to ensure we've serialized and deserialized correctly
        // Input line: FR	+4852+00220	Europe/Paris
        [Test]
        public void ZoneLocations_ContainsFrance()
        {
            var zoneLocations = TzdbDateTimeZoneSource.Default.ZoneLocations;
            var france = zoneLocations.Single(g => g.CountryName == "France");
            // Tolerance of about 2 seconds
            Assert.AreEqual(48.86666, france.Latitude, 0.00055);
            Assert.AreEqual(2.3333, france.Longitude, 0.00055);
            Assert.AreEqual("Europe/Paris", france.ZoneId);
            Assert.AreEqual("FR", france.CountryCode);
            Assert.AreEqual("", france.Comment);
        }

        // Input line: CA	+744144-0944945	America/Resolute	Central Time - Resolute, Nunavut
        // (Note: prior to 2014f, this was "Central Standard Time - Resolute, Nunavut".)
        [Test]
        public void ZoneLocations_ContainsResolute()
        {
            var zoneLocations = TzdbDateTimeZoneSource.Default.ZoneLocations;
            var resolute = zoneLocations.Single(g => g.ZoneId == "America/Resolute");
            // Tolerance of about 2 seconds
            Assert.AreEqual(74.69555, resolute.Latitude, 0.00055);
            Assert.AreEqual(-94.82916, resolute.Longitude, 0.00055);
            Assert.AreEqual("Canada", resolute.CountryName);
            Assert.AreEqual("CA", resolute.CountryCode);
            Assert.AreEqual("Central Time - Resolute, Nunavut", resolute.Comment);
        }

        [Test]
        public void TzdbVersion()
        {
            var source = TzdbDateTimeZoneSource.Default;
            StringAssert.StartsWith("201", source.TzdbVersion);
        }

        [Test]
        public void VersionId()
        {
            var source = TzdbDateTimeZoneSource.Default;
            StringAssert.StartsWith("TZDB: " + source.TzdbVersion, source.VersionId);
        }

        [Test]
        [TestCaseSource(typeof(TimeZoneInfo), "GetSystemTimeZones")]
        public void GuessZoneIdByTransitionsUncached(TimeZoneInfo bclZone)
        {
            // As of October 17th 2013, the Windows time zone database hasn't noticed that
            // Morocco delayed the DST transition in 2013, so we end up with UTC. It's
            // annoying, but it's not actually a code issue. Just ignore it for now. We
            // should check this periodically and remove the hack when it works again.
            // Likewise Libya has somewhat odd representation in the BCL. Worth looking at more closely later.
            if (bclZone.Id == "Morocco Standard Time" || bclZone.Id == "Libya Standard Time")
            {
                return;
            }
            string id = TzdbDateTimeZoneSource.Default.GuessZoneIdByTransitionsUncached(bclZone);

            // Unmappable zones may not be mapped, or may be mapped to something reasonably accurate.
            // We don't mind either way.
            if (!TzdbDateTimeZoneSource.Default.WindowsMapping.PrimaryMapping.ContainsKey(bclZone.Id))
            {
                return;
            }

            Assert.IsNotNull(id);
            var tzdbZone = TzdbDateTimeZoneSource.Default.ForId(id);

            var thisYear = SystemClock.Instance.Now.InUtc().Year;
            LocalDate? lastIncorrectDate = null;
            Offset? lastIncorrectBclOffset = null;
            Offset? lastIncorrectTzdbOffset = null;

            int total = 0;
            int correct = 0;
            // From the start of this year to the end of next year, we should have an 80% hit rate or better.
            // That's stronger than the 70% we limit to in the code, because if it starts going between 70% and 80% we
            // should have another look at the algorithm. (And this is dealing with 80% of days, not 80% of transitions,
            // so it's not quite equivalent anyway.)
            for (var date = new LocalDate(thisYear, 1, 1); date.Year < thisYear + 2; date = date.PlusDays(1))
            {
                Instant startOfUtcDay = date.AtMidnight().InUtc().ToInstant();
                Offset tzdbOffset = tzdbZone.GetUtcOffset(startOfUtcDay);
                Offset bclOffset = Offset.FromTimeSpan(bclZone.GetUtcOffset(startOfUtcDay.ToDateTimeOffset()));
                if (tzdbOffset == bclOffset)
                {
                    correct++;
                }
                else
                {
                    // Useful for debugging (by having somewhere to put a breakpoint) as well as for the message.
                    lastIncorrectDate = date;
                    lastIncorrectBclOffset = bclOffset;
                    lastIncorrectTzdbOffset = tzdbOffset;
                }
                total++;
            }
            Assert.That(correct * 100.0 / total, Is.GreaterThanOrEqualTo(80.0),
                "Last incorrect date for {0}: {1} (BCL: {2}; TZDB: {3})",
                bclZone.Id,
                lastIncorrectDate, lastIncorrectBclOffset, lastIncorrectTzdbOffset);
        }
    }
}
