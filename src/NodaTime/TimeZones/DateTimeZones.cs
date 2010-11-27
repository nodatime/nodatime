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

namespace NodaTime.TimeZones
{
    /// <summary>
    ///   Static access to time zones by ID, UTC etc. TODO: Move these into DateTimeZone gradually.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The UTC time zone is defined to be present in all time zone databases and is built into the
    ///     Noda Time system so it is always available. It is the only one guarenteed to be present in
    ///     all systems.
    ///   </para>
    ///   <para>
    ///     All other time zones are acquired through providers. These are abstractions around the
    ///     method of reading in or building the time zone definitions from the id. There is a list of
    ///     providers and each is asked in turn for the matching time zone. If it does not have one the
    ///     next is asked until one does return a time zone or the list is exhusted. Providers can use
    ///     any mthod they want to resolve the time zone from the id.
    ///   </para>
    ///   <para>
    ///     The majority of time zones will be provided by the <see cref = "DateTimeZoneResourceProvider" />
    ///     implementation which reads the time zones from an internal set of precompiled resources built
    ///     using the NodaTime.ZoneInfoCompilerW tool.
    ///   </para>
    /// </remarks>
    public static class DateTimeZones
    {
        private static DateTimeZonesBase implementaion;

        /// <summary>
        ///   Gets the time zone support implementation object being used by the system.
        /// </summary>
        private static DateTimeZonesBase Implementation
        {
            get { return implementaion ?? (implementaion = new Impl()); }
        }

        /// <summary>
        ///   Sets the time zone system to only support the UTC time zone.
        /// </summary>
        /// <remarks>
        ///   This is only useful when building the Noda Time package when time zones are not available.
        ///   Once this mode has been set it cannot be reset and will prevent all time zone access.
        /// </remarks>
        /// <exception cref = "InvalidOperationException">when this method is called after the NodaTime library has been initialized.</exception>
        public static void SetUtcOnly()
        {
            if (implementaion == null)
            {
                implementaion = new UtcOnlyImpl();
            }
            else if (!(implementaion is UtcOnlyImpl))
            {
                throw new InvalidOperationException("The UTC only state must be set before any calls to the NodaTime library.");
            }
        }

        /// <summary>
        ///   Gets or sets the current time zone.
        /// </summary>
        /// <remarks>
        ///   This is the time zone that is used whenever a time zone is not given to a method. It can
        ///   be set to any valid time zone. Setting it to <c>null</c> causes the <see cref = "SystemDefault" /> time zone to be used.
        ///   cref="DateTimeZone.SystemDefault"/> time zone to be used.
        /// </remarks>
        /// <value>The current <see cref = "DateTimeZone" />. This will never be <c>null</c>.</value>
        public static DateTimeZone Current
        {
            get { return Implementation.DoCurrent; }
            set { Implementation.DoCurrent = value; }
        }

        /// <summary>
        ///   Gets the complete list of valid time zone ids provided by all of the registered
        ///   providers. This list will be sorted in lexigraphical order by the id name.
        /// </summary>
        /// <value>The <see cref = "IEnumerable{T}" /> of string ids.</value>
        public static IEnumerable<string> Ids
        {
            get { return Implementation.DoIds; }
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
        public static void AddProvider(IDateTimeZoneProvider provider)
        {
            Implementation.DoAddProvider(provider);
        }

        /// <summary>
        ///   Removes the given time zone provider from the provider list.
        /// </summary>
        /// <remarks>
        ///   If the provider is not on the list nothing changes.
        /// </remarks>
        /// <param name = "provider">The <see cref = "IDateTimeZoneProvider" /> to remove.</param>
        /// <returns><c>true</c> if the provider was removed.</returns>
        public static bool RemoveProvider(IDateTimeZoneProvider provider)
        {
            return Implementation.DoRemoveProvider(provider);
        }

        /// <summary>
        ///   Returns the time zone with the given id.
        /// </summary>
        /// <param name = "id">The time zone id to find.</param>
        /// <returns>The <see cref = "DateTimeZone" /> with the given id or <c>null</c> if there isn't one defined.</returns>
        public static DateTimeZone ForId(string id)
        {
            return Implementation.DoForId(id);
        }

        #region interface IDateTimeZones
        private abstract class DateTimeZonesBase
        {
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
            /// <value>The system default <see cref = "DateTimeZone" /> this will never be <c>null</c>.</value>
            internal abstract DateTimeZone DoSystemDefault { get; }

            /// <summary>
            ///   Gets or sets the current time zone.
            /// </summary>
            /// <remarks>
            ///   This is the time zone that is used whenever a time zone is not given to a method. It can
            ///   be set to any valid time zone. Setting it to <c>null</c> causes the <see cref = "DateTimeZone.SystemDefault" /> time zone to be used.
            /// </remarks>
            /// <value>The current <see cref = "DateTimeZone" />. This will never be <c>null</c>.</value>
            internal abstract DateTimeZone DoCurrent { get; set; }

            /// <summary>
            ///   Gets the complete list of valid time zone ids provided by all of the registered
            ///   providers. This list will be sorted in lexigraphical order by the id name.
            /// </summary>
            /// <value>The <see cref = "IEnumerable{T}" /> of string ids.</value>
            internal abstract IEnumerable<string> DoIds { get; }

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
            internal abstract void DoAddProvider(IDateTimeZoneProvider provider);

            /// <summary>
            ///   Removes the given time zone provider from the provider list.
            /// </summary>
            /// <remarks>
            ///   If the provider is not on the list nothing changes.
            /// </remarks>
            /// <param name = "provider">The <see cref = "IDateTimeZoneProvider" /> to remove.</param>
            /// <returns><c>true</c> if the provider was removed.</returns>
            internal abstract bool DoRemoveProvider(IDateTimeZoneProvider provider);

            /// <summary>
            ///   Returns the time zone with the given id.
            /// </summary>
            /// <param name = "id">The time zone id to find.</param>
            /// <returns>The <see cref = "DateTimeZone" /> with the given id or <c>null</c> if there isn't one defined.</returns>
            internal abstract DateTimeZone DoForId(string id);
        }
        #endregion // interface IDateTimeZones

        #region Nested type: UtcOnlyImpl
        private class UtcOnlyImpl : DateTimeZonesBase
        {
            private readonly string[] ids = { DateTimeZone.UtcId };

            internal override DateTimeZone DoSystemDefault
            {
                get { return DateTimeZone.Utc; }
            }

            internal override DateTimeZone DoCurrent
            {
                get { return DateTimeZone.Utc; }
                set { throw new NotSupportedException(); }
            }

            internal override IEnumerable<string> DoIds
            {
                get { return ids; }
            }

            internal override void DoAddProvider(IDateTimeZoneProvider provider)
            {
            }

            internal override bool DoRemoveProvider(IDateTimeZoneProvider provider)
            {
                return true;
            }

            internal override DateTimeZone DoForId(string id)
            {
                return DateTimeZone.UtcId == id ? DateTimeZone.Utc : null;
            }
        }
        #endregion // Nested type: UtcOnlyImpl

        #region Nested type: Impl
        /// <summary>
        ///   Provides a standard object that implements the functionality of the static class <see cref = "DateTimeZones" />.
        ///   This makes testing simpler because all of the logic is
        ///   here--only this class needs to be tested and it can use the standard testing methods.
        ///   TODO: Move this into its own class, and make it thread-safe.
        /// </summary>
        private class Impl : DateTimeZonesBase
        {
            private readonly SortedDictionary<string, string> idList = new SortedDictionary<string, string>();
            private readonly LinkedList<IDateTimeZoneProvider> providers = new LinkedList<IDateTimeZoneProvider>();
            private readonly IDictionary<string, DateTimeZone> timeZoneMap = new Dictionary<string, DateTimeZone>();
            private DateTimeZone current;

            /// <summary>
            ///   Initializes a new instance of the <see cref = "Impl" /> class.
            /// </summary>
            public Impl()
            {
                AddStandardProvider();
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
            /// <value>The system default <see cref = "DateTimeZone" /> this will never be <c>null</c>.</value>
            internal override DateTimeZone DoSystemDefault
            {
                get
                {
                    string systemName = TimeZone.CurrentTimeZone.StandardName;
                    string timeZoneId = WindowsToPosixResource.GetIdFromWindowsName(systemName) ?? DateTimeZone.UtcId;
                    return ForId(timeZoneId) ?? DateTimeZone.Utc;
                }
            }

            /// <summary>
            ///   Gets or sets the current time zone.
            /// </summary>
            /// <remarks>
            ///   This is the time zone that is used whenever a time zone is not given to a method. It can
            ///   be set to any valid time zone. Setting it to <c>null</c> causes the <see cref = "DateTimeZone.SystemDefault" /> time zone to be used.
            ///   cref="DateTimeZone.SystemDefault"/> time zone to be used.
            /// </remarks>
            /// <value>The current <see cref = "DateTimeZone" />. This will never be <c>null</c>.</value>
            internal override DateTimeZone DoCurrent
            {
                get { return current ?? DateTimeZone.SystemDefault; }
                set { current = value; }
            }

            /// <summary>
            ///   Gets the complete list of valid time zone ids provided by all of the registered
            ///   providers. This list will be sorted in lexigraphical order by the id name.
            /// </summary>
            /// <value>The <see cref = "IEnumerable{T}" /> of string ids.</value>
            internal override IEnumerable<string> DoIds
            {
                get
                {
                    if (idList.Count == 0)
                    {
                        idList.Add(DateTimeZone.UtcId, null);
                        foreach (var provider in providers)
                        {
                            foreach (string id in provider.Ids)
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
            ///   Adds the standard provider to the list of providers.
            /// </summary>
            private void AddStandardProvider()
            {
                DoAddProvider(new DateTimeZoneResourceProvider("NodaTime.TimeZones.Tzdb"));
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
            internal override void DoAddProvider(IDateTimeZoneProvider provider)
            {
                if (!providers.Contains(provider))
                {
                    providers.AddFirst(provider);
                    timeZoneMap.Clear();
                    idList.Clear();
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
            internal override bool DoRemoveProvider(IDateTimeZoneProvider provider)
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
            ///   Returns the time zone with the given id.
            /// </summary>
            /// <param name = "id">The time zone id to find.</param>
            /// <returns>The <see cref = "DateTimeZone" /> with the given id or <c>null</c> if there isn't one defined.</returns>
            internal override DateTimeZone DoForId(string id)
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
