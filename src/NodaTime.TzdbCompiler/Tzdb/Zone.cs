// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using System.Text;
using NodaTime.TimeZones;
using NodaTime.Utility;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.TzdbCompiler.Tzdb
{
    /// <summary>
    /// Contains the parsed information from one zone line of the TZDB zone database.
    /// </summary>
    /// <remarks>
    /// Immutable, thread-safe
    /// </remarks>
    internal class Zone : IEquatable<Zone>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Zone" /> class.
        /// </summary>
        public Zone(string name, Offset offset, string rules, string format, int untilYear, ZoneYearOffset untilYearOffset)
        {
            this.Name = name;
            this.StandardOffset = offset;
            this.Rules = rules;
            this.Format = format;
            this.UntilYear = untilYear;
            this.UntilYearOffset = untilYearOffset;
        }

        internal ZoneYearOffset UntilYearOffset { get; }

        internal int UntilYear { get; }

        /// <summary>
        /// Returns the format for generating the label for this time zone.
        /// </summary>
        /// <value>The format string.</value>
        internal string Format { get; }

        /// <summary>
        /// Returns the name of the time zone.
        /// </summary>
        /// <value>The time zone name.</value>
        internal string Name { get; }

        /// <summary>
        /// Returns the offset to add to UTC for this time zone's standard time.
        /// </summary>
        /// <value>The offset from UTC.</value>
        internal Offset StandardOffset { get; }

        /// <summary>
        /// The name of the set of rules applicable to this zone line, or
        /// null for just standard time, or an offset for a "fixed savings" rule.
        /// </summary>
        /// <value>The name of the rules to apply for this zone line.</value>
        internal string Rules { get; }

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
            var result = Name == other.Name && StandardOffset == other.StandardOffset && Rules == other.Rules && Format == other.Format && UntilYear == other.UntilYear;
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
        public override bool Equals(object obj) => Equals(obj as Zone);

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data
        ///   structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var hash = HashCodeHelper.Initialize()
                .Hash(Name)
                .Hash(StandardOffset)
                .Hash(Rules)
                .Hash(Format)
                .Hash(UntilYear);
            if (UntilYear != Int32.MaxValue)
            {
                hash = hash.Hash(UntilYearOffset.GetHashCode());
            }

            return hash.Value;
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
            builder.Append(StandardOffset).Append(" ");
            builder.Append(ParserHelper.FormatOptional(Rules)).Append(" ");
            builder.Append(Format);
            if (UntilYear != Int32.MaxValue)
            {
                builder.Append(" ").Append(UntilYear.ToString("D4", CultureInfo.InvariantCulture)).Append(" ").Append(UntilYearOffset);
            }
            return builder.ToString();
        }

        public ZoneRuleSet ResolveRules(IDictionary<string, IList<ZoneRule>> allRules)
        {
            if (Rules == null)
            {
                return new ZoneRuleSet(Format, StandardOffset, Offset.Zero, UntilYear, UntilYearOffset);
            }
            IList<ZoneRule> ruleSet;
            if (allRules.TryGetValue(Rules, out ruleSet))
            {
                var rules = ruleSet.SelectMany(x => x.GetRecurrences(Format));
                return new ZoneRuleSet(rules.ToList(), StandardOffset, UntilYear, UntilYearOffset);
            }
            else
            {
                try
                {
                    // Check if Rules actually just refers to a savings.
                    var savings = ParserHelper.ParseOffset(Rules);
                    return new ZoneRuleSet(Format, StandardOffset, savings, UntilYear, UntilYearOffset);
                }
                catch (FormatException)
                {
                    throw new ArgumentException(
                        $"Daylight savings rule name '{Rules}' for zone {Name} is neither a known ruleset nor a fixed offset");
                }
            }
        }
    }
}
