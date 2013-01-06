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
using System.Collections.Generic;
using NUnit.Framework;
using NodaTime.TimeZones.IO;

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
        public void Test_Instant()
        {
            Dio.TestInstant(Instant.MinValue);
            Dio.TestInstant(Instant.MaxValue);
            Dio.TestInstant(Instant.MinValue.PlusTicks(1));
            Dio.TestInstant(Instant.MaxValue.PlusTicks(-1));
            Instant epoch = DateTimeZoneWriter.InstantConstants.Epoch;
            // Valid optimized cases
            Dio.TestInstant(epoch);
            Dio.TestInstant(epoch + Duration.FromHours(1));
            Dio.TestInstant(epoch + Duration.FromMinutes(1));
            Dio.TestInstant(epoch + Duration.FromSeconds(1));
            // Example from Pacific/Auckland which failed, and a similar one with seconds.
            Dio.TestInstant(Instant.FromUtc(1927, 11, 5, 14, 30));
            Dio.TestInstant(Instant.FromUtc(1927, 11, 5, 14, 30, 5));
            // Out of range cases, or not a multiple of hours, minutes or seconds
            Dio.TestInstant(epoch + Duration.FromMilliseconds(1));
            Dio.TestInstant(epoch - Duration.FromHours(1));
            Dio.TestInstant(epoch + Duration.FromHours(DateTimeZoneWriter.InstantConstants.MaxHours + 1));
            Dio.TestInstant(epoch + Duration.FromMinutes(DateTimeZoneWriter.InstantConstants.MaxMinutes + 1));
            Dio.TestInstant(epoch + Duration.FromSeconds(DateTimeZoneWriter.InstantConstants.MaxSeconds + 1));
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
            Dio.TestOffset(Offset.FromMilliseconds(1));
            Dio.TestOffset(Offset.FromMilliseconds(-1));
            Dio.TestOffset(Offset.FromMilliseconds(1000));
            Dio.TestOffset(Offset.FromMilliseconds(-1000));
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
