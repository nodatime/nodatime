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
            { TzdbStreamFieldId.GeoLocations, (builder, field) => builder.HandleGeoLocationsField(field) }
        };

        private const int AcceptedVersion = 0;

        private readonly IList<string> stringPool;
        private readonly string tzdbVersion;
        private readonly IDictionary<string, string> tzdbIdMap;
        private readonly WindowsZones windowsZones;
        private readonly IDictionary<string, TzdbStreamField> zoneFields;
        private readonly IList<TzdbGeoLocation> geoLocations;
#if PCL
        private readonly IDictionary<string, string> windowsAdditionalStandardNameToIdMapping;
#endif

        private TzdbStreamData(Builder builder)
        {
            stringPool = CheckNotNull(builder.stringPool, "string pool");
            tzdbIdMap = CheckNotNull(builder.tzdbIdMap, "TZDB alias map");
            tzdbVersion = CheckNotNull(builder.tzdbVersion, "TZDB version");
            windowsZones = CheckNotNull(builder.windowsZones, "CLDR Supplemental Windows Zones");
            zoneFields = builder.zoneFields;
            // Check that each alias has a canonical value.
            foreach (var id in tzdbIdMap.Values)
            {
                if (!zoneFields.ContainsKey(id))
                {
                    throw new InvalidNodaDataException("Zone field for ID " + id + " is missing");
                }
            }
            // Add in the canonical IDs as mappings to themselves.
            foreach (var id in zoneFields.Keys)
            {
                tzdbIdMap[id] = id;
            }

            // Check that each Windows mapping has a known canonical ID.
            foreach (var mapZone in windowsZones.MapZones)
            {
                foreach (var id in mapZone.TzdbIds)
                {
                    if (!tzdbIdMap.ContainsKey(id))
                    {
                        throw new InvalidNodaDataException("Windows mapping uses canonical ID " + id + " which is missing");
                    }
                }
            }
#if PCL
            windowsAdditionalStandardNameToIdMapping = CheckNotNull(builder.windowsAdditionalStandardNameToIdMapping,
                "Windows additional standard name to ID mapping");
            // Check that each additional Windows standard name mapping has a known canonical ID.
            foreach (var id in windowsAdditionalStandardNameToIdMapping.Values)
            {
                if (!tzdbIdMap.ContainsKey(id))
                {
                    throw new InvalidNodaDataException("Windows additional standard name mapping uses canonical ID " + id + " which is missing");
                }
            }
#endif
            geoLocations = builder.geoLocations;

            // Check that each geolocation has a valid zone ID
            if (geoLocations != null)
            {
                foreach (var location in geoLocations)
                {
                    if (!tzdbIdMap.ContainsKey(location.ZoneId))
                    {
                        throw new InvalidNodaDataException("Geolocation " + location.CountryName
                            + " uses zone ID " + location.ZoneId + " which is missing");
                    }
                }
            }
        }

        /// <inheritdoc />
        public string TzdbVersion { get { return tzdbVersion; } }

        /// <inheritdoc />
        public IDictionary<string, string> TzdbIdMap { get { return tzdbIdMap; } }

        /// <inheritdoc />
        public WindowsZones WindowsZones { get { return windowsZones; } }

        /// <inheritdoc />
        public IList<TzdbGeoLocation> GeoLocations { get { return geoLocations; } }

        /// <inheritdoc />
        public DateTimeZone CreateZone(string id, string canonicalId)
        {
            using (var stream = zoneFields[canonicalId].CreateStream())
            {
                var reader = new DateTimeZoneReader(stream, stringPool);
                // Skip over the ID before the zone data itself
                reader.ReadString();
                return reader.ReadTimeZone(id);
            }
        }

#if PCL
        /// <inheritdoc />
        public IDictionary<string, string> WindowsAdditionalStandardNameToIdMapping { get { return windowsAdditionalStandardNameToIdMapping; } }
#endif

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
            internal IList<TzdbGeoLocation> geoLocations = null;
            internal WindowsZones windowsZones;
            internal readonly IDictionary<string, TzdbStreamField> zoneFields = new Dictionary<string, TzdbStreamField>();
#if PCL
            internal IDictionary<string, string> windowsAdditionalStandardNameToIdMapping;
#endif

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
// If we're not on the PCL, we don't need to do anything. This is just to support zones where the StandardName
// isn't the same as the Id.
#if PCL
                if (windowsZones == null)
                {
                    throw new InvalidNodaDataException("Field " + field.Id + " without earlier Windows mapping field");
                }
                windowsAdditionalStandardNameToIdMapping = field.ExtractSingleValue(reader => reader.ReadDictionary(), stringPool);
#endif         
            }

            internal void HandleGeoLocationsField(TzdbStreamField field)
            {
                CheckSingleField(field, geoLocations);
                CheckStringPoolPresence(field);
                using (var stream = field.CreateStream())
                {
                    var reader = new DateTimeZoneReader(stream, stringPool);
                    var count = reader.ReadCount();
                    var array = new TzdbGeoLocation[count];
                    for (int i = 0; i < count; i++)
                    {
                        array[i] = TzdbGeoLocation.Read(reader);
                    }
                    geoLocations = array;
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
