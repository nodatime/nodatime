// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Linq;
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Extensions;
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
    /// <para>
    /// Note that although this class implements <see cref="IEquatable{DateTimeZone}"/> by virtue of extending
    /// <see cref="DateTimeZone"/>, the implementation here will always throw <c>NotImplementedException</c> when asked
    /// to compare two different <c>BclDateTimeZone</c> instances.
    /// </para>
    /// <para>
    /// This implementation does not always give the same results as <c>TimeZoneInfo</c>, in that it doesn't replicate
    /// the bugs in the BCL interpretation of the data. These bugs are described in
    /// <a href="http://codeblog.jonskeet.uk/2014/09/30/the-mysteries-of-bcl-time-zone-data/">a blog post</a>, but we're
    /// not expecting them to be fixed any time soon. Being bug-for-bug compatible would not only be tricky, but would be painful
    /// if the BCL were ever to be fixed. As far as we are aware, there are only discrepancies around new year where the zone
    /// changes from observing one rule to observing another.
    /// </para>
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

        private readonly IZoneIntervalMap map;

        /// <summary>
        /// Gets the original <see cref="TimeZoneInfo"/> from which this was created.
        /// </summary>
        /// <value>The original <see cref="TimeZoneInfo"/> from which this was created.</value>
        public TimeZoneInfo OriginalZone { get; }

        /// <summary>
        /// Gets the display name associated with the time zone, as provided by the Base Class Library.
        /// </summary>
        /// <value>The display name associated with the time zone, as provided by the Base Class Library.</value>
        public string DisplayName => OriginalZone.DisplayName;

        private BclDateTimeZone(TimeZoneInfo bclZone, Offset minOffset, Offset maxOffset, IZoneIntervalMap map)
            : base(bclZone.Id, bclZone.SupportsDaylightSavingTime, minOffset, maxOffset)
        {
            this.OriginalZone = bclZone;
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
            Preconditions.CheckNotNull(bclZone, nameof(bclZone));
            Offset standardOffset = bclZone.BaseUtcOffset.ToOffset();
            var rules = bclZone.GetAdjustmentRules();
            if (!bclZone.SupportsDaylightSavingTime || rules.Length == 0)
            {
                var fixedInterval = new ZoneInterval(bclZone.StandardName, Instant.BeforeMinValue, Instant.AfterMaxValue, standardOffset, Offset.Zero);
                return new BclDateTimeZone(bclZone, standardOffset, standardOffset, new SingleZoneIntervalMap(fixedInterval));
            }

            BclAdjustmentRule[] convertedRules = Array.ConvertAll(rules, rule => new BclAdjustmentRule(bclZone, rule));
            Offset minRuleOffset = convertedRules.Aggregate(Offset.MaxValue, (min, rule) => Offset.Min(min, rule.Savings + rule.StandardOffset));
            Offset maxRuleOffset = convertedRules.Aggregate(Offset.MinValue, (min, rule) => Offset.Max(min, rule.Savings + rule.StandardOffset));

            IZoneIntervalMap uncachedMap = BuildMap(convertedRules, standardOffset, bclZone.StandardName);
            IZoneIntervalMap cachedMap = CachingZoneIntervalMap.CacheMap(uncachedMap, CachingZoneIntervalMap.CacheType.Hashtable);
            return new BclDateTimeZone(bclZone, Offset.Min(standardOffset, minRuleOffset), Offset.Max(standardOffset, maxRuleOffset), cachedMap);
        }

        private static IZoneIntervalMap BuildMap(BclAdjustmentRule[] rules, Offset standardOffset, [NotNull] string standardName)
        {
            Preconditions.CheckNotNull(standardName, nameof(standardName));

            // First work out a naive list of partial maps. These will give the right offset at every instant, but not necessarily
            // correct intervals - we may we need to stitch intervals together.
            List<PartialZoneIntervalMap> maps = new List<PartialZoneIntervalMap>();
            // Handle the start of time until the start of the first rule, if necessary.
            if (rules[0].Start.IsValid)
            {
                maps.Add(PartialZoneIntervalMap.ForZoneInterval(standardName, Instant.BeforeMinValue, rules[0].Start, standardOffset, Offset.Zero));
            }
            for (int i = 0; i < rules.Length - 1; i++)
            {
                var beforeRule = rules[i];
                var afterRule = rules[i + 1];
                maps.Add(beforeRule.PartialMap);
                // If there's a gap between this rule and the next one, fill it with a fixed interval.
                if (beforeRule.End < afterRule.Start)
                {
                    maps.Add(PartialZoneIntervalMap.ForZoneInterval(standardName, beforeRule.End, afterRule.Start, standardOffset, Offset.Zero));
                }
            }

            var lastRule = rules[rules.Length - 1];
            maps.Add(lastRule.PartialMap);

            // Handle the end of the last rule until the end of time, if necessary.
            if (lastRule.End.IsValid)
            {
                maps.Add(PartialZoneIntervalMap.ForZoneInterval(standardName, lastRule.End, Instant.AfterMaxValue, standardOffset, Offset.Zero));
            }
            return PartialZoneIntervalMap.ConvertToFullMap(maps);
        }

        /// <summary>
        /// Just a mapping of a TimeZoneInfo.AdjustmentRule into Noda Time types. Very little cleverness here.
        /// </summary>
        private sealed class BclAdjustmentRule
        {
            private static readonly DateTime MaxDate = DateTime.MaxValue.Date;

            /// <summary>
            /// Instant on which this rule starts.
            /// </summary>
            internal Instant Start { get; }

            /// <summary>
            /// Instant on which this rule ends.
            /// </summary>
            internal Instant End { get; }

            /// <summary>
            /// Daylight savings, when applicable within this rule.
            /// </summary>
            internal Offset Savings { get; }

            /// <summary>
            /// The standard offset for the duration of this rule.
            /// </summary>
            internal Offset StandardOffset { get; }

            internal PartialZoneIntervalMap PartialMap { get; }

            internal BclAdjustmentRule(TimeZoneInfo zone, TimeZoneInfo.AdjustmentRule rule)
            {
                // With .NET 4.6, adjustment rules can have their own standard offsets, allowing
                // a much more reasonable set of time zone data. Unfortunately, this isn't directly
                // exposed, but we can detect it by just finding the UTC offset for an arbitrary
                // time within the rule - the start, in this case - and then take account of the
                // possibility of that being in daylight saving time. Fortunately, we only need
                // to do this during the setup.
                var ruleStandardOffset = zone.GetUtcOffset(rule.DateStart);
                if (zone.IsDaylightSavingTime(rule.DateStart))
                {
                    ruleStandardOffset -= rule.DaylightDelta;
                }
                StandardOffset = ruleStandardOffset.ToOffset();

                // Although the rule may have its own standard offset, the start/end is still determined
                // using the zone's standard offset.
                var zoneStandardOffset = zone.BaseUtcOffset.ToOffset();

                // Note: this extends back from DateTime.MinValue to start of time, even though the BCL can represent
                // as far back as 1AD. This is in the *spirit* of a rule which goes back that far.
                Start = rule.DateStart.ToLocalDateTime().WithOffset(zoneStandardOffset).ToInstant();
                // The end instant (exclusive) is the end of the given date, so we need to add a day.
                End = rule.DateEnd == MaxDate ? Instant.AfterMaxValue : rule.DateEnd.ToLocalDateTime().PlusDays(1).WithOffset(zoneStandardOffset).ToInstant();
                Savings = rule.DaylightDelta.ToOffset();

                // Some rules have DST start/end of "January 1st", to indicate that they're just in standard time. This is important
                // for rules which have a standard offset which is different to the standard offset of the zone itself.
                if (IsStandardOffsetOnlyRule(rule))
                {
                    PartialMap = PartialZoneIntervalMap.ForZoneInterval(zone.StandardName, Start, End, StandardOffset, Offset.Zero);
                }
                else
                {
                    var daylightRecurrence = new ZoneRecurrence(zone.DaylightName, Savings, ConvertTransition(rule.DaylightTransitionStart), int.MinValue, int.MaxValue);
                    var standardRecurrence = new ZoneRecurrence(zone.StandardName, Offset.Zero, ConvertTransition(rule.DaylightTransitionEnd), int.MinValue, int.MaxValue);
                    var recurringMap = new DaylightSavingsDateTimeZone("ignored", StandardOffset, standardRecurrence, daylightRecurrence);
                    PartialMap = new PartialZoneIntervalMap(Start, End, recurringMap);
                }
            }

            /// <summary>
            /// The BCL represents "standard-only" rules using two fixed date January 1st transitions.
            /// Currently the time-of-day used for the DST end transition is at one millisecond past midnight... we'll
            /// be slightly more lenient, accepting anything up to 12:01...
            /// </summary>
            private static bool IsStandardOffsetOnlyRule(TimeZoneInfo.AdjustmentRule rule)
            {
                var daylight = rule.DaylightTransitionStart;
                var standard = rule.DaylightTransitionEnd;
                return daylight.IsFixedDateRule && daylight.Day == 1 && daylight.Month == 1 &&
                       daylight.TimeOfDay.TimeOfDay < TimeSpan.FromMinutes(1) &&
                       standard.IsFixedDateRule && standard.Day == 1 && standard.Month == 1 &&
                       standard.TimeOfDay.TimeOfDay < TimeSpan.FromMinutes(1);
            }

            // Converts a TimeZoneInfo "TransitionTime" to a "ZoneYearOffset" - the two correspond pretty closely.
            private static ZoneYearOffset ConvertTransition(TimeZoneInfo.TransitionTime transitionTime)
            {
                // Used for both fixed and non-fixed transitions.
                LocalTime timeOfDay = LocalDateTime.FromDateTime(transitionTime.TimeOfDay).TimeOfDay;

                // Easy case - fixed day of the month.
                if (transitionTime.IsFixedDateRule)
                {
                    return new ZoneYearOffset(TransitionMode.Wall, transitionTime.Month, transitionTime.Day, 0, false, timeOfDay);
                }

                // Floating: 1st Sunday in March etc.
                int dayOfWeek = (int)BclConversions.ToIsoDayOfWeek(transitionTime.DayOfWeek);
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
                return new ZoneYearOffset(TransitionMode.Wall, transitionTime.Month, dayOfMonth, dayOfWeek, advance, timeOfDay);
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
            if (currentSystemDefault?.OriginalZone != local)
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
        public override int GetHashCode() => OriginalZone.GetHashCode();
    }
}
#endif
