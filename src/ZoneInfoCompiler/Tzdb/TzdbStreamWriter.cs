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

using NodaTime.TimeZones;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NodaTime.TimeZones.IO;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    /// Implementation of ITzdbWriter which writes in a custom format directly
    /// to a stream.
    /// </summary>
    /// <remarks>
    /// <para>The file format consists of four bytes indicating the file format version/type (mostly for
    /// future expansion), followed by a number of fields. Each field is identified by a <see cref="TzdbStreamFieldId"/>.
    /// The fields are always written in order, and the format of a field consists of its field ID, a 7-bit-encoded
    /// integer with the size of the data, and then the data itself.
    /// </para>
    /// <para>
    /// The version number does not need to be increased if new fields are added, as the reader will simply ignore
    /// unknown fields. It only needs to be increased for incompatible changes such as a different time zone format,
    /// or if old fields are removed. 
    /// </para>
    /// </remarks>
    internal sealed class TzdbStreamWriter : ITzdbWriter
    {
        private const int Version = 0;

        private readonly Stream stream;

        internal TzdbStreamWriter(Stream stream)
        {
            this.stream = stream;
        }

        public void Write(TzdbDatabase database, WindowsMapping mapping)
        {
            FieldCollection fields = new FieldCollection();

            var stringPool = new List<string>();
            
            // First assemble the fields (writing to the string pool as we go)
            var timeZoneMap = new Dictionary<string, string>();
            foreach (var zone in database.GenerateDateTimeZones())
            {
                timeZoneMap.Add(zone.Id, zone.Id);
                var zoneField = fields.AddField(TzdbStreamFieldId.TimeZone, stringPool);
                zoneField.Writer.WriteString(zone.Id);
                zoneField.Writer.WriteTimeZone(zone);
            }

            fields.AddField(TzdbStreamFieldId.TzdbVersion, null).Writer.WriteString(database.Version);
            fields.AddField(TzdbStreamFieldId.WindowsMappingVersion, null).Writer.WriteString(mapping.Version);

            // Normalize the aliases
            foreach (var key in database.Aliases.Keys)
            {
                var value = database.Aliases[key];
                while (database.Aliases.ContainsKey(value))
                {
                    value = database.Aliases[value];
                }
                timeZoneMap.Add(key, value);
            }

            fields.AddField(TzdbStreamFieldId.TzdbIdMap, stringPool).Writer.WriteDictionary(timeZoneMap);
            fields.AddField(TzdbStreamFieldId.WindowsMapping, stringPool).Writer.WriteDictionary(mapping.WindowsToTzdbIds);

            var stringPoolField = fields.AddField(TzdbStreamFieldId.StringPool, null);
            stringPoolField.Writer.WriteCount(stringPool.Count);
            foreach (string value in stringPool)
            {
                stringPoolField.Writer.WriteString(value);
            }

            // Now write all the fields out, in the right order.
            new BinaryWriter(stream).Write(Version);
            fields.WriteTo(stream);
            
            stream.Close();
        }

        /// <summary>
        /// The data for a field, including the field number itself.
        /// </summary>
        private class FieldData
        {
            internal readonly MemoryStream stream;
            private readonly TzdbStreamFieldId fieldId;
            private readonly DateTimeZoneWriter writer;

            internal FieldData(TzdbStreamFieldId fieldId, IList<string> stringPool)
            {
                this.fieldId = fieldId;
                this.stream = new MemoryStream();
                this.writer = new DateTimeZoneWriter(stream, stringPool);
            }

            internal DateTimeZoneWriter Writer { get { return writer; } }
            internal TzdbStreamFieldId FieldId { get { return fieldId; } }

            internal void WriteTo(Stream output)
            {
                output.WriteByte((byte)fieldId);
                int length = (int) stream.Length;
                // Write 7-bit-encoded integer
                while (length > 0x7f)
                {
                    output.WriteByte((byte) (0x80 | (length & 0x7f)));
                    length = length >> 7;
                }
                output.WriteByte((byte) (length & 0x7f));
                stream.WriteTo(output);
            }
        }

        private class FieldCollection
        {
            private readonly List<FieldData> fields = new List<FieldData>();

            internal FieldData AddField(TzdbStreamFieldId fieldNumber, IList<string> stringPool)
            {
                FieldData ret = new FieldData(fieldNumber, stringPool);
                fields.Add(ret);
                return ret;
            }

            internal void WriteTo(Stream stream)
            {
                foreach (var field in fields.OrderBy(field => field.FieldId))
                {
                    field.WriteTo(stream);
                }
            }
        }
    }
}
