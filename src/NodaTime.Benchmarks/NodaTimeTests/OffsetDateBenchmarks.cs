// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using NodaTime.Calendars;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    public class OffsetDateBenchmarks
    {
        private static readonly Offset OneHourOffset = Offset.FromHours(1);
        private static readonly Offset LargePositiveOffset = Offset.FromHours(12);
        private static readonly LocalDate SampleLocal = new LocalDate(2009, 12, 26);
        private static readonly OffsetDate SampleEarlier = new OffsetDate(SampleLocal, -OneHourOffset);
        private static readonly OffsetDate Sample = new OffsetDate(SampleLocal, Offset.Zero);

        [Benchmark]
        public OffsetDate Construction() => new OffsetDate(SampleLocal, OneHourOffset);

        [Benchmark]
        public int Year() => Sample.Year;

        [Benchmark]
        public int Month() => Sample.Month;

        [Benchmark]
        public int DayOfMonth() => Sample.Day;

        [Benchmark]
        public IsoDayOfWeek DayOfWeek() => Sample.DayOfWeek;

        [Benchmark]
        public int DayOfYear() => Sample.DayOfYear;

        [Benchmark]
        public LocalDate Date() => Sample.Date;

        [Benchmark]
        public Era Era() => Sample.Era;

        [Benchmark]
        public int YearOfEra() => Sample.YearOfEra;

        // This just about stays within the same local day
        [Benchmark]
        public OffsetDate WithOffset() => SampleEarlier.WithOffset(LargePositiveOffset);
    }
}