#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
using System.Text;

namespace NodaTime.TimeZones
{
    internal class PrecalculatedDateTimeZone
        : DateTimeZoneBase
    {
        /**
         * Factory to create instance from builder.
         * 
         * @param id  the zone id
         * @param transitions  the list of Transition objects
         * @param tailZone  optional zone for getting info beyond precalculated tables
         */
        internal static PrecalculatedDateTimeZone create(String id, List<ZoneTransition> transitions, IDateTimeZone tailZone)
        {
            int size = transitions.Count;
            if (size == 0)
            {
                throw new ArgumentException();
            }

            Instant[] trans = new Instant[size];
            Offset[] wallOffsets = new Offset[size];
            Offset[] standardOffsets = new Offset[size];
            String[] nameKeys = new String[size];

            ZoneTransition last = null;
            for (int i = 0; i < size; i++)
            {
                ZoneTransition tr = transitions[i];

                if (!tr.IsTransitionFrom(last))
                {
                    throw new ArgumentException(id);
                }

                trans[i] = tr.Instant;
                wallOffsets[i] = tr.WallOffset;
                standardOffsets[i] = tr.StandardOffset;
                nameKeys[i] = tr.Name;

                last = tr;
            }
            /*
            // TODO: this code needs to be resurrected in some form for Noda Time
            //
            // Some time zones (Australia) have the same name key for
            // summer and winter which messes everything up. Fix it here.
            String[] zoneNameData = new String[5];
            String[][] zoneStrings = new DateFormatSymbols(Locale.ENGLISH).getZoneStrings();
            for (int j = 0; j < zoneStrings.Length; j++) {
                String[] set = zoneStrings[j];
                if (set != null && set.Length == 5 && id.Equals(set[0])) {
                    zoneNameData = set;
                }
            }
            for (int i = 0; i < nameKeys.Length - 1; i++) {
                String curNameKey = nameKeys[i];
                String nextNameKey = nameKeys[i + 1];
                long curOffset = wallOffsets[i];
                long nextOffset = wallOffsets[i + 1];
                long curStdOffset = standardOffsets[i];
                long nextStdOffset = standardOffsets[i + 1];
                Period p = new Period(trans[i], trans[i + 1], PeriodType.YearMonthDay);
                if (curOffset != nextOffset &&
                        curStdOffset == nextStdOffset &&
                        curNameKey.Equals(nextNameKey) &&
                        p.Years == 0 && p.Months > 4 && p.Months < 8 &&
                        curNameKey.Equals(zoneNameData[2]) &&
                        curNameKey.Equals(zoneNameData[4])) {

                    // TODO: can't use console
                    Console.WriteLine("Fixing duplicate name key - " + nextNameKey);
                    Console.WriteLine("     - " + new DateTime(trans[i]) + " - " + new DateTime(trans[i + 1]));
                    if (curOffset > nextOffset) {
                        nameKeys[i] = (curNameKey + "-Summer");
                    }
                    else if (curOffset < nextOffset) {
                        nameKeys[i + 1] = (nextNameKey + "-Summer");
                        i++;
                    }
                }
            }
            */

            return new PrecalculatedDateTimeZone(id, trans, wallOffsets, standardOffsets, nameKeys, tailZone);
        }

        // All array fields have the same length.

        private readonly Instant[] transitions;
        private readonly Offset[] wallOffsets;
        private readonly Offset[] standardOffsets;
        private readonly String[] nameKeys;
        private readonly IDateTimeZone tailZone;

        /**
         * Constructor used ONLY for valid input, loaded via static methods.
         */
        private PrecalculatedDateTimeZone(String id, Instant[] transitions, Offset[] wallOffsets, Offset[] standardOffsets, String[] nameKeys, IDateTimeZone tailZone)
            : base(id, GetLatestOffset(standardOffsets), false)
        {
            this.transitions = transitions;
            this.wallOffsets = wallOffsets;
            this.standardOffsets = standardOffsets;
            this.nameKeys = nameKeys;
            this.tailZone = tailZone;
        }

        /// <summary>
        /// Gets the last offset from the given array.
        /// </summary>
        /// <param name="standardOffsets">The offsets array.</param>
        /// <returns>Returns the last offset in the array or <see cref="Duration.Zero"/> if the array is empty.</returns>
        private static Offset GetLatestOffset(Offset[] offsets)
        {
            if (offsets.Length > 0)
            {
                return offsets[offsets.Length - 1];
            }
            return Offset.Zero;
        }

        /// <summary>
        /// Returns true if this time zone is worth caching. Small time zones or time zones with
        /// lots of quick changes do not work well with <see cref="CachedDateTimeZone"/>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance is cachable; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCachable()
        {
            if (this.tailZone != null)
            {
                return true;
            }
            return CachedDateTimeZone.IsWorthCaching(this.transitions);
        }

        #region DateTimeZoneBase Members

        /// <summary>
        /// Returns the transition occurring strictly after the specified instant,
        /// or null if there are no further transitions.
        /// </summary>
        /// <param name="instant">The instant after which to consider transitions.</param>
        /// <returns>
        /// The instant of the next transition, or null if there are no further transitions.
        /// </returns>
        public override Instant? NextTransition(Instant instant)
        {
            int i = Array.BinarySearch(this.transitions, instant);
            i = (i >= 0) ? (i + 1) : ~i;
            if (i < this.transitions.Length)
            {
                return this.transitions[i];
            }
            if (this.tailZone == null)
            {
                return instant;
            }
            Instant end = this.transitions[this.transitions.Length - 1];
            if (instant < end)
            {
                instant = end;
            }
            return this.tailZone.NextTransition(instant);
        }

        /// <summary>
        /// Returns the transition occurring strictly before the specified instant,
        /// or null if there are no earlier transitions.
        /// </summary>
        /// <param name="instant">The instant before which to consider transitions.</param>
        /// <returns>
        /// The instant of the previous transition, or null if there are no further transitions.
        /// </returns>
        public override Instant? PreviousTransition(Instant instant)
        {
            int i = Array.BinarySearch(this.transitions, instant);
            if (i >= 0)
            {
                if (instant > Instant.MinValue)
                {
                    return instant - Duration.One;
                }
                return instant;
            }
            i = ~i;
            if (i < this.transitions.Length)
            {
                if (i > 0)
                {
                    Instant prev = this.transitions[i - 1];
                    if (prev > Instant.MinValue)
                    {
                        return prev - Duration.One;
                    }
                }
                return instant;
            }
            if (this.tailZone != null)
            {
                Instant? prev = this.tailZone.PreviousTransition(instant);
                if (prev.HasValue && prev.Value < instant)
                {
                    return prev;
                }
            }
            Instant previous = this.transitions[i - 1];
            if (previous > Instant.MinValue)
            {
                return previous - Duration.One;
            }
            return instant;
        }

        /// <summary>
        /// Returns the offset from local time to UTC, where a positive duration indicates that UTC is earlier
        /// than local time. In other words, UTC = local time - (offset from local).
        /// </summary>
        /// <param name="instant">The instant for which to calculate the offset.</param>
        /// <returns>The offset at the specified local time.</returns>
        public override Offset GetOffsetFromLocal(LocalInstant instant)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the offset from UTC, where a positive duration indicates that local time is later
        /// than UTC. In other words, local time = UTC + offset.
        /// </summary>
        /// <param name="instant">The instant for which to calculate the offset.</param>
        /// <returns>
        /// The offset from UTC at the specified instant.
        /// </returns>
        public override Offset GetOffsetFromUtc(Instant instant)
        {
            int i = Array.BinarySearch(this.transitions, instant);
            if (i >= 0)
            {
                return this.wallOffsets[i];
            }
            i = ~i;
            if (i < this.transitions.Length)
            {
                if (i > 0)
                {
                    return this.wallOffsets[i - 1];
                }
                return Offset.Zero;
            }
            if (this.tailZone == null)
            {
                return this.wallOffsets[i - 1];
            }
            return this.tailZone.GetOffsetFromUtc(instant);
        }

        /// <summary>
        /// Returns the name associated with the given instant.
        /// </summary>
        /// <param name="instant">The instant to get the name for.</param>
        /// <returns>
        /// The name of this time. Never returns null.
        /// </returns>
        /// <remarks>
        /// For a fixed time zone this will always return the same value but for a time zone that
        /// honors daylight savings this will return a different name depending on the time of year
        /// it represents. For example in the Pacific Standard Time (UTC-8) it will return either
        /// PST or PDT depending on the time of year.
        /// </remarks>
        public override string Name(Instant instant)
        {
            int i = Array.BinarySearch(this.transitions, instant);
            if (i >= 0)
            {
                return this.nameKeys[i];
            }
            i = ~i;
            if (i < this.transitions.Length)
            {
                if (i > 0)
                {
                    return this.nameKeys[i - 1];
                }
                return DateTimeZones.UtcId;
            }
            if (this.tailZone == null)
            {
                return this.nameKeys[i - 1];
            }
            return this.tailZone.Name(instant);
        }

        #endregion
    }
}

