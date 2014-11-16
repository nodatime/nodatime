// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using NodaTime.Annotations;
using NodaTime.TimeZones.IO;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Most time zones have a relatively small set of transitions at their start until they finally 
    /// settle down to either a fixed time zone or a daylight savings time zone. This provides the
    /// container for the initial zone intervals and a pointer to the time zone that handles all of
    /// the rest until the end of time.
    /// </summary>
    internal sealed class PrecalculatedDateTimeZone : DateTimeZone
    {
        private readonly ZoneInterval[] periods;
        private readonly DateTimeZone tailZone;
        /// <summary>
        /// The first instant covered by the tail zone, or Instant.AfterMaxValue if there's no tail zone.
        /// </summary>
        private readonly Instant tailZoneStart;
        private readonly ZoneInterval firstTailZoneInterval;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrecalculatedDateTimeZone"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="periods">The periods.</param>
        /// <param name="tailZone">The tail zone.</param>
        [VisibleForTesting]
        internal PrecalculatedDateTimeZone(string id, ZoneInterval[] periods, DateTimeZone tailZone)
            : base(id, false,
                   ComputeOffset(periods, tailZone, Offset.Min),
                   ComputeOffset(periods, tailZone, Offset.Max))
        {
            this.tailZone = tailZone;
            this.periods = periods;
            this.tailZone = tailZone;
            this.tailZoneStart = periods[periods.Length - 1].RawEnd; // We want this to be AfterMaxValue for tail-less zones.
            if (tailZone != null)
            {
                // Cache a "clamped" zone interval for use at the start of the tail zone.
                firstTailZoneInterval = tailZone.GetZoneInterval(tailZoneStart).WithStart(tailZoneStart);
            }
            ValidatePeriods(periods, tailZone);
        }

        /// <summary>
        /// Validates that all the periods before the tail zone make sense. We have to start at the beginning of time,
        /// and then have adjoining periods. This is only called in the constructors.
        /// </summary>
        /// <remarks>This is only called from the constructors, but is internal to make it easier to test.</remarks>
        /// <exception cref="ArgumentException">The periods specified are invalid.</exception>
        internal static void ValidatePeriods(ZoneInterval[] periods, DateTimeZone tailZone)
        {
            Preconditions.CheckArgument(periods.Length > 0, "periods", "No periods specified in precalculated time zone");
            Preconditions.CheckArgument(!periods[0].HasStart, "periods", "Periods in precalculated time zone must start with the beginning of time");
            for (int i = 0; i < periods.Length - 1; i++)
            {
                // Safe to use End here: there can't be a period *after* an endless one. Likewise it's safe to use Start on the next 
                // period, as there can't be a period *before* one which goes back to the start of time.
                Preconditions.CheckArgument(periods[i].End == periods[i + 1].Start, "periods", "Non-adjoining ZoneIntervals for precalculated time zone");
            }
            Preconditions.CheckArgument(tailZone != null || periods[periods.Length - 1].RawEnd == Instant.AfterMaxValue, "tailZone", "Null tail zone given but periods don't cover all of time");
        }

        /// <summary>
        /// Gets the zone offset period for the given instant.
        /// </summary>
        /// <param name="instant">The Instant to find.</param>
        /// <returns>The ZoneInterval including the given instant.</returns>
        public override ZoneInterval GetZoneInterval(Instant instant)
        {
            if (tailZone != null && instant >= tailZoneStart)
            {
                // Clamp the tail zone interval to start at the end of our final period, if necessary, so that the
                // join is seamless.
                ZoneInterval intervalFromTailZone = tailZone.GetZoneInterval(instant);
                return intervalFromTailZone.RawStart < tailZoneStart ? firstTailZoneInterval : intervalFromTailZone;
            }
            
            int lower = 0; // Inclusive
            int upper = periods.Length; // Exclusive

            while (lower < upper)
            {
                int current = (lower + upper) / 2;
                var candidate = periods[current];
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
            throw new InvalidOperationException(string.Format("Instant {0} did not exist in time zone {1}", instant, Id));
        }

        /// <summary>
        /// Returns true if this time zone is worth caching. Small time zones or time zones with
        /// lots of quick changes do not work well with <see cref="CachedDateTimeZone"/>.
        /// </summary>
        /// <returns><c>true</c> if this instance is cachable; otherwise, <c>false</c>.</returns>
        public bool IsCachable() =>
            // TODO: Work out some decent rules for this. Previously we would only cache if the
            // tail zone was non-null... which was *always* the case due to the use of NullDateTimeZone.
            // We could potentially go back to returning tailZone != null - benchmarking required.
            true;

        #region I/O
        /// <summary>
        /// Writes the time zone to the specified writer.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        internal void Write(IDateTimeZoneWriter writer)
        {
            Preconditions.CheckNotNull(writer, "writer");

            // We used to create a pool of strings just for this zone. This was more efficient
            // for some zones, as it meant that each string would be written out with just a single
            // byte after the pooling. Optimizing the string pool globally instead allows for
            // roughly the same efficiency, and simpler code here.
            writer.WriteCount(periods.Length);
            Instant? previous = null;
            foreach (var period in periods)
            {
                writer.WriteZoneIntervalTransition(previous, (Instant) (previous = period.RawStart));
                writer.WriteString(period.Name);
                writer.WriteOffset(period.WallOffset);
                writer.WriteOffset(period.Savings);
            }
            writer.WriteZoneIntervalTransition(previous, tailZoneStart);
            // We could just check whether we've got to the end of the stream, but this
            // feels slightly safer.
            writer.WriteByte((byte) (tailZone == null ? 0 : 1));
            if (tailZone != null)
            {
                // This is the only kind of zone we support in the new format. Enforce that...
                var tailDstZone = (DaylightSavingsDateTimeZone)tailZone;
                tailDstZone.Write(writer);
            }
        }

        /// <summary>
        /// Reads a time zone from the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="id">The id.</param>
        /// <returns>The time zone.</returns>
        public static DateTimeZone Read(IDateTimeZoneReader reader, string id)
        {
            int size = reader.ReadCount();
            var periods = new ZoneInterval[size];
            // It's not entirely clear why we don't just assume that the first zone interval always starts at Instant.BeforeMinValue
            // (given that we check that later) but we don't... and changing that now could cause compatibility issues.
            var start = reader.ReadZoneIntervalTransition(null);
            for (int i = 0; i < size; i++)
            {
                var name = reader.ReadString();
                var offset = reader.ReadOffset();
                var savings = reader.ReadOffset();
                var nextStart = reader.ReadZoneIntervalTransition(start);
                periods[i] = new ZoneInterval(name, start, nextStart, offset, savings);
                start = nextStart;
            }
            var tailZone = reader.ReadByte() == 1 ? DaylightSavingsDateTimeZone.Read(reader, id + "-tail") : null;
            return new PrecalculatedDateTimeZone(id, periods, tailZone);
        }
        #endregion // I/O

        #region Offset computation for constructors
        // Essentially Func<Offset, Offset, Offset>
        private delegate Offset OffsetAggregator(Offset x, Offset y);
        private delegate Offset OffsetExtractor<in T>(T input);

        // Reasonably simple way of computing the maximum/minimum offset
        // from either periods or transitions, with or without a tail zone.
        private static Offset ComputeOffset(ZoneInterval[] intervals,
            DateTimeZone tailZone,
            OffsetAggregator aggregator)
        {
            Preconditions.CheckNotNull(intervals, "intervals");
            Preconditions.CheckArgument(intervals.Length > 0, "intervals", "No intervals specified");
            Offset ret = intervals[0].WallOffset;
            for (int i = 1; i < intervals.Length; i++)
            {
                ret = aggregator(ret, intervals[i].WallOffset);
            }
            if (tailZone != null)
            {
                // Effectively a shortcut for picking either tailZone.MinOffset or
                // tailZone.MaxOffset
                Offset bestFromZone = aggregator(tailZone.MinOffset, tailZone.MaxOffset);
                ret = aggregator(ret, bestFromZone);
            }
            return ret;
        }
        #endregion

        protected override bool EqualsImpl(DateTimeZone zone)
        {
            PrecalculatedDateTimeZone otherZone = (PrecalculatedDateTimeZone)zone;

            // Check the individual fields first...
            if (Id != otherZone.Id ||
                !Equals(tailZone, otherZone.tailZone) ||
                tailZoneStart != otherZone.tailZoneStart ||
                !Equals(firstTailZoneInterval, otherZone.firstTailZoneInterval))
            {
                return false;
            }

            // Now all the intervals
            if (periods.Length != otherZone.periods.Length)
            {
                return false;
            }
            for (int i = 0; i < periods.Length; i++)
            {
                if (!periods[i].Equals(otherZone.periods[i]))
                {
                    return false;
                }
            }
            return true;                        
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, Id);
            hash = HashCodeHelper.Hash(hash, tailZoneStart);
            hash = HashCodeHelper.Hash(hash, firstTailZoneInterval);
            hash = HashCodeHelper.Hash(hash, tailZone);
            foreach (var period in periods)
            {
                hash = HashCodeHelper.Hash(hash, period);
            }
            return hash;
        }
    }
}
