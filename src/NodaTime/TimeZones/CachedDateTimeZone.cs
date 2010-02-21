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
using System.Collections.Generic;

namespace NodaTime.TimeZones
{
    /// <summary>
    ///    Provides a
    ///    <see cref="IDateTimeZone"/>
    ///    wrapper class that implements a simple cache to speed
    ///    up the lookup of transitions.
    /// </summary>
    /// <remarks>
    ///    <para>
    ///       This class implements a hash table object optimized for handling the normal access of time zone
    ///       transitions. Using the default settings the time line is divided into 56.9 year sections. As long as
    ///       all time zone requests are within that range there will not be conflicts and the transition
    ///       cache will not be flushed. If access is made outside of the range then the corresponding hash bucket
    ///       will be flushed and the new transition information filled in. Most applications perform there actions within
    ///       a narrow range (one or two years) so the cache should not flush in that case.
    ///    </para>
    ///    <para>
    ///       Within the time range, each bucket represents approximately 40.6 days. Each bucket is a linked list of
    ///       all the transitions that occur within those 40.6 days. We try to only use this cache is the average
    ///       number of transitions is less than or equal to 2 per bucket.
    ///    </para>
    ///    <para>
    ///       TODO: what is the thread safety?
    ///       Answer (Jon): Not sure. On .NET I think we're okay; I'll check with Miguel
    ///       about the Mono memory model. Basically the concern would be whether the
    ///       writes to the array and the values within the Info constructor could be seen out 
    ///       of order; on .NET all writes are in order I believe, and the read would be okay
    ///       as it's a new object (so it can't have any stale values). I may ask Joe though.
    ///       Alternatively, we could lock on cache access or possibly do a
    ///       volatile read.
    ///    </para>
    ///    <para>
    ///       Original name: CachedDateTimeZone
    ///    </para>
    /// </remarks>
    public sealed class CachedDateTimeZone : DateTimeZoneBase
    {
        /// <summary>
        ///    Defines the number of bits to shift an instant value to get the period. This converts
        ///    a number of ticks to a number of 40.6 days periods.
        /// </summary>
        private const int PeriodShift = 45;

        /// <summary>
        ///    The period size is closer to 40.6 but this is close enough for the calculations
        ///    we use.
        /// </summary>
        private const int PeriodSize = 40;

        /// <summary>
        ///    We want to try and keep the number of hash bucket
        /// </summary>
        private const int HalfPeriodSize = PeriodSize / 2;

        private const long TwoYearsInTicks = (366L + 365) * NodaConstants.TicksPerDay;

        private const int DefaultCacheSize = 512;

        private readonly IDateTimeZone timeZone;

        private readonly int cachePeriodMask;

        private readonly Info[] cache;

        /// <summary>
        ///    Initializes a new instance of the
        ///    <see cref="CachedDateTimeZone"/>
        ///    class.
        /// </summary>
        /// <remarks>
        ///    Private constructor to prevent construction. Use
        ///    <see cref="ForZone"/>
        ///    method.
        /// </remarks>
        /// <param name="timeZone">
        ///    The time zone to cache.
        /// </param>
        private CachedDateTimeZone(IDateTimeZone timeZone) : base(timeZone.Id, timeZone.IsFixed)
        {
            this.timeZone = timeZone;
            this.cachePeriodMask = MakeMask(0);
            this.cache = new Info[CachePeriodMask + 1];
        }

        /// <summary>
        ///    Returns a cached time zone for the given time zone.
        /// </summary>
        /// <remarks>
        ///    If the time zone is already cached or it is fixed then it is returned unchanged.
        /// </remarks>
        /// <param name="timeZone">
        ///    The time zone to cache.
        /// </param>
        /// <returns>
        ///    The cached time zone.
        /// </returns>
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
            return new CachedDateTimeZone(timeZone);
        }

        /// <summary>
        ///    Returns true if this time zone is worth caching. Small time zones or time zones with
        ///    lots of quick changes do not work well with
        ///    <see cref="CachedDateTimeZone"/>
        ///    .
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>
        ///    <c>true</c>
        ///    if this instance is worth caching; otherwise,
        ///    <c>false</c>
        ///    .
        /// </returns>
        public static bool IsWorthCaching(IEnumerable<Instant> transitions)
        {
            if (transitions == null)
            {
                throw new ArgumentNullException("transitions");
            }
            // Add up all the distances between transitions that are less than
            // about two years.
            double distances = 0;
            int count = 0;

            Instant previous = Instant.MinValue;
            foreach (var transition in transitions)
            {
                if (previous != Instant.MinValue)
                {
                    Duration difference = transition - previous;
                    if (difference.Ticks < TwoYearsInTicks)
                    {
                        distances += difference.Ticks;
                        count++;
                    }
                }
                previous = transition;
            }

            if (count > 0)
            {
                double average = distances / count;
                average /= NodaConstants.TicksPerDay;
                if (average >= HalfPeriodSize)
                {
                    // Only bother caching if average distance between
                    // transitions is at least HalfPeriodSize days. Why HalfPeriodSize?
                    // CachedDateTimeZone is more efficient if the distance
                    // between transitions is large. With an average of HalfPeriodSize, it
                    // will on average perform about 2 tests per cache
                    // hit.
                    return true;
                }
            }
            return false;
        }

        #region IDateTimeZone Members

        public override Transition? NextTransition(Instant instant)
        {
            var info = GetInfo(instant);
            return info.NextTransition;
        }

        public override Transition? PreviousTransition(Instant instant)
        {
            var info = GetInfo(instant);
            Transition? ret = info.PreviousTransition;
            // If we're at the start of a period and a transition,
            // the caching mechanism will return the wrong value, so
            // ask the time zone itself.
            if (ret != null && ret.Value.Instant == instant)
            {
                return TimeZone.PreviousTransition(instant);
            }
            return ret;
        }

        public override Offset GetOffsetFromUtc(Instant instant)
        {
            var info = GetInfo(instant);
            return info.Offset;
        }

        public override string Name(Instant instant)
        {
            var info = GetInfo(instant);
            return info.Name;
        }

        private int CachePeriodMask
        {
            get { return this.cachePeriodMask; }
        }

        private Info[] Cache
        {
            get { return this.cache; }
        }

        internal IDateTimeZone TimeZone
        {
            get { return this.timeZone; }
        }

        public override void Write(DateTimeZoneWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            writer.WriteTimeZone(TimeZone);
        }

        #endregion

        public static IDateTimeZone Read(DateTimeZoneReader reader, string id)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            IDateTimeZone timeZone = reader.ReadTimeZone(id);
            return new CachedDateTimeZone(timeZone);
        }

        // Although accessed by multiple threads, this method doesn't need to be
        // synchronized.

        /// <summary>
        /// Gets the info.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <returns></returns>
        private Info GetInfo(Instant instant)
        {
            int period = (int)(instant.Ticks >> PeriodShift);
            int index = period & CachePeriodMask;
            Info info = Cache[index];
            if (info == null || (int)((info.PeriodStart.Ticks >> PeriodShift)) != period)
            {
                info = CreateInfo(instant);
                Cache[index] = info;
            }
            while (info.PeriodStart > instant)
            {
                info = info.Previous;
            }
            return info;
        }

        /// <summary>
        /// Creates the info.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <returns></returns>
        private Info CreateInfo(Instant instant)
        {
            // TODO: Extract constants?
            var periodStart = new Instant(instant.Ticks & (0x7ffffL << PeriodShift));
            var info = new Info(TimeZone, periodStart);
            var periodEnd = new Instant(periodStart.Ticks | 0x1fffffffffffL);
            while (true)
            {
                var next = TimeZone.NextTransition(periodStart);
                if (!next.HasValue || next.Value.Instant > periodEnd)
                {
                    break;
                }
                periodStart = next.Value.Instant;
                info = new Info(TimeZone, periodStart, info);
            }

            return info;
        }

        /// <summary>
        /// Makes the mask.
        /// </summary>
        /// <param name="cacheSize">Size of the cache.</param>
        /// <returns></returns>
        private static int MakeMask(int cacheSize)
        {
            if (cacheSize <= 0)
            {
                cacheSize = DefaultCacheSize;
            }
            else
            {
                cacheSize--;
                int shift = 0;
                while (cacheSize > 0)
                {
                    shift++;
                    cacheSize >>= 1;
                }
                cacheSize = 1 << shift;
            }

            return cacheSize - 1;
        }


        /// <summary>
        /// 
        /// </summary>
        private class Info
        {
            private readonly Transition? nextTransition;
            private readonly Transition? previousTransition;
            private readonly Instant periodStart;
            private readonly Info previous;
            private readonly string name;
            private readonly Offset offset;

            public Transition? PreviousTransition { get { return previousTransition; } }
            public Transition? NextTransition { get { return nextTransition; } }
            public Instant PeriodStart { get { return periodStart; } }
            public Info Previous { get { return previous; } }
            public string Name { get { return name; } }
            public Offset Offset  { get { return offset; } }

            /// <summary>
            /// Initializes a new instance of the <see cref="Info"/> class.
            /// </summary>
            /// <param name="zone">The zone.</param>
            /// <param name="periodStart">The period start.</param>
            public Info(IDateTimeZone zone, Instant periodStart)
                : this(zone, periodStart, null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Info"/> class.
            /// </summary>
            /// <param name="zone">The zone.</param>
            /// <param name="periodStart">The period start.</param>
            /// <param name="previous">The previous.</param>
            public Info(IDateTimeZone zone, Instant periodStart, Info previous)
            {
                this.periodStart = periodStart;
                this.previous = previous;
                this.name = zone.Name(PeriodStart);
                this.offset = zone.GetOffsetFromUtc(PeriodStart);
                this.previousTransition = zone.PreviousTransition(PeriodStart + Duration.One);
                this.nextTransition = zone.NextTransition(PeriodStart);
            }
        }
    }
}