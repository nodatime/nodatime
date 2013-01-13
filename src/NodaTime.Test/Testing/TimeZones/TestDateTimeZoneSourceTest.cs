#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2013 Jon Skeet
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
using System.Text;
using NUnit.Framework;
using NodaTime.Testing.TimeZones;

namespace NodaTime.Test.Testing.TimeZones
{
    [TestFixture]
    public class TestDateTimeZoneSourceTest
    {
        // We don't care about the details of the time zones, just the IDs
        private static DateTimeZone CreateZone(string id)
        {
            return new SingleTransitionDateTimeZone(NodaConstants.UnixEpoch, Offset.FromHours(1), Offset.FromHours(2), id);
        }

        [Test]
        public void Ids()
        {
            var source = new TestDateTimeZoneSource.Builder
            {
                CreateZone("x"), CreateZone("y"), CreateZone("a"), CreateZone("b")
            }.Build();
            CollectionAssert.AreEquivalent(source.GetIds(), new[] { "x", "y", "a", "b" });
        }

        [Test]
        public void ForId_Present()
        {
            var zone = CreateZone("x");
            var source = new TestDateTimeZoneSource.Builder
            {
                // The "right" one and some others
                zone, CreateZone("y"), CreateZone("a"), CreateZone("b")
            }.Build();
            Assert.AreSame(zone, source.ForId("x"));
        }

        [Test]
        public void ForId_Missing()
        {
            var source = new TestDateTimeZoneSource.Builder
            {
                CreateZone("x"), CreateZone("y")
            }.Build();
            Assert.Throws<ArgumentException>(() => source.ForId("missing"));
        }

        [Test]
        public void DuplicateIds()
        {
            var builder = new TestDateTimeZoneSource.Builder
            {
                CreateZone("x"), CreateZone("y"), CreateZone("x")
            };
            AssertBuildFails(builder);
        }

        [Test]
        public void ValidWindowsMapping()
        {
            var source = new TestDateTimeZoneSource.Builder
            {
                BclIdsToZoneIds = { { TimeZoneInfo.Local.Id, "x"} },
                Zones = { CreateZone("x"), CreateZone("y") }
            }.Build();
            Assert.AreEqual("x", source.MapTimeZoneId(TimeZoneInfo.Local));
        }

        [Test]
        public void NullZoneViaCollectionInitializer()
        {
            Assert.Throws<ArgumentNullException>(() => new TestDateTimeZoneSource.Builder { null });
        }

        [Test]
        public void NullZoneViaProperty()
        {
            var source = new TestDateTimeZoneSource.Builder
            {
                Zones = { null }
            };
            AssertBuildFails(source);
        }

        [Test]
        public void InvalidWindowsMapping_NullCanonicalId()
        {
            var source = new TestDateTimeZoneSource.Builder
            {
                BclIdsToZoneIds = { { "x", null } },
                Zones = { CreateZone("z") }
            };
            AssertBuildFails(source);
        }

        [Test]
        public void InvalidWindowsMapping_MissingZone()
        {
            var source = new TestDateTimeZoneSource.Builder
            {
                BclIdsToZoneIds = { { TimeZoneInfo.Local.Id, "z" } },
                Zones = { CreateZone("x"), CreateZone("y") }
            };
            AssertBuildFails(source);
        }

        // We don't really care how it fails - just that an exception is thrown.
        // Unfortuntely NUnit requires the exact exception type :(
        private static void AssertBuildFails(TestDateTimeZoneSource.Builder builder)
        {
            try
            {
                builder.Build();
                Assert.Fail("Expected exception");
            }
            catch (Exception)
            {
                // Expected
            }
        }
    }
}
