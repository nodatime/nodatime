#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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
using System.Diagnostics;
using System.Text;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    ///   Represents a range of time for which a particular Offset applies.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This type is immutable and thread-safe.
    ///   </para>
    /// </remarks>
    public class ZoneInterval : IEquatable<ZoneInterval>
    {
        private readonly Instant end;
        private readonly LocalInstant localEnd;
        private readonly LocalInstant localStart;
        private readonly string name;
        private readonly Offset offset;
        private readonly Offset savings;
        private readonly Instant start;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ZoneInterval" /> class.
        /// </summary>
        /// <param name="name">The name of this offset period (e.g. PST or PDT).</param>
        /// <param name="start">The first <see cref="Instant" /> that the <paramref name = "offset" /> applies.</param>
        /// <param name="end">The last <see cref="Instant" /> (exclusive) that the <paramref name = "offset" /> applies.</param>
        /// <param name="offset">The <see cref="Offset" /> from UTC for this period including any daylight savings.</param>
        /// <param name="savings">The <see cref="Offset" /> daylight savings contribution to the offset.</param>
        /// <exception cref="ArgumentException">If <c><paramref name = "start" /> &gt;= <paramref name = "end" /></c>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name = "name" /> parameter is null.</exception>
        public ZoneInterval(string name, Instant start, Instant end, Offset offset, Offset savings)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (start >= end)
            {
                throw new ArgumentException("The start Instant must be less than the end Instant", "start");
            }
            this.name = name;
            this.start = start;
            this.end = end;
            this.offset = offset;
            this.savings = savings;
            localStart = start == Instant.MinValue ? LocalInstant.MinValue : this.start.Plus(this.offset);
            localEnd = end == Instant.MaxValue ? LocalInstant.MaxValue : this.end.Plus(this.offset);
        }

        
        /// <summary>
        /// Returns a copy of this zone interval, but with the given start instant.
        /// </summary>
        internal ZoneInterval WithStart(Instant newStart)
        {
            return new ZoneInterval(name, newStart, end, offset, savings);
        }

        #region Properties
        /// <summary>
        ///   Gets the standard offset for this period. This is the offset without any daylight savings
        ///   contributions.
        /// </summary>
        /// <remarks>
        ///   This is effectively <c>Offset - Savings</c>.
        /// </remarks>
        /// <value>The base Offset.</value>
        public Offset StandardOffset
        {
            [DebuggerStepThrough] get { return Offset - Savings; }
        }

        /// <summary>
        ///   Gets the duration of this period.
        /// </summary>
        /// <remarks>
        ///   This is effectively <c>End - Start</c>.
        /// </remarks>
        /// <value>The Duration of this period.</value>
        public Duration Duration
        {
            [DebuggerStepThrough] get { return End - Start; }
        }

        /// <summary>
        ///   Gets the last Instant (exclusive) that the Offset applies.
        /// </summary>
        /// <value>The last Instant (exclusive) that the Offset applies.</value>
        public Instant End
        {
            [DebuggerStepThrough] get { return end; }
        }

        /// <summary>
        ///   Gets the end time as a LocalInstant.
        /// </summary>
        /// <remarks>
        ///   This is effectively <c>End + Offset</c>.
        /// </remarks>
        /// <value>The ending LocalInstant.</value>
        internal LocalInstant LocalEnd
        {
            [DebuggerStepThrough] get { return localEnd; }
        }

        /// <summary>
        ///   Gets the start time as a LocalInstant.
        /// </summary>
        /// <remarks>
        ///   This is effectively <c>Start + Offset</c>.
        /// </remarks>
        /// <value>The starting LocalInstant.</value>
        internal LocalInstant LocalStart
        {
            [DebuggerStepThrough] get { return localStart; }
        }

        /// <summary>
        /// Returns the local start time of the interval, as LocalDateTime
        /// in the ISO calendar.
        /// </summary>
        public LocalDateTime IsoLocalStart
        {
            [DebuggerStepThrough] get { return new LocalDateTime(localStart); }
        }

        /// <summary>
        /// Returns the local start time of the interval, as LocalDateTime
        /// in the ISO calendar. This does not include any daylight saving 
        /// </summary>
        public LocalDateTime IsoLocalEnd
        {
            [DebuggerStepThrough]
            get { return new LocalDateTime(localEnd); }
        }
        /// <summary>
        ///   Gets the name of this offset period (e.g. PST or PDT).
        /// </summary>
        /// <value>The name of this offset period (e.g. PST or PDT).</value>
        public string Name
        {
            [DebuggerStepThrough] get { return name; }
        }

        /// <summary>
        ///   Gets the offset from UTC for this period. This includes any daylight savings value.
        /// </summary>
        /// <value>The offset from UTC for this period.</value>
        public Offset Offset
        {
            [DebuggerStepThrough] get { return offset; }
        }

        /// <summary>
        ///   Gets the daylight savings value for this period.
        /// </summary>
        /// <value>The savings value.</value>
        public Offset Savings
        {
            [DebuggerStepThrough] get { return savings; }
        }

        /// <summary>
        ///   Gets the first Instant that the Offset applies.
        /// </summary>
        /// <value>The first Instant that the Offset applies.</value>
        public Instant Start
        {
            [DebuggerStepThrough] get { return start; }
        }
        #endregion // Properties

        #region Contains
        /// <summary>
        ///   Determines whether this period contains the given Instant in its range.
        /// </summary>
        /// <remarks>
        /// Usually this is half-open, i.e. the end is exclusive, but an interval with an end point of "the end of time" 
        /// is deemed to be inclusive at the end.
        /// </remarks>
        /// <param name="instant">The instant to test.</param>
        /// <returns>
        ///   <c>true</c> if this period contains the given Instant in its range; otherwise, <c>false</c>.
        /// </returns>
        [DebuggerStepThrough]
        public bool Contains(Instant instant)
        {
            return Start <= instant && (instant < End || End == Instant.MaxValue);
        }

        /// <summary>
        ///   Determines whether this period contains the given LocalInstant in its range.
        /// </summary>
        /// <param name="localInstant">The local instant to test.</param>
        /// <returns>
        ///   <c>true</c> if this period contains the given LocalInstant in its range; otherwise, <c>false</c>.
        /// </returns>
        [DebuggerStepThrough]
        internal bool Contains(LocalInstant localInstant)
        {
            return LocalStart <= localInstant && (localInstant < LocalEnd || End == Instant.MaxValue);
        }
        #endregion // Contains

        #region IEquatable<ZoneInterval> Members
        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///   true if the current object is equal to the <paramref name = "other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        /// </param>
        [DebuggerStepThrough]
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
            return Name == other.Name && Start == other.Start && End == other.End && Offset == other.Offset && Savings == other.Savings;
        }
        #endregion

        #region object Overrides
        /// <summary>
        ///   Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        /// <exception cref="T:System.NullReferenceException">The <paramref name = "obj" /> parameter is null.</exception>
        /// <filterpriority>2</filterpriority>
        [DebuggerStepThrough]
        public override bool Equals(object obj)
        {
            return Equals(obj as ZoneInterval);
        }

        /// <summary>
        ///   Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        ///   A hash code for the current <see cref="T:System.Object" />.
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
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append(Name);
            buffer.Append(":[");
            buffer.Append(Start);
            buffer.Append(", ");
            buffer.Append(End);
            buffer.Append(") ");
            buffer.Append(Offset);
            buffer.Append(" (");
            buffer.Append(Savings);
            buffer.Append(")");
            return buffer.ToString();
        }
        #endregion // object Overrides
        
        #region I/O
        /// <summary>
        ///   Reads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        internal static ZoneInterval Read(DateTimeZoneReader reader)
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

        /// <summary>
        ///   Writes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal void Write(DateTimeZoneWriter writer)
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
        #endregion // I/O
    }
}
