// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Extensions;
using NUnit.Framework;
using System;

namespace NodaTime.Test.Extensions
{
    public class DateTimeExtensionsTest
    {
        private static readonly DateTime DateTimeLocal = new DateTime(2017, 8, 21, 12, 46, 13, DateTimeKind.Local);
        private static readonly DateTime DateTimeUnspecified = new DateTime(2017, 8, 21, 12, 46, 13, DateTimeKind.Unspecified);
        private static readonly DateTime DateTimeUtc = new DateTime(2017, 8, 21, 12, 46, 13, DateTimeKind.Utc);


        [Test]
        public void ToLocalDateTime_LocalKind()
        {
            var expected = new LocalDateTime(2017, 8, 21, 12, 46, 13);
            var actual = DateTimeLocal.ToLocalDateTime();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToLocalDateTime_UnspecifiedKind()
        {
            var expected = new LocalDateTime(2017, 8, 21, 12, 46, 13);
            var actual = DateTimeUnspecified.ToLocalDateTime();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToLocalDateTime_UtcKind()
        {
            var expected = new LocalDateTime(2017, 8, 21, 12, 46, 13);
            var actual = DateTimeUtc.ToLocalDateTime();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToInstant_LocalKind() => Assert.Throws<ArgumentException>(() => DateTimeLocal.ToInstant());

        [Test]
        public void ToInstant_UnspecifiedKind() => Assert.Throws<ArgumentException>(() => DateTimeUnspecified.ToInstant());

        [Test]
        public void ToInstant_UtcKind()
        {
            var expected = Instant.FromUtc(2017, 8, 21, 12, 46, 13);
            var actual = DateTimeUtc.ToInstant();
            Assert.AreEqual(expected, actual);
        }
    }
}
