// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.ComponentModel;
using BenchmarkDotNet.Attributes;

namespace NodaTime.Benchmarks.BclTests
{
    [Config(typeof(BenchmarkConfig))]
    [Category("BCL")]
    internal class UtcDateTimeBenchmarks
    {
        private readonly DateTime sample = new DateTime(2009, 12, 26, 10, 8, 30, DateTimeKind.Utc);

        [Benchmark]
        public DateTime Construction()
        {
            return new DateTime(2009, 12, 26, 10, 8, 30, DateTimeKind.Utc);
        }

        [Benchmark]
        public int Year()
        {
            return sample.Year;
        }

        [Benchmark]
        public int Month()
        {
            return sample.Month;
        }

        [Benchmark]
        public int DayOfMonth()
        {
            return sample.Day;
        }

        [Benchmark]
        public DayOfWeek DayOfWeek()
        {
            return sample.DayOfWeek;
        }

        [Benchmark]
        public int DayOfYear()
        {
            return sample.DayOfYear;
        }

        [Benchmark]
        public int Hour()
        {
            return sample.Hour;
        }

        [Benchmark]
        public int Minute()
        {
            return sample.Minute;
        }

        [Benchmark]
        public int Second()
        {
            return sample.Second;
        }

        [Benchmark]
        public int Millisecond()
        {
            return sample.Millisecond;
        }

        [Benchmark]
        public DateTime ToLocalTime()
        {
            return sample.ToLocalTime();
        }
    }
}