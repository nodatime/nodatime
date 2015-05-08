// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;
using NodaTime.Calendars;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    [BenchmarkTask(platform: BenchmarkPlatform.X86, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, jitVersion: BenchmarkJitVersion.RyuJit)]
    internal class OffsetDateTimeBenchmarks
    {
        private static readonly Offset OneHourOffset = Offset.FromHours(1);
        private static readonly Offset LargePositiveOffset = Offset.FromHours(12);
        private static readonly Offset LargeNegativeOffset = Offset.FromHours(12);
        private static readonly LocalDateTime SampleLocal = new LocalDateTime(2009, 12, 26, 10, 8, 30);
        private static readonly OffsetDateTime SampleEarlier = new OffsetDateTime(SampleLocal, -OneHourOffset);
        private static readonly OffsetDateTime Sample = new OffsetDateTime(SampleLocal, Offset.Zero);
        private static readonly OffsetDateTime SampleLater = new OffsetDateTime(SampleLocal, OneHourOffset);

#if !V1_0
        private static readonly IComparer<OffsetDateTime> LocalComparer = OffsetDateTime.Comparer.Local;
        private static readonly IComparer<OffsetDateTime> InstantComparer = OffsetDateTime.Comparer.Instant;
#endif

        [Benchmark]
        public OffsetDateTime Construction()
        {
            return new OffsetDateTime(SampleLocal, OneHourOffset);
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
        public IsoDayOfWeek IsoDayOfWeek()
        {
            return Sample.IsoDayOfWeek;
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

#if !V1_0
        [Benchmark]
        public LocalDate Date()
        {
            return Sample.Date;
        }

        [Benchmark]
        public LocalTime TimeOfDay()
        {
            return Sample.TimeOfDay;
        }
#endif

        [Benchmark]
        public long TickOfDay()
        {
            return Sample.TickOfDay;
        }

        [Benchmark]
        public int TickOfSecond()
        {
            return Sample.TickOfSecond;
        }

        [Benchmark]
        public int WeekOfWeekYear()
        {
            return Sample.WeekOfWeekYear;
        }

        [Benchmark]
        public int WeekYear()
        {
            return Sample.WeekYear;
        }

        [Benchmark]
        public int ClockHourOfHalfDay()
        {
            return Sample.ClockHourOfHalfDay;
        }

        [Benchmark]
        public Era Era()
        {
            return Sample.Era;
        }

        [Benchmark]
        public int YearOfEra()
        {
            return Sample.YearOfEra;
        }

#if !V1_0
        [Benchmark]
        public int LocalComparer_Compare()
        {
            int value = LocalComparer.Compare(Sample, SampleEarlier);
            value += LocalComparer.Compare(Sample, Sample);
            value += LocalComparer.Compare(Sample, SampleLater);
            return value;
        }

        [Benchmark]
        public int InstantComparer_Compare()
        {
            int value = InstantComparer.Compare(Sample, SampleEarlier);
            value += InstantComparer.Compare(Sample, Sample);
            value += InstantComparer.Compare(Sample, SampleLater);
            return value;
        }
#endif

#if !V1_0 && !V1_1 && !V1_2
        [Benchmark]
        public OffsetDateTime WithOffset_SameLocalDay()
        {
            // This just about stays within the same local day
            return SampleEarlier.WithOffset(LargePositiveOffset);
        }

        [Benchmark]
        public OffsetDateTime WithOffset_DifferentLocalDay()
        {
            // This ends up in the day before
            return SampleEarlier.WithOffset(LargeNegativeOffset);
        }
#endif
    }
}