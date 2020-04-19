// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Linq;
using NodaTime.Testing.TimeZones;
using NodaTime.Xml;
using NUnit.Framework;

namespace NodaTime.Test
{
    /// <summary>
    /// Tests for DateTimeZoneProviders.
    /// </summary>
    public class DateTimeZoneProvidersTest
    {
        [Test]
        public void TzdbProviderUsesTzdbSource()
        {
            Assert.IsTrue(DateTimeZoneProviders.Tzdb.VersionId.StartsWith("TZDB: "));
        }

        [Test]
        public void AllTzdbTimeZonesLoad()
        {
            var allZones = DateTimeZoneProviders.Tzdb.Ids.Select(id => DateTimeZoneProviders.Tzdb[id]).ToList();
            // Just to stop the variable from being lonely. In reality, it's likely there'll be a breakpoint here to inspect a particular zone...
            Assert.IsTrue(allZones.Count > 50);
        }

        [Test]
        public void BclProviderUsesTimeZoneInfoSource()
        {
            Assert.IsTrue(DateTimeZoneProviders.Bcl.VersionId.StartsWith("TimeZoneInfo: "));
        }

        [Test]
        public void SerializationDelegatesToXmlSerializerSettings()
        {
            var original = XmlSerializationSettings.DateTimeZoneProvider;

            try
            {
#pragma warning disable CS0618 // Type or member is obsolete
                var provider1 = new FakeDateTimeZoneSource.Builder().Build().ToProvider();
                DateTimeZoneProviders.Serialization = provider1;
                Assert.AreSame(provider1, XmlSerializationSettings.DateTimeZoneProvider);

                var provider2 = new FakeDateTimeZoneSource.Builder().Build().ToProvider();
                XmlSerializationSettings.DateTimeZoneProvider = provider2;
                Assert.AreSame(provider2, DateTimeZoneProviders.Serialization);
#pragma warning restore CS0618 // Type or member is obsolete
            }
            finally
            {
                XmlSerializationSettings.DateTimeZoneProvider = original;
            }
        }
    }
}
