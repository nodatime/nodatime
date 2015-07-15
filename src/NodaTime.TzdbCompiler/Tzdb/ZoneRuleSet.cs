// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections;
using System.Collections.Generic;
using NodaTime.TimeZones;
using System.Linq;
using System.Collections.ObjectModel;

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
        // Don't pre-calculate more than 100 years into the future. Almost all zones will stop
        // pre-calculating far sooner anyhow. Either a simple DST cycle is detected or the last
        // rule is a fixed offset. If a zone has a fixed offset set more than 100 years into the
        // future, then it won't be observed.
        private static readonly Instant PrecomputationLimit = SystemClock.Instance.GetCurrentInstant().InUtc().LocalDateTime.PlusYears(100).InUtc().ToInstant();

        // Either rules or name+fixedSavings is specified.
        private readonly List<ZoneRecurrence> rules = new List<ZoneRecurrence>();
        private readonly string name;
        private readonly Offset fixedSavings;
        private readonly int upperYear;
        private readonly ZoneYearOffset upperYearOffset;
        private readonly Offset standardOffset;

        internal ZoneRuleSet(List<ZoneRecurrence> rules, Offset standardOffset, int upperYear, ZoneYearOffset upperYearOffset)
        {
            this.rules = rules;
            this.standardOffset = standardOffset;
            this.upperYear = upperYear;
            this.upperYearOffset = upperYearOffset;
        }

        internal ZoneRuleSet(string name, Offset standardOffset, Offset savings, int upperYear, ZoneYearOffset upperYearOffset)
        {
            this.name = name;
            this.standardOffset = standardOffset;
            this.fixedSavings = savings;
            this.upperYear = upperYear;
            this.upperYearOffset = upperYearOffset;
        }

        /// <summary>
        /// Returns <c>true</c> if this rule set extends to the end of time, or
        /// <c>false</c> if it has a finite end point.
        /// </summary>
        internal bool IsInfinite { get { return upperYear == int.MaxValue; } }
        
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
            var offset = upperYearOffset.GetRuleOffset(standardOffset, savings);
            return localInstant.SafeMinus(offset);
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


        internal BoundedIntervalSet ToBoundedIntervalSet(Instant startingInstant, bool finalRuleSet)
        {
            if (name != null)
            {
                // Fixed offset and savings
                var limit = GetUpperLimit(fixedSavings);
                var interval = new ZoneInterval(name, startingInstant, limit, standardOffset + fixedSavings, fixedSavings);
                return new BoundedIntervalSet(PartialZoneIntervalMap.ForZoneInterval(interval));
            }
            var intervals = new List<ZoneInterval>();

            var activeRules = new List<ZoneRecurrence>(this.rules);
            var firstName = rules.First(rule => rule.Savings == Offset.Zero).Name;
            // We should never see this transition the final map, as the first rule should always be a fixed offset one. TODO: validate!
            ZoneTransition lastTransition = new ZoneTransition(Instant.BeforeMinValue, firstName, standardOffset, Offset.Zero);

            while (true)
            {
                ZoneTransition bestTransition = null;
                ZoneRecurrence bestRecurrence = null;
                for (int i = 0; i < activeRules.Count; i++)
                {
                    var rule = activeRules[i];
                    var nextTransition = rule.Next(lastTransition.Instant, lastTransition.StandardOffset, lastTransition.Savings);
                    // Once a rule is no longer active, remove it from the list. That way we can tell
                    // when we can create a tail zone.
                    if (nextTransition == null)
                    {
                        activeRules.RemoveAt(i);
                        i--;
                        continue;
                    }
                    var zoneTransition = new ZoneTransition(nextTransition.Value.Instant, rule.Name, standardOffset, rule.Savings);
                    if (!zoneTransition.IsTransitionFrom(lastTransition))
                    {
                        continue;
                    }
                    if (bestRecurrence == null || zoneTransition.Instant <= bestTransition.Instant)
                    {
                        bestRecurrence = rule;
                        bestTransition = zoneTransition;
                    }
                }
                Instant currentUpperBound = GetUpperLimit(lastTransition.Savings);
                if (bestRecurrence == null || bestTransition.Instant >= currentUpperBound)
                {
                    // No more transitions to find. (We may have run out of rules, or they may be beyond where this rule set expires.)
                    // Still, we had a final transition, so bridge from there to the current upper bound.
                    intervals.Add(lastTransition.ToZoneInterval(currentUpperBound));
                    return new BoundedIntervalSet(intervals, startingInstant, currentUpperBound, null);
                }
                // TODO: Only add the interval if it ends after startingInstant? Don't really need to...
                intervals.Add(lastTransition.ToZoneInterval(bestTransition.Instant));
                if (finalRuleSet && IsInfinite && activeRules.Count == 2 && bestTransition.Instant >= startingInstant)
                {
                    ZoneRecurrence startRule = activeRules[0];
                    ZoneRecurrence endRule = activeRules[1];
                    if (startRule.IsInfinite && endRule.IsInfinite)
                    {
                        var tailZone = new DaylightSavingsDateTimeZone("ignored", standardOffset, startRule, endRule);
                        return new BoundedIntervalSet(intervals, startingInstant, intervals.Last().End, tailZone);
                    }
                }
                lastTransition = bestTransition;
            }
        }

        /// <summary>
        /// A partial zone interval map created from the rules, ending when the rules expire *or* when
        /// the rest of the information can be represented by a tail zone.
        /// </summary>
        internal sealed class BoundedIntervalSet
        {
            internal PartialZoneIntervalMap PartialMap { get; }

            internal DaylightSavingsDateTimeZone TailZone { get; }

            internal BoundedIntervalSet(PartialZoneIntervalMap partialMap)
            {
                this.PartialMap = partialMap;
                this.TailZone = null;
            }

            internal BoundedIntervalSet(List<ZoneInterval> intervals, Instant start, Instant end, DaylightSavingsDateTimeZone tailZone)
            {
                // Extend the final zone interval to the end of time, just so we can easily build a full map...
                // the partial map will be restricted to the end point anyway.
                intervals[intervals.Count - 1] = intervals.Last().WithEnd(Instant.AfterMaxValue);
                this.PartialMap = new PartialZoneIntervalMap(start, end, new BinarySearchZoneIntervalMap(intervals));
                this.TailZone = tailZone;
            }
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
        internal sealed class TransitionIterator
        {
            private readonly ZoneRuleSet ruleSet;
            private readonly Instant startingInstant;
            private Instant instant;

            private List<ZoneRecurrence> rules;

            /// <summary>
            /// Initializes a new instance of the <see cref="TransitionIterator"/> class.
            /// </summary>
            /// <param name="ruleSet">The rule set to iterate over.</param>
            /// <param name="startingInstant">The starting instant.</param>
            internal TransitionIterator(ZoneRuleSet ruleSet, Instant startingInstant)
            {
                this.ruleSet = ruleSet;
                this.startingInstant = startingInstant;
            }

            internal Offset Savings { get; private set; }

            /// <summary>
            /// Returns the first transition. If called after iteration has started, resets to the
            /// beginning and returns the first transition.
            /// </summary>
            /// <returns>The first <see cref="ZoneTransition"/> or null if there are none.</returns>
            internal ZoneTransition First()
            {
                rules = new List<ZoneRecurrence>(ruleSet.rules);
                Savings = Offset.Zero;
                var result = GetFirst();
                SetupNext(result);
                return result;
            }

            /// <summary>
            /// Returns the next transition if any.
            /// </summary>
            /// <returns>The next <see cref="ZoneTransition"/> or null if no more exist.</returns>
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
            /// <returns>The new <see cref="DateTimeZone"/> or null.</returns>
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
                        return new DaylightSavingsDateTimeZone(id, ruleSet.standardOffset, startRule, endRule);
                    }
                }
                return null;
            }

            private ZoneTransition GetFirst()
            {
                if (ruleSet.name != null)
                {
                    // Initial zone info explicitly set, so don't search the rules.
                    return new ZoneTransition(startingInstant, ruleSet.name, ruleSet.standardOffset, ruleSet.fixedSavings);
                }

                // Make a copy before we destroy the rules.
                var saveRules = new List<ZoneRecurrence>(rules);

                // Iterate through all the transitions until startingInstant is reached. Use the name key
                // and savings for whatever rule reaches the limit.

                Instant nextInstant = Instant.BeforeMinValue;
                ZoneTransition firstTransition = null;

                // This code is taken directly from Joda Time, but it's not clear how valid it is. It
                // ends up with some spurious transitions which are then fixed by AddTransition.
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
                                    firstTransition = new ZoneTransition(startingInstant, rule.Name, ruleSet.standardOffset, Offset.Zero);
                                    break;
                                }
                            }
                        }
                        if (firstTransition == null)
                        {
                            // Found no rule without savings. Create a transition with no savings
                            // anyhow, and use the best available name key.
                            firstTransition = new ZoneTransition(startingInstant, next.Name, ruleSet.standardOffset, Offset.Zero);
                        }
                        break;
                    }

                    // Set first to the best transition found so far, but next iteration may find
                    // something closer to lower limit.
                    firstTransition = new ZoneTransition(startingInstant, next.Name, next.StandardOffset, next.Savings);
                    Savings = next.Savings;
                }
                // Restore rules
                rules = saveRules;
                return firstTransition;
            }

            private ZoneTransition GetNext(Instant nextInstant)
            {
                // Find next matching rule.
                ZoneRecurrence nextRule = null;
                Instant nextTicks = Instant.AfterMaxValue;

                for (int i = 0; i < rules.Count; i++)
                {
                    ZoneRecurrence rule = rules[i];
                    Transition? nextTransition = rule.Next(nextInstant, ruleSet.standardOffset, Savings);
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
                if (nextTicks >= PrecomputationLimit)
                {
                    return null;
                }

                // Check if upper limit reached or passed.
                if (ruleSet.upperYear < Int32.MaxValue)
                {
                    Instant upperTicks = ruleSet.GetUpperLimit(Savings);
                    if (nextTicks >= upperTicks)
                    {
                        // At or after upper limit.
                        return null;
                    }
                }
                return new ZoneTransition(nextTicks, nextRule.Name, ruleSet.standardOffset, nextRule.Savings);
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
                    Savings = transition.Savings;
                }
            }
        }
        #endregion
    }
}
