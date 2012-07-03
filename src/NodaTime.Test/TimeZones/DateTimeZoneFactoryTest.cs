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
    public class DateTimeZoneFactoryTest
    {
        [Test]
        public void DefaultProviderIsTzdb()
        {
            Assert.IsTrue(DateTimeZoneFactory.Default.SourceVersionId.StartsWith("TZDB: "));
        }

        [Test]
        public void Construction_NullProvider()
        {
            Assert.Throws<ArgumentNullException>(() => new DateTimeZoneFactory(null));
        }

        [Test]
        public void InvalidProvider_NullVersionId()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2") { VersionId = null };
            Assert.Throws<InvalidDateTimeZoneSourceException>(() => new DateTimeZoneFactory(source));
        }

        [Test]
        public void InvalidProvider_NullIdSequence()
        {
            string[] ids = null;
            var source = new TestDateTimeZoneSource(ids);
            Assert.Throws<InvalidDateTimeZoneSourceException>(() => new DateTimeZoneFactory(source));
        }

        [Test]
        public void InvalidProvider_NullIdWithinSequence()
        {
            var source = new TestDateTimeZoneSource("Test1", null);
            Assert.Throws<InvalidDateTimeZoneSourceException>(() => new DateTimeZoneFactory(source));
        }

        [Test]
        public void CachingForPresentValues()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2");
            var factory = new DateTimeZoneFactory(source);
            var zone = factory["Test1"];
            Assert.IsNotNull(zone);
            Assert.AreEqual("Test1", source.LastRequestedId);
            Assert.AreSame(zone, factory["Test1"]);
        }

        [Test]
        public void ProviderIsNotAskedForUtcIfNotAdvertised()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2");
            var factory = new DateTimeZoneFactory(source);
            var zone = factory[DateTimeZone.UtcId];
            Assert.IsNotNull(zone);
            Assert.IsNull(source.LastRequestedId);
        }

        [Test]
        public void ProviderIsAskedForUtcIfAdvertised()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2", "UTC");
            var factory = new DateTimeZoneFactory(source);
            var zone = factory[DateTimeZone.UtcId];
            Assert.IsNotNull(zone);
            Assert.AreEqual("UTC", source.LastRequestedId);
        }

        [Test]
        public void ProviderIsNotAskedForUnknownIds()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2");
            var factory = new DateTimeZoneFactory(source);
            Assert.Throws<TimeZoneNotFoundException>(() => { var ignored = factory["Unknown"]; });
            Assert.IsNull(source.LastRequestedId);
        }

        [Test]
        public void UtcIsReturnedInIdsIfAdvertisedByProvider()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2", "UTC");
            var factory = new DateTimeZoneFactory(source);
            Assert.True(factory.Ids.Contains(DateTimeZone.UtcId));
        }

        [Test]
        public void UtcIsNotReturnedInIdsIfNotAdvertisedByProvider()
        {
            var source = new TestDateTimeZoneSource("Test1", "Test2");
            var factory = new DateTimeZoneFactory(source);
            Assert.False(factory.Ids.Contains(DateTimeZone.UtcId));
        }

        [Test]
        public void NullIdRejected()
        {
            var factory = new DateTimeZoneFactory(new TestDateTimeZoneSource("Test1", "Test2"));
            // GetType call just to avoid trying to use a property as a statement...
            Assert.Throws<ArgumentNullException>(() => factory[null].GetType());
        }

        [Test]
        public void EmptyIdAccepted()
        {
            var factory = new DateTimeZoneFactory(new TestDateTimeZoneSource("Test1", "Test2"));
            Assert.Throws<TimeZoneNotFoundException>(() => { var ignored = factory[""]; });
        }

        [Test]
        public void VersionIdPassThrough()
        {
            var factory = new DateTimeZoneFactory(new TestDateTimeZoneSource("Test1", "Test2") { VersionId = "foo" });
            Assert.AreEqual("foo", factory.SourceVersionId);
        }

        [Test(Description = "Test for issue 7 in bug tracker")]
        public void Tzdb_IterateOverIds()
        {
            // According to bug, this would go bang
            int count = DateTimeZoneFactory.Tzdb.Ids.Count();

            Assert.IsTrue(count > 1);
            int utcCount = DateTimeZoneFactory.Tzdb.Ids.Count(id => id == DateTimeZone.UtcId);
            Assert.AreEqual(1, utcCount);
        }

        [Test]
        public void Tzdb_ForId_UtcId()
        {
            Assert.AreEqual(DateTimeZone.Utc, DateTimeZoneFactory.Tzdb[DateTimeZone.UtcId]);
        }

        [Test]
        public void Tzdb_ForId_AmericaLosAngeles()
        {
            const string americaLosAngeles = "America/Los_Angeles";
            var actual = DateTimeZoneFactory.Tzdb[americaLosAngeles];
            Assert.IsNotNull(actual);
            Assert.AreNotEqual(DateTimeZone.Utc, actual);
            Assert.AreEqual(americaLosAngeles, actual.Id);
        }

        [Test]
        public void Tzdb_Ids_All()
        {
            var actual = DateTimeZoneFactory.Tzdb.Ids;
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
            foreach (string id in DateTimeZoneFactory.Tzdb.Ids)
            {
                Assert.IsNotNull(DateTimeZoneFactory.Tzdb[id]);
            }
        }

        [Test]
        public void Tzdb_ForId_FixedOffset()
        {
            string id = "UTC+05:30";
            DateTimeZone zone = DateTimeZoneFactory.Tzdb[id];
            Assert.AreEqual(DateTimeZone.ForOffset(Offset.FromHoursAndMinutes(5, 30)), zone);
            Assert.AreEqual(id, zone.Id);
        }

        [Test]
        public void Tzdb_ForId_FixedOffset_NonCanonicalId()
        {
            string id = "UTC+05:00:00";
            DateTimeZone zone = DateTimeZoneFactory.Tzdb[id];
            Assert.AreEqual(zone, DateTimeZone.ForOffset(Offset.FromHours(5)));
            Assert.AreEqual("UTC+05", zone.Id);
        }

        [Test]
        public void Tzdb_ForId_InvalidFixedOffset()
        {
            Assert.Throws<TimeZoneNotFoundException>(() => { var ignored = DateTimeZoneFactory.Tzdb["UTC+5Months"]; });
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
