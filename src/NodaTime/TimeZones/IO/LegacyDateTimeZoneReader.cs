// Copyright 2013 The Noda Time Authors. All rights reserved.
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
    /// Implementation of <see cref="IDateTimeZoneReader"/> which maintains the legacy "resource" format.
    /// </summary>
    internal sealed class LegacyDateTimeZoneReader : IDateTimeZoneReader
    {
        private readonly Stream input;
        private readonly IList<string> stringPool; 

        internal LegacyDateTimeZoneReader(Stream input, IList<string> stringPool)
        {
            this.input = input;
            this.stringPool = stringPool;
        }

        /// <summary>
        /// Reads a non-negative integer value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="LegacyDateTimeZoneWriter.WriteCount" />.
        /// </remarks>
        /// <returns>The integer value from the stream.</returns>
        public int ReadCount()
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
        /// Always throws NotSupportedException
        /// </summary>
        /// <returns>The integer read from the stream</returns>
        public int ReadSignedCount()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads an offset value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="LegacyDateTimeZoneWriter.WriteOffset" />.
        /// </remarks>
        /// <returns>The offset value from the stream.</returns>
        public Offset ReadOffset()
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
                    int units = flag - LegacyDateTimeZoneWriter.MaxMillisHalfHours;
                    return Offset.FromMilliseconds(units * (30 * NodaConstants.MillisecondsPerMinute));
                }
                if ((flag & 0xc0) == LegacyDateTimeZoneWriter.FlagMillisSeconds)
                {
                    int first = flag & ~0xc0;
                    int second = ReadByte() & 0xff;
                    int third = ReadByte() & 0xff;

                    int units = (((first << 8) + second) << 8) + third;
                    units = units - LegacyDateTimeZoneWriter.MaxMillisSeconds;
                    return Offset.FromMilliseconds(units * NodaConstants.MillisecondsPerSecond);
                }
                return Offset.FromMilliseconds(ReadInt32());
            }
        }

        /// <summary>
        /// Reads a long ticks value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="LegacyDateTimeZoneWriter.WriteTicks" />.
        /// </remarks>
        /// <returns>The long ticks value from the stream.</returns>
        private long ReadTicks()
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
             * 11111110  0xfe              1 byte         Instant, LocalInstant, Duration MaxValue
             * 11111111  0xff              1 byte         Instant, LocalInstant, Duration MinValue
             *
             * Remaining bits in field form signed offset from 1970-01-01T00:00:00Z.
             */
            unchecked
            {
                byte flag = ReadByte();
                if (flag == LegacyDateTimeZoneWriter.FlagMinValue)
                {
                    return Int64.MinValue;
                }
                if (flag == LegacyDateTimeZoneWriter.FlagMaxValue)
                {
                    return Int64.MaxValue;
                }

                if ((flag & 0xc0) == LegacyDateTimeZoneWriter.FlagHalfHour)
                {
                    long units = flag - LegacyDateTimeZoneWriter.MaxHalfHours;
                    return units * (30 * NodaConstants.TicksPerMinute);
                }
                if ((flag & 0xc0) == LegacyDateTimeZoneWriter.FlagMinutes)
                {
                    int first = flag & ~0xc0;
                    int second = ReadByte() & 0xff;
                    int third = ReadByte() & 0xff;

                    long units = (((first << 8) + second) << 8) + third;
                    units = units - LegacyDateTimeZoneWriter.MaxMinutes;
                    return units * NodaConstants.TicksPerMinute;
                }
                if ((flag & 0xc0) == LegacyDateTimeZoneWriter.FlagSeconds)
                {
                    long first = flag & ~0xc0;
                    long second = ReadInt32() & 0xffffffffL;

                    long units = (first << 32) + second;
                    units = units - LegacyDateTimeZoneWriter.MaxSeconds;
                    return units * NodaConstants.TicksPerSecond;
                }

                return ReadInt64();
            }
        }

        /// <summary>
        /// Reads a boolean value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="LegacyDateTimeZoneWriter.WriteBoolean" />.
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
        /// The value must have been written by <see cref="LegacyDateTimeZoneWriter.WriteDictionary" />.
        /// </remarks>
        /// <returns>The <see cref="IDictionary{TKey,TValue}" /> value from the stream.</returns>
        public IDictionary<string, string> ReadDictionary()
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
        /// The value must have been written by <see cref="LegacyDateTimeZoneWriter.WriteZoneIntervalTransition" />.
        /// The <paramref name="previous"/> parameter is ignored by this implementation.
        /// </remarks>
        /// <param name="previous">The previous transition written (usually for a given timezone), or null if there is
        /// no previous transition.</param>
        /// <returns>The <see cref="Instant" /> value from the stream.</returns>
        public Instant ReadZoneIntervalTransition(Instant? previous)
        {
            return new Instant(ReadTicks());
        }

        /// <summary>
        /// Reads a string value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="LegacyDateTimeZoneWriter.WriteString" />.
        /// </remarks>
        /// <returns>The string value from the stream.</returns>
        public string ReadString()
        {
            if (stringPool == null)
            {
                int length = ReadCount();
                var data = new byte[length];
                int offset = 0;
                while (offset < length)
                {
                    int bytesRead = input.Read(data, 0, length);
                    if (bytesRead <= 0)
                    {
                        throw new InvalidNodaDataException("Unexpectedly reached end of data with " + (length - offset) + " bytes still to read");
                    }
                    offset += bytesRead;
                }
                return Encoding.UTF8.GetString(data, 0, length);
            }
            else
            {
                int index = ReadCount();
                return stringPool[index];
            }
        }

        /// <summary>
        /// Reads an <see cref="DateTimeZone" /> value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="LegacyDateTimeZoneWriter.WriteTimeZone" />.
        /// </remarks>
        /// <returns>The <see cref="DateTimeZone" /> value from the stream.</returns>
        public DateTimeZone ReadTimeZone(string id)
        {
            int flag = ReadByte();
            switch (flag)
            {
                case LegacyDateTimeZoneWriter.FlagTimeZoneFixed:
                    return FixedDateTimeZone.Read(this, id);
                case LegacyDateTimeZoneWriter.FlagTimeZonePrecalculated:
                    return PrecalculatedDateTimeZone.ReadLegacy(this, id);
                case LegacyDateTimeZoneWriter.FlagTimeZoneNull:
                    return null; // Only used when reading a tail zone
                case LegacyDateTimeZoneWriter.FlagTimeZoneCached:
                    return CachedDateTimeZone.Read(this, id);
                case LegacyDateTimeZoneWriter.FlagTimeZoneDst:
                    return DaylightSavingsDateTimeZone.ReadLegacy(this, id);
                default:
                    throw new InvalidNodaDataException("Unknown time zone flag type: " + flag);
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
        /// <exception cref="InvalidNodaDataException">The data in the stream has been exhausted</exception>
        internal byte ReadByte()
        {
            int value = input.ReadByte();
            if (value == -1)
            {
                throw new InvalidNodaDataException("Unexpected end of data stream");
            }
            return (byte)value;
        }
    }
}
