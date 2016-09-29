// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using NodaTime.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace NodaTime.TzdbCompiler.Tzdb
{
    /// <summary>
    /// Defines one "Rule" line from the tz data. (This may be applied to multiple zones.)
    /// </summary>
    /// <remarks>
    /// Immutable, threadsafe.
    /// </remarks>
    internal class RuleLine : IEquatable<RuleLine>
    {
        /// <summary>
        /// The string to replace "%s" with (if any) when formatting the zone name key.
        /// </summary>
        /// <remarks>This is always used to replace %s, whether or not the recurrence
        /// actually includes savings; it is expected to be appropriate to the recurrence.</remarks>
        private readonly string daylightSavingsIndicator;

        /// <summary>
        /// The recurrence pattern for the rule.
        /// </summary>
        private readonly ZoneRecurrence recurrence;

        /// <summary>
        /// Returns the name of the rule set this rule belongs to.
        /// </summary>
        public string Name => recurrence.Name;

        /// <summary>
        /// The "type" of the rule - usually null, meaning "applies in every year" - but can be
        /// "odd", "even" etc - usually yearistype.sh is used to determine this; Noda Time only supports
        /// "odd" and "even" (used in Australia for data up to and including 2000e).
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleLine" /> class.
        /// </summary>
        /// <param name="recurrence">The recurrence definition of this rule.</param>
        /// <param name="daylightSavingsIndicator">The daylight savings indicator letter for time zone names.</param>
        public RuleLine(ZoneRecurrence recurrence, string daylightSavingsIndicator, string type)
        {
            this.recurrence = recurrence;
            this.daylightSavingsIndicator = daylightSavingsIndicator;
            this.Type = type;
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
        public bool Equals(RuleLine other) => other != null && Equals(recurrence, other.recurrence) && Equals(daylightSavingsIndicator, other.daylightSavingsIndicator);
        #endregion

        #region Operator overloads
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(RuleLine left, RuleLine right) =>        
            ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);

        /// <summary>
        ///   Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(RuleLine left, RuleLine right) => !(left == right);
        #endregion

        /// <summary>
        /// Retrieves the recurrence, after applying the specified name format.
        /// </summary>
        /// <remarks>
        /// Multiple zones may apply the same set of rules as to when they change into/out of
        /// daylight saving time, but with different names.
        /// </remarks>
        /// <param name="zone">The zone for which this rule is being considered.</param>
        public IEnumerable<ZoneRecurrence> GetRecurrences(ZoneLine zone)
        {
            string name = zone.FormatName(recurrence.Savings, daylightSavingsIndicator);
            if (Type == null)
            {
                yield return recurrence.WithName(name);
            }
            else
            {
                Predicate<int> yearPredicate = GetYearPredicate();
                // Apply a little sanity...
                if (recurrence.IsInfinite || recurrence.ToYear - recurrence.FromYear > 1000)
                {
                    throw new NotSupportedException("Noda Time does not support 'typed' rules over large periods");
                }
                for (int year = recurrence.FromYear; year <= recurrence.ToYear; year++)
                {
                    if (yearPredicate(year))
                    {                        
                        yield return recurrence.ForSingleYear(year).WithName(name);
                    }
                }
            }
        }

        private Predicate<int> GetYearPredicate()
        {
            switch (Type)
            {
                case "odd":
                    return year => year % 2 == 1;
                case "even":
                    return year => year % 2 == 0;
                default:
                    throw new NotSupportedException($"Noda Time does not support rules of type {Type}");
            }
        }

        #region Object overrides
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => Equals(obj as RuleLine);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => HashCodeHelper.Hash(recurrence, daylightSavingsIndicator);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(recurrence);
            if (daylightSavingsIndicator != null)
            {
                builder.Append(" \"").Append(daylightSavingsIndicator).Append("\"");
            }
            return builder.ToString();
        }
        #endregion Object overrides
    }
}
