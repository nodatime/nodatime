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
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Represents a range of time for which a particular Offset applies.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type is immutable and thread-safe.
    /// </para>
    /// </remarks>
    public class ZoneInterval
        : IEquatable<ZoneInterval>
    {
        private readonly Instant end;
        private readonly LocalInstant localEnd;
        private readonly LocalInstant localStart;
        private readonly string name;
        private readonly Offset offset;
        private readonly Offset savings;
        private readonly Instant start;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneInterval"/> class.
        /// </summary>
        /// <param name="name">The name of this offset period (e.g. PST or PDT).</param>
        /// <param name="start">The first Instant that the Offset applies.</param>
        /// <param name="end">The last Instant (exclusive) that the Offset applies.</param>
        /// <param name="offset">The offset from UTC for this period.</param>
        /// <param name="savings">The daylight savings contribution to the offset.</param>
        /// <exception cref="ArgumentNullException">If the name parameter is null.</exception>
        public ZoneInterval(string name, Instant start, Instant end, Offset offset, Offset savings)
        {
            if (ReferenceEquals(null, name))
            {
                throw new ArgumentNullException("name");
            }
            this.name = name;
            this.start = start;
            this.end = end;
            this.offset = offset;
            this.savings = savings;
            try
            {
                localStart = this.start + this.offset;
            }
            catch (OverflowException)
            {
                localStart = LocalInstant.MinValue;
            }
            try
            {
                localEnd = this.end + (this.offset - this.savings);
            }
            catch (OverflowException)
            {
                localEnd = LocalInstant.MaxValue;
            }
        }

        #region Properties
        /// <summary>
        /// Gets the first Instant that the Offset applies.
        /// </summary>
        /// <value>The first Instant that the Offset applies.</value>
        public Instant Start { get { return start; } }

        /// <summary>
        /// Gets the last Instant (exclusive) that the Offset applies.
        /// </summary>
        /// <value>The last Instant (exclusive) that the Offset applies.</value>
        public Instant End { get { return end; } }

        /// <summary>
        /// Gets the name of this offset period (e.g. PST or PDT).
        /// </summary>
        /// <value>The name of this offset period (e.g. PST or PDT).</value>
        public string Name { get { return name; } }

        /// <summary>
        /// Gets the offset from UTC for this period. This includes any daylight savings value.
        /// </summary>
        /// <value>The offset from UTC for this period.</value>
        public Offset Offset { get { return offset; } }

        /// <summary>
        /// Gets the daylight savings value for this period.
        /// </summary>
        /// <value>The savings value.</value>
        public Offset Savings { get { return savings; } }

        /// <summary>
        /// Gets the base offset for this period. This is the offset without any daylight savings
        /// contributions.
        /// </summary>
        /// <remarks>
        /// This is effectively <c>Offset - Savings</c>.
        /// </remarks>
        /// <value>The base Offset.</value>
        public Offset BaseOffset { get { return Offset - Savings; } }

        /// <summary>
        /// Gets the duration of this period.
        /// </summary>
        /// <remarks>
        /// This is effectively <c>Start - End</c>.
        /// </remarks>
        /// <value>The Duration of this period.</value>
        public Duration Duration { get { return End - Start; } }

        /// <summary>
        /// Gets the start time as a LocalInstant.
        /// </summary>
        /// <remarks>
        /// This is effectively <c>Start + Offset</c>.
        /// </remarks>
        /// <value>The starting LocalInstant.</value>
        public LocalInstant LocalStart { get { return localStart; } }

        /// <summary>
        /// Gets the end time as a LocalInstant.
        /// </summary>
        /// <remarks>
        /// This is effectively <c>End + Offset</c>.
        /// </remarks>
        /// <value>The ending LocalInstant.</value>
        public LocalInstant LocalEnd { get { return localEnd; } }
        #endregion // Properties

        #region Contains
        /// <summary>
        /// Determines whether this period contains the given Instant in its range.
        /// </summary>
        /// <param name="instant">The instant to test.</param>
        /// <returns>
        /// <c>true</c> if this period contains the given Instant in its range; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Instant instant)
        {
            return Start <= instant && instant < End;
        }

        /// <summary>
        /// Determines whether this period contains the given LocalInstant in its range.
        /// </summary>
        /// <param name="localInstant">The local instant to test.</param>
        /// <returns>
        /// <c>true</c> if this period contains the given LocalInstant in its range; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(LocalInstant localInstant)
        {
            return LocalStart <= localInstant && localInstant < LocalEnd;
        }
        #endregion // Contains

        #region IEquatable<ZoneInterval> Members
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
        public bool Equals(ZoneInterval other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Name == other.Name &&
                   Start == other.Start &&
                   End == other.End &&
                   Offset == other.Offset &&
                   Savings == other.Savings;
        }
        #endregion

        #region object Overrides
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        ///                 </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.
        ///                 </exception><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof (ZoneInterval))
            {
                return false;
            }
            return Equals((ZoneInterval) obj);
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
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, Name);
            hash = HashCodeHelper.Hash(hash, Start);
            hash = HashCodeHelper.Hash(hash, End);
            hash = HashCodeHelper.Hash(hash, Offset);
            hash = HashCodeHelper.Hash(hash, Savings);
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
            var buffer = new StringBuilder();
            buffer.Append(Name);
            buffer.Append(@":[");
            buffer.Append(Start);
            buffer.Append(@", ");
            buffer.Append(End);
            buffer.Append(@") ");
            buffer.Append(Offset);
            buffer.Append(@" (");
            buffer.Append(Savings);
            buffer.Append(@")");
            return buffer.ToString();
        }
        #endregion // object Overrides

        #region operators
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ZoneInterval left, ZoneInterval right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ZoneInterval left, ZoneInterval right)
        {
            return !Equals(left, right);
        }
        #endregion // operators

        #region I/O
        /// <summary>
        /// Writes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Write(DateTimeZoneWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            writer.WriteString(Name);
            writer.WriteInstant(Start);
            writer.WriteInstant(End);
            writer.WriteOffset(Offset);
            writer.WriteOffset(Savings);
        }

        /// <summary>
        /// Reads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static ZoneInterval Read(DateTimeZoneReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            var name = reader.ReadString();
            var start = new Instant(reader.ReadTicks());
            var end = new Instant(reader.ReadTicks());
            var offset = reader.ReadOffset();
            var savings = reader.ReadOffset();
            return new ZoneInterval(name, start, end, offset, savings);
        }
        #endregion // I/O
    }
}