// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime.TzdbCompiler.Tzdb
{
    /// <summary>
    /// A rule set associated with a single Zone line, after any rules
    /// associated with it have been resolved to a collection of ZoneRecurrences.
    /// It may have an upper bound, or extend to infinity: lower bounds aren't known.
    /// Likewise it may have rules associated with it, or just a fixed offset and savings.
    /// </summary>
    internal sealed class ZoneRuleSet
    {
        // Either rules or name+fixedSavings is specified.
        private readonly List<ZoneRecurrence> rules = new List<ZoneRecurrence>();
        private readonly string name;
        private readonly Offset fixedSavings;
        private readonly int upperYear;
        private readonly ZoneYearOffset upperYearOffset;
        internal Offset StandardOffset { get; }

        internal ZoneRuleSet(List<ZoneRecurrence> rules, Offset standardOffset, int upperYear, ZoneYearOffset upperYearOffset)
        {
            this.rules = rules;
            this.StandardOffset = standardOffset;
            this.upperYear = upperYear;
            this.upperYearOffset = upperYearOffset;
        }

        internal ZoneRuleSet(string name, Offset standardOffset, Offset savings, int upperYear, ZoneYearOffset upperYearOffset)
        {
            this.name = name;
            this.StandardOffset = standardOffset;
            this.fixedSavings = savings;
            this.upperYear = upperYear;
            this.upperYearOffset = upperYearOffset;
        }

        /// <summary>
        /// Returns <c>true</c> if this rule set extends to the end of time, or
        /// <c>false</c> if it has a finite end point.
        /// </summary>
        internal bool IsInfinite => upperYear == int.MaxValue;

        internal bool IsFixed => name != null;

        internal IEnumerable<ZoneRecurrence> Rules => rules;

        internal ZoneInterval CreateFixedInterval(Instant start)
        {
            Preconditions.CheckState(IsFixed, "Rule set is not fixed");
            var limit = GetUpperLimit(fixedSavings);
            return new ZoneInterval(name, start, limit, StandardOffset + fixedSavings, fixedSavings);
        }

        /// <summary>
        /// Gets the inclusive upper limit of time that this rule set applies to.
        /// </summary>
        /// <param name="savings">The daylight savings value during the final zone interval.</param>
        /// <returns>The <see cref="LocalInstant"/> of the upper limit for this rule set.</returns>
        internal Instant GetUpperLimit(Offset savings)
        {
            if (IsInfinite)
            {
                return Instant.AfterMaxValue;
            }
            var localInstant = upperYearOffset.GetOccurrenceForYear(upperYear);
            var offset = upperYearOffset.GetRuleOffset(StandardOffset, savings);
            return localInstant.SafeMinus(offset);
        }
    }
}
