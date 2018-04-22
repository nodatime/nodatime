// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using NodaTime.TimeZones.IO;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones.IO
{
    public class ReadWriteTest
    {        
        [Test]
        public void Count()
        {
            var dio = DtzIoHelper.CreateNoStringPool();

            for (int i = 0; i < 16; i++)
            {
                dio.TestCount(i);
            }
            dio.TestCount(0x0f);
            dio.TestCount(0x10);
            dio.TestCount(0x7f);
            dio.TestCount(0x80);
            dio.TestCount(0x81);
            dio.TestCount(0x3fff);
            dio.TestCount(0x4000);
            dio.TestCount(0x4001);
            dio.TestCount(0x1fffff);
            dio.TestCount(0x200000);
            dio.TestCount(0x200001);
            dio.TestCount(Int32.MaxValue);
            dio.TestCount(0, new byte[] { 0x00 });
            dio.TestCount(1, new byte[] { 0x01 });
            dio.TestCount(127, new byte[] { 0x7f });
            dio.TestCount(128, new byte[] { 0x80, 0x01 });
            dio.TestCount(300, new byte[] { 0xac, 0x02 });
        }

        [Test]
        public void SignedCount()
        {
            var dio = DtzIoHelper.CreateNoStringPool();
            for (int i = -16; i < 16; i++)
            {
                dio.TestSignedCount(i);
            }
            dio.TestSignedCount(0x4000000);
            dio.TestSignedCount(-0x4000000);
            dio.TestSignedCount(Int32.MinValue);
            dio.TestSignedCount(Int32.MinValue + 1);
            dio.TestSignedCount(Int32.MaxValue - 1);
            dio.TestSignedCount(Int32.MaxValue);
            dio.TestSignedCount(0, new byte[] { 0x00 });
            dio.TestSignedCount(-1, new byte[] { 0x01 });
            dio.TestSignedCount(1, new byte[] { 0x02 });
            dio.TestSignedCount(-2, new byte[] { 0x03 });
            dio.TestSignedCount(64, new byte[] { 0x80, 0x01 });
            dio.TestSignedCount(-65, new byte[] { 0x81, 0x01 });
            dio.TestSignedCount(128, new byte[] { 0x80, 0x02 });
        }

        [Test]
        public void Test_Dictionary()
        {
            var dio = DtzIoHelper.CreateNoStringPool();
            var expected = new Dictionary<string, string>();
            dio.TestDictionary(expected);

            expected.Add("Able", "able");
            dio.TestDictionary(expected);

            expected.Add("Baker", "baker");
            expected.Add("Charlie", "charlie");
            expected.Add("Delta", "delta");
            dio.TestDictionary(expected);
        }

        [Test]
        public void Test_ZoneIntervalTransition()
        {
            var dio = DtzIoHelper.CreateNoStringPool();
            dio.TestZoneIntervalTransition(null, Instant.BeforeMinValue);
            dio.TestZoneIntervalTransition(null, Instant.MinValue);
            // No test for Instant.MaxValue, as it's not on a tick boundary.
            dio.TestZoneIntervalTransition(null, Instant.AfterMaxValue);

            dio.TestZoneIntervalTransition(null, Instant.MinValue.PlusTicks(1));
            // The ZoneIntervalTransition has precision to the tick (with no real need to change that).
            // Round to the tick just lower than Instant.MaxValue...
            Instant tickBeforeMaxInstant = Instant.FromUnixTimeTicks(Instant.MaxValue.ToUnixTimeTicks());
            dio.TestZoneIntervalTransition(null, tickBeforeMaxInstant);

            // Encoding as hours-since-previous.
            Instant previous = Instant.FromUtc(1990, 1, 1, 11, 30);  // arbitrary
            dio.TestZoneIntervalTransition(previous, previous);
            dio.TestZoneIntervalTransition(previous, previous + Duration.FromHours(
                DateTimeZoneWriter.ZoneIntervalConstants.MinValueForHoursSincePrevious));
            dio.TestZoneIntervalTransition(previous, previous + Duration.FromHours(
                DateTimeZoneWriter.ZoneIntervalConstants.MinValueForHoursSincePrevious - 1));  // too soon
            dio.TestZoneIntervalTransition(previous, previous + Duration.FromHours(1));  // too soon
            dio.TestZoneIntervalTransition(previous, previous + Duration.FromHours(
                DateTimeZoneWriter.ZoneIntervalConstants.MinValueForMinutesSinceEpoch - 1));  // maximum hours
            dio.TestZoneIntervalTransition(previous, previous + Duration.FromHours(
                DateTimeZoneWriter.ZoneIntervalConstants.MinValueForMinutesSinceEpoch));  // out of range
            // A large difference from the previous transition.
            dio.TestZoneIntervalTransition(Instant.MinValue.PlusTicks(1), tickBeforeMaxInstant);

            // Encoding as minutes-since-epoch.
            Instant epoch = DateTimeZoneWriter.ZoneIntervalConstants.EpochForMinutesSinceEpoch;
            dio.TestZoneIntervalTransition(null, epoch);
            dio.TestZoneIntervalTransition(null, epoch +
                Duration.FromMinutes(DateTimeZoneWriter.ZoneIntervalConstants.MinValueForMinutesSinceEpoch));
            dio.TestZoneIntervalTransition(null, epoch + Duration.FromMinutes(int.MaxValue));  // maximum minutes

            // Out of range cases, or not a multiple of minutes since the epoch.
            dio.TestZoneIntervalTransition(null, epoch + Duration.FromHours(1));  // too soon
            dio.TestZoneIntervalTransition(null, epoch + Duration.FromMinutes(1));  // too soon
            dio.TestZoneIntervalTransition(null, epoch + Duration.FromSeconds(1));
            dio.TestZoneIntervalTransition(null, epoch + Duration.FromMilliseconds(1));
            dio.TestZoneIntervalTransition(null, epoch - Duration.FromHours(1));
            dio.TestZoneIntervalTransition(null, epoch + Duration.FromMinutes((long) int.MaxValue + 1));

            // Example from Pacific/Auckland which failed at one time, and a similar one with seconds.
            dio.TestZoneIntervalTransition(null, Instant.FromUtc(1927, 11, 5, 14, 30));
            dio.TestZoneIntervalTransition(null, Instant.FromUtc(1927, 11, 5, 14, 30, 5));
        }

        [Test]
        public void Test_Offset()
        {
            var dio = DtzIoHelper.CreateNoStringPool();
            dio.TestOffset(Offset.MinValue);
            dio.TestOffset(Offset.MaxValue);
            dio.TestOffset(Offset.FromHours(0));
            dio.TestOffset(Offset.FromHours(5));
            dio.TestOffset(Offset.FromHours(-5));
            dio.TestOffset(Offset.FromHoursAndMinutes(5, 15));
            dio.TestOffset(Offset.FromHoursAndMinutes(5, 30));
            dio.TestOffset(Offset.FromHoursAndMinutes(5, 45));
            dio.TestOffset(Offset.FromHoursAndMinutes(-5, -15));
            dio.TestOffset(Offset.FromHoursAndMinutes(-5, -30));
            dio.TestOffset(Offset.FromHoursAndMinutes(-5, -45));
            dio.TestOffset(Offset.FromSeconds(1));
            dio.TestOffset(Offset.FromSeconds(-1));
            dio.TestOffset(Offset.FromSeconds(1000));
            dio.TestOffset(Offset.FromSeconds(-1000));
        }

        [Test]
        public void String_NoPool()
        {
            var dio = DtzIoHelper.CreateNoStringPool();
            dio.TestString("");
            dio.TestString("This is a test string");
        }

        [Test]
        public void String_WithPool()
        {
            var dio = DtzIoHelper.CreateWithStringPool();
            dio.TestString("");
            dio.TestString("This is a test string");
        }

        [Test]
        public void ReadByte_AfterHasMoreData()
        {
            var dio = DtzIoHelper.CreateNoStringPool();
            dio.Writer.WriteByte(5);

            Assert.IsTrue(dio.Reader.HasMoreData);
            Assert.AreEqual((byte) 5, dio.Reader.ReadByte());
        }

        [Test]
        public void ReadString_AfterHasMoreData()
        {
            var dio = DtzIoHelper.CreateNoStringPool();
            dio.Writer.WriteString("foo");

            Assert.IsTrue(dio.Reader.HasMoreData);
            Assert.AreEqual("foo", dio.Reader.ReadString());
        }
    }
}
