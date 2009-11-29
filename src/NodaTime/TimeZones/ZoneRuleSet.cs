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
using System.Collections.Generic;
using NodaTime.Calendars;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Not immutable, not thread safe. 
    /// </para>
    /// </remarks>
    public class ZoneRuleSet
        : IEnumerable<ZoneRule>
    {
        private static readonly int yearLimit;

        /// <summary>
        /// Initializes the <see cref="ZoneRuleSet"/> class.
        /// </summary>
        static ZoneRuleSet()
        {
            // Don't pre-calculate more than 100 years into the future. Almost all zones will stop
            // pre-calculating far sooner anyhow. Either a simple DST cycle is detected or the last
            // rule is a fixed offset. If a zone has a fixed offset set more than 100 years into the
            // future, then it won't be observed.
            LocalInstant now = new LocalInstant(Clock.Now.Ticks);
            yearLimit = IsoCalendarSystem.Instance.Fields.Year.GetValue(now) + 100;
        }

        internal Offset StandardOffset { get; set; }

        private readonly List<ZoneRule> rules = new List<ZoneRule>();
        private string initialNameKey;
        private Offset initialSavings;
        private int upperYear = Int32.MaxValue;
        private ZoneYearOffset upperYearOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneRuleSet"/> class.
        /// </summary>
        public ZoneRuleSet()
        {
        }

        /// <summary>
        /// Sets the fixed savings for this rule set.
        /// </summary>
        /// <param name="nameKey">The name key.</param>
        /// <param name="savings">The savings <see cref="Offset"/>.</param>
        internal void SetFixedSavings(String nameKey, Offset savings)
        {
            initialNameKey = nameKey;
            initialSavings = savings;
        }

        /// <summary>
        /// Adds the given rule to the set if it is not already in the set.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        public void AddRule(ZoneRule rule)
        {
            if (!rules.Contains(rule))
            {
                rules.Add(rule);
            }
        }

        /// <summary>
        /// Sets the inclusive upper limit for where this rule set applies.
        /// </summary>
        /// <param name="year">The end year (inclusive).</param>
        /// <param name="yearOffset">The end point in the year (inclusive).</param>
        internal void SetUpperLimit(int year, ZoneYearOffset yearOffset)
        {
            upperYear = year;
            upperYearOffset = yearOffset;
        }

        /// <summary>
        /// Gets the inclusive upper limit of time that this rule set applies to.
        /// </summary>
        /// <param name="savings">The daylight savings value.</param>
        /// <returns>The <see cref="LocaolInstant"/> of the upper limit for this rule set.</returns>
        internal Instant getUpperLimit(Offset savings)
        {
            if (upperYear == Int32.MaxValue)
            {
                return Instant.MaxValue;
            }
            return upperYearOffset.MakeInstant(upperYear, StandardOffset, savings);
        }

        /// <summary>
        /// Returns an iterator over the transitions defined by this rule set.
        /// </summary>
        /// <param name="startingInstant">The starting instant.</param>
        /// <returns>The <see cref="TransitionIterator"/> that iterates the transitions.</returns>
        internal TransitionIterator Iterator(Instant startingInstant)
        {
            return new TransitionIterator(this, startingInstant);
        }

        /// <summary>
        /// Iterates through all of the time transitions starting with the given instant using the
        /// rules in the RuleSet to generate each one.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Conceptually a rule set is an expression language that describes a sequence of
        /// transitions points on a time-line where there are time dicontinuities. This iterator
        /// takes the rules and generates each transition from a given instant forward through time.
        /// </para>
        /// <para>
        /// This is the most complicated part of the <see cref="DateTimeZoneBuilder"/> classes.
        /// There is no guarentee that the rules will be in any order and even if they were there
        /// can be two rules, one for when DST starts and one for when DST ends, that generate
        /// alternate transitions through the years. The basic process asks each rule for the
        /// earliest transition it can generate that is after the last transition time and then
        /// returns the earliest of those as the next transition.
        /// </para>
        /// <para>
        /// In order to speed things up a bit (and to support the BuildTailZone functionality) we
        /// remove each rule as it becomes exhausted. A rule becomes exhausted when the last
        /// transition time exceeds the end date of the rule. In order to not affect the rule set we
        /// make a copy of the list when we are created and modify the copy.
        /// </para>
        /// <para>
        /// If there are only two rules left in the list and both are infinite rules then we can
        /// return a special class that will encapsulate them and save on generating every
        /// transition for the next hundred years. This is the BuildTailZone functionality.
        /// </para>
        /// <para>
        /// Not immutable, not thread safe. 
        /// </para>
        /// </remarks>
        internal class TransitionIterator
        {
            private readonly ZoneRuleSet ruleSet;
            private readonly ICalendarSystem calendar;
            private readonly Instant startingInstant;

            private List<ZoneRule> rules;
            private Offset savings;
            private Instant instant;

            internal Offset Savings { get { return this.savings; } }

            /// <summary>
            /// Initializes a new instance of the <see cref="TransitionIterator"/> class.
            /// </summary>
            /// <param name="ruleSet">The rule set to iterate over.</param>
            /// <param name="startingInstant">The starting instant.</param>
            internal TransitionIterator(ZoneRuleSet ruleSet, Instant startingInstant)
            {
                this.calendar = IsoCalendarSystem.Instance;
                this.ruleSet = ruleSet;
                this.startingInstant = startingInstant;
            }

            /// <summary>
            /// Returns the first transition. If called after iteration has started, resets to the
            /// beginning and returns the first transition.
            /// </summary>
            /// <returns>The first <see cref="ZoneTransition"/> or <c>null</c> there are none.</returns>
            internal ZoneTransition First()
            {
                this.rules = new List<ZoneRule>(ruleSet.rules);
                this.savings = Offset.Zero;
                ZoneTransition result = GetFirst();
                SetupNext(result);
                return result;
            }

            /// <summary>
            /// Returns the next transition if any.
            /// </summary>
            /// <returns>The next <see cref="ZoneTransition"/> or <c>null</c> if no more.</returns>
            internal ZoneTransition Next()
            {
                ZoneTransition result = GetNext(this.instant);
                SetupNext(result);
                return result;
            }

            /// <summary>
            /// If there are only two rule left and they are both infinite rules then a <see
            /// cref="IDateTimeZone"/> implmentation is returned that encapsulates those rules,
            /// otherwise null is returned.
            /// </summary>
            /// <param name="id">The id of the new <see cref="IDateTimeZone"/>.</param>
            /// <returns>The new <see cref="IDateTimeZone"/> or <c>null</c>.</returns>
            internal IDateTimeZone BuildTailZone(String id)
            {
                if (this.rules.Count == 2)
                {
                    ZoneRule startRule = this.rules[0];
                    ZoneRule endRule = this.rules[1];
                    if (startRule.IsInfinite && endRule.IsInfinite)
                    {
                        // With exactly two infinitely recurring rules left, a simple DSTZone can be
                        // formed.

                        // The order of rules can come in any order, and it doesn't really matter
                        // which rule was chosen the 'start' and which is chosen the 'end'. DSTZone
                        // works properly either way.
                        return new DSTZone(id, this.ruleSet.StandardOffset, startRule.Recurrence, endRule.Recurrence);
                    }
                }
                return null;
            }

            private ZoneTransition GetFirst()
            {
                if (this.ruleSet.initialNameKey != null)
                {
                    // Initial zone info explicitly set, so don't search the rules.
                    return new ZoneTransition(this.startingInstant, this.ruleSet.initialNameKey, this.ruleSet.StandardOffset + this.ruleSet.initialSavings, this.ruleSet.StandardOffset);
                }

                // Make a copy before we destroy the rules.
                var saveRules = new List<ZoneRule>(this.rules);

                // Iterate through all the transitions until firstMillis is reached. Use the name key
                // and savings for whatever rule reaches the limit.

                Instant nextInstant = Instant.MinValue;
                Offset savings = Offset.Zero;
                ZoneTransition firstTransition = null;

                for (ZoneTransition next = GetNext(nextInstant); next != null; next = GetNext(nextInstant))
                {
                    nextInstant = next.Instant;

                    if (nextInstant == this.startingInstant)
                    {
                        firstTransition = next;
                        break;
                    }

                    if (nextInstant > this.startingInstant)
                    {
                        if (firstTransition == null)
                        {
                            // Find first rule without savings. This way a more accurate nameKey is
                            // found even though no rule extends to the RuleSet's lower limit.
                            foreach (var rule in saveRules)
                            {
                                if (rule.Savings == Offset.Zero)
                                {
                                    firstTransition = new ZoneTransition(this.startingInstant, rule.Name, this.ruleSet.StandardOffset + rule.Savings, this.ruleSet.StandardOffset);
                                    break;
                                }
                            }
                        }
                        if (firstTransition == null)
                        {
                            // Found no rule without savings. Create a transition with no savings
                            // anyhow, and use the best available name key.
                            firstTransition = new ZoneTransition(this.startingInstant, next.Name, this.ruleSet.StandardOffset, this.ruleSet.StandardOffset);
                        }
                        break;
                    }

                    // Set first to the best transition found so far, but next iteration may find
                    // something closer to lower limit.
                    firstTransition = new ZoneTransition(this.startingInstant, next);
                    savings = next.Savings;
                }
                // Restore rules
                this.rules = saveRules;
                return firstTransition;
            }

            private ZoneTransition GetNext(Instant nextInstant)
            {
                // Find next matching rule.
                ZoneRule nextRule = null;
                Instant nextTicks = Instant.MaxValue;

                for (int i = 0; i < this.rules.Count; i++)
                {
                    ZoneRule rule = this.rules[i];
                    Instant next = rule.Next(nextInstant, this.ruleSet.StandardOffset, this.savings);
                    if (next <= nextInstant)
                    {
                        this.rules.RemoveAt(i);
                        i--;
                        continue;
                    }
                    // Even if next is same as previous next, choose the rule in order for more
                    // recently added rules to override.
                    if (next <= nextTicks)
                    {
                        // Found a better match.
                        nextRule = rule;
                        nextTicks = next;
                    }
                }

                if (nextRule == null)
                {
                    return null;
                }

                // Stop precalculating if year reaches some arbitrary limit. We can cheat in the
                // conversion because it is an approximation anyway.
                if (calendar.Fields.Year.GetValue(nextTicks + Offset.Zero) >= yearLimit)
                {
                    return null;
                }

                // Check if upper limit reached or passed.
                if (this.ruleSet.upperYear < Int32.MaxValue)
                {
                    Instant upperMillis = this.ruleSet.upperYearOffset.MakeInstant(this.ruleSet.upperYear, this.ruleSet.StandardOffset, this.savings);
                    if (nextTicks >= upperMillis)
                    {
                        // At or after upper limit.
                        return null;
                    }
                }
                return new ZoneTransition(nextTicks, nextRule.Name, this.ruleSet.StandardOffset + nextRule.Savings, this.ruleSet.StandardOffset);
            }

            /// <summary>
            /// Setups the next iteration by saving off necessary data.
            /// </summary>
            /// <param name="transition">The current transition to get the data from.</param>
            private void SetupNext(ZoneTransition transition)
            {
                if (transition != null)
                {
                    this.instant = transition.Instant;
                    this.savings = transition.Savings;
                }
            }
        }

        #region IEnumerable<ZoneRule> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate
        /// through the collection.
        /// </returns>
        public IEnumerator<ZoneRule> GetEnumerator()
        {
            return this.rules.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate
        /// through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.rules.GetEnumerator();
        }

        #endregion
    }
}
