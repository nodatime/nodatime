// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using NodaTime.TimeZones;
using NodaTime.Utility;
using System.Linq;

namespace NodaTime.TzdbCompiler.Tzdb
{
    /// <summary>
    /// Provides a container for the definitions parsed from the TZDB zone info files.
    /// </summary>
    internal class TzdbDatabase
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
        internal IDictionary<string, string> Aliases { get; }

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
        internal TzdbDatabase(string version)
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
        /// Converts a single zone into a DateTimeZone. Mostly used for debugging.
        /// </summary>
        /// <param name="zoneId">The ID of the zone to convert.</param>
        internal DateTimeZone GenerateDateTimeZone(string zoneId)
        {
            return CreateTimeZone(zoneLists[zoneId]);
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
        /// Returns a newly created <see cref="DateTimeZone" /> built from the given time zone data.
        /// </summary>
        /// <param name="zoneList">The time zone definition parts to add.</param>
        private DateTimeZone CreateTimeZone(ZoneList zoneList)
        {
            Preconditions.CheckArgument(zoneList.Count > 0, nameof(zoneList), "Cannot create a time zone without any Zone entries");
            
            var boundedIntervalSets = new List<ZoneRuleSet.BoundedIntervalSet>();

            var instant = Instant.BeforeMinValue;
            var zones = zoneList.ToList();
            for (int i = 0; i < zones.Count; i++)
            {
                var zone = zones[i];
                var ruleSet = zone.ResolveRules(Rules);
                var boundedIntervalSet = ruleSet.ToBoundedIntervalSet(instant, i == zones.Count - 1);
                boundedIntervalSets.Add(boundedIntervalSet);
                instant = boundedIntervalSet.PartialMap.End;
            }

            var zoneId = zoneList.Name;
            DaylightSavingsDateTimeZone tailZone = boundedIntervalSets.Last().TailZone;

            var tailZoneStart = tailZone == null ? Instant.AfterMaxValue : boundedIntervalSets.Last().PartialMap.End;
            var partialMaps = boundedIntervalSets.Select(x => x.PartialMap).ToList();
            if (tailZone != null)
            {
                partialMaps.Add(new PartialZoneIntervalMap(tailZoneStart, Instant.AfterMaxValue, tailZone));
            }
            FixUpBoundaries(partialMaps);
            var fullMap = PartialZoneIntervalMap.ConvertToFullMap(partialMaps);
            var intervals = GetAndFixUpZoneIntervals(fullMap, tailZoneStart);

            if (intervals.Length == 1 && tailZone == null)
            {
                return new FixedDateTimeZone(zoneId, intervals[0].WallOffset, intervals[0].Name);
            }
            else
            {
                return new PrecalculatedDateTimeZone(zoneId, intervals, tailZone);
            }
        }

        private void FixUpBoundaries(List<PartialZoneIntervalMap> partialMaps)
        {
            // First remove any empty maps, to make life simpler.
            partialMaps.RemoveAll(map => map.Start == map.End);

            for (int i = 1; i < partialMaps.Count; i++)
            {
                var previousMap = partialMaps[i - 1];
                var currentMap = partialMaps[i];
                if (previousMap.Start == previousMap.End || currentMap.Start == currentMap.End)
                {
                    continue;
                }
                var lastIntervalOfPrevious = previousMap.GetZoneInterval(previousMap.End - Duration.Epsilon);
                var firstIntervalOfCurrent = currentMap.GetZoneInterval(currentMap.Start);
                // Effectively move the boundary at the start of the current map. We add
                // a new partial zone interval map with a single zone interval which is the
                // extended last interval of the previous map. These will be coalesced in a minute.
                if (firstIntervalOfCurrent.HasEnd && firstIntervalOfCurrent.IsoLocalEnd == lastIntervalOfPrevious.IsoLocalEnd)
                {
                    var end = firstIntervalOfCurrent.End;
                    var start = firstIntervalOfCurrent.Start;
                    partialMaps[i] = partialMaps[i].WithStart(end);
                    partialMaps.Insert(i, PartialZoneIntervalMap.ForZoneInterval(lastIntervalOfPrevious.WithEnd(end).WithStart(start)));
                }
            }
        }

        // TODO: Move the iteration part of this to an extension method on IZoneIntervalMap
        // and remove it as a "normal" method from DateTimeZone.

        /// <summary>
        /// Find all the zone intervals in the given map, up to the specified end point.
        /// Additionally, if two consecutive zone intervals end at the same local time,
        /// the earlier zone interval is extended to the end of the later zone interval,
        /// which is removed. This happens when the standard offset changes between two
        /// rule sets - the final zone interval in the earlier rule set finishes an hour
        /// earlier than it would in the next rule set.
        /// </summary>
        private static ZoneInterval[] GetAndFixUpZoneIntervals(IZoneIntervalMap map, Instant end)
        {
            var list = new List<ZoneInterval>();
            var current = Instant.MinValue;
            ZoneInterval previousInterval = null;
            while (current < end)
            {
                var zoneInterval = map.GetZoneInterval(current);
                /*
                if (previousInterval != null && zoneInterval.HasEnd && zoneInterval.IsoLocalEnd == previousInterval.IsoLocalEnd)
                {
                    zoneInterval = previousInterval.WithEnd(zoneInterval.End);
                    list.RemoveAt(list.Count - 1);
                }*/
                list.Add(zoneInterval);
                previousInterval = zoneInterval;
                // If this is the end of time, this will just fail on the next comparison.
                current = zoneInterval.RawEnd;
            }
            return list.ToArray();
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
