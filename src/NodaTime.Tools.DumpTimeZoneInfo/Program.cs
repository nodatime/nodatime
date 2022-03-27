// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using System.Linq;

namespace NodaTime.Tools.DumpTimeZoneInfo
{
    class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Single command line argument: (--list|zone-id)");
                return;
            }
            if (args[0] == "--list")
            {
                ListZoneIds();
            }
            else
            {
                DumpZone(args[0]);
            }
        }

        private static void ListZoneIds()
        {
            foreach (var zone in TimeZoneInfo.GetSystemTimeZones().OrderBy(x => x.Id))
            {
                Console.WriteLine($"{zone.Id} ({zone.DisplayName})");
            }
        }

        private static void DumpZone(string id)
        {
            var zone = TimeZoneInfo.FindSystemTimeZoneById(id);
            Console.WriteLine(zone.IsDaylightSavingTime(new DateTime(2009, 11, 1)));
            Console.WriteLine(zone.GetUtcOffset(new DateTime(2009, 10, 4, 0, 0, 0, DateTimeKind.Utc)));
            Console.WriteLine($"Zone ID: {zone.Id}");
            Console.WriteLine($"Display name: {zone.DisplayName}");
            Console.WriteLine($"Standard name: {zone.StandardName}");
            Console.WriteLine($"Daylight name: {zone.DaylightName}");
            Console.WriteLine($"Base offset: {zone.BaseUtcOffset}");
            Console.WriteLine($"Supports DST: {zone.SupportsDaylightSavingTime}");
            var rules = zone.GetAdjustmentRules();
            bool windowsRules = AreWindowsStyleRules();
            if (rules != null && rules.Length > 0)
            {
                Console.WriteLine("Rules:");
                foreach (var rule in rules)
                {
                    DumpRule(zone, rule, windowsRules);
                }
            }

            bool AreWindowsStyleRules()
            {
                int windowsRules = rules.Count(IsWindowsRule);
                return windowsRules == rules.Length;

                bool IsWindowsRule(TimeZoneInfo.AdjustmentRule rule) =>
                    rule.DateStart.Month == 1 && rule.DateStart.Day == 1 && rule.DateStart.TimeOfDay.Ticks == 0 &&
                    rule.DateEnd.Month == 12 && rule.DateEnd.Day == 31 && rule.DateEnd.TimeOfDay.Ticks == 0;
            }
        }

        // Most code taken from https://github.com/jskeet/DemoCode/blob/master/DateTimeDemos/TimeZoneInfoExplorer/MainForm.cs
        private static void DumpRule(TimeZoneInfo zone, TimeZoneInfo.AdjustmentRule rule, bool windowsRules)
        {
            var start = rule.DateStart.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var end = rule.DateEnd.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var delta = FormatOffset(rule.DaylightDelta);
            var startTransition = FormatTransition(rule.DaylightTransitionStart);
            var endTransition = FormatTransition(rule.DaylightTransitionEnd);

            // Work out the start of the transition. This depends on the type of rule.
            DateTime ruleStartLocal = windowsRules ? rule.DateStart : rule.DateStart + rule.DaylightTransitionStart.TimeOfDay.TimeOfDay;
            DateTime ruleStartUtc = DateTime.SpecifyKind(ruleStartLocal.Year == 1 ? DateTime.MinValue : ruleStartLocal - zone.BaseUtcOffset, DateTimeKind.Utc);
#if NET6_0_OR_GREATER
            var baseUtcOffsetDelta = rule.BaseUtcOffsetDelta;
#else
            // Effectively fake the BaseUtcOffsetDelta detection for older versions of .NET.
            var ruleStandardOffset = zone.GetUtcOffset(ruleStartUtc) - (zone.IsDaylightSavingTime(ruleStartUtc) ? rule.DaylightDelta : TimeSpan.Zero);
            var baseUtcOffsetDelta = ruleStandardOffset - zone.BaseUtcOffset;
#endif
            var baseUtcOffsetDeltaText = baseUtcOffsetDelta == TimeSpan.Zero ? ""
                : $"Base UTC offset delta: {FormatOffset(baseUtcOffsetDelta)}; ";

            // Note: while this is "usually okay" there are some rules that don't actually start when we'd predict, which is unfortunate.
            bool isDst = zone.IsDaylightSavingTime(ruleStartUtc);
            var forceDaylightText = rule.DaylightDelta == TimeSpan.Zero && isDst ? " (force daylight)" : "";            
            Console.WriteLine($"{start} - {end}: {baseUtcOffsetDeltaText}Daylight delta: {delta}; DST starts {startTransition} and ends {endTransition}{forceDaylightText}");
        }

        private static string FormatOffset(TimeSpan offset)
        {
            string prefix = "+";
            if (offset < TimeSpan.Zero)
            {
                prefix = "-";
                offset = -offset;
            }
            string value = offset.Seconds > 0 ? offset.ToString("hh':'mm':'ss", CultureInfo.InvariantCulture)
                : offset.Minutes > 0 ? offset.ToString("hh':'mm", CultureInfo.InvariantCulture)
                : offset.ToString("hh", CultureInfo.InvariantCulture);
            return prefix + value;
        }

        private static readonly string[] OrdinalWeeks = { "", "1st", "2nd", "3rd", "4th", "Last" };
        private static string FormatTransition(TimeZoneInfo.TransitionTime transition) =>
            transition.IsFixedDateRule ?
                string.Format("{0:MMMM dd} at {1:HH:mm:ss.FFFFFFF}", new DateTime(2000, transition.Month, transition.Day), transition.TimeOfDay) :
                string.Format("{0} {1} of {2:MMMM}; {3:HH:mm:ss.FFFFFFF}", OrdinalWeeks[transition.Week], transition.DayOfWeek, new DateTime(2000, transition.Month, 1), transition.TimeOfDay);
    }
}
