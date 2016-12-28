// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using NodaTime.Calendars;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    public class OffsetDateTimeBenchmarks
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
        public OffsetDateTime Construction() => new OffsetDateTime(SampleLocal, OneHourOffset);

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
        public int Hour() => Sample.Hour;

        [Benchmark]
        public int Minute() => Sample.Minute;

        [Benchmark]
        public int Second() => Sample.Second;

        [Benchmark]
        public int Millisecond() => Sample.Millisecond;

#if !V1_0
        [Benchmark]
        public LocalDate Date() => Sample.Date;

        [Benchmark]
        public LocalTime TimeOfDay() => Sample.TimeOfDay;
#endif

        [Benchmark]
        public long TickOfDay() => Sample.TickOfDay;

        [Benchmark]
        public int TickOfSecond() => Sample.TickOfSecond;

        [Benchmark]
        public int ClockHourOfHalfDay() => Sample.ClockHourOfHalfDay;

        [Benchmark]
        public Era Era() => Sample.Era;

        [Benchmark]
        public int YearOfEra() => Sample.YearOfEra;

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
        // This just about stays within the same local day
        [Benchmark]
        public OffsetDateTime WithOffset_SameLocalDay() => SampleEarlier.WithOffset(LargePositiveOffset);

        // This ends up in the day before
        [Benchmark]
        public OffsetDateTime WithOffset_DifferentLocalDay() => SampleEarlier.WithOffset(LargeNegativeOffset);
#endif
    }
}