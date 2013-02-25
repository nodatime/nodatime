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
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    internal sealed class WindowsZones
    {        
        private readonly string version;
        /// <summary>
        /// Gets the version of the Windows zones mapping data read from the original file.
        /// </summary>
        public string Version { get { return version; } }

        private readonly ReadOnlyCollection<MapZone> mapZones;
        /// <summary>
        /// Gets an immutable collection of mappings from Windows system time zones to
        /// TZDB time zones.
        /// </summary>
        /// <remarks>
        /// Mappings for a single Windows system time zone can appear multiple times
        /// in this list, in different territories. For example, "Central Standard Time"
        /// maps to different TZDB zones in different countries (the US, Canada, Mexico) and
        /// even within a single territory there can be multiple zones. Every Windows system
        /// time zone has a "primary" entry with a territory code of "001" and a single
        /// corresponding TZDB zone.
        /// </remarks>
        public IList<MapZone> MapZones { get { return mapZones; } }

        private readonly NodaReadOnlyDictionary<string, string> primaryMapping;
        /// <summary>
        /// Gets an immutable dictionary of primary mappings, from Windows system time zone ID
        /// to TZDB zone ID. This corresponds to the "001" territory which is present for every zone
        /// within the mapping file.
        /// </summary>
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
