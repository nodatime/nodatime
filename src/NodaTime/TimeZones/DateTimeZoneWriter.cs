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
using System.Text;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides an <see cref="DateTimeZone" /> writer that simply writes the values
    /// without any compression. Can be used as a base for implementing specific 
    /// compression writers by overriding the methods for the types to be compressed.
    /// </summary>
    // TODO: Consider renaming to TzdbDateTimeZoneWriter
    internal class DateTimeZoneWriter
    {
        internal const byte FlagTimeZoneCached = 0;
        internal const byte FlagTimeZoneDst = 1;
        internal const byte FlagTimeZoneFixed = 2;
        internal const byte FlagTimeZoneNull = 3;
        internal const byte FlagTimeZonePrecalculated = 4;

        protected readonly Stream Output;

        /// <summary>
        /// Constructs a DateTimeZoneWriter.
        /// </summary>
        /// <param name="output">Where to send the serialized output.</param>
        internal DateTimeZoneWriter(Stream output)
        {
            Output = output;
        }

        #region DateTimeZoneWriter Members
        /// <summary>
        /// Writes a boolean value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        internal void WriteBoolean(bool value)
        {
            WriteByte((byte)(value ? 1 : 0));
        }

        /// <summary>
        /// Writes the given non-negative integer value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        internal virtual void WriteCount(int value)
        {
            Preconditions.CheckArgumentRange("value", value, 0, int.MaxValue);
            WriteInt32(value);
        }

        /// <summary>
        /// Writes the <see cref="Instant" /> value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        internal void WriteInstant(Instant value)
        {
            WriteTicks(value.Ticks);
        }

        /// <summary>
        /// Writes the <see cref="LocalInstant" /> value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        internal void WriteLocalInstant(LocalInstant value)
        {
            WriteTicks(value.Ticks);
        }

        /// <summary>
        /// Writes the <see cref="Offset" /> value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        internal virtual void WriteOffset(Offset value)
        {
            WriteInt32(value.Milliseconds);
        }

        /// <summary>
        /// Writes the string value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        internal void WriteString(string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            int length = data.Length;
            WriteCount(length);
            Output.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Writes the long ticks value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        internal virtual void WriteTicks(long value)
        {
            WriteInt64(value);
        }

        /// <summary>
        /// Writes the <see cref="DateTimeZone" /> value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        internal void WriteTimeZone(DateTimeZone value)
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
                WriteByte(FlagTimeZoneCached);
                ((CachedDateTimeZone)value).Write(this);
            }
            else if (value is DaylightSavingsDateTimeZone)
            {
                WriteByte(FlagTimeZoneDst);
                ((DaylightSavingsDateTimeZone)value).Write(this);
            }
            else
            {
                throw new ArgumentException("Time zone type unknown to DateTimeZoneWriter");                
            }
        }
        #endregion

        /// <summary>
        /// Writes the given 16 bit integer value to the stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        protected void WriteInt16(short value)
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
        internal void WriteInt64(long value)
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
        internal void WriteByte(byte value)
        {
            unchecked
            {
                Output.WriteByte(value);
            }
        }
    }
}
