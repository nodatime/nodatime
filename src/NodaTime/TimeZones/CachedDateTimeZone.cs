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

using NodaTime.TimeZones.IO;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    ///  Provides a <see cref="DateTimeZone"/> wrapper class that implements a simple cache to
    ///  speed up the lookup of transitions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The cache supports multiple caching strategies which are implemented in nested subclasses of
    /// this one. Until we have a better sense of what the usage behavior is, we cannot tune the
    /// cache. It is possible that we may support multiple strategies selectable at runtime so the
    /// user can tune the performance based on their knowledge of how they are using the system.
    /// </para>
    /// <para>
    /// In fact, only one cache type is currently implemented: an MRU cache existed before
    /// the GetZoneIntervals call was created in DateTimeZone, but as it wasn't being used, it
    /// was more effort than it was worth to update. The mechanism is still available for future
    /// expansion though.
    /// </para>
    /// </remarks>
    internal sealed class CachedDateTimeZone : DateTimeZone
    {
        private readonly IZoneIntervalMap map;
        private readonly DateTimeZone timeZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedDateTimeZone"/> class.
        /// </summary>
        /// <param name="timeZone">The time zone to cache.</param>
        /// <param name="map">The caching map</param>
        private CachedDateTimeZone(DateTimeZone timeZone, IZoneIntervalMap map) : base(timeZone.Id, false, timeZone.MinOffset, timeZone.MaxOffset)
        {
            this.timeZone = timeZone;
            this.map = map;
        }

        /// <summary>
        /// Gets the cached time zone.
        /// </summary>
        /// <value>The time zone.</value>
        internal DateTimeZone TimeZone { get { return timeZone; } }

        /// <summary>
        /// Returns a cached time zone for the given time zone.
        /// </summary>
        /// <remarks>
        /// If the time zone is already cached or it is fixed then it is returned unchanged.
        /// </remarks>
        /// <param name="timeZone">The time zone to cache.</param>
        /// <returns>The cached time zone.</returns>
        internal static DateTimeZone ForZone(DateTimeZone timeZone)
        {
            Preconditions.CheckNotNull(timeZone, "timeZone");
            if (timeZone is CachedDateTimeZone || timeZone.IsFixed)
            {
                return timeZone;
            }
            return new CachedDateTimeZone(timeZone, CachingZoneIntervalMap.CacheMap(timeZone, CachingZoneIntervalMap.CacheType.Hashtable));
        }

        /// <summary>
        /// Delegates fetching a zone interval to the caching map.
        /// </summary>
        public override ZoneInterval GetZoneInterval(Instant instant)
        {
            return map.GetZoneInterval(instant);
        }

        #region I/O
        /// <summary>
        /// Writes the time zone to the specified writer.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        internal void Write(IDateTimeZoneWriter writer)
        {
            Preconditions.CheckNotNull(writer, "writer");
            writer.WriteTimeZone(timeZone);
        }

        /// <summary>
        /// Reads the zone from the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        internal static DateTimeZone Read(IDateTimeZoneReader reader, string id)
        {
            Preconditions.CheckNotNull(reader, "reader");
            var timeZone = reader.ReadTimeZone(id);
            return ForZone(timeZone);
        }

        protected override bool EqualsImpl(DateTimeZone zone)
        {
            return TimeZone.Equals(((CachedDateTimeZone) zone).TimeZone);
        }

        public override int GetHashCode()
        {
            return TimeZone.GetHashCode();
        }
        #endregion
    }
}
