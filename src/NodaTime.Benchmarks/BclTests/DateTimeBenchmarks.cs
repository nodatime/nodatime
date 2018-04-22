// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using BenchmarkDotNet.Attributes;

namespace NodaTime.Benchmarks.BclTests
{
    [Category("BCL")]
    public class DateTimeBenchmarks
    {
        private static readonly DateTime Sample = new DateTime(2009, 12, 26, 10, 8, 30, 234, DateTimeKind.Local);

        [Benchmark]
        public DateTime ConstructionToDay() => new DateTime(2009, 12, 26);

        [Benchmark]
        public DateTime ConstructionToSecond() => new DateTime(2009, 12, 26, 10, 8, 30, DateTimeKind.Local);

        [Benchmark]
        public DateTime ConstructionToMillisecond() => new DateTime(2009, 12, 26, 10, 8, 30, 234, DateTimeKind.Local);

        [Benchmark]
        public int Year() => Sample.Year;

        [Benchmark]
        public int Month() => Sample.Month;

        [Benchmark]
        public int DayOfMonth() => Sample.Day;

        [Benchmark]
        public DayOfWeek DayOfWeek() => Sample.DayOfWeek;

        [Benchmark]
        public int DayOfYear() => Sample.DayOfYear;

        [Benchmark]
        public int Hour() => Sample.Hour;

        [Benchmark]
        public int Minute() => Sample.Minute;

        [Benchmark]
        public int Second() => Sample.Second;

        [Benchmark]
        public int Millisecond() => Sample.Millisecond;

        [Benchmark]
        public DateTime ToUtc() => Sample.ToUniversalTime();

        [Benchmark]
        [Category("Text")]
        public string Format() => Sample.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

        [Benchmark]
        [Category("Text")]
        public bool TryParseExact()
        {
            return DateTime.TryParseExact("26/12/2009 10:08:30", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);
        }
    }
}