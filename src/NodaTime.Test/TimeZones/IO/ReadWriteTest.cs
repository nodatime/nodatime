// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using NodaTime.TimeZones.IO;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones.IO
{
    [TestFixture]
    public class ReadWriteTest
    {
        /// <summary>
        /// Returns the <see cref="DtzIoHelper" /> to use for testing against.
        /// </summary>
        private DtzIoHelper Dio { get; set; }

        [SetUp]
        public void SetUp()
        {
            Dio = DtzIoHelper.CreateNoStringPool();
        }

        private static void RunTests_Integers(Action<int> tester)
        {
            tester(0);
            tester(Int32.MinValue);
            tester(Int32.MaxValue);
            int value = 2;
            for (int i = 0; i < 32; i++)
            {
                tester(value);
                value <<= 1;
            }
            value = -1;
            for (int i = 0; i < 32; i++)
            {
                tester(value);
                value <<= 1;
            }
        }

        [Test]
        public void Test_Count()
        {
            for (int i = 0; i < 16; i++)
            {
                Dio.TestCount(i);
            }
            Dio.TestCount(0x0f);
            Dio.TestCount(0x10);
            Dio.TestCount(0x7f);
            Dio.TestCount(0x80);
            Dio.TestCount(0x81);
            Dio.TestCount(0x3fff);
            Dio.TestCount(0x4000);
            Dio.TestCount(0x4001);
            Dio.TestCount(0x1fffff);
            Dio.TestCount(0x200000);
            Dio.TestCount(0x200001);
            Dio.TestCount(Int32.MaxValue);
            Dio.TestCount(0, new byte[] { 0x00 });
            Dio.TestCount(1, new byte[] { 0x01 });
            Dio.TestCount(127, new byte[] { 0x7f });
            Dio.TestCount(128, new byte[] { 0x80, 0x01 });
            Dio.TestCount(300, new byte[] { 0xac, 0x02 });
        }

        [Test]
        public void Test_SignedCount()
        {
            for (int i = -16; i < 16; i++)
            {
                Dio.TestSignedCount(i);
            }
            Dio.TestSignedCount(0x4000000);
            Dio.TestSignedCount(-0x4000000);
            Dio.TestSignedCount(Int32.MinValue);
            Dio.TestSignedCount(Int32.MinValue + 1);
            Dio.TestSignedCount(Int32.MaxValue - 1);
            Dio.TestSignedCount(Int32.MaxValue);
            Dio.TestSignedCount(0, new byte[] { 0x00 });
            Dio.TestSignedCount(-1, new byte[] { 0x01 });
            Dio.TestSignedCount(1, new byte[] { 0x02 });
            Dio.TestSignedCount(-2, new byte[] { 0x03 });
            Dio.TestSignedCount(64, new byte[] { 0x80, 0x01 });
            Dio.TestSignedCount(-65, new byte[] { 0x81, 0x01 });
            Dio.TestSignedCount(128, new byte[] { 0x80, 0x02 });
        }

        [Test]
        public void Test_Dictionary()
        {
            var expected = new Dictionary<string, string>();
            Dio.TestDictionary(expected);

            expected.Add("Able", "able");
            Dio.TestDictionary(expected);

            expected.Add("Baker", "baker");
            expected.Add("Charlie", "charlie");
            expected.Add("Delta", "delta");
            Dio.TestDictionary(expected);
        }

        [Test]
        public void Test_ZoneIntervalTransition()
        {
            Dio.TestZoneIntervalTransition(null, Instant.MinValue);
            Dio.TestZoneIntervalTransition(null, Instant.MaxValue);
            Dio.TestZoneIntervalTransition(null, Instant.MinValue.PlusTicks(1));
            Dio.TestZoneIntervalTransition(null, Instant.MaxValue.PlusTicks(-1));

            // Encoding as hours-since-previous.
            Instant previous = Instant.FromUtc(1990, 1, 1, 11, 30);  // arbitrary
            Dio.TestZoneIntervalTransition(previous, previous);
            Dio.TestZoneIntervalTransition(previous, previous + Duration.FromHours(
                DateTimeZoneWriter.ZoneIntervalConstants.MinValueForHoursSincePrevious));
            Dio.TestZoneIntervalTransition(previous, previous + Duration.FromHours(
                DateTimeZoneWriter.ZoneIntervalConstants.MinValueForHoursSincePrevious - 1));  // too soon
            Dio.TestZoneIntervalTransition(previous, previous + Duration.FromHours(1));  // too soon
            Dio.TestZoneIntervalTransition(previous, previous + Duration.FromHours(
                DateTimeZoneWriter.ZoneIntervalConstants.MinValueForMinutesSinceEpoch - 1));  // maximum hours
            Dio.TestZoneIntervalTransition(previous, previous + Duration.FromHours(
                DateTimeZoneWriter.ZoneIntervalConstants.MinValueForMinutesSinceEpoch));  // out of range
            // A large difference from the previous transition.
            Dio.TestZoneIntervalTransition(Instant.MinValue.PlusTicks(1), Instant.MaxValue.PlusTicks(-1));

            // Encoding as minutes-since-epoch.
            Instant epoch = DateTimeZoneWriter.ZoneIntervalConstants.EpochForMinutesSinceEpoch;
            Dio.TestZoneIntervalTransition(null, epoch);
            Dio.TestZoneIntervalTransition(null, epoch +
                Duration.FromMinutes(DateTimeZoneWriter.ZoneIntervalConstants.MinValueForMinutesSinceEpoch));
            Dio.TestZoneIntervalTransition(null, epoch + Duration.FromMinutes(int.MaxValue));  // maximum minutes

            // Out of range cases, or not a multiple of minutes since the epoch.
            Dio.TestZoneIntervalTransition(null, epoch + Duration.FromHours(1));  // too soon
            Dio.TestZoneIntervalTransition(null, epoch + Duration.FromMinutes(1));  // too soon
            Dio.TestZoneIntervalTransition(null, epoch + Duration.FromSeconds(1));
            Dio.TestZoneIntervalTransition(null, epoch + Duration.FromMilliseconds(1));
            Dio.TestZoneIntervalTransition(null, epoch - Duration.FromHours(1));
            Dio.TestZoneIntervalTransition(null, epoch + Duration.FromMinutes((long) int.MaxValue + 1));

            // Example from Pacific/Auckland which failed at one time, and a similar one with seconds.
            Dio.TestZoneIntervalTransition(null, Instant.FromUtc(1927, 11, 5, 14, 30));
            Dio.TestZoneIntervalTransition(null, Instant.FromUtc(1927, 11, 5, 14, 30, 5));
        }

        [Test]
        public void Test_Offset()
        {
            Dio.TestOffset(Offset.MinValue);
            Dio.TestOffset(Offset.MaxValue);
            Dio.TestOffset(Offset.FromHours(0));
            Dio.TestOffset(Offset.FromHours(5));
            Dio.TestOffset(Offset.FromHours(-5));
            Dio.TestOffset(Offset.FromHoursAndMinutes(5, 15));
            Dio.TestOffset(Offset.FromHoursAndMinutes(5, 30));
            Dio.TestOffset(Offset.FromHoursAndMinutes(5, 45));
            Dio.TestOffset(Offset.FromHoursAndMinutes(-5, -15));
            Dio.TestOffset(Offset.FromHoursAndMinutes(-5, -30));
            Dio.TestOffset(Offset.FromHoursAndMinutes(-5, -45));
            Dio.TestOffset(Offset.FromSeconds(1));
            Dio.TestOffset(Offset.FromSeconds(-1));
            Dio.TestOffset(Offset.FromSeconds(1000));
            Dio.TestOffset(Offset.FromSeconds(-1000));
        }

        [Test]
        public void Test_String_NoPool()
        {
            Dio.TestString("");
            Dio.TestString("This is a test string");
        }

        [Test]
        public void Test_String_WithPool()
        {
            Dio = DtzIoHelper.CreateWithStringPool();
            Dio.TestString("");
            Dio.TestString("This is a test string");
        }
    }
}
