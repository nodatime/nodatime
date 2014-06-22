// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using JetBrains.Annotations;
using NodaTime.Text;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// An interval between two instants in time (start and end).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The interval includes the start instant and excludes the end instant, unless the end instant
    /// is <see cref="Instant.MaxValue"/> in which case it's deemed to be inclusive.
    /// (An interval stretching to infinity includes the end of time.)
    /// </para>
    /// <para>
    /// The end may equal the start (resulting in an empty interval), but will not be before the start.
    /// </para>
    /// </remarks>
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
#if !PCL
    [Serializable]
#endif
    public struct Interval : IEquatable<Interval>, IXmlSerializable
#if !PCL
        , ISerializable
#endif
    {
        /// <summary>The start of the interval.</summary>
        private readonly Instant start;

        /// <summary>The end of the interval. This will never be earlier than the start.</summary>
        private readonly Instant end;

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval"/> struct.
        /// The interval includes the <paramref name="start"/> instant and excludes the
        /// <paramref name="end"/> instant. The end may equal the start (resulting in an empty interval), but must not be before the start.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="end"/> is earlier than <paramref name="start"/>.</exception>
        /// <param name="start">The start <see cref="Instant"/>.</param>
        /// <param name="end">The end <see cref="Instant"/>.</param>
        public Interval(Instant start, Instant end)
        {
            if (end < start)
            {
                throw new ArgumentOutOfRangeException("end", "The end parameter must be equal to or later than the start parameter");
            }
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Gets the start instant - the inclusive lower bound of the interval.
        /// </summary>
        /// <remarks>
        /// This will never be later than <see cref="End"/>, though it may be equal to it.
        /// </remarks>
        /// <value>The start <see cref="Instant"/>.</value>
        public Instant Start { get { return start; } }

        /// <summary>
        /// Gets the end instant - the exclusive upper bound of the interval.
        /// </summary>
        /// <remarks>
        /// This will never be earlier than <see cref="Start"/>, though it may be equal to it.
        /// If this value is <see cref="Instant.MaxValue"/>, it is treated as an inclusive
        /// upper bound: an interval stretching to infinity includes the end of time.
        /// </remarks>
        /// <value>The end <see cref="Instant"/>.</value>
        public Instant End { get { return end; } }

        /// <summary>
        /// Returns the duration of the interval.
        /// </summary>
        /// <remarks>
        /// This will always be a non-negative duration, though it may be zero.
        /// </remarks>
        /// <value>The duration of the interval.</value>
        public Duration Duration { get { return end - start; } }

        /// <summary>
        /// Returns whether or not this interval contains the given instant.
        /// </summary>
        /// <remarks>
        /// The interval is considered to include the <see cref="Start"/> instant but
        /// not the <see cref="End"/> instant - unless the end is <see cref="Instant.MaxValue"/>, in
        /// which case it's considered to be infinite from the start point onwards.
        /// </remarks>
        /// <param name="instant">Instant to test.</param>
        /// <returns>True if this interval contains the given instant; false otherwise.</returns>
        [Pure]
        public bool Contains(Instant instant)
        {
            return instant >= start && (instant < end || end == Instant.MaxValue);
        }

        #region Implementation of IEquatable<Interval>
        /// <summary>
        /// Indicates whether the value of this interval is equal to the value of the specified interval.
        /// </summary>
        /// <param name="other">The value to compare with this instance.</param>
        /// <returns>
        /// true if the value of this instant is equal to the value of the <paramref name="other" /> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(Interval other)
        {
            return Start == other.Start && End == other.End;
        }
        #endregion

        #region object overrides

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Interval)
            {
                return Equals((Interval)obj);
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, Start);
            hash = HashCodeHelper.Hash(hash, End);
            return hash;
        }

        /// <summary>
        /// Returns a string representation of this interval, in extended ISO-8601 format: the format
        /// is "start/end" where each instant uses a format of "yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFF'Z'".
        /// </summary>
        /// <returns>A string representation of this interval.</returns>
        public override string ToString()
        {
            var pattern = InstantPattern.ExtendedIsoPattern;
            return pattern.Format(Start) + "/" + pattern.Format(End);
        }
        #endregion

        #region Operators
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Interval left, Interval right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Interval left, Interval right)
        {
            return !(left == right);
        }
        #endregion

        #region XML serialization
        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Preconditions.CheckNotNull(reader, "reader");
            var pattern = InstantPattern.ExtendedIsoPattern;
            if (!reader.MoveToAttribute("start"))
            {
                throw new ArgumentException("No start specified in XML for Interval");
            }
            Instant newStart = pattern.Parse(reader.Value).Value;
            if (!reader.MoveToAttribute("end"))
            {
                throw new ArgumentException("No end specified in XML for Interval");
            }
            Instant newEnd = pattern.Parse(reader.Value).Value;
            this = new Interval(newStart, newEnd);
            // Consume the rest of this element, as per IXmlSerializable.ReadXml contract.
            reader.Skip();
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Preconditions.CheckNotNull(writer, "writer");
            var pattern = InstantPattern.ExtendedIsoPattern;
            writer.WriteAttributeString("start", pattern.Format(start));
            writer.WriteAttributeString("end", pattern.Format(end));
        }
        #endregion

#if !PCL
        #region Binary serialization
        private const string StartTicksSerializationName = "start";
        private const string EndTicksSerializationName = "end";

        /// <summary>
        /// Private constructor only present for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to fetch data from.</param>
        /// <param name="context">The source for this deserialization.</param>
        private Interval(SerializationInfo info, StreamingContext context)
            : this(Instant.FromTicksSinceUnixEpoch(info.GetInt64(StartTicksSerializationName)),
                   Instant.FromTicksSinceUnixEpoch(info.GetInt64(EndTicksSerializationName)))
        {
        }

        /// <summary>
        /// Implementation of <see cref="ISerializable.GetObjectData"/>.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination for this serialization.</param>
        [System.Security.SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(StartTicksSerializationName, start.Ticks);
            info.AddValue(EndTicksSerializationName, end.Ticks);
        }
        #endregion
#endif
    }
}
