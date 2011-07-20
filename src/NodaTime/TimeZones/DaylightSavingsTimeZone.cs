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
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides a basic daylight savings time zone. A DST time zone has a simple recurrence
    /// where an extra offset is applied between two dates of a year.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: This class does *not* represent a general purpose, "good for all time"
    /// time zone. It is *only* used as a tail zone within PrecalculatedDateTimeZone; as such
    /// it may not extend the start of time, although it *must* extend to the end of time;
    /// this is validated on construction. Calls to GetZoneInterval(Instant) may fail
    /// with an ArgumentOutOfRangeException, but 
    /// </remarks>
    internal class DaylightSavingsTimeZone : DateTimeZone, IEquatable<DaylightSavingsTimeZone>
    {
        private readonly ZoneRecurrence standardRecurrence;
        private readonly Offset standardOffset;
        private readonly ZoneRecurrence dstRecurrence;
        private readonly Instant minimumValidInstant;

        /// <summary>
        /// Initializes a new instance of the <see cref="DaylightSavingsTimeZone"/> class.
        /// </summary>
        /// <remarks>
        /// At least one of the recurrences (it doesn't matter which) must be a "standard", i.e. not have any savings
        /// applied. The other may still not have any savings (e.g. for America/Resolute) but any savings must be
        /// non-negative.
        /// </remarks>
        /// <param name="id">The id.</param>
        /// <param name="standardOffset">The standard offset.</param>
        /// <param name="startRecurrence">The start recurrence.</param>
        /// <param name="endRecurrence">The end recurrence.</param>
        internal DaylightSavingsTimeZone(String id, Offset standardOffset, ZoneRecurrence startRecurrence, ZoneRecurrence endRecurrence) : base(id, false)
        {
            if (startRecurrence == null)
            {
                throw new ArgumentNullException("startRecurrence");
            }
            if (endRecurrence == null)
            {
                throw new ArgumentNullException("endRecurrence");
            }
            this.standardOffset = standardOffset;
            var dst = startRecurrence;
            var standard = endRecurrence;
            if (!dst.IsInfinite || !standard.IsInfinite)
            {
                throw new ArgumentException("Both recurrences must extend to the end of time");
            }
            if (startRecurrence.Savings == Offset.Zero)
            {
                dst = endRecurrence;
                standard = startRecurrence;
            }
            if (dst.Savings < Offset.Zero)
            {
                // Not necessarily positive... America/Resolute ends up switching
                // between two different zone names, neither of which has daylight savings...
                throw new ArgumentException("Daylight savings must be non-negative");
            }
            if (standard.Savings != Offset.Zero)
            {
                throw new ArgumentException("At least one recurrence must not have savings applied");
            }
            if (dst.Name == standard.Name)
            {
                dst = dst.RenameAppend("-Summer");
            }
            dstRecurrence = dst;
            standardRecurrence = standard;
            minimumValidInstant = ComputeMinimumValidInstant(dstRecurrence, standardRecurrence, standardOffset);
        }

        /// <summary>
        /// Computes the minimum instant which is covered by the combination of the two recurrences.
        /// This isn't necessarily just "the minimum value of the later recurrence" as the earlier one
        /// may have one more valid transition.
        /// </summary>
        private static Instant ComputeMinimumValidInstant(ZoneRecurrence dstRecurrence, ZoneRecurrence standardRecurrence,
            Offset standardOffset)
        {
            int year = Math.Max(dstRecurrence.FromYear, standardRecurrence.FromYear);
            if (year == int.MinValue)
            {
                return Instant.MinValue;
            }
            // Go for two years later, from February 1st - just to get a good run into things. We know there'll
            // be at least a couple of transitions for each recurrence, so we can arbitrarily start with the
            // DST one.
            Instant current = Instant.FromUtc(year + 2, 2, 1, 0, 0);
            // Keep walking backwards until one of of the recurrences gives up.
            while (current > Instant.MinValue)
            {
                Transition? dstTransition = dstRecurrence.Previous(current, standardOffset, Offset.Zero);
                if (dstTransition == null)
                {
                    break;
                }
                current = dstTransition.Value.Instant;
                Transition? standardTransition = standardRecurrence.Previous(current, standardOffset, dstRecurrence.Savings);
                if (standardTransition == null)
                {
                    break;
                }
                current = standardTransition.Value.Instant;                
            }
            return current;
        }

        #region IEquatable<DaylightSavingsTimeZone> Members
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(DaylightSavingsTimeZone other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Id == other.Id && standardOffset == other.standardOffset && dstRecurrence.Equals(other.dstRecurrence) &&
                   standardRecurrence.Equals(other.standardRecurrence);
        }
        #endregion

        #region Object overrides
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        ///                 </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.
        ///                 </exception><filterpriority>2</filterpriority>
        public override bool Equals(Object obj)
        {
            return Equals(obj as DaylightSavingsTimeZone);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            int hashCode = HashCodeHelper.Initialize();
            hashCode = HashCodeHelper.Hash(hashCode, Id);
            hashCode = HashCodeHelper.Hash(hashCode, standardOffset);
            hashCode = HashCodeHelper.Hash(hashCode, dstRecurrence);
            hashCode = HashCodeHelper.Hash(hashCode, standardRecurrence);
            return hashCode;
        }
        #endregion // Object overrides

        /// <summary>
        /// Gets the zone interval for the given instant.
        /// </summary>
        /// <param name="instant">The Instant to test.</param>
        /// <returns>The ZoneInterval in effect at the given instant.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The instant falls outside the bounds
        /// of the recurrence rules of the zone.</exception>
        public override ZoneInterval GetZoneInterval(Instant instant)
        {
            var previous = PreviousTransition(instant + Duration.One);
            var next = NextTransition(instant);

            if (!previous.HasValue || !next.HasValue)
            {
                throw new ArgumentOutOfRangeException("instant",
                    "Specified instant is not covered by the recurrence rules of this time zone");
            }
            var recurrence = FindMatchingRecurrence(instant);
            return new ZoneInterval(recurrence.Name, previous.Value.Instant, next.Value.Instant,
                standardOffset + recurrence.Savings, recurrence.Savings);
        }

        /// <summary>
        /// Gets the zone offset period for the given local instant.
        /// </summary>
        /// <param name="localInstant">The LocalInstant to test.</param>
        /// <exception cref="SkippedTimeException"></exception>
        /// <returns>The ZoneInterval containing the given local instant.</returns>
        internal override ZoneInterval GetZoneInterval(LocalInstant localInstant)
        {
            var standard = localInstant.Minus(standardOffset);
            var daylight = localInstant.Minus(standardOffset + dstRecurrence.Savings);
            var normalRecurrence = FindMatchingRecurrence(standard);
            var daylightRecurrence = FindMatchingRecurrence(daylight);

            // If the normal instant only occurs in the DST recurrence, and the daylight instant only occurs in the
            // standard recurrence, the local instant must be in a gap.
            if (ReferenceEquals(normalRecurrence, dstRecurrence) && ReferenceEquals(daylightRecurrence, standardRecurrence))
            {
                throw new SkippedTimeException(localInstant, this);
            }
            return GetZoneInterval(standard);
        }

        internal override ZoneIntervalPair GetZoneIntervals(LocalInstant localInstant)
        {
            // Work out the instants that this local instant would correspond with
            // in each of standard or daylight saving time.
            var standard = localInstant.Minus(standardOffset);
            var daylight = localInstant.Minus(standardOffset + dstRecurrence.Savings);

            // If daylight is invalid but standard is valid, we must start off with a winter recurrence, which we can return.
            if (standard < minimumValidInstant)
            {
                return ZoneIntervalPair.NoMatch;
            }
            if (daylight < minimumValidInstant)
            {
                // TODO: Make this more efficient by remembering minimum values for
                // each recurrence.
                return FindMatchingRecurrence(standard) == standardRecurrence
                    ? new ZoneIntervalPair(GetZoneInterval(standard), null)
                    : ZoneIntervalPair.NoMatch;
            }

            var normalRecurrence = FindMatchingRecurrence(standard);
            var daylightRecurrence = FindMatchingRecurrence(daylight);

            // If the normal instant only occurs in the DST recurrence, and the daylight instant only occurs in the
            // standard recurrence, the local instant must be in a gap.
            if (ReferenceEquals(normalRecurrence, dstRecurrence) && ReferenceEquals(daylightRecurrence, standardRecurrence))
            {
                return ZoneIntervalPair.NoMatch;
            }
            // If the normal instant occurs in the standard recurrence, and the daylight instant occurs in the
            // DST recurrence, the local instant must be ambiguous.
            if (ReferenceEquals(normalRecurrence, standardRecurrence) && ReferenceEquals(daylightRecurrence, dstRecurrence))
            {   
                // The instant corresponding with daylight saving time is always earlier than the instant
                // corresponding with standard time.
                return new ZoneIntervalPair(GetZoneInterval(daylight), GetZoneInterval(standard));
            }
            // Unambiguous case
            return new ZoneIntervalPair(GetZoneInterval(standard), null);
        }

        /// <summary>
        /// Finds the recurrence containing the given instant, if any.
        /// </summary>
        /// <returns>The recurrence containing the given instant, or null if
        /// the instant occurs before the start of the earlier recurrence.</returns>
        private ZoneRecurrence FindMatchingRecurrence(Instant instant)
        {
            // FIXME: Check against minimum instant.

            // Find the transitions which start *after* the one we're currently in - then
            // pick the later of them, which will be the same "polarity" as the one we're currently
            // in.
            Transition? nextDstStart = dstRecurrence.Next(instant, standardOffset, standardRecurrence.Savings);
            Transition? nextStandardStart = standardRecurrence.Next(instant, standardOffset, dstRecurrence.Savings);
            // Both transitions must be non-null, as our recurrences are infinite.
            return nextDstStart.Value.Instant > nextStandardStart.Value.Instant ? dstRecurrence : standardRecurrence;
        }

        /// <summary>
        /// Returns the transition occurring strictly after the specified instant,
        /// or null if there are no further transitions.
        /// </summary>
        /// <param name="instant">The instant after which to consider transitions.</param>
        /// <returns>
        /// The instant of the next transition, or null if there are no further transitions.
        /// </returns>
        private Transition? NextTransition(Instant instant)
        {
            Transition? start = dstRecurrence.Next(instant, standardOffset, standardRecurrence.Savings);
            Transition? end = standardRecurrence.Next(instant, standardOffset, dstRecurrence.Savings);
            if (start.HasValue)
            {
                if (end.HasValue)
                {
                    return (start.Value.Instant > end.Value.Instant) ? end : start;
                }
                return start;
            }
            return end;
        }

        /// <summary>
        /// Returns the transition occurring strictly before the specified instant,
        /// or null if there are no earlier transitions.
        /// </summary>
        /// <param name="instant">The instant before which to consider transitions.</param>
        /// <returns>
        /// The instant of the previous transition, or null if there are no further transitions.
        /// </returns>
        private Transition? PreviousTransition(Instant instant)
        {
            if (instant <= minimumValidInstant)
            {
                return null;
            }

            Transition? start = dstRecurrence.Previous(instant, standardOffset, standardRecurrence.Savings);
            Transition? end = standardRecurrence.Previous(instant, standardOffset, dstRecurrence.Savings);

            Transition? result = end;
            if (start.HasValue)
            {
                if (end.HasValue)
                {
                    result = (start.Value.Instant > end.Value.Instant) ? start : end;
                }
                else
                {
                    result = start;
                }
            }
            return result;
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
            return FindMatchingRecurrence(instant).Savings + standardOffset;
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
        public override string GetName(Instant instant)
        {
            return FindMatchingRecurrence(instant).Name;
        }

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
            writer.WriteOffset(standardOffset);
            dstRecurrence.Write(writer);
            standardRecurrence.Write(writer);
        }

        internal static DateTimeZone Read(DateTimeZoneReader reader, string id)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            Offset offset = reader.ReadOffset();
            ZoneRecurrence start = ZoneRecurrence.Read(reader);
            ZoneRecurrence end = ZoneRecurrence.Read(reader);
            return new DaylightSavingsTimeZone(id, offset, start, end);
        }
    }
}