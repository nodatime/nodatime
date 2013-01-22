using System;
using System.Globalization;
using NodaTime.Benchmarks.Extensions;
using NodaTime.Benchmarks.Timing;

namespace NodaTime.Benchmarks.FrameworkComparisons
{
    internal class DateTimeBenchmarks
    {
        private readonly DateTime sample = new DateTime(2009, 12, 26, 10, 8, 30, 234, DateTimeKind.Local);

        [Benchmark]
        public void Construction()
        {
            new DateTime(2009, 12, 26, 10, 8, 30, 234, DateTimeKind.Local).Consume();
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

        [Benchmark]
        public void Format()
        {
            sample.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
        
        [Benchmark]
        public void TryParseExact()
        {
            DateTime result;
            DateTime.TryParseExact("26/12/2009 10:08:30", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }
    }
}