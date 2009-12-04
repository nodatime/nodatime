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
using NodaTime.Calendars;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Defines one time zone rule with a validitity range.
    /// </summary>
    /// <remarks>
    /// Immutable, threadsafe.
    /// </remarks>
    public class ZoneRule
        : IEquatable<ZoneRule>
    {
        public ZoneRecurrence Recurrence { get { return this.recurrence; } }
        public string Name { get { return Recurrence.Name; } }
        public Offset Savings { get { return Recurrence.Savings; } }
        public bool IsInfinite { get { return this.toYear == Int32.MaxValue; } }
        public int FromYear { get { return this.fromYear; } }
        public int ToYear { get { return this.toYear; } }
        public string LetterS { get { return this.letterS; } }

        private readonly ZoneRecurrence recurrence;
        private readonly int fromYear;
        private readonly int toYear;
        private readonly string letterS;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneRule"/> class.
        /// </summary>
        /// <param name="recurrence">The recurrence definition of this rule.</param>
        /// <param name="fromYear">The inclusive starting year for this rule.</param>
        /// <param name="toYear">The inclusive ending year for this rule.</param>
        public ZoneRule(ZoneRecurrence recurrence, int fromYear, int toYear)
            : this(recurrence, fromYear, toYear, null)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneRule"/> class.
        /// </summary>
        /// <param name="recurrence">The recurrence definition of this rule.</param>
        /// <param name="fromYear">The inclusive starting year for this rule.</param>
        /// <param name="toYear">The inclusive ending year for this rule.</param>
        public ZoneRule(ZoneRecurrence recurrence, int fromYear, int toYear, string letterS)
        {
            this.recurrence = recurrence;
            this.fromYear = fromYear;
            this.toYear = toYear;
            this.letterS = letterS;
        }

        /// <summary>
        /// Formats the name.
        /// </summary>
        /// <param name="nameFormat">The name format.</param>
        /// <returns></returns>
        public String FormatName(String nameFormat)
        {
            int index = nameFormat.IndexOf('/');
            if (index > 0)
            {
                if (Savings == Offset.Zero)
                {
                    // Extract standard name.
                    return nameFormat.Substring(0, index);
                }
                else
                {
                    return nameFormat.Substring(index + 1);
                }
            }
            index = nameFormat.IndexOf("%s");
            if (index < 0)
            {
                return nameFormat;
            }
            string left = nameFormat.Substring(0, index);
            string right = nameFormat.Substring(index + 2);
            string name;
            if (LetterS == null)
            {
                name = left + right;
            }
            else
            {
                name = left + LetterS + right;
            }
            return name;
        }

        /// <summary>
        /// Returns the next transition instant as defined by the recurrence definition of this rule
        /// within the year boundaries of the rule.
        /// </summary>
        /// <remarks>
        /// If the given instant is before the starting year, the year of the given instant is
        /// adjusted to the beginning of the starting year. The then first transition after the
        /// adjusted instant is determined. If the next adjustment is after the ending year the
        /// input instant is returned otherwise the next transition is returned.
        /// </remarks>
        /// <param name="instant">The <see cref="Instant"/> lower bound for the next trasnition.</param>
        /// <param name="standardOffset">The <see cref="Duration"/> standard offset.</param>
        /// <param name="savings">The <see cref="Duration"/> savings adjustment.</param>
        /// <returns></returns>
        internal Instant Next(Instant instant, Offset standardOffset, Offset savings)
        {
            ICalendarSystem calendar = IsoCalendarSystem.Instance;

            Offset wallOffset = standardOffset + savings;
            Instant adjustedInstant = instant;

            int year;
            if (instant == Instant.MinValue)
            {
                year = Int32.MinValue;
            }
            else
            {
                year = calendar.Fields.Year.GetValue(instant + wallOffset);
            }

            if (year < this.fromYear)
            {
                // First advance instant to start of from year.
                adjustedInstant = calendar.Fields.Year.SetValue(LocalInstant.LocalUnixEpoch, this.fromYear) - wallOffset;
                // Back off one tick to account for next recurrence being exactly at the beginning
                // of the year.
                adjustedInstant = adjustedInstant - Duration.One;
            }

            Instant next = Recurrence.Next(adjustedInstant, standardOffset, savings);

            if (next > instant)
            {
                year = calendar.Fields.Year.GetValue(next + wallOffset);
                if (year > this.toYear)
                {
                    // Out of range, return original value.
                    next = instant;
                }
            }

            return next;
        }

        #region Object overrides

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
            if (obj is ZoneRule)
            {
                return Equals((ZoneRule)obj);
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
            hash = HashCodeHelper.Hash(hash, this.fromYear);
            hash = HashCodeHelper.Hash(hash, this.toYear);
            hash = HashCodeHelper.Hash(hash, this.recurrence);
            return hash;
        }

        #endregion Object overrides

        #region IEquatable<ZoneRule> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(ZoneRule other)
        {
            if (other == null)
            {
                return false;
            }
            return
                this.fromYear == other.fromYear &&
                this.toYear == other.toYear &&
                this.recurrence == other.recurrence;
        }

        #endregion
    }
}
