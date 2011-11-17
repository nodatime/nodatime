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

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides a cache for time zone providers and previously looked up time zones. The process of
    /// loading and creating time zones is potentially long (it could conceivably include network
    /// requests) so caching them is necessary.
    /// </summary>
    internal class TimeZoneCache
    {
        private readonly SortedDictionary<string, string> idList = new SortedDictionary<string, string>();
        private readonly LinkedList<IDateTimeZoneProvider> providers = new LinkedList<IDateTimeZoneProvider>();
        private readonly IDictionary<string, DateTimeZone> timeZoneMap = new Dictionary<string, DateTimeZone>();
        private readonly object accessLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NodaTime.TimeZones.TimeZoneCache" /> class.
        /// </summary>
        /// <param name="isUtcOnly">if set to <c>true</c> only the UTC provider will be available.</param>
        internal TimeZoneCache(bool isUtcOnly)
        {
            AddProvider(new UtcProvider());
            if (!isUtcOnly)
            {
                AddProvider(DateTimeZone.DefaultDateTimeZoneProvider);
            }
        }

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
                return ForId(timeZoneId) ?? DateTimeZone.Utc;
            }
        }

        /// <summary>
        /// Gets the complete list of valid time zone ids provided by all of the registered
        /// providers. This list will be sorted in lexigraphical order by the id name.
        /// </summary>
        /// <value>The <see cref="IEnumerable{T}" /> of string ids.</value>
        internal IEnumerable<string> Ids
        {
            get
            {
                lock (accessLock)
                {
                    if (idList.Count == 0)
                    {
                        idList.Add(DateTimeZone.UtcId, null);
                        foreach (var provider in providers)
                        {
                            foreach (string id in provider.Ids)
                            {
                                if (!idList.ContainsKey(id))
                                {
                                    idList.Add(id, null);
                                }
                            }
                        }
                    }
                    return idList.Keys;
                }
            }
        }

        /// <summary>
        ///   Adds the given time zone provider to the front of the provider list.
        /// </summary>
        /// <remarks>
        ///   Because this adds the new provider to the from of the list, it will be checked first for
        ///   time zone definitions and therefore can override the default system definitions. This
        ///   allows for adding new or replacing existing time zones without updating the system. If
        ///   the provider is already on the list nothing changes.
        /// </remarks>
        /// <param name="provider">The <see cref="IDateTimeZoneProvider" /> to add.</param>
        internal void AddProvider(IDateTimeZoneProvider provider)
        {
            lock (accessLock)
            {
                if (!providers.Contains(provider))
                {
                    providers.AddFirst(provider);
                    timeZoneMap.Clear();
                    idList.Clear();
                }
            }
        }

        /// <summary>
        ///   Removes the given time zone provider from the provider list.
        /// </summary>
        /// <remarks>
        ///   If the provider is not on the list nothing changes.
        /// </remarks>
        /// <param name="provider">The <see cref="IDateTimeZoneProvider" /> to remove.</param>
        /// <returns><c>true</c> if the provider was removed.</returns>
        internal bool RemoveProvider(IDateTimeZoneProvider provider)
        {
            lock (accessLock)
            {
                if (!providers.Contains(provider))
                {
                    return false;
                }
                providers.Remove(provider);
                timeZoneMap.Clear();
                idList.Clear();
                return true;
            }
        }

        /// <summary>
        ///   Returns the time zone with the given id.
        /// </summary>
        /// <param name="id">The time zone id to find.</param>
        /// <returns>The <see cref="DateTimeZone" /> with the given id or <c>null</c> if there isn't one defined.</returns>
        internal DateTimeZone ForId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            if (id == DateTimeZone.UtcId)
            {
                return DateTimeZone.Utc;
            }
            lock(accessLock)
            {
                DateTimeZone result;
                if (timeZoneMap.TryGetValue(id, out result))
                {
                    return result;
                }
                var providerList = new List<IDateTimeZoneProvider>(providers);
                foreach (var provider in providerList)
                {
                    result = provider.ForId(id);
                    if (result != null)
                    {
                        timeZoneMap.Add(id, result);
                        return result;
                    }
                }
            }
            return null;
        }
    }
}