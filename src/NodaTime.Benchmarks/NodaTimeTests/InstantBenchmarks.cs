// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Benchmarks.Framework;
using System.Globalization;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    internal class InstantBenchmarks
    {
        private static readonly Instant Sample = Instant.FromUtc(2011, 8, 24, 12, 29, 30);
        private static readonly DateTimeZone London = DateTimeZoneProviders.Tzdb["Europe/London"];

        [Benchmark]
        public void ToStringIso()
        {
            Sample.ToString("g", CultureInfo.InvariantCulture);
        }

        [Benchmark]
        public void ToStringNumericWithThousandsSeparator()
        {
            Sample.ToString("n", CultureInfo.InvariantCulture);
        }

        [Benchmark]
        public void ToStringNumericWithoutThousandsSeparator()
        {
            Sample.ToString("d", CultureInfo.InvariantCulture);
        }

        [Benchmark]
        public void PlusDuration()
        {
            Sample.Plus(Duration.Epsilon);
        }

        [Benchmark]
        public void PlusOffset()
        {
            Sample.Plus(Offset.Zero);
        }

        [Benchmark]
        public void InUtc()
        {
            Sample.InUtc();
        }

        [Benchmark]
        public void InZoneLondon()
        {
            Sample.InZone(London);
        }
    }
}
