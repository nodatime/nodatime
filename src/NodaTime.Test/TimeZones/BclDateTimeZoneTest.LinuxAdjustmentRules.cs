// Copyright 2022 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NodaTime.TimeZones;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BclAdjustmentRule = NodaTime.TimeZones.BclDateTimeZone.BclAdjustmentRule;

namespace NodaTime.Test.TimeZones
{
    public partial class BclDateTimeZoneTest
    {
        public class LinuxAdjustmentRules
        {
            private static List<LinuxAdjustmentMultiRuleData> TestData => LoadTestData().ToList();

            [TestCaseSource(nameof(TestData))]
            public void SimpleMultiRulesNetCore31(LinuxAdjustmentMultiRuleData data) => AssertRules(data, data.NetCore31Rules);

            [TestCaseSource(nameof(TestData))]
            public void SimpleMultiRulesNet6(LinuxAdjustmentMultiRuleData data) => AssertRules(data, data.Net6Rules);

            private static void AssertRules(LinuxAdjustmentMultiRuleData data, IReadOnlyList<EnhancedAdjustmentRule> sourceRules)
            {
                var expectedIntervals = data.ExpectedIntervals;
                var bclAdjustmentRules = sourceRules
                    .Select(rule => BclAdjustmentRule.ConvertUnixRuleToBclAdjustmentRule(rule.Rule, "Standard", "Daylight", rule.ZoneStandardOffset, rule.RuleStandardOffset, rule.ForceDaylight))
                    .ToArray();
                BclDateTimeZone.FixUnixTransitions(bclAdjustmentRules);

                // Convert our rules to a full map, as there's coalescing code in there that we want to take advantage of.
                var map = BclDateTimeZone.BuildMap(bclAdjustmentRules, Offset.FromTimeSpan(data.ZoneStandardOffset), "Standard"); ;

                // Get all of the zone intervals in the expected interval
                var actualIntervals = new List<ZoneInterval>();
                actualIntervals.Add(map.GetZoneInterval(expectedIntervals[0].RawStart));
                while (actualIntervals.Last().RawEnd < expectedIntervals.Last().RawEnd)
                {
                    actualIntervals.Add(map.GetZoneInterval(actualIntervals.Last().RawEnd));
                }
                Assert.AreEqual(expectedIntervals, actualIntervals);
            }

            private static IEnumerable<LinuxAdjustmentMultiRuleData> LoadTestData()
            {
                using var stream = typeof(BclDateTimeZoneTest).Assembly.GetManifestResourceStream("NodaTime.Test.TestData.LinuxAdjustmentRuleTestData.txt")!;
                using var reader = new StreamReader(stream);

                while (true)
                {
                    var name = reader.ReadLine();
                    if (name is null)
                    {
                        break;
                    }
                    if (name.StartsWith("//") || name == "")
                    {
                        continue;
                    }
                    name = name.TrimEnd(':');
                    var baseOffset = reader.ReadLine()!;
                    Assert.True(baseOffset.StartsWith("Base offset = "));
                    var baseOffsetHours = int.Parse(baseOffset.Split(' ').Last());
                    Assert.AreEqual("---", reader.ReadLine());
                    var netCoreApp31Rules = new List<string>();
                    while (reader.ReadLine() is string line && line != "---")
                    {
                        netCoreApp31Rules.Add(line);
                    }
                    var net6Rules = new List<string>();
                    while (reader.ReadLine() is string line && line != "---")
                    {
                        net6Rules.Add(line);
                    }
                    var nodaTimeIntervals = new List<string>();
                    while (reader.ReadLine() is string line && line != "")
                    {
                        nodaTimeIntervals.Add(line);
                    }
                    yield return new(name, baseOffsetHours, netCoreApp31Rules, net6Rules, nodaTimeIntervals);
                }
            }

            public class LinuxAdjustmentMultiRuleData
            {
                internal string Name { get; }
                internal TimeSpan ZoneStandardOffset;
                internal IReadOnlyList<EnhancedAdjustmentRule> NetCore31Rules { get; }
                internal IReadOnlyList<EnhancedAdjustmentRule> Net6Rules { get; }
                internal IReadOnlyList<ZoneInterval> ExpectedIntervals { get; }

                public LinuxAdjustmentMultiRuleData(string name, int baseUtcOffsetHours, IEnumerable<string> netCore31Rules, IEnumerable<string> net6Rules, IEnumerable<string> expectedIntervals)
                {
                    Name = name;
                    var baseUtcOffset = TimeSpan.FromHours(baseUtcOffsetHours);
                    ZoneStandardOffset = baseUtcOffset;
                    NetCore31Rules = netCore31Rules.Select(text => EnhancedAdjustmentRule.Parse(text, baseUtcOffset)).ToList();
                    Net6Rules = net6Rules.Select(text => EnhancedAdjustmentRule.Parse(text, baseUtcOffset)).ToList();
                    ExpectedIntervals = expectedIntervals.Select(ParseZoneInterval).ToList();
                }

                public override string ToString() => Name;

                private static readonly Regex ZoneIntervalRegex = new Regex(@"^(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z) - (\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z), ([-+][:\d]+), ([-+][:\d]+)$");
                /// <summary>
                /// Parses a zone interval of the form "from - to, standard offset, savings offset" into a ZoneInterval.
                /// Sample line: "2017-10-15T03:00:00Z - 2018-02-18T02:00:00Z, -03, +01"
                /// The name is "Standard" if the daylight offset is zero, and "Daylight" otherwise.
                /// </summary>
                private static ZoneInterval ParseZoneInterval(string text)
                {
                    var match = ZoneIntervalRegex.Match(text);
                    Assert.True(match.Success);
                    var start = InstantPattern.General.Parse(match.Groups[1].Value).Value;
                    if (start == NodaConstants.BclEpoch)
                    {
                        start = Instant.BeforeMinValue;
                    }
                    var end = InstantPattern.General.Parse(match.Groups[2].Value).Value;
                    if (end.InUtc().Year == 9999)
                    {
                        end = Instant.AfterMaxValue;
                    }
                    var standardOffset = OffsetPattern.GeneralInvariant.Parse(match.Groups[3].Value).Value;
                    var savingsOffset = OffsetPattern.GeneralInvariant.Parse(match.Groups[4].Value).Value;
                    return new ZoneInterval(savingsOffset == Offset.Zero ? "Standard" : "Daylight", start, end, standardOffset + savingsOffset, savingsOffset);
                }
            }

            internal class EnhancedAdjustmentRule
            {
                internal TimeZoneInfo.AdjustmentRule Rule;

                /// <summary>
                /// Indicates that this rule really indicates daylight savings,
                /// which are assumed (possibly incorrectly?) to be one hour.
                /// This is set when zone.IsDaylightSavingTime(start) returns true,
                /// but rule.DaylightDelta is zero. Unfortunately we can't determine
                /// the meaning just from the rule.
                /// </summary>
                internal bool ForceDaylight { get; }

                /// <summary>
                /// The UTC offset for the overall zone; this is used to determine when a rule starts and finishes (apparently).
                /// </summary>
                internal TimeSpan ZoneStandardOffset { get; }
                /// <summary>
                /// The UTC offset for the standard intervals in force during this rule.
                /// </summary>
                internal TimeSpan RuleStandardOffset { get; }

                private EnhancedAdjustmentRule(TimeZoneInfo.AdjustmentRule rule, TimeSpan baseUtcOffset, TimeSpan baseUtcOffsetDelta, bool forceDaylight)
                {
                    Rule = rule;
                    ForceDaylight = forceDaylight;
                    ZoneStandardOffset = baseUtcOffset;
                    RuleStandardOffset = baseUtcOffset + baseUtcOffsetDelta;
                }

                private static readonly Regex RuleRegex = new Regex(
                    @"^(?<from>\d{4}-\d{2}-\d{2}) - (?<to>\d{4}-\d{2}-\d{2}): " +
                    @"(Base UTC offset delta: (?<base_offset_delta>[-+][\d:]+); )?" +
                    @"Daylight delta: (?<savings>[-+][\d:]+); " +
                    @"DST starts (?<dst_start>[A-Za-z]+ \d{2} at [\d:\.]+) " +
                    @"and ends (?<dst_end>[A-Za-z]+ \d{2} at [\d:\.]+)(?<force_daylight> \(force daylight\))?$");
                private static readonly LocalDateTimePattern DstPattern = LocalDateTimePattern.CreateWithInvariantCulture("MMMM dd 'at' HH:mm:ss.FFFFFF");

                /// <summary>
                /// Parses a rule of the form "from - to: Daylight delta: offset; DST starts {date} at {time} and ends {date} at {time}"
                /// or "from - to: Base UTC offset delta: offset; Daylight delta: offset; DST starts {date} at {time} and ends {date} at {time}".
                /// Sample lines:
                /// "2019-10-27 - 2019-12-31: Daylight delta: -01; DST starts October 27 at 02:00:00 and ends December 31 at 23:59:59.999"
                /// "1960-01-01 - 1960-04-10: Base UTC offset delta: -01; Daylight delta: +00; DST starts January 01 at 00:00:00 and ends April 10 at 02:59:59.999"
                /// This is the format printed out by NodaTime.Tools.DumpTimeZoneInfo. It only handles fixed date transitions.
                /// </summary>
                internal static EnhancedAdjustmentRule Parse(string text, TimeSpan baseUtcOffset)
                {
                    var match = RuleRegex.Match(text);
                    Assert.True(match.Success);
                    LocalDate fromDate = LocalDatePattern.Iso.Parse(match.Groups["from"].Value).Value;
                    LocalDate toDate = LocalDatePattern.Iso.Parse(match.Groups["to"].Value).Value;
                    var baseUtcOffsetDelta = match.Groups["base_offset_delta"] is { Success: true, Value: string baseUtcOffsetDeltaText }
                        ? OffsetPattern.GeneralInvariant.Parse(baseUtcOffsetDeltaText).Value
                        : Offset.Zero;
                    var savings = OffsetPattern.GeneralInvariant.Parse(match.Groups["savings"].Value).Value;
                    var dstStart = DstPattern.Parse(match.Groups["dst_start"].Value).Value;
                    var dstEnd = DstPattern.Parse(match.Groups["dst_end"].Value).Value;
                    var rule = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(fromDate.ToDateTimeUnspecified(), toDate.ToDateTimeUnspecified(), savings.ToTimeSpan(), CreateTransitionTime(dstStart), CreateTransitionTime(dstEnd));
                    var forceDaylight = match.Groups["force_daylight"].Success;
                    return new EnhancedAdjustmentRule(rule, baseUtcOffset, baseUtcOffsetDelta.ToTimeSpan(), forceDaylight);

                    TimeZoneInfo.TransitionTime CreateTransitionTime(LocalDateTime dateTime) =>
                        TimeZoneInfo.TransitionTime.CreateFixedDateRule(new LocalDate(1, 1, 1).At(dateTime.TimeOfDay).ToDateTimeUnspecified(), dateTime.Month, dateTime.Day);
                }
            }
        }
    }
}
