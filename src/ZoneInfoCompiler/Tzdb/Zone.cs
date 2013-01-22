using System;
using System.Globalization;
using System.Text;
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    ///   Contains the parsed information from one zone line of the TZDB zone database.
    /// </summary>
    /// <remarks>
    ///   Immutable, thread-safe
    /// </remarks>
    internal class Zone : IEquatable<Zone>
    {
        private readonly string format;
        private readonly string name;
        private readonly Offset offset;
        private readonly string rules;
        private readonly int untilYear;
        private readonly ZoneYearOffset untilYearOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="Zone" /> class.
        /// </summary>
        public Zone(string name, Offset offset, string rules, string format, int untilYear, ZoneYearOffset untilYearOffset)
        {
            this.name = name;
            this.offset = offset;
            this.rules = rules;
            this.format = format;
            this.untilYear = untilYear;
            this.untilYearOffset = untilYearOffset;
        }

        internal ZoneYearOffset UntilYearOffset { get { return untilYearOffset; } }

        internal int UntilYear { get { return untilYear; } }

        /// <summary>
        /// Returns the format for generating the label for this time zone.
        /// </summary>
        /// <value>The format string.</value>
        internal string Format { get { return format; } }

        /// <summary>
        /// Returns the name of the time zone.
        /// </summary>
        /// <value>The time zone name.</value>
        internal string Name { get { return name; } }

        /// <summary>
        /// Returns the offset to add to UTC for this time zone.
        /// </summary>
        /// <value>The offset from UTC.</value>
        internal Offset Offset { get { return offset; } }

        /// <summary>
        /// Returns the daylight savings rules name applicable to this zone line.
        /// </summary>
        /// <value>The rules name.</value>
        internal string Rules { get { return rules; } }

        #region IEquatable<Zone> Members
        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   true if the current object is equal to the <paramref name = "other" /> parameter;
        ///   otherwise, false.
        /// </returns>
        public bool Equals(Zone other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            var result = Name == other.Name && Offset == other.Offset && Rules == other.Rules && Format == other.Format && UntilYear == other.UntilYear;
            if (UntilYear != Int32.MaxValue)
            {
                result = result && UntilYearOffset.Equals(other.UntilYearOffset);
            }
            return result;
        }
        #endregion

        /// <summary>
        ///   Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance;
        ///   otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Zone);
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data
        ///   structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, Name);
            hash = HashCodeHelper.Hash(hash, Offset);
            hash = HashCodeHelper.Hash(hash, Rules);
            hash = HashCodeHelper.Hash(hash, Format);
            hash = HashCodeHelper.Hash(hash, UntilYear);
            if (UntilYear != Int32.MaxValue)
            {
                hash = HashCodeHelper.Hash(hash, UntilYearOffset.GetHashCode());
            }

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
            var builder = new StringBuilder();
            builder.Append(Name).Append(" ");
            builder.Append(Offset.ToString()).Append(" ");
            builder.Append(ParserHelper.FormatOptional(Rules)).Append(" ");
            builder.Append(Format);
            if (UntilYear != Int32.MaxValue)
            {
                builder.Append(" ").Append(UntilYear.ToString("D4", CultureInfo.InvariantCulture)).Append(" ").Append(UntilYearOffset);
            }
            return builder.ToString();
        }
    }
}
