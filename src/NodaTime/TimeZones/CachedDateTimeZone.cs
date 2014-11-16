// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    ///  Provides a <see cref="DateTimeZone"/> wrapper class that implements a simple cache to
    ///  speed up the lookup of transitions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The cache supports mulTiple caching strategies which are implemented in nested subclasses of
    /// this one. Until we have a better sense of what the usage behavior is, we cannot tune the
    /// cache. It is possible that we may support multiple strategies selectable at runtime so the
    /// user can tune the performance based on their knowledge of how they are using the system.
    /// </para>
    /// <para>
    /// In fact, only one cache type is currently implemented: an MRU cache existed before
    /// the GetZoneIntervalPair call was created in DateTimeZone, but as it wasn't being used, it
    /// was more effort than it was worth to update. The mechanism is still available for future
    /// expansion though.
    /// </para>
    /// </remarks>
    internal sealed class CachedDateTimeZone : DateTimeZone
    {
        private readonly IZoneIntervalMap map;

        /// <summary>
        /// Gets the cached time zone.
        /// </summary>
        /// <value>The time zone.</value>
        internal DateTimeZone TimeZone { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedDateTimeZone"/> class.
        /// </summary>
        /// <param name="timeZone">The time zone to cache.</param>
        /// <param name="map">The caching map</param>
        private CachedDateTimeZone(DateTimeZone timeZone, IZoneIntervalMap map) : base(timeZone.Id, false, timeZone.MinOffset, timeZone.MaxOffset)
        {
            this.TimeZone = timeZone;
            this.map = map;
        }

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
