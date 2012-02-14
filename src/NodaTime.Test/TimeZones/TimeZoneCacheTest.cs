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
            var cache = new TimeZoneCache(new TestDateTimeZoneProvider());
            var zone = cache["Test1"];
            Assert.IsNotNull(zone);
            Assert.AreSame(zone, cache["Test1"]);
        }

        [Test]
        public void ProviderIsNotAskedForUtc()
        {
            var cache = new TimeZoneCache(new TestDateTimeZoneProvider());
            var zone = cache[DateTimeZone.UtcId];
            // Well it can't come from our test provider...
            Assert.IsNotNull(zone);
        }

        [Test]
        public void ProviderIsNotAskedForUnknownIds()
        {
            var cache = new TimeZoneCache(new TestDateTimeZoneProvider());
            // This would throw...
            var zone = cache["Unknown"];
            Assert.IsNull(zone);
        }

        [Test]
        public void NullIdRejected()
        {
            var cache = new TimeZoneCache(new TestDateTimeZoneProvider());
            // GetType call just to avoid trying to use a property as a statement...
            Assert.Throws<ArgumentNullException>(() => cache[null].GetType());
        }

        [Test]
        public void EmptyIdAccepted()
        {
            var cache = new TimeZoneCache(new TestDateTimeZoneProvider());
            Assert.IsNull(cache[""]);
        }

        [Test]
        public void VersionIdPassThrough()
        {
            var cache = new TimeZoneCache(new TestDateTimeZoneProvider());
            Assert.AreEqual("test-version", cache.ProviderVersionId);
        }

        private class TestDateTimeZoneProvider : IDateTimeZoneProvider
        {
            public IEnumerable<string> Ids
            {
                get { return new[] { "Test1", "Test2" }; }
            }

            public DateTimeZone ForId(string id)
            {
                switch (id)
                {
                    case "Test1":
                        return new SingleTransitionZone(Instant.UnixEpoch, 0, 1);
                    case "Test2":
                        return new SingleTransitionZone(Instant.UnixEpoch, 0, 2);
                    default:
                        throw new InvalidOperationException();
                }
            }

            public string VersionId { get { return "test-version"; } }


            public string MapTimeZoneId(TimeZoneInfo timeZone)
            {
                return "map";
            }
        }
    }
}
