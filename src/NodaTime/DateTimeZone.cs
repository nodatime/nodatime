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
    /// Represents a time zone - a mapping between UTC and local time.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A time zone maps UTC instants to local instants - or, equivalently, to the offset from UTC
    /// at any particular instant.
    /// </para>
    /// <para>The mapping is unambiguous in the "UTC to local" direction, but
    /// the reverse is not true: when the offset changes, usually due to a Daylight Saving transition,
    /// the change either creates a gap (a period of local time which never occurs in the time zone)
    /// or an ambiguity (a period of local time which occurs twice in the time zone). Mapping back from
    /// local time to an instant requires consideration of how these problematic times will be handled.
    /// </para>
    /// </remarks>
    public abstract class DateTimeZone
    {
        /// <summary>
        /// The ID of the UTC (Coordinated Universal Time) time zone.
        /// </summary>
        public const string UtcId = "UTC";

        /// <summary>
        /// Gets the default time zone provider, which is initialized from resources within the NodaTime assembly.
        /// </summary>
        public static readonly DateTimeZoneResourceProvider DefaultDateTimeZoneProvider = new DateTimeZoneResourceProvider("NodaTime.TimeZones.Tzdb");

        private static readonly DateTimeZone UtcZone = new FixedDateTimeZone(Offset.Zero);

        private static TimeZoneCache cache = new TimeZoneCache(false);

        private readonly string id;
        private readonly bool isFixed;

        // We very frequently need to add this to an instant, and there will be relatively few instances
        // of DateTimeZone, so it makes sense to convert to ticks once and take the space cost, instead of
        // performing the same multiplication over and over again.
        private readonly long minOffsetTicks;
        private readonly long maxOffsetTicks;

        /// <summary>
        ///   Gets the UTC (Coordinated Universal Time) time zone.
        /// </summary>
        /// <value>The UTC <see cref="T:NodaTime.DateTimeZone" />.</value>
        public static DateTimeZone Utc { get { return UtcZone; } }

        /// <summary>
        ///   Gets the system default time zone which can only be changed by the system.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     The time zones defined in the operating system are different than the ones defines in
        ///     this library so a mapping will occur. If an exact mapping can be made then that will be
        ///     used otherwise UTC will be used.
        ///   </para>
        /// </remarks>
        /// <value>The system default <see cref="T:NodaTime.DateTimeZone" /> this will never be <c>null</c>.</value>
        public static DateTimeZone SystemDefault { get { return cache.SystemDefault; } }

        /// <summary>
        ///   Returns the time zone with the given id.
        /// </summary>
        /// <param name="id">The time zone id to find.</param>
        /// <returns>The <see cref="DateTimeZone" /> with the given id or <c>null</c> if there isn't one defined.</returns>
        public static DateTimeZone ForId(string id)
        {
            return cache.ForId(id);
        }

        /// <summary>
        ///   Gets the complete list of valid time zone ids provided by all of the registered
        ///   providers. This list will be sorted in lexigraphical order by the id name.
        /// </summary>
        /// <value>The <see cref="IEnumerable{T}" /> of string ids.</value>
        public static IEnumerable<string> Ids { get { return cache.Ids; } }

        /// <summary>
        ///   Adds the given time zone provider to the front of the provider list.
        /// </summary>
        /// <remarks>
        ///   Because this adds the new provider to the from of the list, it will be checked first for
        ///   time zone definitions and therefore can override the default system definitions. This
        ///   allows for adding new or replacing existing time zones without updating the system. If
        ///   the provider is already on the list nothing changes.
        /// </remarks>
        /// <param name="provider">The <see cref="IDateTimeZoneProvider" /> to add.</param>
        public static void AddProvider(IDateTimeZoneProvider provider)
        {
            cache.AddProvider(provider);
        }

        /// <summary>
        ///   Removes the given time zone provider from the provider list.
        /// </summary>
        /// <remarks>
        ///   If the provider is not on the list nothing changes.
        /// </remarks>
        /// <param name="provider">The <see cref="IDateTimeZoneProvider" /> to remove.</param>
        /// <returns><c>true</c> if the provider was removed.</returns>
        public static bool RemoveProvider(IDateTimeZoneProvider provider)
        {
            return cache.RemoveProvider(provider);
        }

        /// <summary>
        ///   Sets the UTC time zone only mode.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     If the mode is set to <c>true</c> then only the UTC time zone provider will be available
        ///     and only the UTC time zone will be accessible. This is mainly for use during the
        ///     building of the system when there is no existing time zone database.
        ///   </para>
        ///   <para>
        ///     When this method is called all existing providers are removed from the list. Then the UTC
        ///     provider will be added and if the <paramref name = "utcOnlyFlag" /> is <c>false</c> then default
        ///     provider. This means that any providers added by user code will be removed.
        ///   </para>
        /// </remarks>
        /// <param name="utcOnlyFlag">if set to <c>true</c> then only the UTC provider will be available.</param>
        internal static void SetUtcOnly(bool utcOnlyFlag)
        {
            cache = new TimeZoneCache(utcOnlyFlag);
        }

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
            return GetZoneInterval(instant).Offset;
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
                    return new ZonedDateTime(localDateTime, pair.EarlyInterval.Offset, this);
                case 2:
                    throw new AmbiguousTimeException(localDateTime, this,
                        new ZonedDateTime(localDateTime, pair.EarlyInterval.Offset, this),
                        new ZonedDateTime(localDateTime, pair.LateInterval.Offset, this));
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
                    return new ZonedDateTime(localDateTime, pair.EarlyInterval.Offset, this);
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
                    return new ZonedDateTime(localDateTime, pair.EarlyInterval.Offset, this);
                case 2:
                    return new ZonedDateTime(localDateTime, pair.LateInterval.Offset, this);
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
        /// <returns>The <see cref="ZonedDateTime"/> representing the earliest time in the given date, in this time zone.</returns>
        public ZonedDateTime AtStartOfDay(LocalDate date)
        {
            LocalInstant localInstant = date.LocalDateTime.LocalInstant;
            ZoneIntervalPair pair = GetZoneIntervals(localInstant);
            switch (pair.MatchingIntervals)
            {
                case 0:
                    var interval = GetIntervalAfterGap(localInstant);
                    return new ZonedDateTime(new LocalDateTime(interval.LocalStart, date.Calendar), interval.Offset, this);
                case 1:
                case 2:
                    return new ZonedDateTime(date.LocalDateTime, pair.EarlyInterval.Offset, this);
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
                    var mapping = new ZonedDateTime(localDateTime, pair.EarlyInterval.Offset, this);
                    return ZoneLocalMapping.UnambiguousResult(mapping);
                case 2:
                    var earlier = new ZonedDateTime(localDateTime, pair.EarlyInterval.Offset, this);
                    var later = new ZonedDateTime(localDateTime, pair.LateInterval.Offset, this);
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
                ZoneInterval candidate = GetZoneInterval(endOfPrevious - Duration.One);
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
            if (localInstant.Minus(guessInterval.Offset) < guessInterval.Start)
            {
                return GetZoneInterval(guessInterval.Start - Duration.One);
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
            if (localInstant.Minus(guessInterval.Offset) < guessInterval.Start)
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
