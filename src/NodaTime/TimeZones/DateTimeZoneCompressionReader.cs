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
using System.IO;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// A very specific compressing binary stream reader for time zones.
    /// </summary>
    internal sealed class DateTimeZoneCompressionReader : DateTimeZoneReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeZoneCompressionReader" /> class.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        internal DateTimeZoneCompressionReader(Stream stream) : base(stream)
        {
        }

        /// <summary>
        /// Reads a non-negative integer value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteCount" />.
        /// </remarks>
        /// <returns>The integer value from the stream.</returns>
        internal override int ReadCount()
        {
            unchecked
            {
                byte flag = ReadByte();
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

        /// <summary>
        /// Reads an integer millisecond value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteMilliseconds" />.
        /// </remarks>
        /// <returns>The integer millisecond value from the stream.</returns>
        internal override int ReadMilliseconds()
        {
            /*
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
            unchecked
            {
                byte flag = ReadByte();
                if (flag == DateTimeZoneCompressionWriter.FlagMinValue)
                {
                    return Int32.MinValue;
                }
                if (flag == DateTimeZoneCompressionWriter.FlagMaxValue)
                {
                    return Int32.MaxValue;
                }

                if ((flag & 0x80) == 0)
                {
                    int units = flag - DateTimeZoneCompressionWriter.MaxMillisHalfHours;
                    return units * (30 * NodaConstants.MillisecondsPerMinute);
                }
                if ((flag & 0xc0) == DateTimeZoneCompressionWriter.FlagMillisSeconds)
                {
                    int first = flag & ~0xc0;
                    int second = ReadByte() & 0xff;
                    int third = ReadByte() & 0xff;

                    int units = (((first << 8) + second) << 8) + third;
                    units = units - DateTimeZoneCompressionWriter.MaxMillisSeconds;
                    return units * NodaConstants.MillisecondsPerSecond;
                }
                return ReadInt32();
            }
        }

        /// <summary>
        /// Reads a long ticks value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteTicks" />.
        /// </remarks>
        /// <returns>The long ticks value from the stream.</returns>
        internal override long ReadTicks()
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
                if (flag == DateTimeZoneCompressionWriter.FlagMinValue)
                {
                    return Int64.MinValue;
                }
                if (flag == DateTimeZoneCompressionWriter.FlagMaxValue)
                {
                    return Int64.MaxValue;
                }

                if ((flag & 0xc0) == DateTimeZoneCompressionWriter.FlagHalfHour)
                {
                    long units = flag - DateTimeZoneCompressionWriter.MaxHalfHours;
                    return units * (30 * NodaConstants.TicksPerMinute);
                }
                if ((flag & 0xc0) == DateTimeZoneCompressionWriter.FlagMinutes)
                {
                    int first = flag & ~0xc0;
                    int second = ReadByte() & 0xff;
                    int third = ReadByte() & 0xff;

                    long units = (((first << 8) + second) << 8) + third;
                    units = units - DateTimeZoneCompressionWriter.MaxMinutes;
                    return units * NodaConstants.TicksPerMinute;
                }
                if ((flag & 0xc0) == DateTimeZoneCompressionWriter.FlagSeconds)
                {
                    long first = flag & ~0xc0;
                    long second = ReadInt32() & 0xffffffffL;

                    long units = (first << 32) + second;
                    units = units - DateTimeZoneCompressionWriter.MaxSeconds;
                    return units * NodaConstants.TicksPerSecond;
                }

                return ReadInt64();
            }
        }
    }
}
