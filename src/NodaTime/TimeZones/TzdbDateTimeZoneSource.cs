// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using NodaTime.TimeZones.Cldr;
using NodaTime.TimeZones.IO;
using NodaTime.Utility;

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
    public sealed class TzdbDateTimeZoneSource : IDateTimeZoneSource
    {
        /// <summary>
        /// The <see cref="TzdbDateTimeZoneSource"/> initialised from resources within the NodaTime assembly.
        /// </summary>
        public static TzdbDateTimeZoneSource Default { get { return DefaultHolder.builtin; } }

        // Class to enable lazy initialization of the default instance.
        private static class DefaultHolder
        {
            static DefaultHolder() {}

            internal static readonly TzdbDateTimeZoneSource builtin = new TzdbDateTimeZoneSource(LoadDefaultDataSource());

            private static ITzdbDataSource LoadDefaultDataSource()
            {
                var assembly = typeof(DefaultHolder).Assembly;
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
        private readonly ITzdbDataSource source;
        /// <summary>
        /// Map from ID (possibly an alias) to canonical ID. This is a read-only wrapper,
        /// and can be returned directly to clients.
        /// </summary>
        private readonly IDictionary<string, string> timeZoneIdMap;
        /// <summary>
        /// Lookup from canonical ID to aliases.
        /// </summary>
        private readonly ILookup<string, string> aliases;
        /// <summary>
        /// Composite version ID including TZDB and Windows mapping version strings.
        /// </summary>
        private readonly string version;
        /// <summary>
        /// List of zone locations, if any. This is a read-only wrapper, and can be
        /// returned directly to clients. It may be null, if the underlying data source
        /// has no location data.
        /// </summary>
        private readonly IList<TzdbZoneLocation> zoneLocations;
#if !PCL
        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbDateTimeZoneSource" /> class from a resource within
        /// the NodaTime assembly.
        /// </summary>
        /// <remarks>For backwards compatibility, this will use the blob time zone data when given the same
        /// base name which would previously have loaded the now-obsolete resource data.</remarks>
        /// <param name="baseName">The root name of the resource file.</param>
        /// <exception cref="InvalidNodaDataException">The data within the resource is invalid.</exception>
        /// <exception cref="MissingManifestResourceException">The resource set cannot be found.</exception>
        [Obsolete("Use TzdbDateTimeZoneSource.Default to access the only TZDB resources within the NodaTime assembly")]
        public TzdbDateTimeZoneSource(string baseName)
            : this(baseName, Assembly.GetExecutingAssembly())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbDateTimeZoneSource" /> class.
        /// </summary>
        /// <remarks>For backwards compatibility, this will use the blob time zone data when given the same
        /// base name which would previously have loaded the now-obsolete resource data from the Noda Time assembly
        /// itself.</remarks>
        /// <param name="baseName">The root name of the resource file.</param>
        /// <param name="assembly">The assembly to search for the time zone resources.</param>
        /// <exception cref="InvalidNodaDataException">The data within the resource is invalid.</exception>
        /// <exception cref="MissingManifestResourceException">The resource set cannot be found.</exception>
        [Obsolete("The resource format for time zone data is deprecated; future versions will only support blob-based data")]
        public TzdbDateTimeZoneSource(string baseName, Assembly assembly)
            : this(TzdbResourceData.FromResource(baseName, assembly))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbDateTimeZoneSource" /> class.
        /// </summary>
        /// <param name="source">The <see cref="ResourceSet"/> to search for the time zone resources.</param>
        /// <exception cref="InvalidNodaDataException">The data within the resource set is invalid.</exception>
        [Obsolete("The resource format for time zone data is deprecated; future versions will only support blob-based data")]
        public TzdbDateTimeZoneSource(ResourceSet source)
            : this(TzdbResourceData.FromResourceSet(source))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbDateTimeZoneSource" /> class.
        /// </summary>
        /// <param name="manager">The <see cref="ResourceManager"/> to search for the time zone resources.</param>
        /// <exception cref="InvalidNodaDataException">The data within the resource manager is invalid.</exception>
        [Obsolete("The resource format for time zone data is deprecated; future versions will only support blob-based data")]
        public TzdbDateTimeZoneSource(ResourceManager manager)
            : this(TzdbResourceData.FromResourceManager(manager))
        {
        }
#endif

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
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="InvalidNodaDataException">The stream contains invalid time zone data, or data which cannot
        /// be read by this version of Noda Time.</exception>
        /// <exception cref="IOException">Reading from the stream failed.</exception>
        /// <exception cref="InvalidOperationException">The supplied stream doesn't support reading.</exception>
        public static TzdbDateTimeZoneSource FromStream(Stream stream)
        {
            Preconditions.CheckNotNull(stream, "stream");
            return new TzdbDateTimeZoneSource(TzdbStreamData.FromStream(stream));
        }

        private TzdbDateTimeZoneSource(ITzdbDataSource source)
        {
            Preconditions.CheckNotNull(source, "source");
            this.source = source;
            timeZoneIdMap = new NodaReadOnlyDictionary<string, string>(source.TzdbIdMap);
            aliases = timeZoneIdMap
                .Where(pair => pair.Key != pair.Value)
                .OrderBy(pair => pair.Key, StringComparer.Ordinal)
                .ToLookup(pair => pair.Value, pair => pair.Key);
            version = source.TzdbVersion + " (mapping: " + source.WindowsMapping.Version + ")";
            var originalZoneLocations = source.ZoneLocations;
            zoneLocations = originalZoneLocations == null ? null : new ReadOnlyCollection<TzdbZoneLocation>(originalZoneLocations);
        }

        /// <inheritdoc />
        public DateTimeZone ForId(string id)
        {
            string canonicalId;
            if (!timeZoneIdMap.TryGetValue(Preconditions.CheckNotNull(id, "id"), out canonicalId))
            {
                throw new ArgumentException("Time zone with ID " + id + " not found in source " + version, "id");
            }
            return source.CreateZone(id, canonicalId);
        }

        /// <inheritdoc />
        [DebuggerStepThrough]
        public IEnumerable<string> GetIds()
        {
            return timeZoneIdMap.Keys;
        }

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
        public string VersionId { get { return "TZDB: " + version; } }

        /// <inheritdoc />
        public string MapTimeZoneId(TimeZoneInfo zone)
        {
#if PCL
            // Our in-memory mapping is effectively from standard name to TZDB ID.
            string id = zone.StandardName;
#else
            string id = zone.Id;
#endif
            string result;
            source.WindowsMapping.PrimaryMapping.TryGetValue(id, out result);
#if PCL
            if (result == null)
            {
                source.WindowsAdditionalStandardNameToIdMapping.TryGetValue(id, out result);
            }
#endif
            return result;
        }

        /// <summary>
        /// Returns a lookup from canonical time zone ID (e.g. "Europe/London") to a group of aliases for that time zone
        /// (e.g. {"Europe/Belfast", "Europe/Guernsey", "Europe/Jersey", "Europe/Isle_of_Man", "GB", "GB-Eire"}).
        /// </summary>
        /// <remarks>
        /// The group of values for a key never contains the canonical ID, only aliases. Any time zone
        /// ID which is itself an alias or has no aliases linking to it will not be present in the lookup.
        /// The aliases within a group are returned in alphabetical (ordinal) order.
        /// </remarks>
        /// <returns>A lookup from canonical ID to the aliases of that ID.</returns>
        public ILookup<string, string> Aliases { get { return aliases; } }

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
        /// <returns>A map from time zone ID to the canonical ID.</returns>
        public IDictionary<string, string> CanonicalIdMap { get { return timeZoneIdMap; } }

        /// <summary>
        /// Returns a read-only list of zone locations known to this source.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Every zone location's time zone ID is guaranteed to be valid within this source (assuming the source
        /// has been validated).
        /// </para>
        /// <para>
        /// The legacy resource format does not include location information,
        /// and this property will throw an exception if the information is requested. It is expected
        /// that callers who wish to use newer features will not be attempting to use legacy formats
        /// for time zone data.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">This is a legacy resource-based data source which does
        /// not include location information.</exception>
        public IList<TzdbZoneLocation> ZoneLocations
        {
            get
            {
                if (zoneLocations == null)
                {
                    throw new InvalidOperationException("Zone location information is not available in the legacy resource format");
                }
                return zoneLocations;
            }
        }

        /// <summary>
        /// Returns just the TZDB version (e.g. "2013a") of the source data.
        /// </summary>
        public string TzdbVersion { get { return source.TzdbVersion; } }

        /// <summary>
        /// Gets the Windows time zone mapping information provided in the CLDR
        /// supplemental "windowsZones.xml" file.
        /// </summary>
        public WindowsZones WindowsMapping { get { return source.WindowsMapping; } }

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
                string canonical;
                if (!CanonicalIdMap.TryGetValue(entry.Value, out canonical))
                {
                    throw new InvalidNodaDataException("Mapping for entry " + entry.Key + " (" + entry.Value + ") is missing");
                }
                if (entry.Value != canonical)
                {
                    throw new InvalidNodaDataException("Mapping for entry " + entry.Key + " (" + entry.Value + ") is not canonical ("
                        + entry.Value + " maps to " + canonical);
                }
            }

            // Check that every Windows mapping has a primary territory
            foreach (var mapZone in source.WindowsMapping.MapZones)
            {
                // Simplest way of checking is to find the primary mapping...
                if (!source.WindowsMapping.PrimaryMapping.ContainsKey(mapZone.WindowsId))
                {
                    throw new InvalidNodaDataException("Windows mapping for standard ID " + mapZone.WindowsId + " has no primary territory");
                }
            }

            // Check that each Windows mapping has a known canonical ID.
            foreach (var mapZone in source.WindowsMapping.MapZones)
            {
                foreach (var id in mapZone.TzdbIds)
                {
                    if (!CanonicalIdMap.ContainsKey(id))
                    {
                        throw new InvalidNodaDataException("Windows mapping uses canonical ID " + id + " which is missing");
                    }
                }
            }

            // Check that each additional Windows standard name mapping has a known canonical ID.
            var additionalMappings = source.WindowsAdditionalStandardNameToIdMapping;
            if (additionalMappings != null)
            {
                foreach (var id in additionalMappings.Values)
                {
                    if (!CanonicalIdMap.ContainsKey(id))
                    {
                        throw new InvalidNodaDataException("Windows additional standard name mapping uses canonical ID " + id + " which is missing");
                    }
                }
            }

            // Check that each zone location has a valid zone ID
            if (zoneLocations != null)
            {
                foreach (var location in zoneLocations)
                {
                    if (!CanonicalIdMap.ContainsKey(location.ZoneId))
                    {
                        throw new InvalidNodaDataException("Zone location " + location.CountryName
                            + " uses zone ID " + location.ZoneId + " which is missing");
                    }
                }
            }
        }
    }
}
