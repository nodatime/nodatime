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

using System;
using NodaTime.Benchmarks.Extensions;
using NodaTime.Benchmarks.Timing;

namespace NodaTime.Benchmarks.FrameworkComparisons
{
    internal class DateTimeBenchmarks
    {
        private readonly DateTime sample = new DateTime(2009, 12, 26, 10, 8, 30, DateTimeKind.Local);

        [Benchmark]
        public void Construction()
        {
            new DateTime(2009, 12, 26, 10, 8, 30, DateTimeKind.Local).Consume();
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
        public void ToUtc()
        {
            sample.ToUniversalTime();
        }
    }
}