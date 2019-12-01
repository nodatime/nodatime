// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text;
using BenchmarkDotNet.Attributes;
using NodaTime.Text;

namespace NodaTime.Benchmarks.NodaTimeTests.Text
{
    [Category("Text")]
    public class LocalDatePatternBenchmarks
    {
        private static readonly LocalDate SampleLocalDate = new LocalDate(2009, 12, 26);
        private static readonly string SampleIsoFormattedDate = "2009-12-26";
        private static readonly LocalDatePattern PatternWithLongMonth = LocalDatePattern.CreateWithInvariantCulture("MMMM dd uuuu");
        private static readonly LocalDatePattern PatternWithShortMonth = LocalDatePattern.CreateWithInvariantCulture("MMM dd uuuu");
        private static readonly LocalDatePattern PatternWithLongDay = LocalDatePattern.CreateWithInvariantCulture("dddd MM dd uuuu");
        private static readonly LocalDatePattern PatternWithShortDay = LocalDatePattern.CreateWithInvariantCulture("ddd MM dd uuuu");

        private static readonly StringBuilder builder = new StringBuilder();

        [Benchmark]
        public void FormatWithIso()
        {
            LocalDatePattern.Iso.Format(SampleLocalDate);
        }

#if !V1
        [Benchmark]
        public void AppendFormatWithIso()
        {
            builder.Clear();
            LocalDatePattern.Iso.AppendFormat(SampleLocalDate, builder);
        }
#endif

        [Benchmark]
        public void FormatWithLongMonth()
        {
            PatternWithLongMonth.Format(SampleLocalDate);
        }

        [Benchmark]
        public void FormatWithShortMonth()
        {
            PatternWithShortMonth.Format(SampleLocalDate);
        }

        [Benchmark]
        public void FormatWithLongDay()
        {
            PatternWithLongDay.Format(SampleLocalDate);
        }

        [Benchmark]
        public void FormatWithShortDay()
        {
            PatternWithShortDay.Format(SampleLocalDate);
        }

        [Benchmark]
        public void ParseIsoDate()
        {
            LocalDatePattern.Iso.Parse(SampleIsoFormattedDate);
        }
    }
}
