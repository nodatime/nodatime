using System;
using System.Xml.Linq;

namespace NodaTime.Web.Storage
{
    /// <summary>
    /// A single result from a benchmark run. This involves calling a single method multiple
    /// times, and recording how long it took.
    /// </summary>
    /// <remarks>
    /// This is similar to BenchmarkResult in NodaTime.Benchmarks, but with full type
    /// and namespace information, and parsing from XElement.
    /// </remarks>
    public sealed class BenchmarkResult
    {
        private const long TicksPerPicosecond = 100 * 1000L;
        private const long TicksPerNanosecond = 100;

        private readonly string type;
        private readonly string clrNamespace;
        private readonly string method;
        private readonly int iterations;
        private readonly TimeSpan duration;
        private readonly string fullyQualifiedMethod;

        public string Type { get { return type; } }
        public string Namespace { get { return clrNamespace; } }
        public string Method { get { return method; } }
        public string FullyQualifiedMethod { get { return fullyQualifiedMethod; } }
        public int Iterations { get { return iterations; } }
        public TimeSpan Duration { get { return duration; } }
        public long CallsPerSecond { get { return iterations * TimeSpan.TicksPerSecond / duration.Ticks; } }
        public long NanosecondsPerCall { get { return (Duration.Ticks * TicksPerNanosecond) / iterations; } }

        private BenchmarkResult(string clrNamespace, string type, string method, int iterations, TimeSpan duration)
        {
            this.clrNamespace = clrNamespace;
            this.type = type;
            this.method = method;
            this.iterations = iterations;
            this.duration = duration;
            // Redundant, but means we only need to format once.
            this.fullyQualifiedMethod = string.Format("{0}.{1}.{2}", clrNamespace, type, method);
        }

        internal static BenchmarkResult FromXElement(XElement element)
        {
            XElement typeElement = element.Parent;
            return new BenchmarkResult(
                clrNamespace: typeElement.Attribute("namespace").Value,
                type: typeElement.Attribute("name").Value,
                method: element.Attribute("method").Value,
                iterations: (int) element.Attribute("iterations"),
                duration: TimeSpan.FromTicks((long) element.Attribute("duration")));
        }
    }
}
