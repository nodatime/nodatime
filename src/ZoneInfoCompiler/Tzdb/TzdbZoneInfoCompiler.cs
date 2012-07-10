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
using System.IO;
using NodaTime.TimeZones;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    ///   Provides a compiler for Olson (TZDB) zone info files into the internal format used by Noda
    ///   Time for its <see cref="DateTimeZone" /> definitions. This read a set of files and generates
    ///   a resource file with the compiled contents suitable for reading with <see cref="TzdbTimeZoneSource" /> or one of its variants.
    /// </summary>
    public class TzdbZoneInfoCompiler
    {
        private readonly ILog log;
        private readonly TzdbZoneInfoParser tzdbParser;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TzdbZoneInfoCompiler" /> class.
        /// </summary>
        /// <param name="log">The log to send all output messages to.</param>
        public TzdbZoneInfoCompiler(ILog log)
        {
            this.log = log;
            tzdbParser = new TzdbZoneInfoParser(this.log);
        }

        /// <summary>
        ///   Adds a recurring savings rule to the time zone builder.
        /// </summary>
        /// <param name="builder">The <see cref="DateTimeZoneBuilder" /> to add to.</param>
        /// <param name="nameFormat">The name format pattern.</param>
        /// <param name="ruleSet">The <see cref="ZoneRecurrenceCollection" /> describing the recurring savings.</param>
        private static void AddRecurring(DateTimeZoneBuilder builder, String nameFormat, IEnumerable<ZoneRule> ruleSet)
        {
            foreach (var rule in ruleSet)
            {
                string name = rule.FormatName(nameFormat);
                builder.AddRecurringSavings(rule.Recurrence.WithName(name));
            }
        }

        /// <summary>
        ///   Compiles the specified files and generates the output resource file.
        /// </summary>
        /// <param name="fileList">The enumeration of <see cref="FileInfo" /> objects.</param>
        /// <param name="output">The destination <see cref="DirectoryInfo" /> object.</param>
        /// <returns></returns>
        internal int Compile(string version, IEnumerable<FileInfo> fileList, ResourceOutput output)
        {
            var database = new TzdbDatabase(version);
            ParseAllFiles(fileList, database);
            GenerateDateTimeZones(database, output);
            LogCounts(database);
            return 0;
        }

        /// <summary>
        ///   Returns a newly created <see cref="DateTimeZone" /> built from the given time zone data.
        /// </summary>
        /// <param name="zoneList">The time zone definition parts to add.</param>
        /// <param name="ruleSets">The rule sets map to use in looking up rules for the time zones..</param>
        private static DateTimeZone CreateTimeZone(ZoneList zoneList, IDictionary<string, IList<ZoneRule>> ruleSets)
        {
            var builder = new DateTimeZoneBuilder();
            foreach (Zone zone in zoneList)
            {
                builder.SetStandardOffset(zone.Offset);
                if (zone.Rules == null)
                {
                    builder.SetFixedSavings(zone.Format, Offset.Zero);
                }
                else
                {
                    IList<ZoneRule> ruleSet;
                    if (ruleSets.TryGetValue(zone.Rules, out ruleSet))
                    {
                        AddRecurring(builder, zone.Format, ruleSet);
                    }
                    else
                    {
                        try
                        {
                            // Check if Rules actually just refers to a savings.
                            var savings = ParserHelper.ParseOffset(zone.Rules);
                            builder.SetFixedSavings(zone.Format, savings);
                        }
                        catch (FormatException)
                        {
                            throw new ArgumentException(
                                string.Format("Daylight savings rule name '{0}' for zone {1} is neither a known ruleset nor a fixed offset", 
                                    zone.Rules, zone.Name));
                        }                        
                    }
                }
                if (zone.UntilYear == Int32.MaxValue)
                {
                    break;
                }
                builder.AddCutover(zone.UntilYear, zone.UntilYearOffset);
            }
            return builder.ToDateTimeZone(zoneList.Name);
        }

        public int Execute(string sourceDirectoryName, ResourceOutput output)
        {
            log.Info("Starting compilation of directory {0}", sourceDirectoryName);
            var sourceDirectory = new DirectoryInfo(sourceDirectoryName);
            var fileList = sourceDirectory.GetFiles();
            //// Using this conditional code makes debugging simpler in Visual Studio because exceptions will
            //// be caught by VS and shown with the exception visualizer.
#if DEBUG
            Compile(sourceDirectory.Name, fileList, output);
#else
            try
            {
                Compile(sourceDirectory.Name, fileList, output);
            }
            catch (Exception e)
            {
                log.Error("{0}", e);
                return 2;
            }
#endif
            log.Info("Done compiling time zones.");
            return 0;
        }

        /// <summary>
        ///   Generates the date time zones from the given parsed time zone information object.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     First we go through the list of time zones and generate an <see cref="DateTimeZone" />
        ///     object for each one. We create a mapping between the time zone name and itself (for
        ///     writing out later). Then we write out the time zone as a resource to the current writer.
        ///   </para>
        ///   <para>
        ///     Second we go through all of the alias mappings and find the actual time zone that they
        ///     map to. we do this by redirecting through aliases until there are no more aliases. This
        ///     allows for on alias to refer to another. We add the alias mapping to the time zone
        ///     mapping created in the first step. When done, we write out the entire mapping as a
        ///     resource. The keys of the mapping can be used as the list of valid time zone ids
        ///     supported by this resource file.
        ///   </para>
        /// </remarks>
        /// <param name="database">The database of parsed zone info records.</param>
        /// <param name="output">The output file <see cref="ResourceOutput" />.</param>
        private static void GenerateDateTimeZones(TzdbDatabase database, ResourceOutput output)
        {
            var timeZoneMap = new Dictionary<string, string>();
            foreach (var zoneList in database.Zones)
            {
                var timeZone = CreateTimeZone(zoneList, database.Rules);
                timeZoneMap.Add(timeZone.Id, timeZone.Id);
                output.WriteTimeZone(timeZone);
            }

            foreach (var key in database.Aliases.Keys)
            {
                var value = database.Aliases[key];
                while (database.Aliases.ContainsKey(value))
                {
                    value = database.Aliases[value];
                }
                timeZoneMap.Add(key, value);
            }
            output.WriteString(TzdbTimeZoneSource.VersionKey, database.Version);
            output.WriteDictionary(TzdbTimeZoneSource.IdMapKey, timeZoneMap);
        }

        /// <summary>
        ///   Writes various informational counts to the log.
        /// </summary>
        /// <param name="database">The database to query for the counts.</param>
        private void LogCounts(TzdbDatabase database)
        {
            log.Info("=======================================");
            log.Info("Rule sets: {0:D}", database.Rules.Count);
            log.Info("Zones:     {0:D}", database.Zones.Count);
            log.Info("Aliases:   {0:D}", database.Aliases.Count);
            log.Info("=======================================");
        }


        /// <summary>
        ///   Parses all of the given files.
        /// </summary>
        /// <param name="files">The <see cref="IEnumerable{T}" /> of <see cref="FileInfo" /> objects.</param>
        /// <param name="database">The <see cref="TzdbDatabase" /> where the parsed data is placed.</param>
        private void ParseAllFiles(IEnumerable<FileInfo> files, TzdbDatabase database)
        {
            foreach (var file in files)
            {
                log.Info("Parsing file {0} . . .", file.Name);
                ParseFile(file, database);
            }
        }

        /// <summary>
        ///   Parses the file defined by the given <see cref="FileInfo" />.
        /// </summary>
        /// <remarks>
        ///   Currently this compiler only handles files in the Olson (TZDB) zone info format.
        /// </remarks>
        /// <param name="file">The file to parse.</param>
        /// <param name="database">The <see cref="TzdbDatabase" /> where the parsed data is placed.</param>
        internal void ParseFile(FileInfo file, TzdbDatabase database)
        {
            log.FileName = file.Name;
            try
            {
                using (FileStream stream = file.OpenRead())
                {
                    tzdbParser.Parse(stream, database);
                }
            }
            finally
            {
                log.FileName = null;
            }
        }
    }
}
