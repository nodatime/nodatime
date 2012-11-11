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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides an implementation of a <see cref="IDateTimeZoneSource" /> that looks
    /// for its time zone definitions from a named resource in an assembly.
    /// </summary>
    /// <remarks>
    /// All calls to <see cref="ForId"/> for fixed-offset IDs advertised by the source (i.e. "UTC" and "UTC+/-Offset")
    /// will return zones equal to those returned by <see cref="DateTimeZone.ForOffset"/>.
    /// </remarks>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    public sealed class TzdbDateTimeZoneSource : IDateTimeZoneSource
    {
        /// <summary>
        /// A source initialised with the built-in version of TZDB.
        /// </summary>
        private static readonly TzdbDateTimeZoneSource builtin = new TzdbDateTimeZoneSource("NodaTime.TimeZones.Tzdb");

        /// <summary>
        /// The resource key for the Windows to TZDB ID mapping dictionary.
        /// This resource key contains hyphens, so cannot conflict with a time zone name.
        /// </summary>
        internal const string WindowsToPosixMapKey = "--meta-WindowsToPosix";

        /// <summary>
        /// The resource key for the Windows to TZDB ID mapping version string.
        /// This resource key contains hyphens, so cannot conflict with a time zone name.
        /// </summary>
        internal const string WindowsToPosixMapVersionKey = "--meta-WindowsToPosixVersion";

        /// <summary>
        /// The resource key for the timezone ID alias dictionary.
        /// This resource key contains hyphens, so cannot conflict with a time zone name.
        /// </summary>
        internal const string IdMapKey = "--meta-IdMap";

        /// <summary>
        /// The resource key for the TZDB version ID.
        /// This resource key contains hyphens, so cannot conflict with a time zone name.
        /// </summary>
        internal const string VersionKey = "--meta-VersionId";

        private readonly ResourceSet source;

        /// <summary>
        /// Map from ID (possibly an alias) to canonical ID.
        /// </summary>
        private readonly IDictionary<string, string> timeZoneIdMap;

        private readonly IDictionary<string, string> windowsIdMap;
        private readonly ILookup<string, string> aliases;
        private readonly string version;

        /// <summary>
        /// The <see cref="TzdbDateTimeZoneSource"/> initialised from resources within the NodaTime assembly.
        /// </summary>
        public static TzdbDateTimeZoneSource Default { get { return builtin; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbDateTimeZoneSource" /> class from a resource within
        /// the NodaTime assembly.
        /// </summary>
        /// <param name="baseName">The root name of the resource file.</param>
        public TzdbDateTimeZoneSource(string baseName)
            : this(baseName, Assembly.GetExecutingAssembly())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbDateTimeZoneSource" /> class.
        /// </summary>
        /// <param name="baseName">The root name of the resource file.</param>
        /// <param name="assembly">The assembly to search for the time zone resources.</param>
        public TzdbDateTimeZoneSource(string baseName, Assembly assembly)
            : this(new ResourceManager(baseName, assembly))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbDateTimeZoneSource" /> class.
        /// </summary>
        /// <param name="source">The <see cref="ResourceManager"/> to search for the time zone resources.</param>
        public TzdbDateTimeZoneSource(ResourceManager source)
            : this(ResourceHelper.GetDefaultResourceSet(source))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbDateTimeZoneSource" /> class.
        /// </summary>
        /// <param name="source">The <see cref="ResourceSet"/> to search for the time zone resources.</param>
        public TzdbDateTimeZoneSource(ResourceSet source)
        {
            this.source = source;
            var mutableIdMap = ResourceHelper.LoadDictionary(source, IdMapKey);
            if (mutableIdMap == null)
            {
                throw new InvalidDataException("No map with key " + IdMapKey + " in resource");
            }
            timeZoneIdMap = new NodaReadOnlyDictionary<string, string>(mutableIdMap);
            aliases = timeZoneIdMap
                .Where(pair => pair.Key != pair.Value)
                .OrderBy(pair => pair.Key, StringComparer.Ordinal)
                .ToLookup(pair => pair.Value, pair => pair.Key);
            windowsIdMap = ResourceHelper.LoadDictionary(source, WindowsToPosixMapKey);
            // TODO(Post-V1): Consider forming inverse map too.
            if (windowsIdMap == null)
            {
                throw new InvalidDataException("No map with key " + WindowsToPosixMapKey + " in resource");
            }
            this.version = source.GetString(VersionKey) + " (mapping: " + source.GetString(WindowsToPosixMapVersionKey) + ")";
        }

        /// <summary>
        /// Returns the time zone definition associated with the given id.
        /// </summary>
        /// <param name="id">The id of the time zone to return.</param>
        /// <returns>
        /// The <see cref="DateTimeZone"/> or null if there is no time zone with the given id.
        /// </returns>
        public DateTimeZone ForId(string id)
        {
            string canonicalId;
            if (!timeZoneIdMap.TryGetValue(id, out canonicalId))
            {
                throw new ArgumentException("Time zone with ID " + id + " not found in source " + version, "id");
            }
            return ResourceHelper.LoadTimeZone(source, canonicalId, id);
        }

        /// <summary>
        /// Returns a sequence of the available IDs from this source.
        /// </summary>
        [DebuggerStepThrough]
        public IEnumerable<string> GetIds()
        {
            return timeZoneIdMap.Keys;
        }

        /// <summary>
        /// Returns a version identifier for this source.
        /// </summary>
        public string VersionId { get { return "TZDB: " + version; } }

        /// <summary>
        /// Attempts to map the system time zone to a zoneinfo ID, and return that ID.
        /// </summary>
        public string MapTimeZoneId(TimeZoneInfo zone)
        {
            string result;
            windowsIdMap.TryGetValue(zone.Id, out result);
            return result;
        }

        /// <summary>
        /// Returns a lookup from canonical ID (e.g. "Europe/London") to a group of aliases
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
    }
}
