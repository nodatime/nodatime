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
using NUnit.Framework;
using NodaTime.Testing.TimeZones;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Tests for TimeZoneCache.
    /// </summary>
    [TestFixture]
    public class TimeZoneCacheTest
    {
        [Test]
        public void Construction_NullProvider()
        {
            Assert.Throws<ArgumentNullException>(() => new TimeZoneCache(null));
        }

        [Test]
        public void CachingForPresentValues()
        {
            var provider = new TestDateTimeZoneProvider("Test1", "Test2");
            var cache = new TimeZoneCache(provider);
            var zone = cache["Test1"];
            Assert.IsNotNull(zone);
            Assert.AreEqual("Test1", provider.LastRequestedId);
            Assert.AreSame(zone, cache["Test1"]);
        }

        [Test]
        public void ProviderIsNotAskedForUtcIfNotAdvertised()
        {
            var provider = new TestDateTimeZoneProvider("Test1", "Test2");
            var cache = new TimeZoneCache(provider);
            var zone = cache[DateTimeZone.UtcId];
            Assert.IsNotNull(zone);
            Assert.IsNull(provider.LastRequestedId);
        }

        [Test]
        public void ProviderIsAskedForUtcIfAdvertised()
        {
            var provider = new TestDateTimeZoneProvider("Test1", "Test2", "UTC");
            var cache = new TimeZoneCache(provider);
            var zone = cache[DateTimeZone.UtcId];
            Assert.IsNotNull(zone);
            Assert.AreEqual("UTC", provider.LastRequestedId);
        }

        [Test]
        public void ProviderIsNotAskedForUnknownIds()
        {
            var provider = new TestDateTimeZoneProvider("Test1", "Test2");
            var cache = new TimeZoneCache(provider);
            var zone = cache["Unknown"];
            Assert.IsNull(zone);
            Assert.IsNull(provider.LastRequestedId);
        }

        [Test]
        public void NullIdRejected()
        {
            var cache = new TimeZoneCache(new TestDateTimeZoneProvider("Test1", "Test2"));
            // GetType call just to avoid trying to use a property as a statement...
            Assert.Throws<ArgumentNullException>(() => cache[null].GetType());
        }

        [Test]
        public void EmptyIdAccepted()
        {
            var cache = new TimeZoneCache(new TestDateTimeZoneProvider("Test1", "Test2"));
            Assert.IsNull(cache[""]);
        }

        [Test]
        public void VersionIdPassThrough()
        {
            var cache = new TimeZoneCache(new TestDateTimeZoneProvider("Test1", "Test2"));
            Assert.AreEqual("test-version", cache.ProviderVersionId);
        }

        private class TestDateTimeZoneProvider : IDateTimeZoneProvider
        {
            public string LastRequestedId { get; set; }
            private readonly string[] ids;

            public TestDateTimeZoneProvider(params string[] ids)
            {
                this.ids = ids;
            }

            public IEnumerable<string> Ids { get { return ids; } }

            public DateTimeZone ForId(string id)
            {
                LastRequestedId = id;
                return new SingleTransitionZone(Instant.UnixEpoch, 0, id.GetHashCode() % 24);
            }

            public string VersionId { get { return "test-version"; } }


            public string MapTimeZoneId(TimeZoneInfo timeZone)
            {
                return "map";
            }
        }
    }
}
