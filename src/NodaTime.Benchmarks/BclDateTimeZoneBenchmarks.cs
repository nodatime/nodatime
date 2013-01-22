using NodaTime.Benchmarks.FrameworkComparisons;
using NodaTime.Benchmarks.Timing;
using NodaTime.TimeZones;

namespace NodaTime.Benchmarks
{
    internal sealed class BclDateTimeZoneBenchmarks
    {
        private static readonly DateTimeZone PacificZone = BclDateTimeZone.FromTimeZoneInfo(TimeZoneInfoBenchmarks.PacificZone);
        private static readonly Instant SummerInstant = Instant.FromDateTimeUtc(TimeZoneInfoBenchmarks.SummerUtc);
        private static readonly Instant WinterInstant = Instant.FromDateTimeUtc(TimeZoneInfoBenchmarks.WinterUtc);

        // This is somewhat unfair due to caching, admittedly...
        [Benchmark]
        public void GetZoneInterval()
        {
            PacificZone.GetZoneInterval(SummerInstant);
            PacificZone.GetZoneInterval(WinterInstant);
        }
    }
}
