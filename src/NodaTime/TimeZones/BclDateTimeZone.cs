// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Annotations;
#if !PCL
using System;
using System.Collections.Generic;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Representation of a time zone converted from a <see cref="TimeZoneInfo"/> from the Base Class Library.
    /// </summary>
    /// <remarks>
    /// Note that although this class implements <see cref="IEquatable{DateTimeZone}"/> by virtue of extending
    /// <see cref="DateTimeZone"/>, the implementation here will always throw <c>NotImplementedException</c> when asked
    /// to compare two different <c>BclDateTimeZone</c> instances.
    /// </remarks>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    [Immutable]
    public sealed class BclDateTimeZone : DateTimeZone
    {
        /// <summary>
        /// This is used to cache the last result of a call to <see cref="ForSystemDefault"/>, but it doesn't
        /// matter if it's out of date - we'll just create another wrapper if necessary. It's not *that* expensive to make
        /// a few more wrappers than we need.
        /// </summary>
        private static BclDateTimeZone systemDefault;

        /// <summary>
        /// This is a bit before the last valid tick where we would be able to construct a LocalDateTime. We need to be a little
        /// bit before that, to avoid failures when we add an offset.
        /// </summary>
        private static readonly Instant CloseToEndOfTime =
            new LocalDateTime(CalendarSystem.Iso.MaxYear, 12, 30, 23, 59, 59, 999, 9999).InUtc().ToInstant();

        private readonly TimeZoneInfo bclZone;
        private readonly IZoneIntervalMap map;

        /// <summary>
        /// Returns the original <see cref="TimeZoneInfo"/> from which this was created.
        /// </summary>
        public TimeZoneInfo OriginalZone { get { return bclZone; } }

        /// <summary>
        /// Returns the display name associated with the time zone, as provided by the Base Class Library.
        /// </summary>
        public string DisplayName { get { return OriginalZone.DisplayName; } }

        private BclDateTimeZone(TimeZoneInfo bclZone, Offset minOffset, Offset maxOffset, IZoneIntervalMap map)
            : base(bclZone.Id, bclZone.SupportsDaylightSavingTime, minOffset, maxOffset)
        {
            this.bclZone = bclZone;
            this.map = map;
        }

        /// <inheritdoc />
        public override ZoneInterval GetZoneInterval(Instant instant)
        {
            return map.GetZoneInterval(instant);
        }

        /// <summary>
        /// Creates a new <see cref="BclDateTimeZone" /> from a <see cref="TimeZoneInfo"/> from the Base Class Library.
        /// </summary>
        /// <param name="bclZone">The original time zone to take information from.</param>
        /// <returns>A <see cref="BclDateTimeZone"/> wrapping the given <c>TimeZoneInfo</c>.</returns>
        public static BclDateTimeZone FromTimeZoneInfo([NotNull] TimeZoneInfo bclZone)
        {
            Preconditions.CheckNotNull(bclZone, "bclZone");
            Offset standardOffset = Offset.FromTimeSpan(bclZone.BaseUtcOffset);
            Offset minSavings = Offset.Zero; // Just in case we have negative savings!
            Offset maxSavings = Offset.Zero;

            var rules = bclZone.GetAdjustmentRules();
            if (!bclZone.SupportsDaylightSavingTime || rules.Length == 0)
            {
                var fixedInterval = new ZoneInterval(bclZone.StandardName, Instant.BeforeMinValue, Instant.AfterMaxValue, standardOffset, Offset.Zero);
                return new BclDateTimeZone(bclZone, standardOffset, standardOffset, new FixedZoneIntervalMap(fixedInterval));
            }
            var adjustmentIntervals = new List<AdjustmentInterval>();
            var headInterval = ComputeHeadInterval(bclZone, rules[0]);
            // The head interval will never extend to the end of time, so calling End is safe.
            Instant previousEnd = headInterval != null ? headInterval.End : Instant.BeforeMinValue;
            
            // TODO(Post-V1): Tidy this up. All of this is horrible.
            for (int i = 0; i < rules.Length; i++)
            {
                var rule = rules[i];
                ZoneRecurrence standard, daylight;
                GetRecurrences(bclZone, rule, out standard, out daylight);

                minSavings = Offset.Min(minSavings, daylight.Savings);
                maxSavings = Offset.Max(maxSavings, daylight.Savings);

                // Find the last valid transition by working back from the end of time. It's safe to unconditionally
                // take the value here, as there must *be* some recurrences.
                var lastStandard = standard.PreviousOrFail(CloseToEndOfTime, standardOffset, daylight.Savings);
                var lastDaylight = daylight.PreviousOrFail(CloseToEndOfTime, standardOffset, Offset.Zero);
                bool standardIsLater = lastStandard.Instant > lastDaylight.Instant;
                Transition lastTransition = standardIsLater ? lastStandard : lastDaylight;
                Offset seamSavings = lastTransition.NewOffset - standardOffset;
                string seamName = standardIsLater ? bclZone.StandardName : bclZone.DaylightName;

                Instant nextStart;

                Instant ruleEnd = GetRuleEnd(rule, lastTransition);

                // Handle the final rule, which may or may not to extend to the end of time. If it doesn't,
                // we transition to standard time at the end of the rule.
                if (i == rules.Length - 1)
                {
                    // If the final transition was to standard time, we can just treat the seam as going on forever.
                    nextStart = standardIsLater ? Instant.MaxValue : ruleEnd;
                    var seam = new ZoneInterval(seamName, lastTransition.Instant, nextStart, lastTransition.NewOffset, seamSavings);
                    var adjustmentZone = new DaylightSavingsDateTimeZone("ignored", standardOffset, standard.ToInfinity(), daylight.ToInfinity());
                    adjustmentIntervals.Add(new AdjustmentInterval(previousEnd, adjustmentZone, seam));
                    previousEnd = nextStart;
                }
                else
                {
                    // Handle one rule going into another. This is the cause of much pain, as there are several options:
                    // 1) Going into a "normal" rule with two transitions per year.
                    // 2) Going into a "single transition" rule, signified by a transition "into" the current offset
                    //    right at the start of the year. This is treated as if the on-the-edge transition doesn't exist.
                    //
                    // Additionally, there's the possibility that the offset at the start of the next rule (i.e. the
                    // one before the first transition) isn't the same as the offset at the end of end of the current rule
                    // (i.e. lastTransition.NewOffset). This only occurs for Namibia time as far as we've seen, but in that
                    // case we create a seam which only goes until the end of this rule, then an extra zone interval for
                    // the time between the start of the rule and the first transition, then we're as normal, starting at
                    // the first transition. See bug 115 for a bit more information.
                    var nextRule = rules[i + 1];
                    ZoneRecurrence nextStandard, nextDaylight;
                    GetRecurrences(bclZone, nextRule, out nextStandard, out nextDaylight);

                    // By using the seam's savings for *both* checks, we can detect "daylight to daylight" and
                    // "standard to standard" transitions as happening at the very start of the rule.
                    var firstStandard = nextStandard.NextOrFail(lastTransition.Instant, standardOffset, seamSavings);
                    var firstDaylight = nextDaylight.NextOrFail(lastTransition.Instant, standardOffset, seamSavings);

                    // Ignore any "right at the start of the rule"  transitions.
                    var firstStandardInstant = firstStandard.Instant == ruleEnd ? Instant.AfterMaxValue : firstStandard.Instant;
                    var firstDaylightInstant = firstDaylight.Instant == ruleEnd ? Instant.AfterMaxValue : firstDaylight.Instant;
                    bool firstStandardIsEarlier = firstStandardInstant < firstDaylightInstant;
                    var firstTransition = firstStandardIsEarlier ? firstStandard : firstDaylight;
                    nextStart = firstTransition.Instant;
                    var seamEnd = nextStart;

                    AdjustmentInterval startOfRuleExtraSeam = null;

                    Offset previousOffset = firstStandardIsEarlier ? firstDaylight.NewOffset : firstStandard.NewOffset;
                    if (previousOffset != lastTransition.NewOffset)
                    {
                        seamEnd = ruleEnd;
                        // Recalculate the *real* transition, as we're now going from a different wall offset...
                        var firstRule = firstStandardIsEarlier ? nextStandard : nextDaylight;
                        nextStart = firstRule.NextOrFail(ruleEnd, standardOffset, previousOffset - standardOffset).Instant;
                        var extraSeam = new ZoneInterval(firstStandardIsEarlier ? bclZone.DaylightName : bclZone.StandardName,
                            ruleEnd, nextStart, previousOffset, previousOffset - standardOffset);
                        // The extra adjustment interval is really just a single zone interval; we'll never need the DaylightSavingsDateTimeZone part.
                        startOfRuleExtraSeam = new AdjustmentInterval(extraSeam.Start, null, extraSeam);
                    }

                    var seam = new ZoneInterval(seamName, lastTransition.Instant, seamEnd, lastTransition.NewOffset, seamSavings);
                    var adjustmentZone = new DaylightSavingsDateTimeZone("ignored", standardOffset, standard.ToInfinity(), daylight.ToInfinity());
                    adjustmentIntervals.Add(new AdjustmentInterval(previousEnd, adjustmentZone, seam));

                    if (startOfRuleExtraSeam != null)
                    {
                        adjustmentIntervals.Add(startOfRuleExtraSeam);
                    }
                    previousEnd = nextStart;
                }
            }
            ZoneInterval tailInterval = previousEnd == Instant.AfterMaxValue ? null
                : new ZoneInterval(bclZone.StandardName, previousEnd, Instant.AfterMaxValue, standardOffset, Offset.Zero);

            IZoneIntervalMap uncachedMap = new BclZoneIntervalMap(adjustmentIntervals, headInterval, tailInterval);
            IZoneIntervalMap cachedMap = CachingZoneIntervalMap.CacheMap(uncachedMap, CachingZoneIntervalMap.CacheType.Hashtable);
            return new BclDateTimeZone(bclZone, standardOffset + minSavings, standardOffset + maxSavings, cachedMap);
        }

        private static Instant GetRuleEnd(TimeZoneInfo.AdjustmentRule rule, Transition transition)
        {
            if (rule.DateEnd.Year == 9999)
            {
                return Instant.MaxValue;
            }
            // We work out the instant at which the *current* offset reaches the end of the given date.
            LocalDateTime ruleEndLocal = LocalDateTime.FromDateTime(rule.DateEnd).PlusDays(1);
            OffsetDateTime ruleEndOffset = new OffsetDateTime(ruleEndLocal, transition.NewOffset);
            return ruleEndOffset.ToInstant();
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
            return new ZoneInterval(zone.StandardName, Instant.BeforeMinValue, firstTransition.Instant, standardOffset, Offset.Zero);
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
                    0, false, LocalDateTime.FromDateTime(transitionTime.TimeOfDay).TimeOfDay);
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
                dayOfWeek, advance, LocalDateTime.FromDateTime(transitionTime.TimeOfDay).TimeOfDay);
        }

        /// <summary>
        /// Interval covered by an adjustment rule. The start instant is that of the
        /// first transition reported by this rule, and the seam covers the gap between
        /// two adjustment rules.
        /// </summary>
        private sealed class AdjustmentInterval
        {
            private readonly Instant start;
            private readonly ZoneInterval seam;
            private readonly DaylightSavingsDateTimeZone adjustmentZone;

            internal Instant Start { get { return start; } }
            internal Instant RawEnd { get { return seam.RawEnd; } }

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

        private sealed class FixedZoneIntervalMap : IZoneIntervalMap
        {
            private readonly ZoneInterval interval;

            internal FixedZoneIntervalMap(ZoneInterval interval)
            {
                this.interval = interval;
            }

            public ZoneInterval GetZoneInterval(Instant instant)
            {
                return interval;
            }
        }

        /// <summary>
        /// The core part of working out zone intervals; separated into its own type to allow for caching.
        /// </summary>
        private sealed class BclZoneIntervalMap : IZoneIntervalMap
        {
            private readonly List<AdjustmentInterval> adjustmentIntervals;

            // We may start and end with a long period of standard time.
            // Either or both of these may be null.
            private readonly ZoneInterval headInterval;
            private readonly ZoneInterval tailInterval;

            internal BclZoneIntervalMap(List<AdjustmentInterval> adjustmentIntervals, ZoneInterval headInterval, ZoneInterval tailInterval)
            {
                this.adjustmentIntervals = adjustmentIntervals;
                this.headInterval = headInterval;
                this.tailInterval = tailInterval;
            }

            /// <summary>
            /// Returns the zone interval for the given instant in time. See <see cref="ZonedDateTime"/> for more details.
            /// </summary>
            public ZoneInterval GetZoneInterval(Instant instant)
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
                    else if (candidate.RawEnd <= instant)
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
        }

        /// <summary>
        /// Returns a time zone converted from the BCL representation of the system local time zone.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is approximately equivalent to calling <see cref="IDateTimeZoneProvider.GetSystemDefault"/> with
        /// an implementation that wraps <see cref="BclDateTimeZoneSource"/> (e.g.
        /// <see cref="DateTimeZoneProviders.Bcl"/>), with the exception that it will succeed even if the current local
        /// time zone was not one of the set of system time zones captured when the source was created (which, while
        /// highly unlikely, might occur either because the local time zone is not a system time zone, or because the
        /// system time zones have themselves changed).
        /// </para>
        /// <para>
        /// This method will retain a reference to the returned <c>BclDateTimeZone</c>, and will attempt to return it if
        /// called repeatedly (assuming that the local time zone has not changed) rather than creating a new instance,
        /// though this behaviour is not guaranteed.
        /// </para>
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
        /// <remarks>
        /// This implementation always throws <c>NotImplementedException</c>.
        /// </remarks>
        /// <exception cref="NotImplementedException">Always.</exception>
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
#endif
