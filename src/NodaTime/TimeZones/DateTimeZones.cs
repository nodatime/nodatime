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

using System.Collections.Generic;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Static access to time zones by ID, UTC etc. TODO: Move these into DateTimeZone gradually.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The UTC time zone is defined to be present in all time zone databases and is built into the
    /// Noda Time system so it is always available. It is the only one guarenteed to be present in
    /// all systems.
    /// </para>
    /// <para>
    /// All other time zones are acquired through providers. These are abstractions around the
    /// method of reading in or building the time zone definitions from the id. There is a list of
    /// providers and each is asked in turn for the matching time zone. If it does not have one the
    /// next is asked until one does return a time zone or the list is exhusted. Providers can use
    /// any mthod they want to resolve the time zone from the id.
    /// </para>
    /// <para>
    /// The majority of time zones will be provided by the <see
    /// cref="DateTimeZoneResourceProvider"/> implementation which reads the time zones from an
    /// internal set of precompiled resources built using the NodaTime.ZoneInfoCompiler tool.
    /// </para>
    /// </remarks>
    public static class DateTimeZones
    {
        private static readonly Impl Implementation = new Impl();

        /// <summary>
        /// Gets or sets the current time zone.
        /// </summary>
        /// <remarks>
        /// This is the time zone that is used whenever a time zone is not given to a method. It can
        /// be set to any valid time zone. Setting it to <c>null</c> causes the <see
        /// cref="DateTimeZone.SystemDefault"/> time zone to be used.
        /// </remarks>
        /// <value>The current <see cref="DateTimeZone"/>. This will never be <c>null</c>.</value>
        public static DateTimeZone Current { get { return Implementation.DoCurrent; } set { Implementation.DoCurrent = value; } }

        /// <summary>
        /// Gets the complete list of valid time zone ids provided by all of the registered
        /// providers. This list will be sorted in lexigraphical order by the id name.
        /// </summary>
        /// <value>The <see cref="IEnumerable{T}"/> of string ids.</value>
        public static IEnumerable<string> Ids { get { return Implementation.DoIds; } }

        /// <summary>
        /// Adds the given time zone provider to the front of the provider list.
        /// </summary>
        /// <remarks>
        /// Because this adds the new provider to the from of the list, it will be checked first for
        /// time zone definitions and therefore can override the default system definitions. This
        /// allows for adding new or replacing existing time zones without updating the system. If
        /// the provider is already on the list nothing changes.
        /// </remarks>
        /// <param name="provider">The <see cref="IDateTimeZoneProvider"/> to add.</param>
        public static void AddProvider(IDateTimeZoneProvider provider)
        {
            Implementation.DoAddProvider(provider);
        }

        /// <summary>
        /// Removes the given time zone provider from the provider list.
        /// </summary>
        /// <remarks>
        /// If the provider is not on the list nothing changes.
        /// </remarks>
        /// <param name="provider">The <see cref="IDateTimeZoneProvider"/> to remove.</param>
        /// <returns><c>true</c> if the provider was removed.</returns>
        public static bool RemoveProvider(IDateTimeZoneProvider provider)
        {
            return Implementation.DoRemoveProvider(provider);
        }

        /// <summary>
        /// Returns the time zone with the given id.
        /// </summary>
        /// <param name="id">The time zone id to find.</param>
        /// <returns>The <see cref="DateTimeZone"/> with the given id or <c>null</c> if there isn't one defined.</returns>
        public static DateTimeZone ForId(string id)
        {
            return Implementation.DoForId(id);
        }

        #region Nested type: Impl
        /// <summary>
        /// Provides a standard object that implements the functionality of the static class <see
        /// cref="DateTimeZones"/>. This makes testing simpler because all of the logic is
        /// here--only this class needs to be tested and it can use the standard testing methods.
        /// TODO: Move this into its own class, and make it thread-safe.
        /// </summary>
        private class Impl
        {
            private readonly SortedDictionary<string, string> idList = new SortedDictionary<string, string>();
            private readonly LinkedList<IDateTimeZoneProvider> providers = new LinkedList<IDateTimeZoneProvider>();
            private readonly IDictionary<string, DateTimeZone> timeZoneMap = new Dictionary<string, DateTimeZone>();
            private DateTimeZone current;

            /// <summary>
            /// Initializes a new instance of the <see cref="Impl"/> class.
            /// </summary>
            public Impl()
            {
                AddStandardProvider();
            }

            /// <summary>
            /// Gets or sets the current time zone.
            /// </summary>
            /// <remarks>
            /// This is the time zone that is used whenever a time zone is not given to a method. It can
            /// be set to any valid time zone. Setting it to <c>null</c> causes the <see
            /// cref="DateTimeZone.SystemDefault"/> time zone to be used.
            /// </remarks>
            /// <value>The current <see cref="DateTimeZone"/>. This will never be <c>null</c>.</value>
            internal DateTimeZone DoCurrent { get { return current ?? DateTimeZone.SystemDefault; } set { current = value; } }

            /// <summary>
            /// Gets the complete list of valid time zone ids provided by all of the registered
            /// providers. This list will be sorted in lexigraphical order by the id name.
            /// </summary>
            /// <value>The <see cref="IEnumerable{T}"/> of string ids.</value>
            internal IEnumerable<string> DoIds
            {
                get
                {
                    if (idList.Count == 0)
                    {
                        idList.Add(DateTimeZone.UtcId, null);
                        foreach (var provider in providers)
                        {
                            foreach (var id in provider.Ids)
                            {
                                if (id != DateTimeZone.UtcId)
                                {
                                    idList.Add(id, null);
                                }
                            }
                        }
                    }
                    return idList.Keys;
                }
            }

            /// <summary>
            /// Adds the standard provider to the list of providers.
            /// </summary>
            private void AddStandardProvider()
            {
                DoAddProvider(new DateTimeZoneResourceProvider("NodaTime.TimeZones.Tzdb"));
            }

            /// <summary>
            /// Adds the given time zone provider to the front of the provider list.
            /// </summary>
            /// <remarks>
            /// Because this adds the new provider to the from of the list, it will be checked first for
            /// time zone definitions and therefore can override the default system definitions. This
            /// allows for adding new or replacing existing time zones without updating the system. If
            /// the provider is already on the list nothing changes.
            /// </remarks>
            /// <param name="provider">The <see cref="IDateTimeZoneProvider"/> to add.</param>
            internal void DoAddProvider(IDateTimeZoneProvider provider)
            {
                if (!providers.Contains(provider))
                {
                    providers.AddFirst(provider);
                    timeZoneMap.Clear();
                    idList.Clear();
                }
            }

            /// <summary>
            /// Removes the given time zone provider from the provider list.
            /// </summary>
            /// <remarks>
            /// If the provider is not on the list nothing changes.
            /// </remarks>
            /// <param name="provider">The <see cref="IDateTimeZoneProvider"/> to remove.</param>
            /// <returns><c>true</c> if the provider was removed.</returns>
            internal bool DoRemoveProvider(IDateTimeZoneProvider provider)
            {
                if (providers.Contains(provider))
                {
                    providers.Remove(provider);
                    timeZoneMap.Clear();
                    idList.Clear();
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Returns the time zone with the given id.
            /// </summary>
            /// <param name="id">The time zone id to find.</param>
            /// <returns>The <see cref="DateTimeZone"/> with the given id or <c>null</c> if there isn't one defined.</returns>
            internal DateTimeZone DoForId(string id)
            {
                DateTimeZone result = DateTimeZone.Utc;
                if (id != DateTimeZone.UtcId)
                {
                    if (!timeZoneMap.TryGetValue(id, out result))
                    {
                        foreach (var provider in providers)
                        {
                            result = provider.ForId(id);
                            if (result != null)
                            {
                                break;
                            }
                        }
                        timeZoneMap.Add(id, result);
                    }
                }
                return result;
            }
        }
        #endregion
    }
}