// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using JetBrains.Annotations;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Commonly-used implementations of the delegates used in resolving a <see cref="LocalDateTime"/> to a
    /// <see cref="ZonedDateTime"/>, and a method to combine two "partial" resolvers into a full one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class contains predefined implementations of <see cref="ZoneLocalMappingResolver"/>,
    /// <see cref="AmbiguousTimeResolver"/>, and <see cref="SkippedTimeResolver"/>, along with
    /// <see cref="CreateMappingResolver"/>, which produces a <c>ZoneLocalMappingResolver</c> from instances of the
    /// other two.
    /// </para>
    /// </remarks>
    /// <threadsafety>All members of this class are thread-safe, as are the values returned by them.</threadsafety>
    public static class Resolvers
    {
        /// <summary>
        /// An <see cref="AmbiguousTimeResolver"/> which returns the earlier of the two matching times.
        /// </summary>
        /// <value>An <see cref="AmbiguousTimeResolver"/> which returns the earlier of the two matching times.</value>
        [NotNull] public static AmbiguousTimeResolver ReturnEarlier { get; } = (earlier, later) => earlier;

        /// <summary>
        /// An <see cref="AmbiguousTimeResolver"/> which returns the later of the two matching times.
        /// </summary>
        /// <value>An <see cref="AmbiguousTimeResolver"/> which returns the later of the two matching times.</value>
        [NotNull] public static AmbiguousTimeResolver ReturnLater { get; } = (earlier, later) => later;

        /// <summary>
        /// An <see cref="AmbiguousTimeResolver"/> which simply throws an <see cref="AmbiguousTimeException"/>.
        /// </summary>
        /// <value>An <see cref="AmbiguousTimeResolver"/> which simply throws an <see cref="AmbiguousTimeException"/>.</value>
        [NotNull] public static AmbiguousTimeResolver ThrowWhenAmbiguous { get; } = (earlier, later) =>
        {
            throw new AmbiguousTimeException(earlier, later);
        };

        /// <summary>
        /// A <see cref="SkippedTimeResolver"/> which returns the final tick of the time zone interval
        /// before the "gap".
        /// </summary>
        /// <value>A <see cref="SkippedTimeResolver"/> which returns the final tick of the time zone interval
        /// before the "gap".</value>
        [NotNull] public static SkippedTimeResolver ReturnEndOfIntervalBefore { get; } = (local, zone, before, after) =>
        {
            Preconditions.CheckNotNull(zone, nameof(zone));
            Preconditions.CheckNotNull(before, nameof(before));
            Preconditions.CheckNotNull(after, nameof(after));
            // Given that there's a zone after before, it can't extend to the end of time.
            return new ZonedDateTime(before.End - Duration.Epsilon, zone, local.Calendar);
        };

        /// <summary>
        /// A <see cref="SkippedTimeResolver"/> which returns the first tick of the time zone interval
        /// after the "gap".
        /// </summary>
        /// <value>
        /// A <see cref="SkippedTimeResolver"/> which returns the first tick of the time zone interval
        /// after the "gap".
        /// </value>
        [NotNull] public static SkippedTimeResolver ReturnStartOfIntervalAfter { get; } = (local, zone, before, after) =>
        {
            Preconditions.CheckNotNull(zone, nameof(zone));
            Preconditions.CheckNotNull(before, nameof(before));
            Preconditions.CheckNotNull(after, nameof(after));
            return new ZonedDateTime(after.Start, zone, local.Calendar);
        };

        /// <summary>
        /// A <see cref="SkippedTimeResolver"/> which shifts values in the "gap" forward by the duration
        /// of the gap (which is usually 1 hour). This corresponds to the instant that would have occured,
        /// had there not been a transition.
        /// </summary>
        /// <value>
        /// A <see cref="SkippedTimeResolver"/> which shifts values in the "gap" forward by the duration
        /// of the gap (which is usually 1 hour). 
        /// </value>
        [NotNull] public static SkippedTimeResolver ReturnForwardShifted { get; } = (local, zone, before, after) =>
        {
            Preconditions.CheckNotNull(zone, nameof(zone));
            Preconditions.CheckNotNull(before, nameof(before));
            Preconditions.CheckNotNull(after, nameof(after));
            return new ZonedDateTime(new OffsetDateTime(local, before.WallOffset).WithOffset(after.WallOffset), zone);
        };

        /// <summary>
        /// A <see cref="SkippedTimeResolver"/> which simply throws a <see cref="SkippedTimeException"/>.
        /// </summary>
        /// <value>A <see cref="SkippedTimeResolver"/> which simply throws a <see cref="SkippedTimeException"/>.</value>
        [NotNull] public static SkippedTimeResolver ThrowWhenSkipped { get; } = (local, zone, before, after) =>
        {
            Preconditions.CheckNotNull(zone, nameof(zone));
            Preconditions.CheckNotNull(before, nameof(before));
            Preconditions.CheckNotNull(after, nameof(after));
            throw new SkippedTimeException(local, zone);
        };

        /// <summary>
        /// A <see cref="ZoneLocalMappingResolver"/> which only ever succeeds in the (usual) case where the result
        /// of the mapping is unambiguous.
        /// </summary>
        /// <remarks>
        /// If the mapping is ambiguous or skipped, this throws <see cref="SkippedTimeException"/> or
        /// <see cref="AmbiguousTimeException"/>, as appropriate. This resolver combines
        /// <see cref="ThrowWhenAmbiguous"/> and <see cref="ThrowWhenSkipped"/>.
        /// </remarks>
        /// <seealso cref="DateTimeZone.AtStrictly"/>
        /// <value>A <see cref="ZoneLocalMappingResolver"/> which only ever succeeds in the (usual) case where the result
        /// of the mapping is unambiguous.</value>
        [NotNull] public static ZoneLocalMappingResolver StrictResolver { get; } =
            CreateMappingResolver(ThrowWhenAmbiguous, ThrowWhenSkipped);

        /// <summary>
        /// A <see cref="ZoneLocalMappingResolver"/> which never throws an exception due to ambiguity or skipped time.
        /// </summary>
        /// <remarks>
        /// Ambiguity is handled by returning the earlier occurrence, and skipped times are shifted forward by the duration
        /// of the gap. This resolver combines <see cref="ReturnEarlier"/> and <see cref="ReturnForwardShifted"/>.
        /// <para>Note: The behavior of this resolver was changed in version 2.0 to fit the most commonly seen real-world
        /// usage pattern.  Previous versions combined the <see cref="ReturnLater"/> and <see cref="ReturnStartOfIntervalAfter"/>
        /// resolvers, which can still be used separately if desired.</para>
        /// </remarks>
        /// <seealso cref="DateTimeZone.AtLeniently"/>
        /// <value>A <see cref="ZoneLocalMappingResolver"/> which never throws an exception due to ambiguity or skipped time.</value>
        [NotNull] public static ZoneLocalMappingResolver LenientResolver { get; } =
            CreateMappingResolver(ReturnEarlier, ReturnForwardShifted);

        /// <summary>
        /// Combines an <see cref="AmbiguousTimeResolver"/> and a <see cref="SkippedTimeResolver"/> to create a
        /// <see cref="ZoneLocalMappingResolver"/>.
        /// </summary>
        /// <remarks>
        /// The <c>ZoneLocalMappingResolver</c> created by this method operates in the obvious way: unambiguous mappings
        /// are returned directly, ambiguous mappings are delegated to the given <c>AmbiguousTimeResolver</c>, and
        /// "skipped" mappings are delegated to the given <c>SkippedTimeResolver</c>.
        /// </remarks>
        /// <param name="ambiguousTimeResolver">Resolver to use for ambiguous mappings.</param>
        /// <param name="skippedTimeResolver">Resolver to use for "skipped" mappings.</param>
        /// <returns>The logical combination of the two resolvers.</returns>
        [NotNull] public static ZoneLocalMappingResolver CreateMappingResolver([NotNull] AmbiguousTimeResolver ambiguousTimeResolver, [NotNull] SkippedTimeResolver skippedTimeResolver)
        {
            Preconditions.CheckNotNull(ambiguousTimeResolver, nameof(ambiguousTimeResolver));
            Preconditions.CheckNotNull(skippedTimeResolver, nameof(skippedTimeResolver));
            return mapping =>
                Preconditions.CheckNotNull(mapping, nameof(mapping)).Count switch
                {
                    0 => skippedTimeResolver(mapping.LocalDateTime, mapping.Zone, mapping.EarlyInterval, mapping.LateInterval),
                    1 => mapping.First(),
                    2 => ambiguousTimeResolver(mapping.First(), mapping.Last()),
                    _ => throw new InvalidOperationException("Mapping has count outside range 0-2; should not happen.")
                };
        }
    }
}
