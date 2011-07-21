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
using System.Collections.Generic;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Most time zones have a relatively small set of transitions at their start until they finally 
    /// settle down to either a fixed time zone or a daylight savings time zone. This provides the
    /// container for the initial zone intervals and a pointer to the time zone that handles all of
    /// the rest until the end of time.
    /// </summary>
    internal class PrecalculatedDateTimeZone : DateTimeZone
    {
        private readonly ZoneInterval[] periods;
        private readonly DateTimeZone tailZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrecalculatedDateTimeZone"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="transitions">The transitions.</param>
        /// <param name="precalcedEnd">The precalced end.</param>
        /// <param name="tailZone">The tail zone.</param>
        internal PrecalculatedDateTimeZone(string id, IList<ZoneTransition> transitions, Instant precalcedEnd, DateTimeZone tailZone)
            : base(id, false,
                   ComputeOffset(transitions, t => t.WallOffset, tailZone, Offset.Min),
                   ComputeOffset(transitions, t => t.WallOffset, tailZone, Offset.Max))
        {
            if (transitions == null)
            {
                throw new ArgumentNullException("transitions");
            }
            this.tailZone = tailZone;
            int size = transitions.Count;
            if (size == 0)
            {
                throw new ArgumentException("There must be at least one transition", "transitions");
            }
            periods = new ZoneInterval[size];
            for (int i = 0; i < size; i++)
            {
                var transition = transitions[i];
                var endInstant = i == size - 1 ? precalcedEnd : transitions[i + 1].Instant;
                var period = new ZoneInterval(transition.Name, transition.Instant, endInstant, transition.WallOffset, transition.Savings);
                periods[i] = period;
            }
            ValidatePeriods(periods, tailZone);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrecalculatedDateTimeZone"/> class.
        /// This is only visible to make testing simpler.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="periods">The periods.</param>
        /// <param name="tailZone">The tail zone.</param>
        internal PrecalculatedDateTimeZone(string id, ZoneInterval[] periods, DateTimeZone tailZone)
            : base(id, false,
                   ComputeOffset(periods, p => p.Offset, tailZone, Offset.Min),
                   ComputeOffset(periods, p => p.Offset, tailZone, Offset.Max))
        {
            this.tailZone = tailZone;
            this.periods = periods;
            ValidatePeriods(periods, tailZone);
        }

        /// <summary>
        /// Validates that all the periods before the tail zone make sense. We have to start at the beginning of time,
        /// and then have adjoining periods. This is only called in the 
        /// </summary>
        /// <remarks>This is only called from the constructors, but is internal to make it easier to test.</remarks>
        /// <exception cref="ArgumentException">The periods specified are invalid</exception>
        internal static void ValidatePeriods(ZoneInterval[] periods, DateTimeZone tailZone)
        {
            if (periods.Length == 0)
            {
                throw new ArgumentException("No periods specified in precalculated time zone");
            }
            if (periods[0].Start != Instant.MinValue)
            {
                throw new ArgumentException("Periods in precalculated time zone must start with the beginning of time");
            }
            for (int i = 0; i < periods.Length - 1; i++)
            {
                if (periods[i].End != periods[i + 1].Start)
                {
                    throw new ArgumentException("Non-adjoining ZoneIntervals for precalculated time zone");
                }
            }
            if (tailZone == null && periods[periods.Length - 1].End != Instant.MaxValue)
            {
                throw new ArgumentException("Null tail zone given but periods don't cover all of time");
            }
        }

        /// <summary>
        /// Gets the zone offset period for the given instant. Null is returned if no period is defined by the time zone
        /// for the given instant.
        /// TODO: Is it even possible for a zone to not have a zone interval for a particular instant? It makes no logical
        /// sense. Suggest we state that this can't happen, and throw an exception...
        /// </summary>
        /// <param name="instant">The Instant to test.</param>
        /// <returns>The defined ZoneOffsetPeriod or <c>null</c>.</returns>
        public override ZoneInterval GetZoneInterval(Instant instant)
        {
            int last = periods.Length - 1;
            if (periods[last].End <= instant)
            {
                return tailZone.GetZoneInterval(instant);
            }
            // TODO: Consider using a binary search instead.
            for (var p = last; p >= 0; p--)
            {
                // TODO: It's not clear how this would happen. Do we need it?
                if (periods[p].End <= instant)
                {
                    break;
                }
                if (periods[p].Contains(instant))
                {
                    return periods[p];
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the zone offset period for the given local instant. Null is returned if no period is defined by the time zone
        /// for the given local instant.
        /// </summary>
        /// <param name="localInstant">The LocalInstant to test.</param>
        /// <returns>The defined ZoneOffsetPeriod or <c>null</c>.</returns>
        internal override ZoneInterval GetZoneInterval(LocalInstant localInstant)
        {
            int last = periods.Length - 1;
            if (periods[last].LocalEnd <= localInstant)
            {
                return tailZone.GetZoneInterval(localInstant);
            }
            for (var p = last; p >= 0; p--)
            {
                if (periods[p].LocalEnd <= localInstant)
                {
                    break;
                }
                if (periods[p].Contains(localInstant))
                {
                    return periods[p];
                }
            }
            throw new InvalidOperationException("Local instant not found in precalculated zone");
        }

        internal override ZoneIntervalPair GetZoneIntervals(LocalInstant localInstant)
        {
            int last = periods.Length - 1;
            ZoneInterval lastPeriod = periods[last];

            // If the result definitely isn't in the precalculated results, we still can't
            // just return the pair from the tail zone - it may map to a value which
            // we don't support.
            if (lastPeriod.LocalEnd <= localInstant)
            {
                ZoneIntervalPair tailZonePair = tailZone.GetZoneIntervals(localInstant);
                switch (tailZonePair.MatchingIntervals)
                {
                    case 0:
                        return ZoneIntervalPair.NoMatch;
                    case 1:
                        // Only valid if our local instant occurs after the cutover
                        if (localInstant.Minus(tailZonePair.EarlyInterval.Offset) >= lastPeriod.End)
                        {
                            return tailZonePair;
                        }
                        return ZoneIntervalPair.NoMatch;
                    case 2:
                        // It's ambiguous in the tail zone. If the instant corresponding to the
                        // earlier interval occurs after the cutover, we can return the pair as-is.
                        if (localInstant.Minus(tailZonePair.EarlyInterval.Offset) >= lastPeriod.End)
                        {
                            return tailZonePair;
                        }
                        // If the instant corresponding to the later interval occurs after the
                        // cutover, we can return an unambiguous pair.
                        if (localInstant.Minus(tailZonePair.LateInterval.Offset) >= lastPeriod.End)
                        {
                            return ZoneIntervalPair.Unambiguous(tailZonePair.LateInterval);
                        }
                        // Both instants occur before the cutover (odd!) - so we have no match.
                        return ZoneIntervalPair.NoMatch;
                }
            }
            if (lastPeriod.Contains(localInstant) && tailZone != null)
            {
                // We may be ambiguous with the tail zone. If the tail zone is unambiguous
                // for this instant, we can use the early interval. If it's ambiguous, it's feasible
                // that we actually have *three* viable intervals. I doubt this will ever happen,
                // but if it does we'll use the later of the two intervals from the tail zone.
                ZoneIntervalPair tailZonePair = tailZone.GetZoneIntervals(localInstant);
                ZoneInterval candidate = tailZonePair.LateInterval ?? tailZonePair.EarlyInterval;

                // Still not necessarily a match... we're only interested in an interval
                // such that the local instant in the candidate interval is after our interval's instant end.
                if (candidate != null && localInstant.Minus(candidate.Offset) >= lastPeriod.End)
                {
                    return ZoneIntervalPair.Ambiguous(lastPeriod, candidate);
                }
            }
            // Okay, not ambiguous with the tail zone - let's look through the precalculated intervals
            for (var p = last; p >= 0; p--)
            {
                ZoneInterval candidate = periods[p];
                if (candidate.LocalEnd <= localInstant)
                {
                    return ZoneIntervalPair.NoMatch;
                }

                if (candidate.Contains(localInstant))
                {
                    // Check if it *also* matches the previous one.
                    if (p != 0 && periods[p - 1].Contains(localInstant))
                    {
                        return ZoneIntervalPair.Ambiguous(periods[p - 1], candidate);
                    }
                    return ZoneIntervalPair.Unambiguous(periods[p]);
                }
            }
            throw new InvalidOperationException("Local instant not found in precalculated zone");
        }

        /// <summary>
        /// Returns true if this time zone is worth caching. Small time zones or time zones with
        /// lots of quick changes do not work well with <see cref="CachedDateTimeZone"/>.
        /// </summary>
        /// <returns><c>true</c> if this instance is cachable; otherwise, <c>false</c>.</returns>
        public bool IsCachable()
        {
            // TODO: Work out some decent rules for this. Previously we would only cache if the
            // tail zone was non-null... which was *always* the case due to the use of NullDateTimeZone.
            // We could potentially go back to returning tailZone != null - benchmarking required.
            return true;
        }

        #region I/O
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
            writer.WriteCount(periods.Length);
            foreach (var period in periods)
            {
                writer.WriteInstant(period.Start);
                writer.WriteString(period.Name);
                writer.WriteOffset(period.Offset);
                writer.WriteOffset(period.Savings);
            }
            var end = periods[periods.Length - 1].End;
            if (end != Instant.MaxValue)
            {
                end = end + Duration.One;
            }
            writer.WriteInstant(end);
            writer.WriteTimeZone(tailZone);
        }

        /// <summary>
        /// Reads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static DateTimeZone Read(DateTimeZoneReader reader, string id)
        {
            int size = reader.ReadCount();
            var periods = new ZoneInterval[size];
            var start = reader.ReadInstant();
            for (int i = 0; i < size; i++)
            {
                var name = reader.ReadString();
                var offset = reader.ReadOffset();
                var savings = reader.ReadOffset();
                var nextStart = reader.ReadInstant();
                periods[i] = new ZoneInterval(name, start, nextStart, offset, savings);
                start = nextStart;
            }
            var tailZone = reader.ReadTimeZone(id + "-tail");
            return new PrecalculatedDateTimeZone(id, periods, tailZone);
        }
        #endregion // I/O

        #region Offset computation for constructors
        // Essentially Func<Offset, Offset, Offset>
        private delegate Offset OffsetAggregator(Offset x, Offset y);
        private delegate Offset OffsetExtractor<T>(T input);

        // Reasonably simple way of computing the maximum/minimum offset
        // from either periods or transitions, with or without a tail zone.
        private static Offset ComputeOffset<T>(IEnumerable<T> elements,
            OffsetExtractor<T> extractor,
            DateTimeZone tailZone,
            OffsetAggregator aggregator)
        {
            if (elements == null)
            {
                throw new ArgumentException("elements");
            }
            Offset ret;
            using (var iterator = elements.GetEnumerator())
            {
                if (!iterator.MoveNext())
                {
                    throw new ArgumentException("No transitions / periods specified");
                }
                ret = extractor(iterator.Current);
                while (iterator.MoveNext())
                {
                    ret = aggregator(ret, extractor(iterator.Current));
                }
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
    }
}