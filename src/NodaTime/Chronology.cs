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
using NodaTime.Calendars;
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// A chronology is a calendar system with an associated time zone, for example
    /// "the ISO calendar in the Europe/London time zone".
    /// TODO: Make this a struct? The hard work will be done in the calendar system
    /// and time zone classes.
    /// </summary>
    public sealed class Chronology : IEquatable<Chronology>
    {
        private static readonly Chronology isoUtc = new Chronology(DateTimeZones.Utc, CalendarSystem.Iso);

        public static Chronology IsoUtc { get { return isoUtc; } }

        private readonly DateTimeZone zone;
        private readonly CalendarSystem calendarSystem;

        public DateTimeZone Zone { get { return zone; } }
        public CalendarSystem Calendar { get { return calendarSystem; } }

        public Chronology(DateTimeZone zone, CalendarSystem calendarSystem)
        {
            if (zone == null)
            {
                throw new ArgumentNullException("zone");
            }
            if (calendarSystem == null)
            {
                throw new ArgumentNullException("calendarSystem");
            }
            this.zone = zone;
            this.calendarSystem = calendarSystem;
        }

        #region Equality
        public bool Equals(Chronology other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return Zone == other.Zone && Calendar == other.Calendar;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Chronology);
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, Zone);
            hash = HashCodeHelper.Hash(hash, Calendar);
            return hash;
        }
        #endregion

        #region Operators
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Chronology left, Chronology right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Chronology left, Chronology right)
        {
            return !(left == right);
        }
        #endregion
    }
}