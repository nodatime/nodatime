// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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
        private readonly SortedList<string, ZoneList> zoneLists;
        /// <summary>
        /// Returns the zone lists. This is only available for the sake of testing.
        /// </summary>
        internal IList<ZoneList> ZoneLists { get { return zoneLists.Values; } } 

        private readonly string version;
        /// <summary>
        /// Returns the version of the TZDB data represented.
        /// </summary>
        internal string Version { get { return version; } }

        private readonly IDictionary<string, string> aliases;
        /// <summary>
        /// Returns the (mutable) map of links from alias to canonical ID.
        /// </summary>
        internal IDictionary<string, string> Aliases { get { return aliases; } }

        private readonly IDictionary<string, IList<ZoneRule>> rules;
        /// <summary>
        /// Mapping from rule name to the zone rules for that name. This is only available for the sake of testing.
        /// </summary>
        internal IDictionary<string, IList<ZoneRule>> Rules { get { return rules; } }

        /// <summary>
        /// A list of the geolocations known to this database.
        /// </summary>
        internal IList<TzdbGeoLocation> GeoLocations { get; set; }

        /// <summary>
        /// The zone list which is currently being defined. This is used to gather all of the time zone
        /// definitions that appear coincident.
        /// </summary>
        private ZoneList currentZoneList;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TzdbDatabase" /> class.
        /// </summary>
        internal TzdbDatabase(string version)
        {
            zoneLists = new SortedList<string, ZoneList>();
            rules = new Dictionary<string, IList<ZoneRule>>();
            aliases = new Dictionary<string, string>();
            currentZoneList = null;
            this.version = version;
        }

        /// <summary>
        ///   Adds the given zone alias to the database.
        /// </summary>
        /// <param name="alias">The zone alias to add.</param>
        internal void AddAlias(ZoneAlias alias)
        {
            aliases.Add(alias.Alias, alias.Existing);
        }

        /// <summary>
        ///   Adds the given rule to the appropriate RuleSet. If there is no existing
        ///   RuleSet, one is created and added to the database.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        internal void AddRule(ZoneRule rule)
        {
            IList<ZoneRule> ruleSet;
            if (!rules.TryGetValue(rule.Name, out ruleSet))
            {
                ruleSet = new List<ZoneRule>();
                rules[rule.Name] = ruleSet;
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
            foreach (var zoneList in zoneLists.Values)
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
                    if (rules.TryGetValue(zone.Rules, out ruleSet))
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

        /// <summary>
        /// Writes various informational counts to the log.
        /// </summary>
        internal void LogCounts()
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("Rule sets:    {0:D}", rules.Count);
            Console.WriteLine("Zones:        {0:D}", zoneLists.Count);
            Console.WriteLine("Aliases:      {0:D}", aliases.Count);
            Console.WriteLine("Geolocations: {0:D}", GeoLocations == null ? 0 : GeoLocations.Count);
            Console.WriteLine("=======================================");
        }
    }
}
