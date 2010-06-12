#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using NodaTime.Benchmarks.Extensions;
using NodaTime.Benchmarks.Timing;
using NodaTime.TimeZones;

namespace NodaTime.Benchmarks
{
    internal class UtcZonedDateTimeBenchmarks
    {
        private readonly ZonedDateTime sample = new ZonedDateTime(2009, 12, 26, 10, 8, 30, DateTimeZones.Utc);

        [Benchmark]
        public void Construction()
        {
            new ZonedDateTime(2009, 12, 26, 10, 8, 30, DateTimeZones.Utc).Consume();
        }

        [Benchmark]
        public void Year()
        {
            sample.Year.Consume();
        }

        [Benchmark]
        public void Month()
        {
            sample.MonthOfYear.Consume();
        }

        [Benchmark]
        public void DayOfMonth()
        {
            sample.DayOfMonth.Consume();
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
            sample.HourOfDay.Consume();
        }

        [Benchmark]
        public void Minute()
        {
            sample.MinuteOfHour.Consume();
        }

        [Benchmark]
        public void Second()
        {
            sample.SecondOfMinute.Consume();
        }

        [Benchmark]
        public void Millisecond()
        {
            sample.MillisecondOfSecond.Consume();
        }
    }
}