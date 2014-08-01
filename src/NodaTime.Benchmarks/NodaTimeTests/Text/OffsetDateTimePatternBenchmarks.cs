// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Benchmarks.Framework;
using NodaTime.Text;

namespace NodaTime.Benchmarks.NodaTimeTests.Text
{
    [Category("Text")]
    internal class OffsetDateTimePatternBenchmarks
    {
        private static readonly OffsetDateTime SampleOffsetDateTime = new LocalDateTime(2009, 12, 26, 10, 8, 30).WithOffset(Offset.FromHours(2));
        private static readonly OffsetDateTimePattern PatternWithLongMonthAndDay = OffsetDateTimePattern.CreateWithInvariantCulture("dddd MMMM dd yyyy HH:mm:ss o<G>");
        private static readonly OffsetDateTimePattern PatternWithNumbersToSecond = OffsetDateTimePattern.CreateWithInvariantCulture("dd/MM/yyyy HH:mm:ss o<G>");

        /// <summary>
        /// This includes both text and numeric parsing to avoid a test which is just a worst case for text.
        /// </summary>
        [Benchmark]
        public void ParseLongMonthAndDay()
        {
            PatternWithLongMonthAndDay.Parse("Friday April 12 2013 20:28:42 +02").GetValueOrThrow();
        }

        // Equivalent to DateTimeBenchmarks.TryParseExact
        [Benchmark]
        public void ParseWithNumbersToSecond()
        {
            PatternWithNumbersToSecond.Parse("26/12/2009 10:08:30 +02").GetValueOrThrow();
        }

        [Benchmark]
        public void FormatWithNumbersToSecond()
        {
            PatternWithNumbersToSecond.Format(SampleOffsetDateTime);
        }

        [Benchmark]
        public void FormatIso()
        {
            OffsetDateTimePattern.ExtendedIsoPattern.Format(SampleOffsetDateTime);
        }

        [Benchmark]
        public void ParseIso_NanosecondPrecision()
        {
            OffsetDateTimePattern.ExtendedIsoPattern.Parse("2014-08-01T13:46:12.123456789+02").GetValueOrThrow();
        }

        [Benchmark]
        public void ParseIso_SecondPrecision()
        {
            OffsetDateTimePattern.ExtendedIsoPattern.Parse("2014-08-01T13:46:12+02").GetValueOrThrow();
        }
    }
}
