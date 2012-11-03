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
using NodaTime.Text;

namespace NodaTime.Benchmarks
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