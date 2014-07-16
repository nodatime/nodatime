// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Annotations;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    // Note: documentation that refers to the LocalDateTime type within this class must use the fully-qualified
    // reference to avoid being resolved to the LocalDateTime property instead.

    /// <summary>
    /// The result of mapping a <see cref="T:NodaTime.LocalDateTime" /> within a time zone, i.e. finding out
    /// at what "global" time the "local" time occurred.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is used as the return type of <see cref="DateTimeZone.MapLocal" />. It allows for
    /// finely-grained handling of the three possible results:
    /// </para>
    /// <list type="bullet">
    ///   <item>
    ///     <term>Unambiguous mapping</term>
    ///     <description>The local time occurs exactly once in the target time zone.</description>
    ///   </item>
    ///   <item>
    ///     <term>Ambiguous mapping</term>
    ///     <description>
    ///       The local time occurs twice in the target time zone, due to the offset from UTC
    ///       changing. This usually occurs for an autumnal daylight saving transition, where the clocks
    ///       are put back by an hour. If the clocks change from 2am to 1am for example, then 1:30am occurs
    ///       twice - once before the transition and once afterwards.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term>Impossible mapping</term>
    ///     <description>
    ///       The local time does not occur at all in the target time zone, due to the offset from UTC
    ///       changing. This usually occurs for a vernal (spring-time) daylight saving transition, where the clocks
    ///       are put forward by an hour. If the clocks change from 1am to 2am for example, then 1:30am is
    ///       skipped entirely.
    ///     </description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <threadsafety>This type is an immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    [Immutable]
    public sealed class ZoneLocalMapping
    {
        private readonly DateTimeZone zone;
        private readonly LocalDateTime localDateTime;
        private readonly ZoneInterval earlyInterval;
        private readonly ZoneInterval lateInterval;
        private readonly int count;

        internal ZoneLocalMapping(DateTimeZone zone, LocalDateTime localDateTime, ZoneInterval earlyInterval, ZoneInterval lateInterval, int count)
        {
            Preconditions.DebugCheckNotNull(zone, "zone");
            Preconditions.DebugCheckNotNull(earlyInterval, "earlyInterval");
            Preconditions.DebugCheckNotNull(lateInterval, "lateInterval");
            Preconditions.DebugCheckArgumentRange("count", count, 0, 2);
            this.zone = zone;
            this.earlyInterval = earlyInterval;
            this.lateInterval = lateInterval;
            this.localDateTime = localDateTime;
            this.count = count;
        }

        /// <summary>
        /// Returns the number of results within this mapping: the number of distinct
        /// <see cref="ZonedDateTime" /> values which map to the original <see cref="T:NodaTime.LocalDateTime" />.
        /// </summary>
        public int Count { get { return count; } }

        /// <summary>
        /// Returns the <see cref="DateTimeZone" /> in which this mapping was performed.
        /// </summary>
        public DateTimeZone Zone { get { return zone; } }

        /// <summary>
        /// Returns the <see cref="T:NodaTime.LocalDateTime" /> which was mapped with in the time zone.
        /// </summary>
        public LocalDateTime LocalDateTime { get { return localDateTime; } }

        /// <summary>
        /// Returns the earlier <see cref="ZoneInterval" /> within this mapping. For unambiguous
        /// mappings, this is the same as <see cref="LateInterval" />; for ambiguous mappings,
        /// this is the interval during which the mapped local time first occurs; for impossible
        /// mappings, this is the interval before which the mapped local time occurs.
        /// </summary>
        public ZoneInterval EarlyInterval { get { return earlyInterval; } }

        /// <summary>
        /// Returns the later <see cref="ZoneInterval" /> within this mapping. For unambiguous
        /// mappings, this is the same as <see cref="EarlyInterval" />; for ambiguous mappings,
        /// this is the interval during which the mapped local time last occurs; for impossible
        /// mappings, this is the interval after which the mapped local time occurs.
        /// </summary>
        public ZoneInterval LateInterval { get { return lateInterval; } }

        /// <summary>
        /// Returns the single <see cref="ZonedDateTime"/> which maps to the original
        /// <see cref="T:NodaTime.LocalDateTime" /> in the mapped <see cref="DateTimeZone" />.
        /// </summary>
        /// <exception cref="SkippedTimeException">The local date/time was skipped in the time zone.</exception>
        /// <exception cref="AmbiguousTimeException">The local date/time was ambiguous in the time zone.</exception>
        /// <returns>The unambiguous result of mapping the local date/time in the time zone.</returns>
        public ZonedDateTime Single()
        {
            switch (count)
            {
                case 0: throw new SkippedTimeException(localDateTime, zone);
                case 1: return BuildZonedDateTime(earlyInterval);
                case 2: throw new AmbiguousTimeException(
                            BuildZonedDateTime(earlyInterval),
                            BuildZonedDateTime(lateInterval));
                default: throw new InvalidOperationException("Can't happen");
            }
        }

        /// <summary>
        /// Returns a <see cref="ZonedDateTime"/> which maps to the original <see cref="T:NodaTime.LocalDateTime" />
        /// in the mapped <see cref="DateTimeZone" />: either the single result if the mapping is unambiguous,
        /// or the earlier result if the local date/time occurs twice in the time zone due to a time zone
        /// offset change such as an autumnal daylight saving transition.
        /// </summary>
        /// <exception cref="SkippedTimeException">The local date/time was skipped in the time zone.</exception>
        /// <returns>The unambiguous result of mapping a local date/time in a time zone.</returns>
        public ZonedDateTime First()
        {
            switch (count)
            {
                case 0: throw new SkippedTimeException(localDateTime, zone);
                case 1: 
                case 2: return BuildZonedDateTime(earlyInterval);
                default: throw new InvalidOperationException("Can't happen");
            }
        }

        /// <summary>
        /// Returns a <see cref="ZonedDateTime"/> which maps to the original <see cref="T:NodaTime.LocalDateTime" />
        /// in the mapped <see cref="DateTimeZone" />: either the single result if the mapping is unambiguous,
        /// or the later result if the local date/time occurs twice in the time zone due to a time zone
        /// offset change such as an autumnal daylight saving transition.
        /// </summary>
        /// <exception cref="SkippedTimeException">The local date/time was skipped in the time zone.</exception>
        /// <returns>The unambiguous result of mapping a local date/time in a time zone.</returns>
        public ZonedDateTime Last()
        {
            switch (count)
            {
                case 0: throw new SkippedTimeException(localDateTime, zone);
                case 1: return BuildZonedDateTime(earlyInterval);
                case 2: return BuildZonedDateTime(lateInterval);
                default: throw new InvalidOperationException("Can't happen");
            }
        }

        private ZonedDateTime BuildZonedDateTime(ZoneInterval interval)
        {
            return new ZonedDateTime(localDateTime.WithOffset(interval.WallOffset), zone);
        }
    }
}
