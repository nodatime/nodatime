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
using NodaTime.Utility;

namespace NodaTime.TimeZones.IO
{
    /// <summary>
    /// Implementation of <see cref="IDateTimeZoneWriter"/> which maintains the legacy "resource" format.
    /// </summary>
    internal sealed class LegacyDateTimeZoneWriter : IDateTimeZoneWriter
    {
        internal const byte FlagTimeZoneCached = 0;
        internal const byte FlagTimeZoneDst = 1;
        internal const byte FlagTimeZoneFixed = 2;
        internal const byte FlagTimeZoneNull = 3;
        internal const byte FlagTimeZonePrecalculated = 4;

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

        private readonly Stream output;
        private readonly IList<string> stringPool; 

        /// <summary>
        /// Constructs a LegacyDateTimeZoneWriter.
        /// </summary>
        /// <param name="output">Where to send the serialized output.</param>
        /// <param name="stringPool">String pool to add strings to, or null for no pool</param>
        internal LegacyDateTimeZoneWriter(Stream output, IList<string> stringPool)
        {
            this.output = output;
            this.stringPool = stringPool;
        }

        /// <summary>
        /// Writes the given non-negative integer value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteCount(int value)
        {
            Preconditions.CheckArgumentRange("value", value, 0, int.MaxValue);
            unchecked
            {
                if (value <= 0x0e)
                {
                    WriteByte((byte)(0xf0 + value));
                    return;
                }
                value -= 0x0f;
                if (value <= 0x7f)
                {
                    WriteByte((byte)value);
                    return;
                }
                value -= 0x80;
                if (value <= 0x3fff)
                {
                    WriteByte((byte)(0x80 + (value >> 8)));
                    WriteByte((byte)(value & 0xff));
                    return;
                }
                value -= 0x4000;

                if (value <= 0x1fffff)
                {
                    WriteByte((byte)(0xc0 + (value >> 16)));
                    WriteInt16((short)(value & 0xffff));
                    return;
                }
                value -= 0x200000;
                if (value <= 0x0fffffff)
                {
                    WriteByte((byte)(0xe0 + (value >> 24)));
                    WriteByte((byte)((value >> 16) & 0xff));
                    WriteInt16((short)(value & 0xffff));
                    return;
                }
                WriteByte(0xff);
                WriteInt32(value + 0x200000 + 0x4000 + 0x80 + 0x0f);
            }
        }

        /// <summary>
        /// Writes the offset value to the stream.
        /// </summary>
        /// <param name="offset">The value to write.</param>
        public void WriteOffset(Offset offset)
        {
            int millis = offset.Milliseconds;
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
                if (millis % (30 * NodaConstants.MillisecondsPerMinute) == 0)
                {
                    // Try to write in 30 minute units.
                    int units = millis / (30 * NodaConstants.MillisecondsPerMinute);
                    if (MinMillisHalfHours <= units && units <= MaxMillisHalfHours)
                    {
                        units = units + MaxMillisHalfHours;
                        WriteByte((byte)(units & 0x7f));
                        return;
                    }
                }

                if (millis % NodaConstants.MillisecondsPerSecond == 0)
                {
                    // Try to write seconds.
                    int seconds = millis / NodaConstants.MillisecondsPerSecond;
                    if (MinMillisSeconds <= seconds && seconds <= MaxMillisSeconds)
                    {
                        seconds = seconds + MaxMillisSeconds;
                        WriteByte((byte)(FlagMillisSeconds | (byte)((seconds >> 16) & 0x3f)));
                        WriteInt16((short)(seconds & 0xffff));
                        return;
                    }
                }

                // Write milliseconds either because the additional precision is
                // required or the minutes didn't fit in the field.

                // Form 11 (64 bits effective precision, but write as if 70 bits)
                WriteByte(FlagMilliseconds);
                WriteInt32(millis);
            }
        }

        /// <summary>
        /// Writes the long ticks value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        private void WriteTicks(long value)
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
                    WriteByte(FlagMinValue);
                    return;
                }
                if (value == Int64.MaxValue)
                {
                    WriteByte(FlagMaxValue);
                    return;
                }
                if (value % (30 * NodaConstants.TicksPerMinute) == 0)
                {
                    // Try to write in 30 minute units.
                    long units = value / (30 * NodaConstants.TicksPerMinute);
                    if (MinHalfHours <= units && units <= MaxHalfHours)
                    {
                        units = units + MaxHalfHours;
                        WriteByte((byte)(units & 0x3f));
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
                        WriteByte((byte)(FlagMinutes | (byte)((minutes >> 16) & 0x3f)));
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
                        WriteByte((byte)(FlagSeconds | (byte)((seconds >> 32) & 0x3f)));
                        WriteInt32((int)(seconds & 0xffffffff));
                        return;
                    }
                }

                // Write milliseconds either because the additional precision is
                // required or the minutes didn't fit in the field.

                // Form 11 (64 bits effective precision, but write as if 70 bits)
                WriteByte(FlagTicks);
                WriteInt64(value);
            }
        }

        /// <summary>
        /// Writes a boolean value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        internal void WriteBoolean(bool value)
        {
            WriteByte((byte)(value ? 1 : 0));
        }

        /// <summary>
        /// Writes the given dictionary of string to string to the stream.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}" /> to write.</param>
        public void WriteDictionary(IDictionary<string, string> dictionary)
        {
            Preconditions.CheckNotNull(dictionary, "dictionary");
            WriteCount(dictionary.Count);
            foreach (var entry in dictionary)
            {
                WriteString(entry.Key);
                WriteString(entry.Value);
            }
        }

        /// <summary>
        /// Writes the <see cref="Instant" /> value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInstant(Instant value)
        {
            WriteTicks(value.Ticks);
        }

        /// <summary>
        /// Writes the string value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteString(string value)
        {
            if (stringPool == null)
            {
                byte[] data = Encoding.UTF8.GetBytes(value);
                int length = data.Length;
                WriteCount(length);
                output.Write(data, 0, data.Length);
            }
            else
            {
                int index = stringPool.IndexOf(value);
                if (index == -1)
                {
                    index = stringPool.Count;
                    stringPool.Add(value);
                }
                WriteCount(index);
            }
        }

        /// <summary>
        /// Writes the <see cref="DateTimeZone" /> value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteTimeZone(DateTimeZone value)
        {
            if (value == null)
            {
                WriteByte(FlagTimeZoneNull);
                return;
            }
            if (value is FixedDateTimeZone)
            {
                WriteByte(FlagTimeZoneFixed);
                ((FixedDateTimeZone)value).Write(this);
            }
            else if (value is PrecalculatedDateTimeZone)
            {
                WriteByte(FlagTimeZonePrecalculated);
                ((PrecalculatedDateTimeZone)value).WriteLegacy(this);
            }
            else if (value is CachedDateTimeZone)
            {
                WriteByte(FlagTimeZoneCached);
                ((CachedDateTimeZone)value).Write(this);
            }
            else if (value is DaylightSavingsDateTimeZone)
            {
                WriteByte(FlagTimeZoneDst);
                ((DaylightSavingsDateTimeZone)value).WriteLegacy(this);
            }
            else
            {
                throw new ArgumentException("Time zone type unknown to DateTimeZoneWriter");                
            }
        }

        /// <summary>
        /// Writes the given 16 bit integer value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        private void WriteInt16(short value)
        {
            unchecked
            {
                WriteByte((byte)((value >> 8) & 0xff));
                WriteByte((byte)(value & 0xff));
            }
        }

        /// <summary>
        /// Writes the given 32 bit integer value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        internal void WriteInt32(int value)
        {
            unchecked
            {
                WriteInt16((short)(value >> 16));
                WriteInt16((short)value);
            }
        }

        /// <summary>
        /// Writes the given 64 bit integer value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        private void WriteInt64(long value)
        {
            unchecked
            {
                WriteInt32((int)(value >> 32));
                WriteInt32((int)value);
            }
        }

        /// <summary>
        /// Writes the given 8 bit integer value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteByte(byte value)
        {
            unchecked
            {
                output.WriteByte(value);
            }
        }
    }
}
