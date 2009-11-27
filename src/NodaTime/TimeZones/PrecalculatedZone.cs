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

namespace NodaTime.TimeZones
{
    internal class PrecalculatedZone
        : IDateTimeZone
    {
        /**
         * Factory to create instance from builder.
         * 
         * @param id  the zone id
         * @param outputID  true if the zone id should be output
         * @param transitions  the list of Transition objects
         * @param tailZone  optional zone for getting info beyond precalculated tables
         */
        static PrecalculatedZone create(String id, bool outputID, List<ZoneTransition> transitions, DSTZone tailZone)
        {
            int size = transitions.Count;
            if (size == 0) {
                throw new ArgumentException();
            }

            LocalInstant[] trans = new LocalInstant[size];
            Duration[] wallOffsets = new Duration[size];
            Duration[] standardOffsets = new Duration[size];
            String[] nameKeys = new String[size];

            ZoneTransition last = null;
            for (int i = 0; i < size; i++) {
                ZoneTransition tr = transitions[i];

                if (!tr.IsTransitionFrom(last)) {
                    throw new ArgumentException(id);
                }

                trans[i] = tr.Instant;
                wallOffsets[i] = tr.WallOffset;
                standardOffsets[i] = tr.StandardOffset;
                nameKeys[i] = tr.Name;

                last = tr;
            }
            /*
            // Some timezones (Australia) have the same name key for
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
            if (tailZone != null) {
                if (tailZone.StartRecurrence.Name.Equals(tailZone.EndRecurrence.Name)) {
                    // TODO: can't use console
                    Console.WriteLine("Fixing duplicate recurrent name key - " + tailZone.StartRecurrence.Name);
                    if (tailZone.StartRecurrence.Savings > Duration.Zero) {
                        tailZone = new DSTZone(
                            tailZone.Id,
                            tailZone.StandardOffset,
                            tailZone.StartRecurrence.RenameAppend("-Summer"),
                            tailZone.EndRecurrence);
                    }
                    else {
                        tailZone = new DSTZone(
                            tailZone.Id,
                            tailZone.StandardOffset,
                            tailZone.StartRecurrence,
                            tailZone.EndRecurrence.RenameAppend("-Summer"));
                    }
                }
            }

            return new PrecalculatedZone((outputID ? id : ""), trans, wallOffsets, standardOffsets, nameKeys, tailZone);
        }

        // All array fields have the same length.

        internal LocalInstant[] Transitions { get; set; }

        internal Duration[] WallOffsets { get; set; }
        internal Duration[] StandardOffsets { get; set; }
        internal String[] NameKeys { get; set; }

        internal DSTZone iTailZone { get; set; }

        /**
         * Constructor used ONLY for valid input, loaded via static methods.
         */
        private PrecalculatedZone(String id, LocalInstant[] transitions, Duration[] wallOffsets, Duration[] standardOffsets, String[] nameKeys, DSTZone tailZone)
        {
            Id = id;
            Transitions = transitions;
            WallOffsets = wallOffsets;
            StandardOffsets = standardOffsets;
            NameKeys = nameKeys;
            iTailZone = tailZone;
        }

        public String getNameKey(LocalInstant instant)
        {
            LocalInstant[] transitions = Transitions;
            int i = Array.BinarySearch(transitions, instant);
            if (i >= 0) {
                return NameKeys[i];
            }
            i = ~i;
            if (i < transitions.Length) {
                if (i > 0) {
                    return NameKeys[i - 1];
                }
                return "UTC";
            }
            if (iTailZone == null) {
                return NameKeys[i - 1];
            }
            return iTailZone.getNameKey(instant);
        }

        public Duration getOffset(LocalInstant instant)
        {
            LocalInstant[] transitions = Transitions;
            int i = Array.BinarySearch(transitions, instant);
            if (i >= 0) {
                return WallOffsets[i];
            }
            i = ~i;
            if (i < transitions.Length) {
                if (i > 0) {
                    return WallOffsets[i - 1];
                }
                return Duration.Zero;
            }
            if (iTailZone == null) {
                return WallOffsets[i - 1];
            }
            return iTailZone.getOffset(instant);
        }

        public Duration getStandardOffset(LocalInstant instant)
        {
            LocalInstant[] transitions = Transitions;
            int i = Array.BinarySearch(transitions, instant);
            if (i >= 0) {
                return StandardOffsets[i];
            }
            i = ~i;
            if (i < transitions.Length) {
                if (i > 0) {
                    return StandardOffsets[i - 1];
                }
                return Duration.Zero;
            }
            if (iTailZone == null) {
                return StandardOffsets[i - 1];
            }
            return iTailZone.getStandardOffset(instant);
        }

        public LocalInstant nextTransition(LocalInstant instant)
        {
            LocalInstant[] transitions = Transitions;
            int i = Array.BinarySearch(transitions, instant);
            i = (i >= 0) ? (i + 1) : ~i;
            if (i < transitions.Length) {
                return transitions[i];
            }
            if (iTailZone == null) {
                return instant;
            }
            LocalInstant end = transitions[transitions.Length - 1];
            if (instant < end) {
                instant = end;
            }
            return iTailZone.nextTransition(instant);
        }

        public LocalInstant previousTransition(LocalInstant instant)
        {
            LocalInstant[] transitions = Transitions;
            int i = Array.BinarySearch(transitions, instant);
            if (i >= 0) {
                if (instant > LocalInstant.MinValue) {
                    return instant - Duration.One;
                }
                return instant;
            }
            i = ~i;
            if (i < transitions.Length) {
                if (i > 0) {
                    LocalInstant prev = transitions[i - 1];
                    if (prev > LocalInstant.MinValue) {
                        return prev - Duration.One;
                    }
                }
                return instant;
            }
            if (iTailZone != null) {
                LocalInstant prev = iTailZone.previousTransition(instant);
                if (prev < instant) {
                    return prev;
                }
            }
            LocalInstant previous = transitions[i - 1];
            if (previous > LocalInstant.MinValue) {
                return previous - Duration.One;
            }
            return instant;
        }

        public bool equals(Object obj)
        {
            if (this == obj) {
                return true;
            }
            if (obj is PrecalculatedZone) {
                PrecalculatedZone other = (PrecalculatedZone)obj;
                return
                    Id == other.Id &&
                    Transitions.Equals(other.Transitions) &&
                    NameKeys.Equals(other.NameKeys) &&
                    WallOffsets.Equals(other.WallOffsets) &&
                    StandardOffsets.Equals(other.StandardOffsets) &&
                    ((iTailZone == null)
                     ? (null == other.iTailZone)
                     : (iTailZone.equals(other.iTailZone)));
            }
            return false;
        }

        public bool isCachable()
        {
            if (iTailZone != null) {
                return true;
            }
            LocalInstant[] transitions = Transitions;
            if (transitions.Length <= 1) {
                return false;
            }

            // Add up all the distances between transitions that are less than
            // about two years.
            double distances = 0;
            int count = 0;

            for (int i = 1; i < transitions.Length; i++) {
                Duration diff = transitions[i] - transitions[i - 1];
                if (diff.Ticks < ((366L + 365) * 24 * 60 * 60 * 1000)) {
                    distances += (double)diff.Ticks;
                    count++;
                }
            }

            if (count > 0) {
                double avg = distances / count;
                avg /= 24 * 60 * 60 * 1000;
                if (avg >= 25) {
                    // Only bother caching if average distance between
                    // transitions is at least 25 days. Why 25?
                    // CachedDateTimeZone is more efficient if the distance
                    // between transitions is large. With an average of 25, it
                    // will on average perform about 2 tests per cache
                    // hit. (49.7 / 25) is approximately 2.
                    return true;
                }
            }

            return false;
        }

        #region IDateTimeZone Members

        public Instant? NextTransition(Instant instant)
        {
            throw new NotImplementedException();
        }

        public Instant? PreviousTransition(Instant instant)
        {
            throw new NotImplementedException();
        }

        public Duration GetOffsetFromUtc(Instant instant)
        {
            throw new NotImplementedException();
        }

        public Duration GetOffsetFromLocal(LocalDateTime localTime)
        {
            throw new NotImplementedException();
        }

        public string Id { get; private set; }

        public bool IsFixed { get; private set; }

        #endregion
    }
}

