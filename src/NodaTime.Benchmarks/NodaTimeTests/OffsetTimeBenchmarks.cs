// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    public class OffsetTimeBenchmarks
    {
        private static readonly Offset OneHourOffset = Offset.FromHours(1);
        private static readonly Offset LargePositiveOffset = Offset.FromHours(12);
        private static readonly LocalTime SampleLocal = new LocalTime(11, 23, 45, 56);
        private static readonly OffsetTime SampleEarlier = new OffsetTime(SampleLocal, -OneHourOffset);
        private static readonly OffsetTime Sample = new OffsetTime(SampleLocal, Offset.Zero);

        [Benchmark]
        public OffsetTime Construction() => new OffsetTime(SampleLocal, OneHourOffset);

        [Benchmark]
        public int Hour() => Sample.Hour;

        [Benchmark]
        public int Minute() => Sample.Minute;

        [Benchmark]
        public int Second() => Sample.Second;

        [Benchmark]
        public int NanosecondOfSecond() => Sample.NanosecondOfSecond;

        [Benchmark]
        public long NanosecondOfDay() => Sample.NanosecondOfDay;

        [Benchmark]
        public int Millisecond() => Sample.Millisecond;

        [Benchmark]
        public int TickOfSecond() => Sample.TickOfSecond;

        [Benchmark]
        public long TickOfDay() => Sample.TickOfDay;

        [Benchmark]
        public LocalTime TimeOfDay() => Sample.TimeOfDay;

        // This just about stays within the same local day
        [Benchmark]
        public OffsetTime WithOffset() => SampleEarlier.WithOffset(LargePositiveOffset);
    }
}