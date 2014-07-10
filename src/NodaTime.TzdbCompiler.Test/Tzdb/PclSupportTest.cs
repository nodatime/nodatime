// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TzdbCompiler.Tzdb;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.TzdbCompiler.Test.Tzdb
{
    [TestFixture]
    public class PclSupportTest
    {
        private static readonly Dictionary<string, string> DetectedMapping =
            TimeZoneInfo.GetSystemTimeZones()
                    .Where(zone => zone.Id != zone.StandardName)
                    .ToDictionary(zone => zone.StandardName, zone => zone.Id);

        [Test]
        public void AllDetectedNamesAreMapped()
        {
            var missingNames = DetectedMapping.Keys.Except(PclSupport.StandardNameToIdMap.Keys).ToList();
            CollectionAssert.IsEmpty(missingNames);
        }

        [Test]
        public void AllDetectedNamesAreMappedCorrectly()
        {
            var incorrectMappings = DetectedMapping.Keys
                .Where(key => PclSupport.StandardNameToIdMap.ContainsKey(key))
                .Where(key => DetectedMapping[key] != PclSupport.StandardNameToIdMap[key])
                .Select(key => "Expected " + key + " => " + DetectedMapping[key] + "; was " + PclSupport.StandardNameToIdMap[key])
                .ToList();
            CollectionAssert.IsEmpty(incorrectMappings);
        }
    }
}
