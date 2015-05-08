// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.ComponentModel;
using System.Globalization;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace NodaTime.Benchmarks.BclTests
{
    [BenchmarkTask(platform: BenchmarkPlatform.X86, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.RyuJit)]
    [Category("BCL")]
    public class DateTimeBenchmarks
    {
        private static readonly DateTime Sample = new DateTime(2009, 12, 26, 10, 8, 30, 234, DateTimeKind.Local);

        [Benchmark]
        public DateTime ConstructionToDay()
        {
            return new DateTime(2009, 12, 26);
        }

        [Benchmark]
        public DateTime ConstructionToSecond()
        {
            return new DateTime(2009, 12, 26, 10, 8, 30, DateTimeKind.Local);
        }

        [Benchmark]
        public DateTime ConstructionToMillisecond()
        {
            return new DateTime(2009, 12, 26, 10, 8, 30, 234, DateTimeKind.Local);
        }

        [Benchmark]
        public int Year()
        {
            return Sample.Year;
        }

        [Benchmark]
        public int Month()
        {
            return Sample.Month;
        }

        [Benchmark]
        public int DayOfMonth()
        {
            return Sample.Day;
        }

        [Benchmark]
        public DayOfWeek DayOfWeek()
        {
            return Sample.DayOfWeek;
        }

        [Benchmark]
        public int DayOfYear()
        {
            return Sample.DayOfYear;
        }

        [Benchmark]
        public int Hour()
        {
            return Sample.Hour;
        }

        [Benchmark]
        public int Minute()
        {
            return Sample.Minute;
        }

        [Benchmark]
        public int Second()
        {
            return Sample.Second;
        }

        [Benchmark]
        public int Millisecond()
        {
            return Sample.Millisecond;
        }

        [Benchmark]
        public DateTime ToUtc()
        {
            return Sample.ToUniversalTime();
        }

        [Benchmark]
        [Category("Text")]
        public string Format()
        {
            return Sample.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }

        [Benchmark]
        [Category("Text")]
        public bool TryParseExact()
        {
            DateTime result;
            return DateTime.TryParseExact("26/12/2009 10:08:30", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }
    }
}