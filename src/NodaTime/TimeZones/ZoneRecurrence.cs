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
using System.Text;
using NodaTime.Calendars;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Extends <see cref="ZoneYearOffset"/> with a name and savings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This represents a recurring transition from or to a daylight savings time. The name is the
    /// name of the time zone during this period (e.g. PST or PDT). The savings is usually 0 or the
    /// daylight offset. This is also used to support some of the tricky transitions that occurred
    /// before that calendars were "standardized."
    /// </para>
    /// <para>
    /// Immutable, thread safe.
    /// </para>
    /// </remarks>
    internal class ZoneRecurrence : IEquatable<ZoneRecurrence>
    {
        private readonly int fromYear;
        private readonly string name;
        private readonly Offset savings;
        private readonly int toYear;
        private readonly ZoneYearOffset yearOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneRecurrence"/> class.
        /// </summary>
        /// <param name="name">The name of the time zone period e.g. PST.</param>
        /// <param name="savings">The savings for this period.</param>
        /// <param name="yearOffset">The year offset of when this period starts in a year.</param>
        /// <param name="fromYear"></param>
        /// <param name="toYear"></param>
        public ZoneRecurrence(String name, Offset savings, ZoneYearOffset yearOffset, int fromYear, int toYear)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (yearOffset == null)
            {
                throw new ArgumentNullException("yearOffset");
            }
            this.name = name;
            this.yearOffset = yearOffset;
            this.savings = savings;
            this.fromYear = fromYear;
            this.toYear = toYear;
        }

        public string Name { get { return name; } }

        public Offset Savings { get { return savings; } }

        public ZoneYearOffset YearOffset { get { return yearOffset; } }

        public int FromYear { get { return fromYear; } }

        public int ToYear { get { return toYear; } }

        public bool IsInfinite { get { return ToYear == Int32.MaxValue; } }

        #region IEquatable<ZoneRecurrence> Members
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(ZoneRecurrence other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return savings == other.savings && fromYear == other.fromYear && toYear == other.toYear && name == other.name && yearOffset == other.yearOffset;
        }
        #endregion

        #region Operator overloads
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ZoneRecurrence left, ZoneRecurrence right)
        {
            if (ReferenceEquals(null, left))
            {
                return ReferenceEquals(null, right);
            }
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ZoneRecurrence left, ZoneRecurrence right)
        {
            return !(left == right);
        }
        #endregion

        /// <summary>
        /// Returns the given instant adjusted one year forward taking into account leap years and other
        /// adjustments like day of week.
        /// </summary>
        /// <remarks>
        /// If the given instant is before the starting year, the year of the given instant is
        /// adjusted to the beginning of the starting year. The then first transition after the
        /// adjusted instant is determined. If the next adjustment is after the ending year the
        /// input instant is returned otherwise the next transition is returned.
        /// </remarks>
        /// <param name="instant">The <see cref="Instant"/> lower bound for the next trasnition.</param>
        /// <param name="standardOffset">The <see cref="Offset"/> standard offset.</param>
        /// <param name="previousSavings">The <see cref="Offset"/> savings adjustment at the given Instant.</param>
        /// <returns></returns>
        internal Transition? Next(Instant instant, Offset standardOffset, Offset previousSavings)
        {
            CalendarSystem calendar = CalendarSystem.Iso;

            Offset wallOffset = standardOffset + previousSavings;

            int year = instant == Instant.MinValue ? Int32.MinValue : calendar.Fields.Year.GetValue(Instant.Add(instant, wallOffset));

            if (year < fromYear)
            {
                // First advance instant to start of from year.
                instant = calendar.Fields.Year.SetValue(LocalInstant.LocalUnixEpoch, fromYear) - wallOffset;
                // Back off one tick to account for next recurrence being exactly at the beginning
                // of the year.
                instant = instant - Duration.One;
            }

            Instant next = yearOffset.Next(instant, standardOffset, previousSavings);

            if (next >= instant)
            {
                year = calendar.Fields.Year.GetValue(Instant.Add(next, wallOffset));
                if (year > toYear)
                {
                    return null;
                }
            }

            return new Transition(next, wallOffset, standardOffset + Savings);
        }

        /// <summary>
        /// Returns the given instant adjusted one year backward taking into account leap years and other
        /// adjustments like day of week.
        /// </summary>
        /// <param name="instant">The <see cref="Instant"/> lower bound for the next trasnition.</param>
        /// <param name="standardOffset">The <see cref="Offset"/> standard offset.</param>
        /// <param name="previousSavings">The <see cref="Offset"/> savings adjustment at the given Instant.</param>
        /// <returns></returns>
        internal Transition? Previous(Instant instant, Offset standardOffset, Offset previousSavings)
        {
            CalendarSystem calendar = CalendarSystem.Iso;

            Offset wallOffset = standardOffset + previousSavings;

            int year = instant == Instant.MaxValue ? Int32.MaxValue : calendar.Fields.Year.GetValue(Instant.Add(instant, wallOffset));

            if (year > toYear)
            {
                // First advance instant to start of year after toYear
                instant = calendar.Fields.Year.SetValue(LocalInstant.LocalUnixEpoch, toYear + 1) - wallOffset;
            }

            Instant previous = yearOffset.Previous(instant, standardOffset, previousSavings);

            if (previous <= instant)
            {
                year = calendar.Fields.Year.GetValue(Instant.Add(previous, wallOffset));
                if (year < fromYear)
                {
                    return null;
                }
            }

            return new Transition(previous, wallOffset, standardOffset + Savings);
        }

        /// <summary>
        /// Returns a new <see cref="ZoneRecurrence"/> with the same settings and given suffix appended
        /// to the original name. Used to created "-Summer" versions of conflicting recurrences.
        /// </summary>
        /// <param name="suffix">The suffix to append to the name key.</param>
        /// <returns>The newly created <see cref="ZoneRecurrence"/> with the new name.</returns>
        internal ZoneRecurrence RenameAppend(String suffix)
        {
            if (suffix == null)
            {
                throw new ArgumentNullException("suffix");
            }
            return new ZoneRecurrence(Name + suffix, Savings, yearOffset, fromYear, toYear);
        }

        /// <summary>
        /// Writes this object to the given <see cref="DateTimeZoneWriter"/>.
        /// </summary>
        /// <param name="writer">Where to send the output.</param>
        internal void Write(DateTimeZoneWriter writer)
        {
            writer.WriteString(Name);
            writer.WriteOffset(Savings);
            YearOffset.Write(writer);
            writer.WriteCount(fromYear);
            writer.WriteCount(toYear);
        }

        /// <summary>
        /// Reads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static ZoneRecurrence Read(DateTimeZoneReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            string name = reader.ReadString();
            Offset savings = reader.ReadOffset();
            ZoneYearOffset yearOffset = ZoneYearOffset.Read(reader);
            int fromYear = reader.ReadCount();
            int toYear = reader.ReadCount();
            return new ZoneRecurrence(name, savings, yearOffset, fromYear, toYear);
        }

        #region Object overrides
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            return Equals(obj as ZoneRecurrence);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, savings);
            hash = HashCodeHelper.Hash(hash, name);
            hash = HashCodeHelper.Hash(hash, yearOffset);
            return hash;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Name);
            builder.Append(" ").Append(Savings);
            builder.Append(" ").Append(YearOffset);
            builder.Append(" [").Append(FromYear).Append("-").Append(ToYear).Append("]");
            return builder.ToString();
        }
        #endregion // Object overrides
    }
}