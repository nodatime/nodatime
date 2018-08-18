// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.TimeZones.Cldr;
using NodaTime.TimeZones.IO;
using NodaTime.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides an implementation of <see cref="IDateTimeZoneSource" /> that loads data originating from the
    /// <a href="http://www.iana.org/time-zones">tz database</a> (also known as the IANA Time Zone database, or zoneinfo
    /// or Olson database).
    /// </summary>
    /// <remarks>
    /// All calls to <see cref="ForId"/> for fixed-offset IDs advertised by the source (i.e. "UTC" and "UTC+/-Offset")
    /// will return zones equal to those returned by <see cref="DateTimeZone.ForOffset"/>.
    /// </remarks>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    [Immutable]
    public sealed class TzdbDateTimeZoneSource : IDateTimeZoneSource
    {
        /// <summary>
        /// Gets the <see cref="TzdbDateTimeZoneSource"/> initialised from resources within the NodaTime assembly.
        /// </summary>
        /// <value>The source initialised from resources within the NodaTime assembly.</value>
        [NotNull] public static TzdbDateTimeZoneSource Default => DefaultHolder.builtin;

        // Class to enable lazy initialization of the default instance.
        private static class DefaultHolder
        {
            static DefaultHolder() {}

            internal static readonly TzdbDateTimeZoneSource builtin = new TzdbDateTimeZoneSource(LoadDefaultDataSource());

            private static TzdbStreamData LoadDefaultDataSource()
            {
                var assembly = typeof(DefaultHolder).GetTypeInfo().Assembly;
                using (Stream stream = assembly.GetManifestResourceStream("NodaTime.TimeZones.Tzdb.nzd"))
                {
                    return TzdbStreamData.FromStream(stream);
                }
            }
        }

        /// <summary>
        /// Original source data - we delegate to this to create actual DateTimeZone instances,
        /// and for windows mappings.
        /// </summary>
        private readonly TzdbStreamData source;

        /// <summary>
        /// Composite version ID including TZDB and Windows mapping version strings.
        /// </summary>
        private readonly string version;

        /// <summary>
        /// Gets a lookup from canonical time zone ID (e.g. "Europe/London") to a group of aliases for that time zone
        /// (e.g. {"Europe/Belfast", "Europe/Guernsey", "Europe/Jersey", "Europe/Isle_of_Man", "GB", "GB-Eire"}).
        /// </summary>
        /// <remarks>
        /// The group of values for a key never contains the canonical ID, only aliases. Any time zone
        /// ID which is itself an alias or has no aliases linking to it will not be present in the lookup.
        /// The aliases within a group are returned in alphabetical (ordinal) order.
        /// </remarks>
        /// <value>A lookup from canonical ID to the aliases of that ID.</value>
        [NotNull] public ILookup<string, string> Aliases { get; }

        /// <summary>
        /// Returns a read-only map from time zone ID to the canonical ID. For example, the key "Europe/Jersey"
        /// would be associated with the value "Europe/London".
        /// </summary>
        /// <remarks>
        /// <para>This map contains an entry for every ID returned by <see cref="GetIds"/>, where
        /// canonical IDs map to themselves.</para>
        /// <para>The returned map is read-only; any attempts to call a mutating method will throw
        /// <see cref="NotSupportedException" />.</para>
        /// </remarks>
        /// <value>A map from time zone ID to the canonical ID.</value>
        [NotNull] public IDictionary<string, string> CanonicalIdMap { get; }

        /// <summary>
        /// Gets a read-only list of zone locations known to this source, or null if the original source data
        /// does not include zone locations.
        /// </summary>
        /// <remarks>
        /// Every zone location's time zone ID is guaranteed to be valid within this source (assuming the source
        /// has been validated).
        /// </remarks>
        /// <value>A read-only list of zone locations known to this source.</value>
        [CanBeNull] public IList<TzdbZoneLocation> ZoneLocations { get; }

        /// <summary>
        /// Gets a read-only list of "zone 1970" locations known to this source, or null if the original source data
        /// does not include zone locations.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This location data differs from <see cref="ZoneLocations"/> in two important respects:
        /// <ul>
        ///   <li>Where multiple similar zones exist but only differ in transitions before 1970,
        ///     this location data chooses one zone to be the canonical "post 1970" zone.
        ///   </li>
        ///   <li>
        ///     This location data can represent multiple ISO-3166 country codes in a single entry. For example,
        ///     the entry corresponding to "Europe/London" includes country codes GB, GG, JE and IM (Britain,
        ///     Guernsey, Jersey and the Isle of Man, respectively).
        ///   </li>
        /// </ul>
        /// </p>
        /// <p>
        /// Every zone location's time zone ID is guaranteed to be valid within this source (assuming the source
        /// has been validated).
        /// </p>
        /// </remarks>
        /// <value>A read-only list of zone locations known to this source.</value>
        [CanBeNull] public IList<TzdbZone1970Location> Zone1970Locations { get; }

        /// <inheritdoc />
        /// <remarks>
        /// <para>
        /// This source returns a string such as "TZDB: 2013b (mapping: 8274)" corresponding to the versions of the tz
        /// database and the CLDR Windows zones mapping file.
        /// </para>
        /// <para>
        /// Note that there is no need to parse this string to extract any of the above information, as it is available
        /// directly from the <see cref="TzdbVersion"/> and <see cref="WindowsZones.Version"/> properties.
        /// </para>
        /// </remarks>
        [NotNull] public string VersionId => "TZDB: " + version;

        /// <summary>
        /// Creates an instance from a stream in the custom Noda Time format. The stream must be readable.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The stream is not closed by this method, but will be read from
        /// without rewinding. A successful call will read the stream to the end.
        /// </para>
        /// <para>
        /// See the user guide for instructions on how to generate an updated time zone database file from a copy of the
        /// (textual) tz database.
        /// </para>
        /// </remarks>
        /// <param name="stream">The stream containing time zone data</param>
        /// <returns>A <c>TzdbDateTimeZoneSource</c> providing information from the given stream.</returns>
        /// <exception cref="InvalidNodaDataException">The stream contains invalid time zone data, or data which cannot
        /// be read by this version of Noda Time.</exception>
        /// <exception cref="IOException">Reading from the stream failed.</exception>
        /// <exception cref="InvalidOperationException">The supplied stream doesn't support reading.</exception>
        [NotNull] public static TzdbDateTimeZoneSource FromStream([NotNull] Stream stream)
        {
            Preconditions.CheckNotNull(stream, nameof(stream));
            return new TzdbDateTimeZoneSource(TzdbStreamData.FromStream(stream));
        }

        private TzdbDateTimeZoneSource([NotNull] TzdbStreamData source)
        {
            Preconditions.CheckNotNull(source, nameof(source));
            this.source = source;
            CanonicalIdMap = new NodaReadOnlyDictionary<string, string>(source.TzdbIdMap);
            Aliases = CanonicalIdMap
                .Where(pair => pair.Key != pair.Value)
                .OrderBy(pair => pair.Key, StringComparer.Ordinal)
                .ToLookup(pair => pair.Value, pair => pair.Key);
            version = source.TzdbVersion + " (mapping: " + source.WindowsMapping.Version + ")";
            var originalZoneLocations = source.ZoneLocations;
            ZoneLocations = originalZoneLocations == null ? null : new ReadOnlyCollection<TzdbZoneLocation>(originalZoneLocations);
            var originalZone1970Locations = source.Zone1970Locations;
            Zone1970Locations = originalZone1970Locations == null ? null : new ReadOnlyCollection<TzdbZone1970Location>(originalZone1970Locations);
        }

        /// <inheritdoc />
        [NotNull] public DateTimeZone ForId([NotNull] string id)
        {
            if (!CanonicalIdMap.TryGetValue(Preconditions.CheckNotNull(id, nameof(id)), out string canonicalId))
            {
                throw new ArgumentException("Time zone with ID " + id + " not found in source " + version, nameof(id));
            }
            return source.CreateZone(id, canonicalId);
        }

        /// <inheritdoc />
        [DebuggerStepThrough]
        [NotNull] public IEnumerable<string> GetIds() => CanonicalIdMap.Keys;

        /// <inheritdoc />
        [CanBeNull] public string GetSystemDefaultId() => MapTimeZoneInfoId(TimeZoneInfoInterceptor.Local);

        [VisibleForTesting, CanBeNull]
        internal string MapTimeZoneInfoId([CanBeNull] TimeZoneInfo timeZone)
        {
            // Unusual, but can happen in some Mono installations.
            if (timeZone == null)
            {
                return null;
            }
            string id = timeZone.Id;
            // First see if it's a Windows time zone ID.
            if (source.WindowsMapping.PrimaryMapping.TryGetValue(id, out string result))
            {
                return result;
            }
            // Next see if it's already a TZDB ID (e.g. .NET Core running on Linux or Mac).
            if (CanonicalIdMap.Keys.Contains(id))
            {
                return id;
            }
            // Maybe it's a Windows zone we don't have a mapping for, or we're on a Mono system
            // where TimeZoneInfo.Local.Id returns "Local" but can actually do the mappings.
            return GuessZoneIdByTransitions(timeZone);
        }

        private readonly ConcurrentDictionary<string, string> guesses = new ConcurrentDictionary<string, string>();

        // Cache around GuessZoneIdByTransitionsUncached
        private string GuessZoneIdByTransitions(TimeZoneInfo zone)
        {
            // FIXME: Stop using StandardName! (We have Id now...)
            return guesses.GetOrAdd(zone.StandardName, _ =>
            {
                // Build the list of candidates here instead of within the method, so that
                // tests can pass in the same list on each iteration.
                var candidates = CanonicalIdMap.Values.Select(ForId).ToList();
                return GuessZoneIdByTransitionsUncached(zone, candidates);
            });
        }

        /// <summary>
        /// In cases where we can't get a zone mapping directly, we try to work out a good fit
        /// by checking the transitions within the next few years.
        /// This can happen if the Windows data isn't up-to-date, or if we're on a system where
        /// TimeZoneInfo.Local.Id just returns "local", or if TimeZoneInfo.Local is a custom time
        /// zone for some reason. We return null if we don't get a 70% hit rate.
        /// We look at all transitions in all canonical IDs for the next 5 years.
        /// Heuristically, this seems to be good enough to get the right results in most cases.
        /// This method used to only be called in the PCL build in 1.x, but it seems reasonable enough to
        /// call it if we can't get an exact match anyway.
        /// </summary>
        /// <param name="zone">Zone to resolve in a best-effort fashion.</param>
        /// <param name="candidates">All the Noda Time zones to consider - normally a list 
        /// obtained from this source.</param>
        internal string GuessZoneIdByTransitionsUncached(TimeZoneInfo zone, List<DateTimeZone> candidates)
        {
            // See https://github.com/nodatime/nodatime/issues/686 for performance observations.
            // Very rare use of the system clock! Windows time zone updates sometimes sacrifice past
            // accuracy for future accuracy, so let's use the current year's transitions.
            int thisYear = SystemClock.Instance.GetCurrentInstant().InUtc().Year;
            Instant startOfThisYear = Instant.FromUtc(thisYear, 1, 1, 0, 0);
            Instant startOfNextYear = Instant.FromUtc(thisYear + 5, 1, 1, 0, 0);
            var instants = candidates.SelectMany(z => z.GetZoneIntervals(startOfThisYear, startOfNextYear))
                                     .Select(zi => Instant.Max(zi.RawStart, startOfThisYear)) // Clamp to start of interval
                                     .Distinct()
                                     .ToList();
            var bclOffsets = instants.Select(instant => Offset.FromTimeSpan(zone.GetUtcOffset(instant.ToDateTimeUtc()))).ToList();
            // For a zone to be mappable, at most 30% of the checks must fail
            // - so if we get to that number (or whatever our "best" so far is)
            // we know we can stop for any particular zone.
            int lowestFailureScore = (instants.Count * 30) / 100;
            DateTimeZone bestZone = null;
            foreach (var candidate in candidates)
            {
                int failureScore = 0;
                for (int i = 0; i < instants.Count; i++)
                {
                    if (candidate.GetUtcOffset(instants[i]) != bclOffsets[i])
                    {
                        failureScore++;
                        if (failureScore == lowestFailureScore)
                        {
                            break;
                        }
                    }
                }
                if (failureScore < lowestFailureScore)
                {
                    lowestFailureScore = failureScore;
                    bestZone = candidate;
                }
            }
            return bestZone?.Id;
        }

        /// <summary>
        /// Gets just the TZDB version (e.g. "2013a") of the source data.
        /// </summary>
        /// <value>The TZDB version (e.g. "2013a") of the source data.</value>
        [NotNull] public string TzdbVersion => source.TzdbVersion;

        /// <summary>
        /// Gets the Windows time zone mapping information provided in the CLDR
        /// supplemental "windowsZones.xml" file.
        /// </summary>
        /// <value>The Windows time zone mapping information provided in the CLDR
        /// supplemental "windowsZones.xml" file.</value>
        [NotNull] public WindowsZones WindowsMapping => source.WindowsMapping;

        /// <summary>
        /// Validates that the data within this source is consistent with itself.
        /// </summary>
        /// <remarks>
        /// Source data is not validated automatically when it's loaded, but any source
        /// loaded from data produced by <c>NodaTime.TzdbCompiler</c> (including the data shipped with Noda Time)
        /// will already have been validated via this method when it was originally produced. This method should
        /// only normally be called explicitly if you have data from a source you're unsure of.
        /// </remarks>
        /// <exception cref="InvalidNodaDataException">The source data is invalid. The source may not function
        /// correctly.</exception>
        public void Validate()
        {
            // Check that each entry has a canonical value. (Every mapping x to y
            // should be such that y maps to itself.)
            foreach (var entry in this.CanonicalIdMap)
            {
                if (!CanonicalIdMap.TryGetValue(entry.Value, out string canonical))
                {
                    throw new InvalidNodaDataException(
                        "Mapping for entry {entry.Key} ({entry.Value}) is missing");
                }
                if (entry.Value != canonical)
                {
                    throw new InvalidNodaDataException(
                        "Mapping for entry {entry.Key} ({entry.Value}) is not canonical ({entry.Value} maps to {canonical}");
                }
            }

            // Check that every Windows mapping has a primary territory
            foreach (var mapZone in source.WindowsMapping.MapZones)
            {
                // Simplest way of checking is to find the primary mapping...
                if (!source.WindowsMapping.PrimaryMapping.ContainsKey(mapZone.WindowsId))
                {
                    throw new InvalidNodaDataException(
                        $"Windows mapping for standard ID {mapZone.WindowsId} has no primary territory");
                }
            }

            // Check that each Windows mapping has a known canonical ID.
            foreach (var mapZone in source.WindowsMapping.MapZones)
            {
                foreach (var id in mapZone.TzdbIds)
                {
                    if (!CanonicalIdMap.ContainsKey(id))
                    {
                        throw new InvalidNodaDataException(
                            $"Windows mapping uses canonical ID {id} which is missing");
                    }
                }
            }

            // Check that each zone location has a valid zone ID
            if (ZoneLocations != null)
            {
                foreach (var location in ZoneLocations)
                {
                    if (!CanonicalIdMap.ContainsKey(location.ZoneId))
                    {
                        throw new InvalidNodaDataException(
                            $"Zone location {location.CountryName} uses zone ID {location.ZoneId} which is missing");
                    }
                }
            }
            if (Zone1970Locations != null)
            {
                foreach (var location in Zone1970Locations)
                {
                    if (!CanonicalIdMap.ContainsKey(location.ZoneId))
                    {
                        throw new InvalidNodaDataException(
                            $"Zone 1970 location {location.Countries[0].Name} uses zone ID {location.ZoneId} which is missing");
                    }
                }
            }
        }
    }
}
