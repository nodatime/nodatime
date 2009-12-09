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
using NodaTime.Utility;
using System.Text;

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
    public class ZoneRecurrence
        : IEquatable<ZoneRecurrence>
    {
        public string Name { get { return this.name; } }
        public Offset Savings { get { return this.savings; } }
        public ZoneYearOffset YearOffset { get { return this.yearOffset; } }

        private readonly string name;
        private readonly ZoneYearOffset yearOffset;
        private readonly Offset savings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneRecurrence"/> class.
        /// </summary>
        /// <param name="name">The name of the time zone period e.g. PST.</param>
        /// <param name="savings">The savings for this period.</param>
        /// <param name="yearOffset">The year offset of when this period starts in a year.</param>
        public ZoneRecurrence(String name, Offset savings, ZoneYearOffset yearOffset)
        {
            this.yearOffset = yearOffset;
            this.name = name;
            this.savings = savings;
        }

        /// <summary>
        /// Returns the given instant adjusted one year forward taking into account leap years and other
        /// adjustments like day of week.
        /// </summary>
        /// <param name="instant">The instant to adjust.</param>
        /// <param name="standardOffset">The standard offset.</param>
        /// <param name="savingsOffset">The daylight savings adjustment.</param>
        /// <returns>The adjusted <see cref="LocalInstant"/>.</returns>
        internal Instant Next(Instant instant, Offset standardOffset, Offset savingsOffset)
        {
            return this.yearOffset.Next(instant, standardOffset, savingsOffset);
        }

        /// <summary>
        /// Returns the given instant adjusted one year backward taking into account leap years and other
        /// adjustments like day of week.
        /// </summary>
        /// <param name="instant">The instant to adjust.</param>
        /// <param name="standardOffset">The standard offset.</param>
        /// <param name="savingsOffset">The daylight savings adjustment.</param>
        /// <returns>The adjusted <see cref="LocalInstant"/>.</returns>
        internal Instant Previous(Instant instant, Offset standardOffset, Offset savingsOffset)
        {
            return this.yearOffset.Previous(instant, standardOffset, savingsOffset);
        }

        /// <summary>
        /// Returns a new <see cref="Recurrence"/> with the same settings and given suffix appended
        /// to the original name. Used to created "-Summer" versions of conflicting recurrences.
        /// </summary>
        /// <param name="appendNameKey">The append name key.</param>
        /// <returns></returns>
        internal ZoneRecurrence RenameAppend(String suffix)
        {
            return new ZoneRecurrence(Name + suffix, Savings, this.yearOffset);
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
            return new ZoneRecurrence(name, savings, yearOffset);
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
            ZoneRecurrence recurrence = obj as ZoneRecurrence;
            if (recurrence != null)
            {
                return Equals(recurrence);
            }
            return false;
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
            hash = HashCodeHelper.Hash(hash, this.savings);
            hash = HashCodeHelper.Hash(hash, this.name);
            hash = HashCodeHelper.Hash(hash, this.yearOffset);
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
            StringBuilder builder = new StringBuilder();
            builder.Append(Name);
            builder.Append(" ").Append(Savings);
            builder.Append(" ").Append(YearOffset);
            return builder.ToString();
        }

        #endregion // Object overrides

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
            if (other == null) {
                return false;
            }
            return
                this.savings == other.savings &&
                this.name == other.name &&
                this.yearOffset == other.yearOffset;
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
            if ((object)left == null || (object)right == null)
            {
                return (object)left == (object)right;
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
    }
}
