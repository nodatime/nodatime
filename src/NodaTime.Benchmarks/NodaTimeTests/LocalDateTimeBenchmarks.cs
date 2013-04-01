// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Benchmarks.Framework;
using NodaTime.Text;

namespace NodaTime.Benchmarks.NodaTimeTests
{
    internal class LocalDateTimeBenchmarks
    {
        private static readonly LocalDateTime SampleLocalDateTime = new LocalDateTime(2009, 12, 26, 10, 8, 30);

        private static readonly LocalDateTimePattern Pattern = LocalDateTimePattern.CreateWithInvariantCulture("dd/MM/yyyy HH:mm:ss");

        [Benchmark]
        public void PatternFormat()
        {
            Pattern.Format(SampleLocalDateTime);
        }

        [Benchmark]
        public void PatternParse()
        {
            var parseResult = Pattern.Parse("26/12/2009 10:08:30");
            parseResult.Value.Consume();
        }

        [Benchmark]
        public void ConstructionToMinute()
        {
            new LocalDateTime(2009, 12, 26, 10, 8).Consume();
        }

        [Benchmark]
        public void ConstructionToSecond()
        {
            new LocalDateTime(2009, 12, 26, 10, 8, 30).Consume();
        }

        [Benchmark]
        public void ConstructionToTick()
        {
            new LocalDateTime(2009, 12, 26, 10, 8, 30, 0, 0).Consume();
        }

        [Benchmark]
        public void Year()
        {
            SampleLocalDateTime.Year.Consume();
        }

        [Benchmark]
        public void Month()
        {
            SampleLocalDateTime.Month.Consume();
        }

        [Benchmark]
        public void DayOfMonth()
        {
            SampleLocalDateTime.Day.Consume();
        }

        [Benchmark]
        public void IsoDayOfWeek()
        {
            SampleLocalDateTime.IsoDayOfWeek.Consume();
        }

        [Benchmark]
        public void DayOfYear()
        {
            SampleLocalDateTime.DayOfYear.Consume();
        }

        [Benchmark]
        public void Hour()
        {
            SampleLocalDateTime.Hour.Consume();
        }

        [Benchmark]
        public void Minute()
        {
            SampleLocalDateTime.Minute.Consume();
        }

        [Benchmark]
        public void Second()
        {
            SampleLocalDateTime.Second.Consume();
        }

        [Benchmark]
        public void Millisecond()
        {
            SampleLocalDateTime.Millisecond.Consume();
        }

        [Benchmark]
        public void Date()
        {
            SampleLocalDateTime.Date.Consume();
        }

        [Benchmark]
        public void TimeOfDay()
        {
            SampleLocalDateTime.TimeOfDay.Consume();
        }
    }
}