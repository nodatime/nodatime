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
        public void Construction_NullProvider()
        {
            Assert.Throws<ArgumentNullException>(() => new DateTimeZoneFactory(null));
        }

        [Test]
        public void InvalidProvider_NullVersionId()
        {
            var provider = new TestDateTimeZoneProvider("Test1", "Test2") { VersionId = null };
            Assert.Throws<InvalidDateTimeZoneProviderException>(() => new DateTimeZoneFactory(provider));
        }

        [Test]
        public void InvalidProvider_NullIdSequence()
        {
            string[] ids = null;
            var provider = new TestDateTimeZoneProvider(ids);
            Assert.Throws<InvalidDateTimeZoneProviderException>(() => new DateTimeZoneFactory(provider));
        }

        [Test]
        public void InvalidProvider_NullIdWithinSequence()
        {
            var provider = new TestDateTimeZoneProvider("Test1", null);
            Assert.Throws<InvalidDateTimeZoneProviderException>(() => new DateTimeZoneFactory(provider));
        }

        [Test]
        public void CachingForPresentValues()
        {
            var provider = new TestDateTimeZoneProvider("Test1", "Test2");
            var factory = new DateTimeZoneFactory(provider);
            var zone = factory["Test1"];
            Assert.IsNotNull(zone);
            Assert.AreEqual("Test1", provider.LastRequestedId);
            Assert.AreSame(zone, factory["Test1"]);
        }

        [Test]
        public void ProviderIsNotAskedForUtcIfNotAdvertised()
        {
            var provider = new TestDateTimeZoneProvider("Test1", "Test2");
            var factory = new DateTimeZoneFactory(provider);
            var zone = factory[DateTimeZone.UtcId];
            Assert.IsNotNull(zone);
            Assert.IsNull(provider.LastRequestedId);
        }

        [Test]
        public void ProviderIsAskedForUtcIfAdvertised()
        {
            var provider = new TestDateTimeZoneProvider("Test1", "Test2", "UTC");
            var factory = new DateTimeZoneFactory(provider);
            var zone = factory[DateTimeZone.UtcId];
            Assert.IsNotNull(zone);
            Assert.AreEqual("UTC", provider.LastRequestedId);
        }

        [Test]
        public void ProviderIsNotAskedForUnknownIds()
        {
            var provider = new TestDateTimeZoneProvider("Test1", "Test2");
            var factory = new DateTimeZoneFactory(provider);
            Assert.Throws<TimeZoneNotFoundException>(() => { var ignored = factory["Unknown"]; });
            Assert.IsNull(provider.LastRequestedId);
        }

        [Test]
        public void UtcIsReturnedInIdsIfAdvertisedByProvider()
        {
            var provider = new TestDateTimeZoneProvider("Test1", "Test2", "UTC");
            var factory = new DateTimeZoneFactory(provider);
            Assert.True(factory.Ids.Contains(DateTimeZone.UtcId));
        }

        [Test]
        public void UtcIsNotReturnedInIdsIfNotAdvertisedByProvider()
        {
            var provider = new TestDateTimeZoneProvider("Test1", "Test2");
            var factory = new DateTimeZoneFactory(provider);
            Assert.False(factory.Ids.Contains(DateTimeZone.UtcId));
        }

        [Test]
        public void NullIdRejected()
        {
            var factory = new DateTimeZoneFactory(new TestDateTimeZoneProvider("Test1", "Test2"));
            // GetType call just to avoid trying to use a property as a statement...
            Assert.Throws<ArgumentNullException>(() => factory[null].GetType());
        }

        [Test]
        public void EmptyIdAccepted()
        {
            var factory = new DateTimeZoneFactory(new TestDateTimeZoneProvider("Test1", "Test2"));
            Assert.Throws<TimeZoneNotFoundException>(() => { var ignored = factory[""]; });
        }

        [Test]
        public void VersionIdPassThrough()
        {
            var factory = new DateTimeZoneFactory(new TestDateTimeZoneProvider("Test1", "Test2") { VersionId = "foo" });
            Assert.AreEqual("foo", factory.ProviderVersionId);
        }

        private class TestDateTimeZoneProvider : IDateTimeZoneProvider
        {
            public string LastRequestedId { get; set; }
            private readonly string[] ids;

            public TestDateTimeZoneProvider(params string[] ids)
            {
                this.ids = ids;
                this.VersionId = "test version";
            }

            public IEnumerable<string> Ids { get { return ids; } }

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
