// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Globalization;
using Minibench.Framework;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    internal class InstantBenchmarks
    {
        private static readonly Instant Sample = Instant.FromUtc(2011, 8, 24, 12, 29, 30);
        private static readonly Offset SmallOffset = Offset.FromHours(1);
        private static readonly Offset LargePositiveOffset = Offset.FromHours(12);
        private static readonly Offset LargeNegativeOffset = Offset.FromHours(-13);
        private static readonly DateTimeZone London = DateTimeZoneProviders.Tzdb["Europe/London"];

        [Benchmark]
        public void ToStringIso()
        {
            Sample.ToString("g", CultureInfo.InvariantCulture);
        }

        [Benchmark]
        public void PlusDuration()
        {
            Sample.Plus(Duration.Epsilon);
        }

#if !NO_INTERNALS
        [Benchmark]
        public void PlusOffset()
        {
            Sample.Plus(Offset.Zero);
        }
#endif

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

#if !V1_0 && !V1_1
        [Benchmark]
        public void WithOffset_SameUtcDay()
        {
            Sample.WithOffset(SmallOffset);
        }

        [Benchmark]
        public void WithOffset_NextUtcDay()
        {
            Sample.WithOffset(LargePositiveOffset);
        }

        [Benchmark]
        public void WithOffset_PreviousUtcDay()
        {
            Sample.WithOffset(LargeNegativeOffset);
        }
#endif
    }
}
