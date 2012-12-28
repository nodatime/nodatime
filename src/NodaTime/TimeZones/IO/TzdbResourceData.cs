#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2013 Jon Skeet
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

#if !PCL

using NodaTime.Utility;
using System.Collections.Generic;
using System.IO;
using System.Resources;

namespace NodaTime.TimeZones.IO
{
    /// <summary>
    /// Matching class for TzdbResourceWriter in the ZoneInfoCompiler project. This
    /// knows how to read TZDB information from a .NET resource file.
    /// </summary>
    internal sealed class TzdbResourceData : ITzdbDataSource
    {
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

        private readonly string tzdbVersion;
        private readonly string windowsMappingVersion;
        private readonly IDictionary<string, string> tzdbIdMap;
        private readonly IDictionary<string, string> windowsMapping;
        private readonly ResourceSet source;

        internal TzdbResourceData(ResourceSet source)
        {
            this.source = source;
            tzdbIdMap = LoadDictionary(source, IdMapKey);
            if (tzdbIdMap == null)
            {
                throw new InvalidDataException("No map with key " + IdMapKey + " in resource");
            }
            windowsMapping = LoadDictionary(source, WindowsToPosixMapKey);
            if (windowsMapping == null)
            {
                throw new InvalidDataException("No map with key " + WindowsToPosixMapKey + " in resource");
            }
            tzdbVersion = source.GetString(VersionKey);
            windowsMappingVersion = source.GetString(WindowsToPosixMapVersionKey);
        }

        /// <inheritdoc />
        public string TzdbVersion { get { return tzdbVersion; } }

        /// <inheritdoc />
        public string WindowsMappingVersion { get { return windowsMappingVersion; } }

        /// <inheritdoc />
        public IDictionary<string, string> TzdbIdMap { get { return tzdbIdMap; } }

        /// <inheritdoc />
        public IDictionary<string, string> WindowsMapping { get { return windowsMapping; } }

        /// <inheritdoc />
        public DateTimeZone CreateZone(string id, string canonicalId)
        {
            object obj = source.GetObject(ResourceHelper.NormalizeAsResourceName(canonicalId));
            // We should never be asked for time zones which don't exist.
            Preconditions.CheckArgument(obj != null, "canonicalId", "ID is not one of the recognized time zone identifiers within this resource");
            byte[] bytes = (byte[])obj;
            using (var stream = new MemoryStream(bytes))
            {
                var reader = new DateTimeZoneReader(stream, null);
                return reader.ReadTimeZone(id);
            }
        }

        /// <summary>
        /// Loads a dictionary of string to string with the given name from the given resource manager.
        /// </summary>
        /// <param name="source">The <see cref="ResourceSet"/> to load from.</param>
        /// <param name="name">The resource name.</param>
        /// <returns>The <see cref="IDictionary{TKey,TValue}"/> or null if there is no such resource.</returns>
        private static IDictionary<string, string> LoadDictionary(ResourceSet source, string name)
        {
            Preconditions.CheckNotNull(source, "source");
            var bytes = source.GetObject(name) as byte[];
            if (bytes != null)
            {
                using (var stream = new MemoryStream(bytes))
                {
                    var reader = new DateTimeZoneReader(stream, null);
                    return reader.ReadDictionary();
                }
            }
            return null;
        }

    }
}

#endif
