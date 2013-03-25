// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Equality comparer for time zones, comparing specific aspects of the zone intervals within
    /// a time zone for a specific interval of the time line.
    /// </summary>
    public sealed class ZoneEqualityComparer : IEqualityComparer<DateTimeZone>
    {
        /// <summary>
        /// Options to use when comparing time zones for equality. Each option makes the comparison more restrictive.
        /// </summary>
        [Flags]
        public enum Options
        {
            /// <summary>
            /// The default comparison, which only cares about the wall offset at any particular
            /// instant, within the interval of the comparer. In other words, if <see cref="DateTimeZone.GetUtcOffset"/>
            /// returns the same value for all instants in the interval, the comparer will consider the zones to be equal.
            /// </summary>
            Default = 0,

            /// <summary>
            /// Instead of only comparing wall offsets, the standard/savings split is also considered. So when this
            /// option is used, two zones which both have a wall offset of +2 at one instant would be considered
            /// unequal if one of those offsets was +1 standard, +1 savings and the other was +2 standard with no daylight
            /// saving.
            /// </summary>
            MatchOffsetComponents = 1 << 0,

            /// <summary>
            /// Compare the names of intervals as well as offsets.
            /// </summary>
            MatchNames = 1 << 1,

            /// <summary>
            /// By default, if two or more consecutive zone intervals have the same values when considered
            /// according to the other options, they can be elided. (So if only wall offsets are considered in the
            /// comparison, and two consecutive zone intervals have the same wall offset, they can be treated
            /// as a single zone interval.) When this option is specified, all transitions within the zone are
            /// used, suppressing any possible elision.
            /// </summary>
            MatchAllTransitions = 1 << 2,

            /// <summary>
            /// By default, the transition instants at the start of the first zone interval and the end of the last
            /// zone interval are not considered relevant. For example, a comparison between Europe/London and UTC for just
            /// an interval of "1st January to 1st February 2000" would consider the two equal, as the offsets would
            /// be equal for each instant in the interval. With this option, they would not be considered equal as
            /// UTC's zone interval would have a start/end of the beginning/end of time, whereas the zone interval in
            /// Europe/London would start in the previous autumn and end in the spring.
            /// </summary>
            MatchStartAndEndTransitions = 1 << 3,

            /// <summary>
            /// The combination of all available match options.
            /// </summary>
            PreciseMatch = MatchNames | MatchOffsetComponents | MatchAllTransitions | MatchStartAndEndTransitions
        }

        /// <summary>
        /// Checks whether the given set of options includes the candidate one. This would be an extension method, but
        /// that causes problems on Mono at the moment.
        /// </summary>
        private static bool CheckOption(Options options, Options candidate)
        {
            return (options & candidate) != 0;
        }

        private readonly Interval interval;
        private readonly Options options;
        private readonly ZoneIntervalEqualityComparer zoneIntervalComparer;
        
        /// <summary>
        /// Creates a new comparer for the given start/end interval, with the default comparison options.
        /// </summary>
        /// <param name="start">The start instant (inclusive) to use for comparisons.</param>
        /// <param name="end">The end instant (exclusive) to use for comparisons.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="end"/> is earlier than <paramref name="start"/>.</exception>
        public ZoneEqualityComparer(Instant start, Instant end) : this(start, end, Options.Default)
        {
        }

        /// <summary>
        /// Creates a new comparer for the given interval, with the default comparison options.
        /// </summary>
        /// <param name="interval">The interval within the time line to use for comparisons.</param>
        public ZoneEqualityComparer(Interval interval) : this(interval, Options.Default)
        {
        }

        /// <summary>
        /// Creates a new comparer for the given start/end interval, with the given comparison options.
        /// </summary>
        /// <param name="start">The start instant (inclusive) to use for comparisons.</param>
        /// <param name="end">The end instant (exclusive) to use for comparisons.</param>
        /// <param name="options">The options to use when comparing time zones.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="end"/> is earlier than <paramref name="start"/>,
        /// or the specified options are invalid.</exception>
        public ZoneEqualityComparer(Instant start, Instant end, Options options) : this(new Interval(start, end), options)
        {
        }

        /// <summary>
        /// Creates a new comparer for the given interval, with the given comparison options.
        /// </summary>
        /// <param name="interval">The interval within the time line to use for comparisons.</param>
        /// <param name="options">The options to use when comparing time zones.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified options are invalid.</exception>
        public ZoneEqualityComparer(Interval interval, Options options)
        {
            this.interval = interval;
            this.options = options;
            if ((options & ~Options.PreciseMatch) != 0)
            {
                throw new ArgumentOutOfRangeException("The value " + options + " is not defined within ZoneEqualityComparer.Options");
            }
            zoneIntervalComparer = new ZoneIntervalEqualityComparer(options, interval);
        }

        public bool Equals(DateTimeZone x, DateTimeZone y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }
            // If we ever need to port this to a platform which doesn't support LINQ,
            // we'll need to reimplement this. Until then, it would seem pointless...
            return GetIntervals(x).SequenceEqual(GetIntervals(y), zoneIntervalComparer);
        }

        public int GetHashCode(DateTimeZone obj)
        {
            unchecked
            {
                int hash = 19;
                foreach (var zoneInterval in GetIntervals(obj))
                {
                    hash = hash * 31 + zoneIntervalComparer.GetHashCode(zoneInterval);
                }
                return hash;                
            }
        }

        private IEnumerable<ZoneInterval> GetIntervals(DateTimeZone zone)
        {
            var unelided = zone.GetZoneIntervals(interval.Start, interval.End);
            return CheckOption(options, Options.MatchAllTransitions) ? unelided : ElideIntervals(unelided);
        }

        private IEnumerable<ZoneInterval> ElideIntervals(IEnumerable<ZoneInterval> zoneIntervals)
        {
            ZoneInterval current = null;
            foreach (var zoneInterval in zoneIntervals)
            {
                if (current == null)
                {
                    current = zoneInterval;
                    continue;
                }
                if (zoneIntervalComparer.EqualExceptStartAndEnd(current, zoneInterval))
                {
                    current = current.WithEnd(zoneInterval.End);
                }
                else
                {
                    yield return current;
                    current = zoneInterval;
                }
            }
            // current will only be null if start == end...
            if (current != null)
            {
                yield return current;
            }
        }

        private sealed class ZoneIntervalEqualityComparer : IEqualityComparer<ZoneInterval>
        {
            private readonly Options options;
            private readonly Interval interval;

            internal ZoneIntervalEqualityComparer(Options options, Interval interval)
            {
                this.options = options;
                this.interval = interval;
            }

            public bool Equals(ZoneInterval x, ZoneInterval y)
            {
                if (!EqualExceptStartAndEnd(x, y))
                {
                    return false;
                }
                return GetEffectiveStart(x) == GetEffectiveStart(y) &&
                    GetEffectiveEnd(x) == GetEffectiveEnd(y);
            }

            public int GetHashCode(ZoneInterval obj)
            {
                int hash = HashCodeHelper.Initialize();
                if (CheckOption(options, Options.MatchOffsetComponents))
                {
                    hash = HashCodeHelper.Hash(hash, obj.StandardOffset);
                    hash = HashCodeHelper.Hash(hash, obj.Savings);
                }
                else
                {
                    hash = HashCodeHelper.Hash(hash, obj.WallOffset);
                }
                if (CheckOption(options, Options.MatchNames))
                {
                    hash = HashCodeHelper.Hash(hash, obj.Name);
                }
                hash = HashCodeHelper.Hash(hash, GetEffectiveStart(obj));
                hash = HashCodeHelper.Hash(hash, GetEffectiveEnd(obj));
                return hash;
            }

            private Instant GetEffectiveStart(ZoneInterval zoneInterval)
            {
                return CheckOption(options, Options.MatchStartAndEndTransitions)
                    ? zoneInterval.Start : Instant.Max(zoneInterval.Start, interval.Start);                
            }

            private Instant GetEffectiveEnd(ZoneInterval zoneInterval)
            {
                return CheckOption(options, Options.MatchStartAndEndTransitions)
                    ? zoneInterval.End : Instant.Min(zoneInterval.End, interval.End);
            }

            /// <summary>
            /// Compares the parts of two zone intervals which are deemed "interesting" by the options.
            /// The wall offset is always compared, regardless of options, but the start/end points are
            /// never compared.
            /// </summary>
            internal bool EqualExceptStartAndEnd(ZoneInterval x, ZoneInterval y)
            {
                if (x.WallOffset != y.WallOffset)
                {
                    return false;
                }
                // As we've already compared wall offsets, we only need to compare savings...
                // If the savings are equal, the standard offset will be too.
                if (CheckOption(options, Options.MatchOffsetComponents) && x.Savings != y.Savings)
                {
                    return false;
                }
                if (CheckOption(options, Options.MatchNames) && x.Name != y.Name)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
