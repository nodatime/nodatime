// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using NodaTime.Benchmarks.Framework;

namespace NodaTime.Benchmarks.BclTests
{
    internal class DateTimeBenchmarks
    {
        private static readonly DateTime Sample = new DateTime(2009, 12, 26, 10, 8, 30, 234, DateTimeKind.Local);

        [Benchmark]
        public void Construction()
        {
            new DateTime(2009, 12, 26, 10, 8, 30, 234, DateTimeKind.Local).Consume();
        }

        [Benchmark]
        public void Year()
        {
            Sample.Year.Consume();
        }

        [Benchmark]
        public void Month()
        {
            Sample.Month.Consume();
        }

        [Benchmark]
        public void DayOfMonth()
        {
            Sample.Day.Consume();
        }

        [Benchmark]
        public void DayOfWeek()
        {
            Sample.DayOfWeek.Consume();
        }

        [Benchmark]
        public void DayOfYear()
        {
            Sample.DayOfYear.Consume();
        }

        [Benchmark]
        public void Hour()
        {
            Sample.Hour.Consume();
        }

        [Benchmark]
        public void Minute()
        {
            Sample.Minute.Consume();
        }

        [Benchmark]
        public void Second()
        {
            Sample.Second.Consume();
        }

        [Benchmark]
        public void Millisecond()
        {
            Sample.Millisecond.Consume();
        }

        [Benchmark]
        public void ToUtc()
        {
            Sample.ToUniversalTime();
        }

        [Benchmark]
        public void Format()
        {
            Sample.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
        
        [Benchmark]
        public void TryParseExact()
        {
            DateTime result;
            DateTime.TryParseExact("26/12/2009 10:08:30", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }
    }
}