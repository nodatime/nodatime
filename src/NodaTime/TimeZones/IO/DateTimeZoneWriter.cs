// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NodaTime.Utility;

namespace NodaTime.TimeZones.IO
{
    /// <summary>
    /// Implementation of <see cref="IDateTimeZoneWriter"/> for the most recent version
    /// of the "blob" format of time zone data. If the format changes, this class will be
    /// renamed (e.g. to DateTimeZoneWriterV0) and the new implementation will replace it.
    /// </summary>
    internal sealed class DateTimeZoneWriter : IDateTimeZoneWriter
    {
        internal static class InstantConstants
        {
            // Special epoch for IO purposes
            internal static readonly Instant Epoch = Instant.FromUtc(1800, 1, 1, 0, 0);
            internal const long MaxHours = (1L << 22) - 1;
            internal const long MaxMinutes = (1L << 30) - 1;
            internal const long MaxSeconds = (1L << 38) - 1;
            internal static readonly Instant MaxOptimizedInstant = Epoch + Duration.MaxValue;

            internal const byte HoursFormat = 0 << 6;
            internal const byte MinutesFormat = 1 << 6; 
            internal const byte SecondsFormat = 2 << 6;
            internal const byte RawFormat = 3 << 6;
            internal const byte MinFormat = 0xe0;
            internal const byte MaxFormat = 0xff;
        }

        // TODO: Work out where best to put these flags, and the code to read/respond to them.
        // It's ugly at the moment.
        internal const byte FlagTimeZoneNull = 0;
        internal const byte FlagTimeZoneFixed = 1;
        internal const byte FlagTimeZoneDst = 2;
        internal const byte FlagTimeZonePrecalculated = 3;

        private readonly Stream output;
        private readonly IList<string> stringPool; 

        /// <summary>
        /// Constructs a DateTimeZoneWriter.
        /// </summary>
        /// <param name="output">Where to send the serialized output.</param>
        /// <param name="stringPool">String pool to add strings to, or null for no pool</param>
        internal DateTimeZoneWriter(Stream output, IList<string> stringPool)
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
            WriteVarint((uint) value);
        }

        /// <summary>
        /// Writes the given (possibly-negative) integer value to the stream.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="WriteCount"/>, this can encode negative numbers. It does, however, use a slightly less
        /// efficient encoding for positive numbers.
        /// <param name="value">The value to write.</param>
        public void WriteSignedCount(int value)
        {
            unchecked
            {
                WriteVarint((uint) ((value >> 31) ^ (value << 1)));  // zigzag encoding
            }
        }

        /// <summary>
        /// Writes the given integer value to the stream as a base-128 varint.
        /// </summary>
        /// <remarks>
        /// The format is a simple 7-bit encoding: while the value is greater than 127 (i.e.
        /// has more than 7 significant bits) we repeatedly write the least-significant 7 bits
        /// with the top bit of the byte set as a continuation bit, then shift the value right
        /// 7 bits.
        /// </remarks>
        /// <param name="value">The value to write.</param>
        private void WriteVarint(uint value)
        {
            unchecked
            {
                while (value > 0x7f)
                {
                    output.WriteByte((byte)(0x80 | (value & 0x7f)));
                    value = value >> 7;
                }
                output.WriteByte((byte)(value & 0x7f));
            }
        }

        /// <summary>
        /// Writes the offset value to the stream.
        /// </summary>
        /// <param name="offset">The value to write.</param>
        public void WriteOffset(Offset offset)
        {
            /*
             * First, add 24 hours to the number of milliseconds, to get a value in the range (0, 172800000).
             * (It's exclusive at both ends, but that's insignificant.)
             * Next, check whether it's an exact multiple of half-hours or minutes, and encode
             * appropriately. In every case, if it's an exact multiple, we know that we'll be able to fit
             * the value into the number of bits available.
             * 
             * first byte      units       max data value (+1)   field length
             * --------------------------------------------------------------
             * 0xxxxxxx        30 minutes  96                    1 byte  (7 data bits)
             * 100xxxxx        minutes     2880                  2 bytes (14 data bits)
             * 101xxxxx        seconds     172800                3 bytes (21 data bits)
             * 110xxxxx        millis      172800000             4 bytes (29 data bits)
             */
            int millis = offset.Milliseconds + NodaConstants.MillisecondsPerStandardDay;
            unchecked
            {
                if (millis % (30 * NodaConstants.MillisecondsPerMinute) == 0)
                {
                    int units = millis / (30 * NodaConstants.MillisecondsPerMinute);
                    WriteByte((byte)units);
                }
                else if (millis % NodaConstants.MillisecondsPerMinute == 0)
                {
                    int minutes = millis / NodaConstants.MillisecondsPerMinute;
                    WriteByte((byte)(0x80 | (minutes >> 8)));
                    WriteByte((byte)(minutes & 0xff));
                }
                else if (millis%NodaConstants.MillisecondsPerSecond == 0)
                {
                    int seconds = millis/NodaConstants.MillisecondsPerSecond;
                    WriteByte((byte) (0xa0 | (byte) ((seconds >> 16))));
                    WriteInt16((short) (seconds & 0xffff));
                }
                else
                {
                    WriteInt32((int) 0xc0000000 | millis);
                }
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
            /*
             * Instant encoding form:
             * - In the first byte, the first two bits indicate the format:
             *   - 00: Hours since 1800, 3 bytes (max year ~2278)
             *   - 01: Minutes since 1800, 4 bytes (max year ~3841)
             *   - 10: Seconds since 1800, 5 bytes (max year ~10510)
             *   - 11: Depends on remaining bits
             *       - 000000 - following 8 bytes give raw bits
             *       - 100000 - Instant.MinValue
             *       - 111111 - Instant.MaxValue
             */
            unchecked
            {
                if (value == Instant.MinValue)
                {
                    WriteByte(InstantConstants.MinFormat);
                    return;
                }
                if (value == Instant.MaxValue)
                {
                    WriteByte(InstantConstants.MaxFormat);
                    return;
                }
                if (value >= InstantConstants.Epoch && value <= InstantConstants.MaxOptimizedInstant)
                {
                    long ticks = (value - InstantConstants.Epoch).Ticks;
                    if (ticks % NodaConstants.TicksPerHour == 0)
                    {
                        long hours = ticks / NodaConstants.TicksPerHour;
                        if (hours <= InstantConstants.MaxHours)
                        {
                            // Hours in 22 bits
                            WriteByte((byte) (InstantConstants.HoursFormat | (hours >> 16)));
                            WriteInt16((short) hours);
                            return;
                        }
                    }
                    if (ticks % NodaConstants.TicksPerMinute == 0)
                    {
                        long minutes = ticks / NodaConstants.TicksPerMinute;
                        if (minutes <= InstantConstants.MaxMinutes)
                        {
                            // Minutes in 30 bits
                            WriteByte((byte)(InstantConstants.MinutesFormat | (minutes >> 24)));
                            WriteByte((byte)(minutes >> 16));
                            WriteInt16((short)minutes);
                            return;
                        }
                    }
                    if (ticks % NodaConstants.TicksPerSecond == 0)
                    {
                        long seconds = ticks / NodaConstants.TicksPerSecond;
                        if (seconds <= InstantConstants.MaxSeconds)
                        {
                            // Seconds in 38 bits
                            WriteByte((byte)(InstantConstants.SecondsFormat | (seconds >> 32)));
                            WriteInt32((int)seconds);
                            return;
                        }
                    }
                }
                // Okay, this is unusual - we're writing out something which is either out of
                // range, or doesn't have a nice round value.
                WriteByte(InstantConstants.RawFormat);
                WriteInt64(value.Ticks);
            }
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
                ((PrecalculatedDateTimeZone)value).Write(this);
            }
            else if (value is CachedDateTimeZone)
            {
                ((CachedDateTimeZone)value).Write(this);
            }
            else if (value is DaylightSavingsDateTimeZone)
            {
                WriteByte(FlagTimeZoneDst);
                ((DaylightSavingsDateTimeZone)value).Write(this);
            }
            else
            {
                throw new ArgumentException("Unknown DateTimeZone type " + value.GetType());
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
        private void WriteInt32(int value)
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
        private void WriteByte(byte value)
        {
            unchecked
            {
                output.WriteByte(value);
            }
        }
    }
}
