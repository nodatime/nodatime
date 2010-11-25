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
using System.IO;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    ///   Wrapper around a DateTimeZoneWriter/DateTimeZoneReader pair that reads whatever is
    ///   written to it.
    /// </summary>
    public class DtzIoHelper
    {
        private readonly IoStream ioStream;
        private readonly string name;

        public DtzIoHelper(string name) : this(name, stream => new DateTimeZoneWriter(stream), stream => new DateTimeZoneReader(stream))
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DtzIoHelper" /> class.
        /// </summary>
        public DtzIoHelper(string name, Func<Stream, IDateTimeZoneWriter> createWriter, Func<Stream, IDateTimeZoneReader> createReader)
        {
            this.name = name;
            ioStream = new IoStream();
            Reader = createReader(ioStream.GetReadStream());
            Writer = createWriter(ioStream.GetWriteStream());
        }

        /// <summary>
        ///   Gets the reader.
        /// </summary>
        /// <value>The reader.</value>
        private IDateTimeZoneReader Reader { get; set; }

        /// <summary>
        ///   Gets the writer.
        /// </summary>
        /// <value>The writer.</value>
        private IDateTimeZoneWriter Writer { get; set; }

        public void Reset()
        {
            ioStream.Reset();
        }

        public void TestBoolean(bool expected)
        {
            Reset();
            Writer.WriteBoolean(expected);
            var actual = Reader.ReadBoolean();
            Assert.AreEqual(expected, actual, name + " bool ");
        }

        public void TestCount(int expected)
        {
            Reset();
            Writer.WriteCount(expected);
            var actual = Reader.ReadCount();
            Assert.AreEqual(expected, actual, name + " Count ");
        }

        public void TestDictionary(IDictionary<string, string> expected)
        {
            Reset();
            Writer.WriteDictionary(expected);
            var actual = Reader.ReadDictionary();
            Assert.AreEqual(expected, actual, name + " Dictionary ");
        }

        public void TestEnum(int expected)
        {
            Reset();
            Writer.WriteEnum(expected);
            var actual = Reader.ReadEnum();
            Assert.AreEqual(expected, actual, name + " Enum ");
        }

        public void TestInstant(Instant expected)
        {
            Reset();
            Writer.WriteInstant(expected);
            var actual = Reader.ReadInstant();
            Assert.AreEqual(expected, actual, name + " Instant ");
        }

        public void TestInteger(int expected)
        {
            Reset();
            Writer.WriteInteger(expected);
            var actual = Reader.ReadInteger();
            Assert.AreEqual(expected, actual, name + " Integer ");
        }

        public void TestLocalInstant(LocalInstant expected)
        {
            Reset();
            Writer.WriteLocalInstant(expected);
            var actual = Reader.ReadLocalInstant();
            Assert.AreEqual(expected, actual, name + " LocalInstant ");
        }

        public void TestMilliseconds(int expected)
        {
            Reset();
            Writer.WriteMilliseconds(expected);
            var actual = Reader.ReadMilliseconds();
            Assert.AreEqual(expected, actual, name + " Milliseconds ");
        }

        public void TestOffset(Offset expected)
        {
            Reset();
            Writer.WriteOffset(expected);
            var actual = Reader.ReadOffset();
            Assert.AreEqual(expected, actual, name + " Offset ");
        }

        public void TestString(string expected)
        {
            Reset();
            Writer.WriteString(expected);
            var actual = Reader.ReadString();
            Assert.AreEqual(expected, actual, name + " string ");
        }

        public void TestTicks(long expected)
        {
            Reset();
            Writer.WriteTicks(expected);
            var actual = Reader.ReadTicks();
            Assert.AreEqual(expected, actual, name + " long ");
        }

        public void TestTimeZone(IDateTimeZone expected)
        {
            Reset();
            Writer.WriteTimeZone(expected);
            var actual = Reader.ReadTimeZone(expected.Id);
            Assert.AreEqual(expected, actual, name + " IDateTimeZone ");
        }

        public void TestZoneRecurrence(ZoneRecurrence expected)
        {
            Reset();
            expected.Write(Writer);
            var actual = ZoneRecurrence.Read(Reader);
            Assert.AreEqual(expected, actual, name + " ZoneRecurrence ");
        }

        public void TestZoneYearOffset(ZoneYearOffset expected)
        {
            Reset();
            expected.Write(Writer);
            var actual = ZoneYearOffset.Read(Reader);
            Assert.AreEqual(expected, actual, name + " ZoneYearOffset ");
        }
    }
}
