// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using NodaTime.Text;

#if !V1_0 && !V1_1
namespace NodaTime.Benchmarks.NodaTimeTests.Text
{
    [Category("Text")]
    public class ZonedDateTimePatternBenchmarks
    {
        private static readonly DateTimeZone SampleZone = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
        private static readonly ZonedDateTime SampleZonedDateTime = new LocalDateTime(2009, 12, 26, 10, 8, 30).InZoneStrictly(SampleZone);
        private static readonly ZonedDateTimePattern PatternWithNumbersToSecond = ZonedDateTimePattern.CreateWithInvariantCulture("dd/MM/yyyy HH:mm:ss", null);

        [Benchmark]
        public void FormatWithNumbersToSecond()
        {
            PatternWithNumbersToSecond.Format(SampleZonedDateTime);
        }

        [Benchmark]
        public void FormatIso()
        {
            ZonedDateTimePattern.ExtendedFormatOnlyIso.Format(SampleZonedDateTime);
        }
    }
}
#endif
