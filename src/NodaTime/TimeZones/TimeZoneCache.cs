#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides a cache for time zone providers and previously looked up time zones. The process of
    /// loading and creating time zones is potentially long (it could conceivably include network
    /// requests) so caching them is necessary.
    /// </summary>
    internal class TimeZoneCache
    {
        private readonly object accessLock = new object();
        private readonly IDateTimeZoneProvider provider;
        private readonly ReadOnlyCollection<string> ids;
        private readonly IDictionary<string, DateTimeZone> timeZoneMap = new Dictionary<string, DateTimeZone>();
        private readonly string providerVersionId;

        internal TimeZoneCache(IDateTimeZoneProvider provider)
        {
            this.provider = Preconditions.CheckNotNull(provider, "provider");
            this.providerVersionId = provider.VersionId;
            var ids = new List<string>(provider.Ids);
            if (!ids.Contains(DateTimeZone.UtcId))
            {
                ids.Add(DateTimeZone.UtcId);
            }
            ids.Sort();
            // Populate the dictionary with null values meaning "the ID is valid, we haven't fetched the zone yet".
            this.ids = ids.AsReadOnly();
            foreach (string id in ids)
            {
                timeZoneMap[id] = null;
            }
            timeZoneMap[DateTimeZone.UtcId] = DateTimeZone.Utc;
        }

        internal string ProviderVersionId { get { return providerVersionId; } }

        /// <summary>
        /// Gets the system default time zone which can only be changed by the system.
        /// </summary>
        /// <remarks>
        /// The time zones defined in the operating system are different than the ones defines in
        /// this library so a mapping will occur. If an exact mapping can be made then that will be
        /// used otherwise UTC will be used.
        /// </remarks>
        /// <value>The system default <see cref="T:NodaTime.DateTimeZone" /> this will never be <c>null</c>.</value>
        internal DateTimeZone SystemDefault
        {
            get
            {
                // TODO: Cache this?
                string systemName = TimeZone.CurrentTimeZone.StandardName;
                string timeZoneId = WindowsToPosixResource.GetIdFromWindowsName(systemName) ?? DateTimeZone.UtcId;
                // Use UTC if we can't find the time zone ID - e.g. if DateTimeZone has been set to use UTC only.
                return this[timeZoneId] ?? DateTimeZone.Utc;
            }
        }

        /// <summary>
        /// Gets the complete list of valid time zone ids provided by all of the registered
        /// providers. This list will be sorted in lexigraphical order.
        /// </summary>
        /// <value>The <see cref="IEnumerable{T}" /> of string ids.</value>
        internal IEnumerable<string> Ids { get { return ids; } }

        /// <summary>
        /// Returns the time zone with the given id.
        /// </summary>
        /// <param name="id">The time zone id to find. Must not be null.</param>
        /// <returns>The <see cref="DateTimeZone" /> with the given id or <c>null</c> if there isn't one defined.</returns>
        internal DateTimeZone this[string id]
        {
            get
            {
                Preconditions.CheckNotNull(id, "id");
                lock (accessLock)
                {
                    DateTimeZone zone;
                    if (!timeZoneMap.TryGetValue(id, out zone))
                    {
                        return null;
                    }
                    if (zone == null)
                    {
                        zone = provider.ForId(id);
                        timeZoneMap[id] = zone;
                    }
                    return zone;
                }
            }
        }
    }
}