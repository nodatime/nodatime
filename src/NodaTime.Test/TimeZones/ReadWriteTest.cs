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
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class ReadWriteTest
    {
        private static void TestCount(int expected)
        {
            var dio = new DtzIoHelper();
            dio.Writer.WriteCount(expected);
            var actual = dio.Reader.ReadCount();
            Assert.AreEqual(expected, actual);
        }

        private static void TestMilliseconds(int expected)
        {
            var dio = new DtzIoHelper();
            dio.Writer.WriteMilliseconds(expected);
            var actual = dio.Reader.ReadMilliseconds();
            Assert.AreEqual(expected, actual);
        }

        private static void TestTicks(long expected)
        {
            var dio = new DtzIoHelper();
            dio.Writer.WriteTicks(expected);
            var actual = dio.Reader.ReadTicks();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WriteRead_Milliseconds()
        {
            const int lowValue = -24 * NodaConstants.MillisecondsPerHour;
            const int millisecondsPerHalfHour = 30 * NodaConstants.MillisecondsPerMinute;
            for (int i = 0; i < 96; i++)
            {
                int value = lowValue + (i * millisecondsPerHalfHour);
                TestMilliseconds(value);
            }
            TestMilliseconds(NodaConstants.MillisecondsPerSecond * 23);
            TestMilliseconds(NodaConstants.MillisecondsPerSecond * -23);
            TestMilliseconds(Int32.MinValue);
            TestMilliseconds(Int32.MaxValue);
            TestMilliseconds(1);
            TestMilliseconds(-1);
        }

        [Test]
        public void WriteRead_Count()
        {
            for (int i = 0; i < 16; i++)
            {
                TestCount(i);
            }
            TestCount(0x0f);
            TestCount(0x10);
            TestCount(0x7f);
            TestCount(0x80);
            TestCount(0x81);
            TestCount(0x3fff);
            TestCount(0x4000);
            TestCount(0x4001);
            TestCount(0x1fffff);
            TestCount(0x200000);
            TestCount(0x200001);
            TestCount(-1);
            TestCount(Int32.MinValue);
            TestCount(Int32.MaxValue);
        }

        [Test]
        public void WriteRead_Ticks()
        {
            TestTicks(Int64.MaxValue);
            TestTicks(Int64.MinValue);
            TestTicks(3575232000000000L);
            TestTicks(3575231999999999L);
        }
    }
}