// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodaTime.TimeZones;

namespace NodaTime.Testing.TimeZones
{
    /// <summary>
    /// A time zone source for test purposes.
    /// Create instances via <see cref="FakeDateTimeZoneSource.Builder"/>.
    /// </summary>
    public sealed class FakeDateTimeZoneSource : IDateTimeZoneSource
    {
        private readonly Dictionary<string, DateTimeZone> zones;
        private readonly Dictionary<string, string> bclToZoneIds;

        private FakeDateTimeZoneSource(string versionId,
            Dictionary<string, DateTimeZone> zones,
            Dictionary<string, string> bclToZoneIds)
        {
            this.VersionId = versionId;
            this.zones = zones;
            this.bclToZoneIds = bclToZoneIds;
        }

        /// <summary>
        /// Creates a time zone provider (<see cref="DateTimeZoneCache"/>) from this source.
        /// </summary>
        /// <returns>A provider backed by this source.</returns>
        public IDateTimeZoneProvider ToProvider() => new DateTimeZoneCache(this);

        /// <inheritdoc />
        public IEnumerable<string> GetIds() => zones.Keys;

        /// <inheritdoc />
        public string VersionId { get; }

        /// <inheritdoc />
        public DateTimeZone ForId(string id)
        {
            Preconditions.CheckNotNull(id, nameof(id));
            if (zones.TryGetValue(id, out DateTimeZone zone))
            {
                return zone;
            }
            throw new ArgumentException("Unknown ID: " + id);
        }

        // TODO: Work out why inheritdoc doesn't work here. What's special about this method?

        /// <summary>
        /// Returns this source's ID for the system default time zone.
        /// </summary>
        /// <returns>
        /// The ID for the system default time zone for this source,
        /// or null if the system default time zone has no mapping in this source.
        /// </returns>
        public string GetSystemDefaultId()
        {
            string id = TimeZoneInfo.Local.Id;
            // We don't care about the return value of TryGetValue - if it's false,
            // canonicalId will be null, which is what we want.
            bclToZoneIds.TryGetValue(id, out string canonicalId);
            return canonicalId;
        }

        /// <summary>
        /// Builder for <see cref="FakeDateTimeZoneSource"/>, allowing the built object to
        /// be immutable, but constructed via object/collection initializers.
        /// </summary>
        public sealed class Builder : IEnumerable<DateTimeZone>
        {
            private readonly Dictionary<string, string> bclIdsToZoneIds = new Dictionary<string, string>();
            private readonly List<DateTimeZone> zones = new List<DateTimeZone>();

            /// <summary>
            /// Gets the dictionary mapping BCL <see cref="TimeZoneInfo"/> IDs to the canonical IDs
            /// served within the provider being built.
            /// </summary>
            /// <value>The dictionary mapping BCL IDs to the canonical IDs served within the provider
            /// being built.</value>
            public IDictionary<string, string> BclIdsToZoneIds => bclIdsToZoneIds;

            /// <summary>
            /// Gets the list of zones, exposed as a property for use when a test needs to set properties as
            /// well as adding zones.
            /// </summary>
            /// <value>The list of zones within the provider being built.</value>
            public IList<DateTimeZone> Zones => zones;

            /// <summary>
            /// Gets the version ID to advertise; defaults to "TestZones".
            /// </summary>
            /// <value>The version ID to advertise; defaults to "TestZones".</value>
            public string VersionId { get; set; }

            /// <summary>
            /// Creates a new builder.
            /// </summary>
            public Builder()
            {
                VersionId = "TestZones";
            }

            /// <summary>
            /// Adds a time zone to the builder.
            /// </summary>
            /// <param name="zone">The zone to add.</param>
            public void Add(DateTimeZone zone)
            {
                Preconditions.CheckNotNull(zone, nameof(zone));
                zones.Add(zone);
            }

            /// <summary>
            /// Returns the zones within the builder. This mostly exists
            /// to enable collection initializers.
            /// </summary>
            /// <returns>An iterator over the zones in this builder.</returns>
            public IEnumerator<DateTimeZone> GetEnumerator() => zones.GetEnumerator();

            /// <summary>
            /// Explicit interface implementation of <see cref="IEnumerator"/>.
            /// </summary>
            /// <returns>An iterator over the zones in this builder.</returns>
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            /// <summary>
            /// Builds a time zone source from this builder. The returned
            /// builder will be independent of this builder; further changes
            /// to this builder will not be reflected in the returned source.
            /// </summary>
            /// <remarks>
            /// This method performs some sanity checks, and throws exceptions if
            /// they're violated. Those exceptions are not documented here, and you
            /// shouldn't be catching them anyway. (This is aimed at testing...)
            /// </remarks>
            /// <returns>The newly-built time zone source.</returns>
            public FakeDateTimeZoneSource Build()
            {
                var zoneMap = zones.ToDictionary(zone => zone.Id);
                foreach (var entry in bclIdsToZoneIds)
                {
                    Preconditions.CheckNotNull(entry.Value, "value");
                    if (!zoneMap.ContainsKey(entry.Value))
                    {
                        throw new InvalidOperationException($"Mapping for BCL {entry.Key}/{entry.Value} has no corresponding zone.");
                    }
                }
                var bclIdMapClone = new Dictionary<string, string>(bclIdsToZoneIds);
                return new FakeDateTimeZoneSource(VersionId, zoneMap, bclIdMapClone);
            }
        }
    }
}
