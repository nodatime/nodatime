// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Benchmarks.Timing;
using NodaTime.Globalization;
using NodaTime.Text;

namespace NodaTime.Benchmarks
{
    internal class InstantBenchmarks
    {
        private static readonly NodaFormatInfo InvariantFormatInfo = NodaFormatInfo.InvariantInfo;
        private static readonly Instant Sample = Instant.FromUtc(2011, 8, 24, 12, 29, 30);
        private static readonly IPattern<Instant> GeneralPattern =
            NodaFormatInfo.InvariantInfo.InstantPatternParser.ParsePattern("g");
        private static readonly IPattern<Instant> NumberPattern =
            NodaFormatInfo.InvariantInfo.InstantPatternParser.ParsePattern("n");

        [Benchmark]
        public void FormatN()
        {
            Sample.ToString("n", InvariantFormatInfo);
        }

        [Benchmark]
        public void FormatN_WithParsedPattern()
        {
            NumberPattern.Format(Sample);
        }

        [Benchmark]
        public void FormatG()
        {
            Sample.ToString("g", InvariantFormatInfo);
        }

        [Benchmark]
        public void FormatD()
        {
            Sample.ToString("d", InvariantFormatInfo);
        }

        [Benchmark]
        public void FormatG_WithParsedPattern()
        {
            GeneralPattern.Format(Sample);
        }
    }
}
