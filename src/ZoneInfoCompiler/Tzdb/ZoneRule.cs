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
using System.Text;
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    ///   Defines one time zone rule with a validitity range.
    /// </summary>
    /// <remarks>
    ///   Immutable, threadsafe.
    /// </remarks>
    internal class ZoneRule : IEquatable<ZoneRule>
    {
        private readonly string letterS;
        private readonly ZoneRecurrence recurrence;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ZoneRule" /> class.
        /// </summary>
        /// <param name="recurrence">The recurrence definition of this rule.</param>
        /// <param name="letterS">The daylight savings indicator letter for time zone names.</param>
        public ZoneRule(ZoneRecurrence recurrence, string letterS)
        {
            this.recurrence = recurrence;
            this.letterS = letterS;
        }

        public string LetterS
        {
            get { return letterS; }
        }

        public string Name
        {
            get { return Recurrence.Name; }
        }

        public ZoneRecurrence Recurrence
        {
            get { return recurrence; }
        }

        #region IEquatable<ZoneRule> Members
        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   true if the current object is equal to the <paramref name = "other" /> parameter;
        ///   otherwise, false.
        /// </returns>
        public bool Equals(ZoneRule other)
        {
            if (other == null)
            {
                return false;
            }
            return recurrence == other.recurrence && Equals(LetterS, other.LetterS);
        }
        #endregion

        #region Operator overloads
        /// <summary>
        ///   Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ZoneRule left, ZoneRule right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
        }

        /// <summary>
        ///   Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ZoneRule left, ZoneRule right)
        {
            return !(left == right);
        }
        #endregion

        /// <summary>
        ///   Formats the name.
        /// </summary>
        /// <param name="nameFormat">The name format.</param>
        /// <returns></returns>
        public String FormatName(String nameFormat)
        {
            if (nameFormat == null)
            {
                throw new ArgumentNullException("nameFormat");
            }
            int index = nameFormat.IndexOf("/", StringComparison.Ordinal);
            if (index > 0)
            {
                return Recurrence.Savings == Offset.Zero ? nameFormat.Substring(0, index) : nameFormat.Substring(index + 1);
            }
            index = nameFormat.IndexOf("%s", StringComparison.Ordinal);
            if (index < 0)
            {
                return nameFormat;
            }
            var left = nameFormat.Substring(0, index);
            var right = nameFormat.Substring(index + 2);
            return LetterS == null ? left + right : left + LetterS + right;
        }

        #region Object overrides
        /// <summary>
        ///   Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance;
        ///   otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        ///   The <paramref name = "obj" /> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            var rule = obj as ZoneRule;
            return rule != null && Equals(rule);
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
            hash = HashCodeHelper.Hash(hash, Recurrence);
            hash = HashCodeHelper.Hash(hash, LetterS);
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
            builder.Append(Recurrence);
            if (LetterS != null)
            {
                builder.Append(" \"").Append(LetterS).Append("\"");
            }
            return builder.ToString();
        }
        #endregion Object overrides
    }
}
