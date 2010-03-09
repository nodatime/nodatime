#region Copyright and license information

// Copyright 2001-2010 Stephen Colebourne
// Copyright 2010 Jon Skeet
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
using System.Text;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// A very specifc compressing binary stream reader for time zones.
    /// </summary>
    public class DateTimeZoneReader
    {
        private readonly Stream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeZoneWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public DateTimeZoneReader(Stream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Writes the given <see cref="IDateTimeZone"/> object to the given stream.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Currently this method only supports writing of 
        /// </para>
        /// <para>
        /// This method uses a <see cref="BinaryWriter"/> to write the time zone to the stream and
        /// <see cref="BinaryWriter"/> does not support leaving the underlying stream open when it
        /// is closed. Because of this there is no good way to guarentee that the input stream will
        /// still be open because the finalizer for <see cref="BinaryWriter"/> will close the
        /// stream. Therefore, we make sure that the stream is always closed.
        /// </para>
        /// </remarks>
        /// <param name="id">The id of the <see cref="IDateTimeZone"/> object to read.</param>
        /// <returns><c>true</c> if the time zone was successfully written.</returns>
        public IDateTimeZone ReadTimeZone(string id)
        {
            int flag = ReadByte();
            if (flag == DateTimeZoneWriter.FlagTimeZoneFixed)
            {
                return FixedDateTimeZone.Read(this, id);
            }
            if (flag == DateTimeZoneWriter.FlagTimeZonePrecalculated)
            {
                return PrecalculatedDateTimeZone.Read(this, id);
            }
            if (flag == DateTimeZoneWriter.FlagTimeZoneCached)
            {
                return CachedDateTimeZone.Read(this, id);
            }
            if (flag == DateTimeZoneWriter.FlagTimeZoneDst)
            {
                return DaylightSavingsTimeZone.Read(this, id);
            }
            if (flag == DateTimeZoneWriter.FlagTimeZoneNull)
            {
                return NullDateTimeZone.Read(this, id);
            }
            var className = ReadString();
            var type = Type.GetType(className);
            if (type == null)
            {
                throw new InvalidOperationException(@"Unknown IDateTimeZone type: " + className);
            }
            var method = type.GetMethod("Read", new[] {typeof (DateTimeZoneReader), typeof (string)});
            if (method != null)
            {
                return method.Invoke(null, new object[] {this, id}) as IDateTimeZone;
            }
            return null;
        }

        /**
         * Ticks encoding formats:
         *
         * upper two bits  units       field length  approximate range
         * ---------------------------------------------------------------
         * 00              30 minutes  1 byte        +/- 16 hours
         * 01              minutes     4 bytes       +/- 1020 years
         * 10              seconds     5 bytes       +/- 4355 years
         * 11000000        ticks       9 bytes       +/- 292,000 years
         * 11111100  0xfc              1 byte         Offset.MaxValue
         * 11111101  0xfd              1 byte         Offset.MinValue
         * 11111110  0xfe              1 byte         Instant, LocalInstant, Duration MaxValue
         * 11111111  0xff              1 byte         Instant, LocalInstant, Duration MinValue
         *
         * Remaining bits in field form signed offset from 1970-01-01T00:00:00Z.
         */

        public long ReadTicks()
        {
            unchecked
            {
                byte flag = (byte) ReadByte();
                if (flag == DateTimeZoneWriter.FlagMinValue)
                {
                    return Int64.MinValue;
                }
                if (flag == DateTimeZoneWriter.FlagMaxValue)
                {
                    return Int64.MaxValue;
                }

                if ((flag & 0xc0) == DateTimeZoneWriter.FlagHalfHour)
                {
                    long units = flag - DateTimeZoneWriter.MaxHalfHours;
                    return units * (30 * NodaConstants.TicksPerMinute);
                }
                if ((flag & 0xc0) == DateTimeZoneWriter.FlagMinutes)
                {
                    int first = flag & ~0xc0;
                    int second = ReadByte() & 0xff;
                    int third = ReadByte() & 0xff;
                    int fourth = ReadByte() & 0xff;

                    long units = (((((first << 8) + second) << 8) + third) << 8) + fourth;
                    units = units - DateTimeZoneWriter.MaxHalfHours;
                    return units * NodaConstants.TicksPerMinute;
                }
                if ((flag & 0xc0) == DateTimeZoneWriter.FlagSeconds)
                {
                    long first = flag & ~0xc0;
                    long second = this.ReadInt32() & 0xffffffffL;

                    long units = (first << 32) + second;
                    units = units - DateTimeZoneWriter.MaxSeconds;
                    return units * NodaConstants.TicksPerSecond;
                }

                return ReadInt64();
            }
        }

        /**
         * Milliseconds encoding formats:
         *
         * upper bits      units       field length  approximate range
         * ---------------------------------------------------------------
         * 0xxxxxxx        30 minutes  1 byte        +/- 24 hours
         * 10xxxxxx        seconds     3 bytes       +/- 24 hours
         * 11111101  0xfd  millis      5 byte        Full range
         * 11111110  0xfe              1 byte        Int32.MaxValue
         * 11111111  0xff              1 byte        Int32.MinValue
         *
         * Remaining bits in field form signed offset from 1970-01-01T00:00:00Z.
         */

        public int ReadMilliseconds()
        {
            unchecked
            {
                byte flag = (byte) ReadByte();
                if (flag == DateTimeZoneWriter.FlagMinValue)
                {
                    return Int32.MinValue;
                }
                if (flag == DateTimeZoneWriter.FlagMaxValue)
                {
                    return Int32.MaxValue;
                }

                if ((flag & 0x80) == 0)
                {
                    int units = flag - DateTimeZoneWriter.MaxMillisHalfHours;
                    return units * (30 * NodaConstants.MillisecondsPerMinute);
                }
                if ((flag & 0xc0) == DateTimeZoneWriter.FlagMillisSeconds)
                {
                    int first = flag & ~0xc0;
                    int second = ReadByte() & 0xff;
                    int third = ReadByte() & 0xff;

                    int units = (((first << 8) + second) << 8) + third;
                    units = units - DateTimeZoneWriter.MaxMillisSeconds;
                    return units * NodaConstants.MillisecondsPerSecond;
                }
                return ReadInt32();
            }
        }

        /// <summary>
        /// Reads a dictionary of string to string from the stream.
        /// </summary>
        /// <returns>The <see cref="IDictionary{TKey,TValue}"/> to read.</returns>
        public IDictionary<string, string> ReadDictionary()
        {
            IDictionary<string, string> results = new Dictionary<string, string>();
            int count = this.ReadCount();
            for (int i = 0; i < count; i++)
            {
                var key = ReadString();
                var value = ReadString();
                results.Add(key, value);
            }
            return results;
        }

        /// <summary>
        /// Writes the offset value to the stream
        /// </summary>
        public Offset ReadOffset()
        {
            int milliseconds = this.ReadMilliseconds();
            return new Offset(milliseconds);
        }

        /// <summary>
        /// Writes the offset value to the stream
        /// </summary>
        public Instant ReadInstant()
        {
            long ticks = this.ReadTicks();
            return new Instant(ticks);
        }

        /// <summary>
        /// Writes the offset value to the stream
        /// </summary>
        public LocalInstant ReadLocalInstant()
        {
            long ticks = this.ReadTicks();
            return new LocalInstant(ticks);
        }

        /// <summary>
        /// Reads the boolean.
        /// </summary>
        /// <returns></returns>
        public bool ReadBoolean()
        {
            return ReadByte() == 0 ? false : true;
        }

        /// <summary>
        /// Reads a number from the stream. The number must have been written by <see
        /// cref="DateTimeZoneWriter.WriteCount"/> as the value is assuemd to be compressed.
        /// </summary>
        /// <returns>The integer value from the stream.</returns>
        public int ReadCount()
        {
            unchecked
            {
                byte flag = (byte) ReadByte();
                if (flag == 0xff)
                {
                    return ReadInt32();
                }
                if (0xf0 <= flag && flag <= 0xfe)
                {
                    return flag & 0x0f;
                }
                int adjustment = 0x0f;
                if ((flag & 0x80) == 0)
                {
                    return flag + adjustment;
                }
                adjustment += 0x80;
                if ((flag & 0xc0) == 0x80)
                {
                    int first = flag & 0x3f;
                    int second = ReadByte();
                    return ((first << 8) + second) + adjustment;
                }
                adjustment += 0x4000;
                if ((flag & 0xe0) == 0xc0)
                {
                    int first = flag & 0x1f;
                    int second = ReadInt16();
                    return ((first << 16) + second) + adjustment;
                }
                else
                {
                    adjustment += 0x200000;
                    int first = flag & 0x0f;
                    int second = ReadByte();
                    int third = ReadInt16();
                    return (((first << 8) + second) << 16) + third + adjustment;
                }
            }
        }

        private long ReadInt64()
        {
            unchecked
            {
                long high = ReadInt32() & 0xffffffff;
                long low = ReadInt32() & 0xffffffff;
                return (high << 32) | low;
            }
        }

        private int ReadInt32()
        {
            unchecked
            {
                int high = ReadInt16() & 0xffff;
                int low = ReadInt16() & 0xffff;
                return (high << 16) | low;
            }
        }

        private int ReadInt16()
        {
            unchecked
            {
                int high = ReadByte() & 0xff;
                int low = ReadByte() & 0xff;
                return (high << 8) | low;
            }
        }

        public int ReadByte()
        {
            return this.stream.ReadByte();
        }

        /// <summary>
        /// Writes the given string to the stream.
        /// </summary>
        public string ReadString()
        {
            int length = this.ReadCount();
            var data = new byte[length];
            stream.Read(data, 0, length);
            return Encoding.UTF8.GetString(data);
        }
    }
}