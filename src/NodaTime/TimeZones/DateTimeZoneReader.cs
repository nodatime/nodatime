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
using System.Text;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides mostly primitive data reading capabilities for use from time zone classes,
    /// for reading data written by <see cref="DateTimeZoneWriter"/>.
    /// </summary>
    // TODO: Consider renaming to TzdbDateTimeZoneReader
    internal sealed class DateTimeZoneReader
    {
        private readonly Stream input;

        internal DateTimeZoneReader(Stream input)
        {
            this.input = input;
        }

        /// <summary>
        /// Reads a non-negative integer value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteCount" />.
        /// </remarks>
        /// <returns>The integer value from the stream.</returns>
        internal int ReadCount()
        {
            unchecked
            {
                byte flag = ReadByte();
                if (flag == 0xff)
                {
                    // Note that this will handle earlier versions of Noda Time which used negative
                    // count values in zone recurrences for DaylightSavingsDateTimeZone. Those
                    // values are now prohibited, but will be read appropriately here.
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

        /// <summary>
        /// Reads an offset value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteOffset" />.
        /// </remarks>
        /// <returns>The offset value from the stream.</returns>
        internal Offset ReadOffset()
        {
            /*
             * Milliseconds encoding formats:
             *
             * upper bits      units       field length  approximate range
             * ---------------------------------------------------------------
             * 0xxxxxxx        30 minutes  1 byte        +/- 24 hours
             * 10xxxxxx        seconds     3 bytes       +/- 24 hours
             * 11111101  0xfd  millis      5 byte        Full range
             *
             * Remaining bits in field form signed offset from 1970-01-01T00:00:00Z.
             */
            unchecked
            {
                byte flag = ReadByte();

                if ((flag & 0x80) == 0)
                {
                    int units = flag - DateTimeZoneWriter.MaxMillisHalfHours;
                    return Offset.FromMilliseconds(units * (30 * NodaConstants.MillisecondsPerMinute));
                }
                if ((flag & 0xc0) == DateTimeZoneWriter.FlagMillisSeconds)
                {
                    int first = flag & ~0xc0;
                    int second = ReadByte() & 0xff;
                    int third = ReadByte() & 0xff;

                    int units = (((first << 8) + second) << 8) + third;
                    units = units - DateTimeZoneWriter.MaxMillisSeconds;
                    return Offset.FromMilliseconds(units * NodaConstants.MillisecondsPerSecond);
                }
                return Offset.FromMilliseconds(ReadInt32());
            }
        }

        /// <summary>
        /// Reads a long ticks value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteTicks" />.
        /// </remarks>
        /// <returns>The long ticks value from the stream.</returns>
        internal long ReadTicks()
        {
            /*
             * Ticks encoding formats:
             *
             * upper two bits  units       field length  approximate range
             * ---------------------------------------------------------------
             * 00              30 minutes  1 byte        +/- 16 hours
             * 01              minutes     3 bytes       +/- 1020 years
             * 10              seconds     5 bytes       +/- 4355 years
             * 11000000        ticks       9 bytes       +/- 292,000 years
             * 11111100  0xfc              1 byte         Offset.MaxValue
             * 11111101  0xfd              1 byte         Offset.MinValue
             * 11111110  0xfe              1 byte         Instant, LocalInstant, Duration MaxValue
             * 11111111  0xff              1 byte         Instant, LocalInstant, Duration MinValue
             *
             * Remaining bits in field form signed offset from 1970-01-01T00:00:00Z.
             */
            unchecked
            {
                byte flag = ReadByte();
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

                    long units = (((first << 8) + second) << 8) + third;
                    units = units - DateTimeZoneWriter.MaxMinutes;
                    return units * NodaConstants.TicksPerMinute;
                }
                if ((flag & 0xc0) == DateTimeZoneWriter.FlagSeconds)
                {
                    long first = flag & ~0xc0;
                    long second = ReadInt32() & 0xffffffffL;

                    long units = (first << 32) + second;
                    units = units - DateTimeZoneWriter.MaxSeconds;
                    return units * NodaConstants.TicksPerSecond;
                }

                return ReadInt64();
            }
        }

        /// <summary>
        /// Reads a boolean value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteBoolean" />.
        /// </remarks>
        /// <returns>The boolean value.</returns>
        internal bool ReadBoolean()
        {
            return ReadByte() == 0 ? false : true;
        }

        /// <summary>
        /// Reads a string to string dictionary value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteDictionary" />.
        /// </remarks>
        /// <returns>The <see cref="IDictionary{TKey,TValue}" /> value from the stream.</returns>
        internal IDictionary<string, string> ReadDictionary()
        {
            IDictionary<string, string> results = new Dictionary<string, string>();
            int count = ReadCount();
            for (int i = 0; i < count; i++)
            {
                string key = ReadString();
                string value = ReadString();
                results.Add(key, value);
            }
            return results;
        }
        
        /// <summary>
        /// Reads an <see cref="Instant" /> value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteInstant" />.
        /// </remarks>
        /// <returns>The <see cref="Instant" /> value from the stream.</returns>
        internal Instant ReadInstant()
        {
            return new Instant(ReadTicks());
        }

        /// <summary>
        /// Reads an <see cref="LocalInstant" /> value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteLocalInstant" />.
        /// </remarks>
        /// <returns>The <see cref="LocalInstant" /> value from the stream.</returns>
        internal LocalInstant ReadLocalInstant()
        {
            return new LocalInstant(ReadTicks());
        }

        /// <summary>
        /// Reads a string value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteString" />.
        /// </remarks>
        /// <returns>The string value from the stream.</returns>
        internal string ReadString()
        {
            int length = ReadCount();
            var data = new byte[length];
            int offset = 0;
            while (offset < length)
            {
                int bytesRead = input.Read(data, 0, length);
                if (bytesRead <= 0)
                {
                    throw new EndOfStreamException("Unexpectedly reached end of data with " + (length - offset) + " bytes still to read");
                }
                offset += bytesRead;
            }
            return Encoding.UTF8.GetString(data, 0, length);
        }

        /// <summary>
        /// Reads an <see cref="DateTimeZone" /> value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteTimeZone" />.
        /// </remarks>
        /// <returns>The <see cref="DateTimeZone" /> value from the stream.</returns>
        internal DateTimeZone ReadTimeZone(string id)
        {
            int flag = ReadByte();
            switch (flag)
            {
                case DateTimeZoneWriter.FlagTimeZoneFixed:
                    return FixedDateTimeZone.Read(this, id);
                case DateTimeZoneWriter.FlagTimeZonePrecalculated:
                    return PrecalculatedDateTimeZone.Read(this, id);
                case DateTimeZoneWriter.FlagTimeZoneNull:
                    return null; // Only used when reading a tail zone
                case DateTimeZoneWriter.FlagTimeZoneCached:
                    return CachedDateTimeZone.Read(this, id);
                case DateTimeZoneWriter.FlagTimeZoneDst:
                    return DaylightSavingsDateTimeZone.Read(this, id);
                default:
                    throw new IOException("Unknown flag type " + flag);
            }
        }

        /// <summary>
        ///   Reads a signed 16 bit integer value from the stream and returns it as an int.
        /// </summary>
        /// <returns>The 16 bit int value.</returns>
        internal int ReadInt16()
        {
            unchecked
            {
                int high = ReadByte();
                int low = ReadByte();
                return (high << 8) | low;
            }
        }

        /// <summary>
        /// Reads a signed 32 bit integer value from the stream and returns it as an int.
        /// </summary>
        /// <returns>The 32 bit int value.</returns>
        internal int ReadInt32()
        {
            unchecked
            {
                int high = ReadInt16() & 0xffff;
                int low = ReadInt16() & 0xffff;
                return (high << 16) | low;
            }
        }

        /// <summary>
        /// Reads a signed 64 bit integer value from the stream and returns it as an long.
        /// </summary>
        /// <returns>The 64 bit long value.</returns>
        internal long ReadInt64()
        {
            unchecked
            {
                long high = ReadInt32() & 0xffffffffL;
                long low = ReadInt32() & 0xffffffffL;
                return (high << 32) | low;
            }
        }

        /// <summary>
        /// Reads a signed 8 bit integer value from the stream and returns it as an int.
        /// </summary>
        /// <returns>The 8 bit int value.</returns>
        /// <exception cref="EndOfStreamException">The data in the stream has been exhausted</exception>
        internal byte ReadByte()
        {
            int value = input.ReadByte();
            if (value == -1)
            {
                throw new EndOfStreamException();
            }
            return (byte)value;
        }
    }
}
