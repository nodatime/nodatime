// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using JetBrains.Annotations;
using NodaTime.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NodaTime.Annotations;
using NodaTime.TimeZones;

namespace NodaTime.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IDateTimeZoneProvider"/>
    /// </summary>
    public static class DateTimeZoneProviderExtensions
    {
        /// <summary>
        /// Returns a lazily-evaluated sequence of time zones from the specified provider,
        /// in the same order in which the IDs are returned by the provider.
        /// </summary>
        /// <param name="provider">The provider to fetch time zones from.</param>
        /// <returns>All the time zones from the provider.</returns>
        public static IEnumerable<DateTimeZone> GetAllZones([NotNull] this IDateTimeZoneProvider provider)
        {
            Preconditions.CheckNotNull(provider, nameof(provider));
            return provider.Ids.Select(id => provider[id]);
        }

        /// <summary>
        /// Dumps the contents of a provider to a writer. This is used in both unit tests and TzdbCompiler, so we can validate
        /// that changes to time zone code don't change behaviour. It's not expected to be exposed publicly; this used to
        /// be in NodaTime.CheckZones, but that didn't really prove its worth. It's currently very fix in format and options;
        /// we can make it more flexible later if we want.
        /// </summary>
        internal static void Dump([NotNull] this IDateTimeZoneProvider provider, [NotNull] TextWriter writer)
        {
            Preconditions.CheckNotNull(provider, nameof(provider));
            Preconditions.CheckNotNull(writer, nameof(writer));
            writer.WriteLine("TZDB version: {0}", provider.VersionId);
            foreach (var id in provider.Ids)
            {
                var zone = provider[id];
                writer.WriteLine($"Zone: {zone.Id}");
                var start = Instant.FromUtc(1800, 1, 1, 0, 0);
                var end = Instant.FromUtc(2100, 1, 1, 0, 0);
                ZoneInterval lastDisplayed = null;

                foreach (var interval in zone.GetZoneIntervals(start, end))
                {
                    writer.WriteLine(interval);
                    lastDisplayed = interval;
                }

                // This will never be null; every interval has at least one zone interval.
                if (lastDisplayed.HasEnd)
                {
                    writer.WriteLine("...");
                    writer.WriteLine(zone.GetZoneInterval(Instant.MaxValue));
                }
                writer.WriteLine();
            }
        }
    }
}
