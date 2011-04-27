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
    ///   Provides an <see cref = "DateTimeZone" /> writer that simply writes the values
    ///   without any compression. Can be used as a base for implementing specific 
    ///   compression writers by overriding the methods for the types to be compressed.
    /// </summary>
    internal class DateTimeZoneWriter
    {
        public const byte FlagTimeZoneCached = 0;
        public const byte FlagTimeZoneDst = 1;
        public const byte FlagTimeZoneFixed = 2;
        public const byte FlagTimeZoneNull = 3;
        public const byte FlagTimeZonePrecalculated = 4;
        public const byte FlagTimeZoneUser = 5;

        protected readonly Stream Output;

        /// <summary>
        ///   Constructs a DateTimeZoneWriter.
        /// </summary>
        /// <param name = "output">Where to send the serialized output.</param>
        public DateTimeZoneWriter(Stream output)
        {
            Output = output;
        }

        #region DateTimeZoneWriter Members
        /// <summary>
        ///   Writes a boolean value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        public virtual void WriteBoolean(bool value)
        {
            WriteInt8((byte)(value ? 1 : 0));
        }

        /// <summary>
        ///   Writes the given non-negative integer value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        public virtual void WriteCount(int value)
        {
            WriteInt32(value);
        }

        /// <summary>
        ///   Writes the given dictionary of string to string to the stream.
        /// </summary>
        /// <param name = "dictionary">The <see cref = "IDictionary{TKey,TValue}" /> to write.</param>
        public virtual void WriteDictionary(IDictionary<string, string> dictionary)
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
        ///   Writes an enumeration's integer value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        public virtual void WriteEnum(int value)
        {
            WriteInteger(value);
        }

        /// <summary>
        ///   Writes the <see cref = "Instant" /> value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        public virtual void WriteInstant(Instant value)
        {
            WriteTicks(value.Ticks);
        }

        /// <summary>
        ///   Writes the integer value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        public virtual void WriteInteger(int value)
        {
            WriteInt32(value);
        }

        /// <summary>
        ///   Writes the <see cref = "LocalInstant" /> value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        public virtual void WriteLocalInstant(LocalInstant value)
        {
            WriteTicks(value.Ticks);
        }

        /// <summary>
        ///   Writes the integer milliseconds value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        public virtual void WriteMilliseconds(int value)
        {
            WriteInt32(value);
        }

        /// <summary>
        ///   Writes the <see cref = "Offset" /> value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        public virtual void WriteOffset(Offset value)
        {
            WriteMilliseconds(value.Milliseconds);
        }

        /// <summary>
        ///   Writes the string value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        public virtual void WriteString(string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            int length = data.Length;
            WriteCount(length);
            Output.Write(data, 0, data.Length);
        }

        /// <summary>
        ///   Writes the long ticks value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        public virtual void WriteTicks(long value)
        {
            WriteInt64(value);
        }

        /// <summary>
        ///   Writes the <see cref = "DateTimeZone" /> value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        public virtual void WriteTimeZone(DateTimeZone value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value is FixedDateTimeZone)
            {
                WriteInt8(FlagTimeZoneFixed);
            }
            else if (value is PrecalculatedDateTimeZone)
            {
                WriteInt8(FlagTimeZonePrecalculated);
            }
            else if (value is CachedDateTimeZone)
            {
                WriteInt8(FlagTimeZoneCached);
            }
            else if (value is DaylightSavingsTimeZone)
            {
                WriteInt8(FlagTimeZoneDst);
            }
            else if (value is NullDateTimeZone)
            {
                WriteInt8(FlagTimeZoneNull);
            }
            else
            {
                WriteInt8(FlagTimeZoneUser);
                WriteString(value.GetType().AssemblyQualifiedName);
            }
            value.Write(this);
        }
        #endregion

        /// <summary>
        ///   Writes the given 16 bit integer value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        protected void WriteInt16(short value)
        {
            unchecked
            {
                WriteInt8((byte)((value >> 8) & 0xff));
                WriteInt8((byte)(value & 0xff));
            }
        }

        /// <summary>
        ///   Writes the given 32 bit integer value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        protected void WriteInt32(int value)
        {
            unchecked
            {
                WriteInt16((short)(value >> 16));
                WriteInt16((short)value);
            }
        }

        /// <summary>
        ///   Writes the given 64 bit integer value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        protected void WriteInt64(long value)
        {
            unchecked
            {
                WriteInt32((int)(value >> 32));
                WriteInt32((int)value);
            }
        }

        /// <summary>
        ///   Writes the given 8 bit integer value to the stream.
        /// </summary>
        /// <param name = "value">The value to write.</param>
        protected void WriteInt8(byte value)
        {
            unchecked
            {
                Output.WriteByte(value);
            }
        }
    }
}
