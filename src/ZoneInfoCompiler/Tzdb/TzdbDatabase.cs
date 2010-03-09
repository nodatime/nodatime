#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
    /// Provides an container for the definitions parsed from the TZDB zone info files.
    /// </summary>
    internal class TzdbDatabase
    {
        private SortedList<string, ZoneList> zoneLists;

        /// <summary>
        /// Gets or sets the daylight savings rule sets.
        /// </summary>
        /// <value>The rule sets.</value>
        internal IDictionary<string, IList<ZoneRule>> Rules { get; private set; }

        /// <summary>
        /// Gets or sets the time zone definitions.
        /// </summary>
        /// <value>The time zone definitions.</value>
        internal IList<ZoneList> Zones { get { return this.zoneLists.Values; } }

        /// <summary>
        /// Gets or sets the time zone alias links.
        /// </summary>
        /// <value>The alias links.</value>
        internal IDictionary<string, string> Aliases { get; private set; }

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
            this.zoneLists = new SortedList<string, ZoneList>();
            Rules = new Dictionary<string, IList<ZoneRule>>();
            Aliases = new Dictionary<string, string>();
            CurrentZoneList = null;
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
        /// Adds the given zone alias to the database.
        /// </summary>
        /// <param name="link">The zone alias to add.</param>
        internal void AddAlias(ZoneAlias alias)
        {
            Aliases.Add(alias.Alias, alias.Existing);
        }

        /// <summary>
        /// Adds the given zone to the current zone list. If there is no zone list or the
        /// zone is for a different named zone a new zone list is created.
        /// </summary>
        /// <param name="zone">The zone to add.</param>
        internal void AddZone(Zone zone)
        {
            string name = zone.Name;
            if (string.IsNullOrEmpty(name))
            {
                if (CurrentZoneList == null)
                {
                    throw new ArgumentException("A continuation zone must be preceeded by an initially named zone");
                }
                name = CurrentZoneList.Name;
            }
            if (CurrentZoneList == null || CurrentZoneList.Name != name)
            {
                CurrentZoneList = new ZoneList();
                this.zoneLists.Add(name, CurrentZoneList);
            }
            CurrentZoneList.Add(zone);
        }
    }
}
