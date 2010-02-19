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
        private readonly Offset standardOffset;
        internal Offset StandardOffset { get { return standardOffset; } }
        private readonly ZoneRecurrence startRecurrence;
        internal ZoneRecurrence StartRecurrence { get { return startRecurrence; } }
        private readonly ZoneRecurrence endRecurrence;
        internal ZoneRecurrence EndRecurrence { get { return endRecurrence; } }

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
                                         ZoneRecurrence endRecurrence) : base(id, false)
        {
            this.standardOffset = standardOffset;
            this.startRecurrence = startRecurrence;
            this.endRecurrence = endRecurrence;

            if (startRecurrence.Name == endRecurrence.Name)
            {
                if (startRecurrence.Savings > Offset.Zero)
                {
                    this.startRecurrence = startRecurrence.RenameAppend("-Summer");
                }
                else
                {
                    this.endRecurrence = endRecurrence.RenameAppend("-Summer");
                }
            }
        }

        public override bool Equals(Object obj)
        {
            return Equals(obj as DaylightSavingsTimeZone);
        }

        public bool Equals(DaylightSavingsTimeZone other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return
                Id == other.Id &&
                StandardOffset == other.StandardOffset &&
                StartRecurrence.Equals(other.StartRecurrence) &&
                EndRecurrence.Equals(other.EndRecurrence);
        }

        public override int GetHashCode()
        {
            int hashCode = HashCodeHelper.Initialize();
            hashCode = HashCodeHelper.Hash(hashCode, Id);
            hashCode = HashCodeHelper.Hash(hashCode, StandardOffset);
            hashCode = HashCodeHelper.Hash(hashCode, StartRecurrence);
            hashCode = HashCodeHelper.Hash(hashCode, EndRecurrence);
            return hashCode;
        }

        private ZoneRecurrence FindMatchingRecurrence(Instant instant)
        {
            Instant? start = StartRecurrence.Next(instant, StandardOffset, EndRecurrence.Savings);
            Instant? end = EndRecurrence.Next(instant, StandardOffset, StartRecurrence.Savings);
            if (start.HasValue)
            {
                if (end.HasValue)
                {
                    return (start.Value > end.Value) ? StartRecurrence : EndRecurrence;
                }
                return StartRecurrence;
            }
            return EndRecurrence;
        }

        #region IDateTimeZone Members

        public override Instant? NextTransition(Instant instant)
        {
            Instant? start = StartRecurrence.Next(instant, StandardOffset, EndRecurrence.Savings);
            Instant? end = EndRecurrence.Next(instant, StandardOffset, StartRecurrence.Savings);
            if (start.HasValue)
            {
                if (end.HasValue)
                {
                    return (start.Value > end.Value) ? end : start;
                }
                return start;
            }
            return end;
        }

        public override Instant? PreviousTransition(Instant instant)
        {
            // Increment in order to handle the case where instant is exactly at
            // a transition.
            instant = instant + Duration.One;
            Instant? start = StartRecurrence.Previous(instant, StandardOffset, EndRecurrence.Savings);
            Instant? end = EndRecurrence.Previous(instant, StandardOffset, StartRecurrence.Savings);

            Instant? result = end;
            if (start.HasValue)
            {
                if (end.HasValue)
                {
                    result = (start > end) ? end : start;
                }
                else
                {
                    result = start;
                }
            }

            if (result.HasValue && result.Value != Instant.MinValue)
            {
                result = result.Value - Duration.One;
            }
            return result;
        }

        public override Offset GetOffsetFromUtc(Instant instant)
        {
            return FindMatchingRecurrence(instant).Savings + StandardOffset;
        }

        public override string Name(Instant instant)
        {
            return FindMatchingRecurrence(instant).Name;
        }

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

        #endregion

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