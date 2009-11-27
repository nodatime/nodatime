#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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

namespace NodaTime.ZoneInfoCompiler.Tzdb
{
    /// <summary>
    /// Provides an container for the definitions parsed from the TZDB zone info files.
    /// </summary>
    internal class TzdbDatabase
    {
        /// <summary>
        /// Gets or sets the daylight savings rule sets.
        /// </summary>
        /// <value>The rule sets.</value>
        internal IDictionary<string, RuleSet> Rules { get; private set; }

        /// <summary>
        /// Gets or sets the time zone definitions.
        /// </summary>
        /// <value>The time zone definitions.</value>
        internal IList<ZoneList> Zones { get; private set; }

        /// <summary>
        /// Gets or sets the time zone alias links.
        /// </summary>
        /// <value>The alias links.</value>
        internal IList<ZoneAlias> Aliases { get; private set; }

        /// <summary>
        /// Gets or sets the current zone list. This is used to gather all of the time zone
        /// definitions that appear coincident.
        /// </summary>
        /// <value>The current zone list.</value>
        private ZoneList CurrentZoneList { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbDatabase"/> class.
        /// </summary>
        internal TzdbDatabase()
        {
            Rules = new Dictionary<string, RuleSet>();
            Zones = new List<ZoneList>();
            Aliases = new List<ZoneAlias>();
            CurrentZoneList = null;
        }

        /// <summary>
        /// Adds the given rule to the appropriate RuleSet. If there is no existing
        /// RuleSet, one is created and added to the database.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        internal void AddRule(Rule rule)
        {
            RuleSet ruleSet;
            if (!Rules.TryGetValue(rule.Name, out ruleSet)) {
                ruleSet = new RuleSet();
                Rules[rule.Name] = ruleSet;
            }
            ruleSet.Add(rule);
        }

        /// <summary>
        /// Adds the given zone alias to the database.
        /// </summary>
        /// <param name="link">The zone alias to add.</param>
        internal void AddAlias(ZoneAlias alias)
        {
            Aliases.Add(alias);
        }

        /// <summary>
        /// Adds the given zone to the current zone list. If there is no zone list or the
        /// zone is for a different named zone a new zone list is created.
        /// </summary>
        /// <param name="zone">The zone to add.</param>
        internal void AddZone(Zone zone)
        {
            if (zone.Name == null) {
                if (CurrentZoneList == null) {
                    throw new ArgumentException("A continuation zone must be preceeded by an initially named zone");
                }
                zone.Name = CurrentZoneList.Name;
            }
            if (CurrentZoneList == null || CurrentZoneList.Name != zone.Name) {
                CurrentZoneList = new ZoneList();
                Zones.Add(CurrentZoneList);
            }
            CurrentZoneList.Add(zone);
        }
    }
}
