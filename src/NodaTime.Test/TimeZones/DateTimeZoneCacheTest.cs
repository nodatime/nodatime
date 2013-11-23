// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime.Testing.TimeZones;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Tests for DateTimeZoneCache.
    /// </summary>
    [TestFixture]
    public class DateTimeZoneCacheTest
    {
        [Test]
        public void Construction_NullProvider()
        {
            Assert.Throws<ArgumentNullException>(() => new DateTimeZoneCache(null));
        }

        [Test]
        public void InvalidProvider_NullVersionId()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2") { VersionId = null };
            Assert.Throws<InvalidDateTimeZoneSourceException>(() => new DateTimeZoneCache(source));
        }

        [Test]
        public void InvalidProvider_NullIdSequence()
        {
            string[] ids = null;
            var source = new TestDateTimeZoneSource(ids);
            Assert.Throws<InvalidDateTimeZoneSourceException>(() => new DateTimeZoneCache(source));
        }

        [Test]
        public void InvalidProvider_NullIdWithinSequence()
        {
            var source = new TestDateTimeZoneSource("Test1", null);
            Assert.Throws<InvalidDateTimeZoneSourceException>(() => new DateTimeZoneCache(source));
        }

        [Test]
        public void CachingForPresentValues()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2");
            var provider = new DateTimeZoneCache(source);
            var zone1a = provider["Test1"];
            Assert.IsNotNull(zone1a);
            Assert.AreEqual("Test1", source.LastRequestedId);

            // Hit up the cache (and thus the source) for Test2
            Assert.IsNotNull(provider["Test2"]);
            Assert.AreEqual("Test2", source.LastRequestedId);

            // Ask for Test1 again
            var zone1b = provider["Test1"];
            // We won't have consulted the source again
            Assert.AreEqual("Test2", source.LastRequestedId);

            Assert.AreSame(zone1a, zone1b);
        }

        [Test]
        public void SourceIsNotAskedForUtcIfNotAdvertised()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2");
            var provider = new DateTimeZoneCache(source);
            var zone = provider[DateTimeZone.UtcId];
            Assert.IsNotNull(zone);
            Assert.IsNull(source.LastRequestedId);
        }

        [Test]
        public void SourceIsNotAskedForUtcIfAdvertised()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2", "UTC");
            var provider = new DateTimeZoneCache(source);
            var zone = provider[DateTimeZone.UtcId];
            Assert.IsNotNull(zone);
            Assert.IsNull(source.LastRequestedId);
        }

        [Test]
        public void SourceIsNotAskedForUnknownIds()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2");
            var provider = new DateTimeZoneCache(source);
            Assert.Throws<DateTimeZoneNotFoundException>(() => { var ignored = provider["Unknown"]; });
            Assert.IsNull(source.LastRequestedId);
        }

        [Test]
        public void UtcIsReturnedInIdsIfAdvertisedByProvider()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2", "UTC");
            var provider = new DateTimeZoneCache(source);
            Assert.True(provider.Ids.Contains(DateTimeZone.UtcId));
        }

        [Test]
        public void UtcIsNotReturnedInIdsIfNotAdvertisedByProvider()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2");
            var provider = new DateTimeZoneCache(source);
            Assert.False(provider.Ids.Contains(DateTimeZone.UtcId));
        }

        [Test]
        public void FixedOffsetSucceedsWhenNotAdvertised()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2");
            var provider = new DateTimeZoneCache(source);
            string id = "UTC+05:30";
            DateTimeZone zone = provider[id];
            Assert.AreEqual(DateTimeZone.ForOffset(Offset.FromHoursAndMinutes(5, 30)), zone);
            Assert.AreEqual(id, zone.Id);
            Assert.IsNull(source.LastRequestedId);
        }

        [Test]
        public void FixedOffsetSucceedsWithoutConsultingSourceWhenAdvertised()
        {
            string id = "UTC+05:30";
            var source = new TestDateTimeZoneSource("Test1", "Test2", id);
            var provider = new DateTimeZoneCache(source);
            DateTimeZone zone = provider[id];
            Assert.AreEqual(DateTimeZone.ForOffset(Offset.FromHoursAndMinutes(5, 30)), zone);
            Assert.AreEqual(id, zone.Id);
            Assert.IsNull(source.LastRequestedId);
        }

        [Test]
        public void FixedOffsetUncached()
        {
            string id = "UTC+05:26";
            var source = new TestDateTimeZoneSource("Test1", "Test2");
            var provider = new DateTimeZoneCache(source);
            DateTimeZone zone1 = provider[id];
            DateTimeZone zone2 = provider[id];
            Assert.AreNotSame(zone1, zone2);
            Assert.AreEqual(zone1, zone2);
        }

        [Test]
        public void FixedOffsetZeroReturnsUtc()
        {
            string id = "UTC+00:00";
            var source = new TestDateTimeZoneSource("Test1", "Test2", id);
            var provider = new DateTimeZoneCache(source);
            DateTimeZone zone = provider[id];
            Assert.AreEqual(DateTimeZone.Utc, zone);
            Assert.IsNull(source.LastRequestedId);
        }

        [Test]
        public void Tzdb_Indexer_InvalidFixedOffset()
        {
            Assert.Throws<DateTimeZoneNotFoundException>(() => { var ignored = DateTimeZoneProviders.Tzdb["UTC+5Months"]; });
        }

        [Test]
        public void NullIdRejected()
        {
            var provider = new DateTimeZoneCache(new TestDateTimeZoneSource("Test1", "Test2"));
            // GetType call just to avoid trying to use a property as a statement...
            Assert.Throws<ArgumentNullException>(() => provider[null].GetType());
        }

        [Test]
        public void EmptyIdAccepted()
        {
            var provider = new DateTimeZoneCache(new TestDateTimeZoneSource("Test1", "Test2"));
            Assert.Throws<DateTimeZoneNotFoundException>(() => { var ignored = provider[""]; });
        }

        [Test]
        public void VersionIdPassThrough()
        {
            var provider = new DateTimeZoneCache(new TestDateTimeZoneSource("Test1", "Test2") { VersionId = "foo" });
            Assert.AreEqual("foo", provider.VersionId);
        }

        [Test(Description = "Test for issue 7 in bug tracker")]
        public void Tzdb_IterateOverIds()
        {
            // According to bug, this would go bang
            int count = DateTimeZoneProviders.Tzdb.Ids.Count();

            Assert.IsTrue(count > 1);
            int utcCount = DateTimeZoneProviders.Tzdb.Ids.Count(id => id == DateTimeZone.UtcId);
            Assert.AreEqual(1, utcCount);
        }

        [Test]
        public void Tzdb_Indexer_UtcId()
        {
            Assert.AreEqual(DateTimeZone.Utc, DateTimeZoneProviders.Tzdb[DateTimeZone.UtcId]);
        }

        [Test]
        public void Tzdb_Indexer_AmericaLosAngeles()
        {
            const string americaLosAngeles = "America/Los_Angeles";
            var actual = DateTimeZoneProviders.Tzdb[americaLosAngeles];
            Assert.IsNotNull(actual);
            Assert.AreNotEqual(DateTimeZone.Utc, actual);
            Assert.AreEqual(americaLosAngeles, actual.Id);
        }

        [Test]
        public void Tzdb_Ids_All()
        {
            var actual = DateTimeZoneProviders.Tzdb.Ids;
            var actualCount = actual.Count();
            Assert.IsTrue(actualCount > 1);
            var utc = actual.Single(id => id == DateTimeZone.UtcId);
            Assert.AreEqual(DateTimeZone.UtcId, utc);
        }

        /// <summary>
        /// Simply tests that every ID in the built-in database can be fetched. This is also
        /// helpful for diagnostic debugging when we want to check that some potential
        /// invariant holds for all time zones...
        /// </summary>
        [Test]
        public void Tzdb_Indexer_AllIds()
        {
            foreach (string id in DateTimeZoneProviders.Tzdb.Ids)
            {
                Assert.IsNotNull(DateTimeZoneProviders.Tzdb[id]);
            }
        }

        private class TestDateTimeZoneSource : IDateTimeZoneSource
        {
            public string LastRequestedId { get; set; }
            private readonly string[] ids;

            public TestDateTimeZoneSource(params string[] ids)
            {
                this.ids = ids;
                this.VersionId = "test version";
            }

            public IEnumerable<string> GetIds() { return ids; }

            public DateTimeZone ForId(string id)
            {
                LastRequestedId = id;
                return new SingleTransitionDateTimeZone(NodaConstants.UnixEpoch, 0, id.GetHashCode() % 24);
            }

            public string VersionId { get; set; }


            public string MapTimeZoneId(TimeZoneInfo timeZone)
            {
                return "map";
            }
        }
    }
}
