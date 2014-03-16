// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Benchmarks.Framework;

namespace NodaTime.Benchmarks.BclTests
{
    [Category("BCL")]
    internal class UtcDateTimeBenchmarks
    {
        private readonly DateTime sample = new DateTime(2009, 12, 26, 10, 8, 30, DateTimeKind.Utc);

        [Benchmark]
        public void Construction()
        {
            new DateTime(2009, 12, 26, 10, 8, 30, DateTimeKind.Utc).Consume();
        }

        [Benchmark]
        public void Year()
        {
            sample.Year.Consume();
        }

        [Benchmark]
        public void Month()
        {
            sample.Month.Consume();
        }

        [Benchmark]
        public void DayOfMonth()
        {
            sample.Day.Consume();
        }

        [Benchmark]
        public void DayOfWeek()
        {
            sample.DayOfWeek.Consume();
        }

        [Benchmark]
        public void DayOfYear()
        {
            sample.DayOfYear.Consume();
        }

        [Benchmark]
        public void Hour()
        {
            sample.Hour.Consume();
        }

        [Benchmark]
        public void Minute()
        {
            sample.Minute.Consume();
        }

        [Benchmark]
        public void Second()
        {
            sample.Second.Consume();
        }

        [Benchmark]
        public void Millisecond()
        {
            sample.Millisecond.Consume();
        }

        [Benchmark]
        public void ToLocalTime()
        {
            sample.ToLocalTime();
        }
    }
}