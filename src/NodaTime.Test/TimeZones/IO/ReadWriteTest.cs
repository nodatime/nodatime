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
    [TestFixture(typeof(DateTimeZoneReader), typeof(DateTimeZoneWriter))]
    [TestFixture(typeof(LegacyDateTimeZoneReader), typeof(LegacyDateTimeZoneWriter))]
    internal class ReadWriteTest<TReader, TWriter>
        where TReader : IDateTimeZoneReader
        where TWriter : IDateTimeZoneWriter
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

        private static void RunTests_Ticks(Action<long> tester)
        {
            tester(Int64.MaxValue);
            tester(Int64.MinValue);
            tester(3575232000000000L);
            tester(3575231999999999L);
            tester(Instant.MinValue.Ticks);
            tester(Instant.MaxValue.Ticks);
            tester(NodaConstants.UnixEpoch.Ticks);
            for (long i = DateTimeZoneWriter.MinHalfHours; i <= DateTimeZoneWriter.MaxHalfHours; i++)
            {
                long value = i * 30 * NodaConstants.TicksPerMinute;
                tester(value);
            }
            const long minuteDiff = (DateTimeZoneWriter.MaxMinutes - DateTimeZoneWriter.MinMinutes) / 1000;
            for (long i = DateTimeZoneWriter.MinMinutes; i <= DateTimeZoneWriter.MaxMinutes; i += minuteDiff)
            {
                long value = i * NodaConstants.TicksPerMinute;
                tester(value);
            }
            const long secondDiff = (DateTimeZoneWriter.MaxSeconds - DateTimeZoneWriter.MinSeconds) / 1000;
            for (long i = DateTimeZoneWriter.MinSeconds; i <= DateTimeZoneWriter.MinSeconds; i += secondDiff)
            {
                tester(i * NodaConstants.TicksPerSecond);
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
            RunTests_Ticks(value => Dio.TestInstant((new Instant(value))));
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
