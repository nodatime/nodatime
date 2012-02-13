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
        /// The first instant covered by the tail zone, or Instant.MaxValue if there's no tail zone.
        /// </summary>
        private readonly Instant tailZoneStart;
        private readonly ZoneInterval firstTailZoneInterval;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrecalculatedDateTimeZone"/> class which uses a tail zone.
        /// </summary>
        /// <param name="id">The id of the time zone.</param>
        /// <param name="transitions">The transitions.</param>
        /// <param name="tailZoneStart">The first instant which isn't covered by the given transitions.</param>
        /// <param name="tailZone">The tail zone, for use beyond precalcedEnd.</param>
        internal PrecalculatedDateTimeZone(string id, IList<ZoneTransition> transitions, Instant tailZoneStart, DateTimeZone tailZone)
            : base(id, false,
                   ComputeOffset(transitions, t => t.WallOffset, tailZone, Offset.Min),
                   ComputeOffset(transitions, t => t.WallOffset, tailZone, Offset.Max))
        {
            if (transitions == null)
            {
                throw new ArgumentNullException("transitions");
            }
            this.tailZone = tailZone;
            this.tailZoneStart = tailZoneStart;
            if (tailZone != null)
            {
                // Cache a "clamped" zone interval for use at the start of the tail zone.
                firstTailZoneInterval = tailZone.GetZoneInterval(tailZoneStart).WithStart(tailZoneStart);
            }
            int size = transitions.Count;
            periods = new ZoneInterval[size];
            for (int i = 0; i < size; i++)
            {
                var transition = transitions[i];
                var endInstant = i == size - 1 ? tailZoneStart : transitions[i + 1].Instant;
                var period = new ZoneInterval(transition.Name, transition.Instant, endInstant, transition.WallOffset, transition.Savings);
                periods[i] = period;
            }
            ValidatePeriods(periods, tailZone);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrecalculatedDateTimeZone"/> class with no tail zone.
        /// </summary>
        /// <param name="id">Time zone ID</param>
        /// <param name="transitions">Transitions, which must span the whole of time</param>
        internal PrecalculatedDateTimeZone(string id, IList<ZoneTransition> transitions)
            : this(id, transitions, Instant.MaxValue, null)

        {
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
                   ComputeOffset(periods, p => p.WallOffset, tailZone, Offset.Min),
                   ComputeOffset(periods, p => p.WallOffset, tailZone, Offset.Max))
        {
            this.tailZone = tailZone;
            this.periods = periods;
            this.tailZone = tailZone;
            this.tailZoneStart = periods[periods.Length - 1].End;
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
                return intervalFromTailZone.Start < tailZoneStart ? firstTailZoneInterval : intervalFromTailZone;
            }

            // TODO(V1-Blocker): Consider using a binary search instead.
            for (var p = periods.Length - 1; p >= 0; p--)
            {
                if (periods[p].Contains(instant))
                {
                    return periods[p];
                }
            }
            throw new InvalidOperationException(string.Format("Instant {0} did not exist in time zone {1}", instant, Id));
        }

        /// <summary>
        /// Returns true if this time zone is worth caching. Small time zones or time zones with
        /// lots of quick changes do not work well with <see cref="CachedDateTimeZone"/>.
        /// </summary>
        /// <returns><c>true</c> if this instance is cachable; otherwise, <c>false</c>.</returns>
        public bool IsCachable()
        {
            // TODO(Post-V1): Work out some decent rules for this. Previously we would only cache if the
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

            // Keep a pool of strings; we don't want to write the same strings out time and time again.
            List<string> stringPool = new List<string>();
            foreach (var period in periods)
            {
                string name = period.Name;
                if (!stringPool.Contains(name))
                {
                    stringPool.Add(name);
                }
            }
            writer.WriteCount(stringPool.Count);
            foreach (string name in stringPool)
            {
                writer.WriteString(name);
            }

            writer.WriteCount(periods.Length);
            foreach (var period in periods)
            {
                writer.WriteInstant(period.Start);
                int nameIndex = stringPool.IndexOf(period.Name);
                if (stringPool.Count < 256)
                {
                    writer.WriteByte((byte)nameIndex);
                }
                else
                {
                    writer.WriteInt32(nameIndex);
                }
                writer.WriteOffset(period.WallOffset);
                writer.WriteOffset(period.Savings);
            }
            writer.WriteInstant(tailZoneStart);
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
            string[] stringPool = new string[reader.ReadCount()];
            for (int i = 0; i < stringPool.Length; i++)
            {
                stringPool[i] = reader.ReadString();
            }

            int size = reader.ReadCount();
            var periods = new ZoneInterval[size];
            var start = reader.ReadInstant();
            for (int i = 0; i < size; i++)
            {
                int nameIndex = stringPool.Length < 256 ? reader.ReadByte() : reader.ReadInt32();
                var name = stringPool[nameIndex];
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