// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.


using BenchmarkDotNet.Attributes;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    public class DateIntervalBenchmarks
    {
        private static readonly DateInterval JanuaryJune2017 = new DateInterval(new LocalDate(2017, 1, 1), new LocalDate(2017, 6, 30));
        private static readonly DateInterval MarchSeptember2017 = new DateInterval(new LocalDate(2017, 3, 1), new LocalDate(2017, 9, 30));
        private static readonly DateInterval AugustDecember2017 = new DateInterval(new LocalDate(2017, 8, 1), new LocalDate(2017, 12, 31));

        [Benchmark]
        public int Length() => JanuaryJune2017.Length;

        [Benchmark]
        public DateInterval Union_Disjoint() => JanuaryJune2017.Union(AugustDecember2017);

        [Benchmark]
        public DateInterval Union_Overlapping() => JanuaryJune2017.Union(MarchSeptember2017);

        [Benchmark]
        public DateInterval Intersection_Disjoint() => JanuaryJune2017.Intersection(AugustDecember2017);

        [Benchmark]
        public DateInterval Intersection_Overlapping() => JanuaryJune2017.Intersection(MarchSeptember2017);
    }
}
