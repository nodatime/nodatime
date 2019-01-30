// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Text;
using NodaTime.TimeZones.IO;
using NodaTime.Utility;
using System;

namespace NodaTime.TimeZones
{
    // Implementation note: this implemented IEquatable<FixedDateTimeZone> for the sake of fitting in with our test infrastructure
    // more than anything else...

    /// <summary>
    /// Basic <see cref="DateTimeZone" /> implementation that has a fixed name key and offset i.e.
    /// no daylight savings.
    /// </summary>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    internal sealed class FixedDateTimeZone : DateTimeZone, IEquatable<FixedDateTimeZone?>
    {
        private readonly ZoneInterval interval;

        /// <summary>
        /// Creates a new fixed time zone.
        /// </summary>
        /// <remarks>The ID and name (for the <see cref="ZoneInterval"/>) are generated based on the offset.</remarks>
        /// <param name="offset">The <see cref="Offset"/> from UTC.</param>
        internal FixedDateTimeZone(Offset offset) : this(MakeId(offset), offset)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedDateTimeZone"/> class.
        /// </summary>
        /// <remarks>The name (for the <see cref="ZoneInterval"/>) is deemed to be the same as the ID.</remarks>
        /// <param name="id">The id.</param>
        /// <param name="offset">The offset.</param>
        internal FixedDateTimeZone([NotNull] string id, Offset offset) : this(id, offset, id)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedDateTimeZone"/> class.
        /// </summary>
        /// <remarks>The name (for the <see cref="ZoneInterval"/>) is deemed to be the same as the ID.</remarks>
        /// <param name="id">The id.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="name">The name to use in the sole <see cref="ZoneInterval"/> in this zone.</param>
        internal FixedDateTimeZone([NotNull] string id, Offset offset, [NotNull] string name) : base(id, true, offset, offset)
        {
            interval = new ZoneInterval(name, Instant.BeforeMinValue, Instant.AfterMaxValue, offset, Offset.Zero);
        }

        /// <summary>
        /// Makes the id for this time zone. The format is "UTC+/-Offset".
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>The generated id string.</returns>
        private static string MakeId(Offset offset)
        {
            if (offset == Offset.Zero)
            {
                return UtcId;
            }
            return UtcId + OffsetPattern.GeneralInvariant.Format(offset);
        }

        /// <summary>
        /// Returns a fixed time zone for the given ID, which must be "UTC" or "UTC[offset]" where "[offset]" can be parsed
        /// using the "general" offset pattern.
        /// </summary>
        /// <param name="id">ID </param>
        /// <returns>The parsed time zone, or null if the ID doesn't match.</returns>
        internal static DateTimeZone? GetFixedZoneOrNull(string id)
        {
            if (!id.StartsWith(UtcId))
            {
                return null;
            }
            if (id == UtcId)
            {
                return Utc;
            }
            var parseResult = OffsetPattern.GeneralInvariant.Parse(id.Substring(UtcId.Length));
            return parseResult.Success ? ForOffset(parseResult.Value) : null;
        }

        /// <summary>
        /// Returns the fixed offset for this time zone.
        /// </summary>
        /// <returns>The fixed offset for this time zone.</returns>
        public Offset Offset => MaxOffset;

        /// <summary>
        /// Returns the name used for the zone interval for this time zone.
        /// </summary>
        /// <returns>The name used for the zone interval for this time zone.</returns>
        public string Name => interval.Name;

        /// <summary>
        /// Gets the zone interval for the given instant. This implementation always returns the same interval.
        /// </summary>
        public override ZoneInterval GetZoneInterval(Instant instant) => interval;

        /// <summary>
        /// Override for efficiency: we know we'll always have an unambiguous mapping for any LocalDateTime.
        /// </summary>
        public override ZoneLocalMapping MapLocal(LocalDateTime localDateTime) =>
            new ZoneLocalMapping(this, localDateTime, interval, interval, 1);

        /// <summary>
        /// Returns the offset from UTC, where a positive duration indicates that local time is later
        /// than UTC. In other words, local time = UTC + offset.
        /// </summary>
        /// <param name="instant">The instant for which to calculate the offset.</param>
        /// <returns>
        /// The offset from UTC at the specified instant.
        /// </returns>
        public override Offset GetUtcOffset(Instant instant) => MaxOffset;

        /// <summary>
        /// Writes the time zone to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal void Write([NotNull] IDateTimeZoneWriter writer)
        {
            Preconditions.CheckNotNull(writer, nameof(writer));
            writer.WriteOffset(Offset);
            writer.WriteString(Name);
        }

        /// <summary>
        /// Reads a fixed time zone from the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="id">The id.</param>
        /// <returns>The fixed time zone.</returns>
        public static DateTimeZone Read([NotNull] IDateTimeZoneReader reader, [NotNull] string id)
        {
            Preconditions.CheckNotNull(reader, nameof(reader));
            Preconditions.CheckNotNull(id, nameof(id));
            var offset = reader.ReadOffset();
            var name = reader.HasMoreData ? reader.ReadString() : id;
            return new FixedDateTimeZone(id, offset, name);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to.</param> 
        /// <filterpriority>2</filterpriority>
        /// <returns>True if the specified value is a <see cref="FixedDateTimeZone"/> with the same name, ID and offset; otherwise, false.</returns>
        public override bool Equals(object? obj) => Equals(obj as FixedDateTimeZone);

        public bool Equals(FixedDateTimeZone? other) =>
            other != null &&
            Offset == other.Offset &&
            Id == other.Id &&
            Name == other.Name;

        /// <summary>
        /// Computes the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode() => HashCodeHelper.Hash(Offset, Id, Name);

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() => Id;
    }
}