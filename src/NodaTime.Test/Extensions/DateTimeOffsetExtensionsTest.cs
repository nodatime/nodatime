// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Extensions;
using NUnit.Framework;
using System;

namespace NodaTime.Test.Extensions
{
    public class DateTimeOffsetExtensionsTest
    {
        private static readonly DateTimeOffset Sample = new DateTimeOffset(2017, 8, 21, 12, 46, 13, TimeSpan.FromHours(1));

        [Test]
        public void ToOffsetDateTime()
        {
            var expected = new OffsetDateTime(new LocalDateTime(2017, 8, 21, 12, 46, 13), Offset.FromHours(1));
            var actual = Sample.ToOffsetDateTime();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToZonedDateTime()
        {
            var expected = new LocalDateTime(2017, 8, 21, 12, 46, 13).InZoneStrictly(DateTimeZone.ForOffset(Offset.FromHours(1)));
            var actual = Sample.ToZonedDateTime();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToInstant()
        {
            // Note hour of 11 rather than 12.
            var expected = Instant.FromUtc(2017, 8, 21, 11, 46, 13);
            var actual = Sample.ToInstant();
            Assert.AreEqual(expected, actual);
        }
    }
}
