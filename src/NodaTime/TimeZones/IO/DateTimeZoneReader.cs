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

namespace NodaTime.TimeZones.IO
{
    /// <summary>
    /// Implementation of <see cref="IDateTimeZoneReader"/> for the most recent version
    /// of the "blob" format of time zone data. If the format changes, this class will be
    /// renamed (e.g. to DateTimeZoneReaderV0) and the new implementation will replace it.
    /// </summary>
    internal sealed class DateTimeZoneReader : IDateTimeZoneReader
    {
        private readonly Stream input;
        private readonly IList<string> stringPool; 

        internal DateTimeZoneReader(Stream input, IList<string> stringPool)
        {
            this.input = input;
            this.stringPool = stringPool;
        }

        /// <summary>
        /// Reads a non-negative integer value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteCount" />, which
        /// documents the format.
        /// </remarks>
        /// <returns>The integer value from the stream.</returns>
        public int ReadCount()
        {
            unchecked
            {
                int ret = 0;
                int shift = 0;
                while (true)
                {
                    int nextByte = ReadByte();
                    ret += (nextByte & 0x7f) << shift;
                    shift += 7;
                    if (nextByte < 0x80)
                    {
                        return ret;
                    }
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
        public Offset ReadOffset()
        {
            unchecked
            {
                byte firstByte = ReadByte();

                int millis;

                if ((firstByte & 0x80) == 0)
                {
                    millis = firstByte * (30 * NodaConstants.MillisecondsPerMinute);
                }
                else
                {
                    int flag = firstByte & 0xe0;      // The flag parts of the first byte
                    int firstData = firstByte & 0x1f; // The data parts of the first byte
                    switch (firstByte & 0xe0)
                    {
                        case 0x80: // Minutes
                            int minutes = (firstData << 8) + ReadByte();
                            millis = minutes * NodaConstants.MillisecondsPerMinute;
                            break;
                        case 0xa0: // Seconds
                            int seconds = (firstData << 16) + (ReadInt16() & 0xffff);
                            millis = seconds * NodaConstants.MillisecondsPerSecond;
                            break;
                        case 0xc0: // Milliseconds
                            millis = (firstData << 24) + (ReadByte() << 16) + (ReadInt16() & 0xffff);
                            break;
                        default:
                            // TODO: Convert this to an appropriate "invalid data detected" exception.
                            throw new IOException("Invalid data");
                    }                    
                }
                millis -= NodaConstants.MillisecondsPerStandardDay;
                return Offset.FromMilliseconds(millis);
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
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteInstant" />.
        /// </remarks>
        /// <returns>The <see cref="Instant" /> value from the stream.</returns>
        public Instant ReadInstant()
        {
            unchecked
            {
                byte firstByte = ReadByte();
                if (firstByte == DateTimeZoneWriter.InstantConstants.MaxFormat)
                {
                    return Instant.MaxValue;
                }
                if (firstByte == DateTimeZoneWriter.InstantConstants.MinFormat)
                {
                    return Instant.MinValue;
                }
                long firstValue = firstByte & 0x3f;
                switch (firstByte & 0xc0)
                {
                    case DateTimeZoneWriter.InstantConstants.HoursFormat:
                        long hours = (firstValue << 16) | (ushort) ReadInt16();
                        return DateTimeZoneWriter.InstantConstants.Epoch + Duration.FromHours(hours);
                    case DateTimeZoneWriter.InstantConstants.MinutesFormat:
                        long minutes = (firstValue << 24) | (uint) (ReadByte() << 16) | (ushort) ReadInt16();
                        return DateTimeZoneWriter.InstantConstants.Epoch + Duration.FromMinutes(minutes);
                    case DateTimeZoneWriter.InstantConstants.SecondsFormat:
                        long seconds = (firstValue << 32) | (uint) ReadInt32();
                        return DateTimeZoneWriter.InstantConstants.Epoch + Duration.FromSeconds(seconds);
                    case DateTimeZoneWriter.InstantConstants.RawFormat:
                        return new Instant(ReadInt64());
                    default:
                        throw new InvalidOperationException("Bug in noda time - invalid byte value!");
                }
            }
        }

        /// <summary>
        /// Reads a string value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteString" />.
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
                        throw new EndOfStreamException("Unexpectedly reached end of data with " + (length - offset) + " bytes still to read");
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
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteTimeZone" />.
        /// </remarks>
        /// <returns>The <see cref="DateTimeZone" /> value from the stream.</returns>
        public DateTimeZone ReadTimeZone(string id)
        {
            int flag = ReadByte();
            switch (flag)
            {
                case DateTimeZoneWriter.FlagTimeZoneFixed:
                    return FixedDateTimeZone.Read(this, id);
                case DateTimeZoneWriter.FlagTimeZonePrecalculated:
                    return CachedDateTimeZone.ForZone(PrecalculatedDateTimeZone.Read(this, id));
                case DateTimeZoneWriter.FlagTimeZoneNull:
                    return null; // Only used when reading a tail zone
                case DateTimeZoneWriter.FlagTimeZoneDst:
                    return CachedDateTimeZone.ForZone(DaylightSavingsDateTimeZone.Read(this, id));
                default:
                    throw new IOException("Unknown flag type " + flag);
            }
        }

        /// <summary>
        ///   Reads a signed 16 bit integer value from the stream and returns it as an int.
        /// </summary>
        /// <returns>The 16 bit int value.</returns>
        private int ReadInt16()
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
        private int ReadInt32()
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
        private long ReadInt64()
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
        private byte ReadByte()
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
