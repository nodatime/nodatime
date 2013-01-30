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
        internal static class ZoneIntervalConstants
        {
            /// <summary>The instant to use as an 'epoch' when writing out a number of minutes-since-epoch.</summary>
            internal static readonly Instant EpochForMinutesSinceEpoch = Instant.FromUtc(1800, 1, 1, 0, 0);

            /// <summary>The marker value representing the beginning of time.</summary>
            internal const int MarkerMinValue = 0;
            /// <summary>The marker value representing the end of time.</summary>
            internal const int MarkerMaxValue = 1;
            /// <summary>The marker value representing an instant as a fixed 64-bit number of ticks.</summary>
            internal const int MarkerRaw = 2;
            /// <summary>The minimum varint value that represents an number of hours-since-previous.</summary>
            /// <remarks>Values below value are reserved for markers.</remarks>
            internal const int MinValueForHoursSincePrevious = 1 << 7;
            /// <summary>The minimum varint value that represents an number of minutes since an epoch.</summary>
            /// <remarks>Values below this are interpreted as hours-since-previous (for a range of about 240 years),
            /// rather than minutes-since-epoch (for a range of about 4000 years)
            /// This choice is somewhat arbitrary, though it results in hour values always taking 2 (or
            /// occasionally 3) bytes when encoded as a varint, while minute values take 4 (or conceivably 5).</remarks>
            internal const int MinValueForMinutesSinceEpoch = 1 << 21;
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
        /// </remarks>
        /// <param name="count">The value to write.</param>
        public void WriteSignedCount(int count)
        {
            unchecked
            {
                WriteVarint((uint) ((count >> 31) ^ (count << 1)));  // zigzag encoding
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

        public void WriteZoneIntervalTransition(Instant? previous, Instant value)
        {
            if (previous != null) {
                Preconditions.CheckArgumentRange("value", value.Ticks, previous.Value.Ticks, long.MaxValue);
            }

            unchecked
            {
                if (value == Instant.MinValue)
                {
                    WriteCount(ZoneIntervalConstants.MarkerMinValue);
                    return;
                }
                if (value == Instant.MaxValue)
                {
                    WriteCount(ZoneIntervalConstants.MarkerMaxValue);
                    return;
                }

                // In practice, most zone interval transitions will occur within 4000-6000 hours of the previous one
                // (i.e. about 5-8 months), and at an integral number of hours difference. We therefore gain a
                // significant reduction in output size by encoding transitions as the whole number of hours since the
                // previous, if possible.

                if (previous != null)
                {
                    // Note that the difference might exceed the range of a long, so we can't use a Duration here.
                    ulong ticks = (ulong) (value.Ticks - previous.Value.Ticks);
                    if (ticks % NodaConstants.TicksPerHour == 0)
                    {
                        ulong hours = ticks / NodaConstants.TicksPerHour;
                        // As noted above, this will generally fall within the 4000-6000 range, although values up to
                        // ~700,000 exist in TZDB.
                        if (ZoneIntervalConstants.MinValueForHoursSincePrevious <= hours &&
                            hours < ZoneIntervalConstants.MinValueForMinutesSinceEpoch)
                        {
                            WriteCount((int) hours);
                            return;
                        }
                    }
                }

                // We can't write the transition out relative to the previous transition, so let's next try writing it
                // out as a whole number of minutes since an (arbitrary, known) epoch.
                if (value >= ZoneIntervalConstants.EpochForMinutesSinceEpoch)
                {
                    ulong ticks = (ulong) (value.Ticks - ZoneIntervalConstants.EpochForMinutesSinceEpoch.Ticks);
                    if (ticks % NodaConstants.TicksPerMinute == 0)
                    {
                        ulong minutes = ticks / NodaConstants.TicksPerMinute;
                        // We typically have a count on the order of 80M here.
                        if (ZoneIntervalConstants.MinValueForMinutesSinceEpoch < minutes && minutes <= int.MaxValue)
                        {
                            WriteCount((int) minutes);
                            return;
                        }
                    }
                }
                // Otherwise, just write out a marker followed by the instant as a 64-bit number of ticks.  Note that
                // while most of the values we write here are actually whole numbers of _seconds_, optimising for that
                // case will save around 2KB (with tzdb 2012j), so doesn't seem worthwhile.
                WriteCount(ZoneIntervalConstants.MarkerRaw);
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
