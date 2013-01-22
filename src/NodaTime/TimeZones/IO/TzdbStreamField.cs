// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.IO;

namespace NodaTime.TimeZones.IO
{
    /// <summary>
    /// An unparsed field within a stream.
    /// </summary>
    internal sealed class TzdbStreamField
    {
        private readonly TzdbStreamFieldId id;
        private readonly byte[] data;

        private TzdbStreamField(TzdbStreamFieldId id, byte[] data)
        {
            this.id = id;
            this.data = data;
        }

        internal TzdbStreamFieldId Id { get { return id; } }

        /// <summary>
        /// Creates a new read-only stream over the data for this field.
        /// </summary>
        internal Stream CreateStream()
        {
            return new MemoryStream(data, false);
        }

        internal T ExtractSingleValue<T>(NodaFunc<DateTimeZoneReader, T> readerFunction, IList<string> stringPool)
        {
            using (var stream = CreateStream())
            {
                return readerFunction(new DateTimeZoneReader(stream, stringPool));
            }
        }

        internal static IEnumerable<TzdbStreamField> ReadFields(Stream input)
        {
            while (true)
            {
                int fieldId = input.ReadByte();
                if (fieldId == -1)
                {
                    yield break;
                }
                TzdbStreamFieldId id = (TzdbStreamFieldId) (byte) fieldId;
                // Read 7-bit-encoded length
                int length = new DateTimeZoneReader(input, null).ReadCount();
                byte[] data = new byte[length];
                int offset = 0;
                while (offset < data.Length)
                {
                    int bytesRead = input.Read(data, offset, data.Length - offset);
                    if (bytesRead == 0)
                    {
                        throw new EndOfStreamException("Stream ended after reading " + offset + " bytes out of " + data.Length);
                    }
                    offset += bytesRead;
                }
                yield return new TzdbStreamField(id, data);
            }
        }
    }
}
