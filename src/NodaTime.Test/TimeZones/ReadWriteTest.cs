#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    public abstract class ReadWriteTest
    {
        /*
        #region Setup/Teardown
        [SetUp]
        public void SetUp()
        {
            dioStandard = new DtzIoHelper("standard", (stream) => new DateTimeZoneWriter(stream), (stream) => new DateTimeZoneReader(stream));
            dioCompress = new DtzIoHelper("compress", (stream) => new DateTimeZoneCompressionWriter(stream), (stream) => new DateTimeZoneCompressionReader(stream));
        }
        #endregion

        private DtzIoHelper dioStandard;
        private DtzIoHelper dioCompress;
        */

        /// <summary>
        ///   Returns the <see cref = "DtzIoHelper" /> to use for testing against.
        /// </summary>
        internal DtzIoHelper Dio { get; set; }

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

        private static void RunTests_Milliseconds(Action<int> tester)
        {
            tester(Int32.MinValue);
            tester(Int32.MaxValue);
            tester(0);
            tester(1);
            tester(-1);
            for (int i = DateTimeZoneCompressionWriter.MinMillisHalfHours; i <= DateTimeZoneCompressionWriter.MaxMillisHalfHours; i++)
            {
                int value = i * 30 * NodaConstants.MillisecondsPerMinute;
                tester(value);
            }

            const int secondDelta = (DateTimeZoneCompressionWriter.MaxMillisSeconds - DateTimeZoneCompressionWriter.MinMillisSeconds) / 1000;
            for (int i = DateTimeZoneCompressionWriter.MinMillisSeconds; i <= DateTimeZoneCompressionWriter.MaxMillisSeconds; i += secondDelta)
            {
                int value = i * NodaConstants.MillisecondsPerSecond;
                tester(value);
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
            tester(Instant.UnixEpoch.Ticks);
            for (long i = DateTimeZoneCompressionWriter.MinHalfHours; i <= DateTimeZoneCompressionWriter.MaxHalfHours; i++)
            {
                long value = i * 30 * NodaConstants.TicksPerMinute;
                tester(value);
            }
            const long minuteDiff = (DateTimeZoneCompressionWriter.MaxMinutes - DateTimeZoneCompressionWriter.MinMinutes) / 1000;
            for (long i = DateTimeZoneCompressionWriter.MinMinutes; i <= DateTimeZoneCompressionWriter.MaxMinutes; i += minuteDiff)
            {
                long value = i * NodaConstants.TicksPerMinute;
                tester(value);
            }
            const long secondDiff = (DateTimeZoneCompressionWriter.MaxSeconds - DateTimeZoneCompressionWriter.MinSeconds) / 1000;
            for (long i = DateTimeZoneCompressionWriter.MinSeconds; i <= DateTimeZoneCompressionWriter.MinSeconds; i += secondDiff)
            {
                tester(i * NodaConstants.TicksPerSecond);
            }
        }

        [Test]
        public void Test_Boolean()
        {
            Dio.TestBoolean(true);
            Dio.TestBoolean(false);
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
            Dio.TestCount(-1);
            Dio.TestCount(Int32.MinValue);
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
        public void Test_Enum()
        {
            RunTests_Integers(Dio.TestEnum);
        }

        [Test]
        public void Test_Instant()
        {
            RunTests_Ticks(value => Dio.TestInstant((new Instant(value))));
        }

        [Test]
        public void Test_Integer()
        {
            RunTests_Integers(Dio.TestInteger);
        }

        [Test]
        public void Test_LocalInstant()
        {
            RunTests_Ticks(value => Dio.TestLocalInstant((new LocalInstant(value))));
        }

        [Test]
        public void Test_Milliseconds()
        {
            RunTests_Milliseconds(Dio.TestMilliseconds);
        }

        [Test]
        public void Test_Offset()
        {
            RunTests_Milliseconds(value => Dio.TestOffset((new Offset(value))));
        }

        public void Test_String()
        {
            Dio.TestString(null);
            Dio.TestString("");
            Dio.TestString("This is a test string");
        }

        [Test]
        public void Test_Ticks()
        {
            RunTests_Ticks(Dio.TestTicks);
        }
    }
}
