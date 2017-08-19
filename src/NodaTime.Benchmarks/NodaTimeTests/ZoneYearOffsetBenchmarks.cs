// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using NodaTime.TimeZones;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    public class ZoneYearOffsetBenchmarks
    {
        private static readonly ZoneYearOffset Sample =
            new ZoneYearOffset(TransitionMode.Wall, 3, 15, (int)IsoDayOfWeek.Sunday, true, new LocalTime(1, 0, 0));

        [Benchmark]
        public LocalInstantWrapper GetOccurrenceForYear() => new LocalInstantWrapper(Sample.GetOccurrenceForYear(2010));

        public struct LocalInstantWrapper
        {
            private readonly LocalInstant value;
            internal LocalInstantWrapper(LocalInstant value) => this.value = value;
        }
    }
}
