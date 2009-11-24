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
using System.Globalization;
using System.Text;
using NodaTime.Utility;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    /// Contains the parsed information from one zone line of the TZDB zone database.
    /// </summary>
    internal class Zone
        : IEquatable<Zone>
    {
        /// <summary>
        /// Gets or sets the name of the time zone
        /// </summary>
        /// <value>The time zone name.</value>
        internal string Name { get; set; }

        /// <summary>
        /// Gets or sets the offset milliseconds to add to UTC for this time zone.
        /// </summary>
        /// <value>The offset milliseconds from UTC.</value>
        internal int OffsetMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the daylight savings rules name applicable to this zone line.
        /// </summary>
        /// <value>The rules name.</value>
        internal string Rules { get; set; }

        /// <summary>
        /// Gets or sets the format for generating the label for this time zone.
        /// </summary>
        /// <value>The format string.</value>
        internal string Format { get; set; }

        /// <summary>
        /// Gets or sets the until year if defined.
        /// </summary>
        /// <value>The until year or 0.</value>
        internal int Year { get; set; }

        /// <summary>
        /// Gets or sets the until month if defined.
        /// </summary>
        /// <value>The month or 0.</value>
        internal int Month { get; set; }

        /// <summary>
        /// Gets or sets the until day if defined.
        /// </summary>
        /// <value>The day number or 0.</value>
        internal int Day { get; set; }

        /// <summary>
        /// Gets or sets the until millisecond time of the day if defined.
        /// </summary>
        /// <value>The millisecond or -1.</value>
        internal int Millisecond { get; set; }

        /// <summary>
        /// Gets or sets the until zone character if defined.
        /// </summary>
        /// <value>The zone character or NUL.</value>
        internal char ZoneCharacter { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Zone"/> class.
        /// </summary>
        internal Zone()
        {
            Millisecond = -1;
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
            builder.Append(Name).Append(" ");
            builder.Append(ParserHelper.FormatOffset(OffsetMilliseconds)).Append(" ");
            builder.Append(ParserHelper.FormatOptional(Rules)).Append(" ");
            builder.Append(Format);
            if (Year > 0) {
                builder.Append(" ").Append(Year.ToString("D4", CultureInfo.InvariantCulture)).Append(" ");
                if (Month > 0) {
                    builder.Append(" ").Append(TzdbZoneInfoParser.Months[Month]);
                    if (Day > 0) {
                        builder.Append(" ").Append(Day.ToString("D", CultureInfo.InvariantCulture)).Append(" ");
                        if (Millisecond >= 0) {
                            builder.Append(" ").Append(ParserHelper.FormatOffset(Millisecond));
                            if (ZoneCharacter != 0) {
                                builder.Append(ZoneCharacter);
                            }
                        }
                    }
                }
            }
            return builder.ToString();
        }

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
            if (obj is Zone) {
                return Equals((Zone)obj);
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
            hash = HashCodeHelper.Hash(hash, Name);
            hash = HashCodeHelper.Hash(hash, OffsetMilliseconds);
            hash = HashCodeHelper.Hash(hash, Rules);
            hash = HashCodeHelper.Hash(hash, Format);
            hash = HashCodeHelper.Hash(hash, Year);
            hash = HashCodeHelper.Hash(hash, Month);
            hash = HashCodeHelper.Hash(hash, Day);
            hash = HashCodeHelper.Hash(hash, Millisecond);
            hash = HashCodeHelper.Hash(hash, ZoneCharacter);
            return hash;
        }

        #region IEquatable<Zone> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(Zone other)
        {
            return 
                Name == other.Name &&
                OffsetMilliseconds == other.OffsetMilliseconds &&
                Rules == other.Rules &&
                Format == other.Format &&
                Year == other.Year &&
                Month == other.Month &&
                Day == other.Day &&
                Millisecond == other.Millisecond &&
                ZoneCharacter == other.ZoneCharacter;
        }

        #endregion
    }
#if woof
        void chain(StringTokenizer st)
        {
            if (iNext != null) {
                iNext.chain(st);
            }
            else {
                iNext = new Zone(iName, st);
            }
        }

        /*
        public DateTimeZone buildDateTimeZone(Map ruleSets) {
            DateTimeZoneBuilder builder = new DateTimeZoneBuilder();
            addToBuilder(builder, ruleSets);
            return builder.toDateTimeZone(iName);
        }
        */

        /**
         * Adds zone info to the builder.
         */
        public void addToBuilder(DateTimeZoneBuilder builder, Map ruleSets)
        {
            addToBuilder(this, builder, ruleSets);
        }

        private static void addToBuilder(Zone zone,
                                         DateTimeZoneBuilder builder,
                                         Map ruleSets)
        {
            for (; zone != null; zone = zone.iNext) {
                builder.setStandardOffset(zone.iOffsetMillis);

                if (zone.iRules == null) {
                    builder.setFixedSavings(zone.iFormat, 0);
                }
                else {
                    try {
                        // Check if iRules actually just refers to a savings.
                        int saveMillis = parseTime(zone.iRules);
                        builder.setFixedSavings(zone.iFormat, saveMillis);
                    }
                    catch (Exception e) {
                        RuleSet rs = (RuleSet)ruleSets.get(zone.iRules);
                        if (rs == null) {
                            throw new IllegalArgumentException
                                ("Rules not found: " + zone.iRules);
                        }
                        rs.addRecurring(builder, zone.iFormat);
                    }
                }

                if (zone.iUntilYear == Integer.MAX_VALUE) {
                    break;
                }

                zone.iUntilDateTimeOfYear.addCutover(builder, zone.iUntilYear);
            }
        }

        public String toString()
        {
            String str =
                "[Zone]\n" +
                "Name: " + iName + "\n" +
                "OffsetMillis: " + iOffsetMillis + "\n" +
                "Rules: " + iRules + "\n" +
                "Format: " + iFormat + "\n" +
                "UntilYear: " + iUntilYear + "\n" +
                iUntilDateTimeOfYear;

            if (iNext == null) {
                return str;
            }

            return str + "...\n" + iNext.toString();
        }
#endif

}
