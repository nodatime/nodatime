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
using System.Collections.Generic;
using NodaTime.Fields;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides a means of programatically creating complex time zones. Currently internal, but we
    /// may want to make it public again eventually.
    /// </summary>
    /// <remarks>
    /// <para>
    /// DateTimeZoneBuilder allows complex DateTimeZones to be constructed. Since creating a new
    /// DateTimeZone this way is a relatively expensive operation, built zones can be written to a
    /// file. Reading back the encoded data is a quick operation.
    /// </para>
    /// <para>
    /// DateTimeZoneBuilder itself is mutable and not thread-safe, but the DateTimeZone objects that
    /// it builds are thread-safe and immutable.
    /// </para>
    /// <para>
    /// It is intended that {@link ZoneInfoCompiler} be used to read time zone data files,
    /// indirectly calling DateTimeZoneBuilder. The following complex example defines the
    /// America/Los_Angeles time zone, with all historical transitions:
    /// </para>
    /// <para>
    /// <example>
    ///     DateTimeZone America_Los_Angeles = new DateTimeZoneBuilder()
    ///         .AddCutover(-2147483648, 'w', 1, 1, 0, false, 0)
    ///         .SetStandardOffset(-28378000)
    ///         .SetFixedSavings("LMT", 0)
    ///         .AddCutover(1883, 'w', 11, 18, 0, false, 43200000)
    ///         .SetStandardOffset(-28800000)
    ///         .AddRecurringSavings("PDT", 3600000, 1918, 1919, 'w',  3, -1, 7, false, 7200000)
    ///         .AddRecurringSavings("PST",       0, 1918, 1919, 'w', 10, -1, 7, false, 7200000)
    ///         .AddRecurringSavings("PWT", 3600000, 1942, 1942, 'w',  2,  9, 0, false, 7200000)
    ///         .AddRecurringSavings("PPT", 3600000, 1945, 1945, 'u',  8, 14, 0, false, 82800000)
    ///         .AddRecurringSavings("PST",       0, 1945, 1945, 'w',  9, 30, 0, false, 7200000)
    ///         .AddRecurringSavings("PDT", 3600000, 1948, 1948, 'w',  3, 14, 0, false, 7200000)
    ///         .AddRecurringSavings("PST",       0, 1949, 1949, 'w',  1,  1, 0, false, 7200000)
    ///         .AddRecurringSavings("PDT", 3600000, 1950, 1966, 'w',  4, -1, 7, false, 7200000)
    ///         .AddRecurringSavings("PST",       0, 1950, 1961, 'w',  9, -1, 7, false, 7200000)
    ///         .AddRecurringSavings("PST",       0, 1962, 1966, 'w', 10, -1, 7, false, 7200000)
    ///         .AddRecurringSavings("PST",       0, 1967, 2147483647, 'w', 10, -1, 7, false, 7200000)
    ///         .AddRecurringSavings("PDT", 3600000, 1967, 1973, 'w', 4, -1,  7, false, 7200000)
    ///         .AddRecurringSavings("PDT", 3600000, 1974, 1974, 'w', 1,  6,  0, false, 7200000)
    ///         .AddRecurringSavings("PDT", 3600000, 1975, 1975, 'w', 2, 23,  0, false, 7200000)
    ///         .AddRecurringSavings("PDT", 3600000, 1976, 1986, 'w', 4, -1,  7, false, 7200000)
    ///         .AddRecurringSavings("PDT", 3600000, 1987, 2147483647, 'w', 4, 1, 7, true, 7200000)
    ///         .ToDateTimeZone("America/Los_Angeles");
    /// </example>
    /// </para>
    /// <para>
    /// Original name: DateTimeZoneBuilder.
    /// </para>
    /// </remarks>
    internal sealed class DateTimeZoneBuilder
    {
        private readonly IList<ZoneRecurrenceCollection> ruleSets = new List<ZoneRecurrenceCollection>();

        /// <summary>
        /// Gets the last rule set in this builder. If there are currently no rule sets,
        /// one that spans all of time is created and returned.
        /// </summary>
        /// <value>The last rule set.</value>
        private ZoneRecurrenceCollection LastRuleSet
        {
            get
            {
                if (ruleSets.Count == 0)
                {
                    AddEndOfTimeRuleSet();
                }
                return ruleSets[ruleSets.Count - 1];
            }
        }

        /// <summary>
        /// Adds a cutover for added rules.
        /// </summary>
        /// <param name="year">The year of cutover.</param>
        /// <param name="yearOffset">The offset into the year of the cutover.</param>
        /// <returns></returns>
        /// <remarks>
        /// A cutover is a point where the standard offset from GMT/UTC changed. This occurs mostly
        /// pre-1900. The standard offset at the cutover defaults to 0.
        /// Call <see cref="DateTimeZoneBuilder.SetStandardOffset"/> afterwards to change it.
        /// </remarks>
        public DateTimeZoneBuilder AddCutover(int year, ZoneYearOffset yearOffset)
        {
            if (yearOffset == null)
            {
                throw new ArgumentNullException("yearOffset");
            }

            if (ruleSets.Count > 0)
            {
                LastRuleSet.SetUpperLimit(year, yearOffset);
            }
            AddEndOfTimeRuleSet();
            return this;
        }

        /// <summary>
        /// Sets the standard offset to use for newly added rules until the next cutover is added.
        /// </summary>
        /// <param name="standardOffset">The standard offset.</param>
        /// <returns>This <see cref="DateTimeZoneBuilder"/> for chaining.</returns>
        public DateTimeZoneBuilder SetStandardOffset(Offset standardOffset)
        {
            LastRuleSet.StandardOffset = standardOffset;
            return this;
        }

        /// <summary>
        /// Sets a fixed savings rule at the cutover.
        /// </summary>
        /// <param name="nameKey">The name key of new rule.</param>
        /// <param name="savings">The <see cref="Duration"/> to add to standard offset.</param>
        /// <returns>This <see cref="DateTimeZoneBuilder"/> for chaining.</returns>
        public DateTimeZoneBuilder SetFixedSavings(String nameKey, Offset savings)
        {
            LastRuleSet.SetFixedSavings(nameKey, savings);
            return this;
        }

        /// <summary>
        /// Adds a recurring daylight saving time rule.
        /// </summary>
        /// <param name="recurrence">The zone recurrence defining the recurrening savings.</param>
        /// <returns>This <see cref="DateTimeZoneBuilder"/> for chaining.</returns> 
        public DateTimeZoneBuilder AddRecurringSavings(ZoneRecurrence recurrence)
        {
            if (recurrence == null)
            {
                throw new ArgumentNullException("recurrence");
            }
            if (recurrence.FromYear <= recurrence.ToYear)
            {
                LastRuleSet.AddRule(recurrence);
            }
            return this;
        }

        /// <summary>
        /// Processes all the rules and builds a DateTimeZone.
        /// </summary>
        /// <param name="zoneId">Time zone ID to assign</param>
        public DateTimeZone ToDateTimeZone(String zoneId)
        {
            if (zoneId == null)
            {
                throw new ArgumentNullException("zoneId");
            }

            var transitions = new List<ZoneTransition>();
            DateTimeZone tailZone = null;
            Instant instant = Instant.MinValue;

            int ruleSetCount = ruleSets.Count;
            bool tailZoneSeamValid = false;
            for (int i = 0; i < ruleSetCount; i++)
            {
                var ruleSet = ruleSets[i];
                var transitionIterator = ruleSet.Iterator(instant);
                ZoneTransition nextTransition = transitionIterator.First();
                if (nextTransition == null)
                {
                    continue;
                }
                AddTransition(transitions, nextTransition);

                while ((nextTransition = transitionIterator.Next()) != null)
                {
                    if (AddTransition(transitions, nextTransition))
                    {
                        if (tailZone != null)
                        {
                            // Got the extra transition before DaylightSavingsTimeZone.
                            // This final transition has a valid start point and offset, but
                            // we don't know where it ends - which is fine, as the tail zone will
                            // take over.
                            tailZoneSeamValid = true;
                            break;
                        }
                    }
                    if (tailZone == null && i == ruleSetCount - 1)
                    {
                        tailZone = transitionIterator.BuildTailZone(zoneId);
                        // If tailZone is not null, don't break out of main loop until at least one
                        // more transition is calculated. This ensures a correct 'seam' to the
                        // DaylightSavingsTimeZone.
                    }
                }

                instant = ruleSet.GetUpperLimit(transitionIterator.Savings);
            }

            // Simple case where we don't have a trailing daylight saving zone.
            if (tailZone == null)
            {
                switch (transitions.Count)
                {
                    case 0:
                        return new FixedDateTimeZone(zoneId, Offset.Zero);
                    case 1:
                        return new FixedDateTimeZone(zoneId, transitions[0].WallOffset);
                    default:
                        var ret = new PrecalculatedDateTimeZone(zoneId, transitions, Instant.MaxValue, null);
                        return ret.IsCachable() ? CachedDateTimeZone.ForZone(ret) : ret;
                }
            }

            // Sanity check
            if (!tailZoneSeamValid)
            {
                throw new InvalidOperationException("Invalid time zone data for id " + zoneId + "; no valid transition before tail zone");
            }

            // The final transition should not be used for a zone interval,
            // although it should have the same offset etc as the tail zone for its starting point.
            var lastTransition = transitions[transitions.Count - 1];
            var firstTailZoneInterval = tailZone.GetZoneInterval(lastTransition.Instant);
            if (lastTransition.StandardOffset != firstTailZoneInterval.StandardOffset ||
                lastTransition.WallOffset != firstTailZoneInterval.WallOffset ||
                lastTransition.Savings != firstTailZoneInterval.Savings ||
                lastTransition.Name != firstTailZoneInterval.Name)
            {
                throw new InvalidOperationException(
                    string.Format("Invalid seam to tail zone in time zone {0}; final transition {1} different to first tail zone interval {2}",
                                  zoneId, lastTransition, firstTailZoneInterval));
            }

            transitions.RemoveAt(transitions.Count - 1);
            var zone = new PrecalculatedDateTimeZone(zoneId, transitions, lastTransition.Instant, tailZone);
            return zone.IsCachable() ? CachedDateTimeZone.ForZone(zone) : zone;
        }

        /// <summary>
        /// Adds the given transition to the transition list if it represents a new transition.
        /// </summary>
        /// <param name="transitions">The list of <see cref="ZoneTransition"/> to add to.</param>
        /// <param name="transition">The transition to add.</param>
        /// <returns><c>true</c> if the transition was added.</returns>
        private static bool AddTransition(IList<ZoneTransition> transitions, ZoneTransition transition)
        {
            int transitionCount = transitions.Count;
            if (transitionCount == 0)
            {
                transitions.Add(transition);
                return true;
            }

            ZoneTransition lastTransition = transitions[transitionCount - 1];
            if (!transition.IsTransitionFrom(lastTransition))
            {
                return false;
            }

            Offset lastOffset = transitions.Count < 2 ? Offset.Zero : transitions[transitions.Count - 2].WallOffset;
            Offset newOffset = lastTransition.WallOffset;
            // If the local time just before the new transition is the same as the local time just
            // before the previous one, just replace the last transition with new one.
            // TODO(Post-V1): It's not clear what this is doing... work it out and give an example
            LocalInstant lastLocalStart = lastTransition.Instant.Plus(lastOffset);
            LocalInstant newLocalStart = transition.Instant.Plus(newOffset);
            if (lastLocalStart == newLocalStart)
            {
                transitions.RemoveAt(transitionCount - 1);
                return AddTransition(transitions, transition);
            }
            transitions.Add(transition);
            return true;
        }

        /// <summary>
        /// Adds a rule set that spans from the last one to the end of time.
        /// </summary>
        private void AddEndOfTimeRuleSet()
        {
            ruleSets.Add(new ZoneRecurrenceCollection());
        }
    }
}