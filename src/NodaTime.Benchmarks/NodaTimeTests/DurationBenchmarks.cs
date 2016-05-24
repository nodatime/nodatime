// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using BenchmarkDotNet.Attributes;
using System.Numerics;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    public class DurationBenchmarks
    {
        private static readonly TimeSpan SampleTimeSpan = new TimeSpan(1, 2, 3);

        [Benchmark]
        public Duration FromDays() =>
#if !V1
            Duration.FromDays(100);
#else
            Duration.FromStandardDays(100);
#endif

        [Benchmark]
        public Duration FromHours() => Duration.FromHours(100);

        [Benchmark]
        public Duration FromMinutes() => Duration.FromMinutes(100);

        [Benchmark]
        public Duration FromSeconds() => Duration.FromSeconds(100);

        [Benchmark]
        public Duration FromMilliseconds() => Duration.FromMilliseconds(100);

        [Benchmark]
        public Duration FromTicks() => Duration.FromTicks(100);

#if !V1
        [Benchmark]
        public Duration FromInt64Nanoseconds() => Duration.FromNanoseconds(int.MaxValue + 1L);

        [Benchmark]
        public Duration FromBigIntegerNanoseconds() => Duration.FromNanoseconds(long.MaxValue + (BigInteger)100);
#endif

        [Benchmark]
        public Duration FromTimeSpan() => Duration.FromTimeSpan(SampleTimeSpan);

    }
}
