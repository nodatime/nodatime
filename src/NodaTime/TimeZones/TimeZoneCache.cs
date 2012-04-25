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
    /// <threadsafety>All members of this type are thread-safe.</threadsafety>
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
            var idList = new List<string>(provider.Ids);
            idList.Sort();
            // Populate the dictionary with null values meaning "the ID is valid, we haven't fetched the zone yet".
            ids = idList.AsReadOnly();
            foreach (string id in ids)
            {
                timeZoneMap[id] = null;
            }
        }

        internal string ProviderVersionId { get { return providerVersionId; } }

        /// <summary>
        /// Gets the system default time zone, as mapped by the underlying provider. If the time zone
        /// is not mapped by this provider, a <see cref="TimeZoneNotFoundException"/> is thrown.
        /// </summary>
        /// <remarks>
        /// Callers should be aware that this method can throw <see cref="TimeZoneNotFoundException"/>,
        /// even with standard Windows time zones.
        /// This could be due to either the Unicode CLDR not being up-to-date with Windows time zone IDs,
        /// or Noda Time not being up-to-date with CLDR - or a provider-specific problem. Callers can use
        /// the null-coalescing operator to effectively provider a default:
        /// </remarks>
        /// <exception cref="TimeZoneNotFoundException">The system default time zone is not mapped by
        /// the current provider.</exception>
        /// <returns>
        /// The provider-specific representation of the system time zone, or null if the time zone
        /// could not be mapped.
        /// </returns>
        public DateTimeZone GetSystemDefault()
        {
            TimeZoneInfo bcl = TimeZoneInfo.Local;
            string id = provider.MapTimeZoneId(bcl);
            if (id == null)
            {
                throw new TimeZoneNotFoundException("TimeZoneInfo ID " + bcl.Id + " is unknown to provider " + providerVersionId);
            }
            return this[id];
        }

        /// <summary>
        /// Gets the system default time zone, as mapped by the underlying provider. If the time zone
        /// is not mapped by this provider, a null reference is returned.
        /// </summary>
        /// <remarks>
        /// Callers should be aware that this method can return null, even with standard Windows time zones.
        /// This could be due to either the Unicode CLDR not being up-to-date with Windows time zone IDs,
        /// or Noda Time not being up-to-date with CLDR - or a provider-specific problem. Callers can use
        /// the null-coalescing operator to effectively provider a default:
        /// </remarks>
        /// <returns>
        /// The provider-specific representation of the system time zone, or null if the time zone
        /// could not be mapped.
        /// </returns>
        public DateTimeZone GetSystemDefaultOrNull()
        {
            TimeZoneInfo bcl = TimeZoneInfo.Local;
            string id = provider.MapTimeZoneId(bcl);
            if (id == null)
            {
                return null;
            }
            return GetZoneOrNull(id);
        }

        /// <summary>
        /// Gets the complete list of valid time zone ids provided by all of the registered
        /// providers. This list will be sorted in lexigraphical order.
        /// </summary>
        /// <value>The <see cref="IEnumerable{T}" /> of string ids.</value>
        internal IEnumerable<string> Ids { get { return ids; } }

        /// <summary>
        /// Returns the time zone with the given id, if it's available.
        /// </summary>
        /// <param name="id">The time zone id to find. Must not be null.</param>
        /// <returns>The <see cref="DateTimeZone" /> with the given id or <c>null</c> if there isn't one defined.</returns>
        internal DateTimeZone GetZoneOrNull(string id)
        {
            Preconditions.CheckNotNull(id, "id");
            lock (accessLock)
            {
                DateTimeZone zone;
                if (!timeZoneMap.TryGetValue(id, out zone))
                {
                    return FixedDateTimeZone.GetFixedZoneOrNull(id);
                }
                if (zone == null)
                {
                    zone = provider.ForId(id);
                    if (zone == null)
                    {
                        throw new InvalidOperationException("Time zone " + id + " is supported by provider " + providerVersionId + " but not returned");
                    }
                    timeZoneMap[id] = zone;
                }
                return zone;
            }
        }

        /// <summary>
        /// Returns the time zone with the given id.
        /// </summary>
        /// <param name="id">The time zone id to find. Must not be null.</param>
        /// <returns>The <see cref="DateTimeZone" /> with the given id or <c>null</c> if there isn't one defined.</returns>
        internal DateTimeZone this[string id]
        {
            get
            {
                var zone = GetZoneOrNull(id);
                if (zone == null)
                {
                    throw new TimeZoneNotFoundException("Time zone " + id + " is unknown to provider " + providerVersionId);
                }
                return zone;
            }
        }
    }
}