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
using System.IO;
using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    ///   Wrapper around a DateTimeZoneWriter/DateTimeZoneReader pair that reads whatever is
    ///   written to it.
    /// </summary>
    internal class DtzIoHelper
    {
        private readonly IoStream ioStream;
        private readonly IList<string> stringPool; 

        /// <summary>
        /// Initializes a new instance of the <see cref="DtzIoHelper" /> class.
        /// </summary>
        private DtzIoHelper(IList<string> stringPool)
        {
            ioStream = new IoStream();
            Reader = new DateTimeZoneReader(ioStream.GetReadStream(), stringPool);
            Writer = new DateTimeZoneWriter(ioStream.GetWriteStream(), stringPool);
            this.stringPool = stringPool;
        }

        internal static DtzIoHelper CreateNoStringPool()
        {
            return new DtzIoHelper(null);
        }

        internal static DtzIoHelper CreateWithStringPool()
        {
            return new DtzIoHelper(new List<string>());
        }

        /// <summary>
        ///   Gets the reader.
        /// </summary>
        /// <value>The reader.</value>
        private DateTimeZoneReader Reader { get; set; }

        /// <summary>
        ///   Gets the writer.
        /// </summary>
        /// <value>The writer.</value>
        private DateTimeZoneWriter Writer { get; set; }

        public void Reset()
        {
            ioStream.Reset();
            if (stringPool != null)
            {
                stringPool.Clear();
            }
        }

        public void TestBoolean(bool expected)
        {
            Reset();
            Writer.WriteBoolean(expected);
            var actual = Reader.ReadBoolean();
            Assert.AreEqual(expected, actual);
        }

        public void TestCount(int expected)
        {
            Reset();
            Writer.WriteCount(expected);
            var actual = Reader.ReadCount();
            Assert.AreEqual(expected, actual);
        }

        public void TestDictionary(IDictionary<string, string> expected)
        {
            Reset();
            Writer.WriteDictionary(expected);
            var actual = Reader.ReadDictionary();
            Assert.AreEqual(expected, actual);
        }

        public void TestInstant(Instant expected)
        {
            Reset();
            Writer.WriteInstant(expected);
            var actual = Reader.ReadInstant();
            Assert.AreEqual(expected, actual);
        }

        public void TestInt32(int expected)
        {
            Reset();
            Writer.WriteInt32(expected);
            var actual = Reader.ReadInt32();
            Assert.AreEqual(expected, actual);
        }

        public void TestLocalInstant(LocalInstant expected)
        {
            Reset();
            Writer.WriteLocalInstant(expected);
            var actual = Reader.ReadLocalInstant();
            Assert.AreEqual(expected, actual);
        }

        public void TestOffset(int testValue)
        {
            // Just ignore any invalid values we're asked to test
            if (testValue < Offset.MinValue.Milliseconds ||
                testValue > Offset.MaxValue.Milliseconds)
            {
                return;
            }
            Offset expected = Offset.FromMilliseconds(testValue);
            Reset();
            Writer.WriteOffset(expected);
            var actual = Reader.ReadOffset();
            Assert.AreEqual(expected, actual);
        }

        public void TestString(string expected)
        {
            Reset();
            Writer.WriteString(expected);
            var actual = Reader.ReadString();
            Assert.AreEqual(expected, actual);
        }

        public void TestTicks(long expected)
        {
            Reset();
            Writer.WriteTicks(expected);
            var actual = Reader.ReadTicks();
            Assert.AreEqual(expected, actual);
        }

        public void TestTimeZone(DateTimeZone expected)
        {
            Reset();
            Writer.WriteTimeZone(expected);
            var actual = Reader.ReadTimeZone(expected.Id);
            Assert.AreEqual(expected, actual);
        }

        public void TestZoneRecurrence(ZoneRecurrence expected)
        {
            Reset();
            expected.Write(Writer);
            var actual = ZoneRecurrence.Read(Reader);
            Assert.AreEqual(expected, actual);
        }

        public void TestZoneYearOffset(ZoneYearOffset expected)
        {
            Reset();
            expected.Write(Writer);
            var actual = ZoneYearOffset.Read(Reader);
            Assert.AreEqual(expected, actual);
        }
    }
}
