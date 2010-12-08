#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
using System.Threading;

namespace NodaTime.TimeZones
{
    /// <summary>
    ///   Provides a cache for time zone providers and previously looked up time zones. The process of
    ///   loading and creating time zones is potentially long (it could conceivably include network
    ///   requests) so caching them is necessary.
    /// </summary>
    internal class TimeZoneCache
    {
        private readonly SortedDictionary<string, string> idList = new SortedDictionary<string, string>();
        private readonly LinkedList<IDateTimeZoneProvider> providers = new LinkedList<IDateTimeZoneProvider>();
        private readonly IDictionary<string, DateTimeZone> timeZoneMap = new Dictionary<string, DateTimeZone>();
        private DateTimeZone current;
        private readonly ReaderWriterLock accessLock = new ReaderWriterLock();

        /// <summary>
        ///   Initializes a new instance of the <see cref = "T:NodaTime.TimeZones.TimeZoneCache" /> class.
        /// </summary>
        /// <param name = "isUtcOnly">if set to <c>true</c> only the UTC provider will be available.</param>
        internal TimeZoneCache(bool isUtcOnly)
        {
            AddProvider(new UtcProvider());
            if (!isUtcOnly)
            {
                AddProvider(DateTimeZone.DefaultDateTimeZoneProvider);
            }
        }

        /// <summary>
        ///   Gets the system default time zone which can only be changed by the system.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     The time zones defined in the operating system are different than the ones defines in
        ///     this library so a mapping will occur. If an exact mapping can be made then that will be
        ///     used otherwise UTC will be used.
        ///   </para>
        /// </remarks>
        /// <value>The system default <see cref = "T:NodaTime.DateTimeZone" /> this will never be <c>null</c>.</value>
        internal DateTimeZone SystemDefault
        {
            get
            {
                string systemName = TimeZone.CurrentTimeZone.StandardName;
                string timeZoneId = WindowsToPosixResource.GetIdFromWindowsName(systemName) ?? DateTimeZone.UtcId;
                return ForId(timeZoneId);
            }
        }

        /// <summary>
        ///   Gets or sets the current time zone.
        /// </summary>
        /// <remarks>
        ///   This is the time zone that is used whenever a time zone is not given to a method. It can
        ///   be set to any valid time zone. Setting it to <c>null</c> causes the
        ///   <see cref = "P:NodaTime.DateTimeZone.SystemDefault" /> time zone to be used.
        /// </remarks>
        /// <value>The current <see cref = "T:NodaTime.DateTimeZone" />. This will never be <c>null</c>.</value>
        internal DateTimeZone Current
        {
            get
            {
                accessLock.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    return current ?? SystemDefault;
                }
                finally
                {
                    accessLock.ReleaseReaderLock();
                }
            }
            set
            {
                accessLock.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    current = value;
                }
                finally
                {
                    accessLock.ReleaseWriterLock();
                }
            }
        }

        /// <summary>
        ///   Gets the complete list of valid time zone ids provided by all of the registered
        ///   providers. This list will be sorted in lexigraphical order by the id name.
        /// </summary>
        /// <value>The <see cref = "IEnumerable{T}" /> of string ids.</value>
        internal IEnumerable<string> Ids
        {
            get
            {
                accessLock.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    if (idList.Count == 0)
                    {
                        LockCookie lockCookie = accessLock.UpgradeToWriterLock(Timeout.Infinite);
                        try
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
                        finally
                        {
                            accessLock.DowngradeFromWriterLock(ref lockCookie);
                        }
                    }
                    return idList.Keys;
                }
                finally
                {
                    accessLock.ReleaseReaderLock();
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
        /// <param name = "provider">The <see cref = "IDateTimeZoneProvider" /> to add.</param>
        internal void AddProvider(IDateTimeZoneProvider provider)
        {
            accessLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (!providers.Contains(provider))
                {
                    LockCookie lockCookie = accessLock.UpgradeToWriterLock(Timeout.Infinite);
                    try
                    {
                        providers.AddFirst(provider);
                        timeZoneMap.Clear();
                        idList.Clear();
                    }
                    finally
                    {
                        accessLock.DowngradeFromWriterLock(ref lockCookie);
                    }
                }
            }
            finally
            {
                accessLock.ReleaseReaderLock();
            }
        }

        /// <summary>
        ///   Removes the given time zone provider from the provider list.
        /// </summary>
        /// <remarks>
        ///   If the provider is not on the list nothing changes.
        /// </remarks>
        /// <param name = "provider">The <see cref = "IDateTimeZoneProvider" /> to remove.</param>
        /// <returns><c>true</c> if the provider was removed.</returns>
        internal bool RemoveProvider(IDateTimeZoneProvider provider)
        {
            accessLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (providers.Contains(provider))
                {
                    LockCookie lockCookie = accessLock.UpgradeToWriterLock(Timeout.Infinite);
                    try
                    {
                        providers.Remove(provider);
                        timeZoneMap.Clear();
                        idList.Clear();
                        return true;
                    }
                    finally
                    {
                        accessLock.DowngradeFromWriterLock(ref lockCookie);
                    }
                }
            }
            finally
            {
                accessLock.ReleaseReaderLock();
            }
            return false;
        }

        /// <summary>
        ///   Returns the time zone with the given id.
        /// </summary>
        /// <param name = "id">The time zone id to find.</param>
        /// <returns>The <see cref = "DateTimeZone" /> with the given id or <c>null</c> if there isn't one defined.</returns>
        internal DateTimeZone ForId(string id)
        {
            DateTimeZone result;
            if (string.IsNullOrEmpty(id))
            {
                result = null;
            }
            else if (id == DateTimeZone.UtcId)
            {
                result = DateTimeZone.Utc;
            }
            else
            {
                accessLock.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    if (!timeZoneMap.TryGetValue(id, out result))
                    {
                        var providerList = new List<IDateTimeZoneProvider>(providers);
                        LockCookie releaseCookie = accessLock.ReleaseLock();
                        try
                        {
                            foreach (var provider in providerList)
                            {
                                result = provider.ForId(id);
                                if (result != null)
                                {
                                    break;
                                }
                            }
                        }
                        finally
                        {
                            accessLock.RestoreLock(ref releaseCookie);
                        }
                        LockCookie lockCookie = accessLock.UpgradeToWriterLock(Timeout.Infinite);
                        // result is null at this point if the id is not found in any provider
                        try
                        {
                            if (timeZoneMap.ContainsKey(id))
                            {
                                result = timeZoneMap[id];
                            }
                            else
                            {
                                timeZoneMap.Add(id, result);
                            }
                        }
                        finally
                        {
                            accessLock.DowngradeFromWriterLock(ref lockCookie);
                        }
                    }
                }
                finally
                {
                    accessLock.ReleaseReaderLock();
                }
            }
            return result;
        }
    }
}