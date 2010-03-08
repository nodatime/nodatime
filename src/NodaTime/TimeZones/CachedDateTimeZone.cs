#region Copyright and license information

// Copyright 2001-2010 Stephen Colebourne
// Copyright 2010 Jon Skeet
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
    ///    Provides a <see cref="IDateTimeZone"/> wrapper class that implements a simple cache to
    ///    speed up the lookup of transitions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This cache implements a simple linked list of periods in most-recently-used order. If the
    /// list grows beyond the CacheSize, the least-recently-used items are dropped in favor of the
    /// new ones.
    /// </para>
    /// <para>
    /// Special care is taken so that this cache will be is a valid state in the event that multiple
    /// threads operate on it at the same time. However, we do not guarentee that all operations
    /// will be processed in order nor that all nodes will be updated. It is possible for nodes to
    /// be dropped from the list in certain race conditions but this is acceptable because those
    /// nodes will simply be recreated on the next request. By not lokcing the object we increase
    /// the performance by a factor of 2.
    /// </para>
    /// </remarks>
    public abstract class CachedDateTimeZone
        : DateTimeZoneBase
    {
        private readonly IDateTimeZone timeZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedDateTimeZone"/> class.
        /// </summary>
        /// <param name="timeZone">The time zone to cache.</param>
        private CachedDateTimeZone(IDateTimeZone timeZone)
            : base(timeZone.Id, false)
        {
            this.timeZone = timeZone;
        }

        /// <summary>
        /// Gets the cached time zone.
        /// </summary>
        /// <value>The time zone.</value>
        internal IDateTimeZone TimeZone
        {
            get { return this.timeZone; }
        }

        /// <summary>
        /// Returns a cached time zone for the given time zone.
        /// </summary>
        /// <remarks>
        /// If the time zone is already cached or it is fixed then it is returned unchanged.
        /// </remarks>
        /// <param name="timeZone">The time zone to cache.</param>
        /// <returns>The cached time zone.</returns>
        public static IDateTimeZone ForZone(IDateTimeZone timeZone)
        {
            if (timeZone == null)
            {
                throw new ArgumentNullException("timeZone");
            }
            if (timeZone is CachedDateTimeZone || timeZone.IsFixed)
            {
                return timeZone;
            }
            return new CachedUsingMruList(timeZone);
        }

        #region Overrides of DateTimeZoneBase

        /// <summary>
        /// Writes the time zone to the specified writer.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        public override void Write(DateTimeZoneWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            writer.WriteTimeZone(this.timeZone);
        }

        /// <summary>
        /// Reads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static IDateTimeZone Read(DateTimeZoneReader reader, string id)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            var timeZone = reader.ReadTimeZone(id);
            return ForZone(timeZone);
        }

        #endregion

        #region Nested type: CachedUsingMruList

        internal class CachedUsingMruList
            : CachedDateTimeZone
        {
            internal const int CacheSize = 128;

            private MruCacheNode head;

            #region Overrides of DateTimeZoneBase

            /// <summary>
            /// Initializes a new instance of the <see cref="CachedDateTimeZone.CachedUsingMruList"/> class.
            /// </summary>
            /// <param name="timeZone">The time zone to cache.</param>
            internal CachedUsingMruList(IDateTimeZone timeZone)
                : base(timeZone)
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
                for (var node = this.head; node != null; node = node.Next)
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
            public override ZoneInterval GetZoneInterval(LocalInstant localInstant)
            {
                MruCacheNode previous = null;
                int count = 0;
                for (var node = this.head; node != null; node = node.Next)
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
                this.head = new MruCacheNode(period, this.head);
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
                    node.Next = this.head;
                    this.head = node;
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
                public MruCacheNode(ZoneInterval period, MruCacheNode next)
                {
                    this.period = period;
                    Next = next;
                }

                /// <summary>
                /// Gets the period in this node.
                /// </summary>
                /// <value>The ZoneOffsetPeriod.</value>
                public ZoneInterval Period
                {
                    get { return this.period; }
                }

                /// <summary>
                /// Gets or sets the next.
                /// </summary>
                /// <value>The next.</value>
                public MruCacheNode Next { get; set; }
            }

            #endregion
        }

        #endregion
    }
}