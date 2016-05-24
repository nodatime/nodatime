// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using BenchmarkDotNet.Attributes;
using NodaTime.Calendars;

namespace NodaTime.Benchmarks.NodaTimeTests.Calendars
{
    public class IsoCalendarBenchmarks
    {
#if !V1
        private static readonly YearMonthDayCalculator IsoCalculator = CalendarSystem.Iso.YearMonthDayCalculator;
        private static readonly YearMonthDay SampleOptimizedYearMonthDay = new YearMonthDay(2014, 6, 28);
        private static readonly YearMonthDay SampleNotOptimizedYearMonthDay = new YearMonthDay(1600, 6, 28);
#endif

        [Benchmark]
        public void GetDaysInMonth_NotFebruary()
        {
            CalendarSystem.Iso.GetDaysInMonth(2012, 1);
        }

        [Benchmark]
        public void GetDaysInMonth_FebruaryNonLeap()
        {
            CalendarSystem.Iso.GetDaysInMonth(2011, 2);
        }

        [Benchmark]
        public void GetDaysInMonth_FebruaryLeap()
        {
            CalendarSystem.Iso.GetDaysInMonth(2012, 2);
        }

#if !V1
        [Benchmark]
        public void GetDaysSinceEpoch_InOptimizedRange()
        {
            IsoCalculator.GetDaysSinceEpoch(SampleOptimizedYearMonthDay);
        }

        [Benchmark]
        public void GetDaysSinceEpoch_OutsideOptimizedRange()
        {
            IsoCalculator.GetDaysSinceEpoch(SampleNotOptimizedYearMonthDay);
        }

        [Benchmark]
        public void GetStartOfYearInDays_InOptimizedRange()
        {
            IsoCalculator.GetStartOfYearInDays(2000);
        }

        [Benchmark]
        public void GetStartOfYearInDays_OutsideOptimizedRange()
        {
            IsoCalculator.GetStartOfYearInDays(1600);
        }
#endif
    }
}
