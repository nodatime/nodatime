// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.TimeZones;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NodaTime.TimeZones.IO;
using NodaTime.TimeZones.Cldr;

namespace NodaTime.TzdbCompiler.Tzdb
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

        public void Write(TzdbDatabase database, WindowsZones cldrWindowsZones)
        {
            FieldCollection fields = new FieldCollection();

            var zones = database.GenerateDateTimeZones().ToList();
            var stringPool = CreateOptimizedStringPool(zones, database.ZoneLocations, cldrWindowsZones);
            
            // First assemble the fields (writing to the string pool as we go)
            foreach (var zone in zones)
            {
                var zoneField = fields.AddField(TzdbStreamFieldId.TimeZone, stringPool);
                WriteZone(zone, zoneField.Writer);
            }

            fields.AddField(TzdbStreamFieldId.TzdbVersion, null).Writer.WriteString(database.Version);

            // Normalize the aliases
            var timeZoneMap = new Dictionary<string, string>();
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

            // Windows mappings
            cldrWindowsZones.Write(fields.AddField(TzdbStreamFieldId.CldrSupplementalWindowsZones, stringPool).Writer);
            fields.AddField(TzdbStreamFieldId.WindowsAdditionalStandardNameToIdMapping, stringPool).Writer.WriteDictionary
                (PclSupport.StandardNameToIdMap.ToDictionary(pair => pair.Key, pair => cldrWindowsZones.PrimaryMapping[pair.Value]));

            // Zone locations, if any.
            var zoneLocations = database.ZoneLocations;
            if (zoneLocations != null)
            {
                var field = fields.AddField(TzdbStreamFieldId.ZoneLocations, stringPool);
                field.Writer.WriteCount(zoneLocations.Count);
                foreach (var zoneLocation in zoneLocations)
                {
                    zoneLocation.Write(field.Writer);
                }
            }

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

        private static void WriteZone(DateTimeZone zone, IDateTimeZoneWriter writer)
        {
            writer.WriteString(zone.Id);
            // For cached zones, simply uncache first.
            var cachedZone = zone as CachedDateTimeZone;
            if (cachedZone != null)
            {
                zone = cachedZone.TimeZone;
            }
            var fixedZone = zone as FixedDateTimeZone;
            if (fixedZone != null)
            {
                writer.WriteByte((byte) DateTimeZoneWriter.DateTimeZoneType.Fixed);
                fixedZone.Write(writer);
            }
            else
            {
                var precalculatedZone = zone as PrecalculatedDateTimeZone;
                if (precalculatedZone != null)
                {
                    writer.WriteByte((byte) DateTimeZoneWriter.DateTimeZoneType.Precalculated);
                    precalculatedZone.Write(writer);
                }
                else
                {
                    throw new ArgumentException("Unserializable DateTimeZone type " + zone.GetType());
                }
            }
        }

        /// <summary>
        /// Creates a string pool which contains the most commonly-used strings within the given set
        /// of zones first. This will allow them to be more efficiently represented when we write them out for real.
        /// </summary>
        private static List<string> CreateOptimizedStringPool(IEnumerable<DateTimeZone> zones,
            IEnumerable<TzdbZoneLocation> zoneLocations, WindowsZones cldrWindowsZones)
        {
            var optimizingWriter = new StringPoolOptimizingFakeWriter();
            foreach (var zone in zones)
            {
                optimizingWriter.WriteString(zone.Id);
                WriteZone(zone, optimizingWriter);
            }
            if (zoneLocations != null)
            {
                foreach (var location in zoneLocations)
                {
                    location.Write(optimizingWriter);
                }
            }
            cldrWindowsZones.Write(optimizingWriter);
            return optimizingWriter.CreatePool();
        }

        /// <summary>
        /// Writer which only cares about strings. It builds a complete list of all strings written for the given
        /// zones, then creates a distinct list in most-prevalent-first order. This allows the most frequently-written
        /// strings to be the ones which are cheapest to write.
        /// </summary>
        private class StringPoolOptimizingFakeWriter : IDateTimeZoneWriter 
        {
            private readonly List<string> allStrings = new List<string>();

            public List<string> CreatePool()
            {
                return allStrings.GroupBy(x => x)
                                 .OrderByDescending(g => g.Count())
                                 .Select(g => g.Key)
                                 .ToList();
            }

            public void WriteString(string value)
            {
                allStrings.Add(value);
            }

            public void WriteOffset(Offset offset) {}
            public void WriteCount(int count) { }
            public void WriteByte(byte value) { }
            public void WriteSignedCount(int count) { }
            public void WriteZoneIntervalTransition(Instant? previous, Instant value) {}

            public void WriteDictionary(IDictionary<string, string> dictionary)
            {
                foreach (var entry in dictionary)
                {
                    WriteString(entry.Key);
                    WriteString(entry.Value);
                }
            }
        }

        /// <summary>
        /// The data for a field, including the field number itself.
        /// </summary>
        private class FieldData
        {
            private readonly MemoryStream stream;
            private readonly TzdbStreamFieldId fieldId;
            private readonly IDateTimeZoneWriter writer;

            internal FieldData(TzdbStreamFieldId fieldId, IList<string> stringPool)
            {
                this.fieldId = fieldId;
                this.stream = new MemoryStream();
                this.writer = new DateTimeZoneWriter(stream, stringPool);
            }

            internal IDateTimeZoneWriter Writer { get { return writer; } }
            internal TzdbStreamFieldId FieldId { get { return fieldId; } }

            internal void WriteTo(Stream output)
            {
                output.WriteByte((byte)fieldId);
                int length = (int) stream.Length;
                // We've got a 7-bit-encoding routine... might as well use it.
                new DateTimeZoneWriter(output, null).WriteCount(length);
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
