using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace NodaTime.Benchmarks
{
    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            Add(Job.LegacyJitX86.WithLaunchCount(2));
            Add(Job.LegacyJitX64.WithLaunchCount(2));
            Add(Job.RyuJitX64.WithLaunchCount(2));
        }
    }
}