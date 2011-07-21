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
    /// IMPORTANT: This class *accepts* recurrences which start from a particular year
    /// rather than being infinite back to the start of time, but *treats* them as if
    /// they were infinite. This makes various calculations easier, but this zone should
    /// only be used as part of a PrecalculatedDateTimeZone which will only ask it for
    /// values within the right portion of the timeline.
    /// </remarks>
    internal class DaylightSavingsTimeZone : DateTimeZone, IEquatable<DaylightSavingsTimeZone>
    {
        private readonly ZoneRecurrence standardRecurrence;
        private readonly Offset standardOffset;
        private readonly ZoneRecurrence dstRecurrence;

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
        internal DaylightSavingsTimeZone(String id, Offset standardOffset, ZoneRecurrence startRecurrence, ZoneRecurrence endRecurrence)
            : base(id, false,
            standardOffset + Offset.Min(startRecurrence.Savings, endRecurrence.Savings),
            standardOffset + Offset.Max(startRecurrence.Savings, endRecurrence.Savings))
        {
            this.standardOffset = standardOffset;
            // Treat the recurrences as if they extended to the start of time.
            startRecurrence = startRecurrence.ToStartOfTime();
            endRecurrence = endRecurrence.ToStartOfTime();
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
            var recurrence = FindMatchingRecurrence(instant);
            return new ZoneInterval(recurrence.Name, previous.Instant, next.Instant,
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

        /// <summary>
        /// Finds the recurrence containing the given instant, if any.
        /// </summary>
        /// <returns>The recurrence containing the given instant, or null if
        /// the instant occurs before the start of the earlier recurrence.</returns>
        private ZoneRecurrence FindMatchingRecurrence(Instant instant)
        {
            // Find the transitions which start *after* the one we're currently in - then
            // pick the later of them, which will be the same "polarity" as the one we're currently
            // in.
            Transition? nextDstStart = dstRecurrence.Next(instant, standardOffset, standardRecurrence.Savings);
            Transition? nextStandardStart = standardRecurrence.Next(instant, standardOffset, dstRecurrence.Savings);
            // Both transitions must be non-null, as our recurrences are infinite.
            return nextDstStart.Value.Instant > nextStandardStart.Value.Instant ? dstRecurrence : standardRecurrence;
        }

        /// <summary>
        /// Returns the transition occurring strictly after the specified instant
        /// </summary>
        /// <param name="instant">The instant after which to consider transitions.</param>
        private Transition NextTransition(Instant instant)
        {
            Transition? dstTransition = dstRecurrence.Next(instant, standardOffset, standardRecurrence.Savings);
            Transition? standardTransition = standardRecurrence.Next(instant, standardOffset, dstRecurrence.Savings);
            if (dstTransition.HasValue)
            {
                if (standardTransition.HasValue)
                {
                    return (dstTransition.Value.Instant > standardTransition.Value.Instant) ? standardTransition.Value : dstTransition.Value;
                }
                return dstTransition.Value;
            }
            if (standardTransition.HasValue)
            {
                return standardTransition.Value;
            }
            throw new ArgumentOutOfRangeException("instant", "Infinite recurrences should always have a next transition");
        }

        /// <summary>
        /// Returns the transition occurring strictly before the specified instant.
        /// </summary>
        /// <param name="instant">The instant before which to consider transitions.</param>
        /// <returns>
        /// The instant of the previous transition, or null if there are no further transitions.
        /// </returns>
        private Transition PreviousTransition(Instant instant)
        {
            Transition? dstTransition = dstRecurrence.Previous(instant, standardOffset, standardRecurrence.Savings);
            Transition? standardTransition = standardRecurrence.Previous(instant, standardOffset, dstRecurrence.Savings);

            if (dstTransition.HasValue)
            {
                if (standardTransition.HasValue)
                {
                    return (dstTransition.Value.Instant > standardTransition.Value.Instant) ? dstTransition.Value : standardTransition.Value;
                }
                else
                {
                    return dstTransition.Value;
                }
            }
            if (standardTransition.HasValue)
            {
                return standardTransition.Value;
            }
            throw new ArgumentOutOfRangeException("instant", "Infinite (start of time) recurrences should always have a previous transition");
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