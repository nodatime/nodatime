// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using NodaTime.TimeZones;
using System.Linq;
using NodaTime.TimeZones.Cldr;
using System.IO;

namespace NodaTime.TzdbCompiler.Tzdb
{
    /// <summary>
    /// Provides a container for the definitions parsed from the TZDB zone info files.
    /// </summary>
    public class TzdbDatabase
    {
        /// <summary>
        /// Returns the zone lists. This is only available for the sake of testing.
        /// </summary>
        internal IDictionary<string, IList<ZoneLine>> Zones { get; }

        /// <summary>
        /// Returns the version of the TZDB data represented.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Returns the (mutable) map of links from alias to canonical ID.
        /// </summary>
        public IDictionary<string, string> Aliases { get; }

        /// <summary>
        /// Mapping from rule name to the zone rules for that name. This is only available for the sake of testing.
        /// </summary>
        internal IDictionary<string, IList<RuleLine>> Rules { get; }

        /// <summary>
        /// A list of the zone locations known to this database from zone.tab.
        /// </summary>
        internal IList<TzdbZoneLocation> ZoneLocations { get; set; }

        /// <summary>
        /// A list of the zone locations known to this database from zone1970.tab.
        /// </summary>
        internal IList<TzdbZone1970Location> Zone1970Locations { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbDatabase" /> class.
        /// </summary>
        public TzdbDatabase(string version)
        {
            Zones = new Dictionary<string, IList<ZoneLine>>();
            Rules = new Dictionary<string, IList<RuleLine>>();
            Aliases = new Dictionary<string, string>();
            Version = version;
        }

        /// <summary>
        /// Returns the data in this database as a <see cref="TzdbDateTimeZoneSource"/> with no
        /// Windows mappings.
        /// </summary>
        public TzdbDateTimeZoneSource ToTzdbDateTimeZoneSource()
        {
            var ms = new MemoryStream();
            var writer = new TzdbStreamWriter();
            writer.Write(this,
                new WindowsZones("n/a", Version, "n/a", new MapZone[0]), // No Windows mappings,
                new Dictionary<string, string>(), // No additional name-to-id mappings
                ms);
            ms.Position = 0;
            return TzdbDateTimeZoneSource.FromStream(ms);
        }

        /// <summary>
        /// Adds the given zone alias to the database.
        /// </summary>
        /// <param name="original">The existing zone ID to map the alias to.</param>
        /// <param name="alias">The zone alias to add.</param>
        internal void AddAlias(string existing, string alias)
        {
            Aliases.Add(alias, existing);
        }

        /// <summary>
        /// Adds the given rule to the appropriate rule set. If there is no existing
        /// rule set, one is created and added to the database.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        internal void AddRule(RuleLine rule)
        {
            if (!Rules.TryGetValue(rule.Name, out IList<RuleLine> ruleSet))
            {
                ruleSet = new List<RuleLine>();
                Rules[rule.Name] = ruleSet;
            }
            ruleSet.Add(rule);
        }

        /// <summary>
        /// Adds the given zone line to the database, creating a new list for
        /// that zone ID if necessary.
        /// </summary>
        /// <param name="zone">The zone to add.</param>
        internal void AddZone(ZoneLine zone)
        {
            if (!Zones.TryGetValue(zone.Name, out IList<ZoneLine> zoneSet))
            {
                zoneSet = new List<ZoneLine>();
                Zones[zone.Name] = zoneSet;
            }
            zoneSet.Add(zone);
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
            return CreateTimeZone(zoneId, Zones[zoneListKey]);
        }

        /// <summary>
        /// Converts each zone in the database into a DateTimeZone.
        /// </summary>
        public IEnumerable<DateTimeZone> GenerateDateTimeZones()
        {
            foreach (var entry in Zones.OrderBy(pair => pair.Key, StringComparer.Ordinal))
            {
                yield return CreateTimeZone(entry.Key, entry.Value);
            }
        }

        /// <summary>
        /// Returns a newly created <see cref="DateTimeZone" /> built from the given time zone data.
        /// </summary>
        /// <param name="zoneList">The time zone definition parts to add.</param>
        private DateTimeZone CreateTimeZone(string id, IList<ZoneLine> zoneList)
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
            Console.WriteLine($"Rule sets:    {Rules.Count:D}", Rules.Count);
            Console.WriteLine($"Zones:        {Zones.Count:D}", Zones.Count);
            Console.WriteLine($"Aliases:      {Aliases.Count:D}", Aliases.Count);
            Console.WriteLine($"Zone locations: {ZoneLocations?.Count ?? 0:D}");
            Console.WriteLine($"Zone1970 locations: {Zone1970Locations?.Count ?? 0:D}");
            Console.WriteLine("=======================================");
        }
    }
}
