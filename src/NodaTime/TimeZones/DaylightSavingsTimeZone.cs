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
    ///    Provides a basic daylight savings time zone. A DST time zone has a simple recurrence
    ///    where an extra offset is applied between two dates of a year.
    /// </summary>
    internal class DaylightSavingsTimeZone
        : DateTimeZoneBase, IEquatable<DaylightSavingsTimeZone>
    {
        private readonly ZoneRecurrence endRecurrence;
        private readonly Offset standardOffset;
        private readonly ZoneRecurrence startRecurrence;

        /// <summary>
        ///    Initializes a new instance of the
        ///    <see cref="DaylightSavingsTimeZone"/>
        ///    class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="standardOffset">The standard offset.</param>
        /// <param name="startRecurrence">
        ///    The start recurrence.
        /// </param>
        /// <param name="endRecurrence">The end recurrence.</param>
        internal DaylightSavingsTimeZone(String id, Offset standardOffset, ZoneRecurrence startRecurrence,
                                         ZoneRecurrence endRecurrence)
            : base(id, false)
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
            var start = startRecurrence;
            var end = endRecurrence;
            if (startRecurrence.Savings == Offset.Zero)
            {
                start = endRecurrence;
                end = startRecurrence;
            }
            if (start.Name == end.Name)
            {
                start = start.RenameAppend("-Summer");
            }
            this.startRecurrence = start;
            this.endRecurrence = end;
        }

        /// <summary>
        /// Gets the standard offset.
        /// </summary>
        /// <value>The standard offset.</value>
        private Offset StandardOffset { get { return standardOffset; } }

        /// <summary>
        /// Gets the start recurrence.
        /// </summary>
        /// <value>The start recurrence.</value>
        private ZoneRecurrence StartRecurrence { get { return startRecurrence; } }

        /// <summary>
        /// Gets the end recurrence.
        /// </summary>
        /// <value>The end recurrence.</value>
        private ZoneRecurrence EndRecurrence { get { return endRecurrence; } }

        #region IEquatable<DaylightSavingsTimeZone> Members
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
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
            return
                Id == other.Id &&
                StandardOffset == other.StandardOffset &&
                StartRecurrence.Equals(other.StartRecurrence) &&
                EndRecurrence.Equals(other.EndRecurrence);
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
            hashCode = HashCodeHelper.Hash(hashCode, StandardOffset);
            hashCode = HashCodeHelper.Hash(hashCode, StartRecurrence);
            hashCode = HashCodeHelper.Hash(hashCode, EndRecurrence);
            return hashCode;
        }
        #endregion // Object overrides

        /// <summary>
        /// Gets the zone offset period for the given instant. Null is returned if no period is defined by the time zone
        /// for the given instant.
        /// </summary>
        /// <param name="instant">The Instant to test.</param>
        /// <returns>The defined ZoneOffsetPeriod or <c>null</c>.</returns>
        public override ZoneInterval GetZoneInterval(Instant instant)
        {
            var previous = PreviousTransition(instant + Duration.One);
            var next = NextTransition(instant);
            // If we are outside of the value range of this TZ return null
            if (!previous.HasValue || !next.HasValue)
            {
                return null;
            }
            var recurrence = FindMatchingRecurrence(instant);
            return new ZoneInterval(recurrence.Name, previous.Value.Instant, next.Value.Instant,
                                    StandardOffset + recurrence.Savings, recurrence.Savings);
        }

        /// <summary>
        /// Gets the zone offset period for the given local instant. Null is returned if no period is defined by the time zone
        /// for the given local instant.
        /// </summary>
        /// <param name="localInstant">The LocalInstant to test.</param>
        /// <exception cref="SkippedTimeException"></exception>
        /// <returns>The defined ZoneOffsetPeriod or <c>null</c>.</returns>
        public override ZoneInterval GetZoneInterval(LocalInstant localInstant)
        {
            var normal = localInstant - StandardOffset;
            var daylight = localInstant - (StandardOffset + startRecurrence.Savings);
            var normalRecurrence = FindMatchingRecurrence(normal);
            var daylightRecurrence = FindMatchingRecurrence(daylight);

            if (ReferenceEquals(normalRecurrence, startRecurrence) &&
                ReferenceEquals(daylightRecurrence, endRecurrence))
            {
                throw new SkippedTimeException(localInstant, this);
            }
            return GetZoneInterval(normal);
        }

        /// <summary>
        /// Finds the matching recurrence.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <returns></returns>
        private ZoneRecurrence FindMatchingRecurrence(Instant instant)
        {
            // Find the transitions which start *after* the one we're currently in - then
            // pick the later of them, which will be the same "polarity" as the one we're currently
            // in.
            Transition? start = StartRecurrence.Next(instant, StandardOffset, EndRecurrence.Savings);
            Transition? end = EndRecurrence.Next(instant, StandardOffset, StartRecurrence.Savings);
            if (start.HasValue)
            {
                if (end.HasValue)
                {
                    return (start.Value.Instant > end.Value.Instant) ? StartRecurrence : EndRecurrence;
                }
                return StartRecurrence;
            }
            return EndRecurrence;
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
            Transition? start = StartRecurrence.Next(instant, StandardOffset, EndRecurrence.Savings);
            Transition? end = EndRecurrence.Next(instant, StandardOffset, StartRecurrence.Savings);
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
            Transition? start = StartRecurrence.Previous(instant, StandardOffset, EndRecurrence.Savings);
            Transition? end = EndRecurrence.Previous(instant, StandardOffset, StartRecurrence.Savings);

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
            return FindMatchingRecurrence(instant).Savings + StandardOffset;
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
        public override void Write(DateTimeZoneWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            writer.WriteOffset(StandardOffset);
            StartRecurrence.Write(writer);
            EndRecurrence.Write(writer);
        }

        public static IDateTimeZone Read(DateTimeZoneReader reader, string id)
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