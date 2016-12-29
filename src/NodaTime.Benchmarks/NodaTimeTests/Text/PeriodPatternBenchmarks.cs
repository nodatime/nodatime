// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using NodaTime.Text;

namespace NodaTime.Benchmarks.NodaTimeTests.Text
{
    [Category("Text")]
    public class PeriodPatternBenchmarks
    {
        private static readonly Period SamplePeriod = new PeriodBuilder
        {
            Years = 100, Months = 1, Days = 10, Hours = 12, Minutes = 35, Seconds = 12, Milliseconds = 123, Ticks = 4567
        }.Build();

        private static readonly string NormalizedText = PeriodPattern.NormalizingIso.Format(SamplePeriod);
        private static readonly string RoundtripText = PeriodPattern.Roundtrip.Format(SamplePeriod);

        [Benchmark]
        public void ParseNormalized()
        {
            PeriodPattern.NormalizingIso.Parse(NormalizedText).GetValueOrThrow();
        }

        [Benchmark]
        public void ParseRoundtrip()
        {
            PeriodPattern.Roundtrip.Parse(RoundtripText).GetValueOrThrow();
        }

        [Benchmark]
        public void FormatNormalized()
        {
            PeriodPattern.NormalizingIso.Format(SamplePeriod);
        }

        [Benchmark]
        public void FormatRoundtrip()
        {
            PeriodPattern.Roundtrip.Format(SamplePeriod);
        }
    }
}
