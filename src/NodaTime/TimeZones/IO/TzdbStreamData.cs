// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Annotations;
using NodaTime.TimeZones.Cldr;
using NodaTime.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace NodaTime.TimeZones.IO
{
    /// <summary>
    /// Provides the raw data exposed by <see cref="TzdbDateTimeZoneSource"/>.
    /// </summary>
    internal sealed class TzdbStreamData
    {
        private static readonly Dictionary<TzdbStreamFieldId, Action<Builder, TzdbStreamField>> FieldHandlers =
            new Dictionary<TzdbStreamFieldId, Action<Builder, TzdbStreamField>>
            {
                [TzdbStreamFieldId.StringPool] = (builder, field) => builder.HandleStringPoolField(field),
                [TzdbStreamFieldId.TimeZone] = (builder, field) => builder.HandleZoneField(field),
                [TzdbStreamFieldId.TzdbIdMap] = (builder, field) => builder.HandleTzdbIdMapField(field),
                [TzdbStreamFieldId.TzdbVersion] = (builder, field) => builder.HandleTzdbVersionField(field),
                [TzdbStreamFieldId.CldrSupplementalWindowsZones] = (builder, field) => builder.HandleSupplementalWindowsZonesField(field),
                [TzdbStreamFieldId.ZoneLocations] = (builder, field) => builder.HandleZoneLocationsField(field),
                [TzdbStreamFieldId.Zone1970Locations] = (builder, field) => builder.HandleZone1970LocationsField(field)
            };

        private const int AcceptedVersion = 0;

        private readonly IReadOnlyList<string> stringPool;
        private readonly IDictionary<string, TzdbStreamField> zoneFields;

        /// <summary>
        /// Returns the TZDB version string.
        /// </summary>
        public string TzdbVersion { get; }

        /// <summary>
        /// Returns the TZDB ID dictionary (alias to canonical ID).
        /// </summary>
        public ReadOnlyDictionary<string, string> TzdbIdMap { get; }

        /// <summary>
        /// Returns the Windows mapping dictionary. (As the type is immutable, it can be exposed directly
        /// to callers.)
        /// </summary>
        public WindowsZones WindowsMapping { get; }

        /// <summary>
        /// Returns the zone locations for the source, or null if no location data is available.
        /// </summary>
        public ReadOnlyCollection<TzdbZoneLocation>? ZoneLocations { get; }

        /// <summary>
        /// Returns the "zone 1970" locations for the source, or null if no such location data is available.
        /// </summary>
        public ReadOnlyCollection<TzdbZone1970Location>? Zone1970Locations { get; }

        [VisibleForTesting]
        internal TzdbStreamData(Builder builder)
        {
            stringPool = CheckNotNull(builder.stringPool, "string pool");
            var mutableIdMap = CheckNotNull(builder.tzdbIdMap, "TZDB alias map");
            TzdbVersion = CheckNotNull(builder.tzdbVersion, "TZDB version");
            WindowsMapping = CheckNotNull(builder.windowsMapping, "CLDR Supplemental Windows Zones");
            zoneFields = builder.zoneFields;
            ZoneLocations = builder.zoneLocations;
            Zone1970Locations = builder.zone1970Locations;

            // Add in the canonical IDs as mappings to themselves.
            foreach (var id in zoneFields.Keys)
            {
                mutableIdMap[id] = id;
            }
            TzdbIdMap = new ReadOnlyDictionary<string, string>(mutableIdMap);
        }

        /// <summary>
        /// Creates the <see cref="DateTimeZone"/> for the given canonical ID, which will definitely
        /// be one of the values of the TzdbAliases dictionary.
        /// </summary>
        /// <param name="id">ID for the returned zone, which may be an alias.</param>
        /// <param name="canonicalId">Canonical ID for zone data</param>
        public DateTimeZone CreateZone(string id, string canonicalId)
        {
            Preconditions.CheckNotNull(id, nameof(id));
            Preconditions.CheckNotNull(canonicalId, nameof(canonicalId));
            using (var stream = zoneFields[canonicalId].CreateStream())
            {
                var reader = new DateTimeZoneReader(stream, stringPool);
                // Skip over the ID before the zone data itself
                reader.ReadString();
                var type = (DateTimeZoneWriter.DateTimeZoneType) reader.ReadByte();
                return type switch
                {
                    DateTimeZoneWriter.DateTimeZoneType.Fixed => FixedDateTimeZone.Read(reader, id),
                    DateTimeZoneWriter.DateTimeZoneType.Precalculated =>
                    CachedDateTimeZone.ForZone(PrecalculatedDateTimeZone.Read(reader, id)),
                    _ => throw new InvalidNodaDataException($"Unknown time zone type {type}")
                };
            }
        }

        // Like Preconditions.CheckNotNull, but specifically for incomplete data.
        private static T CheckNotNull<T>(T? input, string name) where T : class
        {
            if (input is null)
            {
                throw new InvalidNodaDataException($"Incomplete TZDB data. Missing field: {name}");
            }
            return input;
        }

        internal static TzdbStreamData FromStream(Stream stream)
        {
            Preconditions.CheckNotNull(stream, nameof(stream));

            // Using statement to satisfy FxCop, but dispose won't do anything anyway, because
            // we deliberately leave the stream open.
            using (var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true))
            {
                int version = reader.ReadInt32();
                if (version != AcceptedVersion)
                {
                    throw new InvalidNodaDataException($"Unable to read stream with version {version}");
                }
            }
            Builder builder = new Builder();
            foreach (var field in TzdbStreamField.ReadFields(stream))
            {
                // Only handle fields we know about
                if (FieldHandlers.TryGetValue(field.Id, out Action<Builder, TzdbStreamField>? handler))
                {
                    handler(builder, field);
                }
            }
            return new TzdbStreamData(builder);
        }

        /// <summary>
        /// Mutable builder class used during parsing.
        /// </summary>
        [VisibleForTesting]
        internal class Builder
        {
            internal IReadOnlyList<string>? stringPool;
            internal string? tzdbVersion;
            // Note: deliberately mutable, as this is useful later when we map the canonical IDs to themselves.
            // This is a mapping of the aliases from TZDB, at this point.
            internal IDictionary<string, string>? tzdbIdMap;
            internal ReadOnlyCollection<TzdbZoneLocation>? zoneLocations;
            internal ReadOnlyCollection<TzdbZone1970Location>? zone1970Locations;
            internal WindowsZones? windowsMapping;
            internal readonly IDictionary<string, TzdbStreamField> zoneFields = new Dictionary<string, TzdbStreamField>();

            internal void HandleStringPoolField(TzdbStreamField field)
            {
                CheckSingleField(field, stringPool);
                using (var stream = field.CreateStream())
                {
                    var reader = new DateTimeZoneReader(stream, null);
                    int count = reader.ReadCount();
                    var stringPoolArray = new string[count];
                    for (int i = 0; i < count; i++)
                    {
                        stringPoolArray[i] = reader.ReadString();
                    }
                    stringPool = stringPoolArray;
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
                        throw new InvalidNodaDataException($"Multiple definitions for zone {id}");
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
                CheckSingleField(field, windowsMapping);
                windowsMapping = field.ExtractSingleValue(WindowsZones.Read, stringPool);
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
                    zoneLocations = Array.AsReadOnly(array);
                }
            }

            internal void HandleZone1970LocationsField(TzdbStreamField field)
            {
                CheckSingleField(field, zone1970Locations);
                CheckStringPoolPresence(field);
                using (var stream = field.CreateStream())
                {
                    var reader = new DateTimeZoneReader(stream, stringPool);
                    var count = reader.ReadCount();
                    var array = new TzdbZone1970Location[count];
                    for (int i = 0; i < count; i++)
                    {
                        array[i] = TzdbZone1970Location.Read(reader);
                    }
                    zone1970Locations = Array.AsReadOnly(array);
                }
            }

            private static void CheckSingleField(TzdbStreamField field, object? expectedNullField)
            {
                if (expectedNullField != null)
                {
                    throw new InvalidNodaDataException($"Multiple fields of ID {field.Id}");
                }
            }

            private void CheckStringPoolPresence(TzdbStreamField field)
            {
                if (stringPool is null)
                {
                    throw new InvalidNodaDataException($"String pool must be present before field {field.Id}");
                }
            }
        }
    }
}
