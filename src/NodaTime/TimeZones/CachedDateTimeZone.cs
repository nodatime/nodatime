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
    internal abstract class CachedDateTimeZone : DateTimeZone
    {
        #region CacheType enum
        internal enum CacheType
        {
            Hashtable
        }
        #endregion

        private readonly DateTimeZone timeZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedDateTimeZone"/> class.
        /// </summary>
        /// <param name="timeZone">The time zone to cache.</param>
        private CachedDateTimeZone(DateTimeZone timeZone) : base(timeZone.Id, false, timeZone.MinOffset, timeZone.MaxOffset)
        {
            this.timeZone = timeZone;
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
            return ForZone(timeZone, CacheType.Hashtable);
        }

        /// <summary>
        /// Returns a cached time zone for the given time zone.
        /// </summary>
        /// <remarks>
        /// If the time zone is already cached or it is fixed then it is returned unchanged.
        /// </remarks>
        /// <param name="timeZone">The time zone to cache.</param>
        /// <param name="type">The type of cache to store the zone in.</param>
        /// <returns>The cached time zone.</returns>
        private static DateTimeZone ForZone(DateTimeZone timeZone, CacheType type)
        {
            Preconditions.CheckNotNull(timeZone, "timeZone");
            if (timeZone is CachedDateTimeZone || timeZone.IsFixed)
            {
                return timeZone;
            }
            switch (type)
            {
                case CacheType.Hashtable:
                    return new HashArrayCache(timeZone);
                default:
                    throw new ArgumentException("The type parameter is invalid", "type");
            }
        }

        #region I/O
        /// <summary>
        /// Writes the time zone to the specified writer.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        internal void Write(DateTimeZoneWriter writer)
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
        internal static DateTimeZone Read(DateTimeZoneReader reader, string id)
        {
            Preconditions.CheckNotNull(reader, "reader");
            var timeZone = reader.ReadTimeZone(id);
            return ForZone(timeZone);
        }
        #endregion

        #region Nested type: HashArrayCache
        /// <summary>
        /// This provides a simple cache based on two hash tables (one for local instants, another
        /// for instants).
        /// </summary>
        /// <remarks>
        /// Each hash table entry is either entry or contains a node with enough
        /// information for a particular "period" of about 40 days - so multiple calls for time
        /// zone information within the same few years are likely to hit the cache. Note that
        /// a single "period" may include a daylight saving change (or conceivably more than one);
        /// a node therefore has to contain enough intervals to completely represent that period.
        /// 
        /// If another call is made which maps to the same cache entry number but is for a different
        /// period, the existing hash entry is simply overridden.
        /// </remarks>
        private class HashArrayCache : CachedDateTimeZone
        {
            // Currently we have no need or way to create hash cache zones with
            // different cache sizes. But the cache size should always be a power of 2 to get the
            // "period to cache entry" conversion simply as a bitmask operation.
            private const int CacheSize = 512;
            // Mask to AND the period number with in order to get the cache entry index. The
            // result will always be in the range [0, CacheSize).
            private const int CachePeriodMask = CacheSize - 1;

            /// <summary>
            /// Defines the number of bits to shift an instant value to get the period. This
            /// converts a number of ticks to a number of 40.6 days periods.
            /// </summary>
            private const int PeriodShift = 45;

            private readonly HashCacheNode[] instantCache;

            /// <summary>
            /// Initializes a new instance of the <see cref="CachedDateTimeZone"/> class.
            /// </summary>
            /// <param name="timeZone">The time zone to cache.</param>
            internal HashArrayCache(DateTimeZone timeZone) : base(timeZone)
            {
                Preconditions.CheckNotNull(timeZone, "timeZone");
                instantCache = new HashCacheNode[CacheSize];
            }

            /// <summary>
            /// Gets the zone offset period for the given instant. Null is returned if no period is
            /// defined by the time zone for the given instant.
            /// </summary>
            /// <param name="instant">The Instant to test.</param>
            /// <returns>The defined ZoneOffsetPeriod or <c>null</c>.</returns>
            public override ZoneInterval GetZoneInterval(Instant instant)
            {
                int period = (int)(instant.Ticks >> PeriodShift);
                int index = period & CachePeriodMask;
                var node = instantCache[index];
                if (node == null || node.Period != period)
                {
                    node = HashCacheNode.CreateNode(period, TimeZone);
                    instantCache[index] = node;
                }

                // Note: moving this code into an instance method in HashCacheNode makes a surprisingly
                // large performance difference.
                while (node.Interval.Start > instant)
                {
                    node = node.Previous;
                }
                return node.Interval;
            }

            #region Nested type: HashCacheNode
            // Note: I (Jon) have tried optimizing this as a struct containing two ZoneIntervals
            // and a list of zone intervals (normally null) for the rare case where there are more
            // than two zone intervals in a period. It halved the performance...
            private class HashCacheNode
            {
                private readonly ZoneInterval interval;
                internal ZoneInterval Interval { get { return interval; } }

                private readonly int period;
                internal int Period { get { return period; } }

                private readonly HashCacheNode previous;
                internal HashCacheNode Previous { get { return previous; } }

                /// <summary>
                /// Creates a hash table node with all the information for this period.
                /// We start off by finding the interval for the start of the period, and
                /// then repeatedly check whether that interval ends after the end of the
                /// period - at which point we're done. If not, find the next interval, create
                /// a new node referring to that interval and the previous interval, and keep going.
                /// </summary>
                internal static HashCacheNode CreateNode(int period, DateTimeZone zone)
                {
                    var periodStart = new Instant((long)period << PeriodShift);
                    var periodEnd = new Instant((long)(period + 1) << PeriodShift);

                    var interval = zone.GetZoneInterval(periodStart);
                    var node = new HashCacheNode(interval, period, null);

                    // Keep going while the current interval ends before the period.
                    while (interval.End < periodEnd)
                    {
                        interval = zone.GetZoneInterval(interval.End);
                        node = new HashCacheNode(interval, period, node);
                    }

                    return node;
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="HashCacheNode"/> class.
                /// </summary>
                /// <param name="interval">The zone interval.</param>
                /// <param name="period"></param>
                /// <param name="previous">The previous <see cref="HashCacheNode"/> node.</param>
                private HashCacheNode(ZoneInterval interval, int period, HashCacheNode previous)
                {
                    this.period = period;
                    this.interval = interval;
                    this.previous = previous;
                }

                internal ZoneInterval FindInterval(Instant instant)
                {
                    HashCacheNode node = this;
                    while (node.Interval.Start > instant)
                    {
                        node = node.Previous;
                    }
                    return node.Interval;
                }
            }
            #endregion
        }
        #endregion
    }
}