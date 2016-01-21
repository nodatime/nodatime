// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet;

namespace NodaTime.Benchmarks
{
    /// <summary>
    /// Entry point for benchmarking.
    /// </summary>
    internal class Program
    {
        // Run it with args = { "*" } for choosing all of target benchmarks
        private static void Main(string[] args)
        {
            var competitionSwitch = new BenchmarkSwitcher(new[]
            {
                typeof(BclTests.DateTimeBenchmarks),
                typeof(BclTests.DateTimeOffsetBenchmarks),
                typeof(BclTests.TimeZoneInfoBenchmarks),
                typeof(BclTests.UtcDateTimeBenchmarks),
                typeof(NodaTimeTests.Calendars.HebrewCalendarBenchmarks),
                typeof(NodaTimeTests.Calendars.IsoCalendarBenchmarks),
                typeof(NodaTimeTests.JsonNet.FormattingBenchmarks),
                typeof(NodaTimeTests.JsonNet.ParsingBenchmarks),
                typeof(NodaTimeTests.Text.InstantPatternBenchmarks),
                typeof(NodaTimeTests.Text.LocalDatePatternBenchmarks),
                typeof(NodaTimeTests.Text.LocalDateTimePatternBenchmarks),
                typeof(NodaTimeTests.Text.OffsetDateTimePatternBenchmarks),
                typeof(NodaTimeTests.Text.PeriodPatternBenchmarks),
                typeof(NodaTimeTests.Text.ZonedDateTimePatternBenchmarks),
                typeof(NodaTimeTests.BclDateTimeZoneBenchmarks),
                typeof(NodaTimeTests.CachedDateTimeZoneBenchmarks),
                typeof(NodaTimeTests.DurationBenchmarks),
                typeof(NodaTimeTests.InstantBenchmarks),
                typeof(NodaTimeTests.LocalDateBenchmarks),
                typeof(NodaTimeTests.LocalDateTimeBenchmarks),
                typeof(NodaTimeTests.LocalTimeBenchmarks),
                typeof(NodaTimeTests.OffsetBenchmarks),
                typeof(NodaTimeTests.OffsetDateTimeBenchmarks),
                typeof(NodaTimeTests.PacificZonedDateTimeBenchmarks),
                typeof(NodaTimeTests.PeriodBenchmarks),
                typeof(NodaTimeTests.StandardDaylightAlternatingMapBenchmarks),
                typeof(NodaTimeTests.UtcZonedDateTimeBenchmarks)
            });
            competitionSwitch.Run(args);
        }
    }
}
