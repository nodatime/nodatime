// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Annotations;
using NodaTime.Utility;
using JetBrains.Annotations;

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
        /// <summary>
        /// Gets the <see cref="DateTimeZone" /> in which this mapping was performed.
        /// </summary>
        /// <value>The time zone in which this mapping was performed.</value>
        [NotNull] public DateTimeZone Zone { get; }

        /// <summary>
        /// Gets the <see cref="T:NodaTime.LocalDateTime" /> which was mapped within the time zone.
        /// </summary>
        /// <value>The local date and time which was mapped within the time zone.</value>
        public LocalDateTime LocalDateTime { get; }

        /// <summary>
        /// Gets the earlier <see cref="ZoneInterval" /> within this mapping.
        /// </summary>
        /// <remarks>
        /// For unambiguous mappings, this is the same as <see cref="LateInterval" />; for ambiguous mappings,
        /// this is the interval during which the mapped local time first occurs; for impossible
        /// mappings, this is the interval before which the mapped local time occurs.
        /// </remarks>
        /// <value>The earlier zone interval within this mapping.</value>
        [NotNull] public ZoneInterval EarlyInterval { get; }

        /// <summary>
        /// Gets the later <see cref="ZoneInterval" /> within this mapping.
        /// </summary>
        /// <remarks>
        /// For unambiguous
        /// mappings, this is the same as <see cref="EarlyInterval" />; for ambiguous mappings,
        /// this is the interval during which the mapped local time last occurs; for impossible
        /// mappings, this is the interval after which the mapped local time occurs.
        /// </remarks>
        /// <value>The later zone interval within this mapping.</value>
        [NotNull] public ZoneInterval LateInterval { get; }

        /// <summary>
        /// Gets the number of results within this mapping: the number of distinct
        /// <see cref="ZonedDateTime" /> values which map to the original <see cref="T:NodaTime.LocalDateTime" />.
        /// </summary>
        /// <value>The number of results within this mapping: the number of distinct values which map to the
        /// original local date and time.</value>
        public int Count { get; }

        internal ZoneLocalMapping([Trusted] [NotNull] DateTimeZone zone, LocalDateTime localDateTime,
            [Trusted] [NotNull] ZoneInterval earlyInterval, [Trusted] [NotNull] ZoneInterval lateInterval, int count)
        {
            Preconditions.DebugCheckNotNull(zone, nameof(zone));
            Preconditions.DebugCheckNotNull(earlyInterval, nameof(earlyInterval));
            Preconditions.DebugCheckNotNull(lateInterval, nameof(lateInterval));
            Preconditions.DebugCheckArgumentRange(nameof(count), count, 0, 2);
            this.Zone = zone;
            this.EarlyInterval = earlyInterval;
            this.LateInterval = lateInterval;
            this.LocalDateTime = localDateTime;
            this.Count = count;
        }

        /// <summary>
        /// Returns the single <see cref="ZonedDateTime"/> which maps to the original
        /// <see cref="T:NodaTime.LocalDateTime" /> in the mapped <see cref="DateTimeZone" />.
        /// </summary>
        /// <exception cref="SkippedTimeException">The local date/time was skipped in the time zone.</exception>
        /// <exception cref="AmbiguousTimeException">The local date/time was ambiguous in the time zone.</exception>
        /// <returns>The unambiguous result of mapping the local date/time in the time zone.</returns>
        public ZonedDateTime Single() => Count switch
            {
                0 => throw new SkippedTimeException(LocalDateTime, Zone),
                1 => BuildZonedDateTime(EarlyInterval),
                2 => throw new AmbiguousTimeException(
                            BuildZonedDateTime(EarlyInterval),
                            BuildZonedDateTime(LateInterval)),
                _ => throw new InvalidOperationException("Can't happen")
            };

        /// <summary>
        /// Returns a <see cref="ZonedDateTime"/> which maps to the original <see cref="T:NodaTime.LocalDateTime" />
        /// in the mapped <see cref="DateTimeZone" />: either the single result if the mapping is unambiguous,
        /// or the earlier result if the local date/time occurs twice in the time zone due to a time zone
        /// offset change such as an autumnal daylight saving transition.
        /// </summary>
        /// <exception cref="SkippedTimeException">The local date/time was skipped in the time zone.</exception>
        /// <returns>The unambiguous result of mapping a local date/time in a time zone.</returns>
        public ZonedDateTime First() => Count switch
            {
                0 => throw new SkippedTimeException(LocalDateTime, Zone),
                // FIXME: Remove the duplication here (if there's syntax)
                1 => BuildZonedDateTime(EarlyInterval),
                2 => BuildZonedDateTime(EarlyInterval),
                _ => throw new InvalidOperationException("Can't happen")
            };

        /// <summary>
        /// Returns a <see cref="ZonedDateTime"/> which maps to the original <see cref="T:NodaTime.LocalDateTime" />
        /// in the mapped <see cref="DateTimeZone" />: either the single result if the mapping is unambiguous,
        /// or the later result if the local date/time occurs twice in the time zone due to a time zone
        /// offset change such as an autumnal daylight saving transition.
        /// </summary>
        /// <exception cref="SkippedTimeException">The local date/time was skipped in the time zone.</exception>
        /// <returns>The unambiguous result of mapping a local date/time in a time zone.</returns>
        public ZonedDateTime Last() => Count switch
            {
                0 => throw new SkippedTimeException(LocalDateTime, Zone),
                // FIXME: Remove the duplication here (if there's syntax)
                1 => BuildZonedDateTime(LateInterval),
                2 => BuildZonedDateTime(LateInterval),
                _ => throw new InvalidOperationException("Can't happen")
            };

        private ZonedDateTime BuildZonedDateTime(ZoneInterval interval) =>
            new ZonedDateTime(LocalDateTime.WithOffset(interval.WallOffset), Zone);
    }
}
