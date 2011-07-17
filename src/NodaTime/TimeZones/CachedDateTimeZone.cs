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
    /// </remarks>
    internal abstract class CachedDateTimeZone : DateTimeZone
    {
        #region CacheType enum
        internal enum CacheType
        {
            Mru,
            Hashtable
        }
        #endregion

        private readonly DateTimeZone timeZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedDateTimeZone"/> class.
        /// </summary>
        /// <param name="timeZone">The time zone to cache.</param>
        private CachedDateTimeZone(DateTimeZone timeZone) : base(timeZone.Id, false)
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
        internal static DateTimeZone ForZone(DateTimeZone timeZone, CacheType type)
        {
            if (timeZone == null)
            {
                throw new ArgumentNullException("timeZone");
            }
            if (timeZone is CachedDateTimeZone || timeZone.IsFixed)
            {
                return timeZone;
            }
            switch (type)
            {
                case CacheType.Mru:
                    return new MruListCache(timeZone);
                case CacheType.Hashtable:
                    return new HashArrayCache(timeZone);
                default:
                    throw new ArgumentException("The type parameter is invalid", "type");
            }
        }

        #region Overrides of DateTimeZoneBase
        /// <summary>
        /// Writes the time zone to the specified writer.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        internal override void Write(DateTimeZoneWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            writer.WriteTimeZone(timeZone);
        }

        /// <summary>
        /// Reads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        internal static DateTimeZone Read(DateTimeZoneReader reader, string id)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
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

            // Mask to "or" a shifted period value with in order to get the end point.
            // In other words:
            // We shift a number of ticks *right* by PeriodShift to get the period number.
            // We shift the period number *left* by PeriodShift to get the start of the period.
            // We shift the period number *left* by PeriodShift and OR with PeriodEndMask to get
            // the end of the period (inclusive). An alternative would be to increment the period
            // and just shift left, to get the period end in an *exclusive* form.
            private const long PeriodEndMask = (1 << PeriodShift) - 1;

            private readonly HashCacheNode[] instantCache;
            private readonly HashCacheNode[] localInstantCache;

            /// <summary>
            /// Initializes a new instance of the <see cref="CachedDateTimeZone"/> class.
            /// </summary>
            /// <param name="timeZone">The time zone to cache.</param>
            internal HashArrayCache(DateTimeZone timeZone) : base(timeZone)
            {
                if (timeZone == null)
                {
                    throw new ArgumentNullException("timeZone");
                }
                instantCache = new HashCacheNode[CacheSize];
                localInstantCache = new HashCacheNode[CacheSize];
            }

            #region Overrides of DateTimeZoneBase
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
                    node = CreateInstantNode(period);
                    instantCache[index] = node;
                }
                // TODO: Why doesn't this need to check for node being null,
                // as we do in the LocalInterval version?
                while (node.Interval.Start > instant)
                {
                    node = node.Previous;
                }
                return node.Interval;
            }

            /// <summary>
            /// Gets the zone offset period for the given local instant. Null is returned if no period
            /// is defined by the time zone for the given local instant.
            /// </summary>
            /// <param name="localInstant">The LocalInstant to test.</param>
            /// <returns>The defined ZoneOffsetPeriod or <c>null</c>.</returns>
            internal override ZoneInterval GetZoneInterval(LocalInstant localInstant)
            {
                int period = (int)(localInstant.Ticks >> PeriodShift);
                int index = period & CachePeriodMask;
                var node = localInstantCache[index];
                if (node == null || node.Period != period)
                {
                    node = CreateLocalInstantNode(period);
                    localInstantCache[index] = node;
                }
                while (node != null && node.Interval.LocalStart > localInstant)
                {
                    node = node.Previous;
                }
                if (node == null || node.Interval.LocalEnd <= localInstant)
                {
                    throw new SkippedTimeException(localInstant, TimeZone);
                }
                return node.Interval;
            }
            #endregion

            /// <summary>
            /// Creates a hash table node with all the information for this period.
            /// We start off by finding the interval for the start of the period, and
            /// then repeatedly check whether that interval ends after the end of the
            /// period - at which point we're done. If not, find the next interval, create
            /// a new node referring to that interval and the previous interval, and keep going.
            /// 
            /// TODO: Simplify this significantly. There's no need to have anything more than
            /// a single node with a list or array of intervals. In most cases it'll only be one
            /// or two intervals anyway.
            /// </summary>
            private HashCacheNode CreateInstantNode(int period)
            {
                var periodStart = new Instant((long)period << PeriodShift);
                var interval = TimeZone.GetZoneInterval(periodStart);
                var node = new HashCacheNode(interval, period, null);
                var periodEnd = new Instant(periodStart.Ticks | PeriodEndMask);
                while (true)
                {
                    periodStart = node.Interval.End;
                    if (periodStart > periodEnd)
                    {
                        break;
                    }
                    interval = TimeZone.GetZoneInterval(periodStart);
                    node = new HashCacheNode(interval, period, node);
                }

                return node;
            }

            /// <summary>
            /// See CreateInstantNode - this is the equivalent, but for local instants.
            /// 
            /// TODO: Local instants are actually conceivably trickier due to ambiguity
            /// etc. I have a plan for fixing this, but again making this just a list
            /// of intervals which occur at any point in the period would make life easier.
            /// </summary>
            private HashCacheNode CreateLocalInstantNode(int period)
            {
                var periodStart = new LocalInstant((long)period << PeriodShift);
                var interval = TimeZone.GetZoneInterval(periodStart);
                var node = new HashCacheNode(interval, period, null);
                var periodEnd = new LocalInstant(periodStart.Ticks | PeriodEndMask);
                while (true)
                {
                    periodStart = node.Interval.LocalEnd;
                    if (periodStart > periodEnd)
                    {
                        break;
                    }
                    try
                    {
                        interval = TimeZone.GetZoneInterval(periodStart);
                    }
                    catch (SkippedTimeException)
                    {
                        interval = TimeZone.GetZoneInterval(node.Interval.End);
                    }
                    node = new HashCacheNode(interval, period, node);
                }

                return node;
            }

            #region Nested type: HashCacheNode
            /// <summary>
            /// See CreateInstantNode for an explanation.
            /// </summary>
            private class HashCacheNode
            {
                private readonly ZoneInterval interval;
                private readonly int period;
                private readonly HashCacheNode previous;

                /// <summary>
                /// Initializes a new instance of the <see cref="HashCacheNode"/> class.
                /// </summary>
                /// <param name="interval">The zone interval.</param>
                /// <param name="period"></param>
                /// <param name="previous">The previous <see cref="HashCacheNode"/> node.</param>
                internal HashCacheNode(ZoneInterval interval, int period, HashCacheNode previous)
                {
                    this.period = period;
                    this.interval = interval;
                    this.previous = previous;
                }

                internal int Period { get { return period; } }

                internal ZoneInterval Interval { get { return interval; } }

                internal HashCacheNode Previous { get { return previous; } }
            }
            #endregion
        }
        #endregion

        #region Nested type: MruListCache
        /// <summary>
        /// Implements a most-recently-used ordered cache list.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This class implements a simple linked list of periods in most-recently-used order. If
        /// the list grows beyond the CacheSize, the least-recently-used items are dropped in favor
        /// of the new ones. When a lookup is performed, it simply looks through this list in order.
        /// This means it's efficient when many lookups are performed on for roughly the same time
        /// period, but less efficient if they're scattered across many intervals.
        /// </para>
        /// <para>
        /// Special care is taken so that this cache will be is a valid state in the event that
        /// multiple threads operate on it at the same time. However, we do not guarentee that all
        /// operations will be processed in order nor that all nodes will be updated. It is possible
        /// for nodes to be dropped from the list in certain race conditions but this is acceptable
        /// because those nodes will simply be recreated on the next request. By not locking the
        /// object we increase the performance by a factor of 2.
        /// </para>
        /// </remarks>
        private class MruListCache : CachedDateTimeZone
        {
            private const int CacheSize = 128;
            private MruCacheNode head;

            #region Overrides of DateTimeZoneBase
            /// <summary>
            /// Initializes a new instance of the <see cref="CachedDateTimeZone.MruListCache"/> class.
            /// </summary>
            /// <param name="timeZone">The time zone to cache.</param>
            internal MruListCache(DateTimeZone timeZone) : base(timeZone)
            {
            }

            /// <summary>
            /// Gets the zone offset period for the given instant. Null is returned if no period is defined by the time zone
            /// for the given instant.
            /// </summary>
            /// <param name="instant">The Instant to test.</param>
            /// <returns>The defined ZoneOffsetPeriod or <c>null</c>.</returns>
            public override ZoneInterval GetZoneInterval(Instant instant)
            {
                MruCacheNode previous = null;
                int count = 0;
                for (var node = head; node != null; node = node.Next)
                {
                    if (node.Period.Contains(instant))
                    {
                        return Use(previous, node);
                    }
                    if (++count > CacheSize)
                    {
                        break;
                    }
                    previous = node;
                }
                return AddNode(previous, TimeZone.GetZoneInterval(instant));
            }

            /// <summary>
            /// Gets the zone offset period for the given local instant. Null is returned if no period
            /// is defined by the time zone for the given local instant.
            /// </summary>
            /// <param name="localInstant">The LocalInstant to test.</param>
            /// <returns>The defined ZoneOffsetPeriod or <c>null</c>.</returns>
            internal override ZoneInterval GetZoneInterval(LocalInstant localInstant)
            {
                MruCacheNode previous = null;
                int count = 0;
                for (var node = head; node != null; node = node.Next)
                {
                    if (node.Period.Contains(localInstant))
                    {
                        return Use(previous, node);
                    }
                    if (++count > CacheSize)
                    {
                        break;
                    }
                    previous = node;
                }
                return AddNode(previous, TimeZone.GetZoneInterval(localInstant));
            }

            /// <summary>
            /// Adds the given period as a new node in the list.
            /// </summary>
            /// <remarks>
            /// <para>
            /// The new node is added as the head as it is the most-recently-used node. If the <paramref
            /// name="last"/> parameter is not <c>null</c> then any nodes after <paramref name="last"/>
            /// will be dropped which prevents the list from growing without bounds.
            /// </para>
            /// <para>
            /// We do not lock this object before changing because the worst that can happen is if
            /// multiple threads try and change this list at the same time one or more of the new nodes
            /// will be dropped causing the periods not to be cached so they will be reloaded the next
            /// time they are required. This is a small performance hit but much less than the hit of
            /// locking the object on every access.
            /// </para>
            /// </remarks>
            /// <param name="last">The last node to keep. If null then the list is not truncated.</param>
            /// <param name="period">The period to add in the new node.</param>
            /// <returns>The period added.</returns>
            private ZoneInterval AddNode(MruCacheNode last, ZoneInterval period)
            {
                head = new MruCacheNode(period, head);
                if (last != null)
                {
                    last.Next = null;
                }
                return period;
            }

            /// <summary>
            /// Moves the given node to the head of the list to keep the list is a most-recently-used
            /// order.
            /// </summary>
            /// <remarks>
            /// <para>
            /// The previous parameter points to the node immediately before the node to move so we know
            /// where to unlink it from. If the previous parameter is <c>null</c> then node is already
            /// at the head and does not need to be moved.
            /// </para>
            /// <para>
            /// This method does not lock the object before changing because given this order of
            /// operations the worst that will happen with multiple threads is one of the nodes being
            /// moved to the front of the list may be dropped. If they are they will be reloaded the
            /// next time they are requested. This is a small performance hit but much less than the hit
            /// of locking the object on every access.
            /// </para>
            /// </remarks>
            /// <param name="previous">The node before the new one being added. If <c>null</c> then the new node is the head.</param>
            /// <param name="node">The node to move to the front of the list.</param>
            /// <returns>The period in the node being moved.</returns>
            private ZoneInterval Use(MruCacheNode previous, MruCacheNode node)
            {
                if (previous != null)
                {
                    previous.Next = node.Next;
                    node.Next = head;
                    head = node;
                }
                return node.Period;
            }
            #endregion

            #region Nested type: MruCacheNode
            /// <summary>
            /// </summary>
            private class MruCacheNode
            {
                private readonly ZoneInterval period;

                /// <summary>
                /// Initializes a new instance of the <see cref="MruCacheNode"/> class.
                /// </summary>
                /// <param name="period">The period contained in this node.</param>
                /// <param name="next"></param>
                internal MruCacheNode(ZoneInterval period, MruCacheNode next)
                {
                    this.period = period;
                    Next = next;
                }

                /// <summary>
                /// Gets the period in this node.
                /// </summary>
                /// <value>The ZoneOffsetPeriod.</value>
                internal ZoneInterval Period { get { return period; } }

                /// <summary>
                /// Gets or sets the next.
                /// </summary>
                /// <value>The next.</value>
                internal MruCacheNode Next { get; set; }
            }
            #endregion
        }
        #endregion
    }
}