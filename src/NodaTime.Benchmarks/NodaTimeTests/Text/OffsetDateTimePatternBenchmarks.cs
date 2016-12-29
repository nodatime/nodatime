// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using System.Globalization;
using BenchmarkDotNet.Attributes;

#if !V1_0 && !V1_1

namespace NodaTime.Benchmarks.NodaTimeTests.Text
{
    [Category("Text")]
    public class OffsetDateTimePatternBenchmarks
    {
        private static readonly OffsetDateTime TemplateValue = new LocalDateTime(2000, 1, 1, 0, 0).WithOffset(Offset.Zero);
        private static readonly OffsetDateTime SampleOffsetDateTime = new LocalDateTime(2009, 12, 26, 10, 8, 30).WithOffset(Offset.FromHours(2));
        // Note: not using CreateWithInvariantCulture for backward compatibility reasons.
        private static readonly OffsetDateTimePattern PatternWithLongMonthAndDay = OffsetDateTimePattern.Create("dddd MMMM dd yyyy HH:mm:ss o<G>", CultureInfo.InvariantCulture, TemplateValue);
        private static readonly OffsetDateTimePattern PatternWithNumbersToSecond = OffsetDateTimePattern.Create("dd/MM/yyyy HH:mm:ss o<G>", CultureInfo.InvariantCulture, TemplateValue);

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
            OffsetDateTimePattern.ExtendedIso.Format(SampleOffsetDateTime);
        }

#if !V1
        [Benchmark]
        public void ParseIso_NanosecondPrecision()
        {
            OffsetDateTimePattern.ExtendedIso.Parse("2014-08-01T13:46:12.123456789+02").GetValueOrThrow();
        }
#endif

        [Benchmark]
        public void ParseIso_SecondPrecision()
        {
            OffsetDateTimePattern.ExtendedIso.Parse("2014-08-01T13:46:12+02").GetValueOrThrow();
        }
    }
}
#endif
