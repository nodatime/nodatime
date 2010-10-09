#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
        /// Gets the last rule set if there are no rule sets one that spans all of time is created and returned.
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
        /// <remarks>
        /// A cutover is a point where the standard offset from GMT/UTC changed. This occurs mostly
        /// pre-1900. The standard offset at the cutover defaults to 0. Call <see
        /// cref="DateTimeZoneBuilder.SetStandardOffset"/> afterwards to change it.
        /// </remarks>
        /// <param name="year">The year of cutover.</param>
        /// <param name="mode">The transition mode.</param>
        /// <param name="monthOfYear">The month of year.</param>
        /// <param name="dayOfMonth">The day of the month. If negative, set to ((last day of month)
        /// - ~dayOfMonth). For example, if -1, set to last day of month</param>
        /// <param name="dayOfWeek">The day of week. If 0, ignore.</param>
        /// <param name="advanceDayOfWeek">if dayOfMonth does not fall on dayOfWeek, then if advanceDayOfWeek set to <c>true</c>
        /// advance to dayOfWeek when true, otherwise retreat to dayOfWeek when true.</param>
        /// <param name="tickOfDay">The <see cref="Duration"/> into the day. Additional precision for specifying time of day of transitions</param>
        /// <returns>This <see cref="DateTimeZoneBuilder"/> for chaining.</returns>
        public DateTimeZoneBuilder AddCutover(int year, TransitionMode mode, int monthOfYear, int dayOfMonth, int dayOfWeek, bool advanceDayOfWeek,
                                              Offset tickOfDay)
        {
            FieldUtils.VerifyFieldValue(CalendarSystem.Iso.Fields.MonthOfYear, "monthOfYear", monthOfYear);
            FieldUtils.VerifyFieldValue(CalendarSystem.Iso.Fields.DayOfMonth, "dayOfMonth", dayOfMonth, true);
            if (dayOfWeek != 0)
            {
                FieldUtils.VerifyFieldValue(CalendarSystem.Iso.Fields.DayOfWeek, "dayOfWeek", dayOfWeek);
            }
            FieldUtils.VerifyFieldValue(CalendarSystem.Iso.Fields.TickOfDay, "tickOfDay", tickOfDay.Ticks);

            return AddCutover(year, new ZoneYearOffset(mode, monthOfYear, dayOfMonth, dayOfWeek, advanceDayOfWeek, tickOfDay));
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

#if DEBUG_FULL
            Debug.WriteLine(string.Format("{0}: AddCutover({1}, {2})", Name, year, yearOffset));
#endif
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
#if DEBUG_FULL
            Debug.WriteLine(string.Format("{0}: SetStandardOffset({1})", Name, standardOffset));
#endif
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
#if DEBUG_FULL
            Debug.WriteLine(string.Format("{0}: SetFixedSavings({1}, {2})", Name, nameKey, savings));
#endif
            LastRuleSet.SetFixedSavings(nameKey, savings);
            return this;
        }

        /// <summary>
        /// Adds a recurring daylight saving time rule.
        /// </summary>
        /// <param name="nameKey">The name key of new rule.</param>
        /// <param name="savings">The <see cref="Duration"/> to add to standard offset.</param>
        /// <param name="fromYear">First year that rule is in effect. <see cref="Int32.MinValue"/> indicates beginning of time.</param>
        /// <param name="toYear">Last year (inclusive) that rule is in effect. <see cref="Int32.MaxValue"/> indicates end of time.</param>
        /// <param name="mode">The transition mode.</param>
        /// <param name="monthOfYear">The month of year.</param>
        /// <param name="dayOfMonth">The day of the month. If negative, set to ((last day of month)
        /// - ~dayOfMonth). For example, if -1, set to last day of month</param>
        /// <param name="dayOfWeek">The day of week. If 0, ignore.</param>
        /// <param name="advanceDayOfWeek">if dayOfMonth does not fall on dayOfWeek, then if advanceDayOfWeek set to <c>true</c>
        /// advance to dayOfWeek when true, otherwise retreat to dayOfWeek when true.</param>
        /// <param name="tickOfDay">The <see cref="Duration"/> into the day. Additional precision for specifying time of day of transitions</param>
        /// <returns>This <see cref="DateTimeZoneBuilder"/> for chaining.</returns>
        public DateTimeZoneBuilder AddRecurringSavings(String nameKey, Offset savings, int fromYear, int toYear, TransitionMode mode, int monthOfYear,
                                                       int dayOfMonth, int dayOfWeek, bool advanceDayOfWeek, Offset tickOfDay)
        {
            FieldUtils.VerifyFieldValue(CalendarSystem.Iso.Fields.MonthOfYear, "monthOfYear", monthOfYear);
            FieldUtils.VerifyFieldValue(CalendarSystem.Iso.Fields.DayOfMonth, "dayOfMonth", dayOfMonth, true);
            if (dayOfWeek != 0)
            {
                FieldUtils.VerifyFieldValue(CalendarSystem.Iso.Fields.DayOfWeek, "dayOfWeek", dayOfWeek);
            }
            FieldUtils.VerifyFieldValue(CalendarSystem.Iso.Fields.TickOfDay, "tickOfDay", tickOfDay.Ticks);
            var yearOffset = new ZoneYearOffset(mode, monthOfYear, dayOfMonth, dayOfWeek, advanceDayOfWeek, tickOfDay);
            return AddRecurringSavings(new ZoneRecurrence(nameKey, savings, yearOffset, fromYear, toYear));
        }

        /// <summary>
        /// Adds a recurring daylight saving time rule.
        /// </summary>
        /// <param name="nameKey">The name key of new rule.</param>
        /// <param name="savings">The <see cref="Duration"/> to add to standard offset.</param>
        /// <param name="fromYear">First year that rule is in effect. <see cref="Int32.MinValue"/> indicates beginning of time.</param>
        /// <param name="toYear">Last year (inclusive) that rule is in effect. <see cref="Int32.MaxValue"/> indicates end of time.</param>
        /// <param name="yearOffset">The offset into the year.</param>
        /// <returns>This <see cref="DateTimeZoneBuilder"/> for chaining.</returns> 
        public DateTimeZoneBuilder AddRecurringSavings(String nameKey, Offset savings, int fromYear, int toYear, ZoneYearOffset yearOffset)
        {
            if (yearOffset == null)
            {
                throw new ArgumentNullException("yearOffset");
            }
            return AddRecurringSavings(new ZoneRecurrence(nameKey, savings, yearOffset, fromYear, toYear));
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
#if DEBUG_FULL
            Debug.WriteLine(string.Format("{0}: AddRecurringSavings({1})", Name, recurrence));
#endif
            if (recurrence.FromYear <= recurrence.ToYear)
            {
                LastRuleSet.AddRule(recurrence);
            }
            return this;
        }

        /**
         * Processes all the rules and builds a DateTimeZone.
         *
         * @param id  time zone id to assign
         * @param outputID  true if the zone id should be output
         */

        public DateTimeZone ToDateTimeZone(String zoneId)
        {
            if (zoneId == null)
            {
                throw new ArgumentNullException("zoneId");
            }

            var transitions = new List<ZoneTransition>();
            DateTimeZone tailZone = null;
            Instant instant = Instant.MinValue;

            ZoneTransition nextTransition = null;
            int ruleSetCount = ruleSets.Count;
            for (int i = 0; i < ruleSetCount; i++)
            {
                var ruleSet = ruleSets[i];
                var transitionIterator = ruleSet.Iterator(instant);
                nextTransition = transitionIterator.First();
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
                            nextTransition = transitionIterator.Next();
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

            // Check if a simpler zone implementation can be returned.
            if (transitions.Count == 0)
            {
                if (tailZone != null)
                {
                    // This shouldn't happen, but handle just in case.
                    return tailZone;
                }
                return new FixedDateTimeZone(zoneId, Offset.Zero);
            }
            if (transitions.Count == 1 && tailZone == null)
            {
                var transition = transitions[0];
                return new FixedDateTimeZone(zoneId, transition.WallOffset);
            }
            if (tailZone == null)
            {
                tailZone = NullDateTimeZone.Instance;
            }
            var precalcedEnd = nextTransition != null ? nextTransition.Instant : Instant.MaxValue;
            var zone = new PrecalculatedDateTimeZone(zoneId, transitions, precalcedEnd, tailZone);
            if (zone.IsCachable())
            {
                return CachedDateTimeZone.ForZone(zone);
            }
            return zone;
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

            // If local time of new transition is same as last local time, just replace last
            // transition with new one.
            LocalInstant lastLocal = Instant.Add(lastTransition.Instant, lastTransition.WallOffset);
            LocalInstant newLocal = Instant.Add(transition.Instant, transition.WallOffset);

            if (newLocal != lastLocal)
            {
                transitions.Add(transition);
                return true;
            }

            transitions.RemoveAt(transitionCount - 1);
            return AddTransition(transitions, transition);
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