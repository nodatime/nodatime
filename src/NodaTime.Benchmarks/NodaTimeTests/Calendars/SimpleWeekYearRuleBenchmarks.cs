// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using NodaTime.Calendars;

namespace NodaTime.Benchmarks.NodaTimeTests.Calendars
{
    public class SimpleWeekYearRuleBenchmarks
    {
        private static readonly LocalDate Sample = new LocalDate(2009, 12, 26);

        [Benchmark]
        public int IsoWeekOfWeekYear() => WeekYearRules.Iso.GetWeekOfWeekYear(Sample);

        [Benchmark]
        public int IsoWeekYear() => WeekYearRules.Iso.GetWeekYear(Sample);
    }
}
