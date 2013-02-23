// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NodaTime.TimeZones.IO;
using NodaTime.Utility;

namespace NodaTime.TimeZones.Cldr
{
    /// <summary>
    /// Representation of the windowsZones part of CLDR supplemental data.
    /// </summary>
    internal sealed class WindowsZones
    {        
        private readonly string version;
        public string Version { get { return version; } }

        private readonly ReadOnlyCollection<MapZone> mapZones;
        public IList<MapZone> MapZones { get { return mapZones; } }

        private readonly NodaReadOnlyDictionary<string, string> primaryMapping;
        public IDictionary<string, string> PrimaryMapping { get { return primaryMapping; } }

        internal WindowsZones(string version, IList<MapZone> mapZones)
            : this(Preconditions.CheckNotNull(version, "version"),
                   new ReadOnlyCollection<MapZone>(new List<MapZone>(Preconditions.CheckNotNull(mapZones, "mapZones"))))
        {
        }

        private WindowsZones(string version, ReadOnlyCollection<MapZone> mapZones)
        {
            this.version = version;
            this.mapZones = mapZones;
            this.primaryMapping = new NodaReadOnlyDictionary<string, string>(
                mapZones.Where(z => z.Territory == MapZone.PrimaryTerritory)
                        .ToDictionary(z => z.WindowsId, z => z.TzdbIds.Single()));
        }

        private WindowsZones(string version, NodaReadOnlyDictionary<string, string> primaryMapping)
        {
            this.version = version;
            this.primaryMapping = primaryMapping;
            var mapZoneList = new List<MapZone>(primaryMapping.Count);
            foreach (var entry in primaryMapping)
            {
                mapZoneList.Add(new MapZone(entry.Key, MapZone.PrimaryTerritory, new[] { entry.Value }));
            }
            mapZones = new ReadOnlyCollection<MapZone>(mapZoneList);
        }

        internal static WindowsZones FromPrimaryMapping(string version, IDictionary<string, string> mappings)
        {
            return new WindowsZones(Preconditions.CheckNotNull(version, "version"),
                new NodaReadOnlyDictionary<string, string>(Preconditions.CheckNotNull(mappings, "mappings")));
        }

        internal static WindowsZones Read(IDateTimeZoneReader reader)
        {
            string version = reader.ReadString();
            int count = reader.ReadCount();
            var mapZones = new MapZone[count];
            for (int i = 0; i < count; i++)
            {
                mapZones[i] = MapZone.Read(reader);
            }
            return new WindowsZones(version, new ReadOnlyCollection<MapZone>(mapZones));
        }

        internal void Write(IDateTimeZoneWriter writer)
        {
            writer.WriteString(version);
            writer.WriteCount(mapZones.Count);
            foreach (var mapZone in mapZones)
            {
                mapZone.Write(writer);
            }
        }
    }
}
