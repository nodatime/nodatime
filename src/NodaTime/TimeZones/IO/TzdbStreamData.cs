// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones.Cldr;
using NodaTime.Utility;
using System.Collections.Generic;
using System.IO;

namespace NodaTime.TimeZones.IO
{
    internal sealed class TzdbStreamData : ITzdbDataSource
    {
        private static readonly Dictionary<TzdbStreamFieldId, NodaAction<Builder, TzdbStreamField>> FieldHanders =
            new Dictionary<TzdbStreamFieldId, NodaAction<Builder, TzdbStreamField>>
        {
            { TzdbStreamFieldId.StringPool, (builder, field) => builder.HandleStringPoolField(field) },
            { TzdbStreamFieldId.TimeZone, (builder, field) => builder.HandleZoneField(field) },
            { TzdbStreamFieldId.TzdbIdMap, (builder, field) => builder.HandleTzdbIdMapField(field) },
            { TzdbStreamFieldId.TzdbVersion, (builder, field) => builder.HandleTzdbVersionField(field) },
            { TzdbStreamFieldId.CldrSupplementalWindowsZones, (builder, field) => builder.HandleSupplementalWindowsZonesField(field) },
            { TzdbStreamFieldId.WindowsAdditionalStandardNameToIdMapping, (builder, field) => builder.HandleWindowsAdditionalStandardNameToIdMappingField(field) },
            { TzdbStreamFieldId.ZoneLocations, (builder, field) => builder.HandleZoneLocationsField(field) }
        };

        private const int AcceptedVersion = 0;

        private readonly IList<string> stringPool;
        private readonly string tzdbVersion;
        private readonly IDictionary<string, string> tzdbIdMap;
        private readonly WindowsZones windowsZones;
        private readonly IDictionary<string, TzdbStreamField> zoneFields;
        private readonly IList<TzdbZoneLocation> zoneLocations;
        private readonly IDictionary<string, string> windowsAdditionalStandardNameToIdMapping;

        private TzdbStreamData(Builder builder)
        {
            stringPool = CheckNotNull(builder.stringPool, "string pool");
            tzdbIdMap = CheckNotNull(builder.tzdbIdMap, "TZDB alias map");
            tzdbVersion = CheckNotNull(builder.tzdbVersion, "TZDB version");
            windowsZones = CheckNotNull(builder.windowsZones, "CLDR Supplemental Windows Zones");
            zoneFields = builder.zoneFields;
            zoneLocations = builder.zoneLocations;

            // Add in the canonical IDs as mappings to themselves.
            foreach (var id in zoneFields.Keys)
            {
                tzdbIdMap[id] = id;
            }

            windowsAdditionalStandardNameToIdMapping = CheckNotNull(builder.windowsAdditionalStandardNameToIdMapping,
                "Windows additional standard name to ID mapping");
        }

        /// <inheritdoc />
        public string TzdbVersion { get { return tzdbVersion; } }

        /// <inheritdoc />
        public IDictionary<string, string> TzdbIdMap { get { return tzdbIdMap; } }

        /// <inheritdoc />
        public WindowsZones WindowsZones { get { return windowsZones; } }

        /// <inheritdoc />
        public IList<TzdbZoneLocation> ZoneLocations { get { return zoneLocations; } }

        /// <inheritdoc />
        public DateTimeZone CreateZone(string id, string canonicalId)
        {
            using (var stream = zoneFields[canonicalId].CreateStream())
            {
                var reader = new DateTimeZoneReader(stream, stringPool);
                // Skip over the ID before the zone data itself
                reader.ReadString();
                var type = (DateTimeZoneWriter.DateTimeZoneType)reader.ReadByte();
                switch (type)
                {
                    case DateTimeZoneWriter.DateTimeZoneType.Fixed:
                        return FixedDateTimeZone.Read(reader, id);
                    case DateTimeZoneWriter.DateTimeZoneType.Precalculated:
                        return CachedDateTimeZone.ForZone(PrecalculatedDateTimeZone.Read(reader, id));
                    default:
                            throw new InvalidNodaDataException("Unknown time zone type " + type);
                }
            }
        }

        /// <inheritdoc />
        public IDictionary<string, string> WindowsAdditionalStandardNameToIdMapping { get { return windowsAdditionalStandardNameToIdMapping; } }

        // Like Preconditions.CheckNotNull, but specifically for incomplete data.
        private static T CheckNotNull<T>(T input, string name) where T : class
        {
            if (input == null)
            {
                throw new InvalidNodaDataException("Incomplete TZDB data. Missing field: " + name);
            }
            return input;
        }

        internal static TzdbStreamData FromStream(Stream stream)
        {
            Preconditions.CheckNotNull(stream, "stream");
            int version = new BinaryReader(stream).ReadInt32();
            if (version != AcceptedVersion)
            {
                throw new InvalidNodaDataException("Unable to read stream with version " + version);
            }
            Builder builder = new Builder();
            foreach (var field in TzdbStreamField.ReadFields(stream))
            {
                // Only handle fields we know about
                NodaAction<Builder, TzdbStreamField> handler;
                if (FieldHanders.TryGetValue(field.Id, out handler))
                {
                    handler(builder, field);
                }
            }
            return new TzdbStreamData(builder);
        }

        /// <summary>
        /// Mutable builder class used during parsing.
        /// </summary>
        private class Builder
        {
            internal IList<string> stringPool;
            internal string tzdbVersion;
            internal IDictionary<string, string> tzdbIdMap;
            internal IList<TzdbZoneLocation> zoneLocations = null;
            internal WindowsZones windowsZones;
            internal readonly IDictionary<string, TzdbStreamField> zoneFields = new Dictionary<string, TzdbStreamField>();
            internal IDictionary<string, string> windowsAdditionalStandardNameToIdMapping;

            internal void HandleStringPoolField(TzdbStreamField field)
            {
                CheckSingleField(field, stringPool);
                using (var stream = field.CreateStream())
                {
                    var reader = new DateTimeZoneReader(stream, null);
                    int count = reader.ReadCount();
                    stringPool = new string[count];
                    for (int i = 0; i < count; i++)
                    {
                        stringPool[i] = reader.ReadString();
                    }
                }
            }

            internal void HandleZoneField(TzdbStreamField field)
            {
                CheckStringPoolPresence(field);
                // Just read the ID from the zone - we don't parse the data yet.
                // (We could, but we might as well be lazy.)
                using (var stream = field.CreateStream())
                {
                    var reader = new DateTimeZoneReader(stream, stringPool);
                    string id = reader.ReadString();
                    if (zoneFields.ContainsKey(id))
                    {
                        throw new InvalidNodaDataException("Multiple definitions for zone " + id);
                    }
                    zoneFields[id] = field;
                }
            }

            internal void HandleTzdbVersionField(TzdbStreamField field)
            {
                CheckSingleField(field, tzdbVersion);
                tzdbVersion = field.ExtractSingleValue(reader => reader.ReadString(), null);
            }

            internal void HandleTzdbIdMapField(TzdbStreamField field)
            {
                CheckSingleField(field, tzdbIdMap);
                tzdbIdMap = field.ExtractSingleValue(reader => reader.ReadDictionary(), stringPool);
            }

            internal void HandleSupplementalWindowsZonesField(TzdbStreamField field)
            {
                CheckSingleField(field, windowsZones);
                windowsZones = field.ExtractSingleValue(WindowsZones.Read, stringPool);
            }

            internal void HandleWindowsAdditionalStandardNameToIdMappingField(TzdbStreamField field)
            {
                // Even on the non-portable build, we still read the data: the cost is minimal, and it makes
                // it much simpler to validate.
                if (windowsZones == null)
                {
                    throw new InvalidNodaDataException("Field " + field.Id + " without earlier Windows mapping field");
                }
                windowsAdditionalStandardNameToIdMapping = field.ExtractSingleValue(reader => reader.ReadDictionary(), stringPool);
            }

            internal void HandleZoneLocationsField(TzdbStreamField field)
            {
                CheckSingleField(field, zoneLocations);
                CheckStringPoolPresence(field);
                using (var stream = field.CreateStream())
                {
                    var reader = new DateTimeZoneReader(stream, stringPool);
                    var count = reader.ReadCount();
                    var array = new TzdbZoneLocation[count];
                    for (int i = 0; i < count; i++)
                    {
                        array[i] = TzdbZoneLocation.Read(reader);
                    }
                    zoneLocations = array;
                }
            }

            private void CheckSingleField(TzdbStreamField field, object expectedNullField)
            {
                if (expectedNullField != null)
                {
                    throw new InvalidNodaDataException("Multiple fields of ID " + field.Id);
                }
            }

            private void CheckStringPoolPresence(TzdbStreamField field)
            {
                if (stringPool == null)
                {
                    throw new InvalidNodaDataException("String pool must be present before field " + field.Id);
                }
            }
        }
    }
}
