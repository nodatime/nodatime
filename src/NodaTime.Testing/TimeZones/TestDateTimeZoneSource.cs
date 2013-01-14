#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2013 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodaTime.TimeZones;

namespace NodaTime.Testing.TimeZones
{
    /// <summary>
    /// A time zone source for test purposes.
    /// Create instances via <see cref="TestDateTimeZoneSource.Builder"/>.
    /// </summary>
    public sealed class TestDateTimeZoneSource : IDateTimeZoneSource
    {
        private readonly Dictionary<string, DateTimeZone> zones;
        private readonly Dictionary<string, string> bclToZoneIds;
        private readonly string versionId;

        private TestDateTimeZoneSource(string versionId,
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
#if PCL
            throw new NotSupportedException();
#else
            Preconditions.CheckNotNull(timeZone, "timeZone");
            string canonicalId;
            // We don't care about the return value of TryGetValue - if it's false,
            // canonicalId will be null, which is what we want.
            bclToZoneIds.TryGetValue(timeZone.Id, out canonicalId);
            return canonicalId;
#endif
        }

        /// <summary>
        /// Builder for <see cref="TestDateTimeZoneSource"/>, allowing the built object to
        /// be immutable, but constructed via object/collection initializers.
        /// </summary>
        public sealed class Builder : IEnumerable<DateTimeZone>
        {
            private readonly Dictionary<string, string> bclIdsToZoneIds = new Dictionary<string, string>();
            private readonly List<DateTimeZone> zones = new List<DateTimeZone>();

            /// <summary>
            /// The dictionary mapping BCL <see cref="TimeZoneInfo"/> IDs to the canonical IDs
            /// served within the provider being built.
            /// </summary>
            public IDictionary<string, string> BclIdsToZoneIds { get { return bclIdsToZoneIds; } }

            /// <summary>
            /// List of zones, exposed as a property for use when a test needs to set properties as
            /// well as adding zones.
            /// </summary>
            public IList<DateTimeZone> Zones { get { return zones; } }

            /// <summary>
            /// The version ID to advertise; defaults to "TestZones".
            /// </summary>
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
            /// <paramref name="zone">The zone to add.</paramref>
            public void Add(DateTimeZone zone)
            {
                Preconditions.CheckNotNull(zone, "zone");
                zones.Add(zone);
            }

            /// <summary>
            /// Returns the zones within the builder. This mostly exists
            /// to enable collection initializers.
            /// </summary>
            public IEnumerator<DateTimeZone> GetEnumerator()
            {
                return zones.GetEnumerator();
            }
            
            /// <summary>
            /// Explicit interface implementation of <see cref="IEnumerator"/>.
            /// </summary>
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
            public TestDateTimeZoneSource Build()
            {
                var zoneMap = zones.ToDictionary(zone => zone.Id);
                foreach (var entry in bclIdsToZoneIds)
                {
                    Preconditions.CheckNotNull(entry.Value, "value");
                }
                var bclIdMapClone = new Dictionary<string, string>(bclIdsToZoneIds);
                return new TestDateTimeZoneSource(VersionId, zoneMap, bclIdMapClone);
            }
        }
    }
}
