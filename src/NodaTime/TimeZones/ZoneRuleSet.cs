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

// TODO: This is a hack to get the code working. When the real ISoCalendarSystem is ready
//       remove all alias lines in all files in the package and remove the JIsoCalendarSystem.cs file.
using IsoCalendarSystem = NodaTime.TimeZones.JIsoCalendarSystem;

namespace NodaTime.TimeZones
{
    internal class ZoneRuleSet
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
            yearLimit = IsoCalendarSystem.Utc.Year.GetValue(now) + 100;
        }

        internal Duration StandardOffset { get; set; }

        private readonly List<ZoneRule> rules = new List<ZoneRule>();
        private string initialNameKey;
        private Duration initialSavings;
        private int upperYear = Int32.MaxValue;
        private ZoneYearOffset upperYearOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneRuleSet"/> class.
        /// </summary>
        internal ZoneRuleSet()
        {
        }

        /**
         * Copy constructor.
         */
        /*
        RuleSet(RuleSet rs) {
            iStandardOffset = rs.iStandardOffset;
            iRules = new ArrayList(rs.iRules);
            iInitialNameKey = rs.iInitialNameKey;
            iInitialSaveMillis = rs.iInitialSaveMillis;
            iUpperYear = rs.iUpperYear;
            iUpperYearOffset = rs.iUpperYearOffset;
        }
        */

        /// <summary>
        /// Sets the fixed savings for this rule set.
        /// </summary>
        /// <param name="nameKey">The name key.</param>
        /// <param name="savings">The savings <see cref="Duration"/>.</param>
        public void SetFixedSavings(String nameKey, Duration savings)
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
            if (!rules.Contains(rule)) {
                rules.Add(rule);
            }
        }

        /// <summary>
        /// Sets the inclusive upper limit for where this rule set applies.
        /// </summary>
        /// <param name="year">The end year (inclusive).</param>
        /// <param name="yearOffset">The end point in the year (inclusive).</param>
        public void SetUpperLimit(int year, ZoneYearOffset yearOffset)
        {
            upperYear = year;
            upperYearOffset = yearOffset;
        }

        /**
         * Returns a transition at firstTicks with the first name key and
         * offsets for this rule set. This method may return null.
         *
         * @param firstTicks ticks of first transition
         */
        public IEnumerable<ZoneTransition> Transitions(LocalInstant instant)
        {
            ZoneTransition transition = null;
            if (initialNameKey != null) {
                // Initial zone info explicitly set, so don't search the rules.
                transition = new ZoneTransition(instant, initialNameKey, StandardOffset + initialSavings, StandardOffset);
                yield return transition;
            }


        }

        /// <summary>
        /// Returns the first transition that includes the given instant.
        /// </summary>
        /// <param name="firstTicks">The first ticks.</param>
        /// <returns>The first transition or null if there are no transitions.</returns>
        public ZoneTransition firstTransition(LocalInstant instant)
        {
            if (initialNameKey != null) {
                // Initial zone info explicitly set, so don't search the rules.
                return new ZoneTransition(instant, initialNameKey, StandardOffset + initialSavings, StandardOffset);
            }

            // Make a copy before we destroy the rules.
            List<ZoneRule> copy = new List<ZoneRule>(rules);

            // Iterate through all the transitions until firstMillis is reached. Use the name key
            // and savings for whatever rule reaches the limit.

            LocalInstant nextInstant = LocalInstant.MinValue;
            Duration savings = Duration.Zero;
            ZoneTransition firstTransition = null;

            ZoneTransition next;
            while ((next = nextTransition(nextInstant, savings)) != null) {
                nextInstant = next.Instant;

                if (nextInstant == instant) {
                    firstTransition = new ZoneTransition(instant, next);
                    break;
                }

                if (nextInstant > instant) {
                    if (firstTransition == null) {
                        // Find first rule without savings. This way a more accurate nameKey is
                        // found even though no rule extends to the RuleSet's lower limit.
                        foreach (var rule in copy) {
                            if (rule.Savings == Duration.Zero) {
                                firstTransition = new ZoneTransition(instant, rule, StandardOffset);
                                break;
                            }
                        }
                    }
                    if (firstTransition == null) {
                        // Found no rule without savings. Create a transition with no savings
                        // anyhow, and use the best available name key.
                        firstTransition = new ZoneTransition(instant, next.Name, StandardOffset, StandardOffset);
                    }
                    break;
                }

                // Set first to the best transition found so far, but next iteration may find
                // something closer to lower limit.
                firstTransition = new ZoneTransition(instant, next);
                savings = next.Savings;
            }
            // rules = copy;
            return firstTransition;
        }

        /**
         * Returns null if RuleSet is exhausted or upper limit reached. Calling
         * this method will throw away rules as they each become
         * exhausted. Copy the RuleSet before using it to compute transitions.
         *
         * Returned transition may be a duplicate from previous
         * transition. Caller must call isTransitionFrom to filter out
         * duplicates.
         *
         * @param saveMillis savings before next transition
         */
        public ZoneTransition nextTransition(LocalInstant instant, Duration saveTicks)
        {
            IsoCalendarSystem calendar = IsoCalendarSystem.Utc;

            // Find next matching rule.
            ZoneRule nextRule = null;
            LocalInstant nextTicks = LocalInstant.MaxValue;

            for (int i = 0; i < rules.Count; i++) {
                ZoneRule rule = rules[i];
                LocalInstant next = rule.Next(instant, StandardOffset, saveTicks);
                if (next <= instant) {
                    rules.RemoveAt(i);
                    i--;
                    continue;
                }
                // Even if next is same as previous next, choose the rule
                // in order for more recently added rules to override.
                if (next <= nextTicks) {
                    // Found a better match.
                    nextRule = rule;
                    nextTicks = next;
                }
            }

            if (nextRule == null) {
                return null;
            }

            // Stop precalculating if year reaches some arbitrary limit.
            if (calendar.Year.GetValue(nextTicks) >= yearLimit) {
                return null;
            }

            // Check if upper limit reached or passed.
            if (upperYear < Int32.MaxValue) {
                LocalInstant upperMillis = upperYearOffset.MakeInstant(upperYear, StandardOffset, saveTicks);
                if (nextTicks >= upperMillis) {
                    // At or after upper limit.
                    return null;
                }
            }

            return new ZoneTransition(nextTicks, nextRule, StandardOffset);
        }


        /**
         * @param saveMillis savings before upper limit
         */
        public LocalInstant getUpperLimit(Duration saveTicks)
        {
            if (upperYear == Int32.MaxValue) {
                return LocalInstant.MaxValue;
            }
            return upperYearOffset.MakeInstant(upperYear, StandardOffset, saveTicks);
        }

        /**
         * Returns null if none can be built.
         */
        public DSTZone buildTailZone(String id)
        {
            if (rules.Count == 2) {
                ZoneRule startRule = rules[0];
                ZoneRule endRule = rules[1];
                if (startRule.IsInfinite && endRule.IsInfinite) {
                    // With exactly two infinitely recurring rules left, a
                    // simple DSTZone can be formed.

                    // The order of rules can come in any order, and it doesn't
                    // really matter which rule was chosen the 'start' and
                    // which is chosen the 'end'. DSTZone works properly either
                    // way.
                    return new DSTZone(id, StandardOffset, startRule.Recurrence, endRule.Recurrence);
                }
            }
            return null;
        }
    }

}
