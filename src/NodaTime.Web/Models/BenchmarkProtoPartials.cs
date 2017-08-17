namespace NodaTime.Benchmarks
{
    // Partial classes to allow backlinks from child elements to parents.
    public partial class Benchmark
    {
        public BenchmarkType Type { get; set; }
        public BenchmarkRun Run => Type.Run;
        public BenchmarkEnvironment Environment => Run.Environment;
    }

    public partial class BenchmarkType
    {
        public BenchmarkRun Run { get; set; }
        public BenchmarkEnvironment Environment => Run.Environment;

        internal void PopulateLinks()
        {
            foreach (var benchmark in Benchmarks)
            {
                benchmark.Type = this;
            }
        }
    }

    public partial class BenchmarkRun
    {
        public BenchmarkEnvironment Environment { get; set; }

        /// <summary>
        /// Populate links from type to run, and benchmark to type
        /// </summary>
        internal void PopulateLinks()
        {
            foreach (var type in Types_)
            {
                type.Run = this;
                type.PopulateLinks();
            }
        }
    }

    public partial class BenchmarkEnvironment
    {
        public string BriefOperatingSystem =>
            OperatingSystem.StartsWith("Ubuntu") || OperatingSystem.Contains("Linux") ? "Linux"
            : OperatingSystem.Contains("Microsoft Windows") ? "Windows"
            : OperatingSystem;
    }
}
