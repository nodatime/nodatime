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
using System.Reflection;
using System.Text;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides an <see cref="DateTimeZone" /> reader that simply reads the values
    /// without any compression. Can be used as a base for implementing specific 
    /// compression readers by overriding the methods for the types to be compressed.
    /// </summary>
    // TODO: Consider renaming to TzdbDateTimeZoneReader
    internal class DateTimeZoneReader
    {
        internal DateTimeZoneReader(Stream input)
        {
            Input = input;
        }

        protected Stream Input { get; private set; }

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
        /// Reads a non-negative integer value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteCount" />.
        /// </remarks>
        /// <returns>The integer value from the stream.</returns>
        internal virtual int ReadCount()
        {
            return ReadInt32();
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
        /// Reads an offset value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteOffset" />.
        /// </remarks>
        /// <returns>The integer millisecond value from the stream.</returns>
        internal virtual Offset ReadOffset()
        {
            return Offset.FromMilliseconds(ReadInt32());
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
                int bytesRead = Input.Read(data, 0, length);
                if (bytesRead <= 0)
                {
                    throw new EndOfStreamException("Unexpectedly reached end of data with " + (length - offset) + " bytes still to read");
                }
                offset += bytesRead;
            }
            return Encoding.UTF8.GetString(data, 0, length);
        }

        /// <summary>
        /// Reads a long ticks value from the stream.
        /// </summary>
        /// <remarks>
        /// The value must have been written by <see cref="DateTimeZoneWriter.WriteTicks" />.
        /// </remarks>
        /// <returns>The long ticks value from the stream.</returns>
        internal virtual long ReadTicks()
        {
            return ReadInt64();
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
                int high = ReadByte() & 0xff;
                int low = ReadByte() & 0xff;
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
            int value = Input.ReadByte();
            if (value == -1)
            {
                throw new EndOfStreamException();
            }
            return (byte)value;
        }
    }
}
