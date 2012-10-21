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
    internal class LocalTimeBenchmarks
    {
        private readonly LocalTime sample = new LocalTime(10, 8, 30, 300, 1234);
        private static readonly LocalDateTime LocalDateTime = new LocalDateTime(2011, 9, 14, 15, 10, 25);

        [Benchmark]
        public void ConstructionToMinute()
        {
            new LocalTime(15, 10).Consume();
        }

        [Benchmark]
        public void ConstructionToSecond()
        {
            new LocalTime(15, 10, 25).Consume();
        }

        [Benchmark]
        public void ConstructionToMillisecond()
        {
            new LocalTime(15, 10, 25, 500).Consume();
        }

        [Benchmark]
        public void ConstructionToTick()
        {
            new LocalTime(15, 10, 25, 500, 1234).Consume();
        }

        [Benchmark]
        public void ConversionFromLocalDateTime()
        {
            LocalDateTime.TimeOfDay.Consume();
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
        public void TickOfDay()
        {
            sample.TickOfDay.Consume();
        }
    }
}