// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using BenchmarkDotNet.Attributes;
using System.Numerics;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    [Config(typeof(BenchmarkConfig))]
    public class DurationBenchmarks
    {
        private static readonly TimeSpan SampleTimeSpan = new TimeSpan(1, 2, 3);

        [Benchmark]
        public Duration FromDays()
        {
#if !V1
            return Duration.FromDays(100);
#else
            return Duration.FromStandardDays(100);
#endif
        }

        [Benchmark]
        public Duration FromHours()
        {
            return Duration.FromHours(100);
        }

        [Benchmark]
        public Duration FromMinutes()
        {
            return Duration.FromMinutes(100);
        }

        [Benchmark]
        public Duration FromSeconds()
        {
            return Duration.FromSeconds(100);
        }

        [Benchmark]
        public Duration FromMilliseconds()
        {
            return Duration.FromMilliseconds(100);
        }

        [Benchmark]
        public Duration FromTicks()
        {
            return Duration.FromTicks(100);
        }

#if !V1
        [Benchmark]
        public Duration FromInt64Nanoseconds()
        {
            return Duration.FromNanoseconds(int.MaxValue + 1L);
        }

        [Benchmark]
        public Duration FromBigIntegerNanoseconds()
        {
            return Duration.FromNanoseconds(long.MaxValue + (BigInteger) 100);
        }
#endif

        [Benchmark]
        public Duration FromTimeSpan()
        {
            return Duration.FromTimeSpan(SampleTimeSpan);
        }

    }
}
