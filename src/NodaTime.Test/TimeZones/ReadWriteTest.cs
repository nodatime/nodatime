#region Copyright and license information

// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
using NodaTime.TimeZones;
using NUnit.Framework;
using System.IO;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public partial class ReadWriteTest
    {
        [Test]
        public void ReadWriteNumber_0()
        {
            for (int i = 0; i < 16; i++)
            {
                TestValue(i);
            }
            TestValue(0x0f);
            TestValue(0x10);
            TestValue(0x7f);
            TestValue(0x80);
            TestValue(0x81);
            TestValue(0x3fff);
            TestValue(0x4000);
            TestValue(0x4001);
            TestValue(0x1fffff);
            TestValue(0x200000);
            TestValue(0x200001);
            TestValue(-1);
            TestValue(Int32.MinValue);
            TestValue(Int32.MaxValue);
        }

        private void TestValue(int expected)
        {
            MemoryStream stream = new MemoryStream();
            DateTimeZoneWriter writer = new DateTimeZoneWriter(stream);
            writer.WriteNumber(expected);
            stream.Seek(0, SeekOrigin.Begin);
            DateTimeZoneReader reader = new DateTimeZoneReader(stream);
            int actual = reader.ReadNumber();
            Assert.AreEqual(expected, actual);
        }

    }
}
