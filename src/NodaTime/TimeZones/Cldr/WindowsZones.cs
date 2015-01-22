// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.TimeZones.IO;
using NodaTime.Utility;

namespace NodaTime.TimeZones.Cldr
{
    /// <summary>
    /// Representation of the <c>&lt;windowsZones&gt;</c> element of CLDR supplemental data.
    /// </summary>
    /// <remarks>
    /// See <a href="http://cldr.unicode.org/development/development-process/design-proposals/extended-windows-olson-zid-mapping">the CLDR design proposal</a>
    /// for more details of the structure of the file from which data is taken.
    /// </remarks>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    [Immutable]
    public sealed class WindowsZones
    {
        /// <summary>
        /// Gets the version of the Windows zones mapping data read from the original file.
        /// </summary>
        /// <remarks>
        /// As with other IDs, this should largely be treated as an opaque string, but the current method for
        /// generating this from the mapping file extracts a number from an element such as <c>&lt;version number="$Revision: 7825 $"/&gt;</c>.
        /// This is a Subversion revision number, but that association should only be used for diagnostic curiosity, and never
        /// assumed in code.
        /// </remarks>
        /// <value>The version of the Windows zones mapping data read from the original file.</value>
        [NotNull] public string Version { get; }

        /// <summary>
        /// Gets the TZDB version this Windows zone mapping data was created from.
        /// </summary>
        /// <remarks>
        /// The CLDR mapping file usually lags behind the TZDB file somewhat - partly because the
        /// mappings themselves don't always change when the time zone data does. For example, it's entirely
        /// reasonable for a <see cref="TzdbDateTimeZoneSource"/> with a <see cref="TzdbDateTimeZoneSource.TzdbVersion">TzdbVersion</see> of
        /// "2013b" to be supply a <c>WindowsZones</c> object with a <c>TzdbVersion</c> of "2012f".
        /// </remarks>
        /// <value>The TZDB version this Windows zone mapping data was created from.</value>
        [NotNull] public string TzdbVersion { get; }

        /// <summary>
        /// Gets the Windows time zone database version this Windows zone mapping data was created from.
        /// </summary>
        /// <remarks>
        /// At the time of this writing, this is populated (by CLDR) from the registry key
        /// HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones\TzVersion,
        /// so "7dc0101" for example.
        /// </remarks>
        /// <value>The Windows time zone database version this Windows zone mapping data was created from.</value>
        public string WindowsVersion { get; }

        /// <summary>
        /// Gets an immutable collection of mappings from Windows system time zones to
        /// TZDB time zones.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Each mapping consists of a single Windows time zone ID and a single
        /// territory to potentially multiple TZDB IDs that are broadly equivalent to that Windows zone/territory
        /// pair.
        /// </para>
        /// <para>
        /// Mappings for a single Windows system time zone can appear multiple times
        /// in this list, in different territories. For example, "Central Standard Time"
        /// maps to different TZDB zones in different countries (the US, Canada, Mexico) and
        /// even within a single territory there can be multiple zones. Every Windows system time zone covered within
        /// this collection has a "primary" entry with a territory code of "001" (which is the value of
        /// <see cref="MapZone.PrimaryTerritory"/>) and a single corresponding TZDB zone. 
        /// </para>
        /// <para>This collection is not guaranteed to cover every Windows time zone. Some zones may be unmappable
        /// (such as "Mid-Atlantic Standard Time") and there can be a delay between a new Windows time zone being introduced
        /// and it appearing in CLDR, ready to be used by Noda Time. (There's also bound to be a delay between it appearing
        /// in CLDR and being used in your production system.) In practice however, you're unlikely to wish to use a time zone
        /// which isn't covered here.</para>
        /// </remarks>
        /// <value>An immutable collection of mappings from Windows system time zones to
        /// TZDB time zones.</value>
        [NotNull] public IList<MapZone> MapZones { get; }

        /// <summary>
        /// Gets an immutable dictionary of primary mappings, from Windows system time zone ID
        /// to TZDB zone ID. This corresponds to the "001" territory which is present for every zone
        /// within the mapping file.
        /// </summary>
        /// <value>An immutable dictionary of primary mappings, from Windows system time zone ID
        /// to TZDB zone ID.</value>
        [NotNull] public IDictionary<string, string> PrimaryMapping { get; }

        internal WindowsZones([NotNull] string version, [NotNull] string tzdbVersion,
            [NotNull] string windowsVersion, [NotNull] IList<MapZone> mapZones)
            : this(Preconditions.CheckNotNull(version, nameof(version)),
                   Preconditions.CheckNotNull(tzdbVersion, nameof(tzdbVersion)),
                   Preconditions.CheckNotNull(windowsVersion, nameof(windowsVersion)),
                   new ReadOnlyCollection<MapZone>(new List<MapZone>(Preconditions.CheckNotNull(mapZones, nameof(mapZones)))))
        {
        }

        private WindowsZones(string version, string tzdbVersion, string windowsVersion, ReadOnlyCollection<MapZone> mapZones)
        {
            this.Version = version;
            this.TzdbVersion = tzdbVersion;
            this.WindowsVersion = windowsVersion;
            this.MapZones = mapZones;
            this.PrimaryMapping = new NodaReadOnlyDictionary<string, string>(
                mapZones.Where(z => z.Territory == MapZone.PrimaryTerritory)
                        .ToDictionary(z => z.WindowsId, z => z.TzdbIds.Single()));
        }

        private WindowsZones(string version, NodaReadOnlyDictionary<string, string> primaryMapping)
        {
            this.Version = version;
            this.WindowsVersion = "Unknown";
            this.TzdbVersion = "Unknown";
            this.PrimaryMapping = primaryMapping;
            var mapZoneList = new List<MapZone>(primaryMapping.Count);
            foreach (var entry in primaryMapping)
            {
                mapZoneList.Add(new MapZone(entry.Key, MapZone.PrimaryTerritory, new[] { entry.Value }));
            }
            MapZones = new ReadOnlyCollection<MapZone>(mapZoneList);
        }

        internal static WindowsZones FromPrimaryMapping([NotNull] string version, [NotNull] IDictionary<string, string> mappings)
        {
            return new WindowsZones(Preconditions.CheckNotNull(version, nameof(version)),
                new NodaReadOnlyDictionary<string, string>(Preconditions.CheckNotNull(mappings, nameof(mappings))));
        }

        internal static WindowsZones Read(IDateTimeZoneReader reader)
        {
            string version = reader.ReadString();
            string tzdbVersion = reader.ReadString();
            string windowsVersion = reader.ReadString();
            int count = reader.ReadCount();
            var mapZones = new MapZone[count];
            for (int i = 0; i < count; i++)
            {
                mapZones[i] = MapZone.Read(reader);
            }
            return new WindowsZones(version, tzdbVersion, windowsVersion, 
                new ReadOnlyCollection<MapZone>(mapZones));
        }

        internal void Write(IDateTimeZoneWriter writer)
        {
            writer.WriteString(Version);
            writer.WriteString(TzdbVersion);
            writer.WriteString(WindowsVersion);
            writer.WriteCount(MapZones.Count);
            foreach (var mapZone in MapZones)
            {
                mapZone.Write(writer);
            }
        }
    }
}
