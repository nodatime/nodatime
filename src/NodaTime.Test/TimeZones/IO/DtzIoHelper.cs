// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NodaTime.TimeZones;
using NodaTime.TimeZones.IO;

namespace NodaTime.Test.TimeZones.IO
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
        private IDateTimeZoneReader Reader { get; set; }

        /// <summary>
        ///   Gets the writer.
        /// </summary>
        /// <value>The writer.</value>
        private IDateTimeZoneWriter Writer { get; set; }

        public void Reset()
        {
            ioStream.Reset();
            if (stringPool != null)
            {
                stringPool.Clear();
            }
        }

        public void TestCount(int expected)
        {
            Reset();
            Writer.WriteCount(expected);
            var actual = Reader.ReadCount();
            Assert.AreEqual(expected, actual);
            ioStream.AssertEndOfStream();
        }

        public void TestCount(int value, byte[] expected)
        {
            Reset();
            Writer.WriteCount(value);
            ioStream.AssertUnreadContents(expected);
            ioStream.AssertEndOfStream();
        }

        public void TestSignedCount(int expected)
        {
            Reset();
            Writer.WriteSignedCount(expected);
            var actual = Reader.ReadSignedCount();
            Assert.AreEqual(expected, actual);
            ioStream.AssertEndOfStream();
        }

        public void TestSignedCount(int value, byte[] expected)
        {
            Reset();
            Writer.WriteSignedCount(value);
            ioStream.AssertUnreadContents(expected);
            ioStream.AssertEndOfStream();
        }

        public void TestDictionary(IDictionary<string, string> expected)
        {
            Reset();
            Writer.WriteDictionary(expected);
            var actual = Reader.ReadDictionary();
            Assert.AreEqual(expected, actual);
            ioStream.AssertEndOfStream();
        }

        public void TestZoneIntervalTransition(Instant? previous, Instant expected)
        {
            Reset();
            Writer.WriteZoneIntervalTransition(previous, expected);
            var actual = Reader.ReadZoneIntervalTransition(previous);
            Assert.AreEqual(expected, actual);
            ioStream.AssertEndOfStream();
        }

        public void TestOffset(Offset offset)
        {
            Reset();
            Writer.WriteOffset(offset);
            var actual = Reader.ReadOffset();
            Assert.AreEqual(offset, actual);
            ioStream.AssertEndOfStream();
        }

        public void TestString(string expected)
        {
            Reset();
            Writer.WriteString(expected);
            var actual = Reader.ReadString();
            Assert.AreEqual(expected, actual);
            ioStream.AssertEndOfStream();
        }

        public void TestTimeZone(DateTimeZone expected)
        {
            Reset();
            Writer.WriteTimeZone(expected);
            var actual = Reader.ReadTimeZone(expected.Id);
            Assert.AreEqual(expected, actual);
            ioStream.AssertEndOfStream();
        }

        public void TestZoneRecurrence(ZoneRecurrence expected)
        {
            Reset();
            expected.Write(Writer);
            var actual = ZoneRecurrence.Read(Reader);
            Assert.AreEqual(expected, actual);
            ioStream.AssertEndOfStream();
        }

        public void TestZoneYearOffset(ZoneYearOffset expected)
        {
            Reset();
            expected.Write(Writer);
            var actual = ZoneYearOffset.Read(Reader);
            Assert.AreEqual(expected, actual);
            ioStream.AssertEndOfStream();
        }
    }
}
