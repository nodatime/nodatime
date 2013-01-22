using NodaTime.Benchmarks.Extensions;
using NodaTime.Benchmarks.Timing;

namespace NodaTime.Benchmarks
{
    internal class PacificZonedDateTimeBenchmarks
    {
        private static readonly DateTimeZone Pacific = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
        private static readonly LocalDateTime SampleLocal = new LocalDateTime(2009, 12, 26, 10, 8, 30);
        private static readonly ZonedDateTime SampleZoned = Pacific.AtStrictly(SampleLocal);

        [Benchmark]
        public void Construction()
        {
            Pacific.AtStrictly(SampleLocal);
        }

        [Benchmark]
        public void Year()
        {
            SampleZoned.Year.Consume();
        }

        [Benchmark]
        public void Month()
        {
            SampleZoned.Month.Consume();
        }

        [Benchmark]
        public void DayOfMonth()
        {
            SampleZoned.Day.Consume();
        }

        [Benchmark]
        public void IsoDayOfWeek()
        {
            SampleZoned.IsoDayOfWeek.Consume();
        }

        [Benchmark]
        public void DayOfYear()
        {
            SampleZoned.DayOfYear.Consume();
        }

        [Benchmark]
        public void Hour()
        {
            SampleZoned.Hour.Consume();
        }

        [Benchmark]
        public void Minute()
        {
            SampleZoned.Minute.Consume();
        }

        [Benchmark]
        public void Second()
        {
            SampleZoned.Second.Consume();
        }

        [Benchmark]
        public void Millisecond()
        {
            SampleZoned.Millisecond.Consume();
        }

        [Benchmark]
        public void ToInstant()
        {
            SampleZoned.ToInstant();
        }
    }
}