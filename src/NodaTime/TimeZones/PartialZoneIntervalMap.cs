// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;
using System;
using System.Collections.Generic;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Like ZoneIntervalMap, representing just part of the time line. The intervals returned by this map
    /// are clamped to the portion of the time line being represented, to make it easier to work with.
    /// </summary>
    internal sealed class PartialZoneIntervalMap
    {
        private readonly IZoneIntervalMap map;
        private readonly Instant start;
        private readonly Instant end;

        /// <summary>
        /// Start of the interval during which this map is valid.
        /// </summary>
        internal Instant Start { get { return start; } }

        /// <summary>
        /// End (exclusive) of the interval during which this map is valid.
        /// </summary>
        internal Instant End { get { return end; } }

        internal PartialZoneIntervalMap(Instant start, Instant end, IZoneIntervalMap map)
        {
            this.start = start;
            this.end = end;
            this.map = map;
        }

        /// <summary>
        /// Builds a PartialZoneIntervalMap for a single zone interval with the given name, start, end, wall offset and daylight savings.
        /// </summary>
        internal static PartialZoneIntervalMap ForZoneInterval(string name, Instant start, Instant end, Offset wallOffset, Offset savings)
        {
            return ForZoneInterval(new ZoneInterval(name, start, end, wallOffset, savings));
        }

        /// <summary>
        /// Builds a PartialZoneIntervalMap wrapping the given zone interval, taking its start and end as the start and end of
        /// the portion of the time line handled by the partial map.
        /// </summary>
        internal static PartialZoneIntervalMap ForZoneInterval(ZoneInterval interval)
        {
            return new PartialZoneIntervalMap(interval.Start, interval.End, new SingleZoneIntervalMap(interval));
        }

        internal ZoneInterval GetZoneInterval(Instant instant)
        {
            var interval = map.GetZoneInterval(instant);
            // Clamp the interval for the sake of sanity. Checking this every time isn't very efficient,
            // but we're not expecting this to be called too often, due to caching.
            if (interval.Start < Start)
            {
                interval = interval.WithStart(Start);
            }
            if (interval.End > End)
            {
                interval = interval.WithEnd(End);
            }
            return interval;
        }

        /// <summary>
        /// Returns true if this map only contains a single interval; that is, if the first interval includes the end of the map.
        /// </summary>
        private bool IsSingleInterval { get { return map.GetZoneInterval(Start).End >= End; } }

        /// <summary>
        /// Returns a partial zone interval map equivalent to this one, but with the given start point.
        /// </summary>
        internal PartialZoneIntervalMap WithStart(Instant start)
        {
            return new PartialZoneIntervalMap(start, this.End, this.map);
        }

        /// <summary>
        /// Returns a partial zone interval map equivalent to this one, but with the given end point.
        /// </summary>
        internal PartialZoneIntervalMap WithEnd(Instant end)
        {
            return new PartialZoneIntervalMap(this.Start, end, this.map);
        }

        /// <summary>
        /// Converts a sequence of PartialZoneIntervalMaps covering the whole time line into an IZoneIntervalMap.
        /// The partial maps are expected to be in order, with the start of the first map being Instant.BeforeMinValue,
        /// the end of the last map being Instant.AfterMaxValue, and each adjacent pair of maps abutting (i.e. current.End == next.Start).
        /// Zone intervals belonging to abutting maps but which are equivalent in terms of offset and name
        /// are coalesced in the resulting map.
        /// </summary>
        internal static IZoneIntervalMap ConvertToFullMap(IEnumerable<PartialZoneIntervalMap> maps)
        {
            var coalescedMaps = new List<PartialZoneIntervalMap>();
            PartialZoneIntervalMap current = null;
            foreach (var next in maps)
            {
                if (current == null)
                {
                    current = next;
                    continue;
                }

                var lastIntervalOfCurrent = current.GetZoneInterval(current.End - Duration.Epsilon);
                var firstIntervalOfNext = next.GetZoneInterval(next.Start);

                if (!lastIntervalOfCurrent.EqualIgnoreBounds(firstIntervalOfNext))
                {
                    // There's a genuine transition at the boundary of the partial maps. Add the current one, and move on
                    // to the next.
                    coalescedMaps.Add(current);
                    current = next;
                }
                else
                {
                    // The boundary belongs to a single zone interval crossing the two maps. Some coalescing to do.

                    // If both the current and the next map are single zone interval maps, we can just make the current one
                    // go on until the end of the next one instead.
                    if (current.IsSingleInterval && next.IsSingleInterval)
                    {
                        current = ForZoneInterval(lastIntervalOfCurrent.WithEnd(next.End));
                    }
                    else if (current.IsSingleInterval)
                    {
                        // The next map has at least one transition. Add a single new map for the portion of time from the
                        // start of current to the first transition in next, then continue on with the next map, starting at the first transition.
                        coalescedMaps.Add(ForZoneInterval(lastIntervalOfCurrent.WithEnd(firstIntervalOfNext.End)));
                        current = next.WithStart(firstIntervalOfNext.End);
                    }
                    else if (next.IsSingleInterval)
                    {
                        // The current map as at least one transition. Add a version of that, clamped to end at the final transition,
                        // then continue with a new map which takes in the last portion of the current and the whole of next.
                        coalescedMaps.Add(current.WithEnd(lastIntervalOfCurrent.Start));
                        current = ForZoneInterval(firstIntervalOfNext.WithStart(lastIntervalOfCurrent.Start));
                    }
                    else
                    {
                        // Transitions in both maps. Add the part of current before the last transition, and a single map containing
                        // the coalesced interval across the boundary, then continue with the next map, starting at the first transition.
                        coalescedMaps.Add(current.WithEnd(lastIntervalOfCurrent.Start));
                        coalescedMaps.Add(ForZoneInterval(lastIntervalOfCurrent.WithEnd(firstIntervalOfNext.End)));
                        current = next.WithStart(firstIntervalOfNext.End);
                    }
                }
            }
            // We're left with a map extending to the end of time, which couldn't have been coalesced with its predecessors.
            coalescedMaps.Add(current);
            return new CombinedPartialZoneIntervalMap(coalescedMaps.ToArray());
        }

        /// <summary>
        /// Implementation of IZoneIntervalMap used by ConvertToFullMap
        /// </summary>
        private class CombinedPartialZoneIntervalMap : IZoneIntervalMap
        {
            private readonly PartialZoneIntervalMap[] partialMaps;

            internal CombinedPartialZoneIntervalMap(PartialZoneIntervalMap[] partialMaps)
            {
                this.partialMaps = partialMaps;
            }

            public ZoneInterval GetZoneInterval(Instant instant)
            {
                // We assume the maps are ordered, and start with "beginning of time"
                // which means we only need to find the first partial map which ends after
                // the instant we're interested in. This is just a linear search - a binary search
                // would be feasible, but we're not expecting very many entries.
                foreach (var partialMap in partialMaps)
                {
                    if (instant < partialMap.End)
                    {
                        return partialMap.GetZoneInterval(instant);
                    }
                }
                throw new InvalidOperationException("Instant not contained in any map");
            }

        }
    }
}