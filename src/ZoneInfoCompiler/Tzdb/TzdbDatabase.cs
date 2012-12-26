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
using NodaTime.TimeZones;

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    ///   Provides an container for the definitions parsed from the TZDB zone info files.
    /// </summary>
    internal class TzdbDatabase
    {
        private readonly string version;
        private readonly SortedList<string, ZoneList> zoneLists;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TzdbDatabase" /> class.
        /// </summary>
        internal TzdbDatabase(string version)
        {
            zoneLists = new SortedList<string, ZoneList>();
            Rules = new Dictionary<string, IList<ZoneRule>>();
            Aliases = new Dictionary<string, string>();
            currentZoneList = null;
            this.version = version;
        }

        internal string Version { get { return version; } }

        /// <summary>
        ///   Gets or sets the time zone alias links.
        /// </summary>
        /// <value>The alias links.</value>
        internal IDictionary<string, string> Aliases { get; private set; }

        /// <summary>
        ///   Gets or sets the current zone list. This is used to gather all of the time zone
        ///   definitions that appear coincident.
        /// </summary>
        /// <value>The current zone list.</value>
        private ZoneList currentZoneList;

        /// <summary>
        ///   Gets or sets the daylight savings rule sets.
        /// </summary>
        /// <value>The rule sets.</value>
        internal IDictionary<string, IList<ZoneRule>> Rules { get; private set; }

        /// <summary>
        ///   Gets or sets the time zone definitions.
        /// </summary>
        /// <value>The time zone definitions.</value>
        internal IList<ZoneList> ZoneLists
        {
            get { return zoneLists.Values; }
        }

        /// <summary>
        ///   Adds the given zone alias to the database.
        /// </summary>
        /// <param name="alias">The zone alias to add.</param>
        internal void AddAlias(ZoneAlias alias)
        {
            Aliases.Add(alias.Alias, alias.Existing);
        }

        /// <summary>
        ///   Adds the given rule to the appropriate RuleSet. If there is no existing
        ///   RuleSet, one is created and added to the database.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        internal void AddRule(ZoneRule rule)
        {
            IList<ZoneRule> ruleSet;
            if (!Rules.TryGetValue(rule.Name, out ruleSet))
            {
                ruleSet = new List<ZoneRule>();
                Rules[rule.Name] = ruleSet;
            }
            ruleSet.Add(rule);
        }

        /// <summary>
        ///   Adds the given zone to the current zone list. If there is no zone list or the
        ///   zone is for a different named zone a new zone list is created.
        /// </summary>
        /// <param name="zone">The zone to add.</param>
        internal void AddZone(Zone zone)
        {
            var name = zone.Name;
            if (String.IsNullOrEmpty(name))
            {
                if (currentZoneList == null)
                {
                    throw new InvalidOperationException("A continuation zone must be preceeded by an initially named zone");
                }
                name = currentZoneList.Name;
            }
            if (currentZoneList == null || currentZoneList.Name != name)
            {
                currentZoneList = new ZoneList();
                zoneLists.Add(name, currentZoneList);
            }
            currentZoneList.Add(zone);
        }

        /// <summary>
        /// Converts each zone in the database into a DateTimeZone.
        /// </summary>
        internal IEnumerable<DateTimeZone> GenerateDateTimeZones()
        {
            foreach (var zoneList in ZoneLists)
            {
                yield return CreateTimeZone(zoneList);
            }
        }

        /// <summary>
        ///   Returns a newly created <see cref="DateTimeZone" /> built from the given time zone data.
        /// </summary>
        /// <param name="zoneList">The time zone definition parts to add.</param>
        private DateTimeZone CreateTimeZone(ZoneList zoneList)
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
                    if (Rules.TryGetValue(zone.Rules, out ruleSet))
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
                                String.Format("Daylight savings rule name '{0}' for zone {1} is neither a known ruleset nor a fixed offset",
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

        /// <summary>
        /// Adds a recurring savings rule to the time zone builder.
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
    }
}
