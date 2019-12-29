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
    public class NameIdMappingSupportTest
    {
        // If we're on Linux or some other TZDB-based system, these tests are irrelevant.
        private static readonly Dictionary<string, string> DetectedMapping =
            TimeZoneInfo.GetSystemTimeZones().Any(z => z.Id == "Europe/London")
            ? new Dictionary<string, string>()
            : TimeZoneInfo.GetSystemTimeZones()
                    .Where(zone => zone.Id != zone.StandardName)
                    .ToDictionary(zone => zone.StandardName, zone => zone.Id);

        private static readonly string[] ExpectedMissingKeys =
        {
            "Russia TZ 3 Standard Time",
            "Russia TZ 5 Standard Time",
            "Russia TZ 10 Standard Time",
            "Russia TZ 11 Standard Time",
        };

        [Test]
        public void AllDetectedNamesAreMapped()
        {
            var missingPairs = DetectedMapping.Keys.Except(NameIdMappingSupport.StandardNameToIdMap.Keys)
                .Except(ExpectedMissingKeys)
                .Select(name => new { Name = name, Id = DetectedMapping[name] })
                .ToList();
            CollectionAssert.IsEmpty(missingPairs);
        }

        [Test]
        public void AllDetectedNamesAreMappedCorrectly()
        {
            var incorrectMappings = DetectedMapping.Keys
                .Where(key => NameIdMappingSupport.StandardNameToIdMap.ContainsKey(key))
                .Where(key => DetectedMapping[key] != NameIdMappingSupport.StandardNameToIdMap[key])
                .Select(key => $"Expected {key} => {DetectedMapping[key]}; was {NameIdMappingSupport.StandardNameToIdMap[key]}")
                .ToList();
            CollectionAssert.IsEmpty(incorrectMappings);
        }
    }
}
