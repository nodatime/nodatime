// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using NodaTime.Calendars;

namespace NodaTime.Benchmarks.NodaTimeTests.Calendars
{
    [Config(typeof(BenchmarkConfig))]
    public class WeekYearRuleBenchmarks
    {
        private static readonly LocalDate Sample = new LocalDate(2009, 12, 26);

        [Benchmark]
        public int IsoWeekOfWeekYear() => WeekYearRule.Iso.GetWeekOfWeekYear(Sample);

        [Benchmark]
        public int IsoWeekYear() => WeekYearRule.Iso.GetWeekYear(Sample);
    }
}
