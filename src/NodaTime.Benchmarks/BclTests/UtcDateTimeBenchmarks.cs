// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using BenchmarkDotNet.Attributes;

namespace NodaTime.Benchmarks.BclTests
{
    [Category("BCL")]
    public class UtcDateTimeBenchmarks
    {
        private readonly DateTime sample = new DateTime(2009, 12, 26, 10, 8, 30, DateTimeKind.Utc);

        [Benchmark]
        public DateTime Construction() => new DateTime(2009, 12, 26, 10, 8, 30, DateTimeKind.Utc);

        [Benchmark]
        public int Year() => sample.Year;

        [Benchmark]
        public int Month() => sample.Month;

        [Benchmark]
        public int DayOfMonth() => sample.Day;

        [Benchmark]
        public DayOfWeek DayOfWeek() => sample.DayOfWeek;

        [Benchmark]
        public int DayOfYear() => sample.DayOfYear;

        [Benchmark]
        public int Hour() => sample.Hour;

        [Benchmark]
        public int Minute() => sample.Minute;

        [Benchmark]
        public int Second() => sample.Second;

        [Benchmark]
        public int Millisecond() => sample.Millisecond;

        [Benchmark]
        public DateTime ToLocalTime() => sample.ToLocalTime();
    }
}