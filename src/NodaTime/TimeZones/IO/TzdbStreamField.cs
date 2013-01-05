#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2013 Jon Skeet
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
