// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones.IO;
using NodaTime.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NodaTime.TimeZones.Cldr
{
    /// <summary>
    /// Represents a single entry in the CLDR Windows zone mapping file.
    /// </summary>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    public sealed class MapZone
    {
        /// <summary>
        /// Identifier used for the primary territory of each Windows time zone. A zone mapping with
        /// this territory will always have a single entry.
        /// </summary>
        public const string PrimaryTerritory = "001";
        /// <summary>
        /// Identifier used for the "fixed offset" territory. A zone mapping with
        /// this territory will always have a single entry.
        /// </summary>
        public const string FixedOffsetTerritory = "ZZ";

        private readonly string windowsId;
        private readonly string territory;
        private readonly IList<string> tzdbIds;

        /// <summary>
        /// Gets the Windows system time zone identifier for this mapping.
        /// </summary>
        public string WindowsId { get { return windowsId; } }

        /// <summary>
        /// Gets the territory code for this mapping.
        /// </summary>
        /// <remarks>
        /// This is typically either "001" to indicate that it's the primary territory for this ID, or
        /// "ZZ" to indicate a fixed-offset ID, or a different two-character capitalized code
        /// which indicates the geographical territory.
        /// </remarks>
        public string Territory { get { return territory; } }

        /// <summary>
        /// Gets a read-only collection of TZDB zone identifiers for this mapping.
        /// </summary>
        /// <remarks>
        /// For the primary and fixed-offset territory IDs ("001" and "ZZ") this is always
        /// a single zone ID.
        /// </remarks>
        public IList<string> TzdbIds { get { return tzdbIds; } }

        /// <summary>
        /// Creates a new mapping entry. This constructor is public mostly to allow code which needs
        /// to use this class to create 
        /// </summary>
        /// <param name="windowsId">Windows system time zone identifier. Must not be null.</param>
        /// <param name="territory">Territory code. Must not be null.</param>
        /// <param name="tzdbIds">List of territory codes. Must not be null, and must not
        /// contains null values.</param>
        public MapZone(string windowsId, string territory, IList<string> tzdbIds)
            : this(Preconditions.CheckNotNull(windowsId, "windowsId"),
                   Preconditions.CheckNotNull(territory, "territory"),
                   new ReadOnlyCollection<string>(new List<string>(Preconditions.CheckNotNull(tzdbIds, "tzdbIds"))))
        {
        }

        /// <summary>
        /// Private constructor to avoid unnecessary list copying (and validation) when deserializing.
        /// </summary>
        private MapZone(string windowsId, string territory, ReadOnlyCollection<string> tzdbIds)
        {
            this.windowsId = windowsId;
            this.territory = territory;
            this.tzdbIds = tzdbIds;
        }

        /// <summary>
        /// Reads a mapping from a reader.
        /// </summary>
        internal static MapZone Read(IDateTimeZoneReader reader)
        {
            string windowsId = reader.ReadString();
            string territory = reader.ReadString();
            int count = reader.ReadCount();
            string[] tzdbIds = new string[count];
            for (int i = 0; i < count; i++)
            {
                tzdbIds[i] = reader.ReadString();
            }
            return new MapZone(windowsId, territory, new ReadOnlyCollection<string>(tzdbIds));
        }

        /// <summary>
        /// Writes this mapping to a writer.
        /// </summary>
        /// <param name="writer"></param>
        internal void Write(IDateTimeZoneWriter writer)
        {
            writer.WriteString(windowsId);
            writer.WriteString(territory);
            writer.WriteCount(tzdbIds.Count);
            foreach (string id in tzdbIds)
            {
                writer.WriteString(id);
            }
        }
    }
}
