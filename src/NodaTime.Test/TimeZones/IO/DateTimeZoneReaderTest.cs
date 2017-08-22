// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones.IO;
using NodaTime.Utility;
using NUnit.Framework;
using System.IO;

namespace NodaTime.Test.TimeZones.IO
{
    // Tests for DateTimeZoneReader behaviour that isn't already covered by ReadWriteTest.
    public class DateTimeZoneReaderTest
    {
        [Test]
        public void HasMoreData_Buffers()
        {
            var stream = new MemoryStream(new byte[] { 1, 2 });
            var reader = new DateTimeZoneReader(stream, null);

            // HasMoreData reads a byte and buffers it
            Assert.AreEqual(0, stream.Position);
            Assert.IsTrue(reader.HasMoreData);
            Assert.AreEqual(1, stream.Position);
            Assert.IsTrue(reader.HasMoreData);
            Assert.AreEqual(1, stream.Position);

            // Consume the buffered byte
            Assert.AreEqual((byte) 1, reader.ReadByte());
            Assert.AreEqual(1, stream.Position);

            // HasMoreData reads the next byte
            Assert.IsTrue(reader.HasMoreData);
            Assert.AreEqual(2, stream.Position);
            Assert.IsTrue(reader.HasMoreData);
            Assert.AreEqual(2, stream.Position);

            // Consume the buffered byte
            Assert.AreEqual((byte)2, reader.ReadByte());
            Assert.AreEqual(2, stream.Position);

            // No more data
            Assert.IsFalse(reader.HasMoreData);
        }

        [Test]
        public void ReadCount_OutOfRange()
        {
            // Int32.MaxValue + 1 (as a uint) is 10000000_00000000_00000000_00000000
            // So only bit 31 is set.
            // Divided into 7 bit chunks (reverse order), with top bit set for continuation, this is:
            // 10000000 - bits 0-6
            // 10000000 - bits 7-13
            // 10000000 - bits 14-20
            // 10000000 - bits 21-27
            // 00001000 - bits 28-34
            byte[] data = new byte[] { 0x80, 0x80, 0x80, 0x80, 0b0000_1000 };
            var stream = new MemoryStream(data);
            var reader = new DateTimeZoneReader(stream, null);
            Assert.Throws<InvalidNodaDataException>(() => reader.ReadCount());
        }

        [Test]
        public void ReadMilliseconds_InvalidFlag()
        {
            // Top 3 bits are the flag. Valid flag values are 0b100 (minutes), 0b101 (seconds)  and 0b110 (milliseconds)
            byte[] data = new byte[] { 0xe0, 0, 0, 0, 0, 0 }; // Invalid flag (followed by 0s to check that it's not just out of data)
            var stream = new MemoryStream(data);
            var reader = new DateTimeZoneReader(stream, null);
            Assert.Throws<InvalidNodaDataException>(() => reader.ReadMilliseconds());
        }

        [Test]
        public void ReadZoneIntervalTransition_InvalidMarkerValue()
        {
            byte[] data = new byte[] { 4, 0, 0, 0, 0, 0 }; // Marker value of 4 (followed by 0s to check that it's not just out of data)
            var stream = new MemoryStream(data);
            var reader = new DateTimeZoneReader(stream, null);
            Assert.Throws<InvalidNodaDataException>(() => reader.ReadZoneIntervalTransition(previous: NodaConstants.UnixEpoch));
        }

        [Test]
        public void ReadZoneIntervalTransition_NoPreviousValue()
        {
            // Count value between 1 << 7 and 1 << 21 (followed by 0s to check that it's not just out of data)
            byte[] data = new byte[] { 0xff, 0x7f, 0, 0, 0, 0 };
            var stream = new MemoryStream(data);
            var reader = new DateTimeZoneReader(stream, null);
            Assert.Throws<InvalidNodaDataException>(() => reader.ReadZoneIntervalTransition(previous: null));

            // Validate that when we *do* provide a previous value, it doesn't throw
            stream.Position = 0;
            reader = new DateTimeZoneReader(stream, null);
            reader.ReadZoneIntervalTransition(previous: NodaConstants.UnixEpoch);
        }

        [Test]
        public void ReadString_NotEnoughData()
        {
            // We say there are 5 bytes, but there are only 4 left...
            byte[] data = new byte[] { 0x05, 0x40, 0x40, 0x40, 0x40 };
            var stream = new MemoryStream(data);
            var reader = new DateTimeZoneReader(stream, null);
            Assert.Throws<InvalidNodaDataException>(() => reader.ReadString());
        }

        [Test]
        public void ReadByte_NotEnoughData()
        {
            byte[] data = new byte[] { 0x05 };
            var stream = new MemoryStream(data);
            var reader = new DateTimeZoneReader(stream, null);
            // Just check we can read the first byte, then fail on the second
            Assert.AreEqual((byte) 5, reader.ReadByte());
            Assert.Throws<InvalidNodaDataException>(() => reader.ReadByte());
        }
    }
}
