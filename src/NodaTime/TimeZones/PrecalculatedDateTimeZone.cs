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

        internal static PrecalculatedDateTimeZone Create(String id, List<ZoneTransition> transitions, IDateTimeZone tailZone)
        {
            if (transitions == null)
            {
                throw new ArgumentNullException("transitions");
            }
            int size = transitions.Count;
            if (size == 0)
            {
                throw new ArgumentException("There must be at least one transition", "transitions");
            }

            var trans = new Instant[size];
            var wallOffsets = new Offset[size];
            var standardOffsets = new Offset[size];
            var nameKeys = new String[size];

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

        internal readonly ZoneOffsetPeriod[] periods;

        internal readonly Instant[] Transitions;
        internal readonly Offset[] WallOffsets;
        internal readonly Offset[] StandardOffsets;
        internal readonly String[] NameKeys;
        internal readonly IDateTimeZone TailZone;

        /**
         * Constructor used ONLY for valid input, loaded via static methods.
         */

        private PrecalculatedDateTimeZone(String id, Instant[] transitions, Offset[] wallOffsets,
                                          Offset[] standardOffsets, String[] nameKeys, IDateTimeZone tailZone)
            : base(id, false)
        {
            this.Transitions = transitions;
            this.WallOffsets = wallOffsets;
            this.StandardOffsets = standardOffsets;
            this.NameKeys = nameKeys;
            this.TailZone = tailZone;
        }

        /// <summary>
        ///    Gets the last offset from the given array.
        /// </summary>
        /// <param name="offsets">The offsets array.</param>
        /// <returns>
        ///    Returns the last offset in the array or
        ///    <see cref="Duration.Zero"/>
        ///    if the array is empty.
        /// </returns>
        private static Offset GetLatestOffset(Offset[] offsets)
        {
            if (offsets.Length > 0)
            {
                return offsets[offsets.Length - 1];
            }
            return Offset.Zero;
        }

        /// <summary>
        ///    Returns true if this time zone is worth caching. Small time zones or time zones with
        ///    lots of quick changes do not work well with
        ///    <see cref="CachedDateTimeZone"/>
        ///    .
        /// </summary>
        /// <returns>
        ///    <c>true</c>
        ///    if this instance is cachable; otherwise,
        ///    <c>false</c>
        ///    .
        /// </returns>
        public bool IsCachable()
        {
            if (this.TailZone != null)
            {
                return true;
            }
            return CachedDateTimeZone.IsWorthCaching(this.Transitions);
        }

        #region DateTimeZoneBase Members

        /// <summary>
        ///    Returns the transition occurring strictly after the specified instant,
        ///    or null if there are no further transitions.
        /// </summary>
        /// <param name="instant">
        ///    The instant after which to consider transitions.
        /// </param>
        /// <returns>
        ///    The instant of the next transition, or null if there are no further transitions.
        /// </returns>
        public override Transition? NextTransition(Instant instant)
        {
            int i = Array.BinarySearch(this.Transitions, instant);
            i = (i >= 0) ? (i + 1) : ~i;
            if (i < this.Transitions.Length)
            {
                Offset previousOffset = i > 0 ? WallOffsets[i - 1] : Offset.Zero;
                return new Transition(Transitions[i], previousOffset, WallOffsets[i]);
            }
            if (this.TailZone == null)
            {
                return null;
            }
            Instant end = this.Transitions[this.Transitions.Length - 1];
            if (instant < end)
            {
                instant = end;
            }
            return this.TailZone.NextTransition(instant);
        }

        /// <summary>
        ///    Returns the transition occurring strictly before the specified instant,
        ///    or null if there are no earlier transitions.
        /// </summary>
        /// <param name="instant">
        ///    The instant before which to consider transitions.
        /// </param>
        /// <returns>
        ///    The instant of the previous transition, or null if there are no further transitions.
        /// </returns>
        public override Transition? PreviousTransition(Instant instant)
        {
            if (instant == Instant.MinValue)
            {
                return null;
            }
            int i = Array.BinarySearch(this.Transitions, instant);
            // If we find a transition, then the given instant is on a transition, and we want the
            // one before it.
            // If we don't find a transition, then ~i will be the index of next transition,
            // so again we want the one before it... unless we've actually gone off the end of
            // all the transitions, in which case we need to look at the tail zone (if any).
            i = (i < 0 ? ~i : i) - 1;
            if (i < this.Transitions.Length - 1)
            {
                if (i == -1 || Transitions[i] == Instant.MinValue)
                {
                    return null;
                }
                // Assume offset of zero before the first transition. This is consistent with GetOffsetFromUtc
                Offset previousOffset = i > 0 ? WallOffsets[i - 1] : Offset.Zero;
                return new Transition(Transitions[i], previousOffset, WallOffsets[i]);
            }
            // The instant is after the last transition. If we have a tail zone, ask that
            // for the transition; otherwise check whether our last transition is actually valid.
            if (this.TailZone != null)
            {
                Transition? prev = this.TailZone.PreviousTransition(instant);
                if (prev.HasValue && prev.Value.Instant < instant)
                {
                    return prev;
                }
            }
            Instant previous = this.Transitions[i - 1];
            if (previous > Instant.MinValue)
            {
                return new Transition(previous, WallOffsets[i - 2], WallOffsets[i - 1]);
            }
            // When would this happen?
            return null;
        }

        /// <summary>
        ///    Returns the offset from UTC, such that local time = UTC + offset.
        /// </summary>
        /// <param name="instant">
        ///    The instant for which to calculate the offset.
        /// </param>
        /// <returns>
        ///    The offset from UTC at the specified instant.
        /// </returns>
        public override Offset GetOffsetFromUtc(Instant instant)
        {
            int i = Array.BinarySearch(this.Transitions, instant);
            if (i >= 0)
            {
                return this.WallOffsets[i];
            }
            i = ~i;
            if (i < this.Transitions.Length)
            {
                if (i > 0)
                {
                    return this.WallOffsets[i - 1];
                }
                return Offset.Zero;
            }
            if (this.TailZone == null)
            {
                return this.WallOffsets[i - 1];
            }
            return this.TailZone.GetOffsetFromUtc(instant);
        }

        /// <summary>
        ///    Returns the name associated with the given instant.
        /// </summary>
        /// <param name="instant">
        ///    The instant to get the name for.
        /// </param>
        /// <returns>
        ///    The name of this time. Never returns null.
        /// </returns>
        /// <remarks>
        ///    For a fixed time zone this will always return the same value but for a time zone that
        ///    honors daylight savings this will return a different name depending on the time of year
        ///    it represents. For example in the Pacific Standard Time (UTC-8) it will return either
        ///    PST or PDT depending on the time of year.
        /// </remarks>
        public override string Name(Instant instant)
        {
            int i = Array.BinarySearch(this.Transitions, instant);
            if (i >= 0)
            {
                return this.NameKeys[i];
            }
            i = ~i;
            if (i < this.Transitions.Length)
            {
                if (i > 0)
                {
                    return this.NameKeys[i - 1];
                }
                return DateTimeZones.UtcId;
            }
            if (this.TailZone == null)
            {
                return this.NameKeys[i - 1];
            }
            return this.TailZone.Name(instant);
        }

        #endregion

        public override void Write(DateTimeZoneWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            int size = this.Transitions.Length;

            // Create unique string pool.
            var poolSet = new Dictionary<string, string>();
            for (int i = 0; i < size; i++)
            {
                if (!poolSet.ContainsKey(this.NameKeys[i]))
                {
                    poolSet.Add(this.NameKeys[i], null);
                }
            }

            int poolSize = poolSet.Count;
            var pool = new String[poolSize];
            poolSet.Keys.CopyTo(pool, 0);
            writer.WriteNumber(poolSize);
            for (int i = 0; i < poolSize; i++)
            {
                writer.WriteString(pool[i]);
            }

            writer.WriteNumber(size);
            for (int i = 0; i < size; i++)
            {
                writer.WriteTicks(this.Transitions[i].Ticks);
                writer.WriteOffset(this.WallOffsets[i]);
                writer.WriteOffset(this.StandardOffsets[i]);
                string name = this.NameKeys[i];
                for (int p = 0; p < poolSize; p++)
                {
                    if (pool[p] == name)
                    {
                        writer.WriteNumber(p);
                        break;
                    }
                }
            }
            bool hasTailZone = this.TailZone != null;
            writer.WriteBoolean(hasTailZone);
            if (hasTailZone)
            {
                writer.WriteTimeZone(this.TailZone);
            }
        }

        public static IDateTimeZone Read(DateTimeZoneReader reader, string id)
        {
            int poolSize = reader.ReadNumber();
            var pool = new string[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                pool[i] = reader.ReadString();
            }

            int size = reader.ReadNumber();
            var transitions = new Instant[size];
            var wallOffsets = new Offset[size];
            var standardOffsets = new Offset[size];
            var nameKeys = new String[size];
            for (int i = 0; i < size; i++)
            {
                long ticks = reader.ReadTicks();
                transitions[i] = new Instant(ticks);
                wallOffsets[i] = reader.ReadOffset();
                standardOffsets[i] = reader.ReadOffset();
                int index = reader.ReadNumber();
                nameKeys[i] = pool[index];
            }
            bool hasTailZone = reader.ReadBoolean();
            IDateTimeZone tailZone = null;
            if (hasTailZone)
            {
                tailZone = reader.ReadTimeZone(id + "-tail");
            }

            return new PrecalculatedDateTimeZone(id, transitions, wallOffsets, standardOffsets, nameKeys, tailZone);
        }
    }
}