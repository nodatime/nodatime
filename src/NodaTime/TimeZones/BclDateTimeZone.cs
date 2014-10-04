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

        private readonly TimeZoneInfo bclZone;
        private readonly IZoneIntervalMap map;

        /// <summary>
        /// Gets the original <see cref="TimeZoneInfo"/> from which this was created.
        /// </summary>
        /// <value>The original <see cref="TimeZoneInfo"/> from which this was created.</value>
        public TimeZoneInfo OriginalZone { get { return bclZone; } }

        /// <summary>
        /// Gets the display name associated with the time zone, as provided by the Base Class Library.
        /// </summary>
        /// <value>The display name associated with the time zone, as provided by the Base Class Library.</value>
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
            Offset standardOffset = bclZone.BaseUtcOffset.ToOffset();
            var rules = bclZone.GetAdjustmentRules();
            if (!bclZone.SupportsDaylightSavingTime || rules.Length == 0)
            {
                var fixedInterval = new ZoneInterval(bclZone.StandardName, Instant.BeforeMinValue, Instant.AfterMaxValue, standardOffset, Offset.Zero);
                return new BclDateTimeZone(bclZone, standardOffset, standardOffset, new FixedZoneIntervalMap(fixedInterval));
            }

            BclAdjustmentRule[] mappedRules = Array.ConvertAll(rules, rule => new BclAdjustmentRule(bclZone, rule));
            Offset minSavings = mappedRules.Aggregate(Offset.Zero, (min, rule) => Offset.Min(min, rule.Savings));
            Offset maxSavings = mappedRules.Aggregate(Offset.Zero, (min, rule) => Offset.Max(min, rule.Savings));

            IZoneIntervalMap uncachedMap = new BclZoneIntervalMap(mappedRules, standardOffset, bclZone.StandardName, bclZone.DaylightName);
            IZoneIntervalMap cachedMap = CachingZoneIntervalMap.CacheMap(uncachedMap, CachingZoneIntervalMap.CacheType.Hashtable);
            return new BclDateTimeZone(bclZone, standardOffset + minSavings, standardOffset + maxSavings, cachedMap);
        }

        /// <summary>
        /// Just a mapping of a TimeZoneInfo.AdjustmentRule into Noda Time types. Very little cleverness here.
        /// </summary>
        private sealed class BclAdjustmentRule
        {
            private static readonly DateTime MaxDate = DateTime.MaxValue.Date;

            // The first instant on which this rule takes effect.
            private readonly Instant start;
            // The instant on which this rule expires.
            private readonly Instant end;
            private readonly Offset savings; // Do we need this?
            private readonly IZoneIntervalMap intervalMap;

            private readonly ZoneInterval headInterval;
            private readonly ZoneInterval tailInterval;

            /// <summary>
            /// Instant on which this rule starts.
            /// </summary>
            internal Instant Start { get { return start; } }

            /// <summary>
            /// Instant on which this rule ends.
            /// </summary>
            internal Instant End { get { return end; } }

            /// <summary>
            /// Daylight savings, when applicable within this rule.
            /// </summary>
            internal Offset Savings { get { return savings; } }

            /// <summary>
            /// The ZoneInterval at the start of this rule, clamped to start at the start of this rule.
            /// </summary>
            internal ZoneInterval HeadInterval { get { return headInterval; } }

            /// <summary>
            /// The ZoneInterval at the end of this rule, clamped to end at the end of this rule.
            /// </summary>
            internal ZoneInterval TailInterval { get { return tailInterval; } }

            internal BclAdjustmentRule(TimeZoneInfo zone, TimeZoneInfo.AdjustmentRule rule)
            {
                var standardOffset = zone.BaseUtcOffset.ToOffset();
                // Note: this extend back from DateTime.MinValue to start of time, even though the BCL can represent
                // as far back as 1AD. This is in the *spirit* of a rule which goes back that far.
                start = rule.DateStart.ToLocalDateTime().WithOffset(standardOffset).ToInstant();
                // The end instant (exclusive) is the end of the given date, so we need to add a day.
                end = rule.DateEnd == MaxDate ? Instant.AfterMaxValue : rule.DateEnd.ToLocalDateTime().PlusDays(1).WithOffset(standardOffset).ToInstant();
                savings = rule.DaylightDelta.ToOffset();
                var daylightRecurrence = new ZoneRecurrence(zone.DaylightName, savings, ConvertTransition(rule.DaylightTransitionStart), int.MinValue, int.MaxValue);
                var standardRecurrence = new ZoneRecurrence(zone.StandardName, Offset.Zero, ConvertTransition(rule.DaylightTransitionEnd), int.MinValue, int.MaxValue);
                intervalMap = new DaylightSavingsDateTimeZone("ignored", standardOffset, standardRecurrence, daylightRecurrence);

                headInterval = intervalMap.GetZoneInterval(start.IsValid ? start : Instant.MinValue).WithStart(start);
                tailInterval = intervalMap.GetZoneInterval(end.IsValid ? end - Duration.Epsilon : Instant.MaxValue).WithEnd(end);
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
                return new ZoneYearOffset(TransitionMode.Wall, transitionTime.Month, dayOfMonth, dayOfWeek, advance, timeOfDay);
            }

            /// <summary>
            /// Creates a partial zone interval map covering the period between the end of the
            /// head interval and the start of the tail interval - unless the rule extends to the start/end
            /// of time, in which case the partial zone interval map will do so too.
            /// </summary>
            internal PartialZoneIntervalMap ToPartialZoneIntervalMap()
            {
                return new PartialZoneIntervalMap(
                    start.IsValid ? headInterval.End : Instant.BeforeMinValue,
                    end.IsValid ? tailInterval.Start : Instant.AfterMaxValue,
                    intervalMap);
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

        private sealed class PartialZoneIntervalMap
        {
            private readonly Instant start;
            private readonly Instant end;
            private readonly IZoneIntervalMap map;

            /// <summary>
            /// Start of the interval during which this map is valid.
            /// </summary>
            internal Instant Start { get { return start; } }

            /// <summary>
            /// End (exclusive) of the interval during which this map is valid.
            /// </summary>
            internal Instant End { get { return end; } }

            internal PartialZoneIntervalMap(Instant start, Instant end, IZoneIntervalMap map)
            {
                // Allowing empty maps makes life simpler.
                Preconditions.DebugCheckArgument(start <= end, "end",
                    "Invalid start/end combination: {0} - {1}", start, end);
                this.start = start;
                this.end = end;
                this.map = map;
            }

            internal static PartialZoneIntervalMap ForZoneInterval(ZoneInterval interval)
            {
                return new PartialZoneIntervalMap(interval.RawStart, interval.RawEnd, new FixedZoneIntervalMap(interval));
            }

            internal ZoneInterval GetZoneInterval(Instant instant)
            {
                Preconditions.DebugCheckArgument(instant >= start && instant < end, "instant",
                    "Value {0} was not in the range [{0}, {1})", instant, start, end);
                return map.GetZoneInterval(instant);
            }
        }

        /// <summary>
        /// The core part of working out zone intervals; separated into its own type to allow for caching. This type is not heavily
        /// optimized - it is expected that the cache will do the heavy lifting. Correctness in the face of extreme values (and odd rules)
        /// is much more important. There's nothing specific post-construction here... it could be extracted
        /// into a more general place if that would be useful.
        /// </summary>
        private sealed class BclZoneIntervalMap : IZoneIntervalMap
        {
            private readonly PartialZoneIntervalMap[] partialMaps;

            internal BclZoneIntervalMap(BclAdjustmentRule[] rules, Offset standardOffset, string standardName, string daylightName)
            {
                List<PartialZoneIntervalMap> maps = new List<PartialZoneIntervalMap>();
                if (rules[0].Start.IsValid)
                {
                    var ruleHead = rules[0].HeadInterval;
                    var timeHead = new ZoneInterval(standardName, Instant.BeforeMinValue, rules[0].Start, standardOffset, Offset.Zero);
                    maps.AddRange(CoalesceIntervals(timeHead, ruleHead));
                }
                for (int i = 0; i < rules.Length - 1; i++)
                {
                    var beforeRule = rules[i];
                    var afterRule = rules[i + 1];
                    maps.Add(beforeRule.ToPartialZoneIntervalMap());
                    // ZoneInterval can't have start == end, which makes some sense even though it would be handy here...
                    // Basically, we may need to add in another standard time interval between the rules if they don't abut.
                    if (beforeRule.End < afterRule.Start)
                    {
                        maps.AddRange(CoalesceIntervals(beforeRule.TailInterval,
                            new ZoneInterval(standardName, beforeRule.End, afterRule.Start, standardOffset, Offset.Zero),
                        afterRule.HeadInterval));
                    }
                    else
                    {
                        maps.AddRange(CoalesceIntervals(beforeRule.TailInterval, afterRule.HeadInterval));
                    }
                }
                var lastRule = rules[rules.Length - 1];
                maps.Add(lastRule.ToPartialZoneIntervalMap());
                if (lastRule.End.IsValid)
                {
                    var ruleTail = lastRule.TailInterval;
                    var timeTail = new ZoneInterval(standardName, lastRule.End, Instant.AfterMaxValue, standardOffset, Offset.Zero);
                    maps.AddRange(CoalesceIntervals(ruleTail, timeTail));
                }
                this.partialMaps = maps.ToArray();
            }

            // Given some zone intervals, coalesce abutting ones if they have the same offset, and return a matching
            // sequence of PartialZoneIntervalMaps containing them.
            private static IEnumerable<PartialZoneIntervalMap> CoalesceIntervals(params ZoneInterval[] intervals)
            {
                var current = intervals[0];
                for (int i = 1; i < intervals.Length; i++)
                {
                    var candidate = intervals[i];
                    Preconditions.DebugCheckArgument(candidate.RawStart == current.RawEnd, "intervals", "Intervals should abut; {0} and {1} don't.", current, candidate);
                    if (current.WallOffset == candidate.WallOffset)
                    {
                        current = current.WithEnd(candidate.RawEnd);
                    }
                    else
                    {
                        yield return PartialZoneIntervalMap.ForZoneInterval(current);
                        current = candidate;
                    }
                }
                yield return PartialZoneIntervalMap.ForZoneInterval(current);
            }

            public ZoneInterval GetZoneInterval(Instant instant)
            {
                // We assume the maps are ordered, and start with "beginning of time"
                // which means we only need to find the first partial map which ends after
                // the instant we're interested in. This is just a linear search - a binary search
                // would be feasible, but we're not expecting very many entries.
                foreach (var partialMap in partialMaps)
                {
                    if (instant < partialMap.End)
                    {
                        return partialMap.GetZoneInterval(instant);
                    }
                }
                throw new InvalidOperationException("Instant not contained in any map");
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
