// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class LocalInstantTest
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

        private LocalInstant one = new LocalInstant(1L);
        private readonly LocalInstant onePrime = new LocalInstant(1L);
        private LocalInstant negativeOne = new LocalInstant(-1L);
        private LocalInstant threeMillion = new LocalInstant(3000000L);
        private LocalInstant negativeFiftyMillion = new LocalInstant(-50000000L);

        private readonly Duration durationNegativeEpsilon = Duration.FromTicks(-1L);
        private readonly Offset offsetOneHour = Offset.FromHours(1);

        [Test]
        public void TestLocalInstantOperators()
        {
            const long diff = TestTime2 - TestTime1;

            var time1 = new LocalInstant(TestTime1);
            var time2 = new LocalInstant(TestTime2);
            Duration duration = Duration.FromTicks(diff);

            Assert.AreEqual(diff, duration.Ticks);
            Assert.AreEqual(TestTime2, (time1 + duration).Ticks);
            Assert.AreEqual(TestTime1, (time2 - duration).Ticks);
        }

        [Test]
        public void FromDateTime()
        {
            LocalInstant expected = new LocalInstant(2011, 08, 18, 20, 53);
            foreach (DateTimeKind kind in Enum.GetValues(typeof(DateTimeKind)))
            {
                DateTime x = new DateTime(2011, 08, 18, 20, 53, 0, kind);
                LocalInstant actual = LocalInstant.FromDateTime(x);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        [TestCase("0001-01-01")]
        [TestCase("0001-12-31")]
        [TestCase("1969-12-31")]
        [TestCase("1970-01-01")]
        [TestCase("1976-06-19")]
        [TestCase("9999-01-01")]
        public void ToIsoDate(string dateText)
        {
            LocalDate date = LocalDatePattern.IsoPattern.Parse(dateText).Value;
            Assert.AreEqual(date, date.AtMidnight().ToLocalInstant().ToIsoDate());
            Assert.AreEqual(date, date.At(new LocalTime(23, 59, 59)).ToLocalInstant().ToIsoDate());
        }
    }
}
