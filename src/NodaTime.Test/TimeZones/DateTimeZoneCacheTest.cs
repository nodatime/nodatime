#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NodaTime.Testing.TimeZones;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Tests for DateTimeZoneFactory.
    /// </summary>
    [TestFixture]
    public class DateTimeZoneCacheTest
    {
        [Test]
        public void DefaultProviderIsTzdb()
        {
            Assert.IsTrue(DateTimeZoneProviders.Default.VersionId.StartsWith("TZDB: "));
        }

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
            var zone = provider["Test1"];
            Assert.IsNotNull(zone);
            Assert.AreEqual("Test1", source.LastRequestedId);
            Assert.AreSame(zone, provider["Test1"]);
        }

        [Test]
        public void ProviderIsNotAskedForUtcIfNotAdvertised()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2");
            var provider = new DateTimeZoneCache(source);
            var zone = provider[DateTimeZone.UtcId];
            Assert.IsNotNull(zone);
            Assert.IsNull(source.LastRequestedId);
        }

        [Test]
        public void ProviderIsAskedForUtcIfAdvertised()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2", "UTC");
            var provider = new DateTimeZoneCache(source);
            var zone = provider[DateTimeZone.UtcId];
            Assert.IsNotNull(zone);
            Assert.AreEqual("UTC", source.LastRequestedId);
        }

        [Test]
        public void ProviderIsNotAskedForUnknownIds()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2");
            var provider = new DateTimeZoneCache(source);
            Assert.Throws<TimeZoneNotFoundException>(() => { var ignored = provider["Unknown"]; });
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
            Assert.Throws<TimeZoneNotFoundException>(() => { var ignored = provider[""]; });
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
        public void Tzdb_ForId_UtcId()
        {
            Assert.AreEqual(DateTimeZone.Utc, DateTimeZoneProviders.Tzdb[DateTimeZone.UtcId]);
        }

        [Test]
        public void Tzdb_ForId_AmericaLosAngeles()
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
        public void Tzdb_ForId_AllIds()
        {
            foreach (string id in DateTimeZoneProviders.Tzdb.Ids)
            {
                Assert.IsNotNull(DateTimeZoneProviders.Tzdb[id]);
            }
        }

        [Test]
        public void Tzdb_ForId_FixedOffset()
        {
            string id = "UTC+05:30";
            DateTimeZone zone = DateTimeZoneProviders.Tzdb[id];
            Assert.AreEqual(DateTimeZone.ForOffset(Offset.FromHoursAndMinutes(5, 30)), zone);
            Assert.AreEqual(id, zone.Id);
        }

        [Test]
        public void Tzdb_ForId_FixedOffset_NonCanonicalId()
        {
            string id = "UTC+05:00:00";
            DateTimeZone zone = DateTimeZoneProviders.Tzdb[id];
            Assert.AreEqual(zone, DateTimeZone.ForOffset(Offset.FromHours(5)));
            Assert.AreEqual("UTC+05", zone.Id);
        }

        [Test]
        public void Tzdb_ForId_InvalidFixedOffset()
        {
            Assert.Throws<TimeZoneNotFoundException>(() => { var ignored = DateTimeZoneProviders.Tzdb["UTC+5Months"]; });
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
                return new SingleTransitionZone(Instant.UnixEpoch, 0, id.GetHashCode() % 24);
            }

            public string VersionId { get; set; }


            public string MapTimeZoneId(TimeZoneInfo timeZone)
            {
                return "map";
            }
        }
    }
}
