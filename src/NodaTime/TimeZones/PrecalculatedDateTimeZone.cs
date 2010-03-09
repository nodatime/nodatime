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
    /// Most time zones have a relatively small set of transitions at their start until they finally 
    /// settle down to either a fixed time zone or a daylight savings time zone. This provides the
    /// container for the initial zone intervals and a pointer to the time zone that handles all of
    /// the rest until the end of time.
    /// </summary>
    internal class PrecalculatedDateTimeZone
        : DateTimeZoneBase
    {
        private readonly ZoneInterval[] periods;
        private readonly IDateTimeZone tailZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrecalculatedDateTimeZone"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="transitions">The transitions.</param>
        /// <param name="precalcedEnd">The precalced end.</param>
        /// <param name="tailZone">The tail zone.</param>
        public PrecalculatedDateTimeZone(string id, IList<ZoneTransition> transitions, Instant precalcedEnd,
                                         IDateTimeZone tailZone)
            : base(id, false)
        {
            if (transitions == null)
            {
                throw new ArgumentNullException("transitions");
            }
            this.tailZone = tailZone;
            int size = transitions.Count;
            if (size == 0)
            {
                throw new ArgumentException(@"There must be at least one transition", "transitions");
            }
            this.periods = new ZoneInterval[size];
            for (int i = 0; i < size; i++)
            {
                var transition = transitions[i];
                var endInstant = i == size - 1 ? precalcedEnd : transitions[i + 1].Instant;
                var period = new ZoneInterval(transition.Name, transition.Instant, endInstant, transition.WallOffset,
                                              transition.Savings);
                this.periods[i] = period;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrecalculatedDateTimeZone"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="periods">The periods.</param>
        /// <param name="tailZone">The tail zone.</param>
        private PrecalculatedDateTimeZone(string id, ZoneInterval[] periods, IDateTimeZone tailZone)
            : base(id, false)
        {
            this.tailZone = tailZone;
            this.periods = periods;
        }

        /// <summary>
        /// Gets the zone offset period for the given instant. Null is returned if no period is defined by the time zone
        /// for the given instant.
        /// </summary>
        /// <param name="instant">The Instant to test.</param>
        /// <returns>The defined ZoneOffsetPeriod or <c>null</c>.</returns>
        public override ZoneInterval GetZoneInterval(Instant instant)
        {
            int last = this.periods.Length - 1;
            if (this.periods[last].End <= instant)
            {
                return this.tailZone.GetZoneInterval(instant);
            }
            for (var p = last; p >= 0; p--)
            {
                if (this.periods[p].End <= instant)
                {
                    break;
                }
                if (this.periods[p].Contains(instant))
                {
                    return this.periods[p];
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
        public override ZoneInterval GetZoneInterval(LocalInstant localInstant)
        {
            int last = this.periods.Length - 1;
            if (this.periods[last].LocalEnd <= localInstant)
            {
                return this.tailZone.GetZoneInterval(localInstant);
            }
            for (var p = last; p >= 0; p--)
            {
                if (this.periods[p].LocalEnd <= localInstant)
                {
                    break;
                }
                if (this.periods[p].Contains(localInstant))
                {
                    return this.periods[p];
                }
            }
            return null;
        }

        /// <summary>
        /// Returns true if this time zone is worth caching. Small time zones or time zones with
        /// lots of quick changes do not work well with <see cref="CachedDateTimeZone"/>.
        /// </summary>
        /// <returns><c>true</c> if this instance is cachable; otherwise, <c>false</c>.</returns>
        public bool IsCachable()
        {
            return this.tailZone != null;
        }

        #region I/O

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
            writer.WriteCount(this.periods.Length);
            foreach (var period in this.periods)
            {
                writer.WriteInstant(period.Start);
                writer.WriteString(period.Name);
                writer.WriteOffset(period.Offset);
                writer.WriteOffset(period.Savings);
            }
            var end = this.periods[this.periods.Length - 1].End;
            if (end != Instant.MaxValue)
            {
                end = end + Duration.One;
            }
            writer.WriteInstant(end);
            writer.WriteTimeZone(this.tailZone);
        }

        /// <summary>
        /// Reads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static IDateTimeZone Read(DateTimeZoneReader reader, string id)
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
    }
}