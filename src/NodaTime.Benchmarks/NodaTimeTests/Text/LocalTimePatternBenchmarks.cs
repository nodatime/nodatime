// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using NodaTime.Text;

namespace NodaTime.Benchmarks.NodaTimeTests.Text
{
    [Category("Text")]
    public class LocalTimePatternBenchmarks
    {
        private static readonly LocalTime SampleLocalTimeToSecond = new LocalTime(15, 12, 10);
        private static readonly LocalTime SampleLocalTimeToNanos = LocalTime.FromHourMinuteSecondNanosecond(15, 12, 10, 123456789L);
        private static readonly string SampleIsoFormattedTime = "15:12:10";
        private static readonly string SampleIsoFormattedTimeWithNanos = "15:12:10.123456789";

        [Benchmark]
        public void ExtendedIso_Format() =>
            LocalTimePattern.ExtendedIso.Format(SampleLocalTimeToNanos);

        [Benchmark]
        public void ExtendedIso_Parse() =>
            LocalTimePattern.ExtendedIso.Parse(SampleIsoFormattedTimeWithNanos);

        [Benchmark]
        public void IsoPatternToSecond_Format() =>
            LocalTimePattern.GeneralIso.Format(SampleLocalTimeToSecond);

        [Benchmark]
        public void IsoPatternToSecond_Parse() =>
            LocalTimePattern.GeneralIso.Parse(SampleIsoFormattedTime);
    }
}
