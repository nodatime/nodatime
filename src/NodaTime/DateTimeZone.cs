// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Threading;
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
    ///     resolving to the later option of ambiguous matches or the start of the zone interval after the gap for
    ///     skipped times.</description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="ResolveLocal"/> will apply a <see cref="ZoneLocalMappingResolver"/> to the result of
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
    /// <see href="http://www.iana.org/time-zones">"zoneinfo"</see> (aka tz or Olson) database, and the ability to convert .NET's own
    /// <see cref="TimeZoneInfo"/> format into a "native" Noda Time zone. Which of these is most appropriate for you to use
    /// will very much depend on your exact needs. The zoneinfo database is widely used outside Windows, and has more historical data
    /// than the Windows-provided information, but if you need to interoperate with other Windows systems by specifying time zone IDs,
    /// you may wish to stick to the Windows time zones.
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
    /// Note that <c>BclDateTimeZone</c> is not available on the PCL build of Noda Time, so this fallback strategy can
    /// only be used with the desktop version.
    /// </para>
    /// <para>Note that Noda Time does not require that <see cref="DateTimeZone"/> instances be singletons.
    /// As far as reasonably possible, implementations should implement <see cref="IEquatable{DateTimeZone}"/> in such a way
    /// that equivalent time zones compare as equal.</para>
    /// </remarks>
    /// <threadsafety>
    /// All time zone implementations within Noda Time are immutable and thread-safe. See the thread safety
    /// section of the user guide for more information.
    /// </threadsafety>
    public abstract class DateTimeZone : IEquatable<DateTimeZone>, IZoneIntervalMap
    {
        /// <summary>
        /// The ID of the UTC (Coordinated Universal Time) time zone. This ID is always valid, whatever provider is
        /// used. If the provider has its own mapping for UTC, that will be returned by <see cref="DateTimeZoneCache.GetZoneOrNull" />, but otherwise
        /// the value of the <see cref="Utc"/> property will be returned.
        /// </summary>
        internal const string UtcId = "UTC";

        private static readonly DateTimeZone UtcZone = new FixedDateTimeZone(Offset.Zero);
        private const int FixedZoneCacheGranularityMilliseconds = NodaConstants.MillisecondsPerMinute * 30;
        private const int FixedZoneCacheMinimumMilliseconds = -FixedZoneCacheGranularityMilliseconds * 12 * 2; // From UTC-12
        private const int FixedZoneCacheSize = (12 + 15) * 2 + 1; // To UTC+15 inclusive
        private static readonly DateTimeZone[] FixedZoneCache = BuildFixedZoneCache();

        private readonly string id;
        private readonly bool isFixed;

        // We very frequently need to add this to an instant, and there will be relatively few instances
        // of DateTimeZone, so it makes sense to convert to ticks once and take the space cost, instead of
        // performing the same multiplication over and over again.
        private readonly long minOffsetTicks;
        private readonly long maxOffsetTicks;

        /// <summary>
        /// Gets the UTC (Coordinated Universal Time) time zone. This is a single instance which is not
        /// provider-specific; it is guaranteed to have the ID "UTC", but may or may not be the instance returned by
        /// e.g. <c>DateTimeZoneProviders.Tzdb["UTC"]</c>.
        /// </summary>
        /// <value>A UTC <see cref="T:NodaTime.DateTimeZone" />.</value>
        public static DateTimeZone Utc { get { return UtcZone; } }

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
        public static DateTimeZone ForOffset(Offset offset)
        {
            int millis = offset.Milliseconds;
            if (millis % FixedZoneCacheGranularityMilliseconds != 0)
            {
                return new FixedDateTimeZone(offset);
            }
            int index = (millis - FixedZoneCacheMinimumMilliseconds) / FixedZoneCacheGranularityMilliseconds;
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
        protected DateTimeZone(string id, bool isFixed, Offset minOffset, Offset maxOffset)
        {
            this.id = id;
            this.isFixed = isFixed;
            this.minOffsetTicks = minOffset.Ticks;
            this.maxOffsetTicks = maxOffset.Ticks;
        }

        /// <summary>
        /// The provider's ID for the time zone.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This identifies the time zone within the current time zone provider; a different provider may
        /// provide a different time zone with the same ID, or may not provide a time zone with that ID at all.
        /// </para>
        /// </remarks>
        public string Id { get { return id; } }

        /// <summary>
        /// Indicates whether the time zone is fixed, i.e. contains no transitions.
        /// </summary>
        /// <remarks>
        /// This is used as an optimization. If the time zone has no transitions but returns <c>false</c>
        /// for this then the behavior will be correct but the system will have to do extra work. However
        /// if the time zone has transitions and this returns <c>true</c> then the transitions will never
        /// be examined.
        /// </remarks>
        internal bool IsFixed { get { return isFixed; } }

        /// <summary>
        /// Returns the least (most negative) offset within this time zone, over all time.
        /// </summary>
        public Offset MinOffset { get { return Offset.FromTicks(minOffsetTicks); } }

        /// <summary>
        /// Returns the greatest (most positive) offset within this time zone, over all time.
        /// </summary>
        public Offset MaxOffset { get { return Offset.FromTicks(maxOffsetTicks); } }

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
        public virtual Offset GetUtcOffset(Instant instant)
        {
            return GetZoneInterval(instant).WallOffset;
        }

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
        public abstract ZoneInterval GetZoneInterval(Instant instant);

        /// <summary>
        /// Finds all zone intervals for the given local instant. Usually there's one (i.e. only a single
        /// instant is mapped to the given local instant within the time zone) but during DST transitions
        /// there can be either 0 (the given local instant doesn't exist, e.g. local time skipped from 1am to
        /// 2am, but you gave us 1.30am) or 2 (the given local instant is ambiguous, e.g. local time skipped
        /// from 2am to 1am, but you gave us 1.30am).
        /// </summary>
        /// <remarks>
        /// This method is implemented in terms of GetZoneInterval(Instant) within DateTimeZone,
        /// and should work for any zone. However, internal derived classes may override this method
        /// for optimization purposes, e.g. if the zone interval is always ambiguous with
        /// a fixed value.
        /// </remarks>
        /// <param name="localInstant">The local instant to find matching zone intervals for</param>
        /// <returns>The struct containing up to two ZoneInterval references.</returns>
        internal virtual ZoneIntervalPair GetZoneIntervalPair(LocalInstant localInstant)
        {
            Instant firstGuess = new Instant(localInstant.Ticks);
            ZoneInterval interval = GetZoneInterval(firstGuess);

            // Most of the time we'll go into here... the local instant and the instant
            // are close enough that we've found the right instant.
            if (interval.Contains(localInstant))
            {
                ZoneInterval earlier = GetEarlierMatchingInterval(interval, localInstant);
                if (earlier != null)
                {
                    return ZoneIntervalPair.Ambiguous(earlier, interval);
                }
                ZoneInterval later = GetLaterMatchingInterval(interval, localInstant);
                if (later != null)
                {
                    return ZoneIntervalPair.Ambiguous(interval, later);
                }
                return ZoneIntervalPair.Unambiguous(interval);
            }
            else
            {
                // Our first guess was wrong. Either we need to change interval by one (either direction)
                // or we're in a gap.
                ZoneInterval earlier = GetEarlierMatchingInterval(interval, localInstant);
                if (earlier != null)
                {
                    return ZoneIntervalPair.Unambiguous(earlier);
                }
                ZoneInterval later = GetLaterMatchingInterval(interval, localInstant);
                if (later != null)
                {
                    return ZoneIntervalPair.Unambiguous(later);
                }
                return ZoneIntervalPair.NoMatch;
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
            LocalInstant localInstant = date.AtMidnight().LocalInstant;
            ZoneIntervalPair pair = GetZoneIntervalPair(localInstant);
            switch (pair.MatchingIntervals)
            {
                case 0:
                    var interval = GetIntervalAfterGap(localInstant);
                    var localDateTime = new LocalDateTime(interval.LocalStart, date.Calendar);
                    // It's possible that the entire day is skipped. For example, Samoa skipped December 30th 2011.
                    if (localDateTime.Date != date)
                    {
                        throw new SkippedTimeException(date + LocalTime.Midnight, this);
                    }
                    return new ZonedDateTime(localDateTime, interval.WallOffset, this);
                case 1:
                case 2:
                    return new ZonedDateTime(date.AtMidnight(), pair.EarlyInterval.WallOffset, this);
                default:
                    throw new InvalidOperationException("This won't happen.");
            }
        }

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
        /// An an alternative, consider <see cref="ResolveLocal"/>, which uses a caller-provided strategy to
        /// convert the <see cref="ZoneLocalMapping"/> returned here to a <see cref="ZonedDateTime"/>.
        /// </para>
        /// </remarks>
        /// <param name="localDateTime">The local date and time to map in this time zone.</param>
        /// <returns>A mapping of the given local date and time to zero, one or two zoned date/time values.</returns>
        public ZoneLocalMapping MapLocal(LocalDateTime localDateTime)
        {
            LocalInstant localInstant = localDateTime.LocalInstant;
            Instant firstGuess = new Instant(localInstant.Ticks);
            ZoneInterval interval = GetZoneInterval(firstGuess);

            // Most of the time we'll go into here... the local instant and the instant
            // are close enough that we've found the right instant.
            if (interval.Contains(localInstant))
            {
                ZoneInterval earlier = GetEarlierMatchingInterval(interval, localInstant);
                if (earlier != null)
                {
                    return new ZoneLocalMapping(this, localDateTime, earlier, interval, 2);
                }
                ZoneInterval later = GetLaterMatchingInterval(interval, localInstant);
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
                ZoneInterval earlier = GetEarlierMatchingInterval(interval, localInstant);
                if (earlier != null)
                {
                    return new ZoneLocalMapping(this, localDateTime, earlier, earlier, 1);
                }
                ZoneInterval later = GetLaterMatchingInterval(interval, localInstant);
                if (later != null)
                {
                    return new ZoneLocalMapping(this, localDateTime, later, later, 1);
                }
                return new ZoneLocalMapping(this, localDateTime, GetIntervalBeforeGap(localInstant), GetIntervalAfterGap(localInstant), 0);
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
        /// <exception cref="ArgumentNullException"><paramref name="resolver"/> is null.</exception>
        /// <returns>The result of resolving the mapping.</returns>
        public ZonedDateTime ResolveLocal(LocalDateTime localDateTime, ZoneLocalMappingResolver resolver)
        {
            Preconditions.CheckNotNull(resolver, "resolver");
            return resolver(MapLocal(localDateTime));
        }

        /// <summary>
        /// Maps the given <see cref="LocalDateTime"/> to the corresponding <see cref="ZonedDateTime"/>, if and only if
        /// that mapping is unambiguous in this time zone.  Otherwise, <see cref="SkippedTimeException"/> or
        /// <see cref="AmbiguousTimeException"/> is thrown, depending on whether the mapping is ambiguous or the local
        /// date/time is skipped entirely.
        /// </summary>
        /// <remarks>
        /// See <see cref="AtLeniently"/> and <see cref="ResolveLocal"/> for alternative ways to map a local time to a
        /// specific instant.
        /// </remarks>
        /// <param name="localDateTime">The local date and time to map into this time zone.</param>
        /// <exception cref="SkippedTimeException">The given local date/time is skipped in this time zone.</exception>
        /// <exception cref="AmbiguousTimeException">The given local date/time is ambiguous in this time zone.</exception>
        /// <returns>The unambiguous matching <see cref="ZonedDateTime"/> if it exists.</returns>
        public ZonedDateTime AtStrictly(LocalDateTime localDateTime)
        {
            return MapLocal(localDateTime).Single();
        }

        /// <summary>
        /// Maps the given <see cref="LocalDateTime"/> to the corresponding <see cref="ZonedDateTime"/> in a lenient
        /// manner: ambiguous values map to the later of the alternatives, and "skipped" values map to the start of the
        /// zone interval after the "gap".
        /// </summary>
        /// <remarks>
        /// See <see cref="AtStrictly"/> and <see cref="ResolveLocal"/> for alternative ways to map a local time to a
        /// specific instant.
        /// </remarks>
        /// <param name="localDateTime">The local date/time to map.</param>
        /// <returns>The unambiguous mapping if there is one, the later result if the mapping is ambiguous,
        /// or the start of the later zone interval if the given local date/time is skipped.</returns>
        public ZonedDateTime AtLeniently(LocalDateTime localDateTime)
        {
            return ResolveLocal(localDateTime, Resolvers.LenientResolver);
        }
        #endregion

        /// <summary>
        /// Returns the interval before this one, if it contains the given local instant, or null otherwise.
        /// </summary>
        private ZoneInterval GetEarlierMatchingInterval(ZoneInterval interval, LocalInstant localInstant)
        {
            // Micro-optimization to avoid fetching interval.Start multiple times. Seems
            // to give a performance improvement on x86 at least...
            Instant intervalStart = interval.Start;
            if (intervalStart == Instant.MinValue)
            {
                return null;
            }
            // If the tick before this interval started *could* map to a later local instant, let's
            // get the interval and check whether it actually includes the one we want.
            Instant endOfPrevious = intervalStart;
            if (endOfPrevious.Ticks + maxOffsetTicks > localInstant.Ticks)
            {
                ZoneInterval candidate = GetZoneInterval(endOfPrevious - Duration.Epsilon);
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
        private ZoneInterval GetLaterMatchingInterval(ZoneInterval interval, LocalInstant localInstant)
        {
            // Micro-optimization to avoid fetching interval.End multiple times. Seems
            // to give a performance improvement on x86 at least...
            Instant intervalEnd = interval.End;
            if (intervalEnd == Instant.MaxValue)
            {
                return null;
            }
            if (intervalEnd.Ticks + minOffsetTicks <= localInstant.Ticks)
            {
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
            Instant guess = new Instant(localInstant.Ticks);
            ZoneInterval guessInterval = GetZoneInterval(guess);
            // If the local interval occurs before the zone interval we're looking at starts,
            // we need to find the earlier one; otherwise this interval must come after the gap, and
            // it's therefore the one we want.
            if (localInstant.Minus(guessInterval.WallOffset) < guessInterval.Start)
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
            Instant guess = new Instant(localInstant.Ticks);
            ZoneInterval guessInterval = GetZoneInterval(guess);
            // If the local interval occurs before the zone interval we're looking at starts,
            // it's the one we're looking for. Otherwise, we need to find the next interval.
            if (localInstant.Minus(guessInterval.WallOffset) < guessInterval.Start)
            {
                return guessInterval;
            }
            else
            {
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
        public override string ToString()
        {
            return Id;
        }
        #endregion

        /// <summary>
        /// Creates a fixed time zone for offsets -23.5 to +23.5 at every half hour,
        /// fixing the 0 offset as DateTimeZone.Utc.
        /// </summary>
        private static DateTimeZone[] BuildFixedZoneCache()
        {
            DateTimeZone[] ret = new DateTimeZone[FixedZoneCacheSize];
            for (int i = 0; i < FixedZoneCacheSize; i++)
            {
                int offsetMillis = i * FixedZoneCacheGranularityMilliseconds + FixedZoneCacheMinimumMilliseconds;
                ret[i] = new FixedDateTimeZone(Offset.FromMilliseconds(offsetMillis));
            }
            ret[-FixedZoneCacheMinimumMilliseconds / FixedZoneCacheGranularityMilliseconds] = DateTimeZone.Utc;
            return ret;
        }

        #region Equality
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override sealed bool Equals(object obj)
        {
            return Equals(obj as DateTimeZone);
        }

        /// <summary>
        /// Determines whether the specified <see cref="DateTimeZone"/> is equal to this instance.
        /// </summary>
        /// <remarks>
        /// This implementation performs initial checks which would be common to all child implementations,
        /// and then delegates to <see cref="EqualsImpl"/>.
        /// </remarks>
        /// <param name="obj">The <see cref="DateTimeZone"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="DateTimeZone"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(DateTimeZone obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return !ReferenceEquals(obj, null) && obj.GetType() == GetType() && EqualsImpl(obj);
        }

        /// <summary>
        /// Implements equality in derived classes.
        /// </summary>
        /// <param name="zone">The zone to compare with this one. This is guaranteed (when called by <see cref="Equals(DateTimeZone)"/>) to
        /// be a non-null reference of the same type as this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="DateTimeZone"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool EqualsImpl(DateTimeZone zone);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </returns>
        public abstract override int GetHashCode();
        #endregion

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
        /// <seealso cref="GetZoneInterval"/>
        public IEnumerable<ZoneInterval> GetZoneIntervals(Instant start, Instant end)
        {
            // The constructor performs all the validation we need.
            return GetZoneIntervals(new Interval(start, end));
        }

        /// <summary>
        /// Returns all the zone intervals which occur for any instant in the given interval.
        /// </summary>
        /// <remarks>
        /// <para>The zone intervals are returned in chronological order. This method is equivalent to calling <see cref="GetZoneInterval"/> for every
        /// instant in the interval and then collapsing to a set of distinct zone intervals.
        /// The first and last zone intervals are likely to also cover instants outside the given interval;
        /// the zone intervals returned are not truncated to match the start and end points.
        /// </para>
        /// </remarks>
        /// <param name="interval">Interval to find zone intervals for.</param>
        /// <returns>A sequence of zone intervals covering the given interval.</returns>
        /// <seealso cref="GetZoneInterval"/>
        public IEnumerable<ZoneInterval> GetZoneIntervals(Interval interval)
        {
            var current = interval.Start;
            while (current < interval.End)
            {
                var zoneInterval = GetZoneInterval(current);
                yield return zoneInterval;
                current = zoneInterval.End;
            }
        }
    }
}
