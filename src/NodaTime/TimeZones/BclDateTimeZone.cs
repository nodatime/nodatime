#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
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
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Representation of a time zone converted from a <see cref="TimeZoneInfo"/> from the Base Class Library.
    /// </summary>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    public sealed class BclDateTimeZone : DateTimeZone
    {
        private static readonly int TypeInitializationChecking = NodaTime.Utility.TypeInitializationChecker.RecordInitializationStart();

        /// <summary>
        /// This is used to cache the last result of a call to <see cref="ForSystemDefault"/>, but it doesn't
        /// matter if it's out of date - we'll just create another wrapper if necessary. It's not *that* expensive to make
        /// a few more wrappers than we need.
        /// </summary>
        private static BclDateTimeZone systemDefault;

        private readonly TimeZoneInfo bclZone;
        private readonly List<AdjustmentInterval> adjustmentIntervals;

        // We may start and end with a long period of standard time.
        // Either or both of these may be null.
        private readonly ZoneInterval headInterval;
        private readonly ZoneInterval tailInterval;

        /// <summary>
        /// Returns the original <see cref="TimeZoneInfo"/> from which this was created.
        /// </summary>
        public TimeZoneInfo OriginalZone { get { return bclZone; } }

        /// <summary>
        /// Returns the display name associated with the time zone, as provided by the Base Class Library.
        /// </summary>
        public string DisplayName { get { return OriginalZone.DisplayName; } }

        private BclDateTimeZone(TimeZoneInfo bclZone, Offset minOffset, Offset maxOffset, List<AdjustmentInterval> adjustmentIntervals,
            ZoneInterval headInterval, ZoneInterval tailInterval)
            : base(bclZone.Id, bclZone.SupportsDaylightSavingTime, minOffset, maxOffset)
        {
            this.bclZone = bclZone;
            this.adjustmentIntervals = adjustmentIntervals;
            this.headInterval = headInterval;
            this.tailInterval = tailInterval;
        }

        /// <summary>
        /// Returns the zone interval for the given instant in time. See <see cref="ZonedDateTime"/> for more details.
        /// </summary>
        public override ZoneInterval GetZoneInterval(Instant instant)
        {
            if (headInterval != null && headInterval.Contains(instant))
            {
                return headInterval;
            }
            if (tailInterval != null && tailInterval.Contains(instant))
            {
                return tailInterval;
            }

            // Avoid having to worry about Instant.MaxValue for the rest of the class.
            if (instant == Instant.MaxValue)
            {
                return adjustmentIntervals[adjustmentIntervals.Count - 1].GetZoneInterval(instant);
            }

            int lower = 0; // Inclusive
            int upper = adjustmentIntervals.Count; // Exclusive

            while (lower < upper)
            {
                int current = (lower + upper) / 2;
                var candidate = adjustmentIntervals[current];
                if (candidate.Start > instant)
                {
                    upper = current;
                }
                else if (candidate.End <= instant)
                {
                    lower = current + 1;
                }
                else
                {
                    return candidate.GetZoneInterval(instant);
                }
            }
            throw new InvalidOperationException
                ("Instant " + instant + " did not exist in the range of adjustment intervals");
        }

        /// <summary>
        /// Creates a new <see cref="BclDateTimeZone" /> from a <see cref="TimeZoneInfo"/> from the Base Class Library.
        /// </summary>
        /// <param name="bclZone">The original time zone to take information from.</param>
        /// <returns>A Noda Time representation of the given time zone.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bclZone"/> is null.</exception>
        public static BclDateTimeZone FromTimeZoneInfo(TimeZoneInfo bclZone)
        {
            Preconditions.CheckNotNull(bclZone, "bclZone");
            Offset standardOffset = Offset.FromTimeSpan(bclZone.BaseUtcOffset);
            Offset minSavings = Offset.Zero; // Just in case we have negative savings!
            Offset maxSavings = Offset.Zero;

            var rules = bclZone.GetAdjustmentRules();
            if (!bclZone.SupportsDaylightSavingTime || rules.Length == 0)
            {
                var fixedInterval = new ZoneInterval(bclZone.StandardName, Instant.MinValue, Instant.MaxValue, standardOffset, Offset.Zero);
                return new BclDateTimeZone(bclZone, standardOffset, standardOffset, null, fixedInterval, null);
            }
            var adjustmentIntervals = new List<AdjustmentInterval>();
            var headInterval = ComputeHeadInterval(bclZone, rules[0]);
            Instant previousEnd = headInterval != null ? headInterval.End : Instant.MinValue;
            for (int i = 0; i < rules.Length; i++)
            {
                var rule = rules[i];
                ZoneRecurrence standard, daylight;
                GetRecurrences(bclZone, rule, out standard, out daylight);

                minSavings = Offset.Min(minSavings, daylight.Savings);
                maxSavings = Offset.Max(maxSavings, daylight.Savings);

                // Find the last valid transition by working back from the end of time. It's safe to unconditionally
                // take the value here, as there must *be* some recurrences.
                // TODO(Post-V1): Tidy this up. We're basically fundamentally broken around the end of time :(
                var oneDayBeforeTheEndOfTime = Instant.MaxValue - Duration.FromStandardDays(1);
                var lastStandard = standard.PreviousOrFail(oneDayBeforeTheEndOfTime, standardOffset, daylight.Savings);
                var lastDaylight = daylight.PreviousOrFail(oneDayBeforeTheEndOfTime, standardOffset, Offset.Zero);
                bool standardIsLater = lastStandard.Instant > lastDaylight.Instant;
                Transition lastTransition = standardIsLater ? lastStandard : lastDaylight;
                Offset seamSavings = lastTransition.NewOffset - standardOffset;
                string seamName = standardIsLater ? bclZone.StandardName : bclZone.DaylightName;

                Instant nextStart;
                // Now work out the new "previous end" - i.e. where the next adjustment interval will start.
                if (i == rules.Length - 1)
                {
                    // If the final rule ends at the end of DateTime's range, we can just treat the rule
                    // as going on forever.
                    if (rule.DateEnd.Year == 9999)
                    {
                        nextStart = Instant.MaxValue;
                    }
                    else
                    {
                        // This is very odd. Suppose the rule notionally ends on "December 31st 2011". We
                        // actually take that to mean the start of the next day (midnight January 1st 2012)
                        // but in local daylight saving time - whether we actually *were* in daylight saving time or not...
                        Offset daylightOffset = standardOffset + daylight.Savings;
                        LocalDateTime ruleEndLocal = LocalDateTime.FromDateTime(rule.DateEnd).PlusDays(1);
                        OffsetDateTime ruleEndOffset = new OffsetDateTime(ruleEndLocal, daylightOffset);
                        nextStart = ruleEndOffset.ToInstant();
                    }
                }
                else
                {
                    var nextRule = rules[i + 1];
                    // TODO(Post-V1): Tidy this up (we do the same thing on the next iteration...)
                    ZoneRecurrence nextStandard, nextDaylight;
                    GetRecurrences(bclZone, nextRule, out nextStandard, out nextDaylight);

                    // If the "start of seam" transition is a transition into standard time, we want to find the
                    // first transition into daylight time in the new set of rules, and vice versa. Again, there
                    // must *be* a transition, as otherwise the rules are invalid.
                    var firstRecurrence = standardIsLater ? nextDaylight : nextStandard;
                    nextStart = firstRecurrence.NextOrFail(lastTransition.Instant, standardOffset, seamSavings).Instant;
                }
                var seam = new ZoneInterval(seamName, lastTransition.Instant, nextStart, lastTransition.NewOffset, seamSavings);
                var adjustmentZone = new DaylightSavingsDateTimeZone("ignored", standardOffset, standard.ToInfinity(), daylight.ToInfinity());

                adjustmentIntervals.Add(new AdjustmentInterval(previousEnd, adjustmentZone, seam));
                previousEnd = nextStart;
            }
            ZoneInterval tailInterval = previousEnd == Instant.MaxValue ? null
                : new ZoneInterval(bclZone.StandardName, previousEnd, Instant.MaxValue, standardOffset, Offset.Zero);

            return new BclDateTimeZone(bclZone, standardOffset + minSavings, standardOffset + maxSavings, adjustmentIntervals, headInterval, tailInterval);
        }

        /// <summary>
        /// Work out the period of standard time (if any) before the first adjustment rule is applied.
        /// </summary>
        private static ZoneInterval ComputeHeadInterval(TimeZoneInfo zone, TimeZoneInfo.AdjustmentRule rule)
        {
            if (rule.DateStart.Year == 1)
            {
                return null;
            }
            ZoneRecurrence firstStandard, firstDaylight;
            GetRecurrences(zone, rule, out firstStandard, out firstDaylight);
            var standardOffset = Offset.FromTimeSpan(zone.BaseUtcOffset);
            Transition firstTransition = firstDaylight.NextOrFail(Instant.MinValue, standardOffset, Offset.Zero);
            return new ZoneInterval(zone.StandardName, Instant.MinValue, firstTransition.Instant, standardOffset, Offset.Zero);
        }

        /// <summary>
        /// Converts the two adjustment rules in <paramref name="rule"/> into two ZoneRecurrences,
        /// storing them in the out parameters.
        /// </summary>
        private static void GetRecurrences(TimeZoneInfo zone, TimeZoneInfo.AdjustmentRule rule,
            out ZoneRecurrence standardRecurrence, out ZoneRecurrence daylightRecurrence)
        {
            int startYear = rule.DateStart.Year == 1 ? int.MinValue : rule.DateStart.Year;
            int endYear = rule.DateEnd.Year == 9999 ? int.MaxValue : rule.DateEnd.Year;
            Offset daylightOffset = Offset.FromTimeSpan(rule.DaylightDelta);
            ZoneYearOffset daylightStart = ConvertTransition(rule.DaylightTransitionStart);
            ZoneYearOffset daylightEnd = ConvertTransition(rule.DaylightTransitionEnd);
            standardRecurrence = new ZoneRecurrence(zone.StandardName, Offset.Zero, daylightEnd, startYear, endYear);
            daylightRecurrence = new ZoneRecurrence(zone.DaylightName, daylightOffset, daylightStart, startYear, endYear);
        }

        // Converts a TimeZoneInfo "TransitionTime" to a "ZoneYearOffset" - the two correspond pretty closely.
        private static ZoneYearOffset ConvertTransition(TimeZoneInfo.TransitionTime transitionTime)
        {
            // Easy case - fixed day of the month.
            if (transitionTime.IsFixedDateRule)
            {
                return new ZoneYearOffset(TransitionMode.Wall, transitionTime.Month, transitionTime.Day,
                    0, false, Offset.FromTimeSpan(transitionTime.TimeOfDay.TimeOfDay));
            }

            // Floating: 1st Sunday in March etc.
            int dayOfWeek = (int) BclConversions.ToIsoDayOfWeek(transitionTime.DayOfWeek);
            int dayOfMonth;
            bool advance;
            // "Last"
            if (transitionTime.Week == 5)
            {
                advance = false;
                dayOfMonth = -1;
            }
            else
            {
                advance = true;
                // Week 1 corresponds to ">=1"
                // Week 2 corresponds to ">=8" etc
                dayOfMonth = (transitionTime.Week * 7) - 6;
            }
            return new ZoneYearOffset(TransitionMode.Wall, transitionTime.Month, dayOfMonth,
                dayOfWeek, advance, Offset.FromTimeSpan(transitionTime.TimeOfDay.TimeOfDay));
        }

        /// <summary>
        /// Interval covered by an adjustment rule. The start instant is that of the
        /// first transition reported by this rule, and the seam covers the gap between
        /// two adjustment rules.
        /// </summary>
        private class AdjustmentInterval
        {
            private readonly Instant start;
            private readonly ZoneInterval seam;
            private readonly DaylightSavingsDateTimeZone adjustmentZone;

            internal Instant Start { get { return start; } }
            internal Instant End { get { return seam.End; } }

            internal AdjustmentInterval(Instant start, DaylightSavingsDateTimeZone adjustmentZone, ZoneInterval seam)
            {
                this.start = start;
                this.seam = seam;
                this.adjustmentZone = adjustmentZone;
            }

            internal ZoneInterval GetZoneInterval(Instant instant)
            {
                if (seam.Contains(instant))
                {
                    return seam;
                }
                return adjustmentZone.GetZoneInterval(instant);
            }
        }

        /// <summary>
        /// Returns a time zone converted from the BCL representation of the system local time zone.
        /// If this method is called more than once, it may return the same reference multiple times if
        /// the local time zone has not changed.
        /// </summary>
        /// <remarks>
        /// When the source of a <see cref="DateTimeZoneCache"/> is set to an instance of <see cref="BclDateTimeZoneSource"/> it
        /// is highly likely that <see cref="DateTimeZoneCache.GetSystemDefault"/> will succeed - but in
        /// rare cases (such as the set of system time zones changing after the source is installed, or the local zone
        /// not being a normal "system" one) it is possible that it wouldn't be mapped. By contrast, this method will
        /// always succeed.
        /// </remarks>
        /// <returns>A <see cref="BclDateTimeZone"/> wrapping the "local" (system) time zone as returned by
        /// <see cref="TimeZoneInfo.Local"/>.</returns>
        public static BclDateTimeZone ForSystemDefault()
        {
            TimeZoneInfo local = TimeZoneInfo.Local;
            BclDateTimeZone currentSystemDefault = systemDefault;

            // Cached copy is out of date - wrap a new one
            if (currentSystemDefault == null || currentSystemDefault.OriginalZone != local)
            {
                currentSystemDefault = FromTimeZoneInfo(local);
                systemDefault = currentSystemDefault;
            }
            // Always return our local variable; the variable may have changed again.
            return currentSystemDefault;
        }

        /// <inheritdoc />
        protected override bool EqualsImpl(DateTimeZone zone)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return OriginalZone.GetHashCode();
        }
    }
}
