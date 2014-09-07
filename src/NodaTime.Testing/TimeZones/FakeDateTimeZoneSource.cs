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
    /// <remarks>Under the PCL, the mapping from TimeZoneInfo is performed
    /// using the StandardName property instead of the Id property, as the Id
    /// property isn't available. The standard name is almost always the same
    /// anyway, known exceptions including Jerusalem and the Malay Peninsula.</remarks>
    public sealed class FakeDateTimeZoneSource : IDateTimeZoneSource
    {
        private readonly Dictionary<string, DateTimeZone> zones;
        private readonly Dictionary<string, string> bclToZoneIds;
        private readonly string versionId;

        private FakeDateTimeZoneSource(string versionId,
            Dictionary<string, DateTimeZone> zones,
            Dictionary<string, string> bclToZoneIds)
        {
            this.versionId = versionId;
            this.zones = zones;
            this.bclToZoneIds = bclToZoneIds;
        }

        /// <summary>
        /// Creates a time zone provider (<see cref="DateTimeZoneCache"/>) from this source.
        /// </summary>
        /// <returns>A provider backed by this source.</returns>
        public IDateTimeZoneProvider ToProvider()
        {
            return new DateTimeZoneCache(this);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetIds()
        {
            return zones.Keys;
        }

        /// <inheritdoc />
        public string VersionId { get { return versionId; } }

        /// <inheritdoc />
        public DateTimeZone ForId(string id)
        {
            Preconditions.CheckNotNull(id, "id");
            DateTimeZone zone;
            if (zones.TryGetValue(id, out zone))
            {
                return zone;
            }
            throw new ArgumentException("Unknown ID: " + id);
        }

        /// <inheritdoc />
        public string MapTimeZoneId(TimeZoneInfo timeZone)
        {
            Preconditions.CheckNotNull(timeZone, "timeZone");
#if PCL
            string id = timeZone.StandardName;
#else
            string id = timeZone.Id;
#endif
            string canonicalId;
            // We don't care about the return value of TryGetValue - if it's false,
            // canonicalId will be null, which is what we want.
            bclToZoneIds.TryGetValue(id, out canonicalId);
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
            public IDictionary<string, string> BclIdsToZoneIds { get { return bclIdsToZoneIds; } }

            /// <summary>
            /// Gets the list of zones, exposed as a property for use when a test needs to set properties as
            /// well as adding zones.
            /// </summary>
            /// <value>The list of zones within the provider being built.</value>
            public IList<DateTimeZone> Zones { get { return zones; } }

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
                Preconditions.CheckNotNull(zone, "zone");
                zones.Add(zone);
            }

            /// <summary>
            /// Returns the zones within the builder. This mostly exists
            /// to enable collection initializers.
            /// </summary>
            /// <returns>An iterator over the zones in this builder.</returns>
            public IEnumerator<DateTimeZone> GetEnumerator()
            {
                return zones.GetEnumerator();
            }
            
            /// <summary>
            /// Explicit interface implementation of <see cref="IEnumerator"/>.
            /// </summary>
            /// <returns>An iterator over the zones in this builder.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

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
                }
                var bclIdMapClone = new Dictionary<string, string>(bclIdsToZoneIds);
                return new FakeDateTimeZoneSource(VersionId, zoneMap, bclIdMapClone);
            }
        }
    }
}
