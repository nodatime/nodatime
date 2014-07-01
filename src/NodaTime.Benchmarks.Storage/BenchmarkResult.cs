using System;
using System.Xml.Linq;
using BenchmarkResultProto = NodaTime.Benchmarks.Storage.Proto.BenchmarkResult;

namespace NodaTime.Benchmarks.Storage
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

        private readonly string method;
        private readonly int iterations;
        private readonly TimeSpan duration;
        private readonly string fullyQualifiedMethod;

        public string Method { get { return method; } }
        public string FullyQualifiedMethod { get { return fullyQualifiedMethod; } }
        public int Iterations { get { return iterations; } }
        public TimeSpan Duration { get { return duration; } }
        public long CallsPerSecond { get { return iterations * TimeSpan.TicksPerSecond / duration.Ticks; } }
        public long NanosecondsPerCall { get { return (Duration.Ticks * TicksPerNanosecond) / iterations; } }
        public long PicosecondsPerCall { get { return (Duration.Ticks * TicksPerPicosecond) / iterations; } }

        private BenchmarkResult(string qualifiedType, string method, int iterations, TimeSpan duration)
        {
            this.method = method;
            this.iterations = iterations;
            this.duration = duration;
            this.fullyQualifiedMethod = string.Format("{0}.{1}", qualifiedType, method);
        }

        public static BenchmarkResult FromXElement(string qualifiedType, XElement element)
        {
            return new BenchmarkResult(
                qualifiedType: qualifiedType,
                method: element.Attribute("method").Value,
                iterations: (int) element.Attribute("iterations"),
                duration: TimeSpan.FromTicks((long) element.Attribute("duration")));
        }

        public static BenchmarkResult FromProto(string qualifiedType, BenchmarkResultProto proto)
        {
            return new BenchmarkResult(qualifiedType, 
                proto.Method, proto.Iterations, new TimeSpan(proto.DurationTicks));
        }

        public BenchmarkResultProto ToProto()
        {
            return new BenchmarkResultProto.Builder
            {
                Method = method,
                Iterations = iterations,
                DurationTicks = duration.Ticks
            }.Build();
        }
    }
}
