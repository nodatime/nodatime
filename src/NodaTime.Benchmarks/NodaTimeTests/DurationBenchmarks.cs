using NodaTime.Benchmarks.Framework;
// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    public class DurationBenchmarks
    {
        private static readonly TimeSpan SampleTimeSpan = new TimeSpan(1, 2, 3);

        [Benchmark]
        public void FromDays()
        {
            Duration.FromDays(100).Consume();
        }

        [Benchmark]
        public void FromHours()
        {
            Duration.FromHours(100).Consume();
        }

        [Benchmark]
        public void FromMinutes()
        {
            Duration.FromMinutes(100).Consume();
        }

        [Benchmark]
        public void FromSeconds()
        {
            Duration.FromSeconds(100).Consume();
        }

        [Benchmark]
        public void FromMilliseconds()
        {
            Duration.FromMilliseconds(100).Consume();
        }

        [Benchmark]
        public void FromTicks()
        {
            Duration.FromTicks(100).Consume();
        }

        [Benchmark]
        public void FromInt64Nanoseconds()
        {
            Duration.FromNanoseconds(int.MaxValue + 1L).Consume();
        }

        [Benchmark]
        public void FromDecimalNanoseconds()
        {
            Duration.FromNanoseconds(long.MaxValue + 100M).Consume();
        }

        [Benchmark]
        public void FromTimeSpan()
        {
            Duration.FromTimeSpan(SampleTimeSpan).Consume();
        }

    }
}
