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
using System.Collections;
using System.Collections.Generic;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Used to create time zones.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Not immutable, not thread safe. 
    /// </para>
    /// </remarks>
    internal class ZoneRecurrenceCollection : IEnumerable<ZoneRecurrence>
    {
        // Don't pre-calculate more than 100 years into the future. Almost all zones will stop
        // pre-calculating far sooner anyhow. Either a simple DST cycle is detected or the last
        // rule is a fixed offset. If a zone has a fixed offset set more than 100 years into the
        // future, then it won't be observed.
        private static readonly int YearLimit = CalendarSystem.Iso.Fields.Year.GetValue(new LocalInstant(SystemClock.Instance.Now.Ticks)) + 100;

        private readonly List<ZoneRecurrence> rules = new List<ZoneRecurrence>();
        private string initialNameKey;
        private Offset initialSavings;
        private int upperYear = Int32.MaxValue;
        private ZoneYearOffset upperYearOffset;
        internal Offset StandardOffset { private get; set; }

        #region IEnumerable<ZoneRecurrence> Members
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate
        /// through the collection.
        /// </returns>
        public IEnumerator<ZoneRecurrence> GetEnumerator()
        {
            return rules.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate
        /// through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return rules.GetEnumerator();
        }
        #endregion

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
        public void AddRule(ZoneRecurrence rule)
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
        /// <returns>The <see cref="LocalInstant"/> of the upper limit for this rule set.</returns>
        internal Instant GetUpperLimit(Offset savings)
        {
            return upperYear == Int32.MaxValue ? Instant.MaxValue : upperYearOffset.MakeInstant(upperYear, StandardOffset, savings);
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

        #region Nested type: TransitionIterator
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
            private readonly CalendarSystem calendar;
            private readonly ZoneRecurrenceCollection ruleSet;
            private readonly Instant startingInstant;
            private Instant instant;

            private List<ZoneRecurrence> rules;
            private Offset savings;

            /// <summary>
            /// Initializes a new instance of the <see cref="TransitionIterator"/> class.
            /// </summary>
            /// <param name="ruleSet">The rule set to iterate over.</param>
            /// <param name="startingInstant">The starting instant.</param>
            internal TransitionIterator(ZoneRecurrenceCollection ruleSet, Instant startingInstant)
            {
                calendar = CalendarSystem.Iso;
                this.ruleSet = ruleSet;
                this.startingInstant = startingInstant;
            }

            internal Offset Savings { get { return savings; } }

            /// <summary>
            /// Returns the first transition. If called after iteration has started, resets to the
            /// beginning and returns the first transition.
            /// </summary>
            /// <returns>The first <see cref="ZoneTransition"/> or <c>null</c> there are none.</returns>
            internal ZoneTransition First()
            {
                rules = new List<ZoneRecurrence>(ruleSet.rules);
                savings = Offset.Zero;
                var result = GetFirst();
                SetupNext(result);
                return result;
            }

            /// <summary>
            /// Returns the next transition if any.
            /// </summary>
            /// <returns>The next <see cref="ZoneTransition"/> or <c>null</c> if no more.</returns>
            internal ZoneTransition Next()
            {
                var result = GetNext(instant);
                SetupNext(result);
                return result;
            }

            /// <summary>
            /// If there are only two rules left and they are both infinite rules then a <see
            /// cref="DateTimeZone"/> implementation is returned that encapsulates those rules,
            /// otherwise null is returned.
            /// </summary>
            /// <param name="id">The id of the new <see cref="DateTimeZone"/>.</param>
            /// <returns>The new <see cref="DateTimeZone"/> or <c>null</c>.</returns>
            internal DateTimeZone BuildTailZone(String id)
            {
                if (rules.Count == 2)
                {
                    ZoneRecurrence startRule = rules[0];
                    ZoneRecurrence endRule = rules[1];
                    if (startRule.IsInfinite && endRule.IsInfinite)
                    {
                        // With exactly two infinitely recurring rules left, a simple DaylightSavingsTimeZone can be
                        // formed.

                        // The order of rules can come in any order, and it doesn't really matter
                        // which rule was chosen the 'start' and which is chosen the 'end'. DaylightSavingsTimeZone
                        // works properly either way.
                        return new DaylightSavingsTimeZone(id, ruleSet.StandardOffset, startRule, endRule);
                    }
                }
                return null;
            }

            private ZoneTransition GetFirst()
            {
                if (ruleSet.initialNameKey != null)
                {
                    // Initial zone info explicitly set, so don't search the rules.
                    return new ZoneTransition(startingInstant, ruleSet.initialNameKey, ruleSet.StandardOffset, ruleSet.initialSavings);
                }

                // Make a copy before we destroy the rules.
                var saveRules = new List<ZoneRecurrence>(rules);

                // Iterate through all the transitions until startingInstant is reached. Use the name key
                // and savings for whatever rule reaches the limit.

                Instant nextInstant = Instant.MinValue;
                ZoneTransition firstTransition = null;

                for (ZoneTransition next = GetNext(nextInstant); next != null; next = GetNext(nextInstant))
                {
                    nextInstant = next.Instant;

                    if (nextInstant == startingInstant)
                    {
                        firstTransition = next;
                        break;
                    }

                    if (nextInstant > startingInstant)
                    {
                        if (firstTransition == null)
                        {
                            // Find first rule without savings. This way a more accurate nameKey is
                            // found even though no rule extends to the RuleSet's lower limit.
                            foreach (var rule in saveRules)
                            {
                                if (rule.Savings == Offset.Zero)
                                {
                                    firstTransition = new ZoneTransition(startingInstant, rule.Name, ruleSet.StandardOffset, Offset.Zero);
                                    break;
                                }
                            }
                        }
                        if (firstTransition == null)
                        {
                            // Found no rule without savings. Create a transition with no savings
                            // anyhow, and use the best available name key.
                            firstTransition = new ZoneTransition(startingInstant, next.Name, ruleSet.StandardOffset, Offset.Zero);
                        }
                        break;
                    }

                    // Set first to the best transition found so far, but next iteration may find
                    // something closer to lower limit.
                    firstTransition = new ZoneTransition(startingInstant, next.Name, next.StandardOffset, next.Savings);
                    savings = next.Savings;
                }
                // Restore rules
                rules = saveRules;
                return firstTransition;
            }

            private ZoneTransition GetNext(Instant nextInstant)
            {
                // Find next matching rule.
                ZoneRecurrence nextRule = null;
                Instant nextTicks = Instant.MaxValue;

                for (int i = 0; i < rules.Count; i++)
                {
                    ZoneRecurrence rule = rules[i];
                    Transition? nextTransition = rule.Next(nextInstant, ruleSet.StandardOffset, savings);
                    Instant? next = nextTransition == null ? (Instant?)null : nextTransition.Value.Instant;
                    if (!next.HasValue || next.Value <= nextInstant)
                    {
                        rules.RemoveAt(i);
                        i--;
                        continue;
                    }
                    // Even if next is same as previous next, choose the rule in order for more
                    // recently added rules to override.
                    if (next.Value <= nextTicks)
                    {
                        // Found a better match.
                        nextRule = rule;
                        nextTicks = next.Value;
                    }
                }

                if (nextRule == null)
                {
                    return null;
                }

                // Stop precalculating if year reaches some arbitrary limit. We can cheat in the
                // conversion because it is an approximation anyway.
                if (calendar.Fields.Year.GetValue(nextTicks.Plus(Offset.Zero)) >= YearLimit)
                {
                    return null;
                }

                // Check if upper limit reached or passed.
                if (ruleSet.upperYear < Int32.MaxValue)
                {
                    Instant upperTicks = ruleSet.upperYearOffset.MakeInstant(ruleSet.upperYear, ruleSet.StandardOffset, savings);
                    if (nextTicks >= upperTicks)
                    {
                        // At or after upper limit.
                        return null;
                    }
                }
                return new ZoneTransition(nextTicks, nextRule.Name, ruleSet.StandardOffset, nextRule.Savings);
            }

            /// <summary>
            /// Setups the next iteration by saving off necessary data.
            /// </summary>
            /// <param name="transition">The current transition to get the data from.</param>
            private void SetupNext(ZoneTransition transition)
            {
                if (transition != null)
                {
                    instant = transition.Instant;
                    savings = transition.Savings;
                }
            }
        }
        #endregion
    }
}