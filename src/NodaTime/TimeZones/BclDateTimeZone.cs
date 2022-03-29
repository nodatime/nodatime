// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Annotations;
using NodaTime.Extensions;
using NodaTime.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Representation of a time zone converted from a <see cref="TimeZoneInfo"/> from the Base Class Library.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Two instances of this class are deemed equal if and only if they refer to the exact same
    /// <see cref="TimeZoneInfo"/> object.
    /// </para>
    /// <para>
    /// This implementation does not always give the same results as <c>TimeZoneInfo</c>, in that it doesn't replicate
    /// the bugs in the BCL interpretation of the data. These bugs are described in
    /// <a href="https://codeblog.jonskeet.uk/2014/09/30/the-mysteries-of-bcl-time-zone-data/">a blog post</a>, but we're
    /// not expecting them to be fixed any time soon. Being bug-for-bug compatible would not only be tricky, but would be painful
    /// if the BCL were ever to be fixed. As far as we are aware, there are only discrepancies around new year where the zone
    /// changes from observing one rule to observing another.
    /// </para>
    /// <para>
    /// As of version 3.0, a new "incompatible but doing the right thing" category of differences has been implemented,
    /// for time zones which have a transition at 24:00. The Windows time zone data represents this as a transition at 23:59:59.999,
    /// and that's faithfully represented by TimeZoneInfo (and BclDateTimeZone in version 2.x). As of 3.0, this is spotted
    /// and converted to a midnight-on-the-following-day transition.
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
        private static BclDateTimeZone? systemDefault;

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

        private BclDateTimeZone(TimeZoneInfo bclZone, IZoneIntervalMap map)
            : base(bclZone.Id, bclZone.SupportsDaylightSavingTime, map.MinOffset, map.MaxOffset)
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
        public static BclDateTimeZone FromTimeZoneInfo(TimeZoneInfo bclZone)
        {
            Preconditions.CheckNotNull(bclZone, nameof(bclZone));
            Offset standardOffset = bclZone.BaseUtcOffset.ToOffset();
            var rules = bclZone.GetAdjustmentRules();
            if (!bclZone.SupportsDaylightSavingTime || rules.Length == 0)
            {
                var fixedInterval = new ZoneInterval(bclZone.StandardName, Instant.BeforeMinValue, Instant.AfterMaxValue, standardOffset, Offset.Zero);
                return new BclDateTimeZone(bclZone, new SingleZoneIntervalMap(fixedInterval));
            }

            BclAdjustmentRule[] convertedRules;
            if (AreWindowsStyleRules(rules))
            {
                convertedRules = Array.ConvertAll(rules, rule => BclAdjustmentRule.FromWindowsAdjustmentRule(bclZone, rule));
            }
            else
            {
                convertedRules = Array.ConvertAll(rules, rule => BclAdjustmentRule.FromUnixAdjustmentRule(bclZone, rule));
                FixUnixTransitions(convertedRules);
            }

            IZoneIntervalMap uncachedMap = BuildMap(convertedRules, standardOffset, bclZone.StandardName);
            IZoneIntervalMap cachedMap = CachingZoneIntervalMap.CacheMap(uncachedMap);

            return new BclDateTimeZone(bclZone, cachedMap);
        }

        /// <summary>
        /// .NET Core on Unix adjustment rules can't currently be treated like regular Windows ones.
        /// Instead of dividing time into periods by year, the rules are read from TZIF files, so are like
        /// our PrecalculatedDateTimeZone. This is only visible for testing purposes.
        /// </summary>
        internal static bool AreWindowsStyleRules(TimeZoneInfo.AdjustmentRule[] rules)
        {
            int windowsRules = rules.Count(IsWindowsRule);
            return windowsRules == rules.Length;

            bool IsWindowsRule(TimeZoneInfo.AdjustmentRule rule) =>
                rule.DateStart.Month == 1 && rule.DateStart.Day == 1 && rule.DateStart.TimeOfDay.Ticks == 0 &&
                rule.DateEnd.Month == 12 && rule.DateEnd.Day == 31 && rule.DateEnd.TimeOfDay.Ticks == 0 &&
                // In .NET 6.0 on Linux, some zones (e.g. Pacific/Wallis) conform to the above, but also have
                // years that are earlier than we'd ever expect to see on Windows.
                (rule.DateStart.Year == 1 || rule.DateStart.Year > 1600);
        }

        /// <summary>
        /// The Unix rules are sometimes either slightly disjoint, or overlap. Ideally, we should be able to remove
        /// (or at least understand the need for) this code, but until then, it seems to make all the tests pass.
        /// </summary>
        internal static void FixUnixTransitions(BclAdjustmentRule[] rules)
        {
            for (int i = 0; i < rules.Length - 1; i++)
            {
                // If this rule ends after the next one starts, i.e. they overlap,
                // truncate this rule's end time.
                // Examples of when this is needed, in .NET 6 (on Linux):
                // - Antarctica/Macquarie at the end of 2009
                // - Europe/Dublin at the end of 2019
                // - Europe/Prague at the end of 1946
                if (rules[i].End > rules[i + 1].Start)
                {
                    // TODO: add a check that the difference is just DST.
                    rules[i] = rules[i].WithEnd(rules[i + 1].Start);
                }
                // If this rule ends before the next one starts, i.e. there's a gap,
                // and if that gap is the same length as DST, then bring forward the
                // next rule by that DST gap (in other words, treat it as starting in DST instead
                // of in standard time). This is sometimes needed for .NET 6 rules that are
                // on year boundaries: there are two rules where the first one ends at the end of the year
                // and the second one starts at the start of the next year, but they both have the same offset.
                // Examples of when this is needed, in .NET 6 (on Linux):
                // - America/Sao_Paolo at the end of 2017
                // - Antarctica/Macquarie at the end of 2008
                // - America/Creston at the end of 1943
                else if (rules[i].End < rules[i + 1].Start && rules[i].End.PlusNanoseconds(rules[i + 1].Savings.Nanoseconds) == rules[i + 1].Start)
                {
                    rules[i + 1] = rules[i + 1].WithStart(rules[i].End);
                }
            }
        }

        // Visible for testing
        internal static IZoneIntervalMap BuildMap(BclAdjustmentRule[] rules, Offset standardOffset, string standardName)
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
        [VisibleForTesting]
        internal sealed class BclAdjustmentRule
        {
            private static readonly DateTime MaxDate = DateTime.MaxValue.Date;

            /// <summary>
            /// Instant on which this rule starts.
            /// </summary>
            internal Instant Start => PartialMap.Start;

            /// <summary>
            /// Instant on which this rule ends.
            /// </summary>
            internal Instant End => PartialMap.End;

            /// <summary>
            /// Daylight savings, when applicable within this rule.
            /// </summary>
            internal Offset Savings { get; }

            /// <summary>
            /// The standard offset for the duration of this rule.
            /// </summary>
            internal Offset StandardOffset { get; }

            internal PartialZoneIntervalMap PartialMap { get; }

            // Visible for tests
            internal BclAdjustmentRule(ZoneInterval zoneInterval)
            {
                StandardOffset = zoneInterval.StandardOffset;
                Savings = zoneInterval.Savings;
                PartialMap = PartialZoneIntervalMap.ForZoneInterval(zoneInterval);
            }

            private BclAdjustmentRule(Offset standardOffset, Offset savings, PartialZoneIntervalMap partialMap)
            {
                StandardOffset = standardOffset;
                Savings = savings;
                PartialMap = partialMap;
            }
            internal BclAdjustmentRule WithStart(Instant newStart) =>
                new BclAdjustmentRule(StandardOffset, Savings, PartialMap.WithStart(newStart));

            internal BclAdjustmentRule WithEnd(Instant newEnd) =>
                new BclAdjustmentRule(StandardOffset, Savings, PartialMap.WithEnd(newEnd));

            internal static BclAdjustmentRule FromUnixAdjustmentRule(TimeZoneInfo zone, TimeZoneInfo.AdjustmentRule rule)
            {
                // In .NET 6.0 (and onwards, presumably) the final rule is an alternating standard/daylight rule instead
                // of a single zone interval. We can handle it as we do rules on Windows.
                if (!rule.DaylightTransitionStart.IsFixedDateRule || !rule.DaylightTransitionEnd.IsFixedDateRule)
                {
                    return FromWindowsAdjustmentRule(zone, rule);
                }

                // This logic is also performed in the method below, but it's hard to remove that duplication
                // without also making testing harder. (When everything's working, we might refactor.)
                DateTime ruleStartLocal = rule.DateStart + rule.DaylightTransitionStart.TimeOfDay.TimeOfDay;
                DateTime ruleStartUtc = DateTime.SpecifyKind(ruleStartLocal.Year == 1 ? DateTime.MinValue : ruleStartLocal - zone.BaseUtcOffset, DateTimeKind.Utc);
                var forceDaylight = zone.IsDaylightSavingTime(ruleStartUtc) && rule.DaylightDelta == TimeSpan.Zero;
#if NET6_0_OR_GREATER
                var ruleStandardOffset = zone.BaseUtcOffset + rule.BaseUtcOffsetDelta;
#else
                var ruleStandardOffset = zone.GetUtcOffset(ruleStartUtc);
                if (zone.IsDaylightSavingTime(ruleStartUtc))
                {
                    ruleStandardOffset -= rule.DaylightDelta;
                }
#endif
                return ConvertUnixRuleToBclAdjustmentRule(rule, zone.StandardName, zone.DaylightName, zone.BaseUtcOffset, ruleStandardOffset, forceDaylight);                
            }

            [VisibleForTesting]
            internal static BclAdjustmentRule ConvertUnixRuleToBclAdjustmentRule(TimeZoneInfo.AdjustmentRule rule,
                string standardName, string daylightName, TimeSpan zoneStandardOffset, TimeSpan ruleStandardOffset,
                bool forceDaylightSavings)
            {
                // On .NET Core on Unix, each "adjustment rule" is effectively just a zone interval. The transitions are only used
                // to give the time of day values to combine with rule.DateStart and rule.DateEnd. It's all a bit odd.
                // The *last* adjustment rule internally can work like a normal Windows standard/daylight rule, but that's only
                // exposed in .NET 6.0, and those rules are handled in FromUnixAdjustmentRule.
                // (Currently we don't have a way of handling a fixed-date rule to the end of time that really represents alternating
                // standard/daylight. Apparently that's not an issue.)

                // The start of each rule is indicated by the start date with the time-of-day of the transition, interpreted as being in the *zone* standard offset
                // (rather than the *rule* standard offset). Some rules effectively start in daylight time, but only when there are consecutive daylight time
                // rules. This is handled in FixOverlappingUnixRules.

                var bclLocalStart = rule.DateStart + rule.DaylightTransitionStart.TimeOfDay.TimeOfDay;
                var bclUtcStart = DateTime.SpecifyKind(bclLocalStart == DateTime.MinValue ? DateTime.MinValue : bclLocalStart - zoneStandardOffset, DateTimeKind.Utc);
                var bclSavings = rule.DaylightDelta;

                // The end of each rule is indicated by the start date with the time-of-day of the transition, interpreted as being in the *zone* standard offset
                // with the *rule* daylight delta.
                var bclLocalEnd = rule.DateEnd + rule.DaylightTransitionEnd.TimeOfDay.TimeOfDay;
                var bclUtcEnd = DateTime.SpecifyKind(rule.DateEnd == MaxDate ? DateTime.MaxValue : bclLocalEnd - (zoneStandardOffset + bclSavings), DateTimeKind.Utc);

                // For just a couple of time zones in .NET 6, there are adjustment rules which appear to be invalid
                // in the normal expectation of "starts in standard, ends in daylight".
                // Example in America/Creston: 1944-01-01 - 1944-01-01: Daylight delta: +01; DST starts January 01 at 00:00:00 and ends January 01 at 00:00:59.999
                // Handle this by treating the rule as starting in daylight time.
                if (bclUtcStart >= bclUtcEnd)
                {
                    bclUtcStart -= bclSavings;
                }

                // If the zone says that the start of the rule is in DST, but the rule has no daylight savings,
                // assume we actually want an hour of DST but one less hour of standard offset.
                // See Europe/Dublin in 1960 for example, in .NET 6:
                // 1960-04-10 - 1960-10-02: Daylight delta: +00; DST starts April 10 at 03:00:00 and ends October 02 at 02:59:59.999 (force daylight)
                if (forceDaylightSavings)
                {
                    bclSavings = TimeSpan.FromHours(1);
                    ruleStandardOffset -= bclSavings;
                }

                // Handle changes crossing the international date line, which used to be represented as savings of +/-23
                // hours (but could conceivably be more).
                // Note: I can't currently reproduce this in .NET Core 3.1 or .NET 6. It may be a legacy Mono artifact;
                // it does no harm to preserve it, however.
                if (bclSavings.Hours < -14)
                {
                    bclSavings += TimeSpan.FromDays(1);
                }
                else if (bclSavings.Hours > 14)
                {
                    bclSavings -= TimeSpan.FromDays(1);
                }

                // Now all the values are sensible - and in particular, now the daylight savings are in a range that can be represented by
                // Offset - we can converted everything to Noda Time types.
                var nodaStart = bclUtcStart == DateTime.MinValue ? Instant.BeforeMinValue : bclUtcStart.ToInstant();
                // The representation returned to us (not the internal representation) has an end point one second (before .NET 6)
                // or one millisecond (.NET 6 onwards) before the transition. We round up to a properly exclusive end instant.
                var endTimeCompensation = Duration.FromSeconds(1) - Duration.FromMilliseconds(bclLocalEnd.Millisecond);

                var nodaEnd = bclUtcEnd == DateTime.MaxValue ? Instant.AfterMaxValue : bclUtcEnd.ToInstant() + endTimeCompensation;
                var nodaStandard = ruleStandardOffset.ToOffset();
                var nodaSavings = bclSavings.ToOffset();
                var nodaWallOffset = nodaStandard + nodaSavings;

                var zoneInterval = new ZoneInterval(nodaSavings == Offset.Zero ? standardName : daylightName, nodaStart, nodaEnd, nodaWallOffset, nodaSavings);
                return new BclAdjustmentRule(zoneInterval);
            }

            internal static BclAdjustmentRule FromWindowsAdjustmentRule(TimeZoneInfo zone, TimeZoneInfo.AdjustmentRule rule)
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
                var standardOffset = ruleStandardOffset.ToOffset();

                // Although the rule may have its own standard offset, the start/end is still determined
                // using the zone's standard offset.
                var zoneStandardOffset = zone.BaseUtcOffset.ToOffset();

                // Note: this extends back from DateTime.MinValue to start of time, even though the BCL can represent
                // as far back as 1AD. This is in the *spirit* of a rule which goes back that far.
                var start = rule.DateStart == DateTime.MinValue ? Instant.BeforeMinValue : rule.DateStart.ToLocalDateTime().WithOffset(zoneStandardOffset).ToInstant();
                // The end instant (exclusive) is the end of the given date, so we need to add a day.
                var end = rule.DateEnd == MaxDate ? Instant.AfterMaxValue : rule.DateEnd.ToLocalDateTime().PlusDays(1).WithOffset(zoneStandardOffset).ToInstant();
                var savings = rule.DaylightDelta.ToOffset();

                PartialZoneIntervalMap partialMap;
                // Some rules have DST start/end of "January 1st", to indicate that they're just in standard time. This is important
                // for rules which have a standard offset which is different to the standard offset of the zone itself.
                if (IsStandardOffsetOnlyRule(rule))
                {
                    partialMap = PartialZoneIntervalMap.ForZoneInterval(zone.StandardName, start, end, standardOffset, Offset.Zero);
                }
                else
                {
                    var daylightRecurrence = new ZoneRecurrence(zone.DaylightName, savings, ConvertTransition(rule.DaylightTransitionStart), int.MinValue, int.MaxValue);
                    var standardRecurrence = new ZoneRecurrence(zone.StandardName, Offset.Zero, ConvertTransition(rule.DaylightTransitionEnd), int.MinValue, int.MaxValue);
                    IZoneIntervalMap recurringMap = new StandardDaylightAlternatingMap(standardOffset, standardRecurrence, daylightRecurrence);
                    // Fake 1 hour savings if the adjustment rule claims to be 0 savings. See DaylightFakingZoneIntervalMap documentation below for more details.
                    if (savings == Offset.Zero)
                    {
                        recurringMap = new DaylightFakingZoneIntervalMap(recurringMap, zone.DaylightName);
                    }
                    partialMap = new PartialZoneIntervalMap(start, end, recurringMap);
                }
                return new BclAdjustmentRule(standardOffset, savings, partialMap);
            }

            /// <summary>
            /// An implementation of IZoneIntervalMap that delegates to an original map, except for where the result of a
            /// ZoneInterval lookup has the given daylight name. In that case, a new ZoneInterval is built with the same
            /// wall offset (and start/end instants etc), but with a savings of 1 hour. This is only used to work around TimeZoneInfo
            /// adjustment rules with a daylight saving of 0 which are really trying to fake a more comprehensive solution.
            /// (This is currently only seen on Mono on Linux...)
            /// This addresses https://github.com/nodatime/nodatime/issues/746.
            /// If TimeZoneInfo had sufficient flexibility to use different names for different periods of time, we'd have
            /// another problem, as some "daylight names" don't always mean daylight - e.g. "BST" = British Summer Time and British Standard Time.
            /// In this case, the limited nature of TimeZoneInfo works in our favour.
            /// </summary>
            private sealed class DaylightFakingZoneIntervalMap : IZoneIntervalMap
            {
                private readonly IZoneIntervalMap originalMap;
                private readonly string daylightName;

                public Offset MinOffset => originalMap.MinOffset;
                public Offset MaxOffset => originalMap.MaxOffset;

                internal DaylightFakingZoneIntervalMap(IZoneIntervalMap originalMap, string daylightName)
                {
                    this.originalMap = originalMap;
                    this.daylightName = daylightName;
                }

                public ZoneInterval GetZoneInterval(Instant instant)
                {
                    var interval = originalMap.GetZoneInterval(instant);
                    return interval.Name == daylightName
                        ? new ZoneInterval(daylightName, interval.RawStart, interval.RawEnd, interval.WallOffset, Offset.FromHours(1))
                        : interval;
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

            private static readonly LocalTime OneMillisecondBeforeMidnight = new LocalTime(23, 59, 59, 999);

            // Converts a TimeZoneInfo "TransitionTime" to a "ZoneYearOffset" - the two correspond pretty closely.
            private static ZoneYearOffset ConvertTransition(TimeZoneInfo.TransitionTime transitionTime)
            {
                // Used for both fixed and non-fixed transitions.
                LocalTime timeOfDay = LocalDateTime.FromDateTime(transitionTime.TimeOfDay).TimeOfDay;

                // Transitions at midnight are represented in the Windows database by a transition one millisecond early.
                // See BclDateTimeZoneTest.TransitionAtMidnight for a concrete example.
                // We adjust to midnight to represent the correct data - it's clear this is just a data fudge.
                // It's probably done like this to allow the rule to represent "Saturday 24:00" instead of "Sunday 00:00".
                bool addDay = false;
                if (timeOfDay == OneMillisecondBeforeMidnight)
                {
                    timeOfDay = LocalTime.Midnight;
                    addDay = true;
                }

                // Easy case - fixed day of the month.
                if (transitionTime.IsFixedDateRule)
                {
                    return new ZoneYearOffset(TransitionMode.Wall, transitionTime.Month, transitionTime.Day, 0, false, timeOfDay, addDay);
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
                return new ZoneYearOffset(TransitionMode.Wall, transitionTime.Month, dayOfMonth, dayOfWeek, advance, timeOfDay, addDay);
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
        /// <exception cref="InvalidOperationException">The system does not provide a time zone.</exception>
        /// <returns>A <see cref="BclDateTimeZone"/> wrapping the "local" (system) time zone as returned by
        /// <see cref="TimeZoneInfo.Local"/>.</returns>
        public static BclDateTimeZone ForSystemDefault()
        {
            TimeZoneInfo? local = TimeZoneInfoInterceptor.Local;
            if (local is null)
            {
                throw new InvalidOperationException("No system default time zone is available");
            }
            BclDateTimeZone? currentSystemDefault = systemDefault;

            // Cached copy is out of date - wrap a new one.
            // If currentSystemDefault is null, we always enter this block (as local isn't null).
            if (currentSystemDefault?.OriginalZone != local)
            {
                currentSystemDefault = FromTimeZoneInfo(local);
                systemDefault = currentSystemDefault;
            }
            // Always return our local variable; the field may have changed again.
            return currentSystemDefault;
        }
    }
}
