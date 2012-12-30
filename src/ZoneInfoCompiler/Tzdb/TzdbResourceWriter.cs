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

using System.Collections.Generic;
using System.IO;
using System.Resources;
using NodaTime.TimeZones;
using NodaTime.Utility;
using NodaTime.TimeZones.IO;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    /// ITzdbWriter implementation which writes to a BCL resource file (e.g. resx).
    /// </summary>
    internal class TzdbResourceWriter : ITzdbWriter
    {
        private readonly IResourceWriter resourceWriter;

        internal TzdbResourceWriter(IResourceWriter resourceWriter)
        {
            this.resourceWriter = resourceWriter;
        }

        public void Write(TzdbDatabase database, WindowsMapping mapping)
        {
            var timeZoneMap = new Dictionary<string, string>();
            foreach (var zone in database.GenerateDateTimeZones())
            {
                timeZoneMap.Add(zone.Id, zone.Id);
                WriteTimeZone(zone);
            }

            // Normalize the aliases
            foreach (var key in database.Aliases.Keys)
            {
                var value = database.Aliases[key];
                while (database.Aliases.ContainsKey(value))
                {
                    value = database.Aliases[value];
                }
                timeZoneMap.Add(key, value);
            }
            resourceWriter.AddResource(TzdbResourceData.VersionKey, database.Version);
            WriteDictionary(TzdbResourceData.IdMapKey, timeZoneMap);
            WriteDictionary(TzdbResourceData.WindowsToPosixMapKey, mapping.WindowsToTzdbIds);
            resourceWriter.AddResource(TzdbResourceData.WindowsToPosixMapVersionKey, mapping.Version);
            resourceWriter.Close();
        }

        /// <summary>
        ///   Writes dictionary of string to string to  a resource with the given name.
        /// </summary>
        /// <param name="name">The resource name.</param>
        /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}" /> to write.</param>
        private void WriteDictionary(string name, IDictionary<string, string> dictionary)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new DateTimeZoneWriter(stream, null);
                writer.WriteDictionary(dictionary);
                resourceWriter.AddResource(name, stream.ToArray());
            }
        }

        /// <summary>
        /// Writes a time zone to a resource with the time zone ID, normalized.
        /// </summary>
        /// <param name="timeZone">The <see cref="DateTimeZone" /> to write.</param>
        private void WriteTimeZone(DateTimeZone timeZone)
        {
            string normalizedId = TzdbResourceData.NormalizeAsResourceName(timeZone.Id);
            using (var stream = new MemoryStream())
            {
                var writer = new DateTimeZoneWriter(stream, null);
                writer.WriteTimeZone(timeZone);
                resourceWriter.AddResource(normalizedId, stream.ToArray());
            }
        }
    }
}
