#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
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
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// "Constant" fields representing commonly-used implementations of the resolver delegates,
    /// and a method to combine two "partial" resolvers into a full one.
    /// </summary>
    public static class Resolvers
    {
        private static readonly int TypeInitializationChecking = NodaTime.Utility.TypeInitializationChecker.RecordInitializationStart();

        /// <summary>
        /// <see cref="AmbiguousTimeResolver"/> which returns the earlier of the two matching times.
        /// </summary>
        public static readonly AmbiguousTimeResolver ReturnEarlier = (earlier, later) => earlier;

        /// <summary>
        /// <see cref="AmbiguousTimeResolver"/> which returns the later of the two matching times.
        /// </summary>
        public static readonly AmbiguousTimeResolver ReturnLater = (earlier, later) => later;

        /// <summary>
        /// <see cref="AmbiguousTimeResolver"/> which simply throws an <see cref="AmbiguousTimeException"/>.
        /// </summary>
        public static readonly AmbiguousTimeResolver ThrowWhenAmbiguous = (earlier, later) =>
        {
            throw new AmbiguousTimeException(earlier, later);
        };

        /// <summary>
        /// <see cref="SkippedTimeResolver"/> which returns the final tick of the time zone interval
        /// before the "gap".
        /// </summary>
        public static readonly SkippedTimeResolver ReturnEndOfIntervalBefore = (local, zone, before, after) =>
        {
            var localDateTime = new LocalDateTime(before.LocalEnd - Duration.Epsilon, local.Calendar);
            return new ZonedDateTime(localDateTime, before.WallOffset, zone);
        };

        /// <summary>
        /// <see cref="SkippedTimeResolver"/> which returns the first tick of the time zone interval
        /// after the "gap".
        /// </summary>
        public static readonly SkippedTimeResolver ReturnStartOfIntervalAfter = (local, zone, before, after) =>
        {
            Preconditions.CheckNotNull(zone, "zone");
            Preconditions.CheckNotNull(before, "before");
            Preconditions.CheckNotNull(after, "after");
            var localDateTime = new LocalDateTime(after.LocalStart, local.Calendar);
            return new ZonedDateTime(localDateTime, after.WallOffset, zone);
        };

        /// <summary>
        /// <see cref="SkippedTimeResolver"/> which simply throws a <see cref="SkippedTimeException"/> (assuming
        /// the arguments are all non-null).
        /// </summary>
        public static readonly SkippedTimeResolver ThrowWhenSkipped = (local, zone, before, after) =>
        {
            Preconditions.CheckNotNull(zone, "zone");
            Preconditions.CheckNotNull(before, "before");
            Preconditions.CheckNotNull(after, "after");
            throw new SkippedTimeException(local, zone);
        };

        /// <summary>
        /// <see cref="ZoneLocalMappingResolver"/> which only ever succeeds in the (usual) case where the result
        /// of the mapping is unambiguous. Otherwise, it throws <see cref="SkippedTimeException"/> or
        /// <see cref="AmbiguousTimeException"/>. This resolver combines <see cref="ThrowWhenAmbiguous"/> and
        /// <see cref="ThrowWhenSkipped"/>.
        /// </summary>
        public static readonly ZoneLocalMappingResolver StrictResolver = CreateMappingResolver(ThrowWhenAmbiguous, ThrowWhenSkipped);

        /// <summary>
        /// <see cref="ZoneLocalMappingResolver"/> which never throws an exception due to ambiguity or skipped time.
        /// Ambiguity is handled by returning the later occurrence, and skipped times are mapped to the start of the zone interval
        /// after the gap. This resolver combines <see cref="ReturnLater"/> and <see cref="ReturnStartOfIntervalAfter"/>.
        /// </summary>
        public static readonly ZoneLocalMappingResolver LenientResolver = CreateMappingResolver(ReturnLater, ReturnStartOfIntervalAfter);

        /// <summary>
        /// Combines an <see cref="AmbiguousTimeResolver"/> and a <see cref="SkippedTimeResolver"/> into a <see cref="ZoneLocalMappingResolver"/>
        /// in the obvious way: unambiguous mappings are returned directly, ambiguous mappings are delegated to the given
        /// AmbiguousTimeResolver, and "skipped" mappings are delegated to the given SkippedTimeResolver.
        /// </summary>
        /// <param name="ambiguousTimeResolver">Resolver to use for ambiguous mappings.</param>
        /// <param name="skippedTimeResolver">Resolver to use for "skipped" mappings.</param>
        /// <exception name="ArgumentNullException">Either of the arguments is null.</exception>
        /// <returns>The logical combination of the two resolvers.</returns>
        public static ZoneLocalMappingResolver CreateMappingResolver(AmbiguousTimeResolver ambiguousTimeResolver, SkippedTimeResolver skippedTimeResolver)
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
