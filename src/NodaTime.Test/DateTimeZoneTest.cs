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
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class DateTimeZoneTest
    {
        [SetUp]
        public void Setup()
        {
            // Reset the cache...
            DateTimeZone.SetProvider(DateTimeZone.DefaultDateTimeZoneProvider);
        }

        [TearDown]
        public void TearDown()
        {
            // Reset the cache...
            DateTimeZone.SetProvider(DateTimeZone.DefaultDateTimeZoneProvider);
        }

        [Test]
        public void DefaultProviderIsTzdb()
        {
            Assert.IsTrue(DateTimeZone.ProviderVersionId.StartsWith("TZDB: "));
        }

        // The current implementation caches every half hour, -12 to +15.
        [Test]
        public void ForOffset_UncachedExample_NotOnHalfHour()
        {
            var offset = Offset.FromMilliseconds(12345);
            var zone1 = DateTimeZone.ForOffset(offset);
            var zone2 = DateTimeZone.ForOffset(offset);

            Assert.AreNotSame(zone1, zone2);
            Assert.IsTrue(zone1.IsFixed);
            Assert.AreEqual(offset, zone1.MaxOffset);
            Assert.AreEqual(offset, zone1.MinOffset);
        }

        [Test]
        public void ForOffset_UncachedExample_OutsideCacheRange()
        {
            var offset = Offset.FromHours(-14);
            var zone1 = DateTimeZone.ForOffset(offset);
            var zone2 = DateTimeZone.ForOffset(offset);

            Assert.AreNotSame(zone1, zone2);
            Assert.IsTrue(zone1.IsFixed);
            Assert.AreEqual(offset, zone1.MaxOffset);
            Assert.AreEqual(offset, zone1.MinOffset);
        }

        [Test]
        public void ForOffset_CachedExample()
        {
            var offset = Offset.FromHours(2);
            var zone1 = DateTimeZone.ForOffset(offset);
            var zone2 = DateTimeZone.ForOffset(offset);
            // Caching check...
            Assert.AreSame(zone1, zone2);

            Assert.IsTrue(zone1.IsFixed);
            Assert.AreEqual(offset, zone1.MaxOffset);
            Assert.AreEqual(offset, zone1.MinOffset);
        }

        [Test]
        public void ForOffset_Zero_SameAsUtc()
        {
            Assert.AreSame(DateTimeZone.Utc, DateTimeZone.ForOffset(Offset.Zero));
        }
    }
}
