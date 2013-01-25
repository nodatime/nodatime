// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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
                var writer = new LegacyDateTimeZoneWriter(stream, null);
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
                var writer = new LegacyDateTimeZoneWriter(stream, null);
                writer.WriteTimeZone(timeZone);
                resourceWriter.AddResource(normalizedId, stream.ToArray());
            }
        }
    }
}
