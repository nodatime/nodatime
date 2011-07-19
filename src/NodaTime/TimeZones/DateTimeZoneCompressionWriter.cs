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
using System.IO;

namespace NodaTime.TimeZones
{
    /// <summary>
    ///   Very specific compressing binary writer for time zones.
    /// </summary>
    internal class DateTimeZoneCompressionWriter : DateTimeZoneWriter
    {
        internal const long HalfHoursMask = 0x3fL;
        internal const long MaxHalfHours = 0x1fL;
        internal const long MinHalfHours = -MaxHalfHours;

        internal const int MaxMillisHalfHours = 0x3f;
        internal const int MinMillisHalfHours = -MaxMillisHalfHours;

        internal const long MaxMinutes = 0x1fffffL;
        internal const long MinMinutes = -MaxMinutes;

        internal const long MaxSeconds = 0x1fffffffffL;
        internal const long MinSeconds = -MaxSeconds;

        internal const int MaxMillisSeconds = 0x3ffff;
        internal const int MinMillisSeconds = -MaxMillisSeconds;

        internal const byte FlagHalfHour = 0x00;
        internal const byte FlagMinutes = 0x40;
        internal const byte FlagSeconds = 0x80;
        internal const byte FlagMillisSeconds = 0x80;
        internal const byte FlagTicks = 0xc0;
        internal const byte FlagMilliseconds = 0xfd;
        internal const byte FlagMaxValue = 0xfe;
        internal const byte FlagMinValue = 0xff;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DateTimeZoneCompressionWriter" /> class.
        /// </summary>
        /// <param name = "output">Where to send the serialized output.</param>
        internal DateTimeZoneCompressionWriter(Stream output) : base(output)
        {
        }

        /// <summary>
        ///   Writes the given non-negative integer value to the stream.
        /// </summary>
        /// <remarks>
        /// Negative values are handled but in an inefficient way (5 bytes).
        /// </remarks>
        /// <param name = "value">The value to write.</param>
        internal override void WriteCount(int value)
        {
            unchecked
            {
                if (value < 0)
                {
                    WriteInt8(0xff);
                    WriteInt32(value);
                    return;
                }
                if (value <= 0x0e)
                {
                    WriteInt8((byte)(0xf0 + value));
                    return;
                }
                value -= 0x0f;
                if (value <= 0x7f)
                {
                    WriteInt8((byte)value);
                    return;
                }
                value -= 0x80;
                if (value <= 0x3fff)
                {
                    WriteInt8((byte)(0x80 + (value >> 8)));
                    WriteInt8((byte)(value & 0xff));
                    return;
                }
                value -= 0x4000;

                if (value <= 0x1fffff)
                {
                    WriteInt8((byte)(0xc0 + (value >> 16)));
                    WriteInt16((short)(value & 0xffff));
                    return;
                }
                value -= 0x200000;
                if (value <= 0x0fffffff)
                {
                    WriteInt8((byte)(0xe0 + (value >> 24)));
                    WriteInt8((byte)((value >> 16) & 0xff));
                    WriteInt16((short)(value & 0xffff));
                    return;
                }
                WriteInt8(0xff);
                WriteInt32(value + 0x200000 + 0x4000 + 0x80 + 0x0f);
            }
        }

        /// <summary>
        ///   Writes the integer milliseconds value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        internal override void WriteMilliseconds(int value)
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
                if (value == Int32.MinValue)
                {
                    WriteInt8(FlagMinValue);
                    return;
                }
                if (value == Int32.MaxValue)
                {
                    WriteInt8(FlagMaxValue);
                    return;
                }
                if (value % (30 * NodaConstants.MillisecondsPerMinute) == 0)
                {
                    // Try to write in 30 minute units.
                    int units = value / (30 * NodaConstants.MillisecondsPerMinute);
                    if (MinMillisHalfHours <= units && units <= MaxMillisHalfHours)
                    {
                        units = units + MaxMillisHalfHours;
                        WriteInt8((byte)(units & 0x7f));
                        return;
                    }
                }

                if (value % NodaConstants.MillisecondsPerSecond == 0)
                {
                    // Try to write seconds.
                    int seconds = value / NodaConstants.MillisecondsPerSecond;
                    if (MinMillisSeconds <= seconds && seconds <= MaxMillisSeconds)
                    {
                        seconds = seconds + MaxMillisSeconds;
                        WriteInt8((byte)(FlagMillisSeconds | (byte)((seconds >> 16) & 0x3f)));
                        WriteInt16((short)(seconds & 0xffff));
                        return;
                    }
                }

                // Write milliseconds either because the additional precision is
                // required or the minutes didn't fit in the field.

                // Form 11 (64 bits effective precision, but write as if 70 bits)
                WriteInt8(FlagMilliseconds);
                WriteInt32(value);
            }
        }

        /// <summary>
        ///   Writes the long ticks value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        internal override void WriteTicks(long value)
        {
            /*
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
            unchecked
            {
                if (value == Int64.MinValue)
                {
                    WriteInt8(FlagMinValue);
                    return;
                }
                if (value == Int64.MaxValue)
                {
                    WriteInt8(FlagMaxValue);
                    return;
                }
                if (value % (30 * NodaConstants.TicksPerMinute) == 0)
                {
                    // Try to write in 30 minute units.
                    long units = value / (30 * NodaConstants.TicksPerMinute);
                    if (MinHalfHours <= units && units <= MaxHalfHours)
                    {
                        units = units + MaxHalfHours;
                        WriteInt8((byte)(units & 0x3f));
                        return;
                    }
                }

                if (value % NodaConstants.TicksPerMinute == 0)
                {
                    // Try to write minutes.
                    long minutes = value / NodaConstants.TicksPerMinute;
                    if (MinMinutes <= minutes && minutes <= MaxMinutes)
                    {
                        minutes = minutes + MaxMinutes;
                        WriteInt8((byte)(FlagMinutes | (byte)((minutes >> 16) & 0x3f)));
                        WriteInt16((short)(minutes & 0xffff));
                        return;
                    }
                }

                if (value % NodaConstants.TicksPerSecond == 0)
                {
                    // Try to write seconds.
                    long seconds = value / NodaConstants.TicksPerSecond;
                    if (MinSeconds <= seconds && seconds <= MaxSeconds)
                    {
                        seconds = seconds + MaxSeconds;
                        WriteInt8((byte)(FlagSeconds | (byte)((seconds >> 32) & 0x3f)));
                        WriteInt32((int)(seconds & 0xffffffff));
                        return;
                    }
                }

                // Write milliseconds either because the additional precision is
                // required or the minutes didn't fit in the field.

                // Form 11 (64 bits effective precision, but write as if 70 bits)
                WriteInt8(FlagTicks);
                WriteInt64(value);
            }
        }
    }
}
