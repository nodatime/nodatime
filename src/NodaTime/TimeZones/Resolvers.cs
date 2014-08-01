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
        public static readonly AmbiguousTimeResolver ReturnEarlier = (earlier, later) => earlier;

        /// <summary>
        /// An <see cref="AmbiguousTimeResolver"/> which returns the later of the two matching times.
        /// </summary>
        public static readonly AmbiguousTimeResolver ReturnLater = (earlier, later) => later;

        /// <summary>
        /// An <see cref="AmbiguousTimeResolver"/> which simply throws an <see cref="AmbiguousTimeException"/>.
        /// </summary>
        public static readonly AmbiguousTimeResolver ThrowWhenAmbiguous = (earlier, later) =>
        {
            throw new AmbiguousTimeException(earlier, later);
        };

        /// <summary>
        /// A <see cref="SkippedTimeResolver"/> which returns the final tick of the time zone interval
        /// before the "gap".
        /// </summary>
        public static readonly SkippedTimeResolver ReturnEndOfIntervalBefore = (local, zone, before, after) =>
        {
            Preconditions.CheckNotNull(zone, "zone");
            Preconditions.CheckNotNull(before, "before");
            Preconditions.CheckNotNull(after, "after");
            // Given that there's a zone after before, it can't extend to the end of time.
            return new ZonedDateTime(before.End - Duration.Epsilon, zone, local.Calendar);
        };

        /// <summary>
        /// A <see cref="SkippedTimeResolver"/> which returns the first tick of the time zone interval
        /// after the "gap".
        /// </summary>
        public static readonly SkippedTimeResolver ReturnStartOfIntervalAfter = (local, zone, before, after) =>
        {
            Preconditions.CheckNotNull(zone, "zone");
            Preconditions.CheckNotNull(before, "before");
            Preconditions.CheckNotNull(after, "after");
            return new ZonedDateTime(after.Start, zone, local.Calendar);
        };

        /// <summary>
        /// A <see cref="SkippedTimeResolver"/> which simply throws a <see cref="SkippedTimeException"/>.
        /// </summary>
        public static readonly SkippedTimeResolver ThrowWhenSkipped = (local, zone, before, after) =>
        {
            Preconditions.CheckNotNull(zone, "zone");
            Preconditions.CheckNotNull(before, "before");
            Preconditions.CheckNotNull(after, "after");
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
        public static readonly ZoneLocalMappingResolver StrictResolver = CreateMappingResolver(ThrowWhenAmbiguous, ThrowWhenSkipped);

        /// <summary>
        /// A <see cref="ZoneLocalMappingResolver"/> which never throws an exception due to ambiguity or skipped time.
        /// </summary>
        /// <remarks>
        /// Ambiguity is handled by returning the later occurrence, and skipped times are mapped to the start of the zone interval
        /// after the gap. This resolver combines <see cref="ReturnLater"/> and <see cref="ReturnStartOfIntervalAfter"/>.
        /// </remarks>
        /// <seealso cref="DateTimeZone.AtLeniently"/>
        public static readonly ZoneLocalMappingResolver LenientResolver = CreateMappingResolver(ReturnLater, ReturnStartOfIntervalAfter);

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
        public static ZoneLocalMappingResolver CreateMappingResolver([NotNull] AmbiguousTimeResolver ambiguousTimeResolver, [NotNull] SkippedTimeResolver skippedTimeResolver)
        {
            Preconditions.CheckNotNull(ambiguousTimeResolver, "ambiguousTimeResolver");
            Preconditions.CheckNotNull(skippedTimeResolver, "skippedTimeResolver");
            return mapping =>
            {
                Preconditions.CheckNotNull(mapping, "mapping");
                switch (mapping.Count)
                {
                    case 0: return skippedTimeResolver(mapping.LocalDateTime, mapping.Zone, mapping.EarlyInterval, mapping.LateInterval);
                    case 1: return mapping.First();
                    case 2: return ambiguousTimeResolver(mapping.First(), mapping.Last());
                    default: throw new InvalidOperationException("Mapping has count outside range 0-2; should not happen.");
                }
            };
        }
    }
}
