// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Globalization;
using NodaTime.Text;
using NodaTime.TimeZones.IO;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Basic <see cref="DateTimeZone" /> implementation that has a fixed name key and offset i.e.
    /// no daylight savings.
    /// </summary>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    internal sealed class FixedDateTimeZone : DateTimeZone
    {
        private readonly Offset offset;
        private readonly ZoneInterval interval;

        /// <summary>
        /// Creates a new fixed time zone.
        /// </summary>
        /// <param name="offset">The <see cref="Offset"/> from UTC.</param>
        public FixedDateTimeZone(Offset offset) : this(MakeId(offset), offset)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedDateTimeZone"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="offset">The offset.</param>
        public FixedDateTimeZone(string id, Offset offset) : base(id, true, offset, offset)
        {
            this.offset = offset;
            interval = new ZoneInterval(id, Instant.BeforeMinValue, Instant.AfterMaxValue, offset, Offset.Zero);
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
            return string.Format(CultureInfo.InvariantCulture,
                "{0}{1}", UtcId, OffsetPattern.GeneralInvariantPattern.Format(offset));
        }

        /// <summary>
        /// Returns a fixed time zone for the given ID, which must be "UTC" or "UTC[offset]" where "[offset]" can be parsed
        /// using the "general" offset pattern.
        /// </summary>
        /// <param name="id">ID </param>
        /// <returns>The parsed time zone, or null if the ID doesn't match.</returns>
        internal static DateTimeZone GetFixedZoneOrNull(string id)
        {
            if (!id.StartsWith(UtcId))
            {
                return null;
            }
            if (id == UtcId)
            {
                return Utc;
            }
            var parseResult = OffsetPattern.GeneralInvariantPattern.Parse(id.Substring(UtcId.Length));
            return parseResult.Success ? ForOffset(parseResult.Value) : null;
        }

        /// <summary>
        /// Returns the fixed offset for this time zone.
        /// </summary>
        /// <returns>The fixed offset for this time zone.</returns>
        public Offset Offset { get { return offset; } }

        /// <summary>
        /// Gets the zone interval for the given instant. This implementation always returns the same interval.
        /// </summary>
        public override ZoneInterval GetZoneInterval(Instant instant)
        {
            return interval;
        }

        /// <summary>
        /// Override for efficiency: we know we'll always have an unambiguous mapping for any LocalDateTime.
        /// </summary>
        public override ZoneLocalMapping MapLocal(LocalDateTime localDateTime)
        {
            return new ZoneLocalMapping(this, localDateTime, interval, interval, 1);
        }

        /// <summary>
        /// Returns the offset from UTC, where a positive duration indicates that local time is later
        /// than UTC. In other words, local time = UTC + offset.
        /// </summary>
        /// <param name="instant">The instant for which to calculate the offset.</param>
        /// <returns>
        /// The offset from UTC at the specified instant.
        /// </returns>
        public override Offset GetUtcOffset(Instant instant)
        {
            return offset;
        }

        /// <summary>
        /// Writes the time zone to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal void Write(IDateTimeZoneWriter writer)
        {
            Preconditions.CheckNotNull(writer, "writer");
            writer.WriteOffset(offset);
        }

        /// <summary>
        /// Reads a fixed time zone from the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="id">The id.</param>
        /// <returns>The fixed time zone.</returns>
        public static DateTimeZone Read(IDateTimeZoneReader reader, string id)
        {
            Preconditions.CheckNotNull(reader, "reader");
            Preconditions.CheckNotNull(reader, "id");
            var offset = reader.ReadOffset();
            return new FixedDateTimeZone(id, offset);
        }


        protected override bool EqualsImpl(DateTimeZone other)
        {
            FixedDateTimeZone otherZone = (FixedDateTimeZone) other;
            return offset == otherZone.offset && Id == other.Id;
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, offset);
            hash = HashCodeHelper.Hash(hash, Id);
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
            return Id;
        }
    }
}