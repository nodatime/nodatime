#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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

namespace NodaTime.Benchmarks
{
    internal class UtcZonedDateTimeBenchmarks
    {
        private static readonly LocalDateTime SampleLocal = new LocalDateTime(2009, 12, 26, 10, 8, 30);
        private static readonly ZonedDateTime Sample = DateTimeZone.Utc.AtStrictly(SampleLocal);

        [Benchmark]
        public void Construction()
        {
            DateTimeZone.Utc.AtStrictly(SampleLocal);
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
        public void IsoDayOfWeek()
        {
            Sample.IsoDayOfWeek.Consume();
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
    }
}