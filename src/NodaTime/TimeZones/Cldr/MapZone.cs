// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.TimeZones.IO;
using NodaTime.Utility;
using System;
using System.Linq;

namespace NodaTime.TimeZones.Cldr
{
    /// <summary>
    /// Represents a single <c>&lt;mapZone&gt;</c> element in the CLDR Windows zone mapping file. 
    /// </summary>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    [Immutable]
    public sealed class MapZone : IEquatable<MapZone?>
    {
        /// <summary>
        /// Identifier used for the primary territory of each Windows time zone. A zone mapping with
        /// this territory will always have a single entry. The value of this constant is "001".
        /// </summary>
        public const string PrimaryTerritory = "001";
        /// <summary>
        /// Identifier used for the "fixed offset" territory. A zone mapping with
        /// this territory will always have a single entry. The value of this constant is "ZZ".
        /// </summary>
        public const string FixedOffsetTerritory = "ZZ";

        /// <summary>
        /// Gets the Windows system time zone identifier for this mapping, such as "Central Standard Time".
        /// </summary>
        /// <remarks>
        /// <para>
        /// Most Windows system time zone identifiers use the name for the "standard" part of the zone as
        /// the overall identifier. Don't be fooled: just because a time zone includes "standard" in its identifier
        /// doesn't mean that it doesn't observe daylight saving time.
        /// </para>
        /// </remarks>
        /// <value>The Windows system time zone identifier for this mapping, such as "Central Standard Time".</value>
        [NotNull] public string WindowsId { get; }

        /// <summary>
        /// Gets the territory code for this mapping.
        /// </summary>
        /// <remarks>
        /// This is typically either "001" to indicate that it's the primary territory for this ID, or
        /// "ZZ" to indicate a fixed-offset ID, or a different two-character capitalized code
        /// which indicates the geographical territory.
        /// </remarks>
        /// <value>The territory code for this mapping.</value>
        [NotNull] public string Territory { get; }

        /// <summary>
        /// Gets a read-only non-empty collection of TZDB zone identifiers for this mapping, such as
        /// "America/Chicago" and "America/Matamoros" (both of which are TZDB zones associated with the "Central Standard Time"
        /// Windows system time zone).
        /// </summary>
        /// <remarks>
        /// For the primary and fixed-offset territory IDs ("001" and "ZZ") this always
        /// contains exactly one time zone ID.
        /// </remarks>
        /// <value>A read-only non-empty collection of TZDB zone identifiers for this mapping.</value>
        [NotNull] public IList<string> TzdbIds { get; }

        /// <summary>
        /// Creates a new mapping entry.
        /// </summary>
        /// <remarks>
        /// This constructor is only public for the sake of testability.
        /// </remarks>
        /// <param name="windowsId">Windows system time zone identifier. Must not be null.</param>
        /// <param name="territory">Territory code. Must not be null.</param>
        /// <param name="tzdbIds">List of territory codes. Must not be null, and must not
        /// contains null values.</param>
        public MapZone([NotNull] string windowsId, [NotNull] string territory, [NotNull] IList<string> tzdbIds)
            : this(Preconditions.CheckNotNull(windowsId, nameof(windowsId)),
                   Preconditions.CheckNotNull(territory, nameof(territory)),
                   new ReadOnlyCollection<string>(new List<string>(Preconditions.CheckNotNull(tzdbIds, nameof(tzdbIds)))))
        {
        }

        /// <summary>
        /// Private constructor to avoid unnecessary list copying (and validation) when deserializing.
        /// </summary>
        private MapZone(string windowsId, string territory, ReadOnlyCollection<string> tzdbIds)
        {
            this.WindowsId = windowsId;
            this.Territory = territory;
            this.TzdbIds = tzdbIds;
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
            writer.WriteString(WindowsId);
            writer.WriteString(Territory);
            writer.WriteCount(TzdbIds.Count);
            foreach (string id in TzdbIds)
            {
                writer.WriteString(id);
            }
        }

        /// <inheritdoc />
        public bool Equals(MapZone? other) =>
            other != null &&
            WindowsId == other.WindowsId &&
            Territory == other.Territory &&
            TzdbIds.SequenceEqual(other.TzdbIds);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hash = HashCodeHelper.Initialize().Hash(WindowsId).Hash(Territory);
            foreach (var id in TzdbIds)
            {
                hash = hash.Hash(id);
            }
            return hash.Value;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as MapZone);

        /// <inheritdoc />
        public override string ToString()
            => $"Windows ID: {WindowsId}; Territory: {Territory}; TzdbIds: {string.Join(" ", TzdbIds)}";
    }
}
