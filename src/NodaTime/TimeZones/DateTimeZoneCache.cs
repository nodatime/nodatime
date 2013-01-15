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
    /// Provides a stable cache for time zone sources.
    /// </summary>
    /// <remarks>The process of loading and creating time zones is potentially long (it could conceivably include 
    /// network requests) so caching them may be necessary.</remarks>
    /// <threadsafety>All members of this type are thread-safe.</threadsafety>
    public sealed class DateTimeZoneCache : IDateTimeZoneProvider
    {
        private readonly object accessLock = new object();
        private readonly IDateTimeZoneSource source;
        private readonly ReadOnlyCollection<string> ids;
        private readonly IDictionary<string, DateTimeZone> timeZoneMap = new Dictionary<string, DateTimeZone>();
        private readonly string providerVersionId;

        /// <summary>
        /// Creates a provider backed by the given <see cref="IDateTimeZoneSource"/>.
        /// </summary>
        /// <remarks>
        /// Note that the source will never be consulted for requests for the fixed-offset timezones "UTC" and
        /// "UTC+/-Offset" (a standard implementation will be returned instead). This is true even if these IDs are
        /// advertised by the source.
        /// </remarks>
        /// <param name="source">The <see cref="IDateTimeZoneSource"/> for this provider.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        /// <exception cref="InvalidDateTimeZoneSourceException"><paramref name="source"/> violates its contract</exception>
        public DateTimeZoneCache(IDateTimeZoneSource source)
        {
            this.source = Preconditions.CheckNotNull(source, "source");
            this.providerVersionId = source.VersionId;
            if (providerVersionId == null)
            {
                throw new InvalidDateTimeZoneSourceException("Source-returned version ID was null");
            }
            var providerIds = source.GetIds();
            if (providerIds == null)
            {
                throw new InvalidDateTimeZoneSourceException("Source-returned ID sequence was null");
            }
            var idList = new List<string>(providerIds);
            // TODO(Post-V1): Handle duplicates?
            idList.Sort(StringComparer.Ordinal);
            ids = new ReadOnlyCollection<string>(idList);
            // Populate the dictionary with null values meaning "the ID is valid, we haven't fetched the zone yet".
            foreach (string id in ids)
            {
                if (id == null)
                {
                    throw new InvalidDateTimeZoneSourceException("Source-returned ID sequence contained a null reference");
                }
                timeZoneMap[id] = null;
            }
        }

        /// <inheritdoc />
        public string VersionId { get { return providerVersionId; } }

#if !PCL
        /// <inheritdoc />
        public DateTimeZone GetSystemDefault()
        {
            TimeZoneInfo bcl = TimeZoneInfo.Local;
            string id = source.MapTimeZoneId(bcl);
            if (id == null)
            {
                throw new DateTimeZoneNotFoundException("TimeZoneInfo ID " + bcl.Id + " is unknown to source " + providerVersionId);
            }
            return this[id];
        }
#endif

        /// <inheritdoc />
        public ReadOnlyCollection<string> Ids { get { return ids; } }

        /// <inheritdoc />
        public DateTimeZone GetZoneOrNull(string id)
        {
            Preconditions.CheckNotNull(id, "id");
            DateTimeZone fixedZone = FixedDateTimeZone.GetFixedZoneOrNull(id);
            if (fixedZone != null)
            {
                return fixedZone;
            }
            lock (accessLock)
            {
                DateTimeZone zone;
                if (!timeZoneMap.TryGetValue(id, out zone))
                {
                    return null;
                }
                if (zone == null)
                {
                    zone = source.ForId(id);
                    if (zone == null)
                    {
                        throw new InvalidDateTimeZoneSourceException("Time zone " + id + " is supported by source " + providerVersionId + " but not returned");
                    }
                    timeZoneMap[id] = zone;
                }
                return zone;
            }
        }

        /// <inheritdoc />
        public DateTimeZone this[string id]
        {
            get
            {
                var zone = GetZoneOrNull(id);
                if (zone == null)
                {
                    throw new DateTimeZoneNotFoundException("Time zone " + id + " is unknown to source " + providerVersionId);
                }
                return zone;
            }
        }
    }
}
