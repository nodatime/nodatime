// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NodaTime.Benchmarks.Framework;
using NodaTime.Text;

namespace NodaTime.Benchmarks.NodaTimeTests.Text
{
    internal class LocalDateTimePatternBenchmarks
    {
        private static readonly LocalDateTimePattern PatternWithLongMonthAndDay = LocalDateTimePattern.CreateWithInvariantCulture("dddd MMMM dd yyyy HH:mm:ss");

        /// <summary>
        /// This includes both text and numeric parsing to avoid a test which is just a worst case for text.
        /// </summary>
        [Benchmark]
        public void ParseLongMonthAndDay()
        {
            PatternWithLongMonthAndDay.Parse("Friday April 12 2013 20:28:42").GetValueOrThrow();
        }
    }
}
