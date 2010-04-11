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
using System.Text;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Very specific compressing binary writer for time zones.
    /// </summary>
    public class DateTimeZoneWriter
    {
        internal const long MaxHalfHours = 0x3f;
        private const long MinHalfHours = -MaxHalfHours;

        internal const int MaxMillisHalfHours = 0x3f;
        private const int MinMillisHalfHours = -MaxMillisHalfHours;

        private const long MaxMinutes = 0x1fffffL;
        private const long MinMinutes = -MaxMinutes;

        internal const long MaxSeconds = 0x1fffffffffL;
        private const long MinSeconds = -MaxSeconds;

        internal const int MaxMillisSeconds = 0x3ffff;
        private const int MinMillisSeconds = -MaxMillisSeconds;

        internal const byte FlagHalfHour = 0x00;
        internal const byte FlagMinutes = 0x40;
        internal const byte FlagSeconds = 0x80;
        internal const byte FlagMillisSeconds = 0x80;
        private const byte FlagTicks = 0xc0;
        private const byte FlagMilliseconds = 0xfd;
        internal const byte FlagMaxValue = 0xfe;
        internal const byte FlagMinValue = 0xff;

        private const byte FlagTimeZoneUser = 0;
        internal const byte FlagTimeZoneFixed = 1;
        internal const byte FlagTimeZoneCached = 2;
        internal const byte FlagTimeZonePrecalculated = 3;
        internal const byte FlagTimeZoneDst = 4;
        internal const byte FlagTimeZoneNull = 5;

        private readonly Stream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeZoneWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public DateTimeZoneWriter(Stream stream)
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
        /// <param name="timeZone">The <see cref="IDateTimeZone"/> to write.</param>
        /// <returns><c>true</c> if the time zone was successfully written.</returns>
        public void WriteTimeZone(IDateTimeZone timeZone)
        {
            if (timeZone == null)
            {
                throw new ArgumentNullException("timeZone");
            }
            if (timeZone is FixedDateTimeZone)
            {
                WriteInt8(FlagTimeZoneFixed);
            }
            else if (timeZone is PrecalculatedDateTimeZone)
            {
                WriteInt8(FlagTimeZonePrecalculated);
            }
            else if (timeZone is CachedDateTimeZone)
            {
                WriteInt8(FlagTimeZoneCached);
            }
            else if (timeZone is DaylightSavingsTimeZone)
            {
                WriteInt8(FlagTimeZoneDst);
            }
            else if (timeZone is NullDateTimeZone)
            {
                WriteInt8(FlagTimeZoneNull);
            }
            else
            {
                WriteInt8(FlagTimeZoneUser);
                WriteString(timeZone.GetType().AssemblyQualifiedName);
            }
            timeZone.Write(this);
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

        public void WriteTicks(long ticks)
        {
            unchecked
            {
                if (ticks == Int64.MinValue)
                {
                    WriteInt8(FlagMinValue);
                    return;
                }
                if (ticks == Int64.MaxValue)
                {
                    WriteInt8(FlagMaxValue);
                    return;
                }
                if (ticks % (30 * NodaConstants.TicksPerMinute) == 0)
                {
                    // Try to write in 30 minute units.
                    long units = ticks / (30 * NodaConstants.TicksPerMinute);
                    if (MinHalfHours <= units && units <= MaxHalfHours)
                    {
                        units = units + MaxHalfHours;
                        WriteInt8((byte) (units & 0x3f));
                        return;
                    }
                }

                if (ticks % NodaConstants.TicksPerMinute == 0)
                {
                    // Try to write minutes.
                    long minutes = ticks / NodaConstants.TicksPerMinute;
                    if (MinMinutes <= minutes && minutes <= MaxMinutes)
                    {
                        minutes = minutes + MaxMinutes;
                        WriteInt32(((FlagMinutes << 24) | (int) (minutes & 0x3fffffff)));
                        return;
                    }
                }

                if (ticks % NodaConstants.TicksPerSecond == 0)
                {
                    // Try to write seconds.
                    long seconds = ticks / NodaConstants.TicksPerSecond;
                    if (MinSeconds <= seconds && seconds <= MaxSeconds)
                    {
                        seconds = seconds + MaxSeconds;
                        WriteInt8((byte) (FlagSeconds | (byte) ((seconds >> 32) & 0x3f)));
                        WriteInt32((int) (seconds & 0xffffffff));
                        return;
                    }
                }

                // Write milliseconds either because the additional precision is
                // required or the minutes didn't fit in the field.

                // Form 11 (64 bits effective precision, but write as if 70 bits)
                WriteInt8(FlagTicks);
                WriteInt64(ticks);
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

        public void WriteMilliseconds(int milliseconds)
        {
            unchecked
            {
                if (milliseconds == Int32.MinValue)
                {
                    WriteInt8(FlagMinValue);
                    return;
                }
                if (milliseconds == Int32.MaxValue)
                {
                    WriteInt8(FlagMaxValue);
                    return;
                }
                if (milliseconds % (30 * NodaConstants.MillisecondsPerMinute) == 0)
                {
                    // Try to write in 30 minute units.
                    int units = milliseconds / (30 * NodaConstants.MillisecondsPerMinute);
                    if (MinMillisHalfHours <= units && units <= MaxMillisHalfHours)
                    {
                        units = units + MaxMillisHalfHours;
                        WriteInt8((byte) (units & 0x7f));
                        return;
                    }
                }

                if (milliseconds % NodaConstants.MillisecondsPerSecond == 0)
                {
                    // Try to write seconds.
                    int seconds = milliseconds / NodaConstants.MillisecondsPerSecond;
                    if (MinMillisSeconds <= seconds && seconds <= MaxMillisSeconds)
                    {
                        seconds = seconds + MaxMillisSeconds;
                        WriteInt8((byte) (FlagMillisSeconds | (byte) ((seconds >> 16) & 0x3f)));
                        WriteInt16((short) (seconds & 0xffff));
                        return;
                    }
                }

                // Write milliseconds either because the additional precision is
                // required or the minutes didn't fit in the field.

                // Form 11 (64 bits effective precision, but write as if 70 bits)
                WriteInt8(FlagMilliseconds);
                WriteInt32(milliseconds);
            }
        }

        /// <summary>
        /// Writes the given dictionary of string to string to the stream.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}"/> to write.</param>
        public void WriteDictionary(IDictionary<string, string> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            WriteCount(dictionary.Count);
            foreach (var entry in dictionary)
            {
                WriteString(entry.Key);
                WriteString(entry.Value);
            }
        }

        /// <summary>
        /// Writes the offset value to the stream
        /// </summary>
        /// <param name="value">The offset to write.</param>
        public void WriteOffset(Offset value)
        {
            WriteMilliseconds(value.Milliseconds);
        }

        /// <summary>
        /// Writes the offset value to the stream
        /// </summary>
        /// <param name="value">The offset to write.</param>
        public void WriteInstant(Instant value)
        {
            WriteTicks(value.Ticks);
        }

        /// <summary>
        /// Writes the offset value to the stream
        /// </summary>
        /// <param name="value">The offset to write.</param>
        public void WriteLocalInstant(LocalInstant value)
        {
            WriteTicks(value.Ticks);
        }

        /// <summary>
        /// Writes the boolean.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public void WriteBoolean(bool value)
        {
            WriteInt8((byte) (value ? 1 : 0));
        }

        /// <summary>
        /// Writes the given number to the stream. The number is compressed in the output into the
        /// fewest necessary bytes.
        /// </summary>
        /// <remarks>
        /// This method is optimized for positive numbers. Negative number always take 5 bytes so if
        /// negative numbers are likely, then <see cref="WriteInt32"/> should be used.
        /// </remarks>
        /// <param name="value">The value to write.</param>
        public void WriteCount(int value)
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
                    WriteInt8((byte) (0xf0 + value));
                    return;
                }
                value -= 0x0f;
                if (value <= 0x7f)
                {
                    WriteInt8((byte) value);
                    return;
                }
                value -= 0x80;
                if (value <= 0x3fff)
                {
                    WriteInt8((byte) (0x80 + (value >> 8)));
                    WriteInt8((byte) (value & 0xff));
                    return;
                }
                value -= 0x4000;

                if (value <= 0x1fffff)
                {
                    WriteInt8((byte) (0xc0 + (value >> 16)));
                    WriteInt16((short) (value & 0xffff));
                    return;
                }
                value -= 0x200000;
                if (value <= 0x0fffffff)
                {
                    WriteInt8((byte) (0xe0 + (value >> 24)));
                    WriteInt8((byte) ((value >> 16) & 0xff));
                    WriteInt16((short) (value & 0xffff));
                    return;
                }
                WriteInt8(0xff);
                WriteInt32(value + 0x200000 + 0x4000 + 0x80 + 0x0f);
            }
        }

        /// <summary>
        /// Writes the given value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        private void WriteInt64(long value)
        {
            unchecked
            {
                WriteInt32((int) (value >> 32));
                WriteInt32((int) value);
            }
        }

        /// <summary>
        /// Writes the given value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        private void WriteInt32(int value)
        {
            unchecked
            {
                WriteInt16((short) (value >> 16));
                WriteInt16((short) value);
            }
        }

        /// <summary>
        /// Writes the given value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        private void WriteInt16(short value)
        {
            unchecked
            {
                WriteInt8((byte) ((value >> 8) & 0xff));
                WriteInt8((byte) (value & 0xff));
            }
        }

        /// <summary>
        /// Writes the given value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt8(byte value)
        {
            unchecked
            {
                stream.WriteByte(value);
            }
        }

        /// <summary>
        /// Writes the given string to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteString(string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            int length = data.Length;
            WriteCount(length);
            stream.Write(data, 0, data.Length);
        }
    }
}