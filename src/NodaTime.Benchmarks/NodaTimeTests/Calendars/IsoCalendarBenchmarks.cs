// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NodaTime.Benchmarks.Framework;

namespace NodaTime.Benchmarks.NodaTimeTests.Calendars
{
    public class IsoCalendarBenchmarks
    {
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
    }
}
