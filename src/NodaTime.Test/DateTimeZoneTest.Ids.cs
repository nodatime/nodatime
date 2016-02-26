// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Test
{
    /// <summary>
    /// Tests for code in DateTimeZone and code which will be moving out
    /// of DateTimeZones into DateTimeZone over time.
    /// </summary>
    public partial class DateTimeZoneTest
    {
        [Test]
        public void UtcIsNotNull()
        {
            Assert.IsNotNull(DateTimeZone.Utc);
        }
    }
}
