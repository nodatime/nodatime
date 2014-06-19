// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Benchmarks.Framework;
using NodaTime.Text;

namespace NodaTime.Benchmarks.NodaTimeTests.Text
{
    [Category("Text")]
    internal class LocalDatePatternBenchmarks
    {
        private static readonly LocalDate SampleLocalDate = new LocalDate(2009, 12, 26);
        private static readonly LocalDatePattern PatternWithLongMonth = LocalDatePattern.CreateWithInvariantCulture("MMMM dd yyyy");
        private static readonly LocalDatePattern PatternWithShortMonth = LocalDatePattern.CreateWithInvariantCulture("MMM dd yyyy");
        private static readonly LocalDatePattern PatternWithLongDay = LocalDatePattern.CreateWithInvariantCulture("dddd MM dd yyyy");
        private static readonly LocalDatePattern PatternWithShortDay = LocalDatePattern.CreateWithInvariantCulture("ddd MM dd yyyy");

        [Benchmark]
        public void FormatWithIso()
        {
            LocalDatePattern.IsoPattern.Format(SampleLocalDate);
        }

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
    }
}
