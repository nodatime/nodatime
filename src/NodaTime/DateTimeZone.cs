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
using NodaTime.TimeZones;

namespace NodaTime
{
    /// <summary>
    /// Represents a time zone - a mapping between UTC and local time. A time zone maps UTC instants to local instants
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
    ///     <description><see cref="AtExactly"/> will throw an exception if the mapping from local time is either ambiguous
    ///     or impossible, i.e. if there is anything other than one instant which maps to the given local time.</description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="AtEarlier"/> will throw an exception if the mapping from local time is impossible, i.e.
    ///     if that time is skipped, but will return the earlier of two valid mappings where there is ambiguity.</description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="AtLater"/> will throw an exception if the mapping from local time is impossible, i.e.
    ///     if that time is skipped, but will return the later of two valid mappings where there is ambiguity.</description>
    ///   </item>
    ///   <item>
    ///     <description><see cref="MapLocalDateTime"/> will not throw any exceptions, but return a <see cref="ZoneLocalMapping"/>
    ///     with complete information about whether the given local time occurs zero times, once or twice. This is the most
    ///     fine-grained approach, which is the fiddliest to use but puts the caller in the most control.</description>
    ///   </item>
    /// </list>
    /// </para>
    /// <para>
    /// Noda Time has two sources of time zone data available: a built-in copy of the
    /// <see href="http://www.iana.org/time-zones">"zoneinfo"</see> (aka tz or Olson) database, and the ability to convert .NET's own
    /// <see cref="TimeZoneInfo"/>  format into a "native" Noda Time zone. Which of these is most appropriate for you to use
    /// will very much depend on your exact needs. The zoneinfo database is widely used outside Windows, and has more historical data
    /// than the Windows-provided information, but if you need to interoperate with other Windows systems by specifying time zone IDs,
    /// you may wish to stick to the Windows time zones.
    /// </para>
    /// <para>The static <see cref="ForId"/> method will fetch a time zone with the given ID; the set of valid IDs is dependent on the
    /// current time zone provider, which can be replaced with the <see cref="SetProvider"/> method. The default provider is
    /// a <see cref="TzdbTimeZoneProvider"/> which loads its data from inside the Noda Time assembly; it can be replaced either with another
    /// TzdbTimeZoneProvider instance which may obtain data from external resources (e.g. to use a more recent version of the zoneinfo database
    /// than the version of Noda Time you're using) or an instance of <see cref="BclTimeZoneProvider"/>.
    /// </para>
    /// <para>Unlike many other date/time APIs, Noda Time does not use the system default time zone without you explicitly asking it to.
    /// You can fetch the Noda Time representation of the system default time zone using <see cref="GetSystemDefault"/>, which will attempt to
    /// find an appropriate time zone using the current provider. You should be aware that this may fail, however (in which case the method will return null)
    /// if no mapping is found. This could occur due to the system having a "custom" time zone installed, or there being no mapping for the BCL zone ID
    /// to the provider's set of IDs due to the BCL zone ID being added recently. You can always use <see cref="BclTimeZone.ForSystemDefault"/> to convert
    /// the local <see cref="TimeZoneInfo"/> to guarantee that a representation is available.</para>
    /// </remarks>
    public abstract class DateTimeZone
    {
        /// <summary>
        /// The ID of the UTC (Coordinated Universal Time) time zone. This ID is always valid, whatever provider is
        /// used. If the provider has its own mapping for UTC, that will be returned by <see cref="ForId" />, but otherwise
        /// the value of the <see cref="Utc"/> property will be returned.
        /// </summary>
        public const string UtcId = "UTC";

        /// <summary>
        /// Gets the default time zone provider, which is initialized from resources within the NodaTime assembly.
        /// </summary>
        public static readonly TzdbTimeZoneProvider DefaultDateTimeZoneProvider = new TzdbTimeZoneProvider("NodaTime.TimeZones.Tzdb");

        private static readonly DateTimeZone UtcZone = new FixedDateTimeZone(Offset.Zero);

        private static TimeZoneCache cache = new TimeZoneCache(DefaultDateTimeZoneProvider);
        private static readonly object cacheLock = new object();

        private readonly string id;
        private readonly bool isFixed;

        // We very frequently need to add this to an instant, and there will be relatively few instances
        // of DateTimeZone, so it makes sense to convert to ticks once and take the space cost, instead of
        // performing the same multiplication over and over again.
        private readonly long minOffsetTicks;
        private readonly long maxOffsetTicks;

        /// <summary>
        /// Gets the UTC (Coordinated Universal Time) time zone. This is a single instance which is not
        /// provider-specific; it may or may not be the value returned by <c>DateTimeZone.ForId("UTC")</c>; that
        /// depends on whether the current provider has its own mapping for the UTC ID.
        /// </summary>
        /// <value>The UTC <see cref="T:NodaTime.DateTimeZone" />.</value>
        public static DateTimeZone Utc { get { return UtcZone; } }

        /// <summary>
        /// Gets the system default time zone, as mapped by the underlying provider.
        /// </summary>
        /// <remarks>
        /// Callers should be aware that this method can return null, even with standard Windows time zones.
        /// This could be due to either the Unicode CLDR not being up-to-date with Windows time zone IDs,
        /// or Noda Time not being up-to-date with CLDR - or a provider-specific problem. Callers can use
        /// the null-coalescing operator to effectively provider a default:
        /// </remarks>
        /// <returns>
        /// The provider-specific representation of the system time zone, or null if the time zone
        /// could not be mapped.
        /// </returns>
        public static DateTimeZone GetSystemDefault()
        {
            return cache.GetSystemDefault();
        }

        /// <summary>
        ///   Returns the time zone with the given id.
        /// </summary>
        /// <param name="id">The time zone id to find.</param>
        /// <returns>The <see cref="DateTimeZone" /> with the given id or <c>null</c> if there isn't one defined.</returns>
        public static DateTimeZone ForId(string id)
        {
            TimeZoneCache localCache;
            lock (cacheLock)
            {
                localCache = cache;
            }
            return localCache[id];
        }

        /// <summary>
        /// Returns a version identifier for the time zone provider.
        /// </summary>
        public static string ProviderVersionId        
        {
            get
            {
                lock (cacheLock)
                {
                    return cache.ProviderVersionId;
                }
            }
        }

        /// <summary>
        /// Sets the provider to use for time zone lookup. Note that this is a global change; it is expected
        /// that users will only call this on start-up if at all.
        /// </summary>
        /// <param name="provider">The provider to use for time zones.</param>
        public static void SetProvider(IDateTimeZoneProvider provider)
        {
            var localCache = new TimeZoneCache(provider);
            lock (cacheLock)
            {
                cache = localCache;
            }
        }

        /// <summary>
        /// Gets the complete list of valid time zone ids provided by all of the registered
        /// providers. This list will be sorted in lexigraphical order by the id name.
        /// </summary>
        /// <remarks>
        /// The ID "UTC" will always be present in this list; it is guaranteed to be available regardless
        /// of the time zone provider in use.
        /// </remarks>
        /// <value>The <see cref="IEnumerable{T}" /> of string ids.</value>
        public static IEnumerable<string> Ids { get { return cache.Ids; } }

        /// <summary>
        ///   Initializes a new instance of the <see cref="T:NodaTime.DateTimeZone" /> class.
        /// </summary>
        /// <param name="id">The unique id of this time zone.</param>
        /// <param name="isFixed">Set to <c>true</c> if this time zone has no transitions.</param>
        /// <param name="minOffset">Minimum offset applied within this zone</param>
        /// <param name="maxOffset">Maximum offset applied within this zone</param>
        protected DateTimeZone(string id, bool isFixed, Offset minOffset, Offset maxOffset)
        {
            this.id = id;
            this.isFixed = isFixed;
            this.minOffsetTicks = minOffset.TotalTicks;
            this.maxOffsetTicks = maxOffset.TotalTicks;
        }

        /// <summary>
        ///   The database ID for the time zone.
        /// </summary>
        /// <remarks>
        ///   This must be unique across all time zone providers.
        /// </remarks>
        public string Id { get { return id; } }

        /// <summary>
        ///   Indicates whether the time zone is fixed, i.e. contains no transitions.
        /// </summary>
        /// <remarks>
        ///   This is used as an optimization. If the time zone has not transitions but returns <c>true</c>
        ///   for this then the behavior will be correct but the system will have to do extra work. However
        ///   if the time zone has transitions and this returns <c>false</c> then the transitions will never
        ///   be examined.
        /// </remarks>
        public bool IsFixed { get { return isFixed; } }

        /// <summary>
        /// Returns the least offset within this time zone.
        /// </summary>
        public Offset MinOffset { get { return Offset.FromTicks(minOffsetTicks); } }

        /// <summary>
        /// Returns the greatest offset within this time zone.
        /// </summary>
        public Offset MaxOffset { get { return Offset.FromTicks(maxOffsetTicks); } }

        #region Core abstract/virtual methods
        /// <summary>
        /// Returns the offset from UTC, where a positive duration indicates that local time is
        /// later than UTC. In other words, local time = UTC + offset.
        /// </summary>
        /// <remarks>
        /// This is mostly a convenience method for calling <code>GetZoneInterval(instant).Offset</code>,
        /// although it can also be overridden for more efficiency.
        /// </remarks>
        /// <param name="instant">The instant for which to calculate the offset.</param>
        /// <returns>
        /// The offset from UTC at the specified instant.
        /// </returns>
        public virtual Offset GetOffsetFromUtc(Instant instant)
        {
            return GetZoneInterval(instant).WallOffset;
        }

        /// <summary>
        ///   Gets the zone interval for the given instant. This will never return null.
        /// </summary>
        /// <param name="instant">The <see cref="T:NodaTime.Instant" /> to query.</param>
        /// <returns>The defined <see cref="T:NodaTime.TimeZones.ZoneInterval" />.</returns>
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
        /// and should work for any zone. However, derived classes may override this method
        /// for optimization purposes, e.g. if the zone interval is always ambiguous with
        /// a fixed value.
        /// </remarks>
        /// <param name="localInstant">The local instant to find matching zone intervals for</param>
        /// <returns>The struct containing up to two ZoneInterval references.</returns>
        internal virtual ZoneIntervalPair GetZoneIntervals(LocalInstant localInstant)
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


        /// <summary>
        /// Writes the time zone to the specified writer. Used within ZoneInfoCompiler.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        internal abstract void Write(DateTimeZoneWriter writer);
        #endregion

        #region Conversion between local dates/times and ZonedDateTime
        /// <summary>
        /// If the given local date/time is mapped unambiguously to a single ZonedDateTime value, that is returned.
        /// For ambiguous or skipped local date/time values, AmbiguousTimeException or SkippedTimeException are thrown respectively.
        /// </summary>
        /// <param name="localDateTime">The local date and time to map in this time zone.</param>
        /// <returns>The unambiguous <see cref="ZonedDateTime"/> with the same local date and time as the given parameter in this time zone.</returns>
        /// <exception cref="SkippedTimeException">The given LocalDateTime is skipped due to a transition where the clocks go forward</exception>
        /// <exception cref="AmbiguousTimeException">The given LocalDateTime is ambiguous due to a transition where the clocks go forward</exception>
        public ZonedDateTime AtExactly(LocalDateTime localDateTime)
        {
            ZoneIntervalPair pair = GetZoneIntervals(localDateTime.LocalInstant);
            switch (pair.MatchingIntervals)
            {
                case 0:
                    throw new SkippedTimeException(localDateTime, this);
                case 1:
                    return new ZonedDateTime(localDateTime, pair.EarlyInterval.WallOffset, this);
                case 2:
                    throw new AmbiguousTimeException(localDateTime, this,
                        new ZonedDateTime(localDateTime, pair.EarlyInterval.WallOffset, this),
                        new ZonedDateTime(localDateTime, pair.LateInterval.WallOffset, this));
                default:
                    throw new InvalidOperationException("This won't happen.");
            }
        }

        /// <summary>
        /// If the given local date/time is mapped unambiguously to a single ZonedDateTime value, that is returned.
        /// For ambiguous local date/time values, the earlier mapping is returned.
        /// For skipped local date/time values, SkippedTimeException is thrown.
        /// </summary>
        /// <param name="localDateTime">The local date and time to map in this time zone.</param>
        /// <returns>The <see cref="ZonedDateTime"/> value with the same local time as <paramref name="localDateTime"/>,
        /// either unambiguously or the earlier of two ambiguous possibilities.</returns>
        /// <exception cref="SkippedTimeException">The given LocalDateTime is skipped due to a transition where the clocks go forward</exception>
        public ZonedDateTime AtEarlier(LocalDateTime localDateTime)
        {
            ZoneIntervalPair pair = GetZoneIntervals(localDateTime.LocalInstant);
            switch (pair.MatchingIntervals)
            {
                case 0:
                    throw new SkippedTimeException(localDateTime, this);
                case 1:
                case 2:
                    return new ZonedDateTime(localDateTime, pair.EarlyInterval.WallOffset, this);
                default:
                    throw new InvalidOperationException("This won't happen.");
            }
        }

        /// <summary>
        /// If the given local date/time is mapped unambiguously to a single ZonedDateTime value, that is returned.
        /// For ambiguous local date/time values, the later mapping is returned.
        /// For skipped local date/time values, SkippedTimeException is thrown.
        /// </summary>
        /// <param name="localDateTime">The local date and time to map in this time zone.</param>
        /// <returns>The <see cref="ZonedDateTime"/> value with the same local time as <paramref name="localDateTime"/>,
        /// either unambiguously or the earlier of two ambiguous possibilities.</returns>
        /// <exception cref="SkippedTimeException">The given LocalDateTime is skipped due to a transition where the clocks go forward</exception>
        public ZonedDateTime AtLater(LocalDateTime localDateTime)
        {
            ZoneIntervalPair pair = GetZoneIntervals(localDateTime.LocalInstant);
            switch (pair.MatchingIntervals)
            {
                case 0:
                    throw new SkippedTimeException(localDateTime, this);
                case 1:
                    return new ZonedDateTime(localDateTime, pair.EarlyInterval.WallOffset, this);
                case 2:
                    return new ZonedDateTime(localDateTime, pair.LateInterval.WallOffset, this);
                default:
                    throw new InvalidOperationException("This won't happen.");
            }
        }

        /// <summary>
        /// Returns the ZonedDateTime with a LocalDateTime as early as possible on the given date.
        /// If midnight exists unambiguously on the given date, it is returned.
        /// If the given date has an ambiguous start time (e.g. the clocks go back from 1am to midnight)
        /// then the earlier ZonedDateTime is returned. If the given date has no midnight (e.g. the clocks
        /// go forward from midnight to 1am) then the earliest valid value is returned; this will be the instant
        /// of the transition.
        /// </summary>
        /// <param name="date">The local date to map in this time zone.</param>
        /// <exception cref="SkippedTimeException">The entire day was skipped due to a very large time zone transition.
        /// (This is extremely rare.)</exception>
        /// <returns>The <see cref="ZonedDateTime"/> representing the earliest time in the given date, in this time zone.</returns>
        public ZonedDateTime AtStartOfDay(LocalDate date)
        {
            LocalInstant localInstant = date.LocalDateTime.LocalInstant;
            ZoneIntervalPair pair = GetZoneIntervals(localInstant);
            switch (pair.MatchingIntervals)
            {
                case 0:
                    var interval = GetIntervalAfterGap(localInstant);
                    var localDateTime = new LocalDateTime(interval.LocalStart, date.Calendar);
                    // It's possible that the entire day is skipped. Known case: Samoa skipped December 30th 2011.
                    if (localDateTime.Date != date)
                    {
                        throw new SkippedTimeException(date + LocalTime.Midnight, this);
                    }
                    return new ZonedDateTime(localDateTime, interval.WallOffset, this);
                case 1:
                case 2:
                    return new ZonedDateTime(date.LocalDateTime, pair.EarlyInterval.WallOffset, this);
                default:
                    throw new InvalidOperationException("This won't happen.");
            }
        }

        /// <summary>
        /// Returns complete information about how the given LocalDateTime is mapped in this time zone.
        /// </summary>
        /// <remarks>Use this method if you need to know whether the given value is ambiguous, or if you
        /// want to find out about a potential "gap" in local dates and times due to a daylight saving transition.
        /// </remarks>
        /// <param name="localDateTime">The local date and time to map in this time zone.</param>
        /// <returns>A mapping of the given local date and time to zero, one or two zoned date/time values.</returns>
        public ZoneLocalMapping MapLocalDateTime(LocalDateTime localDateTime)
        {
            LocalInstant localInstant = localDateTime.LocalInstant;
            ZoneIntervalPair pair = GetZoneIntervals(localInstant);
            switch (pair.MatchingIntervals)
            {
                case 0:
                    var before = GetIntervalBeforeGap(localInstant);
                    var after = GetIntervalAfterGap(localInstant);
                    return ZoneLocalMapping.SkippedResult(before, after);
                case 1:
                    var mapping = new ZonedDateTime(localDateTime, pair.EarlyInterval.WallOffset, this);
                    return ZoneLocalMapping.UnambiguousResult(mapping);
                case 2:
                    var earlier = new ZonedDateTime(localDateTime, pair.EarlyInterval.WallOffset, this);
                    var later = new ZonedDateTime(localDateTime, pair.LateInterval.WallOffset, this);
                    return ZoneLocalMapping.AmbiguousResult(earlier, later);
                default:
                    throw new InvalidOperationException("This won't happen.");
            }
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
                ZoneInterval candidate = GetZoneInterval(endOfPrevious - Duration.OneTick);
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
            // If the local interval occurs before the zone interval we're looking at start,
            // we need to find the earlier one; otherwise this interval must come after the gap, and
            // it's therefore the one we want.
            if (localInstant.Minus(guessInterval.WallOffset) < guessInterval.Start)
            {
                return GetZoneInterval(guessInterval.Start - Duration.OneTick);
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
        ///   Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Id;
        }
        #endregion
    }
}
