using System;
using System.Collections.Generic;
using System.Text;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Basic <see cref="IDateTimeZone" /> implementation that represents no time zone.
    /// </summary>
    /// <remarks>
    /// This type is thread-safe and immutable.
    /// </remarks>
    public sealed class NullDateTimeZone
        : DateTimeZoneBase, IEquatable<NullDateTimeZone>
    {
        public static readonly NullDateTimeZone Instance = new NullDateTimeZone("NullTimeZone");

        /// <summary>
        /// Initializes a new instance of the <see cref="NullDateTimeZone"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        private NullDateTimeZone(string id)
            : base(id, true)
        {
        }

        /// <summary>
        /// Gets the zone offset period for the given instant. Null is returned if no period is defined by the time zone
        /// for the given instant.
        /// </summary>
        /// <param name="instant">The Instant to test.</param>
        /// <returns>The defined ZoneOffsetPeriod or <c>null</c>.</returns>
        public override ZoneOffsetPeriod GetPeriod(Instant instant)
        {
            return null;
        }

        /// <summary>
        /// Gets the zone offset period for the given local instant. Null is returned if no period is defined by the time zone
        /// for the given local instant.
        /// </summary>
        /// <param name="localInstant">The LocalInstant to test.</param>
        /// <returns>The defined ZoneOffsetPeriod or <c>null</c>.</returns>
        public override ZoneOffsetPeriod GetPeriod(LocalInstant localInstant)
        {
            return null;
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
            return Offset.Zero;
        }

        /// <summary>
        /// Returns the offset from local time to UTC, where a positive duration indicates that UTC is earlier
        /// than local time. In other words, UTC = local time - (offset from local).
        /// </summary>
        /// <param name="instant">The instant for which to calculate the offset.</param>
        /// <returns>The offset at the specified local time.</returns>
        public override Offset GetOffsetFromLocal(LocalInstant instant)
        {
            return Offset.Zero;
        }

        /// <summary>
        /// Writes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(DateTimeZoneWriter writer)
        {
        }

        /// <summary>
        /// Reads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static IDateTimeZone Read(DateTimeZoneReader reader, string id)
        {
            return new NullDateTimeZone(id);
        }

        #region Implementation of IEquatable<NullDateTimeZone>

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
        public bool Equals(NullDateTimeZone other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return IsFixed == other.IsFixed &&
                   Id == other.Id;
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
            return Equals(obj as NullDateTimeZone);
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
            hash = HashCodeHelper.Hash(hash, IsFixed);
            hash = HashCodeHelper.Hash(hash, Id);
            return hash;
        }

        #endregion // Object overrides
    }
}
