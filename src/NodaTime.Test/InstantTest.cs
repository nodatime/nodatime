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
    public partial class InstantTest
    {
        private const long Y2002Days =
            365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 +
            365 + 366 + 365 + 365 + 365 + 366 + 365;

        private const long Y2003Days =
            365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 + 365 + 366 + 365 + 365 +
            365 + 366 + 365 + 365 + 365 + 366 + 365 + 365;

        // 2002-04-05
        private const long TestTime1 =
            (Y2002Days + 31L + 28L + 31L + 5L - 1L) * NodaConstants.MillisecondsPerStandardDay + 12L * NodaConstants.MillisecondsPerHour +
            24L * NodaConstants.MillisecondsPerMinute;

        // 2003-05-06
        private const long TestTime2 =
            (Y2003Days + 31L + 28L + 31L + 30L + 6L - 1L) * NodaConstants.MillisecondsPerStandardDay + 14L * NodaConstants.MillisecondsPerHour +
            28L * NodaConstants.MillisecondsPerMinute;

        private Instant one = new Instant(1L);
        private readonly Instant onePrime = new Instant(1L);
        private Instant negativeOne = new Instant(-1L);
        private Instant threeMillion = new Instant(3000000L);
        private Instant negativeFiftyMillion = new Instant(-50000000L);

        private readonly Duration durationNegativeEpsilon = Duration.FromTicks(-1L);
        private readonly Offset offsetOneHour = Offset.FromHours(1);

        [Test]
        public void TestInstantOperators()
        {
            const long diff = TestTime2 - TestTime1;

            var time1 = new Instant(TestTime1);
            var time2 = new Instant(TestTime2);
            Duration duration = time2 - time1;

            Assert.AreEqual(diff, duration.Ticks);
            Assert.AreEqual(TestTime2, (time1 + duration).Ticks);
            Assert.AreEqual(TestTime1, (time2 - duration).Ticks);
        }

        [Test]
        public void FromUtcNoSeconds()
        {
            Instant viaUtc = DateTimeZone.Utc.AtStrictly(new LocalDateTime(2008, 4, 3, 10, 35, 0)).ToInstant();
            Assert.AreEqual(viaUtc, Instant.FromUtc(2008, 4, 3, 10, 35));
        }

        [Test]
        public void FromUtcWithSeconds()
        {
            Instant viaUtc = DateTimeZone.Utc.AtStrictly(new LocalDateTime(2008, 4, 3, 10, 35, 23)).ToInstant();
            Assert.AreEqual(viaUtc, Instant.FromUtc(2008, 4, 3, 10, 35, 23));
        }

        [Test]
        public void InUtc()
        {
            ZonedDateTime viaInstant = Instant.FromUtc(2008, 4, 3, 10, 35, 23).InUtc();
            ZonedDateTime expected = DateTimeZone.Utc.AtStrictly(new LocalDateTime(2008, 4, 3, 10, 35, 23));
            Assert.AreEqual(expected, viaInstant);
        }

        [Test]
        public void InZone()
        {
            DateTimeZone london = DateTimeZoneProviders.Tzdb["Europe/London"];
            ZonedDateTime viaInstant = Instant.FromUtc(2008, 6, 10, 13, 16, 17).InZone(london);

            // London is UTC+1 in the Summer, so the above is 14:16:17 local.
            LocalDateTime local = new LocalDateTime(2008, 6, 10, 14, 16, 17);
            ZonedDateTime expected = london.AtStrictly(local);

            Assert.AreEqual(expected, viaInstant);
        }

        [Test]
        public void FromTicksSinceUnixEpoch()
        {
            Instant actual = Instant.FromTicksSinceUnixEpoch(12345L);
            Instant expected = new Instant(12345L);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromMillisecondsSinceUnixEpoch_Valid()
        {
            Instant actual = Instant.FromMillisecondsSinceUnixEpoch(12345L);
            Instant expected = new Instant(12345L * NodaConstants.TicksPerMillisecond);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromMillisecondsSinceUnixEpoch_TooLarge()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Instant.FromMillisecondsSinceUnixEpoch(long.MaxValue / 100));
        }

        [Test]
        public void FromMillisecondsSinceUnixEpoch_TooSmall()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Instant.FromMillisecondsSinceUnixEpoch(long.MinValue / 100));
        }

        [Test]
        public void FromSecondsSinceUnixEpoch_Valid()
        {
            Instant actual = Instant.FromSecondsSinceUnixEpoch(12345L);
            Instant expected = new Instant(12345L * NodaConstants.TicksPerSecond);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromSecondsSinceUnixEpoch_TooLarge()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Instant.FromSecondsSinceUnixEpoch(long.MaxValue / 1000000));
        }

        [Test]
        public void FromSecondsSinceUnixEpoch_TooSmall()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Instant.FromSecondsSinceUnixEpoch(long.MinValue / 1000000));
        }

        [Test]
        public void InZoneWithCalendar()
        {
            CalendarSystem copticCalendar = CalendarSystem.GetCopticCalendar(4);
            DateTimeZone london = DateTimeZoneProviders.Tzdb["Europe/London"];
            ZonedDateTime viaInstant = Instant.FromUtc(2004, 6, 9, 11, 10).InZone(london, copticCalendar);

            // Date taken from CopticCalendarSystemTest. Time will be 12:10 (London is UTC+1 in Summer)
            LocalDateTime local = new LocalDateTime(1720, 10, 2, 12, 10, 0, copticCalendar);
            ZonedDateTime expected = london.AtStrictly(local);
            Assert.AreEqual(expected, viaInstant);
        }

        [Test]
        public void Max()
        {
            Instant x = new Instant(100);
            Instant y = new Instant(200);
            Assert.AreEqual(y, Instant.Max(x, y));
            Assert.AreEqual(y, Instant.Max(y, x));
            Assert.AreEqual(x, Instant.Max(x, Instant.MinValue));
            Assert.AreEqual(x, Instant.Max(Instant.MinValue, x));
            Assert.AreEqual(Instant.MaxValue, Instant.Max(Instant.MaxValue, x));
            Assert.AreEqual(Instant.MaxValue, Instant.Max(x, Instant.MaxValue));
        }

        [Test]
        public void Min()
        {
            Instant x = new Instant(100);
            Instant y = new Instant(200);
            Assert.AreEqual(x, Instant.Min(x, y));
            Assert.AreEqual(x, Instant.Min(y, x));
            Assert.AreEqual(Instant.MinValue, Instant.Min(x, Instant.MinValue));
            Assert.AreEqual(Instant.MinValue, Instant.Min(Instant.MinValue, x));
            Assert.AreEqual(x, Instant.Min(Instant.MaxValue, x));
            Assert.AreEqual(x, Instant.Min(x, Instant.MaxValue));
        }

        [Test]
        public void ToDateTimeUtc()
        {
            Instant x = Instant.FromUtc(2011, 08, 18, 20, 53);
            DateTime expected = new DateTime(2011, 08, 18, 20, 53, 0, DateTimeKind.Utc);
            DateTime actual = x.ToDateTimeUtc();
            Assert.AreEqual(expected, actual);

            // Kind isn't checked by Equals...
            Assert.AreEqual(DateTimeKind.Utc, actual.Kind);
        }

        [Test]
        public void ToDateTimeOffset()
        {
            Instant x = Instant.FromUtc(2011, 08, 18, 20, 53);
            DateTimeOffset expected = new DateTimeOffset(2011, 08, 18, 20, 53, 0, TimeSpan.Zero);
            Assert.AreEqual(expected, x.ToDateTimeOffset());
        }

        [Test]
        public void FromDateTimeOffset()
        {
            DateTimeOffset dateTimeOffset = new DateTimeOffset(2011, 08, 18, 20, 53, 0, TimeSpan.FromHours(5));
            Instant expected = Instant.FromUtc(2011, 08, 18, 15, 53);
            Assert.AreEqual(expected, Instant.FromDateTimeOffset(dateTimeOffset));
        }

        [Test]
        public void FromDateTimeUtc_Invalid()
        {
            Assert.Throws<ArgumentException>(() => Instant.FromDateTimeUtc(new DateTime(2011, 08, 18, 20, 53, 0, DateTimeKind.Local)));
            Assert.Throws<ArgumentException>(() => Instant.FromDateTimeUtc(new DateTime(2011, 08, 18, 20, 53, 0, DateTimeKind.Unspecified)));
        }

        [Test]
        public void FromDateTimeUtc_Valid()
        {
            DateTime x = new DateTime(2011, 08, 18, 20, 53, 0, DateTimeKind.Utc);
            Instant expected = Instant.FromUtc(2011, 08, 18, 20, 53);
            Assert.AreEqual(expected, Instant.FromDateTimeUtc(x));
        }
    }
}
