// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Zone interval map implemented by an array of zone intervals: fetching a zone interval just
    /// amounts to finding the right entry in the array.
    /// </summary>
    internal sealed class BinarySearchZoneIntervalMap : IZoneIntervalMap
    {
        private readonly ZoneInterval[] intervals;

        internal BinarySearchZoneIntervalMap(IEnumerable<ZoneInterval> intervals)
        {
            this.intervals = intervals.ToArray();
            Preconditions.CheckArgument(this.intervals.Length > 0, nameof(intervals), "No intervals specified in binary search zone interval map");
            Preconditions.CheckArgument(!this.intervals[0].HasStart, nameof(intervals), "First intervals in binary search zone interval map must start at the beginning of time");
            for (int i = 0; i < this.intervals.Length - 1; i++)
            {
                // Safe to use End here: there can't be an interval *after* an endless one. Likewise it's safe to use Start on the next 
                // period, as there can't be a period *before* one which goes back to the start of time.
                Preconditions.CheckArgument(this.intervals[i].End == this.intervals[i + 1].Start, nameof(intervals), "Non-adjoining ZoneIntervals for binary search zone interval map");
            }
            Preconditions.CheckArgument(!this.intervals[this.intervals.Length - 1].HasEnd, nameof(intervals), "Final interval in binary search zone interval map must end at the end of time");
        }

        public ZoneInterval GetZoneInterval(Instant instant)
	    {
            int lower = 0; // Inclusive
            int upper = intervals.Length; // Exclusive

            while (lower < upper)
            {
                int current = (lower + upper) / 2;
                var candidate = intervals[current];
                if (candidate.RawStart > instant)
                {
                    upper = current;
                }
                // Safe to use RawEnd, as it's just for the comparison.
                else if (candidate.RawEnd <= instant)
                {
                    lower = current + 1;
                }
                else
                {
                    return candidate;
                }
            }
            // Note: this would indicate a bug. The time zone is meant to cover the whole of time.
            throw new InvalidOperationException($"Instant {instant} did not exist in interval map.");
        }
    }
}
