// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Test
{
    /// <summary>
    /// Tests for DateTimeZoneProviders.
    /// </summary>
    [TestFixture]
    public class DateTimeZoneProvidersTest
    {
        [Test]
        public void DefaultProviderIsTzdb()
        {
#pragma warning disable 0618
            Assert.AreSame(DateTimeZoneProviders.Tzdb, DateTimeZoneProviders.Default);
#pragma warning restore 0618
        }

        [Test]
        public void TzdbProviderUsesTzdbSource()
        {
            Assert.IsTrue(DateTimeZoneProviders.Tzdb.VersionId.StartsWith("TZDB: "));
        }

#if !PCL
        [Test]
        public void BclProviderUsesTimeZoneInfoSource()
        {
            Assert.IsTrue(DateTimeZoneProviders.Bcl.VersionId.StartsWith("TimeZoneInfo: "));
        }
#endif
    }
}
