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
            Console.WriteLine($"Zone ID: {zone.Id}");
            Console.WriteLine($"Display name: {zone.DisplayName}");
            Console.WriteLine($"Standard name: {zone.StandardName}");
            Console.WriteLine($"Daylight name: {zone.DaylightName}");
            Console.WriteLine($"Base offset: {zone.BaseUtcOffset}");
            Console.WriteLine($"Supports DST: {zone.SupportsDaylightSavingTime}");
            var rules = zone.GetAdjustmentRules();
            if (rules != null && rules.Length > 0)
            {
                Console.WriteLine("Rules:");
                foreach (var rule in rules)
                {
                    DumpRule(zone, rule);
                }
            }
        }

        // Most code taken from https://github.com/jskeet/DemoCode/blob/master/DateTimeDemos/TimeZoneInfoExplorer/MainForm.cs
        private static void DumpRule(TimeZoneInfo zone, TimeZoneInfo.AdjustmentRule rule)
        {
            var start = rule.DateStart.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var end = rule.DateEnd.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var delta = FormatOffset(rule.DaylightDelta);
            var startTransition = FormatTransition(rule.DaylightTransitionStart);
            var endTransition = FormatTransition(rule.DaylightTransitionEnd);
            Console.WriteLine($"{start} - {end}: Daylight delta: {delta}; DST starts {startTransition} and ends {endTransition}");
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
        private static string FormatTransition(TimeZoneInfo.TransitionTime transition)
        {
            return transition.IsFixedDateRule ?
                string.Format("{0:MMMM dd} at {1:HH:mm:ss}", new DateTime(2000, transition.Month, transition.Day), transition.TimeOfDay) :
                string.Format("{0} {1} of {2:MMMM}; {3:HH:mm:ss}", OrdinalWeeks[transition.Week], transition.DayOfWeek, new DateTime(2000, transition.Month, 1), transition.TimeOfDay);
        }
    }
}
