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
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// Provides a caching factory for time zone providers. The process of
    /// loading and creating time zones is potentially long (it could conceivably include network
    /// requests) so caching them is necessary.
    /// </summary>
    /// <threadsafety>All members of this type are thread-safe.</threadsafety>
    public class DateTimeZoneFactory
    {
        private static readonly int TypeInitializationChecking = NodaTime.Utility.TypeInitializationChecker.RecordInitializationStart();

        private static readonly DateTimeZoneFactory tzdbFactory = new DateTimeZoneFactory(new TzdbTimeZoneProvider("NodaTime.TimeZones.Tzdb"));
        private static readonly DateTimeZoneFactory bclFactory = new DateTimeZoneFactory(new BclTimeZoneProvider());

        /// <summary>
        /// Gets the default time zone factory, which is initialized from resources within the NodaTime assembly.
        /// </summary>
        public static DateTimeZoneFactory Default { get { return Tzdb; } }

        /// <summary>
        /// Gets a time zone factory which uses the <see cref="TzdbTimeZoneProvider"/>.
        /// </summary>
        public static DateTimeZoneFactory Tzdb { get { return tzdbFactory; } }

        /// <summary>
        /// Gets a time zone factory which uses the <see cref="BclTimeZoneProvider"/>.
        /// </summary>
        public static DateTimeZoneFactory Bcl { get { return bclFactory; } }

        private readonly object accessLock = new object();
        private readonly IDateTimeZoneProvider provider;
        private readonly ReadOnlyCollection<string> ids;
        private readonly IDictionary<string, DateTimeZone> timeZoneMap = new Dictionary<string, DateTimeZone>();
        private readonly string providerVersionId;

        /// <summary>
        /// Creates a factory backed by the given <see cref="IDateTimeZoneProvider"/>.
        /// </summary>
        /// <remarks>
        /// The provider is immediately asked for its version ID and supported time zone IDs; those properties
        /// are then not requested again for the lifetime of this factory.
        /// </remarks>
        /// <param name="provider">The <see cref="IDateTimeZoneProvider"/>; must not be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> is null</exception>
        /// <exception cref="InvalidDateTimeZoneProviderException"><paramref name="provider"/> violates its contract</exception>
        public DateTimeZoneFactory(IDateTimeZoneProvider provider)
        {
            this.provider = Preconditions.CheckNotNull(provider, "provider");
            this.providerVersionId = provider.VersionId;
            if (providerVersionId == null)
            {
                throw new InvalidDateTimeZoneProviderException("Provider-returned version ID was null");
            }
            var providerIds = provider.Ids;
            if (providerIds == null)
            {
                throw new InvalidDateTimeZoneProviderException("Provider-returned ID sequence was null");
            }
            var idList = new List<string>(providerIds);
            // TODO(Post-V1): Handle duplicates?
            idList.Sort();
            ids = idList.AsReadOnly();
            // Populate the dictionary with null values meaning "the ID is valid, we haven't fetched the zone yet".
            foreach (string id in ids)
            {
                if (id == null)
                {
                    throw new InvalidDateTimeZoneProviderException("Provider-returned ID sequence contained a null reference");
                }
                timeZoneMap[id] = null;
            }
        }

        /// <summary>
        /// The version ID of the provider, cached within the factory.
        /// </summary>
        public string ProviderVersionId { get { return providerVersionId; } }

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
        /// Gets the complete list of valid time zone ids provided by the provider associated
        /// with this factory.
        /// </summary>
        /// <remarks>
        /// This list will be sorted in lexigraphical order. It cannot be modified by callers, and
        /// will never be modified by the factory either: client code can safely treat it as thread-safe
        /// and deeply immutable.
        /// </remarks>
        /// <value>The <see cref="IEnumerable{T}" /> of string ids.</value>
        public ReadOnlyCollection<string> Ids { get { return ids; } }

        /// <summary>
        /// Returns the time zone with the given ID, if it's available.
        /// </summary>
        /// <param name="id">The time zone id to find. Must not be null.</param>
        /// <returns>The <see cref="DateTimeZone" /> with the given ID or <c>null</c> if the provider does not support it.</returns>
        /// <exception cref="InvalidDateTimeZoneProviderException">The time zone provider violates its contract by failing
        /// to support a time zone it previously advertised.</exception>
        public DateTimeZone GetZoneOrNull(string id)
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
                        throw new InvalidDateTimeZoneProviderException("Time zone " + id + " is supported by provider " + providerVersionId + " but not returned");
                    }
                    timeZoneMap[id] = zone;
                }
                return zone;
            }
        }

        /// <summary>
        /// Returns the time zone with the given id.
        /// </summary>
        /// <remarks>Unlike <see cref="GetZoneOrNull"/>, this indexer will never return a null reference. If the ID is not
        /// supported by the provider, it will throw <see cref="TimeZoneNotFoundException"/>.</remarks>
        /// <param name="id">The time zone id to find. Must not be null.</param>
        /// <returns>The <see cref="DateTimeZone" /> with the given ID.</returns>
        /// <exception cref="TimeZoneNotFoundException">The underlying provider does not support the given ID.</exception>
        public DateTimeZone this[string id]
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
