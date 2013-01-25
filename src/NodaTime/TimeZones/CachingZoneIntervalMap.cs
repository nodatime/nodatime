// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Helper methods for creating IZoneIntervalMaps which cache results.
    /// </summary>
    internal static class CachingZoneIntervalMap
    {
        /// <summary>
        /// The type of cache to build.
        /// </summary>
        internal enum CacheType
        {
            Hashtable
        }

        /// <summary>
        /// Returns a caching map for the given input map.
        /// </summary>
        internal static IZoneIntervalMap CacheMap(IZoneIntervalMap map, CacheType type)
        {
            switch (type)
            {
                case CacheType.Hashtable:
                    return new HashArrayCache(map);
                default:
                    throw new ArgumentException("The type parameter is invalid", "type");
            }
        }

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
        internal sealed class HashArrayCache : IZoneIntervalMap
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
            private readonly IZoneIntervalMap map;

            internal HashArrayCache(IZoneIntervalMap map)
            {
                this.map = Preconditions.CheckNotNull(map, "map");
                instantCache = new HashCacheNode[CacheSize];
            }

            /// <summary>
            /// Gets the zone offset period for the given instant. Null is returned if no period is
            /// defined by the time zone for the given instant.
            /// </summary>
            /// <param name="instant">The Instant to test.</param>
            /// <returns>The defined ZoneOffsetPeriod or null.</returns>
            public ZoneInterval GetZoneInterval(Instant instant)
            {
                int period = (int)(instant.Ticks >> PeriodShift);
                int index = period & CachePeriodMask;
                var node = instantCache[index];
                if (node == null || node.Period != period)
                {
                    node = HashCacheNode.CreateNode(period, map);
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
            private sealed class HashCacheNode
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
                internal static HashCacheNode CreateNode(int period, IZoneIntervalMap map)
                {
                    var periodStart = new Instant((long)period << PeriodShift);
                    var periodEnd = new Instant((long)(period + 1) << PeriodShift);

                    var interval = map.GetZoneInterval(periodStart);
                    var node = new HashCacheNode(interval, period, null);

                    // Keep going while the current interval ends before the period.
                    while (interval.End < periodEnd)
                    {
                        interval = map.GetZoneInterval(interval.End);
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
            }
            #endregion
        }
        #endregion

    }
}
