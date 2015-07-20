// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using NodaTime.TimeZones;
using System.Linq;

namespace NodaTime.TzdbCompiler.Tzdb
{
    /// <summary>
    /// Provides a container for the definitions parsed from the TZDB zone info files.
    /// </summary>
    public class TzdbDatabase
    {
        private readonly SortedList<string, ZoneList> zoneLists;
        /// <summary>
        /// Returns the zone lists. This is only available for the sake of testing.
        /// </summary>
        internal IList<ZoneList> ZoneLists => zoneLists.Values;

        /// <summary>
        /// Returns the version of the TZDB data represented.
        /// </summary>
        internal string Version { get; }

        /// <summary>
        /// Returns the (mutable) map of links from alias to canonical ID.
        /// </summary>
        public IDictionary<string, string> Aliases { get; }

        /// <summary>
        /// Mapping from rule name to the zone rules for that name. This is only available for the sake of testing.
        /// </summary>
        internal IDictionary<string, IList<ZoneRule>> Rules { get; }

        /// <summary>
        /// A list of the zone locations known to this database from zone.tab.
        /// </summary>
        internal IList<TzdbZoneLocation> ZoneLocations { get; set; }

        /// <summary>
        /// A list of the zone locations known to this database from zone1970.tab.
        /// </summary>
        internal IList<TzdbZone1970Location> Zone1970Locations { get; set; }

        /// <summary>
        /// The zone list which is currently being defined. This is used to gather all of the time zone
        /// definitions that appear coincident.
        /// </summary>
        private ZoneList currentZoneList;

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbDatabase" /> class.
        /// </summary>
        public TzdbDatabase(string version)
        {
            zoneLists = new SortedList<string, ZoneList>();
            Rules = new Dictionary<string, IList<ZoneRule>>();
            Aliases = new Dictionary<string, string>();
            currentZoneList = null;
            this.Version = version;
        }

        /// <summary>
        /// Adds the given zone alias to the database.
        /// </summary>
        /// <param name="alias">The zone alias to add.</param>
        internal void AddAlias(ZoneAlias alias)
        {
            Aliases.Add(alias.Alias, alias.Existing);
        }

        /// <summary>
        /// Adds the given rule to the appropriate RuleSet. If there is no existing
        /// RuleSet, one is created and added to the database.
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
        /// Adds the given zone to the current zone list. If there is no zone list or the
        /// zone is for a different named zone a new zone list is created.
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
        /// Converts a single zone into a DateTimeZone. As well as for testing purposes,
        /// this can be used to resolve aliases.
        /// </summary>
        /// <param name="zoneId">The ID of the zone to convert.</param>
        public DateTimeZone GenerateDateTimeZone(string zoneId)
        {
            string zoneListKey = zoneId;
            // Recursively resolve aliases
            while (Aliases.ContainsKey(zoneListKey))
            {
                zoneListKey = Aliases[zoneListKey];
            }
            return CreateTimeZone(zoneId, zoneLists[zoneListKey]);
        }

        /// <summary>
        /// Converts each zone in the database into a DateTimeZone.
        /// </summary>
        public IEnumerable<DateTimeZone> GenerateDateTimeZones()
        {
            foreach (var zoneList in zoneLists.Values)
            {
                yield return CreateTimeZone(zoneList.Name, zoneList);
            }
        }

        /// <summary>
        /// Returns a newly created <see cref="DateTimeZone" /> built from the given time zone data.
        /// </summary>
        /// <param name="zoneList">The time zone definition parts to add.</param>
        private DateTimeZone CreateTimeZone(string id, ZoneList zoneList)
        {
            var ruleSets = zoneList.Select(zone => zone.ResolveRules(Rules)).ToList();
            return DateTimeZoneBuilder.Build(id, ruleSets);
        }

        /// <summary>
        /// Writes various informational counts to the log.
        /// </summary>
        internal void LogCounts()
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("Rule sets:    {0:D}", Rules.Count);
            Console.WriteLine("Zones:        {0:D}", zoneLists.Count);
            Console.WriteLine("Aliases:      {0:D}", Aliases.Count);
            Console.WriteLine("Zone locations: {0:D}", ZoneLocations?.Count ?? 0);
            Console.WriteLine("Zone1970 locations: {0:D}", Zone1970Locations?.Count ?? 0);
            Console.WriteLine("=======================================");
        }
    }
}
