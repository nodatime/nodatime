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
using System.Text;
using NodaTime.Utility;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    /// Contains the definition of when in a year a daylight savings adjust occurs.
    /// </summary>
    internal class DateTimeOfYear
        : IEquatable<DateTimeOfYear>
    {
        internal static readonly DateTimeOfYear StartOfYear = new DateTimeOfYear();

        /// <summary>
        /// Contains the zone character.
        /// </summary>
        private char zoneCharacter;

        /// <summary>
        /// Gets or sets the month of year the rule starts.
        /// </summary>
        /// <value>The integer month of year.</value>
        internal int MonthOfYear { get; set; }

        /// <summary>
        /// Gets or sets the day of month this rule starts.
        /// </summary>
        /// <value>The integer day of month.</value>
        internal int DayOfMonth { get; set; }

        /// <summary>
        /// Gets or sets the day of week this rule starts.
        /// </summary>
        /// <value>The integer day of week (0=Sun, 1=Mon, etc.)</value>
        internal int DayOfWeek { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [advance day of week].
        /// </summary>
        /// <value><c>true</c> if [advance day of week]; otherwise, <c>false</c>.</value>
        internal bool AdvanceDayOfWeek { get; set; }

        /// <summary>
        /// Gets or sets the millisecond of day when the rule takes effect.
        /// </summary>
        /// <value>The integer millisecond of day.</value>
        internal int MillisecondOfDay { get; set; }

        /// <summary>
        /// Gets or sets the zone character.
        /// </summary>
        /// <value>The zone character.</value>
        internal char ZoneCharacter
        {
            get
            {
                return this.zoneCharacter;
            }
            set
            {
                this.zoneCharacter = NormalizeZoneCharacter(value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeOfYear"/> class.
        /// </summary>
        internal DateTimeOfYear() 
            : this(1, 1, 0, false, 0, 'w')
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeOfYear"/> class.
        /// </summary>
        /// <param name="month">The month of the rule.</param>
        /// <param name="day">The day of the rule.</param>
        /// <param name="dayOfWeek">The day of week of the rule.</param>
        /// <param name="advance">if set to <c>true</c> [advance] of the rule.</param>
        /// <param name="millisecond">The millisecond of the rule.</param>
        /// <param name="zone">The zone character of the rule.</param>
        internal DateTimeOfYear(int month, int day, int dayOfWeek, bool advance, int millisecond, char zone)
        {
            MonthOfYear = month;
            DayOfMonth = day;
            DayOfWeek = dayOfWeek;
            AdvanceDayOfWeek = advance;
            MillisecondOfDay = millisecond;
            ZoneCharacter = zone;
        }

        /// <summary>
        /// Normalizes the zone characater.
        /// </summary>
        /// <param name="c">The character to normalize.</param>
        /// <returns>The normalized zone character.</returns>
        private static char NormalizeZoneCharacter(char c)
        {
            switch (c) {
                case 's':
                case 'S':
                    // Standard time
                    return 's';
                case 'u':
                case 'U':
                case 'g':
                case 'G':
                case 'z':
                case 'Z':
                    // UTC
                    return 'u';
                case 'w':
                case 'W':
                default:
                    // Wall time
                    return 'w';
            }
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
            builder.Append(TzdbZoneInfoParser.Months[MonthOfYear]).Append(" ");
            if (DayOfMonth == -1) {
                builder.Append("last").Append(TzdbZoneInfoParser.DaysOfWeek[DayOfWeek]).Append(" ");
            }
            else if (DayOfWeek == 0) {
                builder.Append(DayOfMonth).Append(" ");
            }
            else {
                builder.Append(TzdbZoneInfoParser.DaysOfWeek[DayOfWeek]);
                if (AdvanceDayOfWeek) {
                    builder.Append(">=");
                }
                else {
                    builder.Append("<=");
                }
                builder.Append(DayOfMonth).Append(" ");
            }
            builder.Append(ParserHelper.FormatOffset(MillisecondOfDay));
            return builder.ToString();
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
            hash = HashCodeHelper.Hash(hash, MonthOfYear);
            hash = HashCodeHelper.Hash(hash, DayOfMonth);
            hash = HashCodeHelper.Hash(hash, DayOfWeek);
            hash = HashCodeHelper.Hash(hash, AdvanceDayOfWeek);
            hash = HashCodeHelper.Hash(hash, MillisecondOfDay);
            hash = HashCodeHelper.Hash(hash, ZoneCharacter);
            return hash;
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
            return Equals((DateTimeOfYear)obj);
        }

        #region IEquatable<DateTimeOfYear> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(DateTimeOfYear other)
        {
            return
                MonthOfYear == other.MonthOfYear &&
                DayOfMonth == other.DayOfMonth &&
                DayOfWeek == other.DayOfWeek &&
                AdvanceDayOfWeek == other.AdvanceDayOfWeek &&
                MillisecondOfDay == other.MillisecondOfDay &&
                ZoneCharacter == other.ZoneCharacter;
        }

        #endregion
    }
#if qqq
        /**
         * Adds a recurring savings rule to the builder.
         */
        public void addRecurring(DateTimeZoneBuilder builder, String nameKey,
                                 int saveMillis, int fromYear, int toYear)
        {
            builder.addRecurringSavings(nameKey, saveMillis,
                                        fromYear, toYear,
                                        iZoneChar,
                                        iMonthOfYear,
                                        iDayOfMonth,
                                        iDayOfWeek,
                                        iAdvanceDayOfWeek,
                                        iMillisOfDay);
        }

        /**
         * Adds a cutover to the builder.
         */
        public void addCutover(DateTimeZoneBuilder builder, int year) {
            builder.addCutover(year,
                               iZoneChar,
                               iMonthOfYear,
                               iDayOfMonth,
                               iDayOfWeek,
                               iAdvanceDayOfWeek,
                               iMillisOfDay);
        }

        public String toString() {
            return
                "MonthOfYear: " + iMonthOfYear + "\n" +
                "DayOfMonth: " + iDayOfMonth + "\n" +
                "DayOfWeek: " + iDayOfWeek + "\n" +
                "AdvanceDayOfWeek: " + iAdvanceDayOfWeek + "\n" +
                "MillisOfDay: " + iMillisOfDay + "\n" +
                "ZoneChar: " + iZoneChar + "\n";
        }
#endif

}
