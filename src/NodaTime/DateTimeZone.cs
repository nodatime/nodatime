// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// Represents a time zone - a mapping between UTC and local time. A time zone maps UTC instants to local times
    ///  - or, equivalently, to the offset from UTC at any particular instant.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The mapping is unambiguous in the "UTC to local" direction, but
    /// the reverse is not true: when the offset changes, usually due to a Daylight Saving transition,
    /// the change either creates a gap (a period of local time which never occurs in the time zone)
    /// or an ambiguity (a period of local time which occurs twice in the time zone). Mapping back from
    /// local time to an instant requires consideration of how these problematic times will be handled.
    /// </para>
    /// <para>
    /// Noda Time provides various options when mapping local time to a specific instant:
    /// <list type="bullet">
    ///   <item>
    ///     <description><see cref="AtStrictly"/> will throw an exception if the mapping from local time is either ambiguous
    ///     or impossible, i.e. if there is anything other than one instant which maps to the given local time.</description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="AtLeniently"/> will never throw an exception due to ambiguous or skipped times,
    ///     resolving to the earlier option of ambiguous matches, or to a value that's forward-shifted by the duration
    ///     of the gap for skipped times.</description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="ResolveLocal(LocalDateTime, ZoneLocalMappingResolver)"/> will apply a <see cref="ZoneLocalMappingResolver"/> to the result of
    ///     a mapping.</description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="MapLocal"/> will return a <see cref="ZoneLocalMapping"/>
    ///     with complete information about whether the given local time occurs zero times, once or twice. This is the most
    ///     fine-grained approach, which is the fiddliest to use but puts the caller in the most control.</description>
    ///   </item>
    /// </list>
    /// </para>
    /// <para>
    /// Noda Time has two built-in sources of time zone data available: a copy of the
    /// <a href="http://www.iana.org/time-zones">tz database</a> (also known as the IANA Time Zone database, or zoneinfo
    /// or Olson database), and the ability to convert .NET's own <see cref="TimeZoneInfo"/> format into a "native" Noda
    /// Time zone. Which of these is most appropriate for you to use will very much depend on your exact needs. The
    /// zoneinfo database is widely used outside Windows, and has more historical data than the Windows-provided
    /// information, but if you need to interoperate with other Windows systems by specifying time zone IDs, you may
    /// wish to stick to the Windows time zones.
    /// </para>
    /// <para>
    /// To obtain a <see cref="DateTimeZone"/> for a given timezone ID, use one of the methods on
    /// <see cref="IDateTimeZoneProvider"/> (and see <see cref="DateTimeZoneProviders"/> for access to the built-in
    /// providers). The UTC timezone is also available via the <see cref="Utc"/> property on this class.
    /// </para>
    /// <para>
    /// To obtain a <see cref="DateTimeZone"/> representing the system default time zone, you can either call
    /// <see cref="IDateTimeZoneProvider.GetSystemDefault"/> on a provider to obtain the <see cref="DateTimeZone"/> that
    /// the provider considers matches the system default time zone, or you can construct a
    /// <c>BclDateTimeZone</c> via <c>BclDateTimeZone.ForSystemDefault</c>, which returns a
    /// <see cref="DateTimeZone"/> that wraps the system local <see cref="TimeZoneInfo"/>. The latter will always
    /// succeed, but has access only to that information available via the .NET time zone; the former may contain more
    /// complete data, but may (in uncommon cases) fail to find a matching <see cref="DateTimeZone"/>.
    /// Note that <c>BclDateTimeZone</c> is not available on the .NET Standard 1.3 build of Noda Time, so this fallback strategy can
    /// only be used with the desktop version.
    /// </para>
    /// <para>
    /// Note that Noda Time does not require that <see cref="DateTimeZone"/> instances be singletons.
    /// Comparing two time zones for equality is not straightforward: if you care about whether two
    /// zones act the same way within a particular portion of time, use <see cref="ZoneEqualityComparer"/>.
    /// Additional guarantees are provided by <see cref="IDateTimeZoneProvider"/> and <see cref="ForOffset(Offset)"/>.
    /// </para>
    /// </remarks>
    /// <threadsafety>
    /// All time zone implementations within Noda Time are immutable and thread-safe.
    /// See the thread safety section of the user guide for more information.
    /// It is expected that third party implementations will be immutable and thread-safe as well:
    /// code within Noda Time assumes that it can hand out time zones to any thread without any concerns. If you
    /// implement a non-thread-safe time zone, you will need to use it extremely carefully. We'd recommend that you
    /// avoid this if possible.
    /// </threadsafety>
    [Immutable]
    public abstract class DateTimeZone : IZoneIntervalMapWithMinMax
    {
        /// <summary>
        /// The ID of the UTC (Coordinated Universal Time) time zone. This ID is always valid, whatever provider is
        /// used. If the provider has its own mapping for UTC, that will be returned by <see cref="DateTimeZoneCache.GetZoneOrNull" />, but otherwise
        /// the value of the <see cref="Utc"/> property will be returned.
        /// </summary>
        internal const string UtcId = "UTC";

        /// <summary>
        /// Gets the UTC (Coordinated Universal Time) time zone.
        /// </summary>
        /// <remarks>
        /// This is a single instance which is not provider-specific; it is guaranteed to have the ID "UTC", and to
        /// compare equal to an instance returned by calling <see cref="ForOffset"/> with an offset of zero, but it may
        /// or may not compare equal to an instance returned by e.g. <c>DateTimeZoneProviders.Tzdb["UTC"]</c>.
        /// </remarks>
        /// <value>A UTC <see cref="T:NodaTime.DateTimeZone" />.</value>
        [NotNull] public static DateTimeZone Utc { get; } = new FixedDateTimeZone(Offset.Zero);
        private const int FixedZoneCacheGranularitySeconds = NodaConstants.SecondsPerMinute * 30;
        private const int FixedZoneCacheMinimumSeconds = -FixedZoneCacheGranularitySeconds * 12 * 2; // From UTC-12
        private const int FixedZoneCacheSize = (12 + 15) * 2 + 1; // To UTC+15 inclusive
        private static readonly DateTimeZone[] FixedZoneCache = BuildFixedZoneCache();

        /// <summary>
        /// Returns a fixed time zone with the given offset.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The returned time zone will have an ID of "UTC" if the offset is zero, or "UTC+/-Offset"
        /// otherwise. In the former case, the returned instance will be equal to <see cref="Utc"/>.
        /// </para>
        /// <para>
        /// Note also that this method is not required to return the same <see cref="DateTimeZone"/> instance for
        /// successive requests for the same offset; however, all instances returned for a given offset will compare
        /// as equal.
        /// </para>
        /// </remarks>
        /// <param name="offset">The offset for the returned time zone</param>
        /// <returns>A fixed time zone with the given offset.</returns>
        [NotNull] public static DateTimeZone ForOffset(Offset offset)
        {
            int seconds = offset.Seconds;
            if (seconds % FixedZoneCacheGranularitySeconds != 0)
            {
                return new FixedDateTimeZone(offset);
            }
            int index = (seconds - FixedZoneCacheMinimumSeconds) / FixedZoneCacheGranularitySeconds;
            if (index < 0 || index >= FixedZoneCacheSize)
            {
                return new FixedDateTimeZone(offset);
            }
            return FixedZoneCache[index];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NodaTime.DateTimeZone" /> class.
        /// </summary>
        /// <param name="id">The unique id of this time zone.</param>
        /// <param name="isFixed">Set to <c>true</c> if this time zone has no transitions.</param>
        /// <param name="minOffset">Minimum offset applied within this zone</param>
        /// <param name="maxOffset">Maximum offset applied within this zone</param>
        protected DateTimeZone([NotNull] string id, bool isFixed, Offset minOffset, Offset maxOffset)
        {
            this.Id = Preconditions.CheckNotNull(id, nameof(id));
            this.IsFixed = isFixed;
            this.MinOffset = minOffset;
            this.MaxOffset = maxOffset;
        }

        /// <summary>
        /// Get the provider's ID for the time zone.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This identifies the time zone within the current time zone provider; a different provider may
        /// provide a different time zone with the same ID, or may not provide a time zone with that ID at all.
        /// </para>
        /// </remarks>
        /// <value>The provider's ID for the time zone.</value>
        [NotNull] public string Id { get; }

        /// <summary>
        /// Indicates whether the time zone is fixed, i.e. contains no transitions.
        /// </summary>
        /// <remarks>
        /// This is used as an optimization. If the time zone has no transitions but returns <c>false</c>
        /// for this then the behavior will be correct but the system will have to do extra work. However
        /// if the time zone has transitions and this returns <c>true</c> then the transitions will never
        /// be examined.
        /// </remarks>
        /// <value>true if the time zone is fixed; false otherwise.</value>
        internal bool IsFixed { get; }

        /// <summary>
        /// Gets the least (most negative) offset within this time zone, over all time.
        /// </summary>
        /// <value>The least (most negative) offset within this time zone, over all time.</value>
        public Offset MinOffset { get; }

        /// <summary>
        /// Gets the greatest (most positive) offset within this time zone, over all time.
        /// </summary>
        /// <value>The greatest (most positive) offset within this time zone, over all time.</value>
        public Offset MaxOffset { get; }

        #region Core abstract/virtual methods
        /// <summary>
        /// Returns the offset from UTC, where a positive duration indicates that local time is
        /// later than UTC. In other words, local time = UTC + offset.
        /// </summary>
        /// <remarks>
        /// This is mostly a convenience method for calling <c>GetZoneInterval(instant).WallOffset</c>,
        /// although it can also be overridden for more efficiency.
        /// </remarks>
        /// <param name="instant">The instant for which to calculate the offset.</param>
        /// <returns>
        /// The offset from UTC at the specified instant.
        /// </returns>
        public virtual Offset GetUtcOffset(Instant instant) => GetZoneInterval(instant).WallOffset;

        /// <summary>
        /// Gets the zone interval for the given instant; the range of time around the instant in which the same Offset
        /// applies (with the same split between standard time and daylight saving time, and with the same offset).
        /// </summary>
        /// <remarks>
        /// This will always return a valid zone interval, as time zones cover the whole of time.
        /// </remarks>
        /// <param name="instant">The <see cref="T:NodaTime.Instant" /> to query.</param>
        /// <returns>The defined <see cref="T:NodaTime.TimeZones.ZoneInterval" />.</returns>
        /// <seealso cref="GetZoneIntervals(Interval)"/>
        [NotNull] public abstract ZoneInterval GetZoneInterval(Instant instant);

        /// <summary>
        /// Returns complete information about how the given <see cref="LocalDateTime" /> is mapped in this time zone.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Mapping a local date/time to a time zone can give an unambiguous, ambiguous or impossible result, depending on
        /// time zone transitions. Use the return value of this method to handle these cases in an appropriate way for
        /// your use case.
        /// </para>
        /// <para>
        /// As an alternative, consider <see cref="ResolveLocal(LocalDateTime, ZoneLocalMappingResolver)"/>, which uses a caller-provided strategy to
        /// convert the <see cref="ZoneLocalMapping"/> returned here to a <see cref="ZonedDateTime"/>.
        /// </para>
        /// </remarks>
        /// <param name="localDateTime">The local date and time to map in this time zone.</param>
        /// <returns>A mapping of the given local date and time to zero, one or two zoned date/time values.</returns>
        [NotNull] public virtual ZoneLocalMapping MapLocal(LocalDateTime localDateTime)
        {
            LocalInstant localInstant = localDateTime.ToLocalInstant();
            Instant firstGuess = localInstant.MinusZeroOffset();
            ZoneInterval interval = GetZoneInterval(firstGuess);

            // Most of the time we'll go into here... the local instant and the instant
            // are close enough that we've found the right instant.
            if (interval.Contains(localInstant))
            {
                ZoneInterval? earlier = GetEarlierMatchingInterval(interval, localInstant);
                if (earlier != null)
                {
                    return new ZoneLocalMapping(this, localDateTime, earlier, interval, 2);
                }
                ZoneInterval? later = GetLaterMatchingInterval(interval, localInstant);
                if (later != null)
                {
                    return new ZoneLocalMapping(this, localDateTime, interval, later, 2);
                }
                return new ZoneLocalMapping(this, localDateTime, interval, interval, 1);
            }
            else
            {
                // Our first guess was wrong. Either we need to change interval by one (either direction)
                // or we're in a gap.
                ZoneInterval? earlier = GetEarlierMatchingInterval(interval, localInstant);
                if (earlier != null)
                {
                    return new ZoneLocalMapping(this, localDateTime, earlier, earlier, 1);
                }
                ZoneInterval? later = GetLaterMatchingInterval(interval, localInstant);
                if (later != null)
                {
                    return new ZoneLocalMapping(this, localDateTime, later, later, 1);
                }
                return new ZoneLocalMapping(this, localDateTime, GetIntervalBeforeGap(localInstant), GetIntervalAfterGap(localInstant), 0);
            }
        }
        #endregion

        #region Conversion between local dates/times and ZonedDateTime
        /// <summary>
        /// Returns the earliest valid <see cref="ZonedDateTime"/> with the given local date.
        /// </summary>
        /// <remarks>
        /// If midnight exists unambiguously on the given date, it is returned.
        /// If the given date has an ambiguous start time (e.g. the clocks go back from 1am to midnight)
        /// then the earlier ZonedDateTime is returned. If the given date has no midnight (e.g. the clocks
        /// go forward from midnight to 1am) then the earliest valid value is returned; this will be the instant
        /// of the transition.
        /// </remarks>
        /// <param name="date">The local date to map in this time zone.</param>
        /// <exception cref="SkippedTimeException">The entire day was skipped due to a very large time zone transition.
        /// (This is extremely rare.)</exception>
        /// <returns>The <see cref="ZonedDateTime"/> representing the earliest time in the given date, in this time zone.</returns>
        public ZonedDateTime AtStartOfDay(LocalDate date)
        {
            LocalDateTime midnight = date.AtMidnight();
            var mapping = MapLocal(midnight);
            switch (mapping.Count)
            {
                // Midnight doesn't exist. Maybe we just skip to 1am (or whatever), or maybe the whole day is missed.
                case 0:
                    var interval = mapping.LateInterval;
                    // Safe to use Start, as it can't extend to the start of time.
                    var offsetDateTime = new OffsetDateTime(interval.Start, interval.WallOffset, date.Calendar);
                    // It's possible that the entire day is skipped. For example, Samoa skipped December 30th 2011.
                    // We know the two values are in the same calendar here, so we just need to check the YearMonthDay.
                    if (offsetDateTime.YearMonthDay != date.YearMonthDay)
                    {
                        throw new SkippedTimeException(midnight, this);
                    }
                    return new ZonedDateTime(offsetDateTime, this);
                // Unambiguous or occurs twice, we can just use the offset from the earlier interval.
                case 1:
                case 2:
                    return new ZonedDateTime(midnight.WithOffset(mapping.EarlyInterval.WallOffset), this);
                default:
                    throw new InvalidOperationException("This won't happen.");
            }
        }

        /// <summary>
        /// Maps the given <see cref="LocalDateTime"/> to the corresponding <see cref="ZonedDateTime"/>, following
        /// the given <see cref="ZoneLocalMappingResolver"/> to handle ambiguity and skipped times.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method for calling <see cref="MapLocal"/> and passing the result to the resolver.
        /// Common options for resolvers are provided in the static <see cref="Resolvers"/> class.
        /// </para>
        /// <para>
        /// See <see cref="AtStrictly"/> and <see cref="AtLeniently"/> for alternative ways to map a local time to a
        /// specific instant.
        /// </para>
        /// </remarks>
        /// <param name="localDateTime">The local date and time to map in this time zone.</param>
        /// <param name="resolver">The resolver to apply to the mapping.</param>
        /// <returns>The result of resolving the mapping.</returns>
        public ZonedDateTime ResolveLocal(LocalDateTime localDateTime, [NotNull] ZoneLocalMappingResolver resolver)
        {
            Preconditions.CheckNotNull(resolver, nameof(resolver));
            return resolver(MapLocal(localDateTime));
        }

        /// <summary>
        /// Maps the given <see cref="LocalDateTime"/> to the corresponding <see cref="ZonedDateTime"/>, if and only if
        /// that mapping is unambiguous in this time zone.  Otherwise, <see cref="SkippedTimeException"/> or
        /// <see cref="AmbiguousTimeException"/> is thrown, depending on whether the mapping is ambiguous or the local
        /// date/time is skipped entirely.
        /// </summary>
        /// <remarks>
        /// See <see cref="AtLeniently"/> and <see cref="ResolveLocal(LocalDateTime, ZoneLocalMappingResolver)"/> for alternative ways to map a local time to a
        /// specific instant.
        /// </remarks>
        /// <param name="localDateTime">The local date and time to map into this time zone.</param>
        /// <exception cref="SkippedTimeException">The given local date/time is skipped in this time zone.</exception>
        /// <exception cref="AmbiguousTimeException">The given local date/time is ambiguous in this time zone.</exception>
        /// <returns>The unambiguous matching <see cref="ZonedDateTime"/> if it exists.</returns>
        public ZonedDateTime AtStrictly(LocalDateTime localDateTime) =>
            ResolveLocal(localDateTime, Resolvers.StrictResolver);

        /// <summary>
        /// Maps the given <see cref="LocalDateTime"/> to the corresponding <see cref="ZonedDateTime"/> in a lenient
        /// manner: ambiguous values map to the earlier of the alternatives, and "skipped" values are shifted forward
        /// by the duration of the "gap".
        /// </summary>
        /// <remarks>
        /// See <see cref="AtStrictly"/> and <see cref="ResolveLocal(LocalDateTime, ZoneLocalMappingResolver)"/> for alternative ways to map a local time to a
        /// specific instant.
        /// <para>Note: The behavior of this method was changed in version 2.0 to fit the most commonly seen real-world
        /// usage pattern.  Previous versions returned the later instance of ambiguous values, and returned the start of
        /// the zone interval after the gap for skipped value.  The previous functionality can still be used if desired,
        /// by using <see cref="ResolveLocal(LocalDateTime, ZoneLocalMappingResolver)"/>, passing in a resolver
        /// created from <see cref="Resolvers.ReturnLater"/> and <see cref="Resolvers.ReturnStartOfIntervalAfter"/>.</para>
        /// </remarks>
        /// <param name="localDateTime">The local date/time to map.</param>
        /// <returns>The unambiguous mapping if there is one, the earlier result if the mapping is ambiguous,
        /// or the forward-shifted value if the given local date/time is skipped.</returns>
        public ZonedDateTime AtLeniently(LocalDateTime localDateTime) =>
            ResolveLocal(localDateTime, Resolvers.LenientResolver);
        #endregion

        /// <summary>
        /// Returns the interval before this one, if it contains the given local instant, or null otherwise.
        /// </summary>
        private ZoneInterval? GetEarlierMatchingInterval(ZoneInterval interval, LocalInstant localInstant)
        {
            // Micro-optimization to avoid fetching interval.Start multiple times. Seems
            // to give a performance improvement on x86 at least...
            // If the zone interval extends to the start of time, the next check will definitely evaluate to false.
            Instant intervalStart = interval.RawStart;
            // This allows for a maxOffset of up to +1 day, and the "truncate towards beginning of time"
            // nature of the Days property.
            if (localInstant.DaysSinceEpoch <= intervalStart.DaysSinceEpoch + 1)
            {
                // We *could* do a more accurate check here based on the actual maxOffset, but it's probably
                // not worth it.
                ZoneInterval candidate = GetZoneInterval(intervalStart - Duration.Epsilon);
                if (candidate.Contains(localInstant))
                {
                    return candidate;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the next interval after this one, if it contains the given local instant, or null otherwise.
        /// </summary>
        private ZoneInterval? GetLaterMatchingInterval(ZoneInterval interval, LocalInstant localInstant)
        {
            // Micro-optimization to avoid fetching interval.End multiple times. Seems
            // to give a performance improvement on x86 at least...
            // If the zone interval extends to the end of time, the next check will
            // definitely evaluate to false.
            Instant intervalEnd = interval.RawEnd;
            // Crude but cheap first check to see whether there *might* be a later interval.
            // This allows for a minOffset of up to -1 day, and the "truncate towards beginning of time"
            // nature of the Days property.
            if (localInstant.DaysSinceEpoch >= intervalEnd.DaysSinceEpoch - 1)
            {
                // We *could* do a more accurate check here based on the actual maxOffset, but it's probably
                // not worth it.
                ZoneInterval candidate = GetZoneInterval(intervalEnd);
                if (candidate.Contains(localInstant))
                {
                    return candidate;
                }
            }
            return null;
        }

        private ZoneInterval GetIntervalBeforeGap(LocalInstant localInstant)
        {
            Instant guess = localInstant.MinusZeroOffset();
            ZoneInterval guessInterval = GetZoneInterval(guess);
            // If the local interval occurs before the zone interval we're looking at starts,
            // we need to find the earlier one; otherwise this interval must come after the gap, and
            // it's therefore the one we want.
            if (localInstant.Minus(guessInterval.WallOffset) < guessInterval.RawStart)
            {
                return GetZoneInterval(guessInterval.Start - Duration.Epsilon);
            }
            else
            {
                return guessInterval;
            }
        }

        private ZoneInterval GetIntervalAfterGap(LocalInstant localInstant)
        {
            Instant guess = localInstant.MinusZeroOffset();
            ZoneInterval guessInterval = GetZoneInterval(guess);
            // If the local interval occurs before the zone interval we're looking at starts,
            // it's the one we're looking for. Otherwise, we need to find the next interval.
            if (localInstant.Minus(guessInterval.WallOffset) < guessInterval.RawStart)
            {
                return guessInterval;
            }
            else
            {
                // Will definitely be valid - there can't be a gap after an infinite interval.
                return GetZoneInterval(guessInterval.End);
            }
        }

        #region Object overrides
        /// <summary>
        /// Returns the ID of this time zone.
        /// </summary>
        /// <returns>
        /// The ID of this time zone.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() => Id;
        #endregion

        /// <summary>
        /// Creates a fixed time zone for offsets -12 to +15 at every half hour,
        /// fixing the 0 offset as DateTimeZone.Utc.
        /// </summary>
        [NotNull]
        private static DateTimeZone[] BuildFixedZoneCache()
        {
            DateTimeZone[] ret = new DateTimeZone[FixedZoneCacheSize];
            for (int i = 0; i < FixedZoneCacheSize; i++)
            {
                int offsetSeconds = i * FixedZoneCacheGranularitySeconds + FixedZoneCacheMinimumSeconds;
                ret[i] = new FixedDateTimeZone(Offset.FromSeconds(offsetSeconds));
            }
            ret[-FixedZoneCacheMinimumSeconds / FixedZoneCacheGranularitySeconds] = Utc;
            return ret;
        }

        /// <summary>
        /// Returns all the zone intervals which occur for any instant in the interval [<paramref name="start"/>, <paramref name="end"/>).
        /// </summary>
        /// <remarks>
        /// <para>This method is simply a convenience method for calling <see cref="GetZoneIntervals(Interval)"/> without
        /// explicitly constructing the interval beforehand.
        /// </para>
        /// </remarks>
        /// <param name="start">Inclusive start point of the interval for which to retrieve zone intervals.</param>
        /// <param name="end">Exclusive end point of the interval for which to retrieve zone intervals.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="end"/> is earlier than <paramref name="start"/>.</exception>
        /// <returns>A sequence of zone intervals covering the given interval.</returns>
        /// <seealso cref="DateTimeZone.GetZoneInterval"/>
        [NotNull] public IEnumerable<ZoneInterval> GetZoneIntervals(Instant start, Instant end) =>
            // The constructor performs all the validation we need.
            GetZoneIntervals(new Interval(start, end));

        /// <summary>
        /// Returns all the zone intervals which occur for any instant in the given interval.
        /// </summary>
        /// <remarks>
        /// <para>The zone intervals are returned in chronological order.
        /// This method is equivalent to calling <see cref="DateTimeZone.GetZoneInterval"/> for every
        /// instant in the interval and then collapsing to a set of distinct zone intervals.
        /// The first and last zone intervals are likely to also cover instants outside the given interval;
        /// the zone intervals returned are not truncated to match the start and end points.
        /// </para>
        /// </remarks>
        /// <param name="interval">Interval to find zone intervals for. This is allowed to be unbounded (i.e.
        /// infinite in both directions).</param>
        /// <returns>A sequence of zone intervals covering the given interval.</returns>
        /// <seealso cref="DateTimeZone.GetZoneInterval"/>
        [NotNull] public IEnumerable<ZoneInterval> GetZoneIntervals(Interval interval)
        {
            var current = interval.HasStart ? interval.Start : Instant.MinValue;
            var end = interval.RawEnd;
            while (current < end)
            {
                var zoneInterval = GetZoneInterval(current);
                yield return zoneInterval;
                // If this is the end of time, this will just fail on the next comparison.
                current = zoneInterval.RawEnd;
            }
        }

        /// <summary>
        /// Returns the zone intervals within the given interval, potentially coalescing some of the
        /// original intervals according to options.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is equivalent to <see cref="GetZoneIntervals(Interval)"/>, but may coalesce some intervals.
        /// For example, if the <see cref="ZoneEqualityComparer.Options.OnlyMatchWallOffset"/> is specified,
        /// and two consecutive zone intervals have the same offset but different names, a single zone interval
        /// will be returned instead of two separate ones. When zone intervals are coalesced, all aspects of
        /// the first zone interval are used except its end instant, which is taken from the second zone interval.
        /// </para>
        /// <para>
        /// As the options are only used to determine which intervals to coalesce, the
        /// <see cref="ZoneEqualityComparer.Options.MatchStartAndEndTransitions"/> option does not affect
        /// the intervals returned.
        /// </para>
        /// </remarks>
        /// <param name="interval">Interval to find zone intervals for. This is allowed to be unbounded (i.e.
        /// infinite in both directions).</param>
        /// <param name="options"></param>
        /// <returns></returns>
        [NotNull] public IEnumerable<ZoneInterval> GetZoneIntervals(Interval interval, ZoneEqualityComparer.Options options)
        {
            if ((options & ~ZoneEqualityComparer.Options.StrictestMatch) != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options),
                    $"The value {options} is not defined within ZoneEqualityComparer.Options");
            }
            var zoneIntervalEqualityComparer = new ZoneEqualityComparer.ZoneIntervalEqualityComparer(options, interval);
            var originalIntervals = GetZoneIntervals(interval);
            return zoneIntervalEqualityComparer.CoalesceIntervals(originalIntervals);
        }
    }
}
