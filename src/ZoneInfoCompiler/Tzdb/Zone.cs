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
using System.Globalization;
using System.Text;
using NodaTime.Fields;
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
        private readonly int dayOfMonth;
        private readonly string format;
        private readonly int monthOfYear;
        private readonly string name;
        private readonly Offset offset;
        private readonly string rules;
        private readonly Offset tickOfDay;
        private readonly int year;
        private readonly char zoneCharacter;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Zone" /> class.
        /// </summary>
        public Zone(string name, Offset offset, string rules, string format, int year, int monthOfYear, int dayOfMonth, Offset tickOfDay, char zoneCharacter)
        {
            FieldUtils.VerifyFieldValue(CalendarSystem.Iso.Fields.MonthOfYear, "monthOfYear", monthOfYear);
            FieldUtils.VerifyFieldValue(CalendarSystem.Iso.Fields.DayOfMonth, "dayOfMonth", dayOfMonth);
            FieldUtils.VerifyFieldValue(CalendarSystem.Iso.Fields.TickOfDay, "tickOfDay", tickOfDay.Ticks);
            this.name = name;
            this.offset = offset;
            this.rules = rules;
            this.format = format;
            this.year = year;
            this.monthOfYear = monthOfYear;
            this.dayOfMonth = dayOfMonth;
            this.tickOfDay = tickOfDay;
            this.zoneCharacter = zoneCharacter;
        }

        internal Zone(string name, Offset offset, string rules, string format)
            : this(name, offset, rules, format, Int32.MaxValue, 1, 1, Offset.Zero, (char)0)
        {
            
        }

        /// <summary>
        ///   Gets or sets the until day if defined.
        /// </summary>
        /// <value>The day number or 0.</value>
        internal int DayOfMonth
        {
            get { return dayOfMonth; }
        }

        /// <summary>
        ///   Gets or sets the format for generating the label for this time zone.
        /// </summary>
        /// <value>The format string.</value>
        internal string Format
        {
            get { return format; }
        }

        /// <summary>
        ///   Gets or sets the until month if defined.
        /// </summary>
        /// <value>The month or 0.</value>
        internal int MonthOfYear
        {
            get { return monthOfYear; }
        }

        /// <summary>
        ///   Gets or sets the name of the time zone
        /// </summary>
        /// <value>The time zone name.</value>
        internal string Name
        {
            get { return name; }
        }

        /// <summary>
        ///   Gets or sets the offset to add to UTC for this time zone.
        /// </summary>
        /// <value>The offset from UTC.</value>
        internal Offset Offset
        {
            get { return offset; }
        }

        /// <summary>
        ///   Gets or sets the daylight savings rules name applicable to this zone line.
        /// </summary>
        /// <value>The rules name.</value>
        internal string Rules
        {
            get { return rules; }
        }

        /// <summary>
        ///   Gets or sets the until offset time of the day if defined.
        /// </summary>
        /// <value>The offset or Offset.MinValue.</value>
        internal Offset TickOfDay
        {
            get { return tickOfDay; }
        }

        /// <summary>
        ///   Gets or sets the until year if defined.
        /// </summary>
        /// <value>The until year or 0.</value>
        internal int Year
        {
            get { return year; }
        }

        /// <summary>
        ///   Gets or sets the until zone character if defined.
        /// </summary>
        /// <value>The zone character or NUL.</value>
        internal char ZoneCharacter
        {
            get { return zoneCharacter; }
        }

        #region IEquatable<Zone> Members
        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name = "other">An object to compare with this object.</param>
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
            var result = Name == other.Name && Offset == other.Offset && Rules == other.Rules && Format == other.Format && Year == other.Year;
            if (Year != Int32.MaxValue)
            {
                result = result && MonthOfYear == other.MonthOfYear && DayOfMonth == other.DayOfMonth && TickOfDay == other.TickOfDay &&
                         ZoneCharacter == other.ZoneCharacter;
            }
            return result;
        }
        #endregion

        /// <summary>
        ///   Determines whether the specified <see cref = "System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name = "obj">The <see cref = "System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref = "System.Object" /> is equal to this instance;
        ///   otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref = "T:System.NullReferenceException">
        ///   The <paramref name = "obj" /> parameter is null.
        /// </exception>
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
            hash = HashCodeHelper.Hash(hash, Year);
            if (Year != Int32.MaxValue)
            {
                hash = HashCodeHelper.Hash(hash, MonthOfYear);
                hash = HashCodeHelper.Hash(hash, DayOfMonth);
                hash = HashCodeHelper.Hash(hash, TickOfDay);
                hash = HashCodeHelper.Hash(hash, ZoneCharacter);
            }

            return hash;
        }

        /// <summary>
        ///   Returns a <see cref = "System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref = "System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Name).Append(" ");
            builder.Append(Offset.ToString()).Append(" ");
            builder.Append(ParserHelper.FormatOptional(Rules)).Append(" ");
            builder.Append(Format);
            if (Year != Int32.MaxValue)
            {
                builder.Append(" ").Append(Year.ToString("D4", CultureInfo.InvariantCulture)).Append(" ");
                if (MonthOfYear > 0)
                {
                    builder.Append(" ").Append(TzdbZoneInfoParser.Months[MonthOfYear]);
                    if (DayOfMonth > 0)
                    {
                        builder.Append(" ").Append(DayOfMonth.ToString("D", CultureInfo.InvariantCulture)).Append(" ");
                        if (TickOfDay > Offset.Zero)
                        {
                            builder.Append(" ").Append(TickOfDay.ToString());
                            if (ZoneCharacter != 0)
                            {
                                builder.Append(ZoneCharacter);
                            }
                        }
                    }
                }
            }
            return builder.ToString();
        }
    }
}
